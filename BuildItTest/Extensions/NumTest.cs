using BuildIt.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildItTest.Extensions
{
    [TestClass]
    public class NumTest
    {
        [TestMethod]
        public void TestNum()
        {
            Assert.AreEqual(256, Num.SwapBytes(1));
            Assert.AreEqual(1, Num.SwapBytes(256));
            Assert.AreEqual(2, Num.SwapBytes(512));
            Assert.AreEqual(15, Num.SwapBytes(3840));
        }
    }
}
