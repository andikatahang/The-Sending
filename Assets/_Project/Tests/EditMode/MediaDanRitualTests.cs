using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class MediaDanRitualTests
    {
        static readonly LangkahRitual[] UrutanBenar =
        {
            LangkahRitual.LetakkanMedia, LangkahRitual.TaburGaram, LangkahRitual.TutupKelor,
            LangkahRitual.NyalakanApi, LangkahRitual.AdukBambu, LangkahRitual.KuburAbu,
        };

        static GameState SiapRitual(int media)
        {
            var s = new GameState { MalamKe = 3, KondisiIbu = 40 };
            MediaSantet.Temukan(s, media);
            RitualPenangkal.Pelajari(s, dariBuSumi: true);
            return s;
        }

        [Test]
        public void TemukanMencatatMalamPenemuan()
        {
            var s = new GameState { MalamKe = 2 };
            MediaSantet.Temukan(s, Kode.M1);
            Assert.IsTrue(s.MediaDitemukan[Kode.M1]);
            Assert.AreEqual(2, s.MalamMediaDitemukan[Kode.M1]);
        }

        [Test]
        public void MenyerahkanMediaMengurangi8DanTidakBisaDinetralkan()
        {
            var s = new GameState { MalamKe = 2, KondisiIbu = 48 };
            MediaSantet.Temukan(s, Kode.M1);
            MediaSantet.Serahkan(s, Kode.M1);
            Assert.AreEqual(40, s.KondisiIbu);
            Assert.IsFalse(MediaSantet.BisaDinetralkan(s, Kode.M1));
            Assert.Throws<InvalidOperationException>(() => MediaSantet.Netralkan(s, Kode.M1));
        }

        [Test]
        public void MediaYangBelumDitemukanTidakBisaDiserahkan()
        {
            Assert.Throws<InvalidOperationException>(() => MediaSantet.Serahkan(new GameState(), Kode.M1));
        }

        [Test]
        public void IndeksMediaDiLuarRentangDitolak()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => MediaSantet.Temukan(new GameState(), 7));
        }

        [Test]
        public void RitualDariHalaman4TanpaBahanBelumBisaDimulai()
        {
            var s = new GameState { MalamKe = 3 };
            MediaSantet.Temukan(s, Kode.M2);
            RitualPenangkal.Pelajari(s, dariBuSumi: false);
            Assert.IsFalse(RitualPenangkal.BisaMulai(s, Kode.M2));

            RitualPenangkal.AmbilBahan(s, BahanRitual.Garam);
            RitualPenangkal.AmbilBahan(s, BahanRitual.Kelor);
            RitualPenangkal.AmbilBahan(s, BahanRitual.BambuKuning);
            Assert.IsTrue(RitualPenangkal.BisaMulai(s, Kode.M2));
        }

        [Test]
        public void RitualTanpaPengetahuanDitolak()
        {
            var s = new GameState();
            MediaSantet.Temukan(s, Kode.M1);
            Assert.Throws<InvalidOperationException>(() => new RitualPenangkal(s, Kode.M1));
        }

        [Test]
        public void RitualUrutanBenarMenetralkanMediaDanMenambah8()
        {
            var s = SiapRitual(Kode.M1);
            var r = new RitualPenangkal(s, Kode.M1);
            foreach (var langkah in UrutanBenar) Assert.IsTrue(r.Lakukan(langkah));
            Assert.IsTrue(r.Selesai);
            Assert.IsTrue(s.MediaDinetralkan[Kode.M1]);
            Assert.AreEqual(48, s.KondisiIbu);
        }

        [Test]
        public void RitualSalahUrutanGagalDanHarusDiulang()
        {
            var s = SiapRitual(Kode.M1);
            var r = new RitualPenangkal(s, Kode.M1);
            r.Lakukan(LangkahRitual.LetakkanMedia);
            Assert.IsFalse(r.Lakukan(LangkahRitual.NyalakanApi));
            Assert.AreEqual(0, r.Kemajuan);
            Assert.IsFalse(s.MediaDinetralkan[Kode.M1]);
        }

        [Test]
        public void MediaYangSudahDinetralkanTidakMenambahLagi()
        {
            var s = SiapRitual(Kode.M1);
            MediaSantet.Netralkan(s, Kode.M1);
            Assert.IsFalse(RitualPenangkal.BisaMulai(s, Kode.M1));
            Assert.Throws<InvalidOperationException>(() => MediaSantet.Netralkan(s, Kode.M1));
        }

        [Test]
        public void MenetralkanKendiIndukMenandaiRitualTerakhirSelesai()
        {
            var s = SiapRitual(Kode.M7);
            MediaSantet.Netralkan(s, Kode.M7);
            Assert.IsTrue(s.RitualTerakhirSelesai);
        }
    }
}
