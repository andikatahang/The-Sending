using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class InventarisTests
    {
        [Test]
        public void StateBaruTanpaBarang()
        {
            Assert.IsEmpty(new GameState().Barang);
        }

        [Test]
        public void TambahMenyimpanDanMengumumkan()
        {
            var s = new GameState();
            Perubahan? terakhir = null;
            s.Berubah += p => terakhir = p;

            Inventaris.Tambah(s, Inventaris.ObatIbu);

            Assert.IsTrue(Inventaris.Punya(s, Inventaris.ObatIbu));
            Assert.IsFalse(Inventaris.Punya(s, Inventaris.MapSertifikat));
            Assert.AreEqual(Perubahan.Barang, terakhir);
        }

        [Test]
        public void TambahDuaKaliTidakMenggandakan()
        {
            var s = new GameState();
            int jumlahEvent = 0;
            s.Berubah += p => jumlahEvent++;

            Inventaris.Tambah(s, Inventaris.ObatIbu);
            Inventaris.Tambah(s, Inventaris.ObatIbu);

            Assert.AreEqual(1, s.Barang.Count);
            Assert.AreEqual(1, jumlahEvent);
        }

        [Test]
        public void BarangTidakDikenalDitolak()
        {
            var s = new GameState();
            Assert.Throws<ArgumentException>(() => Inventaris.Tambah(s, "Linggis?"));
            Assert.IsEmpty(s.Barang);
        }

        [TestCase(Inventaris.ObatIbu)]
        [TestCase(Inventaris.BukuHarianAyah)]
        [TestCase(Inventaris.MapSertifikat)]
        [TestCase(Inventaris.KunciGudang)]
        [TestCase(Inventaris.Linggis)]
        public void SetiapBarangPunyaNamaDanKeterangan(string id)
        {
            Assert.IsTrue(Inventaris.Dikenal(id));
            Assert.IsNotEmpty(Inventaris.Nama(id));
            Assert.IsNotEmpty(Inventaris.Keterangan(id));
        }

        [Test]
        public void NamaBarangTidakDikenalDitolak()
        {
            Assert.IsFalse(Inventaris.Dikenal("X"));
            Assert.IsFalse(Inventaris.Dikenal(null));
            Assert.Throws<ArgumentException>(() => Inventaris.Nama("X"));
        }
    }
}
