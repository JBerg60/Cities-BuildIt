using BuildIt.Osm;

namespace BuildIt.Builds
{
    public class Highway : OsmTrackCollection
    {
        public Highway(double lat, double lon) : base(lat, lon)
        {

        }

        protected override string Query => Overpass.Highway;

        protected override Tracktype Tracktype => Tracktype.Road;

        public override void PostProcess()
        {
            foreach (Track track in Tracks)
            {
                // loop over track backwards
                for (int i = track.Nodes.Count - 2; i > 0; i--)
                {
                    Node node = track.Nodes[i];
                    Node nextNode = track.Nodes[i + 1];

                    // leave bridged and tunnels intact
                    if (node.Segment.IsBridge || node.Segment.IsTunnel)
                        continue;

                    // flat node that has same position as the next 
                    if (node.MapX == nextNode.MapX && node.MapY == nextNode.MapY)
                        track.Nodes.RemoveAt(i);
                }
            }
        }

        protected override Node CreateNode(Track track, Segment segment, Osm.Node node, int x, int y)
        {
            Node n = new Node(track, segment, node)
            {
                MapX = x,
                MapY = y
            };

            if (!segment.Name.Equals(segment.Ref))
                n.Name = $"{segment.Name} ({segment.Ref})";

            n.Prefab = "Highway";

            if (segment.Lanes == 2)
                n.Prefab = "Rural Highway";

            if (segment.IsTunnel)
                n.Prefab += " Tunnel";

            if (segment.IsBridge)
                n.Prefab += " Bridge";

            return n;
        }
    }
}