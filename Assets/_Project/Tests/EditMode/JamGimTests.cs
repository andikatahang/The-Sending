using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class JamGimTests
    {
        [Test]
        public void NilaiAwalGameStateSamaDenganMulaiPrologDanTengahMalamSama()
        {
            Assert.AreEqual(JamGim.MulaiProlog, new GameState().MenitGim);
            Assert.AreEqual(PenentuEnding.TengahMalam, JamGim.TengahMalam);
        }

        [Test]
        public void SatuJamGimSama250DetikNyata()
        {
            var s = new GameState { MalamKe = 3, MenitGim = JamGim.MulaiMalam };
            JamGim.Maju(s, 250f);
            Assert.AreEqual(19 * 60f, s.MenitGim, 0.001f);
        }

        [TestCase(1, 1470f)]
        [TestCase(5, 1470f)]
        [TestCase(6, 1260f)]
        [TestCase(7, 1440f)]
        public void JamBerhentiDiBatasSetiapMalam(int malam, float batas)
        {
            var s = new GameState { MalamKe = malam, MenitGim = JamGim.MulaiMalam };
            JamGim.Maju(s, 100000f);
            Assert.AreEqual(batas, s.MenitGim);
        }

        [Test]
        public void BatasKerasHanyaMalam6Dan7()
        {
            var malam5 = new GameState { MalamKe = 5, MenitGim = JamGim.BatasLunak };
            var malam6 = new GameState { MalamKe = 6, MenitGim = JamGim.Jam21 };
            Assert.IsFalse(JamGim.BatasKerasTercapai(malam5));
            Assert.IsTrue(JamGim.BatasKerasTercapai(malam6));
        }

        [Test]
        public void WaktuHabisMalam7MemicuEnding()
        {
            var s = new GameState { MalamKe = 7, MenitGim = JamGim.Jam23, HasilSidang = HasilSidang.Berhasil };
            JamGim.Maju(s, 100000f);
            Assert.AreEqual(Ending.Terlambat, s.Ending);
        }

        [Test]
        public void SisaWaktuMalam7DalamDetikNyata()
        {
            var s = new GameState { MalamKe = 7, MenitGim = JamGim.Jam23 };
            Assert.AreEqual(250f, JamGim.SisaDetikMalam7(s), 0.001f);
        }

        [TestCase(1260f, "21.00")]
        [TestCase(1050f, "17.30")]
        [TestCase(1470f, "00.30")]
        public void FormatJam(float menit, string teks)
        {
            Assert.AreEqual(teks, JamGim.Format(menit));
        }

        [TestCase(1, 1259f, false)]
        [TestCase(1, 1260f, true)]
        [TestCase(1, 1319f, true)]
        [TestCase(1, 1320f, false)]
        [TestCase(6, 1200f, true)]
        [TestCase(6, 1260f, false)]
        [TestCase(0, 1260f, false)]
        public void JendelaObat(int malam, float menit, bool boleh)
        {
            Assert.AreEqual(boleh, JamGim.DalamJendelaObat(new GameState { MalamKe = malam, MenitGim = menit }));
        }

        [Test]
        public void ObatMenambah2HanyaSekaliSemalam()
        {
            var s = new GameState { MalamKe = 2, MenitGim = JamGim.Jam21, KondisiIbu = 40 };
            Assert.IsTrue(KondisiIbu.BeriObat(s));
            Assert.IsFalse(KondisiIbu.BeriObat(s));
            Assert.AreEqual(42, s.KondisiIbu);
        }

        [Test]
        public void ObatDiLuarJendelaTidakMenambah()
        {
            var s = new GameState { MalamKe = 2, MenitGim = JamGim.Jam22, KondisiIbu = 40 };
            Assert.IsFalse(KondisiIbu.BeriObat(s));
            Assert.AreEqual(40, s.KondisiIbu);
        }

        [TestCase(1, 1260f, 1320f)]
        [TestCase(5, 1260f, 1320f)]
        [TestCase(6, 1200f, 1260f)]
        public void MulaiDanAkhirJendelaObat(int malam, float mulai, float akhir)
        {
            Assert.AreEqual(mulai, JamGim.MulaiJendelaObat(malam));
            Assert.AreEqual(akhir, JamGim.AkhirJendelaObat(malam));
            Assert.IsTrue(JamGim.DalamJendelaObat(new GameState { MalamKe = malam, MenitGim = mulai }));
            Assert.IsFalse(JamGim.DalamJendelaObat(new GameState { MalamKe = malam, MenitGim = akhir }));
        }
    }
}
