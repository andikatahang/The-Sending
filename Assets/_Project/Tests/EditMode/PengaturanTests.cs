using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PengaturanTests
    {
        [Test]
        public void BawaanSesuaiDesain()
        {
            var p = new Pengaturan();
            Assert.AreEqual(new[] { 1f, 1f, 1f, 1f, 1f }, p.Volume);
            Assert.AreEqual(Pengaturan.SensitivitasAwal, p.Sensitivitas);
            Assert.IsFalse(p.BalikY);
            Assert.IsTrue(p.LayarPenuh);
        }

        [Test]
        public void NamaGrupSesuaiMixerStyleGuide()
        {
            Assert.AreEqual(new[] { "Master", "Music", "SFX", "Ambience", "Dialog" }, Pengaturan.NamaGrup);
        }

        [Test]
        public void RapikanMembatasiVolumeKeNolSampaiSatu()
        {
            var p = new Pengaturan { Volume = new[] { 1.5f, -0.2f, 0.4f, 1f, 0f } };
            p.Rapikan();
            Assert.AreEqual(new[] { 1f, 0f, 0.4f, 1f, 0f }, p.Volume);
        }

        [Test]
        public void RapikanMembatasiSensitivitas()
        {
            Assert.AreEqual(Pengaturan.SensitivitasMaks, new Pengaturan { Sensitivitas = 9f }.Rapikan().Sensitivitas);
            Assert.AreEqual(Pengaturan.SensitivitasMin, new Pengaturan { Sensitivitas = 0f }.Rapikan().Sensitivitas);
        }

        [Test]
        public void RapikanMenggantiNaNDenganNilaiBawaan()
        {
            var p = new Pengaturan { Sensitivitas = float.NaN, Volume = new[] { float.NaN, 1f, 1f, 1f, 1f } };
            p.Rapikan();
            Assert.AreEqual(Pengaturan.SensitivitasAwal, p.Sensitivitas);
            Assert.AreEqual(1f, p.Volume[0]);
        }

        [Test]
        public void RapikanMemperbaikiArrayVolumeYangHilangAtauPendek()
        {
            Assert.AreEqual(new[] { 1f, 1f, 1f, 1f, 1f }, new Pengaturan { Volume = null }.Rapikan().Volume);
            Assert.AreEqual(new[] { 0.3f, 0.5f, 1f, 1f, 1f }, new Pengaturan { Volume = new[] { 0.3f, 0.5f } }.Rapikan().Volume);
        }

        [Test]
        public void JsonBolakBalikMenjagaNilai()
        {
            var p = new Pengaturan { Sensitivitas = 0.12f, BalikY = true, LayarPenuh = false };
            p.Volume[1] = 0.3f;
            var q = Pengaturan.DariJson(p.KeJson());
            Assert.AreEqual(0.3f, q.Volume[1], 0.0001f);
            Assert.AreEqual(0.12f, q.Sensitivitas, 0.0001f);
            Assert.IsTrue(q.BalikY);
            Assert.IsFalse(q.LayarPenuh);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("{bukan json")]
        public void JsonKosongAtauRusakMenghasilkanBawaan(string json)
        {
            var p = Pengaturan.DariJson(json);
            Assert.AreEqual(new[] { 1f, 1f, 1f, 1f, 1f }, p.Volume);
            Assert.AreEqual(Pengaturan.SensitivitasAwal, p.Sensitivitas);
            Assert.IsTrue(p.LayarPenuh);
        }

        [Test]
        public void JsonDenganNilaiDiLuarBatasDirapikan()
        {
            var p = Pengaturan.DariJson("{\"Volume\":[2,-1],\"Sensitivitas\":9}");
            Assert.AreEqual(new[] { 1f, 0f, 1f, 1f, 1f }, p.Volume);
            Assert.AreEqual(Pengaturan.SensitivitasMaks, p.Sensitivitas);
        }

        [TestCase(1f, 0f)]
        [TestCase(0.5f, -6.02f)]
        [TestCase(0f, -80f)]
        [TestCase(-1f, -80f)]
        public void KeDesibelMengubahLinearKeDesibel(float linear, float desibel)
        {
            Assert.AreEqual(desibel, Pengaturan.KeDesibel(linear), 0.01f);
        }
    }
}
