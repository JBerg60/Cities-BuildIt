using BuildIt.Geo;
using ColossalFramework.Math;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BuildIt
{
    public class BuildTool : ToolBase
    {
        private static readonly Object lockObj = new Object();

        public static BuildTool instance;
        private ToolBase prevTool;

        private long frames = 0;
        private int trackId = -1;
        private List<Node> track;

        public Queue<Node> ActionQueue { get; } = new Queue<Node>();

        protected Node nextNode;

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
        }

        protected override void OnDisable()
        {
            if (m_toolController.NextTool == null && prevTool != null && prevTool != this)
            {
                prevTool.enabled = true;
            }
            prevTool = null;
        }

        protected override void OnDestroy()
        {
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (nextNode == null)
                return;

            ToolManager.instance.m_drawCallData.m_overlayCalls++;
            OverlayEffect OverlayEffect = RenderManager.instance.OverlayEffect;

            OverlayEffect.DrawCircle(cameraInfo, Color.yellow, new Vector3(nextNode.MapX, 0, nextNode.MapY), 40f, -1f, 1025f, false, true);

            for (int i = 1; i < track.Count; i++)
            {
                Bezier3 bezier = new Bezier3();
                // start point
                bezier.a = new Vector3(track[i - 1].MapX, 0, track[i - 1].MapY);
                // end point
                bezier.d = new Vector3(track[i].MapX, 0, track[i].MapY);

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
                if ((frames % 8 == 0) && !SimulationManager.instance.SimulationPaused)
                {
                    if (ActionQueue.Count > 0)
                    {
                        nextNode = ActionQueue.Dequeue();

                        // make the complete track available for drawing
                        if (nextNode.TrackId != trackId)
                        {
                            track = ActionQueue.ToArray().Where(a => a.TrackId == nextNode.TrackId).ToList();
                            trackId = nextNode.TrackId;
                        }
                    }
                    else
                    {
                        nextNode = null;
                    }
                }
            }
        }
    }
}