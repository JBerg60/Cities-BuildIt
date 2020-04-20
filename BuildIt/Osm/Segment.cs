using System.Collections.Generic;

namespace BuildIt.Osm
{
    public class Segment
    {
        public long Id { get; set; }
        
        public string Name 
        { 
            get
            {
                if (Tags.ContainsKey("name")) return Tags["name"];
                if (Tags.ContainsKey("nat_ref")) return Tags["nat_ref"];
                if (Tags.ContainsKey("ref")) return Tags["ref"];
                return string.Empty;
            }
        }

        public string Ref
        {
            get
            {
                if (Tags.ContainsKey("nat_ref")) return Tags["nat_ref"];
                if (Tags.ContainsKey("ref")) return Tags["ref"];
                return string.Empty;
            }
        }

        public Node FirstNode
        {
            get
            {
                return Nodes.Count > 0 ? Nodes[0] : null;
            }
        }

        public Node LastNode
        {
            get
            {
                return Nodes.Count > 0 ? Nodes[Nodes.Count - 1] : null;
            }
        }

        public List<Node> Nodes { get; } = new List<Node>();

        public Dictionary<string, string> Tags { get; } = new Dictionary<string, string>();
    }
}
