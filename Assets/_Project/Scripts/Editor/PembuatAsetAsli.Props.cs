using System.IO;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Props dari "Props TheSending" (pindahan ke Art/ThirdParty). Nama prefab yang sama dengan prefab Pengganti
    // otomatis menggantikannya (PembuatPengganti.Pasang memuat Prefabs/Asli lebih dulu); logika tidak berubah.
    public static partial class PembuatAsetAsli
    {
        const string Props = Pihak3 + "/";

        static void BuatProps()
        {
            // Diisi Task 2-4.
        }

        // Material URP Lit dari satu tekstur warna; dipasang ke semua renderer (material bawaan FBX hanya-baca).
        static void Bahan(GameObject g, string folder, string baseColor, string warnaHex = "FFFFFF")
        {
            string jalurTex = Props + folder + "/" + baseColor;
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(jalurTex) ?? throw new FileNotFoundException(jalurTex);
            string jalurMat = $"{Props}{folder}/Bahan_{Path.GetFileNameWithoutExtension(baseColor)}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(jalurMat);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, jalurMat);
            }
            mat.SetTexture("_BaseMap", tex);
            mat.SetColor("_BaseColor", PembuatRig.Hex(warnaHex));
            mat.SetFloat("_Smoothness", 0f);
            EditorUtility.SetDirty(mat);
            foreach (var r in g.GetComponentsInChildren<Renderer>())
                r.sharedMaterials = System.Array.ConvertAll(r.sharedMaterials, _ => mat);
        }

        // Jumlah segitiga per prefab Asli, untuk mencatat model berat (Global Constraints).
        static void LaporSegitiga()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { FolderPrefab }))
            {
                var p = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                int n = 0;
                foreach (var mf in p.GetComponentsInChildren<MeshFilter>(true))
                    if (mf.sharedMesh != null) n += mf.sharedMesh.triangles.Length / 3;
                foreach (var sm in p.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    if (sm.sharedMesh != null) n += sm.sharedMesh.triangles.Length / 3;
                Debug.Log($"[Segitiga] {p.name}: {n}");
            }
        }
    }
}
