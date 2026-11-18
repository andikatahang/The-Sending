using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class KalenderJawaTests
    {
        // Tabel Game Story 3.1
        [TestCase(0, Hari.Rabu, Pasaran.Wage)]   // Prolog = malam yang sama dengan Malam 1
        [TestCase(1, Hari.Rabu, Pasaran.Wage)]
        [TestCase(2, Hari.Kamis, Pasaran.Kliwon)]
        [TestCase(3, Hari.Jumat, Pasaran.Legi)]
        [TestCase(4, Hari.Sabtu, Pasaran.Pahing)]
        [TestCase(5, Hari.Minggu, Pasaran.Pon)]
        [TestCase(6, Hari.Senin, Pasaran.Wage)]
        [TestCase(7, Hari.Selasa, Pasaran.Kliwon)]
        public void HariDanPasaranSesuaiTabelGameStory(int malam, Hari hari, Pasaran pasaran)
        {
            Assert.AreEqual(hari, KalenderJawa.HariMalam(malam));
            Assert.AreEqual(pasaran, KalenderJawa.PasaranMalam(malam));
        }

        [Test]
        public void NamaMalam7AdalahMalamSelasaKliwon()
        {
            Assert.AreEqual("Malam Selasa Kliwon", KalenderJawa.NamaMalam(7));
        }

        [Test]
        public void UrutanPakuMalam3DimulaiDariJumat()
        {
            CollectionAssert.AreEqual(
                new[] { Hari.Jumat, Hari.Sabtu, Hari.Minggu, Hari.Senin, Hari.Selasa, Hari.Rabu, Hari.Kamis },
                KalenderJawa.UrutanPaku(3));
        }

        [Test]
        public void UrutanLilinMalam7DimulaiDariKliwon()
        {
            CollectionAssert.AreEqual(
                new[] { Pasaran.Kliwon, Pasaran.Legi, Pasaran.Pahing, Pasaran.Pon, Pasaran.Wage },
                KalenderJawa.UrutanLilin(7));
        }

        [TestCase(-1)]
        [TestCase(8)]
        public void MalamDiLuarRentangDitolak(int malam)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => KalenderJawa.HariMalam(malam));
        }

        [TestCase(1, 6)]
        [TestCase(2, 5)]   // Game Story Malam 2: Selasa Kliwon jatuh lima malam lagi
        [TestCase(6, 1)]
        [TestCase(7, 0)]
        public void SisaMalamKeSelasaKliwon(int malam, int sisa)
        {
            Assert.AreEqual(sisa, KalenderJawa.SisaMalamKeSelasaKliwon(malam));
        }

        [TestCase(1, false, false)]
        [TestCase(2, false, false)]
        [TestCase(2, true, true)]
        [TestCase(3, false, true)]
        public void HitungMundurBerjalanSejakAksaraTerbaca(int malam, bool aksara, bool berjalan)
        {
            var s = new GameState { MalamKe = malam };
            if (aksara) TujuanMalam.Selesaikan(s, TujuanMalam.BacaAksara);
            Assert.AreEqual(berjalan, KalenderJawa.HitungMundurBerjalan(s));
        }
    }
}
