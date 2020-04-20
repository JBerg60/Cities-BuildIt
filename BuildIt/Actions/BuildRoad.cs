using ColossalFramework.Math;
using System;
using System.Collections;
using UnityEngine;

namespace BuildIt.Actions
{
    public class BuildRoad
    {
        const int halfsquare = 1920 / 2;
        const int segmentLength = 96;

        NetInfo netInfo;
        Vector3 startDirection;
        Vector3 startPos;
        Vector3 endPos;

        protected Randomizer rand;

        ushort startNetNodeId;
        ushort endNetNodeId;
        public void Build()
        {
            // add a road for testing, straight line, following terrain heigt
            netInfo = PrefabCollection<NetInfo>.FindLoaded("Highway");

            startPos = new Vector3(-halfsquare, 0, 0);
            CreateNode(out startNetNodeId, ref rand, netInfo, startPos, 0);
            for (int x = -halfsquare + segmentLength; x < halfsquare; x += segmentLength)
            {
                endPos = new Vector3(x, 0, 0);
                CreateNode(out endNetNodeId, ref rand, netInfo, endPos, 0);

                startDirection = VectorUtils.NormalizeXZ(endPos - startPos);
                SimulationManager.instance.AddAction(AddRoad(rand, netInfo, startNetNodeId, endNetNodeId, startDirection, "JBerg street"));

                startNetNodeId = endNetNodeId;
                startPos = endPos;
            }
        }

        private void CreateNode(out ushort node, ref Randomizer rand, NetInfo netInfo, Vector3 pos, float elevation)
        {
            pos.y = TerrainManager.instance.SampleRawHeightSmoothWithWater(pos, false, 0f);
            NetManager.instance.CreateNode(out node, ref rand, netInfo, pos, SimulationManager.instance.m_currentBuildIndex);
            SimulationManager.instance.m_currentBuildIndex += 1u;
        }

        private IEnumerator AddRoad(Randomizer rand, NetInfo netInfo, ushort startNode, ushort endNode, Vector3 startDirection, string streetName)
        {
            ushort segmentId;
            try
            {
                if (NetManager.instance.CreateSegment(out segmentId, ref rand, netInfo, startNode, endNode, startDirection, -startDirection, 
                    SimulationManager.instance.m_currentBuildIndex, SimulationManager.instance.m_currentBuildIndex, false))
                {
                    SimulationManager.instance.m_currentBuildIndex += 2u;
                    NetManager.instance.SetSegmentNameImpl(segmentId, streetName);
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
