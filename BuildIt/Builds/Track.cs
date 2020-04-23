using System.Collections.Generic;

namespace BuildIt.Builds
{
    public enum Tracktype
    {
        Road, 
        Train,
        Airline,
        Shipline,
        Water
    }

    public class Track
    {
        public Track(int id, Tracktype type)
        {
            Id = id;
            Type = type;
        }

        public int Id { get; }

        public Tracktype Type { get; }

        public List<Node> Nodes { get; } = new List<Node>();
        public string Name { get; set; }
    }
}
