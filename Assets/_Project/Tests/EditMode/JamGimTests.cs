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

        // K4 opsi B: duduk menunggu di kursi samping ranjang Ibu (Malam 1).
        [TestCase(19 * 60f)]
        [TestCase(20 * 60f + 59f)]
        public void Malam1SebelumObatMenungguSampai21(float menit)
        {
            var s = new GameState { MalamKe = 1, MenitGim = menit };
            Assert.AreEqual(JamGim.Jam21, JamGim.KejadianBerikutnya(s));
            Assert.IsTrue(JamGim.Tunggu(s));
            Assert.AreEqual(JamGim.Jam21, s.MenitGim);
        }

        [Test]
        public void TidakBisaMenungguMelewatiObatYangBelumDiberikan()
        {
            var s = new GameState { MalamKe = 1, MenitGim = JamGim.Jam21 + 10f };
            Assert.IsNull(JamGim.KejadianBerikutnya(s));
            Assert.IsFalse(JamGim.Tunggu(s));
            Assert.AreEqual(JamGim.Jam21 + 10f, s.MenitGim);
        }

        [Test]
        public void SetelahObatDiberikanMenungguSampai23()
        {
            var s = new GameState { MalamKe = 1, MenitGim = JamGim.Jam21 + 10f, ObatSudahDiberikan = true };
            bool jamDiumumkan = false;
            s.Berubah += p => jamDiumumkan |= p == Perubahan.Jam;
            Assert.IsTrue(JamGim.Tunggu(s));
            Assert.AreEqual(JamGim.Jam23, s.MenitGim);
            Assert.IsTrue(jamDiumumkan);
        }

        [Test]
        public void JendelaObatSudahLewatMenungguSampai23()
        {
            var s = new GameState { MalamKe = 1, MenitGim = JamGim.Jam22 + 5f };
            Assert.AreEqual(JamGim.Jam23, JamGim.KejadianBerikutnya(s));
        }

        [Test]
        public void TidakAdaKejadianSetelahCutscene23AtauSaatTibaWaktunya()
        {
            var sudah = new GameState { MalamKe = 1, MenitGim = JamGim.Jam22, ObatSudahDiberikan = true };
            TujuanMalam.Selesaikan(sudah, TujuanMalam.Cutscene23);
            var tiba = new GameState { MalamKe = 1, MenitGim = JamGim.Jam23, ObatSudahDiberikan = true };
            Assert.IsNull(JamGim.KejadianBerikutnya(sudah));
            Assert.IsNull(JamGim.KejadianBerikutnya(tiba));
        }

        [TestCase(0)]
        [TestCase(2)]
        public void MenungguHanyaDiMalam1(int malam)
        {
            var s = new GameState { MalamKe = malam, MenitGim = 19 * 60f };
            Assert.IsNull(JamGim.KejadianBerikutnya(s));
            Assert.IsFalse(JamGim.Tunggu(s));
        }

        [Test]
        public void Malam2SebelumAksaraTidakAdaYangDitunggu()
        {
            var s = new GameState { MalamKe = 2, MenitGim = JamGim.MulaiMalam };
            Assert.IsNull(JamGim.KejadianBerikutnya(s));
        }

        [Test]
        public void Malam2SetelahAksaraMenungguJamObat()
        {
            var s = new GameState { MalamKe = 2, MenitGim = 19 * 60f };
            TujuanMalam.Selesaikan(s, TujuanMalam.BacaAksara);
            Assert.AreEqual(JamGim.Jam21, JamGim.KejadianBerikutnya(s));
        }

        [Test]
        public void Malam2DalamJendelaObatTidakMelompat()
        {
            var s = new GameState { MalamKe = 2, MenitGim = JamGim.Jam21 + 10f };
            TujuanMalam.Selesaikan(s, TujuanMalam.BacaAksara);
            Assert.IsNull(JamGim.KejadianBerikutnya(s));
        }

        [Test]
        public void Malam2SetelahPilihanWiryoTidakMenunggu()
        {
            var s = new GameState { MalamKe = 2, MenitGim = 19 * 60f };
            TujuanMalam.Selesaikan(s, TujuanMalam.BacaAksara);
            TujuanMalam.Selesaikan(s, TujuanMalam.PilihanWiryo);
            Assert.IsNull(JamGim.KejadianBerikutnya(s));
        }

        [TestCase(false, false, 1350f, false)]   // 22.30, aksara belum terbaca
        [TestCase(true, false, 1200f, false)]    // 20.00, menunggu jam obat
        [TestCase(true, true, 1265f, true)]      // obat sudah diberikan
        [TestCase(true, false, 1320f, true)]     // 22.00, jam obat lewat tanpa obat
        public void WiryoDatangSetelahAksaraDanObat(bool aksara, bool obat, float menit, bool datang)
        {
            var s = new GameState { MalamKe = 2, MenitGim = menit, ObatSudahDiberikan = obat };
            if (aksara) TujuanMalam.Selesaikan(s, TujuanMalam.BacaAksara);
            Assert.AreEqual(datang, JamGim.SaatnyaWiryoDatang(s));
        }

        [Test]
        public void WiryoTidakDatangLagiSetelahPilihanWiryo()
        {
            var s = new GameState { MalamKe = 2, MenitGim = 1320f, ObatSudahDiberikan = true };
            TujuanMalam.Selesaikan(s, TujuanMalam.BacaAksara);
            TujuanMalam.Selesaikan(s, TujuanMalam.PilihanWiryo);
            Assert.IsFalse(JamGim.SaatnyaWiryoDatang(s));
        }

        // Malam 3: Ibu menjerit setiap jam dinding berdentang.
        [TestCase(1139f, 1141f, true)]    // 18.59 -> 19.01
        [TestCase(1141f, 1199f, false)]
        [TestCase(1199f, 1260f, true)]    // kursi tunggu melompat lewat 20.00
        [TestCase(1200f, 1200f, false)]
        public void JamBerdentangSetiapJamPenuh(float sebelum, float sesudah, bool dentang)
        {
            Assert.AreEqual(dentang, JamGim.JamBerdentang(sebelum, sesudah));
        }

        // K19-A: Malam 3 tidak ditutup sebelum obat diberikan atau jendela obat lewat.
        [TestCase(false, 1200f, false)]   // 20.00, belum waktunya
        [TestCase(true, 1265f, true)]     // obat sudah diberikan
        [TestCase(false, 1320f, true)]    // 22.00, jendela lewat
        public void ObatSelesaiSetelahDiberikanAtauJendelaLewat(bool obat, float menit, bool selesai)
        {
            var s = new GameState { MalamKe = 3, MenitGim = menit, ObatSudahDiberikan = obat };
            Assert.AreEqual(selesai, JamGim.ObatSelesai(s));
        }

        static GameState Malam3TujuanSelesai(float menit)
        {
            var s = new GameState { MalamKe = 3, MenitGim = menit };
            foreach (var id in TujuanMalam.DaftarWajib(3)) TujuanMalam.Selesaikan(s, id);
            return s;
        }

        [Test]
        public void Malam3SebelumTujuanSelesaiTidakMenunggu()
        {
            Assert.IsNull(JamGim.KejadianBerikutnya(new GameState { MalamKe = 3, MenitGim = 19 * 60f }));
        }

        [Test]
        public void Malam3SetelahTujuanSelesaiMenungguJamObat()
        {
            Assert.AreEqual(JamGim.Jam21, JamGim.KejadianBerikutnya(Malam3TujuanSelesai(19 * 60f)));
        }

        [Test]
        public void Malam3DalamJendelaObatTidakMelompat()
        {
            Assert.IsNull(JamGim.KejadianBerikutnya(Malam3TujuanSelesai(JamGim.Jam21 + 10f)));
        }
    }
}
