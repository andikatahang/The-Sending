using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class SidangWargaTests
    {
        static GameState DenganBukti(params int[] bukti)
        {
            var s = new GameState { MalamKe = 6 };
            foreach (var b in bukti) s.BuktiKunci[b] = true;
            return s;
        }

        [TestCase(0, Kode.B4)]
        [TestCase(0, Kode.B5)]
        [TestCase(1, Kode.B1)]
        [TestCase(1, Kode.B6)]
        [TestCase(1, Kode.B7)]
        [TestCase(2, Kode.B2)]
        [TestCase(2, Kode.B3)]
        public void BuktiValidYangDimilikiDianggapBenar(int tuduhan, int bukti)
        {
            Assert.IsTrue(SidangWarga.JawabanBenar(DenganBukti(bukti), tuduhan, ItemPapan.Bukti(bukti)));
        }

        [Test]
        public void BuktiYangBelumDimilikiDianggapSalah()
        {
            Assert.IsFalse(SidangWarga.JawabanBenar(DenganBukti(), 0, ItemPapan.Bukti(Kode.B4)));
        }

        [Test]
        public void BuktiUntukTuduhanLainDianggapSalah()
        {
            Assert.IsFalse(SidangWarga.JawabanBenar(DenganBukti(Kode.B3), 0, ItemPapan.Bukti(Kode.B3)));
        }

        [Test]
        public void MenunjukkanKatanyaSelaluSalah()
        {
            var s = DenganBukti(Kode.B6);
            s.KatanyaDiketahui[Kode.K4] = true;
            s.StatusKatanya[Kode.K4] = StatusInfo.Bukti;
            Assert.IsFalse(SidangWarga.JawabanBenar(s, 1, ItemPapan.Katanya(Kode.K4)));
        }

        [TestCase(3, HasilSidang.Berhasil)]
        [TestCase(2, HasilSidang.Berhasil)]
        [TestCase(1, HasilSidang.Gagal)]
        [TestCase(0, HasilSidang.Gagal)]
        public void MinimalDuaBenarUntukBerhasil(int benar, HasilSidang hasil)
        {
            var s = DenganBukti();
            Assert.AreEqual(hasil, SidangWarga.Selesaikan(s, benar));
            Assert.AreEqual(hasil, s.HasilSidang);
        }

        [Test]
        public void DiamDanIkutMenuduhLangsungGagal()
        {
            var a = DenganBukti();
            var b = DenganBukti();
            Assert.AreEqual(HasilSidang.Diam, SidangWarga.Diam(a));
            Assert.AreEqual(HasilSidang.Menuduh, SidangWarga.IkutMenuduh(b));
            Assert.IsTrue(a.JalurFitnah);
            Assert.IsTrue(b.JalurFitnah);
        }

        [Test]
        public void HasilTidakBisaDitetapkanDuaKali()
        {
            var s = DenganBukti();
            SidangWarga.Selesaikan(s, 2);
            Assert.Throws<InvalidOperationException>(() => SidangWarga.Diam(s));
        }

        [Test]
        public void TuduhanDiLuarRentangDitolak()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SidangWarga.JawabanBenar(DenganBukti(), 3, ItemPapan.Bukti(Kode.B1)));
        }
    }
}
