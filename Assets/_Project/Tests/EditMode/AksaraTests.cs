using System;
using System.Linq;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class AksaraTests
    {
        [Test]
        public void SetiapSukuPunyaEmpatPilihanBerbedaTermasukJawaban()
        {
            foreach (var kata in BacaanAksara.Kata)
                foreach (var suku in kata)
                {
                    var p = AksaraJawa.Pilihan(suku);
                    Assert.AreEqual(4, p.Length, suku);
                    Assert.AreEqual(4, p.Distinct().Count(), suku);
                    CollectionAssert.Contains(p, suku);
                }
        }

        [Test]
        public void PilihanSukuTidakDikenalDitolak()
        {
            Assert.Throws<ArgumentException>(() => AksaraJawa.Pilihan("XA"));
        }

        [Test]
        public void BacaanBenarMenyelesaikanDuaKata()
        {
            var b = new BacaanAksara();
            foreach (var kata in BacaanAksara.Kata)
                foreach (var suku in kata) Assert.IsTrue(b.Pilih(suku), suku);

            Assert.IsTrue(b.Selesai);
            Assert.AreEqual(9, BacaanAksara.JumlahSuku);
            Assert.AreEqual(BacaanAksara.JumlahSuku, b.Terbaca);
            Assert.IsNull(b.SukuSekarang);
            Assert.IsFalse(b.Pilih("RA"));
        }

        [Test]
        public void PilihanSalahTidakMemajukanBacaan()
        {
            var b = new BacaanAksara();
            Assert.IsFalse(b.Pilih("KA"));
            Assert.AreEqual(0, b.Terbaca);
            Assert.AreEqual("RA", b.SukuSekarang);
        }

        [Test]
        public void SetelahRatmiBacaanPindahKeSelasaKliwon()
        {
            var b = new BacaanAksara();
            b.Pilih("RA");
            b.Pilih("T");
            b.Pilih("MI");
            Assert.AreEqual(1, b.KataKe);
            Assert.AreEqual(3, b.Terbaca);
            Assert.AreEqual("SE", b.SukuSekarang);
        }
    }
}
