using BuildIt.Builds;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BuildItTest.Builds
{
    [TestClass]
    public class HighwayTest
    {
        // Bunde, Limburg NL
        private const double lat = 50.90111;
        private const double lon = 5.73407;

        private static Highway highway;
        private static Airline airline;

        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            // simulate previous prefab build
            airline = new Airline(lat, lon);
            airline.ResetTRackId();
            airline.Build();
            highway = new Highway(lat, lon);
        }

        [TestMethod]
        public void CanParseXML()
        {
            string xml = UnittestTools.ReadResource("testhighway.xml");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList xmlNodeList;
            xmlNodeList = doc.SelectNodes("/osm/node");
            Assert.AreEqual(1325, xmlNodeList.Count);

            XmlNode node = xmlNodeList[0];
            long id = Convert.ToInt64(node.Attributes["id"].Value);
            Assert.AreEqual(2291232, id);
        }

        [TestMethod]
        public void CanBuildHighway()
        {
            // check if Airline is build
            Assert.IsNotNull(airline);
            Assert.AreEqual(1, airline.Tracks.Count);
            Assert.AreEqual(1, airline.Tracks[0].Id);

            // simulate highway build
            Assert.IsInstanceOfType(highway, typeof(Highway));
            string xml = UnittestTools.ReadResource("testhighway.xml");
            int unresolved = highway.Build(xml);

            // for debugging
            highway.ToCSV(@"D:\Downloads\tracks.csv");

            Assert.AreEqual(0, unresolved);
            Assert.AreEqual(6, highway.Tracks.Count);
            Assert.AreEqual("A2", highway.Tracks[0].Name);
            Assert.AreEqual(197, highway.Tracks[0].Nodes.Count);

            // first track = 1!! Because of airline build, this is track 2 !!!
            // note Tracks are sorted by name
            List<Track> sorted = highway.Tracks.OrderBy(t => t.Id).ToList();
            Assert.AreEqual(2, sorted[0].Id); 

            Assert.AreEqual(0, highway.Tracks[0].Nodes[0].Elevation);

            // Koning Willem-Alexandertunnel is in Track with Id 2 ??
            Node node = sorted[0].Nodes.FirstOrDefault(n => n.Id == 4683253807);
            Assert.IsNotNull(node);
            Assert.AreEqual(329098539, node.Segment.Id);
            Assert.AreEqual("Koning Willem-Alexandertunnel (A2)", node.Name);
            Assert.AreEqual(2, node.Segment.Lanes);
            Assert.IsTrue(node.Segment.IsTunnel);
            Assert.IsFalse(node.Segment.IsBridge);
        }

        //[TestMethod]
        //public void CanCreateRailwayData()
        //{
        //    string xml = UnittestTools.ReadResource("testrailway.xml");
        //    int unresolved = geoData.GetRailways(xml);
        //    Assert.AreEqual(41, unresolved);
        //    Assert.AreEqual(41, geoData.Tracks.Count);
        //    // for debugging
        //    //geoData.ToCSV(@"D:\Downloads\tracks.csv");
        //}
    }
}