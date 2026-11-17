using NUnit.Framework;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;

namespace TheSending.Tests
{
    // Memeriksa SEMUA aset DataDialog di proyek: kode efek, lompatan, dan jumlah pilihan.
    public class DataDialogTests
    {
        [Test]
        public void SemuaAsetDialogLolosPemeriksaan()
        {
            var guid = AssetDatabase.FindAssets("t:DataDialog", new[] { "Assets/_Project/Data" });
            Assert.IsNotEmpty(guid, "Tidak ada aset DataDialog di Assets/_Project/Data.");
            foreach (var g in guid)
            {
                string jalur = AssetDatabase.GUIDToAssetPath(g);
                var data = AssetDatabase.LoadAssetAtPath<DataDialog>(jalur);
                Assert.IsNotNull(data, jalur);
                CollectionAssert.IsEmpty(PemeriksaDialog.Periksa(data.Baris), jalur);
            }
        }
    }
}
