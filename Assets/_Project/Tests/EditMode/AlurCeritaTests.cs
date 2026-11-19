using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class AlurCeritaTests
    {
        static void TutupMalam(GameState s)
        {
            foreach (var id in TujuanMalam.DaftarWajib(s.MalamKe)) TujuanMalam.Selesaikan(s, id);
            PergantianMalam.Lanjut(s);
        }

        static void BeriObatTepatWaktu(GameState s)
        {
            s.MenitGim = s.MalamKe == 6 ? JamGim.Jam20 : JamGim.Jam21;
            Assert.IsTrue(KondisiIbu.BeriObat(s), $"Obat Malam {s.MalamKe}");
        }

        static void Netralkan(GameState s, int media)
        {
            var r = new RitualPenangkal(s, media);
            foreach (LangkahRitual l in System.Enum.GetValues(typeof(LangkahRitual))) r.Lakukan(l);
            Assert.IsTrue(r.Selesai);
        }

        // Jalur wajib sampai sidang berhasil. Mengembalikan state di awal Malam 7.
        static GameState MainSampaiMalam7()
        {
            var s = new GameState();
            TutupMalam(s);                                          // Prolog -> Malam 1 (60)

            BeriObatTepatWaktu(s);                                  // 62
            BukuHarian.Temukan(s, Kode.H1);
            BukuHarian.Temukan(s, Kode.H2);
            TutupMalam(s);                                          // Malam 2 (50)

            BeriObatTepatWaktu(s);                                  // 52
            MediaSantet.Temukan(s, Kode.M1);
            TutupMalam(s);                                          // Malam 3 (40)

            BeriObatTepatWaktu(s);                                  // 42
            Assert.IsTrue(TujuhPaku.Ketuk(s, TujuhPaku.SakaBenar));   // M2
            foreach (var h in KalenderJawa.UrutanPaku(TujuhPaku.MalamPaku)) Assert.AreEqual(HasilCabut.Benar, TujuhPaku.Cabut(s, h));
            BukuHarian.Temukan(s, Kode.H3);                         // B1
            BukuHarian.Temukan(s, Kode.H4);
            KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf);
            KepercayaanSumi.Tambah(s, AksiSumi.BantuLampuDanAir);
            Assert.IsTrue(KepercayaanSumi.BisaAjarkanRitual(s));
            RitualPenangkal.Pelajari(s, dariBuSumi: true);
            Netralkan(s, Kode.M1);                                  // 50
            Netralkan(s, Kode.M2);                                  // 58
            TutupMalam(s);                                          // Malam 4 (46)

            BeriObatTepatWaktu(s);                                  // 48
            PapanBukti.TambahKatanya(s, Kode.K1);
            PapanBukti.TambahKatanya(s, Kode.K2);
            MediaSantet.Temukan(s, Kode.M3);
            PapanBukti.TambahKatanya(s, Kode.K3);
            Netralkan(s, Kode.M3);                                  // 56
            TutupMalam(s);                                          // Malam 5 (44)

            BeriObatTepatWaktu(s);                                  // 46
            MediaSantet.Temukan(s, Kode.M5);
            PapanBukti.TambahBukti(s, Kode.B3);
            PapanBukti.TambahBukti(s, Kode.B4);
            Assert.IsTrue(KepercayaanSumi.BisaBeriKesaksian(s));
            PapanBukti.TambahBukti(s, Kode.B5);
            BukuHarian.Temukan(s, Kode.H7);                         // B7
            Netralkan(s, Kode.M5);                                  // 54
            TutupMalam(s);                                          // Malam 6 (42)

            BeriObatTepatWaktu(s);                                  // 44
            int benar = 0;
            if (SidangWarga.JawabanBenar(s, 0, ItemPapan.Bukti(Kode.B4))) benar++;
            if (SidangWarga.JawabanBenar(s, 1, ItemPapan.Bukti(Kode.B7))) benar++;
            if (SidangWarga.JawabanBenar(s, 2, ItemPapan.Bukti(Kode.B3))) benar++;
            Assert.AreEqual(HasilSidang.Berhasil, SidangWarga.Selesaikan(s, benar));
            TutupMalam(s);                                          // Malam 7 (32)
            return s;
        }

        [Test]
        public void JalurUtamaSampaiEndingSejati()
        {
            var s = MainSampaiMalam7();
            Assert.AreEqual(7, s.MalamKe);
            Assert.AreEqual(32, s.KondisiIbu);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K1]);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K2]);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K3]);

            BeriObatTepatWaktu(s);                                  // Laras memberi obat
            MediaSantet.Temukan(s, Kode.M7);
            PapanBukti.TambahBukti(s, Kode.B8);
            Assert.IsTrue(LilinSesaji.Buat(7).Pilih(Pasaran.Kliwon));
            PenentuEnding.Pilih(s, PilihanAkhir.Lapor);
            Netralkan(s, Kode.M7);

            Assert.AreEqual(Ending.Sejati, s.Ending);
        }

        [Test]
        public void JalurUtamaKehabisanWaktuMenjadiTerlambat()
        {
            var s = MainSampaiMalam7();
            JamGim.Maju(s, 100000f);
            Assert.AreEqual(Ending.Terlambat, s.Ending);
        }

        [Test]
        public void JalurBalasDendam()
        {
            var s = MainSampaiMalam7();
            MediaSantet.Temukan(s, Kode.M7);
            PenentuEnding.Pilih(s, PilihanAkhir.Balas);
            PenentuEnding.LolosDariWiryo(s);
            Assert.AreEqual(Ending.BalasDendam, s.Ending);
        }

        [Test]
        public void SidangDiamMenujuFitnahDanIbuBertahanSampaiTengahMalam()
        {
            var s = new GameState { MalamKe = 6, KondisiIbu = 6 };
            SidangWarga.Diam(s);
            TutupMalam(s);                                          // -12 ditahan menjadi 1
            Assert.AreEqual(7, s.MalamKe);
            Assert.AreEqual(1, s.KondisiIbu);
            Assert.AreEqual(Ending.Belum, s.Ending);

            KondisiIbu.SetelahTertangkap(s);
            Assert.AreEqual(1, s.KondisiIbu);

            JamGim.Maju(s, 100000f);
            Assert.AreEqual(Ending.Fitnah, s.Ending);
        }

        [Test]
        public void SaveDiTengahJalanBisaDilanjutkan()
        {
            var s = MainSampaiMalam7();
            var muat = SaveData.DariJson(SaveData.KeJson(s));
            Assert.AreEqual(HasilSidang.Berhasil, muat.HasilSidang);
            Assert.AreEqual(s.KondisiIbu, muat.KondisiIbu);
            Assert.IsTrue(muat.MediaDinetralkan[Kode.M5]);
        }
    }
}
