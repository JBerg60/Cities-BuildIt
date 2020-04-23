using BuildIt.Extensions;
using BuildIt.Geo;
using BuildIt.Osm;
using System.Collections.Generic;
using System.Linq;

namespace BuildIt.Builds
{
    public abstract class OsmTrackCollection : TrackCollection
    {
        protected Overpass overpass;

        public OsmTrackCollection(double lat, double lon) : base(lat, lon)
        {
            overpass = new Overpass(leftTopLat, leftTopLon, rightBottomLat, rightBottomLon);
        }

        public override int Build()
        {
            string xml = overpass.Query(Query);
            overpass.ParseSegmentsAndNodes(xml);
            return ParseTracks();
        }

        public virtual int Build(string xml)
        {
            overpass.ParseSegmentsAndNodes(xml);
            return ParseTracks();
        }

        protected abstract string Query { get; }

        protected abstract Tracktype Tracktype { get; }

        // returns the unresolved segment count
        protected virtual int ParseTracks()
        {
            Tracks.Clear();

            // make a copy of overpass Segments
            List<Segment> segments = new List<Segment>(overpass.Segments);
            List<Segment> starts = new List<Segment>(GetStartSegments(overpass.Segments));

            foreach (Segment start in starts)
            {
                ConstructTrack(segments, start);
            }

            // at this point we should have only segments that tweak of from other tracks
            List<Segment> rest = new List<Segment>(GetStartSegments(segments));
            foreach (Segment start in rest)
            {
                ConstructTrack(segments, start);
            }

            // sort by name, without creating a new list
            Tracks.Sort((x, y) => x.Name.CompareTo(y.Name));

            PostProcess();

            // should be 0!
            return segments.Count;
        }

        public abstract void PostProcess();

        protected virtual void ConstructTrack(List<Segment> segments, Segment start)
        {
            Track track = new Track(++trackId, Tracktype) { Name = start.Name };
            AddSegmentToTrack(track, start);
            segments.Remove(start);

            Segment segment = start;
            while ((segment = NextSegmentInTrack(segments, segment)) != null)
            {
                AddSegmentToTrack(track, segment);
                segments.Remove(segment);
            }

            Tracks.Add(track);
        }

        protected virtual void AddSegmentToTrack(Track track, Segment segment)
        {
            foreach (Osm.Node node in segment.Nodes)
            {
                int x = (int)GeoTools.LonDistance(node.Lon, lon, lat);
                int y = (int)GeoTools.LatDistance(node.Lat, lat);

                // do not add segments that are outside the map
                if (!OnMap(x, y))
                {
                    // TODO: try to make outside connection
                    continue;
                }

                track.Nodes.Add(CreateNode(track, segment, node, x, y));
            }
        }        

        protected virtual bool OnMap(int x, int y)
        {
            // TODO: make outside connection later
            int edge = MapSize / 2 - 48;
            return x.IsBetweenII(-edge, edge) && y.IsBetweenEE(-edge, edge);
        }

        protected virtual Segment NextSegmentInTrack(List<Segment> segments, Segment segment)
        {
            return segments.FirstOrDefault(s => s.FirstNode == segment.LastNode);
        }

        protected virtual List<Segment> GetStartSegments(List<Segment> segments)
        {
            List<Segment> startSegments = new List<Segment>();

            foreach (Segment segment in segments)
            {
                var foundSegement = segments.FirstOrDefault(s => s.LastNode == segment.FirstNode);
                if (foundSegement == null)
                {
                    startSegments.Add(segment);
                }
            }

            return startSegments;
        }
    }
}
