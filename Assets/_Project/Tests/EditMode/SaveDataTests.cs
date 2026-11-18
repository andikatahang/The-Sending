using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class SaveDataTests
    {
        [Test]
        public void StateBaruDimulaiDiPrologDenganKondisi60()
        {
            var s = new GameState();
            Assert.AreEqual(0, s.MalamKe);
            Assert.AreEqual(60, s.KondisiIbu);
            Assert.AreEqual(1050f, s.MenitGim);
            Assert.AreEqual(Kode.JumlahMedia, s.MediaDitemukan.Length);
            Assert.AreEqual(Ending.Belum, s.Ending);
            Assert.IsFalse(s.JalurFitnah);
        }

        [Test]
        public void SimpanLaluMuatMengembalikanIsiYangSama()
        {
            var s = new GameState { MalamKe = 4, KondisiIbu = 37, MenitGim = 1290f, KepercayaanSumi = 2 };
            s.MediaDitemukan[Kode.M3] = true;
            s.BuktiKunci[Kode.B4] = true;
            s.StatusKatanya[Kode.K3] = StatusInfo.Terbantahkan;
            s.TujuanSelesai.Add("CabutPaku");

            var hasil = SaveData.DariJson(SaveData.KeJson(s));

            Assert.AreEqual(4, hasil.MalamKe);
            Assert.AreEqual(37, hasil.KondisiIbu);
            Assert.AreEqual(1290f, hasil.MenitGim);
            Assert.AreEqual(2, hasil.KepercayaanSumi);
            Assert.IsTrue(hasil.MediaDitemukan[Kode.M3]);
            Assert.IsTrue(hasil.BuktiKunci[Kode.B4]);
            Assert.AreEqual(StatusInfo.Terbantahkan, hasil.StatusKatanya[Kode.K3]);
            CollectionAssert.AreEqual(new[] { "CabutPaku" }, hasil.TujuanSelesai);
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        [TestCase("ini bukan json")]
        public void DataKosongAtauRusakDitolak(string json)
        {
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void MalamDiLuarRentangDitolak()
        {
            var asli = SaveData.KeJson(new GameState());
            var json = asli.Replace("\"MalamKe\":0", "\"MalamKe\":9");
            Assert.AreNotEqual(asli, json);
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void KondisiDiLuarRentangDitolak()
        {
            var asli = SaveData.KeJson(new GameState());
            var json = asli.Replace("\"KondisiIbu\":60", "\"KondisiIbu\":150");
            Assert.AreNotEqual(asli, json);
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void PanjangArraySalahDitolak()
        {
            var asli = SaveData.KeJson(new GameState());
            var json = asli.Replace(
                "\"BuktiKunci\":[false,false,false,false,false,false,false,false]",
                "\"BuktiKunci\":[false]");
            Assert.AreNotEqual(asli, json);
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void EndingTidakDikenalDitolak()
        {
            var asli = SaveData.KeJson(new GameState());
            var json = asli.Replace("\"Ending\":0", "\"Ending\":99");
            Assert.AreNotEqual(asli, json);
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void MenitGimDiLuarRentangDitolak()
        {
            var asli = SaveData.KeJson(new GameState());
            var json = asli.Replace("\"MenitGim\":1050.0", "\"MenitGim\":-5.0");
            Assert.AreNotEqual(asli, json);
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void PintuSumiDiLuarRentangDitolak()
        {
            var asli = SaveData.KeJson(new GameState());
            var json = asli.Replace("\"PintuSumiTerbukaMulaiMalam\":0", "\"PintuSumiTerbukaMulaiMalam\":9");
            Assert.AreNotEqual(asli, json);
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void StatusKatanyaTidakDikenalDitolak()
        {
            var asli = SaveData.KeJson(new GameState());
            var json = asli.Replace("\"StatusKatanya\":[0,0,0,0]", "\"StatusKatanya\":[0,0,0,7]");
            Assert.AreNotEqual(asli, json);
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void EventBerubahMengirimJenisPerubahan()
        {
            var s = new GameState();
            Perubahan? diterima = null;
            s.Berubah += p => diterima = p;
            s.Umumkan(Perubahan.Kondisi);
            Assert.AreEqual(Perubahan.Kondisi, diterima);
        }

        [Test]
        public void BarangIkutTersimpan()
        {
            var s = new GameState();
            Inventaris.Tambah(s, Inventaris.ObatIbu);
            var hasil = SaveData.DariJson(SaveData.KeJson(s));
            CollectionAssert.AreEqual(new[] { Inventaris.ObatIbu }, hasil.Barang);
        }

        [Test]
        public void BarangTidakDikenalDitolak()
        {
            var s = new GameState();
            s.Barang.Add("Linggis?");
            Assert.Throws<FormatException>(() => SaveData.DariJson(SaveData.KeJson(s)));
        }

        [Test]
        public void FotoIkutTersimpan()
        {
            var s = new GameState();
            PapanBukti.TambahFoto(s, PapanBukti.FotoPakuMalam1);
            var hasil = SaveData.DariJson(SaveData.KeJson(s));
            CollectionAssert.AreEqual(new[] { PapanBukti.FotoPakuMalam1 }, hasil.Foto);
        }

        [Test]
        public void FotoTidakDikenalDiSaveDitolak()
        {
            var s = new GameState();
            s.Foto.Add("Hantu");
            Assert.Throws<FormatException>(() => SaveData.DariJson(SaveData.KeJson(s)));
        }
    }
}
