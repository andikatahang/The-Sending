using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Menu jeda di scene gameplay. Esc membuka dan menutup; saat pengaturan terbuka, Esc kembali ke menu jeda.
    // Jalan lebih dulu dari panel lain: Esc yang menutup Jurnal, inventaris, atau gembok dibaca di sini saat gim
    // masih dijeda, jadi Esc yang sama tidak ikut membuka menu jeda.
    [DefaultExecutionOrder(-50)]
    public class MenuJeda : MonoBehaviour
    {
        [SerializeField] PanelPengaturan pengaturan;

        GameObject panel;
        CanvasGroup grup;
        Button tombolLanjut;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvasMenu("MenuJeda", 50).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0f, 0f, 0f, 0.8f), Vector2.zero, Vector2.one);
            latar.raycastTarget = true;
            panel = latar.gameObject;
            grup = panel.AddComponent<CanvasGroup>();
            var t = latar.transform;
            UiPabrik.BuatTeks(t, "Judul", 72, TextAnchor.MiddleCenter, new Vector2(0.2f, 0.66f), new Vector2(0.8f, 0.82f)).text = "JEDA";
            tombolLanjut = UiPabrik.BuatTombol(t, "Lanjut", new Vector2(0.38f, 0.52f), new Vector2(0.62f, 0.60f), Tutup);
            UiPabrik.BuatTombol(t, "Pengaturan", new Vector2(0.38f, 0.41f), new Vector2(0.62f, 0.49f), BukaPengaturan);
            UiPabrik.BuatTombol(t, "Keluar ke Menu Utama", new Vector2(0.38f, 0.30f), new Vector2(0.62f, 0.38f), KeMenuUtama);
            panel.SetActive(false);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || !gm.Kontrol.JedaMenu.WasPressedThisFrame()) return;
            if (pengaturan != null && pengaturan.Terbuka) pengaturan.Tutup();
            else if (Terbuka) Tutup();
            else if (!gm.Dijeda) Buka();   // dialog, Jurnal, dan kartu teks tidak ditimpa menu jeda
        }

        // Panel dimatikan atau dihancurkan saat terbuka (misalnya pindah scene): lepaskan jeda.
        void OnDisable() => Tutup();

        void Buka()
        {
            panel.SetActive(true);
            GameManager.Instance.Jeda(true);
            Fokus();
        }

        void Fokus()
        {
            grup.interactable = true;
            UiPabrik.Pilih(tombolLanjut);
        }

        void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            UiPabrik.Pilih(null);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }

        void BukaPengaturan()
        {
            if (pengaturan == null)
            {
                Debug.LogWarning("MenuJeda tanpa PanelPengaturan.", this);
                return;
            }
            grup.interactable = false;   // navigasi keyboard tidak boleh pindah ke tombol di balik pengaturan
            pengaturan.Buka(Fokus);
        }

        // Jeda dilepas oleh OnDisable saat scene ini dibongkar.
        void KeMenuUtama() => GameManager.Instance.MuatScene(GameManager.SceneMenuUtama);
    }
}
