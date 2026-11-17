using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class EfekDialogTests
    {
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void KodeKosongTidakMengubahApaPun(string kode)
        {
            var s = new GameState();
            EfekDialog.Jalankan(s, kode);
            Assert.AreEqual(SaveData.KeJson(new GameState()), SaveData.KeJson(s));
        }

        [Test]
        public void BeberapaEfekDipisahTitikKoma()
        {
            var s = new GameState();
            EfekDialog.Jalankan(s, "katanya:K1; bukti:B2");
            Assert.IsTrue(s.BuktiKunci[Kode.B2]);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K1]);
        }

        [Test]
        public void HalamanTujuhMemberiB7()
        {
            var s = new GameState();
            EfekDialog.Jalankan(s, "halaman:H7");
            Assert.IsTrue(s.HalamanDiary[Kode.H7]);
            Assert.IsTrue(s.BuktiKunci[Kode.B7]);
        }

        [Test]
        public void KepercayaanBertambah()
        {
            var s = new GameState { MalamKe = 3 };
            EfekDialog.Jalankan(s, "kepercayaan:MintaMaaf");
            Assert.AreEqual(1, s.KepercayaanSumi);
        }

        [Test]
        public void TujuanDitandaiSelesai()
        {
            var s = new GameState { MalamKe = 5 };
            EfekDialog.Jalankan(s, "tujuan:BicaraLaras");
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.BicaraLaras));
        }

        [Test]
        public void MediaDitemukanLaluDiserahkan()
        {
            var s = new GameState { MalamKe = 2 };
            EfekDialog.Jalankan(s, "media:M1;serahkan:M1");
            Assert.IsTrue(s.MediaDiserahkan[Kode.M1]);
            Assert.AreEqual(52, s.KondisiIbu);
        }

        [Test]
        public void PilihanAkhirLewatPenentuEnding()
        {
            var s = new GameState { MalamKe = 7, HasilSidang = HasilSidang.Berhasil, MenitGim = 1300f };
            EfekDialog.Jalankan(s, "pilihan-akhir:Lapor");
            Assert.AreEqual(PilihanAkhir.Lapor, s.PilihanAkhir);
        }

        [TestCase("bukit:B1")]
        [TestCase("bukti:B9")]
        [TestCase("bukti:B0")]
        [TestCase("bukti:K1")]
        [TestCase("kepercayaan:Peluk")]
        [TestCase("tujuan:")]
        [TestCase("tujuan:BicaraLarss")]
        [TestCase("tuduh-sumi:xyz")]
        [TestCase("kepercayaan:1")]
        public void KodeSalahDitolak(string kode)
        {
            Assert.Throws<FormatException>(() => EfekDialog.Jalankan(new GameState { MalamKe = 3 }, kode));
        }

        [Test]
        public void EfekGabunganYangSalahTidakMengubahApaPun()
        {
            var s = new GameState();
            Assert.Throws<FormatException>(() => EfekDialog.Jalankan(s, "bukti:B5;halaman:H9"));
            Assert.IsFalse(s.BuktiKunci[Kode.B5]);
        }

        [Test]
        public void BarangMasukInventaris()
        {
            var s = new GameState();
            EfekDialog.Jalankan(s, "barang:ObatIbu");
            Assert.IsTrue(Inventaris.Punya(s, Inventaris.ObatIbu));
        }

        [Test]
        public void BarangTidakDikenalDitolakTanpaMengubahState()
        {
            var s = new GameState();
            Assert.Throws<FormatException>(() => EfekDialog.Jalankan(s, "katanya:K1; barang:Linggis"));
            Assert.IsFalse(s.KatanyaDiketahui[Kode.K1]);
        }

        [Test]
        public void FotoLewatEfek()
        {
            var s = new GameState { MalamKe = 1 };
            EfekDialog.Jalankan(s, "tujuan:PotretPaku; foto:PakuMalam1");
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.PotretPaku));
            CollectionAssert.Contains(s.Foto, PapanBukti.FotoPakuMalam1);
        }

        [Test]
        public void FotoTidakDikenalLewatEfekDitolak()
        {
            Assert.Throws<FormatException>(() => EfekDialog.Jalankan(new GameState(), "foto:Hantu"));
        }
    }
}
