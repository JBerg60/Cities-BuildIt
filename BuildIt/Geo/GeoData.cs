using BuildIt.Osm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace BuildIt.Geo
{
    public class GeoData
    {
        private double lat;
        private double lon;

        private double leftTopLat;
        private double leftTopLon;

        private double rightBottomLat;
        private double rightBottomLon;

        public const int MapSize = 1920 * 9;

        private static int trackId = 0;

        public List<Track> Tracks { get; } = new List<Track>();

        protected Overpass overpass;

        public GeoData(double lat, double lon)
        {
            this.lat = lat;
            this.lon = lon;

            leftTopLat = GeoTools.DistanceToLat(lat, -MapSize / 2);
            leftTopLon = GeoTools.DistanceToLon(lon, lat, -MapSize / 2);

            rightBottomLat = GeoTools.DistanceToLat(lat, MapSize / 2);
            rightBottomLon = GeoTools.DistanceToLon(lon, lat, MapSize / 2);

            overpass = new Overpass(leftTopLat, leftTopLon, rightBottomLat, rightBottomLon);
        }

        public int GetHighways(string xml = null)
        {
            if (string.IsNullOrEmpty(xml)) xml = overpass.Query(Overpass.Highway);
            overpass.ParseSegmentsAndNodes(xml);
            return ParseTracks();
        }

        public int GetRailways(string xml = null)
        {
            if (string.IsNullOrEmpty(xml)) xml = overpass.Query(Overpass.Railway);
            overpass.ParseSegmentsAndNodes(xml);
            return ParseTracks();
        }

        // returns the unresolved segment count
        protected virtual int ParseTracks()
        {
            Tracks.Clear();
            trackId = 0;

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

            // should be 0!
            return segments.Count;
        }

        protected void ConstructTrack(List<Segment> segments, Segment start)
        {
            Track track = new Track(++trackId) { Name = start.Name };
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

        protected void AddSegmentToTrack(Track track, Segment segment)
        {
            foreach (var n in segment.Nodes)
            {
                track.Nodes.Add(new Node(track.Id, n)
                {
                    MapX = (int)GeoTools.Distance(leftTopLat, leftTopLon, leftTopLat, n.Lon) - MapSize / 2,
                    MapY = (int)GeoTools.Distance(leftTopLat, leftTopLon, n.Lat, leftTopLon) - MapSize / 2
                });
            }
        }

        protected Segment NextSegmentInTrack(List<Segment> segments, Segment segment)
        {
            return segments.FirstOrDefault(s => s.FirstNode == segment.LastNode);
        }


        public void ToCSV(string filename)
        {
            var saveCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                int newTrack;
                using (StreamWriter csv = new StreamWriter(filename, false))
                {
                    csv.WriteLine("type,new_track,latitude,longitude");
                    foreach (Track track in Tracks)
                    {
                        newTrack = 1;
                        foreach (Node n in track.Nodes)
                        {
                            csv.WriteLine("T,{0},{1},{2}", newTrack, n.OsmNode.Lat, n.OsmNode.Lon);
                            newTrack = 0;
                        }
                    }
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = saveCulture;
            }
        }

        protected List<Segment> GetStartSegments(List<Segment> segments)
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