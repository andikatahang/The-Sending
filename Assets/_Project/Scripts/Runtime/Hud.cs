using System.Collections.Generic;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Titik bidik, prompt interaksi, jam HP, baterai, dan notifikasi singkat.
    public class Hud : MonoBehaviour
    {
        const float LamaNotifikasi = 3f;

        [SerializeField] PemainInteraksi interaksi;

        Text titik, prompt, jam, baterai, notifikasi;
        readonly Queue<string> antrean = new Queue<string>();
        float sisaNotifikasi;
        string pesanTerakhir;   // pesan terakhir yang masuk antrean; null setelah semua selesai tampil

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("HUD", 10).transform;
            akar.SetParent(transform, false);
            titik = UiPabrik.BuatTeks(akar, "TitikBidik", 28, TextAnchor.MiddleCenter, new Vector2(0.49f, 0.48f), new Vector2(0.51f, 0.52f));
            titik.text = "·";
            prompt = UiPabrik.BuatTeks(akar, "Prompt", 30, TextAnchor.UpperCenter, new Vector2(0.3f, 0.36f), new Vector2(0.7f, 0.46f));
            jam = UiPabrik.BuatTeks(akar, "Jam", 34, TextAnchor.UpperRight, new Vector2(0.75f, 0.9f), new Vector2(0.98f, 0.98f));
            baterai = UiPabrik.BuatTeks(akar, "Baterai", 24, TextAnchor.UpperRight, new Vector2(0.75f, 0.85f), new Vector2(0.98f, 0.9f));
            notifikasi = UiPabrik.BuatTeks(akar, "Notifikasi", 28, TextAnchor.LowerLeft, new Vector2(0.02f, 0.04f), new Vector2(0.6f, 0.14f));
        }

        void OnEnable()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogWarning("Hud tidak menemukan GameManager; notifikasi perubahan tidak akan muncul.", this);
                return;
            }
            gm.Berubah += SaatBerubah;
            gm.StateDiganti += SaatStateDiganti;
        }

        void OnDisable()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            gm.Berubah -= SaatBerubah;
            gm.StateDiganti -= SaatStateDiganti;
        }

        // Notifikasi milik state lama tidak berlaku lagi setelah save dimuat.
        void SaatStateDiganti()
        {
            antrean.Clear();
            notifikasi.text = "";
            sisaNotifikasi = 0f;
            pesanTerakhir = null;
        }

        // Pesan kembar berurutan (memotret ulang, perubahan ganda dari satu aksi) cukup tampil sekali.
        public void Tampilkan(string pesan)
        {
            if (string.IsNullOrEmpty(pesan) || pesan == pesanTerakhir) return;
            pesanTerakhir = pesan;
            antrean.Enqueue(pesan);
        }

        void SaatBerubah(Perubahan p)
        {
            switch (p)
            {
                case Perubahan.Bukti: Tampilkan("Bukti baru masuk Papan Bukti."); break;
                case Perubahan.Katanya: Tampilkan("Papan Bukti diperbarui."); break;
                case Perubahan.Diary: Tampilkan("Halaman buku harian ditemukan."); break;
                case Perubahan.Media: Tampilkan("Catatan media santet diperbarui."); break;
                case Perubahan.Barang: Tampilkan("Barang baru masuk inventaris (I)."); break;
                case Perubahan.Foto: Tampilkan("Foto masuk Papan Bukti."); break;
            }
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var s = gm.State;

            // Saat adegan berskrip hanya notifikasi yang tampil (teks pengganti suara harus tetap terbaca).
            bool tampil = !gm.Sinematik;
            titik.enabled = prompt.enabled = jam.enabled = baterai.enabled = tampil;

            prompt.text = interaksi != null && interaksi.Sasaran != null ? $"[E] {interaksi.Sasaran.Prompt}" : "";
            jam.text = JamGim.Format(s.MenitGim);
            baterai.text = $"Baterai {Mathf.CeilToInt(s.BateraiPersen)}%";

            if (sisaNotifikasi > 0f)
            {
                sisaNotifikasi -= Time.unscaledDeltaTime;
                if (sisaNotifikasi > 0f) return;
                notifikasi.text = "";
                if (antrean.Count == 0) pesanTerakhir = null;   // pesan yang sama boleh tampil lagi nanti
            }
            else if (antrean.Count > 0)
            {
                notifikasi.text = antrean.Dequeue();
                sisaNotifikasi = LamaNotifikasi;
            }
        }
    }
}
