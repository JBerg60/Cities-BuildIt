using BuildIt.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BuildItTest.Geo
{
    [TestClass]
    public class GeoToolsTest
    {
        // Bunde, Limburg NL
        private const double lat = 50.90111;
        private const double lon = 5.73407;

        const int MapSize = 1920 * 9;

        [TestMethod]
        public void AreConstantsCorrect()
        {
            Assert.AreEqual(1852, GeoTools.NauticalMile, 0.2);
            Assert.AreEqual(6378.137f, GeoTools.EarthRadius, 15);
        }

        [TestMethod]
        public void CanCalculateDistance()
        {
            double leftTopLat = GeoTools.DistanceToLat(lat, -MapSize / 2);
            double leftTopLon = GeoTools.DistanceToLon(lon, lat, -MapSize / 2);

            double rightBottomLat = GeoTools.DistanceToLat(lat, MapSize / 2);
            double rightBottomLon = GeoTools.DistanceToLon(lon, lat, MapSize / 2);

            double distance = GeoTools.Distance(leftTopLat, leftTopLon, rightBottomLat, rightBottomLon);
            double mapHypothenysa = Math.Sqrt(2 * Math.Pow(MapSize, 2));
            Assert.AreEqual(24437.61f, mapHypothenysa, 0.01);
            Assert.AreEqual(distance, mapHypothenysa, 0.1);
        }

        [TestMethod]
        public void CanCalculateDistanceFromLat()
        {
            double calcLat = GeoTools.DistanceToLat(lat, 60 * GeoTools.NauticalMile);
            Assert.AreEqual(lat + 1, calcLat, 0.01);
        }

        [TestMethod]
        public void CanCalculateDistanceFromLon()
        {
            double latCorrection = Math.Cos(lat * (Math.PI / 180));
            Assert.AreEqual(0.63, latCorrection, 0.01);

            double calcLong = GeoTools.DistanceToLon(lon, lat, 60 * GeoTools.NauticalMile * latCorrection);
            Assert.AreEqual(lon + 1, calcLong, 0.01);
        }

        [TestMethod]
        public void CanCalculateLatDistance()
        {
            double dist = GeoTools.LatDistance(lat - 1, lat);
            Assert.AreEqual(GeoTools.NauticalMile * -60, dist);

            dist = GeoTools.LatDistance(lat + 1, lat);
            Assert.AreEqual(GeoTools.NauticalMile * 60, dist);

            double leftTopLat = GeoTools.DistanceToLat(lat, -MapSize / 2);
            dist = GeoTools.LatDistance(leftTopLat, lat);
            Assert.AreEqual(-MapSize / 2, dist, 0.01);
        }

        [TestMethod]
        public void CanCalculateLonDistance()
        {
            double latCorrection = Math.Cos(lat * (Math.PI / 180));
            double dist = GeoTools.LonDistance(lon - 1, lon, lat);
            Assert.AreEqual(GeoTools.NauticalMile * -60 * latCorrection, dist);

            double leftTopLon = GeoTools.DistanceToLon(lon, lat, -MapSize / 2);
            dist = GeoTools.LonDistance(leftTopLon, lon, lat);
            Assert.AreEqual(-MapSize / 2, dist, 0.01);
        }
    }
}