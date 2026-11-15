using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PenentuEndingTests
    {
        static GameState Malam7(HasilSidang sidang, float menit = 1300f) =>
            new GameState { MalamKe = 7, HasilSidang = sidang, MenitGim = menit, KondisiIbu = 30 };

        [Test]
        public void StateAwalBelumAdaEnding()
        {
            Assert.AreEqual(Ending.Belum, PenentuEnding.Hitung(new GameState()));
        }

        [Test]
        public void KondisiNolSebelumSidangAdalahTerlambat()
        {
            var s = new GameState { MalamKe = 4, KondisiIbu = 0 };
            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Hitung(s));
        }

        [TestCase(HasilSidang.Gagal)]
        [TestCase(HasilSidang.Diam)]
        [TestCase(HasilSidang.Menuduh)]
        public void JalurFitnahBelumBerakhirSebelumTengahMalam(HasilSidang sidang)
        {
            Assert.AreEqual(Ending.Belum, PenentuEnding.Hitung(Malam7(sidang)));
        }

        [TestCase(HasilSidang.Gagal)]
        [TestCase(HasilSidang.Diam)]
        [TestCase(HasilSidang.Menuduh)]
        public void JalurFitnahBerakhirFitnahTepatTengahMalam(HasilSidang sidang)
        {
            Assert.AreEqual(Ending.Fitnah, PenentuEnding.Hitung(Malam7(sidang, PenentuEnding.TengahMalam)));
        }

        [Test]
        public void JalurFitnahTidakPernahMenjadiTerlambatWalauKondisiNol()
        {
            var s = Malam7(HasilSidang.Gagal, PenentuEnding.TengahMalam);
            s.KondisiIbu = 0;
            Assert.AreEqual(Ending.Fitnah, PenentuEnding.Hitung(s));
        }

        [Test]
        public void SidangGagalDiMalam6BelumBerakhir()
        {
            var s = new GameState { MalamKe = 6, HasilSidang = HasilSidang.Gagal, KondisiIbu = 0 };
            Assert.AreEqual(Ending.Belum, PenentuEnding.Hitung(s));
        }

        [Test]
        public void KondisiNolSetelahSidangBerhasilAdalahTerlambat()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.KondisiIbu = 0;
            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Hitung(s));
        }

        [Test]
        public void WaktuHabisDiJalurUtamaTanpaRitualAdalahTerlambat()
        {
            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Hitung(Malam7(HasilSidang.Berhasil, PenentuEnding.TengahMalam)));
        }

        [Test]
        public void BalasDanLolosSebelumTengahMalamAdalahBalasDendam()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.PilihanAkhir = PilihanAkhir.Balas;
            s.LolosKejaranWiryo = true;
            Assert.AreEqual(Ending.BalasDendam, PenentuEnding.Hitung(s));
        }

        [Test]
        public void BalasTetapiWaktuHabisSebelumLolosAdalahTerlambat()
        {
            var s = Malam7(HasilSidang.Berhasil, PenentuEnding.TengahMalam);
            s.PilihanAkhir = PilihanAkhir.Balas;
            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Hitung(s));
        }

        [Test]
        public void BalasBelumLolosSebelumTengahMalamBelumBerakhir()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.PilihanAkhir = PilihanAkhir.Balas;
            Assert.AreEqual(Ending.Belum, PenentuEnding.Hitung(s));
        }

        [Test]
        public void LaporDanRitualSelesaiSebelumTengahMalamAdalahSejati()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.PilihanAkhir = PilihanAkhir.Lapor;
            s.RitualTerakhirSelesai = true;
            Assert.AreEqual(Ending.Sejati, PenentuEnding.Hitung(s));
        }

        [Test]
        public void CekMenyimpanEndingDanMengumumkanSekali()
        {
            var s = new GameState { MalamKe = 3, KondisiIbu = 0 };
            int jumlah = 0;
            s.Berubah += p => { if (p == Perubahan.Ending) jumlah++; };

            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Cek(s));
            PenentuEnding.Cek(s);

            Assert.AreEqual(Ending.Terlambat, s.Ending);
            Assert.AreEqual(1, jumlah);
        }

        [Test]
        public void EndingYangSudahDitetapkanTidakBerubah()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.PilihanAkhir = PilihanAkhir.Lapor;
            s.RitualTerakhirSelesai = true;
            PenentuEnding.Cek(s);

            s.MenitGim = PenentuEnding.TengahMalam;
            s.KondisiIbu = 0;

            Assert.AreEqual(Ending.Sejati, PenentuEnding.Cek(s));
        }
    }
}
