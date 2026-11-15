using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class BateraiKompasTests
    {
        [Test]
        public void BateraiAwalPenuhDanHabisSetelah600Detik()
        {
            var s = new GameState();
            Assert.AreEqual(BateraiSenter.Penuh, s.BateraiPersen);
            BateraiSenter.Pakai(s, 300f);
            Assert.AreEqual(50f, s.BateraiPersen, 0.001f);
            BateraiSenter.Pakai(s, 1000f);
            Assert.AreEqual(0f, s.BateraiPersen);
            Assert.IsFalse(BateraiSenter.BisaNyala(s));
        }

        [Test]
        public void IsiMengembalikanBateraiPenuh()
        {
            var s = new GameState { BateraiPersen = 3f };
            BateraiSenter.Isi(s);
            Assert.AreEqual(BateraiSenter.Penuh, s.BateraiPersen);
        }

        [Test]
        public void BateraiDiLuarRentangDitolakSaatMuatSave()
        {
            var asli = SaveData.KeJson(new GameState());
            var json = asli.Replace("\"BateraiPersen\":100.0", "\"BateraiPersen\":150.0");
            Assert.AreNotEqual(asli, json);
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [TestCase(20f, 0f)]
        [TestCase(12f, 0f)]
        [TestCase(6f, 585f)]
        [TestCase(0f, 1080f)]
        public void JarumKompasMakinCepatSaatMediaMakinDekat(float jarak, float derajatPerDetik)
        {
            Assert.AreEqual(derajatPerDetik, Kompas.KecepatanPutar(jarak), 0.001f);
        }

        [Test]
        public void JarakTidakValidMembuatJarumDiam()
        {
            Assert.AreEqual(0f, Kompas.KecepatanPutar(float.NaN));
            Assert.AreEqual(0f, Kompas.KecepatanPutar(-1f));
        }

        [TestCase(3, 2, 5, true)]
        [TestCase(1, 2, 5, false)]
        [TestCase(6, 2, 5, false)]
        public void RentangMalamInklusif(int malam, int dari, int sampai, bool aktif)
        {
            Assert.AreEqual(aktif, RentangMalam.Berisi(malam, dari, sampai));
        }
    }
}
