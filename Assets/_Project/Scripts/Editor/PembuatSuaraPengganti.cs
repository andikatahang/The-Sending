using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Lima WAV sintetis (mono, 16 bit, 22050 Hz) sebagai pengganti sampai tim mendapat rekaman asli.
    // Tidak menimpa file dengan nama dasar yang sama. Azan tidak pernah dibuat sintetis.
    public static class PembuatSuaraPengganti
    {
        const int Hz = 22050;

        public static void Buat()
        {
            Tulis(PembuatPengganti.FolderSfx, "DentangJam", Dentang());
            Tulis(PembuatPengganti.FolderSfx, "PakuJatuh", LogamJatuh());
            Tulis(PembuatPengganti.FolderSfx, "ListrikPadam", ListrikPadam());
            Tulis(PembuatPengganti.FolderSfx, "LangkahDiAtap", Langkah());
            Tulis(PembuatPengganti.FolderMusik, "DroneHoror", Drone());
        }

        static void Tulis(string folder, string nama, float[] sampel)
        {
            if (PembuatPengganti.AdaSuara(folder, nama)) return;

            string jalur = $"{folder}/{nama}.wav";
            int byteData = sampel.Length * 2;
            using (var w = new BinaryWriter(File.Create(jalur)))
            {
                w.Write(Encoding.ASCII.GetBytes("RIFF"));
                w.Write(36 + byteData);
                w.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
                w.Write(16);            // ukuran blok fmt
                w.Write((short)1);      // PCM
                w.Write((short)1);      // mono
                w.Write(Hz);
                w.Write(Hz * 2);        // byte per detik
                w.Write((short)2);      // byte per sampel
                w.Write((short)16);     // bit per sampel
                w.Write(Encoding.ASCII.GetBytes("data"));
                w.Write(byteData);
                foreach (float s in sampel) w.Write((short)(Mathf.Clamp(s, -1f, 1f) * 32767f));
            }
            AssetDatabase.ImportAsset(jalur);
            var klip = AssetDatabase.LoadAssetAtPath<AudioClip>(jalur);
            if (klip == null || klip.samples != sampel.Length)
                throw new InvalidOperationException($"WAV sintetis gagal diimpor atau panjangnya salah: {jalur}");
        }

        static float[] Kosong(float detik) => new float[(int)(detik * Hz)];

        // Menambahkan nada sinus yang meluruh, mulai detik ke-`mulai`. luruh 0 = keras tetap.
        static void Nada(float[] s, float mulai, float frek, float lama, float keras, float luruh)
        {
            int awal = (int)(mulai * Hz), n = (int)(lama * Hz);
            for (int i = 0; i < n && awal + i < s.Length; i++)
            {
                float t = i / (float)Hz;
                s[awal + i] += keras * Mathf.Exp(-luruh * t) * Mathf.Sin(2f * Mathf.PI * frek * t);
            }
        }

        // Menambahkan derau yang meluruh. Benih tetap supaya file yang dibuat ulang sama persis.
        static void Derau(float[] s, float mulai, float lama, float keras, float luruh, int benih)
        {
            var acak = new System.Random(benih);
            int awal = (int)(mulai * Hz), n = (int)(lama * Hz);
            for (int i = 0; i < n && awal + i < s.Length; i++)
                s[awal + i] += keras * Mathf.Exp(-luruh * i / Hz) * (float)(acak.NextDouble() * 2.0 - 1.0);
        }

        // Tiga dentang, jeda 1,2 detik.
        static float[] Dentang()
        {
            var s = Kosong(4.5f);
            for (int k = 0; k < 3; k++)
            {
                Nada(s, k * 1.2f, 660f, 2f, 0.45f, 2.5f);
                Nada(s, k * 1.2f, 1320f, 2f, 0.2f, 4f);
            }
            return s;
        }

        // Tiga denting logam pendek.
        static float[] LogamJatuh()
        {
            var s = Kosong(1.5f);
            float[] mulai = { 0f, 0.25f, 0.6f }, frek = { 2600f, 3100f, 2850f };
            for (int k = 0; k < 3; k++)
            {
                Nada(s, mulai[k], frek[k], 0.4f, 0.4f, 14f);
                Derau(s, mulai[k], 0.03f, 0.3f, 80f, 10 + k);
            }
            return s;
        }

        // Dengung 50 Hz, klik, lalu nada rendah yang meluruh.
        static float[] ListrikPadam()
        {
            var s = Kosong(1.6f);
            Nada(s, 0f, 50f, 0.8f, 0.3f, 0f);
            Nada(s, 0f, 100f, 0.8f, 0.15f, 0f);
            Derau(s, 0.8f, 0.05f, 0.8f, 60f, 3);
            Nada(s, 0.8f, 40f, 0.8f, 0.4f, 5f);
            return s;
        }

        // Lima hentakan berat, jeda 0,9 detik.
        static float[] Langkah()
        {
            var s = Kosong(5f);
            for (int k = 0; k < 5; k++)
            {
                Nada(s, 0.3f + k * 0.9f, 55f, 0.5f, 0.8f, 9f);
                Derau(s, 0.3f + k * 0.9f, 0.12f, 0.25f, 30f, 20 + k);
            }
            return s;
        }

        // Loop 20 detik. Semua frekuensi kelipatan 0,05 Hz, jadi setiap nada genap putarannya dan sambungan loop mulus.
        static float[] Drone()
        {
            var s = Kosong(20f);
            Nada(s, 0f, 55f, 20f, 0.3f, 0f);
            Nada(s, 0f, 58.05f, 20f, 0.25f, 0f);
            Nada(s, 0f, 82.4f, 20f, 0.15f, 0f);
            Nada(s, 0f, 110.1f, 20f, 0.1f, 0f);
            return s;
        }
    }
}
