using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class TujuhPakuTests
    {
        static GameState SakaDitemukan()
        {
            var s = new GameState { MalamKe = 3, KondisiIbu = 40 };
            Assert.IsTrue(TujuhPaku.Ketuk(s, TujuhPaku.SakaBenar));
            return s;
        }

        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, false)]
        [TestCase(3, true)]
        public void HanyaSakaBenarBerbunyiBeronggaDanMenemukanM2(int saka, bool berongga)
        {
            var s = new GameState { MalamKe = 3 };
            Assert.AreEqual(berongga, TujuhPaku.Ketuk(s, saka));
            Assert.AreEqual(berongga, s.MediaDitemukan[Kode.M2]);
        }

        [TestCase(-1)]
        [TestCase(4)]
        public void NomorSakaDiLuarRentangDitolak(int saka)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TujuhPaku.Ketuk(new GameState { MalamKe = 3 }, saka));
        }

        [Test]
        public void UrutanMulaiJumatMenyelesaikanCabutPaku()
        {
            var s = SakaDitemukan();
            var urutan = new[] { Hari.Jumat, Hari.Sabtu, Hari.Minggu, Hari.Senin, Hari.Selasa, Hari.Rabu, Hari.Kamis };
            foreach (var h in urutan)
            {
                Assert.IsFalse(TujuanMalam.Sudah(s, TujuanMalam.CabutPaku), h.ToString());
                Assert.AreEqual(HasilCabut.Benar, TujuhPaku.Cabut(s, h), h.ToString());
                Assert.IsTrue(TujuhPaku.SudahDicabut(s, h), h.ToString());
            }
            Assert.IsTrue(TujuhPaku.Selesai(s));
            Assert.AreEqual(TujuhPaku.Jumlah, s.PakuDicabut);
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.CabutPaku));
            Assert.AreEqual(40, s.KondisiIbu);
        }

        [Test]
        public void SalahUrutanMengurangi5DanKemajuanTetap()
        {
            var s = SakaDitemukan();
            TujuhPaku.Cabut(s, Hari.Jumat);
            Assert.AreEqual(HasilCabut.Salah, TujuhPaku.Cabut(s, Hari.Minggu));
            Assert.AreEqual(35, s.KondisiIbu);
            Assert.AreEqual(1, s.PakuDicabut);
            Assert.IsFalse(TujuhPaku.SudahDicabut(s, Hari.Minggu));
            Assert.AreEqual(HasilCabut.Benar, TujuhPaku.Cabut(s, Hari.Sabtu));
        }

        [Test]
        public void PakuYangSudahDicabutDitolakTanpaPenalti()
        {
            var s = SakaDitemukan();
            TujuhPaku.Cabut(s, Hari.Jumat);
            Assert.AreEqual(HasilCabut.Ditolak, TujuhPaku.Cabut(s, Hari.Jumat));
            Assert.AreEqual(40, s.KondisiIbu);
        }

        // Catatan 0B: cek selesai sebelum memberi penalti.
        [Test]
        public void SetelahSelesaiTidakAdaPenalti()
        {
            var s = SakaDitemukan();
            foreach (var h in KalenderJawa.UrutanPaku(TujuhPaku.MalamPaku)) TujuhPaku.Cabut(s, h);
            foreach (Hari h in Enum.GetValues(typeof(Hari))) Assert.AreEqual(HasilCabut.Ditolak, TujuhPaku.Cabut(s, h));
            Assert.AreEqual(40, s.KondisiIbu);
        }

        [Test]
        public void SebelumSakaDitemukanDitolak()
        {
            var s = new GameState { MalamKe = 3, KondisiIbu = 40 };
            Assert.AreEqual(HasilCabut.Ditolak, TujuhPaku.Cabut(s, Hari.Minggu));
            Assert.AreEqual(40, s.KondisiIbu);
            Assert.AreEqual(0, s.PakuDicabut);
        }
    }
}
