using BuildIt.Osm;

namespace BuildIt.Builds
{
    public class River : OsmTrackCollection
    {        

        public River(double lat, double lon) : base(lat, lon)
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
                Elevation = -20
            };
        }

        protected override string Query => Overpass.River;

        protected override Tracktype Tracktype => Tracktype.Water;
    }
}
