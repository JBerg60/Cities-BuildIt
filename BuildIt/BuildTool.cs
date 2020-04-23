using BuildIt.Builds;
using ColossalFramework.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuildIt
{
    public class BuildTool : ToolBase
    {
        private static readonly object lockObj = new System.Object();

        public static BuildTool instance;
        private ToolBase prevTool;

        private NetInfo netInfo;
        private ushort startNetNodeId;
        private ushort endNetNodeId;
        private Vector3 startPos;
        private Vector3 endPos;

        private long frames = 0;
        private int trackId = -1;
        private List<Node> track;

        private StringBuilder builderLog = new StringBuilder();

        public Queue<Node> ActionQueue { get; } = new Queue<Node>();

        protected Node currentNode;
        protected Node nextNode;

        protected Randomizer rand = SimulationManager.instance.m_randomizer;

        private const int terrainSize = TerrainManager.RAW_RESOLUTION + 1;
        private const float terrainTransform = terrainSize / (1920f * 9);

        protected override void Awake()
        {
            m_toolController = FindObjectOfType<ToolController>();
            enabled = false;
        }

        protected override void OnEnable()
        {
            prevTool = m_toolController.CurrentTool;
            m_toolController.CurrentTool = this;
            frames = 0;
            trackId = -1;
            builderLog = new StringBuilder();
        }

        protected override void OnDisable()
        {
            if (m_toolController.NextTool == null && prevTool != null && prevTool != this)
            {
                prevTool.enabled = true;
            }
            prevTool = null;
            File.WriteAllText(@"D:\Downloads\builder.txt", builderLog.ToString());
        }

        protected override void OnDestroy()
        {
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (currentNode == null)
                return;

            ToolManager.instance.m_drawCallData.m_overlayCalls++;
            OverlayEffect OverlayEffect = RenderManager.instance.OverlayEffect;

            // draw the cursor, that eats the remaining track
            OverlayEffect.DrawCircle(cameraInfo, Color.yellow, new Vector3(currentNode.MapX, 0, currentNode.MapY), 80f, -1f, 1025f, false, true);

            // draw the remaining track
            for (int i = 0; i < track.Count - 1; i++)
            {
                Bezier3 bezier = new Bezier3();
                // start point
                bezier.a = new Vector3(track[i].MapX, 0, track[i].MapY);
                // end point
                bezier.d = new Vector3(track[i + 1].MapX, 0, track[i + 1].MapY);

                Vector3 startDir = VectorUtils.NormalizeXZ(bezier.d - bezier.a);
                Vector3 endDir = VectorUtils.NormalizeXZ(bezier.a - bezier.d);

                NetSegment.CalculateMiddlePoints(bezier.a, startDir, bezier.d, endDir, true, true, out bezier.b, out bezier.c);

                OverlayEffect.DrawBezier(cameraInfo, Color.white, bezier, 8f, 100000f, -100000f, -1f, 1280f, false, true);
            }
        }

        public override void SimulationStep()
        {
            lock (lockObj)
            {
                frames++;
                if ((frames % 2 == 0) && !SimulationManager.instance.SimulationPaused)
                {
                    if (ActionQueue.Count > 0)
                    {
                        currentNode = ActionQueue.Dequeue();
                       
                        // make the complete track available for drawing
                        if (currentNode.Track.Id != trackId)
                        {
                            // make a copy of the track, for visualisation
                            // in the ongoing process, we will delete nodes
                            track = ActionQueue.ToArray().Where(a => a.Track.Id == currentNode.Track.Id).ToList();
                            trackId = currentNode.Track.Id;

                            builderLog.AppendLine($"Change track to {currentNode.Track.Name} ({trackId}), {track.Count} nodes");

                            startPos = new Vector3(currentNode.MapX, 0, currentNode.MapY);
                            startPos.y = TerrainManager.instance.SampleRawHeightSmooth(startPos)
                                + currentNode.Elevation;

                            // we do not need nodes for water, as we modify the terrain
                            if (currentNode.Track.Type != Tracktype.Water)
                            {
                                // collect data to build
                                netInfo = PrefabCollection<NetInfo>.FindLoaded(currentNode.Prefab);
                                if (netInfo == null)
                                    netInfo = PrefabCollection<NetInfo>.FindLoaded("Highway");

                                CreateNode(out startNetNodeId, netInfo, startPos);
                            }
                        }
                        else
                        {
                            endPos = new Vector3(currentNode.MapX, 0, currentNode.MapY);
                            endPos.y = TerrainManager.instance.SampleRawHeightSmooth(startPos)
                                + currentNode.Elevation;

                            if (currentNode.Track.Type == Tracktype.Water)
                            {
                                ModifyTerrain(startPos, endPos, currentNode);
                            }
                            else
                            {
                                // use the NetManager to add segments with a prefab
                                // collect data to build
                                netInfo = PrefabCollection<NetInfo>.FindLoaded(currentNode.Prefab);
                                if (netInfo == null)
                                    netInfo = PrefabCollection<NetInfo>.FindLoaded("Highway");

                                CreateNode(out endNetNodeId, netInfo, endPos);

                                Vector3 startDirection = VectorUtils.NormalizeXZ(endPos - startPos);
                                SimulationManager.instance.AddAction(AddRoad(rand, netInfo, startNetNodeId, endNetNodeId, startDirection, $"{currentNode.Name}"));

                                builderLog.AppendLine($"Prefab:{netInfo.name}, Name: {currentNode.Name} @ {startPos.x:###0}:{startPos.z:###0}:{startPos.y:###0} to {endPos.x:###0}:{endPos.z:###0}:{endPos.y:###0}");

                                startNetNodeId = endNetNodeId;
                            }
                            startPos = endPos;
                        }

                        // as roads are drawn, we do not need the track
                        track.Remove(currentNode);
                    }
                    else
                    {
                        currentNode = null;
                    }
                }
            }
        }

        private void ModifyTerrain(Vector3 startPos, Vector3 endPos, Node currentNode)
        {
            // for watecap = 20 we need radius 100 and depth 50 (= very large river)
            const int radius = 80; // width is twice the radius

            int dist = (int)Vector3.Distance(startPos, endPos);

            // take dist / 5 steps between the 2 points
            int steps = (int)Math.Max(1, dist / 5f);

            builderLog.AppendLine($"Draw {currentNode.Name} from {startPos.x}, {startPos.z} to {endPos.x}, {endPos.z} @ height {currentNode.TerrainHeight} to elevation {currentNode.Elevation} distance {dist} steps: {steps}");            

            Vector3 dirVector = endPos - startPos;

            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minZ = int.MaxValue;
            int maxZ = int.MinValue;

            Vector3 between;
            // now apply the brush
            for (int s = 0; s < steps; s++)
            {
                between = startPos + (s * (1f / steps) * dirVector);                
                builderLog.AppendLine($"Apply brush at {between.x}, {between.z} => terrainHeight {currentNode.TerrainHeight} with elevation {currentNode.Elevation}");

                for (int z = -radius; z <= radius; z += 10)
                {
                    for (int x = -radius; x <= radius; x += 10)
                    {
                        int xpos = (int)((between.x + x + (1920 * 9 / 2)) * terrainTransform);
                        int zpos = (int)((between.z + z + (1920 * 9 / 2)) * terrainTransform);
                        if (xpos >= 0 && zpos >= 0 && xpos < terrainSize && zpos < terrainSize)
                        {
                            ushort rawHeight = TerrainManager.instance.RawHeights[zpos * terrainSize + xpos];
                            ushort newHeight = BrushToRawHeight(radius, x, z, currentNode.TerrainHeight, currentNode.Elevation);

                            // prevent that the circle brush highten the terrain
                            if (newHeight < rawHeight)
                            {
                                builderLog.AppendLine($"Rawheight @ {xpos}, {zpos} was {rawHeight}, set to {newHeight}");
                                TerrainManager.instance.RawHeights[zpos * terrainSize + xpos] = newHeight;
                            }
                        }

                        if (xpos < minX) minX = xpos;
                        if (xpos > maxX) maxX = xpos;
                        if (zpos < minZ) minZ = zpos;
                        if (zpos > maxZ) maxZ = zpos;
                    }
                }
            }

            builderLog.AppendLine($"Update terrain area {minX}, {minZ} to {maxX}, {maxZ}");
            TerrainModify.UpdateArea(minX, minZ, maxX, maxZ, true, false, false);
        }

        private ushort BrushToRawHeight(int radius, int x, int z, float terrainHeight, int elevation)
        {
            const int scale = 65536 / 1024;

            // distance is sqrt 2 longer then radius
            double dist = Math.Sqrt((double)(x * x + z * z));

            double perc;
            if (dist > radius)
            {
                // ignore every point outside the radius, making the brush a circle
                perc = 0;
            }
            else
            {
                perc = Math.Max(radius - dist, 0.1f) / radius;
                // add 10%, to get a solid brush in the middle
                perc = Math.Min(1, perc + 0.1f);
            }


            double result = terrainHeight + (perc * elevation);
            
            if (result < 0) result = 0;
            if (result > 1023) result = 1023;
            return (ushort)(scale * result);
        }

        private void CreateNode(out ushort node, NetInfo netInfo, Vector3 pos)
        {
            NetManager.instance.CreateNode(out node, ref rand, netInfo, pos, SimulationManager.instance.m_currentBuildIndex);
            SimulationManager.instance.m_currentBuildIndex += 1u;
        }

        private IEnumerator AddRoad(Randomizer rand, NetInfo netInfo, ushort startNode, ushort endNode, Vector3 startDirection, string segmenttName)
        {
            ushort segmentId;
            try
            {
                if (NetManager.instance.CreateSegment(out segmentId, ref rand, netInfo, startNode, endNode, startDirection, -startDirection,
                    SimulationManager.instance.m_currentBuildIndex, SimulationManager.instance.m_currentBuildIndex, false))
                {
                    SimulationManager.instance.m_currentBuildIndex += 2u;
                    if (!string.IsNullOrEmpty(segmenttName))
                        NetManager.instance.SetSegmentNameImpl(segmentId, segmenttName);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
            yield return null;
        }
    }
}