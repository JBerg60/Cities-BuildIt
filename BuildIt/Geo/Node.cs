namespace BuildIt.Geo
{
    public class Node
    {

        public Node(int trackId, Osm.Node node)
        {
            TrackId = trackId;
            OsmNode = node;            
        }

        public int TrackId { get; }

        public Osm.Node OsmNode { get; }

        public int MapX { get; set; }
        public int MapY { get; set; }
    }
}
