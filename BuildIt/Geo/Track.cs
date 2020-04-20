using System.Collections.Generic;

namespace BuildIt.Geo
{
    public class Track
    {
        public Track(int id)
        {
            Id = id;
        }

        public int Id { get; }
        public List<Node> Nodes { get; } = new List<Node>();
        public string Name { get; set; }
    }
}
