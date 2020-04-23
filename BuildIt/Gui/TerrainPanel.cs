using BuildIt.Builds;
using ColossalFramework.UI;
using System.IO;
using UnityEngine;

namespace BuildIt.Gui
{
    public class TerrainPanel : UIPanel
    {
        private UIButton buildButton;
        private UIButton dumpButton;
        //private UIButton testButton;

        // Bunde, Limburg NL
        private const double lat = 50.90111;

        private const double lon = 5.73407;

        public override void OnDestroy()
        {
            Destroy(buildButton);
            Destroy(dumpButton);
            //Destroy(testButton);
        }

        public override void Start()
        {
            buildButton = UIHelper.MakeButton(this, "Build It", 10, 210, 135);
            buildButton.eventClick += BuildButtonClick;

            dumpButton = UIHelper.MakeButton(this, "Dump info", 145, 210, 135);
            dumpButton.eventClick += DumpButtonClick;

            //testButton = UIHelper.MakeButton(this, "Highways", 225, 210, 50);
            //testButton.eventClick += HighwaysButtonClick;
        }

        private void BuildButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            Debug.Log("Starting build");

            // generate Airplane line, north / south
            Airline airline = new Airline(lat, lon);
            airline.Build();
            foreach (Track track in airline.Tracks)
            {
                foreach (Node node in track.Nodes)
                {
                    //BuildTool.instance.ActionQueue.Enqueue(node);
                }
            }
            Debug.Log("Airline generated");

            River river = new River(lat, lon);
            int unresolved = river.Build();

            Debug.Log($"OSM rivers fetched, {river.Tracks.Count} tracks,  {unresolved} unresolved segments!");
            
            foreach (Track track in river.Tracks)
            {
                foreach (Node node in track.Nodes)
                {
                    Vector3 pos = new Vector3(node.MapX, 0, node.MapY);
                    node.TerrainHeight = TerrainManager.instance.SampleRawHeightSmooth(pos);
                    BuildTool.instance.ActionQueue.Enqueue(node);
                }
            }

            Highway highway = new Highway(lat, lon);
            unresolved = highway.Build();

            Debug.Log($"OSM highways fetched, {highway.Tracks.Count} tracks, {unresolved} unresolved segments!");

            foreach (Track track in highway.Tracks)
            {
                foreach (Node node in track.Nodes)
                {
                    BuildTool.instance.ActionQueue.Enqueue(node);
                }
            }
        }

        private void DumpButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            using (StreamWriter writer = new StreamWriter(@"D:\Downloads\prefabs.txt", false))
            {
                for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
                {
                    writer.WriteLine($"nr: {i} => {PrefabCollection<NetInfo>.GetLoaded(i).name}");
                }
            }
        }

        //private void HighwaysButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        //{
        //}
    }
}