using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PetaSceneTests
    {
        [Test]
        public void EndingSelaluKeEpilog()
        {
            Assert.AreEqual("Scene_Epilog", PetaScene.Untuk(new GameState { MalamKe = 7, Ending = Ending.Fitnah }));
            Assert.AreEqual("Scene_Epilog", PetaScene.Untuk(new GameState { MalamKe = 3, Ending = Ending.Terlambat }));
        }

        [Test]
        public void KejaranWiryoBelumLolosKeWatuireng()
        {
            var s = new GameState { MalamKe = 7, PilihanAkhir = PilihanAkhir.Lapor };
            Assert.AreEqual("Scene_Watuireng", PetaScene.Untuk(s));
            s.LolosKejaranWiryo = true;
            Assert.AreEqual("Scene_RumahKeluarga", PetaScene.Untuk(s));
            Assert.AreEqual("Scene_RumahKeluarga", PetaScene.Untuk(new GameState { MalamKe = 7 }));
        }

        [Test]
        public void PrologDimulaiDiJalanDesa()
        {
            Assert.AreEqual("Scene_Desa", PetaScene.Untuk(new GameState()));
        }

        [Test]
        public void PrologSetelahTibaDiRumahDimuatDiRumah()
        {
            var s = new GameState();
            TujuanMalam.Selesaikan(s, TujuanMalam.TibaDiRumah);
            Assert.AreEqual("Scene_RumahKeluarga", PetaScene.Untuk(s));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        public void SetiapMalamDimulaiDiRumahKeluarga(int malam)
        {
            Assert.AreEqual(PetaScene.RumahKeluarga, PetaScene.Untuk(new GameState { MalamKe = malam }));
        }

        // Desa dan rumah Bu Sumi tidak punya checkpoint, jadi "Lanjutkan" tetap dari rumah keluarga.
        [Test]
        public void Malam3SetelahKeRumahBuSumiTetapDimulaiDiRumah()
        {
            var s = new GameState { MalamKe = 3 };
            TujuanMalam.Selesaikan(s, TujuanMalam.KunjungiSumi);
            Assert.AreEqual(PetaScene.RumahKeluarga, PetaScene.Untuk(s));
            Assert.AreEqual("Scene_RumahBuSumi", PetaScene.RumahBuSumi);
        }

        // K17-B: titik muncul di Scene_Desa pada Malam 3.
        [TestCase(false, false)]
        [TestCase(true, true)]
        public void Malam3DiDesaMunculDiDepanRumahBuSumiSetelahBerkunjung(bool sudahBerkunjung, bool diDepanBuSumi)
        {
            var s = new GameState { MalamKe = 3 };
            if (sudahBerkunjung) TujuanMalam.Selesaikan(s, TujuanMalam.KunjungiSumi);
            Assert.AreEqual(diDepanBuSumi, PetaScene.DiDepanRumahBuSumi(s));
        }
    
        [Test]
        public void NamaSceneMalam5()
        {
            Assert.AreEqual("Scene_Makam", PetaScene.Makam);
            Assert.AreEqual("Scene_RumahPakde", PetaScene.RumahPakde);
        }

        // K32-A: asal dicatat untuk titik muncul di desa; "Lanjutkan" tetap dari rumah keluarga.
        [Test]
        public void CatatAsalDisimpanDanTidakMengubahSceneLanjutkan()
        {
            var s = new GameState { MalamKe = 5 };
            Assert.AreEqual("", s.SceneAsal);
            PetaScene.CatatAsal(s, PetaScene.Makam);
            Assert.AreEqual(PetaScene.Makam, s.SceneAsal);
            Assert.AreEqual(PetaScene.RumahKeluarga, PetaScene.Untuk(s));
        }
    }
}
