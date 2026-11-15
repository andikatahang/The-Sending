using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class KepercayaanSumiTests
    {
        [Test]
        public void SetiapAksiHanyaMenambahSekali()
        {
            var s = new GameState { MalamKe = 3 };
            Assert.IsTrue(KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf));
            Assert.IsFalse(KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf));
            Assert.AreEqual(1, s.KepercayaanSumi);
        }

        [Test]
        public void TigaAksiMencapaiMaksimal3()
        {
            var s = new GameState { MalamKe = 4 };
            KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf);
            KepercayaanSumi.Tambah(s, AksiSumi.BantuLampuDanAir);
            KepercayaanSumi.Tambah(s, AksiSumi.BelaDiPosRonda);
            Assert.AreEqual(3, s.KepercayaanSumi);
            Assert.IsTrue(KepercayaanSumi.BisaBeriMinyakSereh(s));
        }

        [Test]
        public void AmbangRitualDanKesaksian()
        {
            var s = new GameState { MalamKe = 3 };
            Assert.IsFalse(KepercayaanSumi.BisaAjarkanRitual(s));
            KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf);
            Assert.IsTrue(KepercayaanSumi.BisaAjarkanRitual(s));
            Assert.IsFalse(KepercayaanSumi.BisaBeriKesaksian(s));
            KepercayaanSumi.Tambah(s, AksiSumi.BantuLampuDanAir);
            Assert.IsTrue(KepercayaanSumi.BisaBeriKesaksian(s));
        }

        [Test]
        public void MenuduhMengurangi1DanMenutupPintuSampaiMalamBerikutnya()
        {
            var s = new GameState { MalamKe = 3 };
            KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf);
            KepercayaanSumi.Tuduh(s);
            Assert.AreEqual(0, s.KepercayaanSumi);
            Assert.IsFalse(KepercayaanSumi.PintuTerbuka(s));
            s.MalamKe = 4;
            Assert.IsTrue(KepercayaanSumi.PintuTerbuka(s));
        }

        [Test]
        public void MenuduhTidakMembuatNilaiNegatif()
        {
            var s = new GameState { MalamKe = 3 };
            KepercayaanSumi.Tuduh(s);
            Assert.AreEqual(0, s.KepercayaanSumi);
        }

        [Test]
        public void MinyakSerehHanyaBisaDiterimaDenganKepercayaan3()
        {
            var s = new GameState { MalamKe = 4, KepercayaanSumi = 2 };
            Assert.Throws<InvalidOperationException>(() => KepercayaanSumi.TerimaMinyakSereh(s));
            s.KepercayaanSumi = 3;
            KepercayaanSumi.TerimaMinyakSereh(s);
            Assert.IsTrue(s.PunyaMinyakSereh);
        }
    }
}
