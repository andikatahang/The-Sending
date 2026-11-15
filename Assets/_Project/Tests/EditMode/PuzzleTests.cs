using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PuzzleTests
    {
        [Test]
        public void UrutanBenarMenyelesaikanPuzzle()
        {
            var p = new PuzzleUrutan<int>(new[] { 1, 2, 3 }, ulangDariAwalSaatSalah: false);
            Assert.IsTrue(p.Pilih(1));
            Assert.IsTrue(p.Pilih(2));
            Assert.IsFalse(p.Selesai);
            Assert.IsTrue(p.Pilih(3));
            Assert.IsTrue(p.Selesai);
        }

        [Test]
        public void SalahDenganUlangDariAwalMengembalikanKemajuanKeNol()
        {
            var p = new PuzzleUrutan<int>(new[] { 1, 2, 3 }, ulangDariAwalSaatSalah: true);
            p.Pilih(1);
            Assert.IsFalse(p.Pilih(3));
            Assert.AreEqual(0, p.Kemajuan);
        }

        [Test]
        public void SalahTanpaUlangMempertahankanKemajuan()
        {
            var p = new PuzzleUrutan<int>(new[] { 1, 2, 3 }, ulangDariAwalSaatSalah: false);
            p.Pilih(1);
            Assert.IsFalse(p.Pilih(3));
            Assert.AreEqual(1, p.Kemajuan);
        }

        [Test]
        public void PilihSetelahSelesaiDitolak()
        {
            var p = new PuzzleUrutan<int>(new[] { 1 }, false);
            p.Pilih(1);
            Assert.IsFalse(p.Pilih(1));
        }

        [Test]
        public void KunciKosongDitolak()
        {
            Assert.Throws<ArgumentException>(() => new PuzzleUrutan<int>(new int[0], false));
        }

        [TestCase("2003", true)]
        [TestCase("2030", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void GembokLemariHanyaTerbukaDengan2003(string kode, bool terbuka)
        {
            Assert.AreEqual(terbuka, GembokLemari.Cek(kode));
        }

        [Test]
        public void AksaraRatmiDibacaBerurutan()
        {
            var p = AksaraJawa.Buat(AksaraJawa.Ratmi);
            foreach (var aksara in AksaraJawa.Ratmi) Assert.IsTrue(p.Pilih(aksara));
            Assert.IsTrue(p.Selesai);
        }

        [Test]
        public void PakuMalam3HarusDimulaiDariJumat()
        {
            var p = UrutanPaku.Buat(3);
            Assert.IsFalse(p.Pilih(Hari.Minggu));
            Assert.IsTrue(p.Pilih(Hari.Jumat));
            Assert.IsTrue(p.Pilih(Hari.Sabtu));
        }

        [Test]
        public void LilinMalam7HarusDimulaiDariKliwon()
        {
            var p = LilinSesaji.Buat(7);
            Assert.IsFalse(p.Pilih(Pasaran.Legi));
            foreach (var pasaran in KalenderJawa.UrutanLilin(7)) Assert.IsTrue(p.Pilih(pasaran));
            Assert.IsTrue(p.Selesai);
        }

        [TestCase(0.5f, true)]
        [TestCase(0.6f, true)]
        [TestCase(0.61f, false)]
        public void KerekanJatuhJikaDitarikTerlaluCepat(float kecepatan, bool aman)
        {
            Assert.AreEqual(aman, KerekanSumur.TarikanAman(kecepatan));
        }

        [Test]
        public void TanahMakamTidakPernahCocokDanSelesaiSetelahTigaTitik()
        {
            var t = new PerbandinganTanah();
            Assert.IsFalse(t.Bandingkan(0));
            Assert.IsFalse(t.Bandingkan(1));
            Assert.IsFalse(t.SemuaSudahDibandingkan);
            Assert.IsFalse(t.Bandingkan(2));
            Assert.IsTrue(t.SemuaSudahDibandingkan);
            Assert.Throws<ArgumentOutOfRangeException>(() => t.Bandingkan(3));
        }
    }
}
