using System;
using System.Collections.Generic;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class JalannyaSidangTests
    {
        static GameState Malam6(params int[] bukti)
        {
            var s = new GameState { MalamKe = 6, MenitGim = JamGim.Jam21 };
            foreach (var b in bukti) s.BuktiKunci[b] = true;
            return s;
        }

        [Test]
        public void DuaBenarDariTigaBerhasilDanTujuanSelesai()
        {
            var s = Malam6(Kode.B4, Kode.B3);
            var j = new JalannyaSidang(s);
            Assert.IsTrue(j.Jawab(ItemPapan.Bukti(Kode.B4)));
            Assert.IsFalse(j.Jawab(ItemPapan.Bukti(Kode.B3)));   // B3 bukan bukti tuduhan kedua
            Assert.IsTrue(j.Jawab(ItemPapan.Bukti(Kode.B3)));
            Assert.AreEqual(2, j.Benar);
            Assert.IsTrue(j.Selesai);
            Assert.AreEqual(HasilSidang.Berhasil, s.HasilSidang);
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.SidangWarga));
        }

        [Test]
        public void SatuBenarGagalDanMasukJalurFitnah()
        {
            var s = Malam6(Kode.B4);
            var j = new JalannyaSidang(s);
            j.Jawab(ItemPapan.Bukti(Kode.B4));
            j.Jawab(ItemPapan.Katanya(Kode.K1));
            j.Jawab(ItemPapan.Katanya(Kode.K1));
            Assert.AreEqual(HasilSidang.Gagal, s.HasilSidang);
            Assert.IsTrue(s.JalurFitnah);
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.SidangWarga));
        }

        // CL Fase 0B: tujuan SidangWarga hanya ditandai setelah hasil ditetapkan.
        [Test]
        public void HasilBelumDitetapkanSebelumTuduhanKetiga()
        {
            var s = Malam6(Kode.B4, Kode.B7);
            var j = new JalannyaSidang(s);
            j.Jawab(ItemPapan.Bukti(Kode.B4));
            j.Jawab(ItemPapan.Bukti(Kode.B7));
            Assert.AreEqual(2, j.Tuduhan);
            Assert.IsFalse(j.Selesai);
            Assert.AreEqual(HasilSidang.Belum, s.HasilSidang);
            Assert.IsFalse(TujuanMalam.Sudah(s, TujuanMalam.SidangWarga));
        }

        [Test]
        public void DiamKapanPunLangsungGagal()
        {
            var s = Malam6(Kode.B4);
            var j = new JalannyaSidang(s);
            j.Jawab(ItemPapan.Bukti(Kode.B4));
            j.Diam();
            Assert.AreEqual(HasilSidang.Diam, s.HasilSidang);
            Assert.IsTrue(j.Selesai);
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.SidangWarga));
        }

        [Test]
        public void IkutMenuduhLangsungGagal()
        {
            var s = Malam6();
            new JalannyaSidang(s).IkutMenuduh();
            Assert.AreEqual(HasilSidang.Menuduh, s.HasilSidang);
            Assert.IsTrue(s.JalurFitnah);
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.SidangWarga));
        }

        [Test]
        public void TidakBisaMenjawabSetelahSelesai()
        {
            var s = Malam6(Kode.B4);
            var j = new JalannyaSidang(s);
            j.Diam();
            Assert.Throws<InvalidOperationException>(() => j.Jawab(ItemPapan.Bukti(Kode.B4)));
            Assert.Throws<InvalidOperationException>(() => new JalannyaSidang(s));
        }

        [Test]
        public void PilihanJawabanBuktiLaluKatanyaYangDiketahui()
        {
            var s = Malam6(Kode.B5, Kode.B2);
            s.KatanyaDiketahui[Kode.K3] = true;
            s.KatanyaDiketahui[Kode.K1] = true;
            CollectionAssert.AreEqual(
                new List<ItemPapan> { ItemPapan.Bukti(Kode.B2), ItemPapan.Bukti(Kode.B5), ItemPapan.Katanya(Kode.K1), ItemPapan.Katanya(Kode.K3) },
                JalannyaSidang.PilihanJawaban(s));
        }
    }
}
