using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class KondisiIbuTests
    {
        [Test]
        public void NilaiAwalGameStateSamaDenganKonstantaAwal()
        {
            Assert.AreEqual(KondisiIbu.Awal, new GameState().KondisiIbu);
        }

        [Test]
        public void UbahDibatasi0Sampai100()
        {
            var s = new GameState { KondisiIbu = 95 };
            KondisiIbu.Ubah(s, 20);
            Assert.AreEqual(100, s.KondisiIbu);
            KondisiIbu.Ubah(s, -250);
            Assert.AreEqual(0, s.KondisiIbu);
        }

        [Test]
        public void KondisiNolLangsungMenetapkanTerlambat()
        {
            var s = new GameState { MalamKe = 2, KondisiIbu = 5 };
            KondisiIbu.Ubah(s, KondisiIbu.Tertangkap);
            Assert.AreEqual(Ending.Terlambat, s.Ending);
        }

        [Test]
        public void JalurFitnahMenahanKondisiMinimalSatu()
        {
            var s = new GameState { MalamKe = 6, KondisiIbu = 5, HasilSidang = HasilSidang.Gagal };
            KondisiIbu.Ubah(s, KondisiIbu.PergantianMalam);
            Assert.AreEqual(KondisiIbu.MinJalurFitnah, s.KondisiIbu);
            Assert.AreEqual(Ending.Belum, s.Ending);
        }

        [Test]
        public void UbahMengumumkanPerubahanKondisi()
        {
            var s = new GameState();
            bool diumumkan = false;
            s.Berubah += p => diumumkan |= p == Perubahan.Kondisi;
            KondisiIbu.Ubah(s, KondisiIbu.MediaDinetralkan);
            Assert.IsTrue(diumumkan);
            Assert.AreEqual(68, s.KondisiIbu);
        }

        [Test]
        public void PenaltiTertangkapTetapBerlakuSetelahMuatCheckpoint()
        {
            // Desain 4.2: muat checkpoint dulu, baru kurangi 5.
            var checkpoint = SaveData.KeJson(new GameState { MalamKe = 4, KondisiIbu = 40 });
            var s = SaveData.DariJson(checkpoint);
            KondisiIbu.SetelahTertangkap(s);
            Assert.AreEqual(35, s.KondisiIbu);
        }
    }
}
