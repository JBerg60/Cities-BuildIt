using BuildIt.Geo;
using BuildIt.Osm;
using System;

namespace BuildIt.Builds
{
    public class Airline : TrackCollection
    {
        public Airline(double lat, double lon) : base(lat, lon)
        {

        }

        public override int Build()
        {
            Tracks.Clear();

            Track track = new Track(++trackId, Tracktype.Airline);            

            Node node;

            Osm.Segment segment = new Osm.Segment();
            segment.Tags.Add("ref", "North/South connection");
            
            Osm.Node osmNode;

            segment.Id = 1;

            // make a track with segment size of 96 meter. (180 segments North/South)
            for(int y = -MapSize / 2; y <= MapSize /2; y += 96)
            {
                osmNode = new Osm.Node();
                osmNode.Lat = GeoTools.DistanceToLat(lat, y);
                osmNode.Lon = lon;

                node = CreateNode(track, segment, osmNode, 0, y);

                track.Nodes.Add(node);
            }

            Tracks.Add(track);

            return 0;
        }

        protected override Node CreateNode(Track track, Segment segment, Osm.Node node, int x, int y)
        {
            return new Node(track, segment, node)
            {
                MapX = x,
                MapY = y,
                Elevation = 1100,
                Prefab = "Airplane Path"
            };
        }
    }
}
