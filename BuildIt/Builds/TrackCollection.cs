using BuildIt.Geo;
using BuildIt.Osm;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BuildIt.Builds
{
    public abstract class TrackCollection
    {
        public const int MapSize = 1920 * 9;

        protected double lat;
        protected double lon;

        protected double leftTopLat;
        protected double leftTopLon;

        protected double rightBottomLat;
        protected double rightBottomLon;

        protected static int trackId = 0;

        public List<Track> Tracks { get; } = new List<Track>();

        public TrackCollection(double lat, double lon)
        {
            this.lat = lat;
            this.lon = lon;

            leftTopLat = GeoTools.DistanceToLat(lat, -MapSize / 2);
            leftTopLon = GeoTools.DistanceToLon(lon, lat, -MapSize / 2);

            rightBottomLat = GeoTools.DistanceToLat(lat, MapSize / 2);
            rightBottomLon = GeoTools.DistanceToLon(lon, lat, MapSize / 2);
        }

        public abstract int Build();

        public void ToCSV(string filename)
        {
            var saveCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                int newTrack;
                string trackName;
                using (StreamWriter csv = new StreamWriter(filename, false))
                {
                    csv.WriteLine("type,new_track,latitude,longitude,mapx,mapy,prefab,elevation");
                    foreach (Track track in Tracks)
                    {
                        newTrack = 1;
                        trackName = track.Name;
                        foreach (Node n in track.Nodes)
                        {
                            csv.WriteLine("T,{0},{1:F10},{2:F10},{3},{4},{5},{6},{7}",
                                newTrack, n.OsmNode.Lat, n.OsmNode.Lon, n.MapX, n.MapY, n.Prefab, n.Elevation, trackName);
                            newTrack = 0;
                            trackName = "";
                        }
                    }
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = saveCulture;
            }
        }

        protected abstract Node CreateNode(Track track, Segment segment, Osm.Node node, int x, int y);

        // for Unittesting
        public void ResetTRackId()
        {
            trackId = 0;
        }
    }
}