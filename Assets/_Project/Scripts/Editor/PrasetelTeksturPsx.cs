using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Prasetel impor tekstur gaya PSX untuk semua tekstur di Assets/_Project/Art.
    // Hanya berlaku pada impor pertama (belum ada .meta), supaya tim masih bisa menyetel satu tekstur lewat Inspector.
    public class PrasetelTeksturPsx : AssetPostprocessor
    {
        const string Folder = "Assets/_Project/Art/";
        const int UkuranMaks = 256;

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(Folder)) return;
            var importer = (TextureImporter)assetImporter;
            if (!importer.importSettingsMissing) return;
            importer.maxTextureSize = UkuranMaks;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
        }
    }
}
