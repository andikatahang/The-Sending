using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Menyiapkan aset pihak ketiga di Art/ThirdParty: material built-in jadi URP Lit, semua material tanpa kilap
    // (Smoothness 0), dan tekstur memakai prasetel PSX. Aman dijalankan berulang.
    // Batchmode: -executeMethod TheSending.EditorTools.PembuatAsetLuar.Siapkan
    public static class PembuatAsetLuar
    {
        const string Folder = "Assets/_Project/Art/ThirdParty";
        const string ShaderLit = "Universal Render Pipeline/Lit";

        [MenuItem("The Sending/Siapkan Aset Luar")]
        public static void Siapkan()
        {
            int tekstur;
            AssetDatabase.StartAssetEditing();
            try { tekstur = SetelTekstur(); }
            finally { AssetDatabase.StopAssetEditing(); }
            int material = UbahMaterial();
            AssetDatabase.SaveAssets();
            Debug.Log($"Aset luar siap: {material} material diubah, {tekstur} tekstur disetel.");
        }

        static int SetelTekstur()
        {
            int n = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { Folder }))
            {
                var jalur = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetImporter.GetAtPath(jalur) is not TextureImporter imp) continue;
                if (PembuatAsetAsli.TeksturProps(jalur)) continue;   // props: resolusi asli <= 1024 (PembuatAsetAsli.Bahan)
                const int maks = PrasetelTeksturPsx.UkuranMaks; // daun pohon juga 256: sudah dicek di pratinjau, masih terbaca
                if (imp.maxTextureSize == maks && imp.filterMode == FilterMode.Point && !imp.mipmapEnabled) continue;
                PrasetelTeksturPsx.Terapkan(imp, maks);
                imp.SaveAndReimport();
                n++;
            }
            return n;
        }

        static int UbahMaterial()
        {
            var lit = Shader.Find(ShaderLit);
            int n = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Material", new[] { Folder }))
            {
                var jalur = AssetDatabase.GUIDToAssetPath(guid);
                if (!jalur.EndsWith(".mat")) continue; // material bawaan FBX hanya-baca; prefab memakai file .mat
                var mat = AssetDatabase.LoadAssetAtPath<Material>(jalur);
                if (mat == null) continue;
                var nama = mat.shader.name;
                if (nama.StartsWith("Universal Render Pipeline/"))
                {
                    if (TanpaKilap(mat)) n++;
                }
                else if (nama == "Standard" || nama == "Standard (Specular setup)")
                {
                    KeUrpLit(mat, lit);
                    n++;
                }
                else Debug.LogWarning($"PembuatAsetLuar: shader '{nama}' tidak dikenal, dilewati: {jalur}");
            }
            return n;
        }

        static bool TanpaKilap(Material mat)
        {
            if (!mat.HasProperty("_Smoothness") || mat.GetFloat("_Smoothness") == 0f) return false;
            mat.SetFloat("_Smoothness", 0f);
            EditorUtility.SetDirty(mat);
            return true;
        }

        static Texture Tex(Material m, string p) => m.HasProperty(p) ? m.GetTexture(p) : null;
        static float Angka(Material m, string p, float awal) => m.HasProperty(p) ? m.GetFloat(p) : awal;

        static void KeUrpLit(Material mat, Shader lit)
        {
            // Baca nilai shader Standard sebelum shader diganti.
            var dasar = Tex(mat, "_MainTex");
            var skala = mat.GetTextureScale("_MainTex");
            var geser = mat.GetTextureOffset("_MainTex");
            var warna = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;
            var normal = Tex(mat, "_BumpMap");
            float kuatNormal = Angka(mat, "_BumpScale", 1f);
            var oklusi = Tex(mat, "_OcclusionMap");
            float kuatOklusi = Angka(mat, "_OcclusionStrength", 1f);
            float mode = Angka(mat, "_Mode", 0f); // 0 opaque, 1 cutout, 2 fade, 3 transparent
            float batas = Angka(mat, "_Cutoff", 0.5f);
            float logam = Angka(mat, "_Metallic", 0f);
            bool pendar = mat.IsKeywordEnabled("_EMISSION");
            var warnaPendar = mat.HasProperty("_EmissionColor") ? mat.GetColor("_EmissionColor") : Color.black;
            var petaPendar = Tex(mat, "_EmissionMap");

            mat.shader = lit;
            mat.shaderKeywords = new string[0];
            mat.SetTexture("_BaseMap", dasar);
            mat.SetTextureScale("_BaseMap", skala);
            mat.SetTextureOffset("_BaseMap", geser);
            mat.SetColor("_BaseColor", warna);
            mat.SetTexture("_BumpMap", normal);
            mat.SetFloat("_BumpScale", kuatNormal);
            mat.SetTexture("_OcclusionMap", oklusi);
            mat.SetFloat("_OcclusionStrength", kuatOklusi);
            mat.SetTexture("_MetallicGlossMap", null);
            mat.SetTexture("_SpecGlossMap", null);
            mat.SetFloat("_WorkflowMode", 1f);
            mat.SetFloat("_Metallic", logam);
            mat.SetFloat("_Smoothness", 0f);
            mat.SetFloat("_AlphaClip", mode == 1f ? 1f : 0f);
            mat.SetFloat("_Cutoff", batas);
            mat.SetFloat("_Surface", mode >= 2f ? 1f : 0f);
            mat.SetFloat("_Blend", 0f);
            // Emisi hanya jika kata kunci _EMISSION menyala di material lama; warna putih tanpa kata kunci bukan emisi.
            mat.SetColor("_EmissionColor", pendar ? warnaPendar : Color.black);
            mat.SetTexture("_EmissionMap", pendar ? petaPendar : null);
            mat.globalIlluminationFlags = pendar
                ? MaterialGlobalIlluminationFlags.BakedEmissive
                : MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            BaseShaderGUI.SetMaterialKeywords(mat, LitGUI.SetMaterialKeywords);
            EditorUtility.SetDirty(mat);
        }
    }
}
