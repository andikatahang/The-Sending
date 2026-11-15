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
    }
}
