using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class KewaspadaanSantetTests
    {
        [Test]
        public void DiamSambilMenahanNapasTidakBersuara()
        {
            Assert.AreEqual(0f, KewaspadaanSantet.RadiusSuara(Gerak.Diam, tahanNapas: true, minyakSereh: false));
            Assert.IsFalse(KewaspadaanSantet.Terdengar(0.5f, Gerak.Diam, true, false));
        }

        [Test]
        public void BerjalanTidakMemanggilSosokKecualiSangatDekat()
        {
            Assert.IsFalse(KewaspadaanSantet.Terdengar(3f, Gerak.Jalan, false, false));
            Assert.IsTrue(KewaspadaanSantet.Terdengar(1.2f, Gerak.Jalan, false, false));
            Assert.IsTrue(KewaspadaanSantet.Terdengar(7f, Gerak.Lari, false, false));
        }

        [Test]
        public void LariTerdengarLebihJauhDariJalan()
        {
            Assert.IsTrue(KewaspadaanSantet.Terdengar(7f, Gerak.Lari, false, false));
            Assert.IsFalse(KewaspadaanSantet.Terdengar(7f, Gerak.Jalan, false, false));
        }

        [Test]
        public void MinyakSerehMemperkecilRadiusLangkah()
        {
            Assert.AreEqual(4.8f, KewaspadaanSantet.RadiusSuara(Gerak.Lari, false, minyakSereh: true), 0.001f);
            Assert.IsFalse(KewaspadaanSantet.Terdengar(6f, Gerak.Lari, false, true));
        }

        [Test]
        public void CahayaHanyaTerlihatJikaSenterMenyalaDanMengarah()
        {
            Assert.IsTrue(KewaspadaanSantet.CahayaTerlihat(5f, true, true));
            Assert.IsFalse(KewaspadaanSantet.CahayaTerlihat(5f, false, true));
            Assert.IsFalse(KewaspadaanSantet.CahayaTerlihat(5f, true, false));
            Assert.IsFalse(KewaspadaanSantet.CahayaTerlihat(7f, true, true));
        }

        [Test]
        public void TingkatKekuatanMenghitungMediaDiserahkanDanMediaLamaYangBelumDinetralkan()
        {
            var s = new GameState { MalamKe = 4 };
            s.MediaDitemukan[Kode.M1] = true; s.MalamMediaDitemukan[Kode.M1] = 2; s.MediaDiserahkan[Kode.M1] = true;
            s.MediaDitemukan[Kode.M2] = true; s.MalamMediaDitemukan[Kode.M2] = 3;                                     // lama, belum dinetralkan
            s.MediaDitemukan[Kode.M3] = true; s.MalamMediaDitemukan[Kode.M3] = 4;                                     // malam ini, belum dihitung
            s.MediaDitemukan[Kode.M4] = true; s.MalamMediaDitemukan[Kode.M4] = 3; s.MediaDinetralkan[Kode.M4] = true; // sudah dinetralkan
            Assert.AreEqual(2, KewaspadaanSantet.TingkatKekuatan(s));
        }

        [Test]
        public void TingkatLebihTinggiLebihCepatDanLebihSering()
        {
            Assert.Greater(KewaspadaanSantet.KecepatanKejar(2), KewaspadaanSantet.KecepatanKejar(0));
            Assert.Less(KewaspadaanSantet.JedaKemunculanDetik(2), KewaspadaanSantet.JedaKemunculanDetik(0));
            Assert.AreEqual(KewaspadaanSantet.JedaMinimum, KewaspadaanSantet.JedaKemunculanDetik(10));
        }
    }
}
