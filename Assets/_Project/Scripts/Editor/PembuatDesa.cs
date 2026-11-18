using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Membuat Scene_Desa: jalan masuk desa untuk Prolog (denah K7). Jangan sunting scene dengan tangan; ubah angka di sini.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatDesa.Buat
    public static class PembuatDesa
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.Desa + ".unity";
        const float SetengahLebar = 6.5f, UjungJalan = 100f;
        const string Sfx = PembuatPengganti.FolderSfx;

        [MenuItem("The Sending/Buat Scene Desa")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();
            var rig = PembuatRig.Buat(new Vector3(0f, 0.05f, 2f));

            Lingkungan();
            var sutradara = new GameObject("Sutradara Prolog").AddComponent<AlurProlog>();
            Set(sutradara, "adegan", rig.Adegan);
            Set(sutradara, "kartu", rig.Kartu);
            Set(sutradara, "dialog", rig.Dialog);
            Set(sutradara, "hud", rig.Hud);
            Set(sutradara, "dialogOjek", PembuatDataFase1.Muat(PembuatDataFase1.Ojek));

            // Gapura dan ojek
            PembuatPengganti.Pasang("Gapura", null, Vector3.zero);
            var ojek = PembuatPengganti.Pasang("Ojek", null, new Vector3(1.5f, 0f, 1f), 180f);
            Set(sutradara, "ojek", ojek.transform);
            Set(sutradara, "bidikGapura", Titik("BidikGapura", new Vector3(-2.5f, 1.7f, 7f), new Vector3(0.5f, 1.2f, 0f)));
            // Cutscene ojek pergi (3,5 detik): ojek lurus ke z = -25 dalam 3 detik, suara motor sejak awal.
            var ojekPergi = PembuatTimeline.Buat("Prolog_OjekPergi", 3.5f);
            PembuatTimeline.Gerak(ojekPergi.Tl, ojekPergi.Director, ojek,
                (0f, ojek.transform), (3f, Titik("TitikOjekPergi", new Vector3(1.5f, 0f, -25f), new Vector3(1.5f, 0f, -30f))));
            PembuatTimeline.Sinyal(ojekPergi.Tl, 0.05f, AlurProlog.SinyalMotorPergi);
            PembuatTimeline.Simpan(ojekPergi.Tl);
            Set(sutradara, "timelineOjek", ojekPergi.Pemutar);

            // Zona tutorial dan azan
            var selebarJalan = new Vector3(SetengahLebar * 2f, 3f, 1.5f);
            Set(sutradara, "zonaSenter", Zona("ZonaSenter", new Vector3(0f, 1.5f, 18f), selebarJalan));
            Set(sutradara, "zonaAzan", Zona("ZonaAzan", new Vector3(0f, 1.5f, 58f), selebarJalan));
            Set(sutradara, "zonaBuSumi", Zona("ZonaBuSumi", new Vector3(0f, 1.5f, 72f), selebarJalan));

            // Rumah Bu Sumi di kanan jalan, menghadap jalan. Jendela prefab (1.2, 1.5, 2.5) jatuh di (7.5, 1.5, 76.2).
            PembuatPengganti.Pasang("RumahKecil", null, new Vector3(10f, 0f, 75f), -90f);
            Balok("AmbangJendela", new Vector3(7.25f, 0.45f, 76.2f), new Vector3(0.1f, 0.9f, 1.4f), "Gedek");
            Balok("CahayaJendela", new Vector3(7.47f, 1.5f, 76.2f), new Vector3(0.02f, 1.1f, 1.1f), "Kain");
            var siluet = PembuatPengganti.Pasang("BuSumi_Siluet", null, new Vector3(7.38f, 0f, 76.2f), -90f);
            var tirai = Balok("Tirai", new Vector3(7.2f, 1.5f, 76.2f), new Vector3(0.03f, 1.2f, 1.3f), "Kain");
            tirai.SetActive(false);
            Set(sutradara, "siluetBuSumi", siluet);
            Set(sutradara, "tiraiJendela", tirai);
            Set(sutradara, "lampuJendela", Lampu("LampuJendela", new Vector3(7.1f, 1.7f, 76.2f), "FFB46B", 4f, 2f));
            var bidikJendela = Titik("BidikJendela", new Vector3(2.5f, 1.6f, 74.5f), new Vector3(7.4f, 1.5f, 76.2f));
            Set(sutradara, "bidikJendela", bidikJendela);
            // Cutscene Bu Sumi (4 detik): kamera adegan mendekat pelan ke jendela, tirai menutup pada 2,5 detik.
            var buSumi = PembuatTimeline.Buat("Prolog_BuSumi", 4f);
            PembuatTimeline.Gerak(buSumi.Tl, buSumi.Director, rig.KameraCutscene.gameObject,
                (0f, bidikJendela), (4f, Titik("BidikJendelaDekat", new Vector3(3.5f, 1.6f, 75f), new Vector3(7.4f, 1.5f, 76.2f))));
            PembuatTimeline.Sinyal(buSumi.Tl, 2.5f, AlurProlog.SinyalTiraiDitutup);
            PembuatTimeline.Simpan(buSumi.Tl);
            Set(sutradara, "timelineBuSumi", buSumi.Pemutar);

            // Rumah keluarga: prefab menghadap selatan (ke arah pemain), pintunya memindahkan ke Scene_RumahKeluarga.
            PembuatPengganti.Pasang("RumahKeluargaLuar", null, new Vector3(0f, 0f, UjungJalan), 180f);
            var pintu = Balok("PintuRumah", new Vector3(0f, 1.05f, UjungJalan - 0.15f), new Vector3(1.1f, 2.1f, 0.2f), "KayuGelap");
            var alurPintu = pintu.AddComponent<InteraktifAlur>();
            SetTeks(alurPintu, "prompt", "Masuk rumah");
            Set(sutradara, "pintuRumah", alurPintu);

            // Suara. Azan tidak punya pengganti sintetis; selama filenya belum ada hanya teks ini yang tampil.
            // Tidak ada suara telepon: suara tokoh ditunda, telepon Laras hanya kartu teks.
            Set(sutradara, "suaraMotor", BuatSuara(rig, "MotorMenjauh", Sfx, "SFX", "(Suara: motor menjauh.)"));
            Set(sutradara, "suaraAzan", BuatSuara(rig, "AzanMagrib", Sfx, "Ambience", "(Suara: azan magrib dari kejauhan.)"));
            Set(sutradara, "suaraPintu", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)"));

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene desa dibuat: {JalurScene}");
        }

        static void Lingkungan()
        {
            var bulan = new GameObject("CahayaBulan").AddComponent<Light>();
            bulan.type = LightType.Directional;
            bulan.intensity = 0.15f;
            bulan.color = Hex("6D86A8");
            bulan.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            Balok("Tanah", new Vector3(0f, -0.1f, 50f), new Vector3(40f, 0.2f, 140f), "Tanah");
            Batas("BatasBarat", new Vector3(-SetengahLebar, 2f, 50f), new Vector3(0.2f, 4f, 140f));
            Batas("BatasTimur", new Vector3(SetengahLebar, 2f, 50f), new Vector3(0.2f, 4f, 140f));
            Batas("BatasSelatan", new Vector3(0f, 2f, -1.5f), new Vector3(SetengahLebar * 2f, 4f, 0.2f));

            float[] zLampu = { 10f, 44f, 90f };
            for (int i = 0; i < zLampu.Length; i++)
            {
                float x = i % 2 == 0 ? -3f : 3f;
                PembuatPengganti.Pasang("LampuJalan", null, new Vector3(x, 0f, zLampu[i]), x < 0f ? 0f : 180f);
                Lampu("CahayaLampuJalan", new Vector3(x < 0f ? x + 0.5f : x - 0.5f, 2.6f, zLampu[i]), "FFB46B", 8f, 1.5f);
            }

            // Kebun bambu z 20-40: enam rumpun di tiap sisi, selang-seling jaraknya dari jalan.
            for (int i = 0; i < 6; i++)
            {
                float z = 20f + i * 4f, x = 3.5f + i % 2;
                PembuatPengganti.Pasang("RumpunBambu", null, new Vector3(-x, 0f, z), i * 37f);
                PembuatPengganti.Pasang("RumpunBambu", null, new Vector3(x, 0f, z + 2f), i * 53f);
            }

            // Jembatan bambu z 48-56. Air hanya tampilan; kedua sisi jembatan ditutup pembatas.
            var air = Balok("Air", new Vector3(0f, 0.01f, 52f), new Vector3(SetengahLebar * 2f, 0.02f, 6f), "Gelap");
            Object.DestroyImmediate(air.GetComponent<Collider>());
            PembuatPengganti.Pasang("JembatanBambu", null, new Vector3(0f, 0f, 52f));
            Batas("BatasAirBarat", new Vector3(-3.75f, 1f, 52f), new Vector3(5.5f, 2f, 6.2f));
            Batas("BatasAirTimur", new Vector3(3.75f, 1f, 52f), new Vector3(5.5f, 2f, 6.2f));
        }
    }
}
