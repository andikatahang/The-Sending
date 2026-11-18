using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PetaSceneTests
    {
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
        [TestCase(7)]
        public void SetiapMalamDimulaiDiRumahKeluarga(int malam)
        {
            Assert.AreEqual(PetaScene.RumahKeluarga, PetaScene.Untuk(new GameState { MalamKe = malam }));
        }
    }
}
