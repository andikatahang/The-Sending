using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Prasetel impor tekstur gaya PSX untuk semua tekstur di Assets/_Project/Art.
    // Hanya berlaku pada impor pertama (belum ada .meta), supaya tim masih bisa menyetel satu tekstur lewat Inspector.
    public class PrasetelTeksturPsx : AssetPostprocessor
    {
        const string Folder = "Assets/_Project/Art/";
        public const int UkuranMaks = 256;

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(Folder)) return;
            var importer = (TextureImporter)assetImporter;
            if (!importer.importSettingsMissing) return;
            Terapkan(importer, UkuranMaks);
        }

        // Tipe tekstur (mis. normal map) tidak diubah.
        public static void Terapkan(TextureImporter importer, int ukuranMaks)
        {
            importer.maxTextureSize = ukuranMaks;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
        }
    }
}
