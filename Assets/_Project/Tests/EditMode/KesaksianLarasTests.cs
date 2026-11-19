using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class KesaksianLarasTests
    {
        [Test]
        public void AwalnyaLarasBisaDiajakBicara()
        {
            Assert.IsTrue(KesaksianLaras.BisaBicara(new GameState()));
        }

        [Test]
        public void DibentakLarasMengunciDiriTanpaTeh()
        {
            var s = new GameState();
            KesaksianLaras.Bentak(s);
            Assert.IsTrue(s.LarasMengunciDiri);
            Assert.IsFalse(KesaksianLaras.BisaBicara(s));
            Assert.IsFalse(KesaksianLaras.BeriTeh(s));
            Assert.IsTrue(s.LarasMengunciDiri);
        }

        [Test]
        public void TehMembukaKunciDanHabisDipakai()
        {
            var s = new GameState();
            KesaksianLaras.Bentak(s);
            Inventaris.Tambah(s, Inventaris.TehHangat);
            Assert.IsTrue(KesaksianLaras.BisaBicara(s));
            Assert.IsTrue(KesaksianLaras.BeriTeh(s));
            Assert.IsFalse(s.LarasMengunciDiri);
            Assert.IsFalse(Inventaris.Punya(s, Inventaris.TehHangat));
        }

        [Test]
        public void TehTidakDipakaiJikaLarasTidakMengunciDiri()
        {
            var s = new GameState();
            Inventaris.Tambah(s, Inventaris.TehHangat);
            Assert.IsFalse(KesaksianLaras.BeriTeh(s));
            Assert.IsTrue(Inventaris.Punya(s, Inventaris.TehHangat));
        }
    }
}
