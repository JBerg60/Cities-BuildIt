using BuildIt.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;

namespace BuildItTest.Geo
{
    [TestClass]
    public class GeoDataTest
    {
        // Bunde, Limburg NL
        const double lat = 50.90111;
        const double lon = 5.73407;

        private static GeoData geoData;

        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            geoData = new GeoData(lat, lon);
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
        public void CanCreateGeoData()
        {
            Assert.IsInstanceOfType(geoData, typeof(GeoData));
            string xml = UnittestTools.ReadResource("testhighway.xml");
            int unresolved = geoData.GetHighways(xml);
            Assert.AreEqual(0, unresolved);
            Assert.AreEqual(6, geoData.Tracks.Count);
            // for debugging
            //geoData.ToCSV(@"D:\Downloads\tracks.csv");
        }

        [TestMethod]
        public void CanCreateRailwayData()
        {
            string xml = UnittestTools.ReadResource("testrailway.xml");
            int unresolved = geoData.GetRailways(xml);
            Assert.AreEqual(41, unresolved);
            Assert.AreEqual(41, geoData.Tracks.Count);
            // for debugging
            geoData.ToCSV(@"D:\Downloads\tracks.csv");
        }
    }
}
