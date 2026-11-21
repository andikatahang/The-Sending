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
        [SerializeField] PemainGerak pemain;   // bar napas dan prompt saat bersembunyi; boleh kosong (scene tanpa pemain)

        Text titik, prompt, jam, baterai, notifikasi, napas, hitungMundur, tujuan;
        readonly Queue<string> antrean = new Queue<string>();
        bool obatSelesaiTerakhir;   // Perubahan.Jam datang tiap frame; baris tujuan hanya dihitung ulang saat status obat berubah
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
            napas = UiPabrik.BuatTeks(akar, "Napas", 26, TextAnchor.LowerCenter, new Vector2(0.35f, 0.16f), new Vector2(0.65f, 0.22f));
            hitungMundur = UiPabrik.BuatTeks(akar, "HitungMundur", 24, TextAnchor.UpperRight, new Vector2(0.6f, 0.75f), new Vector2(0.98f, 0.85f));
            tujuan = UiPabrik.BuatTeks(akar, "Tujuan", 22, TextAnchor.UpperRight, new Vector2(0.5f, 0.7f), new Vector2(0.98f, 0.75f));
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
            PerbaruiTujuan();
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
            PerbaruiTujuan();
        }

        void PerbaruiTujuan()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            obatSelesaiTerakhir = JamGim.ObatSelesai(gm.State);
            var baris = TujuanMalam.BarisHud(gm.State);
            tujuan.text = baris.Length == 0 ? "" : "Tujuan: " + baris;
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
            if (p == Perubahan.Jam) { if (JamGim.ObatSelesai(GameManager.Instance.State) != obatSelesaiTerakhir) PerbaruiTujuan(); }
            else PerbaruiTujuan();
            switch (p)
            {
                case Perubahan.Bukti: Tampilkan("Bukti baru masuk Papan Bukti (Jurnal [Tab])."); break;
                case Perubahan.Katanya: Tampilkan("Papan Bukti diperbarui (Jurnal [Tab])."); break;
                case Perubahan.Diary: Tampilkan("Halaman buku harian ditemukan. Baca di Jurnal [Tab]."); break;
                case Perubahan.Media: Tampilkan("Catatan media santet diperbarui."); break;
                case Perubahan.Barang: Tampilkan("Barang baru masuk inventaris [I]."); break;
                case Perubahan.Foto: Tampilkan("Foto masuk Papan Bukti (Jurnal [Tab])."); break;
            }
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var s = gm.State;

            // Saat adegan berskrip hanya notifikasi yang tampil (teks pengganti suara harus tetap terbaca).
            bool tampil = !gm.Sinematik;
            titik.enabled = prompt.enabled = jam.enabled = baterai.enabled = napas.enabled = hitungMundur.enabled = tujuan.enabled = tampil;

            prompt.text = pemain != null && pemain.Bersembunyi ? "[E] Keluar     [Spasi] Tahan napas"
                : interaksi != null && interaksi.Sasaran != null ? $"[E] {interaksi.Sasaran.Prompt}" : "";
            napas.text = TeksNapas();
            hitungMundur.text = TeksHitungMundur(s);
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

        // Bar napas tampil selama menahan napas dan selama napas belum pulih penuh.
        string TeksNapas()
        {
            if (pemain == null) return "";
            var n = pemain.Napas;
            if (!n.SedangMenahan && n.Sisa >= Napas.BatasDetik) return "";
            return "Napas " + new string('|', Mathf.CeilToInt(n.Sisa / Napas.BatasDetik * 10f));
        }

        // Game Story Malam 2: hitung mundur menuju weton Ibu, Selasa Kliwon (Malam 7).
        static string TeksHitungMundur(GameState s)
        {
            if (!KalenderJawa.HitungMundurBerjalan(s)) return "";
            int sisa = KalenderJawa.SisaMalamKeSelasaKliwon(s.MalamKe);
            string teks = sisa == 0 ? "Malam ini Selasa Kliwon" : $"Selasa Kliwon: {sisa} malam lagi";
            if (s.MalamKe != 6 || s.HasilSidang != HasilSidang.Belum) return teks;
            // Menit nyata sampai warga berkumpul pukul 21.00 (Malam 6 punya laju jam sendiri).
            int menit = Mathf.CeilToInt((JamGim.Batas(6) - s.MenitGim) * JamGim.DetikPerJamGim(6) / 60f / 60f);
            return teks + "\n" + (menit <= 0 ? "Warga berkumpul sekarang!" : $"Warga berkumpul pukul 21.00: {menit} menit lagi");
        }
    }
}
