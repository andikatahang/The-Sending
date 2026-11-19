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
    // Membuat Scene_Desa: jalan masuk desa untuk Prolog (denah K7) dan jalan malam ke rumah Bu Sumi di Malam 3 (K17-B).
    // Jangan sunting scene dengan tangan; ubah angka di sini.
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
            SetAngka(rig.Langkah, "bawaan", (int)JenisPermukaan.Tanah);

            Lingkungan();
            var sutradara = new GameObject("Sutradara Prolog").AddComponent<AlurProlog>();
            Set(sutradara, "adegan", rig.Adegan);
            Set(sutradara, "kartu", rig.Kartu);
            Set(sutradara, "dialog", rig.Dialog);
            Set(sutradara, "hud", rig.Hud);
            Set(sutradara, "dialogOjek", PembuatDataFase1.Muat(PembuatDataFase1.Ojek));

            // Gapura dan mobil yang mengantar Raka, parkir di luar gapura menghadap ke selatan.
            PembuatPengganti.Pasang("Gapura", null, Vector3.zero);
            var ojek = BuatMobil(new Vector3(1.2f, 0f, -3.2f));
            Set(sutradara, "ojek", ojek.transform);
            Set(sutradara, "bidikGapura", Titik("BidikGapura", new Vector3(-2.5f, 1.7f, 7f), new Vector3(0.5f, 1.2f, 0f)));
            // Cutscene mobil pergi (3,5 detik): mobil lurus ke z = -25 dalam 3 detik, suara mesin sejak awal.
            var ojekPergi = PembuatTimeline.Buat("Prolog_OjekPergi", 3.5f);
            PembuatTimeline.Gerak(ojekPergi.Tl, ojekPergi.Director, ojek,
                (0f, ojek.transform), (3f, Titik("TitikOjekPergi", new Vector3(1.5f, ojek.transform.position.y, -25f), new Vector3(1.5f, ojek.transform.position.y, -30f))));
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
            // Kayu: yang bisa diinjak dari luar hanya lantai emper (teras).
            PembuatPengganti.Pasang("RumahKeluargaLuar", null, new Vector3(0f, 0f, UjungJalan), 180f)
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Kayu;
            var pintu = Balok("PintuRumah", new Vector3(0f, 1.05f, UjungJalan - 0.15f), new Vector3(1.1f, 2.1f, 0.2f), "KayuGelap");
            var alurPintu = pintu.AddComponent<InteraktifAlur>();
            SetTeks(alurPintu, "prompt", "Masuk rumah");
            Set(sutradara, "pintuRumah", alurPintu);

            // Suara. Azan tidak punya pengganti sintetis; selama filenya belum ada hanya teks ini yang tampil.
            // Tidak ada suara telepon: suara tokoh ditunda, telepon Laras hanya kartu teks.
            Set(sutradara, "suaraMotor", BuatSuara(rig, "MobilPergi", Sfx, "SFX", "(Suara: mobil menjauh.)"));
            Set(sutradara, "suaraAzan", BuatSuara(rig, "AzanMagrib", Sfx, "Ambience", "(Suara: azan magrib dari kejauhan.)"));
            var suaraPintu = BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)");
            Set(sutradara, "suaraPintu", suaraPintu);
            Malam3(rig, alurPintu, suaraPintu);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene desa dibuat: {JalurScene}");
        }

        // Malam 3 (K17-B): Raka berjalan dari rumah keluarga ke rumah Bu Sumi (pintu di (7,5; 73,5), menghadap jalan) dan kembali.
        // Pintu rumah keluarga dipakai bersama AlurProlog; AlurProlog mati di luar Prolog dan melepas langganannya.
        static void Malam3(Rig rig, InteraktifAlur pintuRumah, Suara suaraPintu)
        {
            var alur = new GameObject("Sutradara Malam 3").AddComponent<AlurDesaMalam3>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "pintuRumah", pintuRumah);
            Set(alur, "suaraPintu", suaraPintu);
            Set(alur, "suaraKetuk", BuatSuara(rig, "KetukPintu", Sfx, "SFX", "(Suara: ketukan di pintu.)"));

            var pintu = Logika("PintuBuSumi", new Vector3(7.3f, 0f, 73.5f), new Vector3(0.25f, 2.2f, 1f));
            var ketuk = pintu.AddComponent<InteraktifAlur>();
            SetTeks(ketuk, "prompt", "Ketuk pintu Bu Sumi");
            Set(alur, "pintuBuSumi", ketuk);
            Set(alur, "titikDepanRumah", Titik("TitikDepanRumahMalam3", new Vector3(0f, 0.1f, 96.8f), new Vector3(0f, 0.1f, 80f)));
            Set(alur, "titikDepanBuSumi", Titik("TitikDepanBuSumi", new Vector3(6.2f, 0.05f, 73.5f), new Vector3(0f, 0.05f, 73.5f)));
        }

        // Latar Scene_MainMenu: desa malam tanpa pemain dan tanpa logika (hanya tampilan).
        internal static void Latar()
        {
            Lingkungan();
            PembuatPengganti.Pasang("Gapura", null, Vector3.zero);
            PembuatPengganti.Pasang("RumahKecil", null, new Vector3(10f, 0f, 75f), -90f);
            Balok("CahayaJendela", new Vector3(7.47f, 1.5f, 76.2f), new Vector3(0.02f, 1.1f, 1.1f), "Kain");
            Lampu("LampuJendela", new Vector3(7.1f, 1.7f, 76.2f), "FFB46B", 4f, 2f);
            PembuatPengganti.Pasang("RumahKeluargaLuar", null, new Vector3(0f, 0f, UjungJalan), 180f);
        }

        static void Lingkungan()
        {
            var bulan = new GameObject("CahayaBulan").AddComponent<Light>();
            bulan.type = LightType.Directional;
            bulan.intensity = 0.15f;
            bulan.color = Hex("6D86A8");
            bulan.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Tanah hutan sampai sekitar 40 m dari jalan (kabut menelan sisanya), dan jalan tanah di koridor.
            Balok("Tanah", new Vector3(0f, -0.1f, 55f), new Vector3(100f, 0.2f, 200f), "TanahHutan");
            Jalan("Jalan", -45f, 48.5f);
            Jalan("Jalan", 55.5f, 100f);
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
            // Sungai berlanjut ke dalam hutan di kedua sisi.
            var air = Balok("Air", new Vector3(0f, 0.01f, 52f), new Vector3(100f, 0.02f, 6f), "Gelap");
            Object.DestroyImmediate(air.GetComponent<Collider>());
            PembuatPengganti.Pasang("JembatanBambu", null, new Vector3(0f, 0f, 52f))
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Kayu;
            Batas("BatasAirBarat", new Vector3(-3.75f, 1f, 52f), new Vector3(5.5f, 2f, 6.2f));
            Batas("BatasAirTimur", new Vector3(3.75f, 1f, 52f), new Vector3(5.5f, 2f, 6.2f));
            Hutan();
        }

        static void Jalan(string nama, float z1, float z2)
        {
            var g = Balok(nama, new Vector3(0f, 0.005f, (z1 + z2) / 2f), new Vector3(5f, 0.01f, z2 - z1), "Jalan");
            Object.DestroyImmediate(g.GetComponent<Collider>());
        }

        const string Alam = PembuatAsetAsli.Alam;
        static readonly string[] Pohon = { "SawtoothOak", "PlaneTree", "Ginkgo_wood" };
        static readonly string[] Semak = { "JapaneseLaurel", "JapaneseEurya", "Mugwort", "ViolaGrypoceras" };
        static readonly string[] Batu = { "Rock_A", "Rock_B", "Rock_C", "Rock_D" };
        static readonly string[] Kayu = { "Decayed_wood_A", "Decayed_wood_B", "Decayed_wood_C", "Decayed_wood_D", "Decayed_wood_E" };

        // Hutan di luar dinding koridor (|x| > 6.5): pohon berdaun lebar, semak, batu, kayu lapuk, dan daun gugur.
        // Seed tetap: hasilnya sama setiap kali dibangun. Tanpa collider (pemain tidak bisa keluar koridor).
        // Tetap kosong: jalan mobil, sungai, rumah Bu Sumi dan halamannya, rumah keluarga.
        static void Hutan()
        {
            var induk = new GameObject("Hutan").transform;
            var acak = new System.Random(2003);
            float Acak(float a, float b) => a + (float)acak.NextDouble() * (b - a);
            string Pilih(string[] daftar) => daftar[acak.Next(daftar.Length)];

            const float jarak = 5f;
            for (float z = -45f; z <= 150f; z += jarak)
                for (float jauh = 7f; jauh <= 45f; jauh += jarak)
                    foreach (float sisi in new[] { -1f, 1f })
                    {
                        float x = sisi * (jauh + Acak(0f, jarak - 1f)), pz = z + Acak(0f, jarak - 1f);
                        if (Terlarang(x, pz)) continue;
                        double jenis = acak.NextDouble();
                        float putar = Acak(0f, 360f);
                        var posisi = new Vector3(x, 0f, pz);
                        if (jenis < 0.55) PembuatAsetAsli.Vendor(Alam + Pilih(Pohon) + ".prefab", induk, posisi, putar, Acak(0.85f, 1.3f));
                        else if (jenis < 0.8) PembuatAsetAsli.Vendor(Alam + Pilih(Semak) + ".prefab", induk, posisi, putar, Acak(0.8f, 1.4f));
                        else if (jenis < 0.9) PembuatAsetAsli.Vendor(Alam + Pilih(Batu) + ".prefab", induk, posisi, putar, Acak(0.7f, 1.3f));
                        else PembuatAsetAsli.Vendor(Alam + Pilih(Kayu) + ".prefab", induk, posisi, putar, 1f);
                        if (acak.NextDouble() < 0.4)
                            PembuatAsetAsli.Vendor(Alam + "FallenLeaves.prefab", induk, posisi + new Vector3(Acak(-2f, 2f), 0f, Acak(-2f, 2f)), Acak(0f, 360f), Acak(1f, 1.8f));
                    }

            // Di tepi jalan (dalam koridor) hanya daun gugur dan bunga kecil setinggi mata kaki.
            for (float z = 4f; z <= 92f; z += 6f)
                foreach (float sisi in new[] { -1f, 1f })
                {
                    float x = sisi * Acak(4.5f, 6.2f), pz = z + Acak(0f, 4f);
                    if (pz > 46f && pz < 58f) continue;
                    var nama = acak.NextDouble() < 0.6 ? "FallenLeaves" : "ViolaGrypoceras";
                    PembuatAsetAsli.Vendor(Alam + nama + ".prefab", induk, new Vector3(x, 0f, pz), Acak(0f, 360f), 1f);
                }
        }

        static bool Terlarang(float x, float z) =>
            Mathf.Abs(x) < SetengahLebar + 0.5f
            || (z > 46.5f && z < 57.5f)                              // sungai
            || (x > 6f && x < 16f && z > 67f && z < 83f)            // rumah Bu Sumi
            || (Mathf.Abs(x) < 11f && z > 95f && z < 116f);         // rumah keluarga

        // Model mobil Sketchfab (diimpor glTFast, sudah berskala meter, depan di +Z). Tanpa model: pakai prefab pengganti.
        const string ModelMobil = "Assets/_Project/Art/Models/Desa/MobilLMPV.glb";

        static GameObject BuatMobil(Vector3 posisi)
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelMobil);
            if (model == null) Debug.LogWarning($"{ModelMobil} belum terimpor (paket glTFast?); memakai prefab pengganti Ojek.");
            var mobil = model != null ? (GameObject)PrefabUtility.InstantiatePrefab(model) : PembuatPengganti.Pasang("Ojek", null, Vector3.zero);
            mobil.name = "Ojek";
            mobil.transform.SetPositionAndRotation(posisi, Quaternion.Euler(0f, 180f, 0f));
            // Pivot glb ada di tengah tinggi mobil: angkat supaya roda di posisi.y. Kunci Timeline memakai tinggi ini juga.
            float bawah = float.MaxValue;
            foreach (var r in mobil.GetComponentsInChildren<Renderer>()) bawah = Mathf.Min(bawah, r.bounds.min.y);
            if (bawah != float.MaxValue) mobil.transform.position += Vector3.up * (posisi.y - bawah);
            return mobil;
        }
    }
}
