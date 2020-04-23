using BuildIt.Osm;

namespace BuildIt.Builds
{
    public class Canal : OsmTrackCollection
    {
        public Canal(double lat, double lon) : base(lat, lon)
        {

        }

        public override void PostProcess()
        {
        }

        protected override Node CreateNode(Track track, Segment segment, Osm.Node node, int x, int y)
        {
            return new Node(track, segment, node)
            {
                MapX = x,
                MapY = y,
                Elevation = -20,
                Width = 80
            };
        }

        protected override string Query => Overpass.Canal;

        protected override Tracktype Tracktype => Tracktype.Water;
    }
}
