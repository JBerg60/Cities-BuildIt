using BuildIt.Osm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BuildItTest.Osm
{
    [TestClass]
    public class OverpassTest
    {
        // Bunde, Limburg NL
        const double lat = 50.90111;
        const double lon = 5.73407;

        const double leftTopLat = 50.82;
        const double leftTopLon = 5.61;
        const double bottomRightLat = 50.98;
        const double bottomRightLon = 5.86;        

        [TestMethod]
        public void CanGetOverpassQuery()
        {
            Overpass overpass = new Overpass(leftTopLat, leftTopLon, bottomRightLat, bottomRightLon);
            Assert.IsInstanceOfType(overpass, typeof(Overpass));
            string str = overpass.Query(Overpass.Highway);
            Assert.IsFalse(string.IsNullOrEmpty(str));
            Assert.AreEqual("<?xml", str.Substring(0, 5));
            //File.WriteAllText(@"C:\Users\John\tmp\testrailway.xml", overpass.Query(Overpass.Railway));
        }

        [TestMethod]
        public void CanGetOverpassHighways()
        {
            Overpass overpass = new Overpass(leftTopLat, leftTopLon, bottomRightLat, bottomRightLon);
            string xml = UnittestTools.ReadResource("testhighway.xml");
            // does ReadResource work? (for testing we do NOT need to query overpass.de)
            Assert.IsFalse(string.IsNullOrEmpty(xml));
            Assert.AreEqual("<?xml", xml.Substring(0, 5));
            overpass.ParseSegmentsAndNodes(xml);
            Assert.AreEqual(1325, overpass.Nodes.Count);
            Assert.AreEqual(285, overpass.Segments.Count);
            Assert.AreEqual("A2", overpass.Segments[0].Name);
            Assert.AreEqual(4842325, overpass.Segments[0].Id);
            Assert.AreEqual(2, overpass.Segments[0].Nodes.Count);
            Assert.AreEqual(10, overpass.Segments[0].Tags.Count);
        }
    }
}