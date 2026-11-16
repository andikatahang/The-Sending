using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TheSending.EditorTools
{
    // Memasang gaya PSX (docs/style-guide.md) ke aset URP proyek. Aman dijalankan berulang.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatGayaPsx.Pasang
    public static class PembuatGayaPsx
    {
        // Material dari PSX_Layar.shadergraph. Dibuat tim di editor (rencana 0C, "Persiapan Tim" bagian A).
        const string JalurMaterial = "Assets/_Project/Art/Shaders/PSX_Layar.mat";
        const string JalurVolume = "Assets/_Project/Settings/PSX_Volume.asset";
        const string NamaFitur = "PSX_Layar";
        const string NamaFiturSsao = "ScreenSpaceAmbientOcclusion";
        static readonly string[] JalurUrp = { "Assets/Settings/PC_RPAsset.asset", "Assets/Settings/Mobile_RPAsset.asset" };
        static readonly string[] JalurRenderer = { "Assets/Settings/PC_Renderer.asset", "Assets/Settings/Mobile_Renderer.asset" };

        // Nilai style guide. Ubah di sini, jalankan ulang menu, lalu perbarui docs/style-guide.md.
        public const float SkalaRender = 0.4f;
        public const float BitWarna = 5f, KuatDither = 1f;
        public const float IntensitasGrain = 0.4f, IntensitasVignette = 0.35f, KehalusanVignette = 0.4f, Saturasi = -30f;
        public const float KepadatanKabut = 0.06f;
        public static readonly Color WarnaKabut = new Color32(0x1A, 0x1F, 0x24, 0xFF);
        public static readonly Color WarnaAmbient = new Color32(0x05, 0x07, 0x0A, 0xFF);

        const int MsaaMati = 1;               // MsaaQuality.Disabled
        const int UpscalingPoint = 2;         // UpscalingFilterSelection.Point
        const string UpscalerPoint = "Nearest-Neighbor";

        [MenuItem("The Sending/Pasang Gaya PSX")]
        public static void Pasang()
        {
            var material = MuatMaterial();   // gagal lebih dulu, sebelum ada aset yang diubah
            Directory.CreateDirectory(Path.GetDirectoryName(JalurVolume));
            AssetDatabase.Refresh();
            var volume = AmbilAtauBuatVolume();
            foreach (var jalur in JalurUrp) AturUrp(jalur, volume);
            foreach (var jalur in JalurRenderer) AturRenderer(jalur, material);
            AssetDatabase.SaveAssets();
            Debug.Log("Gaya PSX terpasang.");
        }

        // Kabut dan ambient untuk scene yang sedang dibuka. Dipanggil pembuat scene.
        public static void TerapkanKeScene()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = WarnaAmbient;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = KepadatanKabut;
            RenderSettings.fogColor = WarnaKabut;
        }

        // Latar = warna kabut (langit tidak ikut berkabut), post-processing aktif, tanpa anti-aliasing.
        public static void SiapkanKamera(Camera kamera)
        {
            kamera.clearFlags = CameraClearFlags.SolidColor;
            kamera.backgroundColor = WarnaKabut;
            var data = kamera.GetUniversalAdditionalCameraData();
            data.renderPostProcessing = true;
            data.antialiasing = AntialiasingMode.None;
        }

        static Material MuatMaterial()
        {
            var m = AssetDatabase.LoadAssetAtPath<Material>(JalurMaterial);
            if (m == null)
                throw new FileNotFoundException(
                    "Material PSX belum ada. Tim harus membuat PSX_Layar.shadergraph dan PSX_Layar.mat di editor " +
                    "(docs/superpowers/plans/2026-11-16-fase-0c-gaya-dan-menu.md, Persiapan Tim bagian A).", JalurMaterial);
            if (ShaderUtil.ShaderHasError(m.shader))
                throw new InvalidOperationException($"Shader '{m.shader.name}' pada {JalurMaterial} punya error; buka graph-nya di editor.");
            if (!m.HasProperty("_BitWarna") || !m.HasProperty("_KuatDither"))
                throw new InvalidOperationException(
                    $"{JalurMaterial} tidak punya properti _BitWarna/_KuatDither. Periksa kolom Reference kedua properti di Blackboard graph.");
            m.SetFloat("_BitWarna", BitWarna);
            m.SetFloat("_KuatDither", KuatDither);
            EditorUtility.SetDirty(m);
            return m;
        }

        static VolumeProfile AmbilAtauBuatVolume()
        {
            var profil = AssetDatabase.LoadAssetAtPath<VolumeProfile>(JalurVolume);
            if (profil == null)
            {
                profil = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profil, JalurVolume);
            }
            Komponen<FilmGrain>(profil).intensity.Override(IntensitasGrain);
            var vignette = Komponen<Vignette>(profil);
            vignette.intensity.Override(IntensitasVignette);
            vignette.smoothness.Override(KehalusanVignette);
            Komponen<ColorAdjustments>(profil).saturation.Override(Saturasi);
            Komponen<Tonemapping>(profil).mode.Override(TonemappingMode.Neutral);
            EditorUtility.SetDirty(profil);
            return profil;
        }

        static T Komponen<T>(VolumeProfile profil) where T : VolumeComponent
        {
            if (!profil.TryGet<T>(out var komponen))
            {
                komponen = profil.Add<T>();
                AssetDatabase.AddObjectToAsset(komponen, profil);
            }
            EditorUtility.SetDirty(komponen);
            return komponen;
        }

        static void AturUrp(string jalur, VolumeProfile volume)
        {
            var urp = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(jalur);
            if (urp == null) throw new FileNotFoundException("Aset URP tidak ditemukan.", jalur);
            var so = new SerializedObject(urp);
            Properti(so, "m_RenderScale").floatValue = SkalaRender;
            Properti(so, "m_MSAA").intValue = MsaaMati;
            Properti(so, "m_UpscalingFilter").intValue = UpscalingPoint;
            var upscaler = so.FindProperty("m_SelectedUpscalerName");   // hanya ada jika kerangka upscaler baru aktif
            if (upscaler != null) upscaler.stringValue = UpscalerPoint;
            Properti(so, "m_VolumeProfile").objectReferenceValue = volume;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void AturRenderer(string jalur, Material material)
        {
            var data = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(jalur);
            if (data == null) throw new FileNotFoundException("Renderer URP tidak ditemukan.", jalur);

            var fitur = data.rendererFeatures.Find(f => f != null && f.name == NamaFitur) as FullScreenPassRendererFeature;
            if (fitur == null)
            {
                // Sama dengan yang dilakukan Inspector URP saat "Add Renderer Feature": sub-aset + daftar + peta id lokal.
                fitur = ScriptableObject.CreateInstance<FullScreenPassRendererFeature>();
                fitur.name = NamaFitur;
                AssetDatabase.AddObjectToAsset(fitur, data);
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(fitur, out _, out long idLokal);
                var so = new SerializedObject(data);
                var daftar = Properti(so, "m_RendererFeatures");
                daftar.arraySize++;
                daftar.GetArrayElementAtIndex(daftar.arraySize - 1).objectReferenceValue = fitur;
                var peta = Properti(so, "m_RendererFeatureMap");
                peta.arraySize++;
                peta.GetArrayElementAtIndex(peta.arraySize - 1).longValue = idLokal;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            fitur.injectionPoint = FullScreenPassRendererFeature.InjectionPoint.AfterRenderingPostProcessing;
            fitur.fetchColorBuffer = true;
            fitur.requirements = ScriptableRenderPassInput.None;
            fitur.passMaterial = material;
            fitur.passIndex = 0;
            EditorUtility.SetDirty(fitur);

            // SSAO bawaan template tidak cocok dengan gaya PSX dan memberatkan laptop lemah.
            foreach (var f in data.rendererFeatures)
            {
                if (f == null || f.name != NamaFiturSsao) continue;
                f.SetActive(false);
                EditorUtility.SetDirty(f);
            }
            EditorUtility.SetDirty(data);
        }

        static SerializedProperty Properti(SerializedObject so, string nama) =>
            so.FindProperty(nama) ?? throw new MissingFieldException(so.targetObject.GetType().Name, nama);
    }
}
