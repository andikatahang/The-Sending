using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PerilakuSantetTests
    {
        static Pengamatan Jalan(float jarak) => new Pengamatan { Jarak = jarak, Gerak = Gerak.Lari };   // langkah yang terdengar = lari
        static Pengamatan Senyap(float jarak) => new Pengamatan { Jarak = jarak, Gerak = Gerak.Diam, TahanNapas = true };

        [Test]
        public void MulaiDariPatroli()
        {
            Assert.AreEqual(KeadaanSantet.Patroli, new PerilakuSantet().Keadaan);
        }

        [Test]
        public void LangkahTerdengarDariJauhMembuatSelidiki()
        {
            var p = new PerilakuSantet();
            Assert.IsFalse(p.Tick(0.1f, Jalan(3.5f)));
            Assert.AreEqual(KeadaanSantet.Selidiki, p.Keadaan);
        }

        [Test]
        public void LangkahTerdengarDariDekatMembuatKejar()
        {
            var p = new PerilakuSantet();
            Assert.IsFalse(p.Tick(0.1f, Jalan(2.5f)));
            Assert.AreEqual(KeadaanSantet.Kejar, p.Keadaan);
        }

        [Test]
        public void TerdengarDalamJarakTangkapBerartiTertangkap()
        {
            Assert.IsTrue(new PerilakuSantet().Tick(0.1f, Jalan(0.8f)));
        }

        [Test]
        public void SaatMengejarPemainTerjangkauTertangkapWalauMenahanNapas()
        {
            var p = new PerilakuSantet();
            p.Tick(0.1f, Jalan(2.5f));
            Assert.IsTrue(p.Tick(0.1f, Senyap(0.8f)));
        }

        [Test]
        public void DiamMenahanNapasTidakKetahuanWalauDekat()
        {
            var p = new PerilakuSantet();
            Assert.IsFalse(p.Tick(0.1f, Senyap(0.5f)));
            Assert.AreEqual(KeadaanSantet.Patroli, p.Keadaan);
        }

        [Test]
        public void BersembunyiTanpaMenahanNapasKetahuanDariDekat()
        {
            var p = new PerilakuSantet();
            var sembunyi = new Pengamatan { Jarak = 1.2f, Gerak = Gerak.Jalan, Bersembunyi = true };
            Assert.IsFalse(p.Tick(0.1f, sembunyi));
            Assert.AreEqual(KeadaanSantet.Selidiki, p.Keadaan);   // bukan Kejar: pemain bersembunyi

            sembunyi.Jarak = 0.9f;
            Assert.IsTrue(p.Tick(0.1f, sembunyi));
        }

        [Test]
        public void BersembunyiSambilMenahanNapasAman()
        {
            var p = new PerilakuSantet();
            Assert.IsFalse(p.Tick(0.1f, new Pengamatan { Jarak = 0.5f, Bersembunyi = true, TahanNapas = true }));
            Assert.AreEqual(KeadaanSantet.Patroli, p.Keadaan);
        }

        [Test]
        public void SenterYangMengenaiDariJauhMembuatSelidiki()
        {
            var p = new PerilakuSantet();
            var sorot = Senyap(10f);
            sorot.SenterMenyala = true;
            sorot.SenterMengarah = true;
            Assert.IsFalse(p.Tick(0.1f, sorot));
            Assert.AreEqual(KeadaanSantet.Selidiki, p.Keadaan);
        }

        [Test]
        public void SelidikiKembaliKePatroliSetelahLamaSelidiki()
        {
            var p = new PerilakuSantet();
            p.Tick(0.1f, Jalan(3.5f));
            p.Tick(PerilakuSantet.LamaSelidiki + 0.1f, Senyap(20f));
            Assert.AreEqual(KeadaanSantet.Patroli, p.Keadaan);
        }

        [Test]
        public void KejarBerubahSelidikiSaatPemainBersembunyi()
        {
            var p = new PerilakuSantet();
            p.Tick(0.1f, Jalan(2.5f));
            p.Tick(0.1f, new Pengamatan { Jarak = 2f, Bersembunyi = true, TahanNapas = true });
            Assert.AreEqual(KeadaanSantet.Selidiki, p.Keadaan);
        }

        [Test]
        public void KejarBerubahSelidikiSaatPemainTerlaluJauh()
        {
            var p = new PerilakuSantet();
            p.Tick(0.1f, Jalan(2.5f));
            p.Tick(0.1f, Senyap(PerilakuSantet.JarakLepas + 1f));
            Assert.AreEqual(KeadaanSantet.Selidiki, p.Keadaan);
        }
    }
}
