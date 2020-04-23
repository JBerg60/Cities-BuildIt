namespace BuildIt.Builds
{
    public class Node
    {
        public Node(Track track, Osm.Segment segment, Osm.Node node)
        {
            Track = track;
            Segment = segment;
            OsmNode = node;

            Name = Segment.Name;
        }

        public long Id { get => OsmNode.Id; }

        public Track Track { get; }

        public Osm.Node OsmNode { get; }

        public Osm.Segment Segment { get; }

        public int MapX { get; set; }
        public int MapY { get; set; }

        public string Prefab { get; set; }

        public virtual string Name { get; set; }  

        public int Elevation { get; set; } = 0;
        public float TerrainHeight { get; set; }
        public int Width { get; set; }
    }
}