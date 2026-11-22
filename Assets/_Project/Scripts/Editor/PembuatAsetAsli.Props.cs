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
            // --- Penggantian visual (Task 2) ---
            // Benda dinding (StopKontak, JamDinding, FotoKeluarga): titik nol di tengah, sama dengan pengganti.
            Prefab("Gapura", r => Kayu(Taruh(Props + "gapura/2Gapura.fbx", r, Vector3.zero, new Vector3(5f, 4f, 0.8f), 0f), "BatuPutih"), false);
            Prefab("PosRonda", r => Taruh(Props + "pos-ronda/source/model.glb", r, Vector3.zero, new Vector3(3f, 2.8f, 2.5f), 0f), true);
            Prefab("SumurTimba", r => Bahan(Taruh(Props + "well-building-sumur/source/Sumur_Baked.fbx", r, Vector3.zero,
                new Vector3(1.3f, 2.4f, 1.3f), 0f), "well-building-sumur", "textures/Diffuse_Bake.jpg"), false);
            Prefab("Sumur", r => Bahan(Taruh(Props + "well-building-sumur/source/Sumur_Baked.fbx", r, Vector3.zero,
                new Vector3(1.3f, 2.4f, 1.3f), 0f), "well-building-sumur", "textures/Diffuse_Bake.jpg"), false);
            Prefab("TanggaBambu", r => Bahan(Taruh(Props + "wooden-ladder/source/Ladder Wooden.fbx", r, Vector3.zero,
                new Vector3(0.6f, 2.5f, 0.15f), 0f), "wooden-ladder", "textures/Ladder_Wooden_BaseColor.png"), false);
            Prefab("Boneka", r => Bahan(Taruh(Props + "voodoo-doll/source/Doll(stab).fbx", r, Vector3.zero,
                new Vector3(0.3f, 0.12f, 0.4f), 0f), "voodoo-doll", "textures/voodoo_-_base.jpg"), false);
            Prefab("Gembok", r => Bahan(Taruh(Props + "door-padlock-key/source/padlock.fbx", r, Vector3.zero,
                new Vector3(0.08f, 0.12f, 0.04f), 0f), "door-padlock-key", "textures/padlock_Padlock_BaseColor.png"), false);
            Prefab("StopKontak", r => Bahan(Taruh(Props + "power-outlet-light-switch/source/Untitled 2.fbx", r, new Vector3(0f, -0.05f, 0f),
                new Vector3(0.1f, 0.1f, 0.03f), 0f), "power-outlet-light-switch", "textures/BaseColor.png"), false);
            Prefab("JamDinding", r => Bahan(Taruh(Props + "antique-wall-clock/source/clock.obj", r, new Vector3(0f, -0.35f, 0f),
                new Vector3(0.35f, 0.7f, 0.15f), 0f), "antique-wall-clock", "textures/clock_low_clock_BaseColor.png"), false);
            Prefab("LemariPakaian", r => Bahan(Taruh(Props + "wooden-wardrobe/source/FurnitureDresser001a_substanceuv.fbx", r, Vector3.zero,
                new Vector3(1f, 2f, 0.6f), 0f), "wooden-wardrobe", "textures/FurnitureDresser001a_color.jpg"), false);
            Prefab("Linggis", r => Bahan(Taruh(Props + "tools-asset/source/Tools.fbx", r, Vector3.zero, new Vector3(0.05f, 1.2f, 0.12f), 0f,
                hanya: new[] { "Fomka1" }), "tools-asset", "textures/Fomka1_baseColor.jpg"), false);
            Prefab("Tang", r => Bahan(Taruh(Props + "tools-asset/source/Tools.fbx", r, Vector3.zero, new Vector3(0.22f, 0.03f, 0.08f), 0f,
                hanya: new[] { "Pliers" }), "tools-asset", "textures/PliersMat_baseColor.png"), false);
            Prefab("FotoKeluarga", FotoKeluarga, false);
        }

        // Bingkai kayu + foto (props-lepas/foto-keluarga.jpg). Titik nol di tengah seperti pengganti (0,6 x 0,45 x 0,04).
        static void FotoKeluarga(Transform r)
        {
            Kotak(r, "Bingkai", "KayuGelap", Vector3.zero, new Vector3(0.6f, 0.45f, 0.04f), false);
            var foto = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(foto.GetComponent<Collider>());
            foto.name = "Foto";
            foto.transform.SetParent(r, false);
            foto.transform.localPosition = new Vector3(0f, 0f, 0.021f);   // sisi +Z; pengganti diputar 180° di pembuat
            foto.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            foto.transform.localScale = new Vector3(0.52f, 0.37f, 1f);
            Bahan(foto, "props-lepas", "foto-keluarga.jpg");
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
