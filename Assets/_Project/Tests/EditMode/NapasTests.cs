using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class NapasTests
    {
        [Test]
        public void MenahanMengurangiSisa()
        {
            var n = new Napas();
            n.Tick(3f, true);
            Assert.IsTrue(n.SedangMenahan);
            Assert.AreEqual(Napas.BatasDetik - 3f, n.Sisa, 0.001f);
        }

        [Test]
        public void SetelahBatasHabisNapasTerlepasWalauTombolMasihDitahan()
        {
            var n = new Napas();
            n.Tick(Napas.BatasDetik, true);
            Assert.IsFalse(n.SedangMenahan);
            n.Tick(1f, true);
            Assert.IsFalse(n.SedangMenahan);
        }

        [Test]
        public void HarusMelepasTombolSebelumBisaMenahanLagi()
        {
            var n = new Napas();
            n.Tick(Napas.BatasDetik, true);
            n.Tick(2f, false);                 // lepas, pulih 1 detik
            n.Tick(0.1f, true);
            Assert.IsTrue(n.SedangMenahan);
        }

        [Test]
        public void TidakMenahanMemulihkanSampaiBatas()
        {
            var n = new Napas();
            n.Tick(4f, true);
            n.Tick(100f, false);
            Assert.IsFalse(n.SedangMenahan);
            Assert.AreEqual(Napas.BatasDetik, n.Sisa, 0.001f);
        }
    }
}
