using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // WAV sintetis (mono, 16 bit, 22050 Hz) sebagai pengganti sampai tim mendapat rekaman asli.
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
            Tulis(PembuatPengganti.FolderSfx, "PapanKopong", Kopong());
            Tulis(PembuatPengganti.FolderSfx, "KetukPintu", Ketukan());
            Tulis(PembuatPengganti.FolderSfx, "GemerincingPaku", Gemerincing());
            Tulis(PembuatPengganti.FolderSfx, "DetakJantung", Detak());
            Tulis(PembuatPengganti.FolderSfx, "StingerKejutan", Stinger());
            Tulis(PembuatPengganti.FolderSfx, "GerimisDiAtap", Gerimis());
            Tulis(PembuatPengganti.FolderSfx, "KetukPadat", KetukPadat());
            Tulis(PembuatPengganti.FolderSfx, "KetukBerongga", KetukBerongga());
            Tulis(PembuatPengganti.FolderSfx, "PakuDicabut", CabutPaku());
            Tulis(PembuatPengganti.FolderSfx, "ApiAnglo", ApiArang());
            Tulis(PembuatPengganti.FolderSfx, "PintuDibanting", Banting());
            Tulis(PembuatPengganti.FolderSfx, "KerekanDerit", KerekanDerit());
            Tulis(PembuatPengganti.FolderSfx, "TaliDitarik", TaliDitarik());
            Tulis(PembuatPengganti.FolderSfx, "BendaMasukAir", BendaMasukAir());
            Tulis(PembuatPengganti.FolderSfx, "Kentongan", Kentongan());
            Tulis(PembuatPengganti.FolderSfx, "AyamBerkokok", AyamBerkokok());
            Tulis(PembuatPengganti.FolderSfx, "GaliTanah", GaliTanah());
            Tulis(PembuatPengganti.FolderSfx, "KainJatuh", KainJatuh());
            Tulis(PembuatPengganti.FolderSfx, "AnjingMenggonggong", AnjingMenggonggong());
            Tulis(PembuatPengganti.FolderSfx, "LaciDibuka", LaciDibuka());
            Tulis(PembuatPengganti.FolderSfx, "KerumunanMarah", KerumunanMarah());
            Tulis(PembuatPengganti.FolderSfx, "OborBerkobar", Kobaran(4f, 0.3f, 21));
            Tulis(PembuatPengganti.FolderSfx, "RumahTerbakar", RumahTerbakar());
            Tulis(PembuatPengganti.FolderSfx, "GeramIbu", GeramIbu());
        }

        // Fase dijumlah supaya nada yang berubah tetap mulus.
        static void Sapu(float[] s, float mulai, float lama, float frekAwal, float frekAkhir, float keras)
        {
            int awal = (int)(mulai * Hz), n = (int)(lama * Hz);
            double fase = 0;
            for (int i = 0; i < n && awal + i < s.Length; i++)
            {
                float t = i / (float)Hz, u = t / lama;
                fase += 2.0 * Math.PI * Mathf.Lerp(frekAwal, frekAkhir, u) / Hz;
                s[awal + i] += keras * (float)Math.Sin(fase) * Mathf.Sin(Mathf.PI * u);
            }
        }

        // Decit kayu kering naik-turun dua kali.
        static float[] KerekanDerit()
        {
            var s = Kosong(1.6f);
            Sapu(s, 0f, 0.7f, 600f, 900f, 0.2f);
            Sapu(s, 0.8f, 0.7f, 650f, 850f, 0.2f);
            Derau(s, 0f, 1.5f, 0.03f, 0.5f, 100);
            return s;
        }

        // Gesekan tali: derau dihaluskan, mengeras lalu hilang.
        static float[] TaliDitarik()
        {
            var s = Kosong(1.2f);
            var acak = new System.Random(101);
            float y = 0f;
            for (int i = 0; i < s.Length; i++)
            {
                y += 0.3f * ((float)(acak.NextDouble() * 2.0 - 1.0) - y);
                s[i] = y * 0.6f * Mathf.Sin(Mathf.PI * i / s.Length);
            }
            return s;
        }

        // Cebur: derau singkat dan nada rendah menurun, lalu gema air.
        static float[] BendaMasukAir()
        {
            var s = Kosong(1.4f);
            Derau(s, 0f, 0.12f, 0.6f, 25f, 102);
            Sapu(s, 0f, 0.3f, 300f, 90f, 0.5f);
            Nada(s, 0.5f, 180f, 0.6f, 0.15f, 7f);
            return s;
        }

        // Dua pukulan kentongan kayu: nada rendah bergaung.
        static float[] Kentongan()
        {
            var s = Kosong(1.6f);
            for (int k = 0; k < 2; k++)
            {
                Nada(s, k * 0.5f, 200f, 0.9f, 0.6f, 6f);
                Nada(s, k * 0.5f, 400f, 0.5f, 0.25f, 10f);
                Derau(s, k * 0.5f, 0.03f, 0.3f, 80f, 103 + k);
            }
            return s;
        }

        // Kokok: tiga nada naik-turun, lalu satu panjang.
        static float[] AyamBerkokok()
        {
            var s = Kosong(2.2f);
            Sapu(s, 0f, 0.25f, 500f, 900f, 0.3f);
            Sapu(s, 0.3f, 0.25f, 700f, 1100f, 0.3f);
            Sapu(s, 0.6f, 0.25f, 800f, 1200f, 0.3f);
            Sapu(s, 0.95f, 1f, 1100f, 600f, 0.3f);
            return s;
        }

        // Dua ayunan pacul: hentak rendah dan derau tanah.
        static float[] GaliTanah()
        {
            var s = Kosong(1.4f);
            for (int k = 0; k < 2; k++)
            {
                Nada(s, k * 0.7f, 90f, 0.25f, 0.5f, 15f);
                Derau(s, k * 0.7f, 0.35f, 0.4f, 10f, 104 + k);
            }
            return s;
        }

        // Kain bergesek di dahan lalu jatuh ke tanah.
        static float[] KainJatuh()
        {
            var s = Kosong(1.3f);
            Derau(s, 0f, 0.6f, 0.15f, 2f, 106);
            Derau(s, 0.9f, 0.2f, 0.3f, 20f, 107);
            return s;
        }

        // Tiga gonggongan pendek.
        static float[] AnjingMenggonggong()
        {
            var s = Kosong(1.5f);
            for (int k = 0; k < 3; k++)
            {
                Sapu(s, k * 0.4f, 0.15f, 450f, 250f, 0.5f);
                Derau(s, k * 0.4f, 0.12f, 0.2f, 20f, 108 + k);
            }
            return s;
        }

        // Kunci diputar (klik), lalu laci kayu ditarik.
        static float[] LaciDibuka()
        {
            var s = Kosong(1.2f);
            Nada(s, 0f, 2400f, 0.05f, 0.4f, 60f);
            Nada(s, 0.15f, 1800f, 0.05f, 0.4f, 60f);
            Sapu(s, 0.4f, 0.6f, 180f, 140f, 0.15f);
            Derau(s, 0.4f, 0.6f, 0.12f, 1f, 111);
            return s;
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

        // Dua ketukan kayu kopong: nada rendah dengan harmonik, meluruh cepat.
        static float[] Kopong()
        {
            var s = Kosong(1.2f);
            for (int k = 0; k < 2; k++)
            {
                Nada(s, k * 0.45f, 170f, 0.4f, 0.6f, 12f);
                Nada(s, k * 0.45f, 340f, 0.3f, 0.25f, 18f);
                Derau(s, k * 0.45f, 0.04f, 0.3f, 60f, 40 + k);
            }
            return s;
        }

        // Tiga ketukan pintu kayu.
        static float[] Ketukan()
        {
            var s = Kosong(1.6f);
            for (int k = 0; k < 3; k++)
            {
                Nada(s, 0.1f + k * 0.35f, 110f, 0.25f, 0.7f, 20f);
                Derau(s, 0.1f + k * 0.35f, 0.05f, 0.4f, 50f, 50 + k);
            }
            return s;
        }

        // Loop 4 detik: denting paku acak. Semua denting habis sebelum akhir klip, jadi sambungan loop mulus.
        static float[] Gemerincing()
        {
            var s = Kosong(4f);
            var acak = new System.Random(7);
            for (int k = 0; k < 14; k++)
            {
                float mulai = (float)acak.NextDouble() * 3.5f;
                Nada(s, mulai, 2400f + (float)acak.NextDouble() * 1600f, 0.3f, 0.18f, 18f);
            }
            return s;
        }

        // Loop 2 detik: detak "lub-dub" dua kali (60 per menit).
        static float[] Detak()
        {
            var s = Kosong(2f);
            for (int k = 0; k < 2; k++)
            {
                Nada(s, k, 50f, 0.25f, 0.8f, 14f);
                Nada(s, k + 0.28f, 45f, 0.25f, 0.6f, 16f);
            }
            return s;
        }

        // Kejutan: gugus nada tinggi sumbang, dentum rendah, dan derau; meluruh dalam sekitar 1,5 detik.
        static float[] Stinger()
        {
            var s = Kosong(2f);
            foreach (float f in new[] { 880f, 932f, 1245f, 1318f }) Nada(s, 0f, f, 1.8f, 0.18f, 2.5f);
            Nada(s, 0f, 55f, 1.5f, 0.5f, 3f);
            Derau(s, 0f, 0.4f, 0.35f, 8f, 60);
            return s;
        }

        // Loop 10 detik: gerimis = derau pelan yang dihaluskan (filter lolos-bawah satu kutub).
        static float[] Gerimis()
        {
            var s = Kosong(10f);
            var acak = new System.Random(11);
            float y = 0f;
            for (int i = 0; i < s.Length; i++)
            {
                y += 0.15f * ((float)(acak.NextDouble() * 2.0 - 1.0) - y);
                s[i] = y * 0.5f;
            }
            return s;
        }

        // Dua ketukan pada kayu padat: pendek, tinggi, cepat luruh.
        static float[] KetukPadat()
        {
            var s = Kosong(0.8f);
            for (int k = 0; k < 2; k++)
            {
                Nada(s, k * 0.3f, 320f, 0.15f, 0.6f, 40f);
                Derau(s, k * 0.3f, 0.02f, 0.3f, 120f, 70 + k);
            }
            return s;
        }

        // Dua ketukan pada kayu berongga: lebih rendah dan bergaung.
        static float[] KetukBerongga()
        {
            var s = Kosong(1.4f);
            for (int k = 0; k < 2; k++)
            {
                Nada(s, k * 0.4f, 140f, 0.8f, 0.6f, 5f);
                Nada(s, k * 0.4f, 280f, 0.6f, 0.25f, 8f);
                Derau(s, k * 0.4f, 0.03f, 0.3f, 60f, 72 + k);
            }
            return s;
        }

        // Paku dicabut: decit logam yang naik (fase dijumlah supaya nadanya mulus), lalu denting.
        static float[] CabutPaku()
        {
            var s = Kosong(1f);
            int n = (int)(0.5f * Hz);
            double fase = 0;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)Hz;
                fase += 2.0 * Math.PI * (900.0 + 1400.0 * t / 0.5) / Hz;
                s[i] += 0.15f * (float)Math.Sin(fase) * (1f - t / 0.5f);
            }
            Nada(s, 0.55f, 2900f, 0.4f, 0.35f, 12f);
            return s;
        }

        // Loop 6 detik: api arang = derau pelan yang dihaluskan dan letupan acak. Letupan habis sebelum akhir klip.
        static float[] ApiArang()
        {
            var s = Kosong(6f);
            var acak = new System.Random(13);
            float y = 0f;
            for (int i = 0; i < s.Length; i++)
            {
                y += 0.05f * ((float)(acak.NextDouble() * 2.0 - 1.0) - y);
                s[i] = y * 0.4f;
            }
            for (int k = 0; k < 18; k++) Derau(s, (float)acak.NextDouble() * 5.5f, 0.02f, 0.5f, 150f, 80 + k);
            return s;
        }

        // Pintu kayu dibanting: dentum rendah, derau, dan getar papan.
        static float[] Banting()
        {
            var s = Kosong(1.5f);
            Nada(s, 0f, 70f, 1f, 0.9f, 6f);
            Derau(s, 0f, 0.15f, 0.7f, 25f, 90);
            Nada(s, 0.02f, 190f, 0.4f, 0.3f, 15f);
            return s;
        }
    
        // Gumam banyak orang: sapuan suara rendah acak di atas derau, sesekali teriakan.
        static float[] KerumunanMarah()
        {
            var s = Kosong(6f);
            var acak = new System.Random(31);
            Derau(s, 0f, 6f, 0.08f, 0f, 32);
            for (int k = 0; k < 30; k++)
            {
                float f = 110f + (float)acak.NextDouble() * 120f;
                Sapu(s, (float)acak.NextDouble() * 5.4f, 0.4f + (float)acak.NextDouble() * 0.5f, f, f * 0.8f, 0.12f);
            }
            for (int k = 0; k < 4; k++) Sapu(s, 0.8f + k * 1.3f, 0.5f, 320f, 240f, 0.18f);
            return s;
        }

        // Derau api bergemuruh dengan letupan; keras dan jumlah letupan mengatur besarnya.
        static float[] Kobaran(float detik, float keras, int benih)
        {
            var s = Kosong(detik);
            var acak = new System.Random(benih);
            float y = 0f;
            for (int i = 0; i < s.Length; i++)
            {
                y += 0.08f * ((float)(acak.NextDouble() * 2.0 - 1.0) - y);
                s[i] = y * keras;
            }
            for (int k = 0; k < detik * 4; k++) Derau(s, (float)acak.NextDouble() * (detik - 0.1f), 0.02f, 0.5f, 150f, benih + k + 1);
            return s;
        }

        // Kobaran besar, dentum rendah, dan kayu runtuh.
        static float[] RumahTerbakar()
        {
            var s = Kobaran(8f, 0.6f, 41);
            Nada(s, 0f, 45f, 8f, 0.2f, 0.1f);
            Derau(s, 3f, 0.6f, 0.6f, 6f, 42);
            Nada(s, 3f, 80f, 1f, 0.5f, 4f);
            return s;
        }

        // Geram rendah bergetar dari tenggorokan.
        static float[] GeramIbu()
        {
            var s = Kosong(3f);
            for (int i = 0; i < s.Length; i++)
            {
                float t = i / (float)Hz, u = t / 3f;
                float getar = 0.6f + 0.4f * Mathf.Sin(2f * Mathf.PI * 9f * t);
                s[i] = 0.4f * getar * Mathf.Sin(Mathf.PI * u) * (Mathf.Sin(2f * Mathf.PI * 70f * t) + 0.5f * Mathf.Sin(2f * Mathf.PI * 143f * t));
            }
            Derau(s, 0f, 3f, 0.08f, 0.3f, 51);
            return s;
        }
    }
}
