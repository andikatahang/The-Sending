using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class Malam7Tests
    {
        static GameState Malam7(HasilSidang h, float menit) =>
            new GameState { MalamKe = 7, HasilSidang = h, MenitGim = menit, KondisiIbu = 50 };

        [Test]
        public void LajuJamMalam7SetengahLajuBiasa()
        {
            Assert.AreEqual(250f, JamGim.DetikPerJamGim(7));
            var s = Malam7(HasilSidang.Berhasil, JamGim.MulaiMalam);
            JamGim.Maju(s, 250f);
            Assert.AreEqual(19 * 60f, s.MenitGim, 0.01f);
        }

        [Test]
        public void SisaDetikMalam7Dari18Sampai24()
        {
            Assert.AreEqual(1500f, JamGim.SisaDetikMalam7(Malam7(HasilSidang.Berhasil, JamGim.MulaiMalam)), 0.01f);
        }

        [Test]
        public void ObatOtomatisSekaliDanHanyaJalurUtama()
        {
            var s = Malam7(HasilSidang.Berhasil, JamGim.Jam21 - 1f);
            Assert.IsFalse(ObatMalam7.Cek(s));
            s.MenitGim = JamGim.Jam21;
            Assert.IsTrue(ObatMalam7.Cek(s));
            Assert.AreEqual(52, s.KondisiIbu);
            Assert.IsFalse(ObatMalam7.Cek(s));

            Assert.IsFalse(ObatMalam7.Cek(Malam7(HasilSidang.Gagal, JamGim.Jam21 + 30f)));

            var m = Malam7(HasilSidang.Berhasil, JamGim.Jam21 - 1f);
            JamGim.Maju(m, 10f);
            Assert.IsTrue(m.ObatSudahDiberikan);
        }

        [Test]
        public void MajukanDijepitDiTengahMalamDanMemicuEnding()
        {
            var s = Malam7(HasilSidang.Berhasil, 23 * 60 + 50f);
            JamGim.Majukan(s, 15f);
            Assert.AreEqual(1440f, s.MenitGim);
            Assert.AreEqual(Ending.Terlambat, s.Ending);

            var f = Malam7(HasilSidang.Gagal, 23 * 60 + 55f);
            JamGim.Majukan(f, 5f);
            Assert.AreEqual(Ending.Fitnah, f.Ending);

            var n = Malam7(HasilSidang.Berhasil, JamGim.MulaiMalam);
            JamGim.Majukan(n, 0f);
            Assert.AreEqual(JamGim.MulaiMalam, n.MenitGim);
        }

        [Test]
        public void WaktuHabisSetelahLaporTanpaRitualTerlambat()
        {
            var s = Malam7(HasilSidang.Berhasil, JamGim.Jam21);
            PenentuEnding.Pilih(s, PilihanAkhir.Lapor);
            JamGim.Maju(s, 100000f);
            Assert.AreEqual(Ending.Terlambat, s.Ending);
        }

        [Test]
        public void KokokHanyaDekatKandang()
        {
            Assert.IsTrue(KokokCemani.Berkokok(2.9f));
            Assert.IsFalse(KokokCemani.Berkokok(3.1f));
        }
    }
}
