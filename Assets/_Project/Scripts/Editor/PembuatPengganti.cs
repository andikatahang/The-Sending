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
            ("KayuTua", "5B4030"), ("Linoleum", "8A8068"), ("KayuGelap", "2E2018"), ("Gedek", "8A7A55"), ("Tanah", "3B3026"),
            ("Genteng", "6B3A2A"), ("Bambu", "6F7F3A"), ("Kain", "B9AE96"), ("Kulit", "C69C7B"),
            ("Karat", "5A1A12"), ("Kertas", "E6E0D1"), ("Logam", "6E7074"), ("Gelap", "0B0D10"),
            ("Atap", "4A3B2C"), ("Tembok", "5A5850"), ("TanahHutan", "33302A"), ("Jalan", "4A4038"), ("Kafan", "4A4438"), ("Batik", "6B3A26"), ("Daun", "3E5A2E"), ("BambuKuning", "B59A3A"), ("Api", "E8742A"),
            ("Sofa", "6A5A48"), ("BatuPutih", "A8A49A"), ("Terakota", "7A3420"),
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
                K("KayuTua", 0f, 0.35f, 0f, 1.2f, 0.1f, 2.0f),          // dipan; kolong setinggi 0,3 m untuk bersembunyi
                K("KayuTua", -0.55f, 0.15f, -0.95f, 0.08f, 0.3f, 0.08f),
                K("KayuTua", 0.55f, 0.15f, -0.95f, 0.08f, 0.3f, 0.08f),
                K("KayuTua", -0.55f, 0.15f, 0.95f, 0.08f, 0.3f, 0.08f),
                K("KayuTua", 0.55f, 0.15f, 0.95f, 0.08f, 0.3f, 0.08f),
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
            ("LampuNeon", new[] { K("Kain", 0f, 0.04f, 0f, 1.2f, 0.08f, 0.15f) }),   // lampu neon plafon, alas di titik nol
            ("LampuTeras", new[] { K("Logam", 0f, 0.1f, 0f, 0.25f, 0.2f, 0.25f) }),
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
            // Malam 2
            ("SosokSantet", new[]
            {
                T("Gelap", 0f, 1.0f, 0f, 0.3f, 1.0f, 0.26f),                   // tubuh kurus 2 m
                K("Kafan", 0f, 0.95f, 0f, 0.4f, 1.6f, 0.32f, 0f, 8f),          // lilitan kain kafan kotor
                K("Gelap", 0f, 2.07f, 0f, 0.24f, 0.26f, 0.24f),                // kepala, puncak 2,2 m
                K("Gelap", 0f, 1.8f, 0.13f, 0.3f, 0.75f, 0.05f),               // rambut menutup wajah
                K("Gelap", -0.25f, 1.25f, 0.04f, 0.06f, 0.95f, 0.06f, 0f, 0f, 4f),
                K("Gelap", 0.25f, 1.25f, 0.04f, 0.06f, 0.95f, 0.06f, 0f, 0f, -4f),
            }),
            ("MbahWiryo", new[]
            {
                K("Gelap", 0f, 0.4f, 0f, 0.3f, 0.8f, 0.2f),
                K("Gelap", 0f, 1.1f, 0f, 0.42f, 0.6f, 0.22f),
                K("Gelap", -0.27f, 1.1f, 0f, 0.1f, 0.55f, 0.1f),
                K("Gelap", 0.27f, 1.1f, 0f, 0.1f, 0.55f, 0.1f),
                K("Kulit", 0f, 1.52f, 0f, 0.22f, 0.24f, 0.22f),
                K("KayuGelap", 0f, 1.66f, 0f, 0.25f, 0.08f, 0.25f),             // blangkon
                K("KayuGelap", 0f, 1.62f, -0.13f, 0.1f, 0.08f, 0.06f),          // mondolan blangkon
            }),
            ("Boneka", new[]
            {
                K("Kain", 0f, 0.035f, 0f, 0.12f, 0.06f, 0.2f),
                B("Kain", 0f, 0.05f, 0.14f, 0.09f),
                K("Karat", 0f, 0.036f, 0.03f, 0.125f, 0.065f, 0.02f),         // lilitan benang merah
                K("Karat", 0f, 0.036f, -0.05f, 0.125f, 0.065f, 0.02f),
                T("Logam", 0f, 0.1f, 0.14f, 0.008f, 0.04f, 0.008f),           // jarum di kepala
                T("Logam", 0f, 0.085f, -0.01f, 0.008f, 0.04f, 0.008f),        // jarum di perut
            }),
            ("Linggis", new[]
            {
                T("Logam", 0f, 0.6f, 0f, 0.03f, 0.6f, 0.03f, 0f, 0f, 8f),
                K("Logam", 0.06f, 0.04f, 0f, 0.1f, 0.03f, 0.03f),
            }),
            ("Karung", new[]
            {
                K("Kain", -0.35f, 0.25f, 0f, 0.7f, 0.5f, 0.8f),
                K("Kain", 0.35f, 0.25f, 0f, 0.7f, 0.5f, 0.8f, 0f, 6f),
                K("Kain", 0f, 0.72f, 0f, 0.7f, 0.45f, 0.75f, 0f, -5f),
            }),
            ("AlatTani", new[]
            {
                K("KayuGelap", 0f, 1.0f, 0f, 1.6f, 0.06f, 0.3f),              // rak dinding
                T("KayuTua", -0.5f, 0.75f, 0.05f, 0.04f, 0.75f, 0.04f),        // pacul
                K("Logam", -0.5f, 0.1f, 0.12f, 0.2f, 0.2f, 0.02f),
                T("KayuTua", 0.1f, 0.6f, 0.05f, 0.03f, 0.6f, 0.03f),           // sabit
                K("Logam", 0.18f, 1.18f, 0.05f, 0.18f, 0.04f, 0.02f),
                T("KayuTua", 0.6f, 0.8f, 0.05f, 0.04f, 0.8f, 0.04f),           // garu
                K("Logam", 0.6f, 0.05f, 0.1f, 0.35f, 0.08f, 0.05f),
            }),
            ("PapanLantai", new[] { K("KayuTua", 0f, 0.012f, 0f, 0.3f, 0.024f, 1.2f) }),
            ("LubangLantai", new[] { K("Gelap", 0f, 0.003f, 0f, 0.3f, 0.006f, 1.2f) }),
            ("KotakKayu", new[]
            {
                K("KayuTua", 0f, 0.15f, 0f, 0.5f, 0.3f, 0.35f),
                K("KayuGelap", 0f, 0.31f, 0f, 0.52f, 0.03f, 0.37f),
            }),
            ("LemariPakaian", new[]
            {
                K("KayuGelap", 0f, 1.0f, 0f, 1.0f, 2.0f, 0.6f),
                K("KayuTua", 0f, 1.0f, 0.305f, 0.02f, 1.8f, 0.01f),
                K("Logam", -0.06f, 1.0f, 0.31f, 0.02f, 0.12f, 0.02f),
                K("Logam", 0.06f, 1.0f, 0.31f, 0.02f, 0.12f, 0.02f),
            }),
            // Malam 3: tokoh
            ("PakdeDarmo", Sosok("Batik", "KayuGelap")),
            ("BuSumi", new[]
            {
                K("Kain", 0f, 0.4f, 0f, 0.32f, 0.8f, 0.22f),                   // jarik
                K("Kain", 0f, 1.08f, 0f, 0.38f, 0.56f, 0.22f),                 // kebaya lusuh
                K("Kain", -0.25f, 1.08f, 0f, 0.09f, 0.52f, 0.09f),
                K("Kain", 0.25f, 1.08f, 0f, 0.09f, 0.52f, 0.09f),
                K("Kulit", 0f, 1.47f, 0f, 0.2f, 0.22f, 0.2f),
                K("Kertas", 0f, 1.57f, -0.02f, 0.22f, 0.08f, 0.22f),           // rambut putih
                B("Kertas", 0f, 1.53f, -0.14f, 0.13f),                         // sanggul
            }),
            // Malam 3: rumah keluarga
            ("BuahTangan", new[]
            {
                K("Kertas", 0f, 0.06f, 0f, 0.3f, 0.12f, 0.2f),
                B("Karat", -0.06f, 0.15f, 0f, 0.1f),
                B("Daun", 0.07f, 0.15f, 0.02f, 0.1f),
            }),
            ("KotakSepatu", new[]
            {
                K("Kertas", 0f, 0.06f, 0f, 0.32f, 0.12f, 0.2f),
                K("Tanah", 0f, 0.125f, 0f, 0.33f, 0.01f, 0.21f),
            }),
            ("Tang", new[]
            {
                K("Logam", 0f, 0.01f, 0.05f, 0.04f, 0.02f, 0.1f),
                K("Karat", -0.02f, 0.01f, -0.05f, 0.02f, 0.02f, 0.12f, 0f, 10f),
                K("Karat", 0.02f, 0.01f, -0.05f, 0.02f, 0.02f, 0.12f, 0f, -10f),
            }),
            ("HalamanSobek", new[] { K("Kertas", 0f, 0.003f, 0f, 0.15f, 0.006f, 0.2f, 0f, 12f) }),
            ("Dempul", new[] { K("Gelap", 0f, 0f, 0f, 0.02f, 0.95f, 0.2f) }),
            ("PakuSaka", new[] { T("Karat", 0f, 0f, 0f, 0.015f, 0.04f, 0.015f, 0f, 0f, 90f) }),
            ("Anglo", new[]
            {
                T("Tanah", 0f, 0.15f, 0f, 0.4f, 0.15f, 0.4f),
                T("Gelap", 0f, 0.305f, 0f, 0.32f, 0.005f, 0.32f),
            }),
            ("ApiAnglo", new[]
            {
                B("Api", 0f, 0.36f, 0f, 0.18f),
                B("Api", 0.05f, 0.42f, 0.03f, 0.1f),
            }),
            ("RakBumbu", new[]
            {
                K("KayuTua", 0f, 0.01f, 0f, 0.45f, 0.02f, 0.2f),
                T("Kertas", -0.12f, 0.08f, 0f, 0.08f, 0.06f, 0.08f),          // toples garam
                T("Karat", 0f, 0.07f, 0f, 0.07f, 0.05f, 0.07f),
                T("Tanah", 0.12f, 0.07f, 0f, 0.07f, 0.05f, 0.07f),
            }),
            // Langkah Ritual Penangkal di meja samping anglo (K25-B)
            ("GaramToples", new[]
            {
                T("Kertas", 0f, 0.07f, 0f, 0.09f, 0.07f, 0.09f),
                T("KayuTua", 0f, 0.145f, 0f, 0.1f, 0.005f, 0.1f),
            }),
            ("IkatKelor", new[]
            {
                B("Daun", 0f, 0.04f, 0f, 0.12f),
                B("Daun", 0.06f, 0.04f, 0.03f, 0.09f),
                T("KayuTua", -0.08f, 0.02f, 0f, 0.01f, 0.06f, 0.01f, 0f, 0f, 90f),
            }),
            ("KorekApi", new[] { K("Kertas", 0f, 0.01f, 0f, 0.05f, 0.02f, 0.035f), K("Karat", 0.026f, 0.01f, 0f, 0.002f, 0.018f, 0.03f) }),
            ("BambuKuning", new[] { K("BambuKuning", 0f, 0.6f, 0f, 0.05f, 1.2f, 0.03f, 0f, 0f, 8f) }),
            ("PohonKelor", new[]
            {
                T("KayuTua", 0f, 1.1f, 0f, 0.12f, 1.1f, 0.12f),
                B("Daun", 0f, 2.4f, 0f, 1.3f),
                B("Daun", 0.4f, 2.1f, 0.2f, 0.8f),
            }),
            ("Sofa", new[]
            {
                K("Sofa", 0f, 0.22f, 0f, 1.9f, 0.44f, 0.8f),                  // dudukan
                K("Sofa", 0f, 0.6f, -0.3f, 1.9f, 0.4f, 0.2f),                 // sandaran di -Z
                K("Sofa", -0.85f, 0.55f, 0.05f, 0.2f, 0.3f, 0.7f),
                K("Sofa", 0.85f, 0.55f, 0.05f, 0.2f, 0.3f, 0.7f),
            }),
            // Malam 3: rumah Bu Sumi
            ("Sumur", new[]
            {
                T("Tembok", 0f, 0.4f, 0f, 1.1f, 0.4f, 1.1f),
                T("Gelap", 0f, 0.805f, 0f, 0.85f, 0.005f, 0.85f),
                K("KayuGelap", -0.6f, 0.9f, 0f, 0.08f, 1.8f, 0.08f),
                K("KayuGelap", 0.6f, 0.9f, 0f, 0.08f, 1.8f, 0.08f),
                K("KayuGelap", 0f, 1.76f, 0f, 1.3f, 0.08f, 0.08f),
                T("KayuTua", 0.3f, 0.92f, 0.35f, 0.12f, 0.12f, 0.12f),          // timba di bibir sumur
            }),
            ("RakBotol", new[]
            {
                K("KayuGelap", 0f, 0.9f, 0f, 1.0f, 1.8f, 0.3f),
                T("Bambu", -0.3f, 1.3f, 0.12f, 0.07f, 0.1f, 0.07f),
                T("Karat", 0f, 1.3f, 0.12f, 0.07f, 0.1f, 0.07f),
                T("Daun", 0.3f, 1.3f, 0.12f, 0.07f, 0.1f, 0.07f),
                T("Kertas", -0.15f, 0.8f, 0.12f, 0.07f, 0.1f, 0.07f),
            }),
            ("Lumpang", new[]
            {
                T("Tembok", 0f, 0.15f, 0f, 0.4f, 0.15f, 0.4f),
                T("Gelap", 0f, 0.302f, 0f, 0.25f, 0.002f, 0.25f),
                T("KayuTua", 0.1f, 0.45f, 0f, 0.05f, 0.25f, 0.05f, 0f, 0f, 20f), // alu
            }),
            ("BaleBale", new[]
            {
                K("Bambu", 0f, 0.4f, 0f, 2.0f, 0.08f, 1.0f),
                K("Bambu", -0.9f, 0.18f, -0.42f, 0.08f, 0.36f, 0.08f),
                K("Bambu", 0.9f, 0.18f, -0.42f, 0.08f, 0.36f, 0.08f),
                K("Bambu", -0.9f, 0.18f, 0.42f, 0.08f, 0.36f, 0.08f),
                K("Bambu", 0.9f, 0.18f, 0.42f, 0.08f, 0.36f, 0.08f),
            }),
            // Malam 4: sumur belakang dengan kerekan (bibir 0,8 m, tiang 1,8 m)
            ("SumurTimba", new[]
            {
                T("Tembok", 0f, 0.4f, 0f, 1.1f, 0.4f, 1.1f),
                T("Gelap", 0f, 0.805f, 0f, 0.85f, 0.005f, 0.85f),
                K("KayuGelap", -0.6f, 0.9f, 0f, 0.1f, 1.8f, 0.1f),
                K("KayuGelap", 0.6f, 0.9f, 0f, 0.1f, 1.8f, 0.1f),
                K("KayuGelap", 0f, 1.76f, 0f, 1.3f, 0.1f, 0.1f),
                T("KayuTua", 0f, 1.6f, 0f, 0.14f, 0.1f, 0.14f, 0f, 0f, 90f),   // kerekan
            }),
            // Plafon dapur 3 x 3 m dengan lubang di tengah sebagai jalan naik
            ("PlafonAnyaman", new[]
            {
                K("Gedek", -1.0f, 0f, 0f, 1.0f, 0.04f, 3f),
                K("Gedek", 1.0f, 0f, 0f, 1.0f, 0.04f, 3f),
                K("Gedek", 0f, 0f, -1.0f, 1.0f, 0.04f, 1.0f),
                K("Gedek", 0f, 0f, 1.0f, 1.0f, 0.04f, 1.0f),
            }),
            ("TanggaBambu", new[]
            {
                K("Bambu", -0.25f, 1.25f, 0f, 0.06f, 2.5f, 0.06f, 8f),
                K("Bambu", 0.25f, 1.25f, 0f, 0.06f, 2.5f, 0.06f, 8f),
                K("Bambu", 0f, 0.4f, -0.03f, 0.5f, 0.04f, 0.04f),
                K("Bambu", 0f, 0.9f, -0.07f, 0.5f, 0.04f, 0.04f),
                K("Bambu", 0f, 1.4f, -0.12f, 0.5f, 0.04f, 0.04f),
                K("Bambu", 0f, 1.9f, -0.16f, 0.5f, 0.04f, 0.04f),
            }),
            ("TaliTambang", new[]
            {
                T("Kain", 0f, 0.5f, 0f, 0.025f, 0.5f, 0.025f),
                T("Kain", 0.03f, 0.1f, 0f, 0.06f, 0.05f, 0.06f, 0f, 0f, 90f),   // gulungan
            }),
            ("PengaitBesi", new[]
            {
                T("Logam", 0f, 0.15f, 0f, 0.012f, 0.15f, 0.012f),
                K("Logam", 0.04f, 0.02f, 0f, 0.1f, 0.012f, 0.012f),
                K("Logam", 0.085f, 0.05f, 0f, 0.012f, 0.06f, 0.012f),
            }),
            ("BungkusanHitam", new[]
            {
                B("Gelap", 0f, 0.07f, 0f, 0.16f),
                K("Karat", 0f, 0.12f, 0f, 0.05f, 0.01f, 0.05f),                 // simpul benang merah
            }),
            ("TelurCemani", new[]
            {
                B("Gelap", 0f, 0.03f, 0f, 0.06f),
                T("Logam", 0.02f, 0.06f, 0f, 0.004f, 0.03f, 0.004f, 0f, 0f, 30f), // jarum
            }),
            ("Kentongan", new[]
            {
                T("KayuTua", 0f, 0.3f, 0f, 0.12f, 0.3f, 0.12f, 0f, 0f, 90f),
                K("Gelap", 0f, 0.3f, 0.1f, 0.4f, 0.03f, 0.02f),                  // celah bunyi
                K("KayuGelap", 0f, 0.6f, 0f, 0.02f, 0.1f, 0.02f),
            }),
            // Gardu 3 x 2,5 m: tiang, atap, bangku, termos, gelas kopi, papan catur
            ("PosRonda", new[]
            {
                K("KayuGelap", -1.4f, 1.25f, -1.1f, 0.12f, 2.5f, 0.12f),
                K("KayuGelap", 1.4f, 1.25f, -1.1f, 0.12f, 2.5f, 0.12f),
                K("KayuGelap", -1.4f, 1.25f, 1.1f, 0.12f, 2.5f, 0.12f),
                K("KayuGelap", 1.4f, 1.25f, 1.1f, 0.12f, 2.5f, 0.12f),
                K("Atap", 0f, 2.6f, 0f, 3.4f, 0.12f, 2.9f),
                K("KayuTua", 0f, 0.5f, 0f, 2.8f, 0.06f, 2.2f),                   // lantai gardu
                K("KayuTua", 0f, 0.8f, -0.9f, 2.6f, 0.05f, 0.4f),                // bangku
                K("KayuTua", 0f, 0.65f, 0.05f, 0.7f, 0.04f, 0.7f),               // meja catur
                K("Kertas", 0f, 0.675f, 0.05f, 0.5f, 0.005f, 0.5f),              // papan catur
                T("Logam", 0.9f, 0.62f, 0.3f, 0.07f, 0.12f, 0.07f),              // termos
                T("Kertas", 0.55f, 0.57f, 0.4f, 0.04f, 0.04f, 0.04f),            // gelas kopi
            }),
            ("PakJoko", Sosok("Kain", "KayuGelap")),
            ("Warga1", Sosok("Batik", "Gelap")),
            ("Warga2", Sosok("Gelap", "KayuTua")),
            // Malam 5: makam, pohon asem, rumah Pakde
            ("PohonAsem", new[]
            {
                T("KayuGelap", 0f, 2.5f, 0f, 0.7f, 2.5f, 0.7f),
                T("KayuGelap", 1.2f, 4.6f, 0f, 0.25f, 1.4f, 0.25f, 0f, 0f, -50f), // dahan atas (kain kafan)
                B("Daun", 0f, 5.6f, 0f, 4.2f),
                B("Daun", 1.8f, 5.2f, 0.6f, 2.6f),
            }),
            ("Pacul", new[]
            {
                T("KayuTua", 0f, 0.55f, 0f, 0.035f, 0.55f, 0.035f),
                K("Logam", 0f, 1.08f, 0.12f, 0.2f, 0.02f, 0.25f, 20f),
            }),
            ("Galah", new[] { T("Bambu", 0f, 2f, 0f, 0.04f, 2f, 0.04f) }),
            ("KendiTanahMerah", new[]
            {
                B("Terakota", 0f, 0.1f, 0f, 0.2f),
                T("Terakota", 0f, 0.22f, 0f, 0.06f, 0.04f, 0.06f),
                T("Karat", 0f, 0.262f, 0f, 0.05f, 0.002f, 0.05f),              // tanah merah di mulut
            }),
            ("KainKafan", new[]
            {
                K("Kertas", 0f, -0.45f, 0f, 0.4f, 0.9f, 0.01f),                // tergantung dari titik nol (dahan)
                K("Karat", 0f, -0.35f, 0.006f, 0.2f, 0.2f, 0.002f),            // rajah
            }),
            ("NisanKayu", new[] { K("KayuTua", 0f, 0.35f, 0f, 0.25f, 0.7f, 0.05f) }),
            ("NisanBatu", new[] { K("Tembok", 0f, 0.3f, 0f, 0.4f, 0.6f, 0.12f) }),
            ("NisanHadi", new[]
            {
                K("KayuTua", 0f, 0.4f, 0f, 0.3f, 0.8f, 0.05f),
                K("Kertas", 0f, 0.55f, 0.026f, 0.22f, 0.12f, 0.002f),          // papan nama
            }),
            ("GundukanMakam", new[] { K("Tanah", 0f, 0.15f, 0f, 0.9f, 0.3f, 2.0f) }),
            ("Kamboja", new[]
            {
                T("KayuTua", 0f, 1f, 0f, 0.18f, 1f, 0.18f),
                B("Daun", 0f, 2.4f, 0f, 1.8f),
                B("Kertas", 0.5f, 2.6f, 0.5f, 0.3f),                           // bunga putih
            }),
            ("GerbangMakam", new[]
            {
                K("BatuPutih", -1.2f, 1.1f, 0f, 0.4f, 2.2f, 0.4f),
                K("BatuPutih", 1.2f, 1.1f, 0f, 0.4f, 2.2f, 0.4f),
                K("BatuPutih", 0f, 2.35f, 0f, 2.8f, 0.3f, 0.4f),
            }),
            ("PakLasno", Sosok("Kain", "Batik")),   // peci = topi gelap dari Sosok
            ("Anjing", new[]
            {
                K("KayuGelap", 0f, 0.45f, 0f, 0.3f, 0.3f, 0.9f),
                K("KayuGelap", 0f, 0.6f, 0.5f, 0.2f, 0.2f, 0.25f),             // kepala ke depan (+Z)
                K("KayuGelap", -0.1f, 0.15f, 0.3f, 0.06f, 0.3f, 0.06f),
                K("KayuGelap", 0.1f, 0.15f, 0.3f, 0.06f, 0.3f, 0.06f),
                K("KayuGelap", -0.1f, 0.15f, -0.3f, 0.06f, 0.3f, 0.06f),
                K("KayuGelap", 0.1f, 0.15f, -0.3f, 0.06f, 0.3f, 0.06f),
            }),
            ("MangkukAnjing", new[] { T("Logam", 0f, 0.04f, 0f, 0.25f, 0.04f, 0.25f) }),
            ("PotBunga", new[]
            {
                T("Terakota", 0f, 0.3f, 0f, 0.5f, 0.3f, 0.5f),
                B("Daun", 0f, 0.8f, 0f, 0.7f),
            }),
            ("MejaKerjaLaci", new[]
            {
                K("KayuTua", 0f, 0.73f, 0f, 1.2f, 0.04f, 0.6f),
                K("KayuGelap", 0.35f, 0.36f, 0f, 0.45f, 0.7f, 0.55f),          // blok laci
                K("Logam", 0.35f, 0.55f, 0.28f, 0.1f, 0.02f, 0.01f),           // pegangan laci
                K("KayuGelap", -0.55f, 0.36f, 0f, 0.05f, 0.7f, 0.55f),
            }),
            // Rumah 8 x 6 m, depan (+Z) dengan teras; pintu tidak dibuka (Lapis B masuk lewat skrip pembuat).
            ("RumahPakdeLuar", new[]
            {
                K("BatuPutih", 0f, 1.6f, 0f, 8f, 3.2f, 6f),
                K("Genteng", 0f, 3.4f, 0f, 8.6f, 0.3f, 6.6f),
                K("Tembok", 0f, 0.1f, 4f, 8f, 0.2f, 2f),                       // lantai teras
                K("BatuPutih", -3.8f, 1.6f, 4.8f, 0.25f, 3.2f, 0.25f),
                K("BatuPutih", 3.8f, 1.6f, 4.8f, 0.25f, 3.2f, 0.25f),
                K("KayuGelap", 0f, 1.05f, 3.01f, 1f, 2.1f, 0.05f),             // pintu
            }),
            ("PagarBesi", new[]
            {
                K("Logam", 0f, 0.05f, 0f, 2f, 0.05f, 0.05f),
                K("Logam", 0f, 1.2f, 0f, 2f, 0.05f, 0.05f),
                K("Logam", -0.75f, 0.6f, 0f, 0.03f, 1.2f, 0.03f),
                K("Logam", -0.25f, 0.6f, 0f, 0.03f, 1.2f, 0.03f),
                K("Logam", 0.25f, 0.6f, 0f, 0.03f, 1.2f, 0.03f),
                K("Logam", 0.75f, 0.6f, 0f, 0.03f, 1.2f, 0.03f),
            }),
            ("GelasTeh", new[] { T("Kertas", 0f, 0.05f, 0f, 0.07f, 0.05f, 0.07f) }),
            ("SisaMakanan", new[]
            {
                K("Daun", 0f, 0.01f, 0f, 0.25f, 0.02f, 0.25f),                 // bungkus daun pisang
                B("Kertas", 0f, 0.05f, 0f, 0.15f),                             // nasi
            }),
            ("TumpukanTanah", new[] { B("Tanah", 0f, 0f, 0f, 0.8f) }),
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
