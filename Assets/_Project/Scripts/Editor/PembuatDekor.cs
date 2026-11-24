using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Dekor per scene di prefab Prefabs/Dekor/Dekor_<Scene>: tim menggeser, menambah, atau menghapus dekor di scene lalu
    // Overrides > Apply All (atau di Prefab Mode), dan Bangun Ulang Semua tidak menimpanya. Prefab yang ada hanya dipasang;
    // hapus prefabnya untuk membuat ulang dari skrip. Hanya dekor murni (tanpa skrip Runtime, tidak dirujuk field mana pun,
    // tampil di setiap malam); benda cerita tetap di skrip pembuat.
    internal static class PembuatDekor
    {
        internal const string Folder = "Assets/_Project/Prefabs/Dekor";

        internal static GameObject Pasang(string scene, Action<Transform> buat)
        {
            string jalur = $"{Folder}/Dekor_{scene}.prefab";
            var ada = AssetDatabase.LoadAssetAtPath<GameObject>(jalur);
            if (ada != null) return (GameObject)PrefabUtility.InstantiatePrefab(ada);
            var akar = new GameObject("Dekor_" + scene);   // di titik nol: posisi lokal anak = posisi dunia
            buat(akar.transform);
            Directory.CreateDirectory(Folder);
            PrefabUtility.SaveAsPrefabAssetAndConnect(akar, jalur, InteractionMode.AutomatedAction);
            return akar;
        }
    }
}
