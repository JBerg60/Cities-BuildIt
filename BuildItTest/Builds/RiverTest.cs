using BuildIt.Builds;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildItTest.Builds
{
    [TestClass]
    public class RiverTest
    {
        // Bunde, Limburg NL
        private const double lat = 50.90111;
        private const double lon = 5.73407;

        private static River river;

        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            river = new River(lat, lon);
        }

        [TestMethod]
        public void CanBuildRiver()
        {
            Assert.IsInstanceOfType(river, typeof(River));
            string xml = UnittestTools.ReadResource("testriver.xml");
            int unresolved = river.Build(xml);
            Assert.AreEqual(0, unresolved);
            Assert.AreEqual(11, river.Tracks.Count);
            Assert.AreEqual("Geul", river.Tracks[0].Name);

            Assert.AreEqual(Tracktype.Water, river.Tracks[0].Type);

            TestRiver testRiver = new TestRiver(lat, lon);
            Assert.AreEqual("[waterway=river]", testRiver.TestQuery);

            // for debugging
            //river.ToCSV(@"D:\Downloads\rivers.csv");
        }
    }

    // override River class, to test protected properties
    internal class TestRiver : River
    {
        public TestRiver(double lat, double lon) : base(lat, lon) { }

        public string TestQuery { get => Query; }
    }
}
