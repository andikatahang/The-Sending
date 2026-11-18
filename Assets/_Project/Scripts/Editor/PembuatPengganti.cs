using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheSending.EditorTools
{
    // Aset pengganti sampai tim mendapat aset asli: material polos dan prefab dari primitif.
    // TIDAK menimpa file yang sudah ada, supaya prefab dan material yang sudah ditukar tim aman.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatPengganti.Buat
    public static class PembuatPengganti
    {
        const string FolderMaterial = "Assets/_Project/Art/Materials/Pengganti";
        const string FolderPrefab = "Assets/_Project/Prefabs/Pengganti";
        internal const string FolderSfx = "Assets/_Project/Audio/SFX", FolderMusik = "Assets/_Project/Audio/Music";
        static readonly string[] AkhiranSuara = { ".wav", ".ogg", ".mp3" };

        static readonly (string nama, string hex)[] DaftarMaterial =
        {
            ("KayuTua", "5B4030"), ("KayuGelap", "2E2018"), ("Gedek", "8A7A55"), ("Tanah", "3B3026"),
            ("Genteng", "6B3A2A"), ("Bambu", "6F7F3A"), ("Kain", "B9AE96"), ("Kulit", "C69C7B"),
            ("Karat", "5A1A12"), ("Kertas", "E6E0D1"), ("Logam", "6E7074"), ("Gelap", "0B0D10"),
            ("Atap", "4A3B2C"), ("Tembok", "5A5850"), ("TanahHutan", "33302A"), ("Jalan", "4A4038"),
        };

        readonly struct Bagian
        {
            public readonly PrimitiveType Bentuk;
            public readonly string Material;
            public readonly Vector3 Posisi, Ukuran, Putar;

            public Bagian(PrimitiveType bentuk, string material, Vector3 posisi, Vector3 ukuran, Vector3 putar)
            {
                Bentuk = bentuk; Material = material; Posisi = posisi; Ukuran = ukuran; Putar = putar;
            }
        }

        // K = kotak, T = tabung (tinggi = 2 x sy), B = bola. Posisi = titik tengah bagian.
        static Bagian K(string m, float x, float y, float z, float sx, float sy, float sz, float rx = 0f, float ry = 0f, float rz = 0f) =>
            new Bagian(PrimitiveType.Cube, m, new Vector3(x, y, z), new Vector3(sx, sy, sz), new Vector3(rx, ry, rz));
        static Bagian T(string m, float x, float y, float z, float sx, float sy, float sz, float rx = 0f, float ry = 0f, float rz = 0f) =>
            new Bagian(PrimitiveType.Cylinder, m, new Vector3(x, y, z), new Vector3(sx, sy, sz), new Vector3(rx, ry, rz));
        static Bagian B(string m, float x, float y, float z, float s) =>
            new Bagian(PrimitiveType.Sphere, m, new Vector3(x, y, z), Vector3.one * s, Vector3.zero);

        // Sosok berdiri setinggi 1,67 m dari kotak-kotak.
        static Bagian[] Sosok(string baju, string bawah, string kulit = "Kulit") => new[]
        {
            K(bawah, 0f, 0.4f, 0f, 0.3f, 0.8f, 0.2f),
            K(baju, 0f, 1.1f, 0f, 0.4f, 0.6f, 0.22f),
            K(baju, -0.27f, 1.1f, 0f, 0.1f, 0.55f, 0.1f),
            K(baju, 0.27f, 1.1f, 0f, 0.1f, 0.55f, 0.1f),
            K(kulit, 0f, 1.52f, 0f, 0.22f, 0.24f, 0.22f),
            K("Gelap", 0f, 1.62f, -0.02f, 0.24f, 0.1f, 0.24f),
        };

        static readonly (string nama, Bagian[] bagian)[] DaftarPrefab =
        {
            // Tokoh
            ("Laras", Sosok("Kain", "KayuGelap")),
            ("Laras_Berdoa", new[]
            {
                K("KayuGelap", 0f, 0.15f, -0.2f, 0.3f, 0.3f, 0.5f),
                K("Kain", 0f, 0.6f, 0f, 0.4f, 0.6f, 0.22f),
                K("Kain", 0f, 0.75f, 0.18f, 0.3f, 0.1f, 0.25f),
                K("Kulit", 0f, 1.0f, 0.03f, 0.22f, 0.24f, 0.22f, 20f),
                K("Gelap", 0f, 1.1f, 0f, 0.24f, 0.1f, 0.24f, 20f),
            }),
            ("BidanWulan", Sosok("Kertas", "Logam")),
            ("BuRatmi_Berbaring", new[]
            {
                K("Kain", 0f, 0.12f, 0.1f, 0.45f, 0.2f, 1.3f),
                K("Kulit", 0f, 0.12f, -0.72f, 0.22f, 0.2f, 0.24f),
                K("Gelap", 0f, 0.1f, -0.86f, 0.24f, 0.18f, 0.08f),
            }),
            ("BuSumi_Siluet", Sosok("Gelap", "Gelap", "Gelap")),
            ("Ojek", new[]
            {
                K("Logam", 0f, 0.5f, 0f, 0.25f, 0.4f, 1.6f),
                T("Gelap", 0f, 0.3f, 0.7f, 0.6f, 0.05f, 0.6f, 0f, 0f, 90f),
                T("Gelap", 0f, 0.3f, -0.7f, 0.6f, 0.05f, 0.6f, 0f, 0f, 90f),
                K("Kertas", 0f, 0.85f, 0.8f, 0.15f, 0.12f, 0.05f),
                K("KayuGelap", 0f, 1.15f, -0.1f, 0.4f, 0.6f, 0.25f),
                B("Gelap", 0f, 1.6f, -0.1f, 0.3f),
            }),
            // Perabot dan benda rumah
            ("Ranjang", new[]
            {
                K("KayuTua", 0f, 0.25f, 0f, 1.2f, 0.3f, 2.0f),
                K("Kain", 0f, 0.47f, 0f, 1.1f, 0.14f, 1.9f),
                K("KayuGelap", 0f, 0.6f, -1.0f, 1.2f, 0.8f, 0.08f),
            }),
            ("LemariAyah", new[]
            {
                K("KayuGelap", 0f, 1.0f, 0f, 1.2f, 2.0f, 0.6f),
                K("KayuTua", 0f, 1.0f, 0.305f, 0.03f, 1.8f, 0.01f),
            }),
            ("Gembok", new[]
            {
                K("Logam", 0f, 0f, 0f, 0.1f, 0.12f, 0.04f),
                K("Logam", 0f, 0.09f, 0f, 0.07f, 0.06f, 0.02f),
            }),
            ("FotoKeluarga", new[]
            {
                K("KayuGelap", 0f, 0f, 0f, 0.6f, 0.45f, 0.04f),
                K("Kertas", 0f, 0f, 0.022f, 0.5f, 0.35f, 0.01f),
            }),
            ("JamDinding", new[]
            {
                K("KayuGelap", 0f, 0f, 0f, 0.3f, 0.6f, 0.1f),
                T("Kertas", 0f, 0.12f, 0.055f, 0.22f, 0.005f, 0.22f, 90f),
                K("Logam", 0f, -0.15f, 0.055f, 0.04f, 0.2f, 0.01f),
            }),
            ("Kalender", new[]
            {
                K("Kertas", 0f, 0f, 0f, 0.35f, 0.5f, 0.02f),
                K("Karat", 0f, 0.2f, 0.012f, 0.35f, 0.1f, 0.01f),
            }),
            ("Meja", new[]
            {
                K("KayuTua", 0f, 0.73f, 0f, 1.0f, 0.05f, 0.6f),
                K("KayuTua", -0.45f, 0.35f, -0.25f, 0.06f, 0.7f, 0.06f),
                K("KayuTua", 0.45f, 0.35f, -0.25f, 0.06f, 0.7f, 0.06f),
                K("KayuTua", -0.45f, 0.35f, 0.25f, 0.06f, 0.7f, 0.06f),
                K("KayuTua", 0.45f, 0.35f, 0.25f, 0.06f, 0.7f, 0.06f),
            }),
            ("Kursi", new[]
            {
                K("KayuGelap", 0f, 0.22f, 0f, 0.34f, 0.44f, 0.34f),
                K("KayuTua", 0f, 0.45f, 0f, 0.4f, 0.05f, 0.4f),
                K("KayuTua", 0f, 0.75f, -0.18f, 0.4f, 0.55f, 0.04f),
            }),
            ("Paku", new[]
            {
                T("Karat", -0.08f, 0.02f, 0.03f, 0.02f, 0.07f, 0.02f, 90f, 25f),
                T("Karat", 0.05f, 0.02f, -0.04f, 0.02f, 0.07f, 0.02f, 90f, -40f),
                T("Karat", 0.1f, 0.02f, 0.09f, 0.02f, 0.07f, 0.02f, 90f, 80f),
            }),
            ("Teplok", new[]
            {
                T("Logam", 0f, 0.06f, 0f, 0.1f, 0.06f, 0.1f),
                T("Kertas", 0f, 0.2f, 0f, 0.06f, 0.08f, 0.06f),
            }),
            ("Laci", new[]
            {
                K("KayuGelap", 0f, 0.425f, 0f, 0.9f, 0.85f, 0.45f),
                K("KayuTua", 0f, 0.6f, 0.23f, 0.8f, 0.02f, 0.01f),
            }),
            ("StopKontak", new[] { K("Kertas", 0f, 0f, 0f, 0.1f, 0.1f, 0.03f) }),
            ("SakaGuru", new[]
            {
                K("Tanah", 0f, 0.1f, 0f, 0.45f, 0.2f, 0.45f),
                K("KayuGelap", 0f, 1.5f, 0f, 0.3f, 3.0f, 0.3f),
            }),
            // Jalan masuk desa
            ("Gapura", new[]
            {
                K("Genteng", -2.5f, 1.75f, 0f, 0.8f, 3.5f, 0.8f),
                K("Genteng", 2.5f, 1.75f, 0f, 0.8f, 3.5f, 0.8f),
                K("KayuGelap", -2.5f, 3.6f, 0f, 1.1f, 0.2f, 1.1f),
                K("KayuGelap", 2.5f, 3.6f, 0f, 1.1f, 0.2f, 1.1f),
                K("KayuTua", 0f, 3.3f, 0f, 5.0f, 0.25f, 0.3f),
            }),
            ("JembatanBambu", new[]
            {
                K("Bambu", 0f, 0.05f, 0f, 2.0f, 0.1f, 8.0f),
                K("Bambu", -1.0f, 0.3f, 0f, 0.08f, 0.08f, 8.0f),
                K("Bambu", -1.0f, 0.7f, 0f, 0.08f, 0.08f, 8.0f),
                K("Bambu", 1.0f, 0.3f, 0f, 0.08f, 0.08f, 8.0f),
                K("Bambu", 1.0f, 0.7f, 0f, 0.08f, 0.08f, 8.0f),
            }),
            ("RumpunBambu", new[]
            {
                T("Bambu", 0f, 2.5f, 0f, 0.1f, 2.5f, 0.1f),
                T("Bambu", 0.3f, 2.5f, 0.2f, 0.1f, 2.5f, 0.1f, 0f, 0f, 4f),
                T("Bambu", -0.3f, 2.5f, 0.1f, 0.1f, 2.5f, 0.1f, 0f, 0f, -4f),
                T("Bambu", 0.1f, 2.5f, -0.3f, 0.1f, 2.5f, 0.1f, 4f),
                T("Bambu", -0.2f, 2.5f, -0.2f, 0.1f, 2.5f, 0.1f, -3f),
            }),
            ("LampuJalan", new[]
            {
                T("KayuGelap", 0f, 1.5f, 0f, 0.08f, 1.5f, 0.08f),
                K("KayuGelap", 0.25f, 2.9f, 0f, 0.5f, 0.06f, 0.06f),
                K("Logam", 0.5f, 2.8f, 0f, 0.25f, 0.12f, 0.25f),
            }),
            // Rumah Bu Sumi dari luar: 6 x 3 x 5 m, jendela di (1.2, 1.5, 2.5).
            ("RumahKecil", new[]
            {
                K("Gedek", 0f, 1.5f, 0f, 6f, 3f, 5f),
                K("Genteng", 0f, 3.3f, 0f, 6.6f, 0.6f, 5.6f),
                K("KayuGelap", -1.5f, 1.0f, 2.51f, 0.9f, 2.0f, 0.04f),
                K("KayuGelap", 1.2f, 1.5f, 2.51f, 1.3f, 1.3f, 0.04f),
                K("Gelap", 1.2f, 1.5f, 2.535f, 1.1f, 1.1f, 0.01f),
            }),
            // Rumah keluarga dari luar: titik nol di ambang pintu depan, badan rumah ke arah -Z.
            ("RumahKeluargaLuar", new[]
            {
                K("Gedek", 0f, 1.5f, -6f, 14f, 3f, 12f),
                K("Genteng", 0f, 3.6f, -6f, 15f, 1.2f, 13f),
                K("KayuGelap", -2f, 1.5f, 1.2f, 0.25f, 3f, 0.25f),
                K("KayuGelap", 2f, 1.5f, 1.2f, 0.25f, 3f, 0.25f),
                K("Genteng", 0f, 3.1f, 0.8f, 6f, 0.15f, 2f),
                K("KayuGelap", 0f, 1.1f, 0.02f, 1.2f, 2.2f, 0.06f),
            }),
        };

        [MenuItem("The Sending/Buat Aset Pengganti")]
        public static void Buat()
        {
            foreach (var folder in new[] { FolderMaterial, FolderPrefab, FolderSfx, FolderMusik })
                Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
            foreach (var m in DaftarMaterial) Material(m.nama);
            foreach (var p in DaftarPrefab) BuatPrefab(p.nama, p.bagian);
            PembuatSuaraPengganti.Buat();
            AssetDatabase.SaveAssets();
            Debug.Log($"Aset pengganti siap: {DaftarMaterial.Length} material, {DaftarPrefab.Length} prefab.");
        }

        internal static Material Material(string nama)
        {
            string jalur = $"{FolderMaterial}/{nama}.mat";
            var ada = AssetDatabase.LoadAssetAtPath<Material>(jalur);
            if (ada != null) return ada;

            string hex = Array.Find(DaftarMaterial, d => d.nama == nama).hex
                ?? throw new ArgumentException($"Material pengganti '{nama}' tidak ada di daftar.");
            var shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? throw new InvalidOperationException("Shader 'Universal Render Pipeline/Lit' tidak ditemukan.");
            var baru = new Material(shader);
            if (!ColorUtility.TryParseHtmlString("#" + hex, out var warna))
                throw new FormatException($"Warna material '{nama}' salah: {hex}");
            baru.SetColor("_BaseColor", warna);
            baru.SetFloat("_Smoothness", 0f);
            AssetDatabase.CreateAsset(baru, jalur);
            return baru;
        }

        // Menaruh prefab visual sebagai anak objek logika (induk null = di akar scene). Posisi dan putaran lokal.
        // Prefab aset asli (Prefabs/Asli, PembuatAsetAsli) dipakai bila ada; jika tidak, prefab pengganti.
        internal static GameObject Pasang(string namaPrefab, Transform induk, Vector3 posisiLokal, float putarY = 0f)
        {
            string jalur = $"{FolderPrefab}/{namaPrefab}.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PembuatAsetAsli.FolderPrefab}/{namaPrefab}.prefab")
                ?? AssetDatabase.LoadAssetAtPath<GameObject>(jalur)
                ?? throw new FileNotFoundException($"Prefab pengganti belum ada: {jalur}. Jalankan PembuatPengganti.Buat.");
            var g = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            g.transform.SetParent(induk, false);
            g.transform.localPosition = posisiLokal;
            g.transform.localRotation = Quaternion.Euler(0f, putarY, 0f);
            return g;
        }

        internal static bool AdaSuara(string folder, string nama) =>
            Array.Exists(AkhiranSuara, akhiran => File.Exists($"{folder}/{nama}{akhiran}"));

        // Klip suara dengan nama dasar itu (.wav, .ogg, atau .mp3). Null = belum ada; adegan memakai teks pengganti.
        internal static AudioClip Klip(string folder, string nama)
        {
            foreach (var akhiran in AkhiranSuara)
            {
                var klip = AssetDatabase.LoadAssetAtPath<AudioClip>($"{folder}/{nama}{akhiran}");
                if (klip != null) return klip;
            }
            return null;
        }

        static void BuatPrefab(string nama, Bagian[] bagian)
        {
            string jalur = $"{FolderPrefab}/{nama}.prefab";
            if (File.Exists(jalur)) return;   // sudah ada (mungkin sudah ditukar tim): jangan ditimpa

            var akar = new GameObject(nama);
            foreach (var b in bagian)
            {
                var g = GameObject.CreatePrimitive(b.Bentuk);
                g.name = b.Material;
                g.transform.SetParent(akar.transform, false);
                g.transform.localPosition = b.Posisi;
                g.transform.localRotation = Quaternion.Euler(b.Putar);
                g.transform.localScale = b.Ukuran;
                g.GetComponent<Renderer>().sharedMaterial = Material(b.Material);
            }
            PrefabUtility.SaveAsPrefabAsset(akar, jalur);
            Object.DestroyImmediate(akar);
        }
    }
}
