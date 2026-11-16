using System;
using UnityEngine;

namespace TheSending.Core
{
    // Pengaturan pemain (bukan bagian save cerita). Runtime menyimpannya sebagai JSON di PlayerPrefs.
    [Serializable]
    public class Pengaturan
    {
        // Urutan = grup Audio Mixer di style guide. Parameter mixer yang diekspos bernama "Vol" + nama grup.
        public static readonly string[] NamaGrup = { "Master", "Music", "SFX", "Ambience", "Dialog" };
        public const float SensitivitasMin = 0.02f, SensitivitasMaks = 0.3f, SensitivitasAwal = 0.08f;
        public const float DesibelSenyap = -80f;

        public float[] Volume = { 1f, 1f, 1f, 1f, 1f };   // 0..1 per grup
        public float Sensitivitas = SensitivitasAwal;
        public bool BalikY;
        public bool LayarPenuh = true;

        // Memaksa semua nilai ke rentang yang sah. Dipanggil setelah memuat dan setelah setiap perubahan.
        public Pengaturan Rapikan()
        {
            var v = new float[NamaGrup.Length];
            for (int i = 0; i < v.Length; i++)
                v[i] = Volume != null && i < Volume.Length ? Batasi(Volume[i], 0f, 1f, 1f) : 1f;
            Volume = v;
            Sensitivitas = Batasi(Sensitivitas, SensitivitasMin, SensitivitasMaks, SensitivitasAwal);
            return this;
        }

        public static float KeDesibel(float linear) =>
            linear <= 0.0001f ? DesibelSenyap : 20f * (float)Math.Log10(linear);

        public string KeJson() => JsonUtility.ToJson(this);

        // PlayerPrefs adalah data luar: kosong atau rusak berarti pengaturan bawaan, bukan error.
        public static Pengaturan DariJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return new Pengaturan();
            try
            {
                return (JsonUtility.FromJson<Pengaturan>(json) ?? new Pengaturan()).Rapikan();
            }
            catch (ArgumentException)
            {
                return new Pengaturan();
            }
        }

        static float Batasi(float nilai, float min, float maks, float bawaan) =>
            float.IsNaN(nilai) ? bawaan : Math.Min(maks, Math.Max(min, nilai));
    }
}
