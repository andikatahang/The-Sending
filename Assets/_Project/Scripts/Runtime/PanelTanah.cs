using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Perbandingan tanah kendi dengan sampel makam (Malam 5): tabel Warna/Tekstur/Bau. E atau Esc menutup.
    public class PanelTanah : MonoBehaviour
    {
        static readonly string[] NamaBaris = { "Warna", "Tekstur", "Bau" };

        GameObject panel;
        Text judulSampel, kolomKendi, kolomSampel;
        Action saatSelesai;
        int frameBuka;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Tanah", 16).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.05f, 0.04f, 0.03f, 0.96f), new Vector2(0.15f, 0.2f), new Vector2(0.85f, 0.8f));
            panel = latar.gameObject;
            var t = latar.transform;
            UiPabrik.BuatTeks(t, "Judul", 30, TextAnchor.UpperLeft, new Vector2(0.04f, 0.86f), new Vector2(0.96f, 0.97f)).text = "PERBANDINGAN TANAH";
            UiPabrik.BuatTeks(t, "Baris", 26, TextAnchor.UpperLeft, new Vector2(0.04f, 0.3f), new Vector2(0.24f, 0.76f)).text =
                "\n" + string.Join("\n", NamaBaris);
            UiPabrik.BuatTeks(t, "JudulKendi", 26, TextAnchor.UpperLeft, new Vector2(0.26f, 0.68f), new Vector2(0.6f, 0.76f)).text = "Tanah kendi";
            judulSampel = UiPabrik.BuatTeks(t, "JudulSampel", 26, TextAnchor.UpperLeft, new Vector2(0.62f, 0.68f), new Vector2(0.96f, 0.76f));
            kolomKendi = UiPabrik.BuatTeks(t, "Kendi", 26, TextAnchor.UpperLeft, new Vector2(0.26f, 0.3f), new Vector2(0.6f, 0.68f));
            kolomSampel = UiPabrik.BuatTeks(t, "Sampel", 26, TextAnchor.UpperLeft, new Vector2(0.62f, 0.3f), new Vector2(0.96f, 0.68f));
            UiPabrik.BuatTeks(t, "Hasil", 28, TextAnchor.UpperLeft, new Vector2(0.04f, 0.14f), new Vector2(0.96f, 0.26f)).text = "Tidak cocok.";
            UiPabrik.BuatTeks(t, "Petunjuk", 22, TextAnchor.UpperLeft, new Vector2(0.04f, 0.03f), new Vector2(0.96f, 0.12f)).text = "[E] / [Esc] tutup";
            panel.SetActive(false);
        }

        public void Buka(int sampel, string[] kendi, string[] sampelTeks, Action selesai)
        {
            var gm = GameManager.Instance;
            if (Terbuka || gm == null) return;
            saatSelesai = selesai;
            frameBuka = Time.frameCount;   // tombol E yang membuka panel tidak ikut terbaca
            judulSampel.text = $"Sampel {sampel + 1}";
            kolomKendi.text = "\n" + string.Join("\n", kendi);
            kolomSampel.text = "\n" + string.Join("\n", sampelTeks);
            panel.SetActive(true);
            gm.Jeda(true);
        }

        void Update()
        {
            if (!Terbuka || Time.frameCount == frameBuka) return;
            var kb = Keyboard.current;
            if (kb != null && (kb.eKey.wasPressedThisFrame || kb.escapeKey.wasPressedThisFrame)) Tutup();
        }

        // Panel dimatikan atau dihancurkan saat terbuka (misalnya ganti scene): lepaskan jeda.
        void OnDisable() => Tutup();

        void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
            var selesai = saatSelesai;
            saatSelesai = null;
            selesai?.Invoke();
        }
    }
}
