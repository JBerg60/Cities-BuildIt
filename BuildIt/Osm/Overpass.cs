using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace BuildIt.Osm
{
    public class Overpass
    {
        public const string BaseUrl = @"http://overpass-api.de/api/interpreter";

        private double leftTopLat;
        private double leftTopLon;
        private double rightBottomLat;
        private double rightBottomLon;

        public string Format { get; set; } = "xml";
        public int Timeout { get; set; } = 15;

        public const string Highway = "[highway=motorway]";
        public const string Railway = "[railway=rail][electrified=contact_line]";

        public List<Node> Nodes { get; } = new List<Node>();
        public List<Segment> Segments { get; } = new List<Segment>();

        public Overpass(double leftTopLat, double leftTopLon, double rightBottomLat, double rightBottomLon)
        {
            this.leftTopLat = leftTopLat;
            this.leftTopLon = leftTopLon;
            this.rightBottomLat = rightBottomLat;
            this.rightBottomLon = rightBottomLon;
        }

        public string Query(string filter)
        {
            var saveCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

                string query = $@"
                    [out:{Format}][timeout:{Timeout}];
                    way{filter}({leftTopLat},{leftTopLon},{rightBottomLat},{rightBottomLon});
                    (._;>;);
                ";

                query = $"{Regex.Replace(query, @"\s+", string.Empty)}out meta;";

                using (WebClient client = new WebClient())
                {
                    return client.DownloadString($"{BaseUrl}?data={Uri.EscapeUriString(query)}");
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = saveCulture;
            }
        }

        public void ParseSegmentsAndNodes(string xml)
        {
            Nodes.Clear();
            Segments.Clear();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList xmlNodeList;
            xmlNodeList = doc.SelectNodes("/osm/node");
            foreach(XmlNode xmlNode in xmlNodeList)
            {
                Nodes.Add(new Node()
                {
                    Id = Convert.ToInt64(xmlNode.Attributes["id"].Value),
                    Lat = Convert.ToDouble(xmlNode.Attributes["lat"].Value),
                    Lon = Convert.ToDouble(xmlNode.Attributes["lon"].Value)
                });
            }

            xmlNodeList = doc.SelectNodes("/osm/way");
            Segment segment;
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                segment = new Segment()
                {
                    Id = Convert.ToInt64(xmlNode.Attributes["id"].Value)
                };

                foreach(XmlNode n in xmlNode.ChildNodes)
                {
                    if (n.Name == "tag")
                        segment.Tags.Add(n.Attributes["k"].Value, n.Attributes["v"].Value);

                    if (n.Name == "nd")
                        segment.Nodes.Add(Nodes.FirstOrDefault(nd => nd.Id == Convert.ToInt64(n.Attributes["ref"].Value)));
                }
                Segments.Add(segment);
            }
        }        
    }
}