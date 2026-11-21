using System.IO;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Membuat Scene_MainMenu dan mendaftarkan scene ke Build Settings.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatMenuUtama.BangunSemua
    public static class PembuatMenuUtama
    {
        const string JalurScene = "Assets/_Project/Scenes/" + GameManager.SceneMenuUtama + ".unity";
        // Lebih tipis dari kabut gameplay supaya desa terlihat dari ketinggian kamera.
        const float KabutMenu = 0.035f;

        [MenuItem("The Sending/Bangun Ulang Semua (gaya PSX + scene)")]
        public static void BangunSemua()
        {
            PembuatGayaPsx.Pasang();
            PembuatPengganti.Buat();
            PembuatAsetAsli.Buat();
            PembuatDataFase1.Buat();
            PembuatDataFase2.Buat();
            PembuatDataFase3.Buat();
            PembuatDataFase4.Buat();
            PembuatDataFase5.Buat();
            PembuatSandbox.Buat();
            PembuatDesa.Buat();
            PembuatRumahKeluarga.Buat();
            PembuatRumahBuSumi.Buat();
            PembuatMakam.Buat();
            PembuatRumahPakde.Buat();
            Buat();   // terakhir: mendaftarkan scene dan meninggalkan menu utama terbuka
        }

        [MenuItem("The Sending/Buat Scene Menu Utama")]
        public static void Buat()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();

            new GameObject("GameManager").AddComponent<GameManager>();

            // Latar: desa malam dilihat dari atas sudut jalan, ke arah rumah Bu Sumi dan rumah keluarga.
            PembuatDesa.Latar();
            RenderSettings.fogDensity = KabutMenu;
            var kamera = new GameObject("Kamera");
            kamera.tag = "MainCamera";
            kamera.transform.position = new Vector3(-5f, 10f, 68f);
            var bidikan = new Vector3(1.5f, 0f, 88f);
            kamera.transform.LookAt(bidikan);
            var melayang = new SerializedObject(kamera.AddComponent<KameraMenuMelayang>());
            melayang.FindProperty("bidikan").vector3Value = bidikan;
            melayang.ApplyModifiedPropertiesWithoutUndo();
            PembuatGayaPsx.SiapkanKamera(kamera.AddComponent<Camera>());
            kamera.AddComponent<AudioListener>();

            var ui = new GameObject("UI");
            var pengaturan = ui.AddComponent<PanelPengaturan>();
            PembuatSandbox.Set(ui.AddComponent<MenuUtama>(), "pengaturan", pengaturan);

            EditorSceneManager.SaveScene(scene, JalurScene);
            DaftarkanScene();
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene menu utama dibuat: {JalurScene}");
        }

        // Urutan = indeks build. Menu utama harus indeks 0 supaya build dibuka dari sana.
        public static void DaftarkanScene()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(JalurScene, true),
                new EditorBuildSettingsScene(PembuatDesa.JalurScene, true),
                new EditorBuildSettingsScene(PembuatRumahKeluarga.JalurScene, true),
                new EditorBuildSettingsScene(PembuatSandbox.JalurScene, true),
                new EditorBuildSettingsScene(PembuatRumahBuSumi.JalurScene, true),
                new EditorBuildSettingsScene(PembuatMakam.JalurScene, true),
                new EditorBuildSettingsScene(PembuatRumahPakde.JalurScene, true),
            };
        }
    }
}
