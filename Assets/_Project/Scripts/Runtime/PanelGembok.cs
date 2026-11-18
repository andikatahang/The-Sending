using TheSending.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Gembok 4 angka lemari ayah (Game Story Malam 1). Angka diketik; kode dicek oleh GembokLemari di Core.
    public class PanelGembok : MonoBehaviour
    {
        const int JumlahAngka = 4;
        const string Petunjuk = "Ketik 4 angka.   [Backspace] hapus   [Esc] batal";

        [SerializeField] Hud hud;

        GameObject panel;
        Text angka, petunjuk;
        string kode = "";

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Gembok", 16).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.05f, 0.04f, 0.03f, 0.95f), new Vector2(0.3f, 0.3f), new Vector2(0.7f, 0.7f));
            panel = latar.gameObject;
            UiPabrik.BuatTeks(latar.transform, "Judul", 32, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.75f), new Vector2(0.95f, 0.95f)).text =
                "GEMBOK LEMARI BAPAK";
            angka = UiPabrik.BuatTeks(latar.transform, "Angka", 110, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.3f), new Vector2(0.95f, 0.75f));
            petunjuk = UiPabrik.BuatTeks(latar.transform, "Petunjuk", 24, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.28f));
            panel.SetActive(false);
        }

        public void Buka()
        {
            var gm = GameManager.Instance;
            if (Terbuka || gm == null || gm.Dijeda) return;
            kode = "";
            petunjuk.text = Petunjuk;
            panel.SetActive(true);
            gm.Jeda(true);
            Gambar();
        }

        void Update()
        {
            if (!Terbuka) return;
            var gm = GameManager.Instance;
            var kb = Keyboard.current;
            if (gm == null || kb == null) return;

            if (gm.Kontrol.JedaMenu.WasPressedThisFrame())
            {
                Tutup();
                return;
            }
            if (kb.backspaceKey.wasPressedThisFrame && kode.Length > 0) kode = kode.Substring(0, kode.Length - 1);
            for (int d = 0; d <= 9 && kode.Length < JumlahAngka; d++)
            {
                // Di enum Key, Digit1..Digit9 berurutan lalu Digit0; Numpad0..Numpad9 berurutan.
                Key atas = d == 0 ? Key.Digit0 : Key.Digit1 + (d - 1);
                if (kb[atas].wasPressedThisFrame || kb[Key.Numpad0 + d].wasPressedThisFrame) kode += d;
            }
            if (kode.Length == JumlahAngka) Coba(gm);
            Gambar();
        }

        void Coba(GameManager gm)
        {
            if (GembokLemari.Buka(gm.State, kode))
            {
                Tutup();
                if (hud != null) hud.Tampilkan("Gembok terbuka.");
                return;
            }
            kode = "";
            petunjuk.text = "Gembok tidak terbuka.\n" + Petunjuk;
        }

        // Panel dimatikan atau dihancurkan saat terbuka: lepaskan jeda.
        void OnDisable() => Tutup();

        void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }

        void Gambar() => angka.text = kode.PadRight(JumlahAngka, '_');
    }
}
