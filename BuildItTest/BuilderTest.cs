using BuildIt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildItTest
{
    [TestClass]
    public class BuilderTest
    {
        private int ticks;

        [TestMethod]
        public void CanTick()
        {
            Builder builder = new Builder();
            Assert.IsFalse(builder.IsRunning);
            builder.OnUpdate += BuilderOnUpdate;

            ticks = 0;
            builder.Next();
            Assert.AreEqual(0, ticks);

            builder.Start();
            builder.Next();
            Assert.AreEqual(1, ticks);
        }

        private void BuilderOnUpdate(object sender, System.EventArgs e)
        {
            ticks++;
        }
    }
}
