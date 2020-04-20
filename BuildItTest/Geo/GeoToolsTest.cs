using System;
using BuildIt.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildItTest.Geo
{
    [TestClass]
    public class GeoToolsTest
    {
        // Bunde, Limburg NL
        const double lat = 50.90111;
        const double lon = 5.73407;

        [TestMethod]
        public void CanCalculateDistance()
        {
            double leftTopLat = GeoTools.DistanceToLat(lat, -GeoData.MapSize / 2);
            double leftTopLon = GeoTools.DistanceToLon(lon, lat, -GeoData.MapSize / 2);

            double rightBottomLat = GeoTools.DistanceToLat(lat, GeoData.MapSize / 2);
            double rightBottomLon = GeoTools.DistanceToLon(lon, lat, GeoData.MapSize / 2);

            double distance = GeoTools.Distance(leftTopLat, leftTopLon, rightBottomLat, rightBottomLon);
            double mapHypothenysa = Math.Sqrt(2 * Math.Pow(GeoData.MapSize, 2));
            Assert.AreEqual(24437.61f, mapHypothenysa, 0.01);
            Assert.AreEqual(distance, mapHypothenysa, 0.1);
        }
    }
}
