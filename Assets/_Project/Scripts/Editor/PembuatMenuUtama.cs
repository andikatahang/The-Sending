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

        [MenuItem("The Sending/Bangun Ulang Semua (gaya PSX + scene)")]
        public static void BangunSemua()
        {
            PembuatGayaPsx.Pasang();
            PembuatPengganti.Buat();
            PembuatAsetAsli.Buat();
            PembuatDataFase1.Buat();
            PembuatSandbox.Buat();
            PembuatDesa.Buat();
            PembuatRumahKeluarga.Buat();
            Buat();   // terakhir: mendaftarkan scene dan meninggalkan menu utama terbuka
        }

        [MenuItem("The Sending/Buat Scene Menu Utama")]
        public static void Buat()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();

            new GameObject("GameManager").AddComponent<GameManager>();

            var kamera = new GameObject("Kamera");
            kamera.tag = "MainCamera";
            kamera.transform.position = new Vector3(0f, 1.4f, -3.5f);
            kamera.transform.rotation = Quaternion.Euler(8f, 0f, 0f);
            PembuatGayaPsx.SiapkanKamera(kamera.AddComponent<Camera>());
            kamera.AddComponent<AudioListener>();

            // Latar sederhana supaya gaya PSX terlihat di belakang menu: lantai, meja, dan lampu teplok hangat.
            var lantai = GameObject.CreatePrimitive(PrimitiveType.Plane);
            lantai.name = "Lantai";
            var meja = GameObject.CreatePrimitive(PrimitiveType.Cube);
            meja.name = "Meja";
            meja.transform.position = new Vector3(0.8f, 0.4f, 1f);
            meja.transform.localScale = new Vector3(1.2f, 0.8f, 0.6f);
            var teplok = new GameObject("Teplok").AddComponent<Light>();
            teplok.type = LightType.Point;
            teplok.color = new Color32(0xFF, 0xB4, 0x6B, 0xFF);
            teplok.range = 6f;
            teplok.intensity = 2f;
            teplok.transform.position = new Vector3(0.8f, 1.1f, 1f);

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
            };
        }
    }
}
