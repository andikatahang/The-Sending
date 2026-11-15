using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PapanBuktiTests
    {
        [Test]
        public void KatanyaBaruBerlabelKatanya()
        {
            var s = new GameState();
            PapanBukti.TambahKatanya(s, Kode.K1);
            Assert.IsTrue(s.KatanyaDiketahui[Kode.K1]);
            Assert.AreEqual(StatusInfo.Katanya, s.StatusKatanya[Kode.K1]);
        }

        [TestCase(Kode.K1, Kode.B2)]
        [TestCase(Kode.K1, Kode.B3)]
        [TestCase(Kode.K2, Kode.B7)]
        [TestCase(Kode.K3, Kode.B4)]
        [TestCase(Kode.K3, Kode.B5)]
        public void BuktiPembantahMengubahKatanyaMenjadiTerbantahkan(int k, int b)
        {
            var s = new GameState();
            PapanBukti.TambahKatanya(s, k);
            PapanBukti.TambahBukti(s, b);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[k]);
        }

        [Test]
        public void KatanyaYangDidapatSetelahBuktiLangsungTerbantahkan()
        {
            var s = new GameState();
            PapanBukti.TambahBukti(s, Kode.B4);
            PapanBukti.TambahKatanya(s, Kode.K3);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K3]);
        }

        [Test]
        public void K4MenjadiBuktiSetelahB6()
        {
            var s = new GameState();
            PapanBukti.TambahKatanya(s, Kode.K4);
            PapanBukti.TambahBukti(s, Kode.B6);
            Assert.AreEqual(StatusInfo.Bukti, s.StatusKatanya[Kode.K4]);
        }

        [Test]
        public void BuktiYangTidakTerkaitTidakMengubahKatanya()
        {
            var s = new GameState();
            PapanBukti.TambahKatanya(s, Kode.K2);
            PapanBukti.TambahBukti(s, Kode.B3);
            Assert.AreEqual(StatusInfo.Katanya, s.StatusKatanya[Kode.K2]);
        }

        [Test]
        public void B1ButuhHalaman1DanHalaman3()
        {
            var s = new GameState();
            BukuHarian.Temukan(s, Kode.H1);
            Assert.IsFalse(s.BuktiKunci[Kode.B1]);
            BukuHarian.Temukan(s, Kode.H3);
            Assert.IsTrue(s.BuktiKunci[Kode.B1]);
        }

        [Test]
        public void Halaman7AdalahB7()
        {
            var s = new GameState();
            BukuHarian.Temukan(s, Kode.H7);
            Assert.IsTrue(s.BuktiKunci[Kode.B7]);
        }

        [Test]
        public void Halaman4MengajarkanRitualTanpaBahan()
        {
            var s = new GameState();
            BukuHarian.Temukan(s, Kode.H4);
            Assert.IsTrue(s.RitualDiketahui);
            Assert.IsFalse(RitualPenangkal.BahanLengkap(s));
        }

        [Test]
        public void BuktiBaruDiumumkan()
        {
            var s = new GameState();
            bool diumumkan = false;
            s.Berubah += p => diumumkan |= p == Perubahan.Bukti;
            PapanBukti.TambahBukti(s, Kode.B3);
            Assert.IsTrue(diumumkan);
        }

        [Test]
        public void IndeksDiLuarRentangDitolak()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => PapanBukti.TambahBukti(new GameState(), 8));
            Assert.Throws<ArgumentOutOfRangeException>(() => PapanBukti.TambahKatanya(new GameState(), 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => BukuHarian.Temukan(new GameState(), 8));
        }
    }
}
