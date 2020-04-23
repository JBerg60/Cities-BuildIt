using BuildIt.Builds;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildItTest.Builds
{
    [TestClass]
    public class AirlineTest
    {
        // Bunde, Limburg NL
        private const double lat = 50.90111;
        private const double lon = 5.73407;

        const int MapSize = 1920 * 9;

        private static Airline airline;

        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            airline = new Airline(lat, lon);
        }

        [TestMethod]
        public void CanBuild()
        {
            Assert.IsInstanceOfType(airline, typeof(Airline));
            airline.Build();
            Assert.AreEqual(1, airline.Tracks.Count);
            Assert.AreEqual(181, airline.Tracks[0].Nodes.Count);
            Assert.AreEqual(-MapSize / 2, airline.Tracks[0].Nodes[0].MapY);
            Assert.AreEqual(MapSize / 2, airline.Tracks[0].Nodes[180].MapY);
            Assert.AreEqual("Airplane Path", airline.Tracks[0].Nodes[0].Prefab);
            Assert.AreEqual("North/South connection", airline.Tracks[0].Nodes[180].Name);
            Assert.AreEqual(1100, airline.Tracks[0].Nodes[180].Elevation);
            Assert.AreEqual(Tracktype.Airline, airline.Tracks[0].Type);

            airline.ToCSV(@"D:\Downloads\airlines.csv");
        }
    }
}
