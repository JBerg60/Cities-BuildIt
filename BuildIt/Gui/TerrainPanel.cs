using BuildIt.Geo;
using ColossalFramework.UI;
using UnityEngine;

namespace BuildIt.Gui
{
    public class TerrainPanel : UIPanel
    {
        private UIButton okButton;
        private UIButton flatButton;
        private UIButton testButton;

        // Bunde, Limburg NL
        const double lat = 50.90111;
        const double lon = 5.73407;

        public override void OnDestroy()
        {
            Destroy(okButton);
            Destroy(flatButton);
            Destroy(testButton);
        }

        public override void Start()
        {
            okButton = UIHelper.MakeButton(this, "Generate Terrain", 10, 210, 135);
            okButton.eventClick += OkButtonClick;

            flatButton = UIHelper.MakeButton(this, "Flatten", 155, 210, 70);
            flatButton.eventClick += FlattenButtonClick;

            testButton = UIHelper.MakeButton(this, "Highways", 225, 210, 50);
            testButton.eventClick += HighwaysButtonClick;
        }

        private void OkButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            //Model.TerrainTool.instance.Generate((int)Mathf.Pow(2.0f, smoothness), scale, offset, (int)Mathf.Pow(2.0f, blur) + 1);
        }

        private void FlattenButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            //Model.TerrainTool.instance.Flatten();
        }

        private void HighwaysButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            GeoData geoData = new GeoData(lat, lon);
            int unresolved = geoData.GetHighways();

            Debug.Log($"OSM highways fetched, {unresolved} segments ignored!");

            foreach(Track track in geoData.Tracks)
            {
                foreach (Node node in track.Nodes)
                {
                    BuildTool.instance.ActionQueue.Enqueue(node);
                }
            }
        }
    }
}
