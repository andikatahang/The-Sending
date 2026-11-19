using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class MalamTests
    {
        static void SelesaikanSemua(GameState s)
        {
            foreach (var id in TujuanMalam.DaftarWajib(s.MalamKe)) TujuanMalam.Selesaikan(s, id);
        }

        [Test]
        public void Malam1TidakSelesaiSebelumCutscene23()
        {
            var s = new GameState { MalamKe = 1 };
            TujuanMalam.Selesaikan(s, TujuanMalam.BukaLemari);
            TujuanMalam.Selesaikan(s, TujuanMalam.PotretPaku);
            Assert.IsFalse(TujuanMalam.MalamSelesai(s));
            TujuanMalam.Selesaikan(s, TujuanMalam.Cutscene23);
            Assert.IsTrue(TujuanMalam.MalamSelesai(s));
        }

        [Test]
        public void SetiapMalam0Sampai6PunyaTujuanWajib()
        {
            for (int malam = 0; malam <= 6; malam++)
                Assert.IsNotEmpty(TujuanMalam.DaftarWajib(malam), $"Malam {malam}");
        }

        [Test]
        public void Malam3ButuhEmpatTujuanWajib()
        {
            CollectionAssert.AreEqual(
                new[] { TujuanMalam.PakdeDatang, TujuanMalam.CabutPaku, TujuanMalam.KunjungiSumi, TujuanMalam.RitualPertama },
                TujuanMalam.DaftarWajib(3));
        }

        [Test]
        public void Malam7TidakPernahSelesaiLewatTujuan()
        {
            Assert.IsFalse(TujuanMalam.MalamSelesai(new GameState { MalamKe = 7 }));
        }

        [Test]
        public void TujuanYangSamaTidakDicatatDuaKali()
        {
            var s = new GameState();
            TujuanMalam.Selesaikan(s, TujuanMalam.TibaDiRumah);
            TujuanMalam.Selesaikan(s, TujuanMalam.TibaDiRumah);
            Assert.AreEqual(1, s.TujuanSelesai.Count);
        }

        [Test]
        public void LanjutSebelumTujuanSelesaiDitolak()
        {
            Assert.Throws<InvalidOperationException>(() => PergantianMalam.Lanjut(new GameState()));
        }

        [Test]
        public void PrologKeMalam1TanpaPenguranganDanJamBerlanjut()
        {
            var s = new GameState { MenitGim = 1110f };
            SelesaikanSemua(s);
            PergantianMalam.Lanjut(s);
            Assert.AreEqual(1, s.MalamKe);
            Assert.AreEqual(60, s.KondisiIbu);
            Assert.AreEqual(1110f, s.MenitGim);
            Assert.IsEmpty(s.TujuanSelesai);
        }

        [Test]
        public void PergantianMalamMengurangi12DanMeresetJamDanObat()
        {
            var s = new GameState { MalamKe = 1, MenitGim = JamGim.BatasLunak, ObatSudahDiberikan = true };
            SelesaikanSemua(s);
            PergantianMalam.Lanjut(s);
            Assert.AreEqual(2, s.MalamKe);
            Assert.AreEqual(48, s.KondisiIbu);
            Assert.AreEqual(JamGim.MulaiMalam, s.MenitGim);
            Assert.IsFalse(s.ObatSudahDiberikan);
        }

        [Test]
        public void PergantianMalamBisaMemicuTerlambat()
        {
            var s = new GameState { MalamKe = 3, KondisiIbu = 10 };
            SelesaikanSemua(s);
            PergantianMalam.Lanjut(s);
            Assert.AreEqual(Ending.Terlambat, s.Ending);
        }
    }
}
