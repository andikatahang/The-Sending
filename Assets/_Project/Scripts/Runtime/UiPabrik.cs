using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Membangun uGUI lewat kode, supaya tidak ada prefab UI yang harus disambung manual.
    public static class UiPabrik
    {
        static readonly Color WarnaTeks = new Color(0.9f, 0.88f, 0.82f);
        static readonly Color WarnaTombol = new Color(0.10f, 0.09f, 0.08f, 0.92f);
        static readonly Color WarnaTombolSorot = new Color(0.353f, 0.102f, 0.071f, 1f);   // karat #5A1A12
        static readonly Color WarnaTombolTekan = new Color(0.55f, 0.20f, 0.12f, 1f);

        static Font font, fontTulisan;

        // Satu pintu untuk font. VT323 = UI dan subtitle, Kalam = tulisan tangan buku harian.
        // File .ttf ditaruh tim di Assets/_Project/Art/Fonts/Resources/ dengan nama di bawah.
        public static Font Font => font != null ? font : (font = MuatFont("VT323-Regular"));
        public static Font FontTulisan => fontTulisan != null ? fontTulisan : (fontTulisan = MuatFont("Kalam-Regular"));

        static Font MuatFont(string nama)
        {
            var f = Resources.Load<Font>(nama);
            if (f != null) return f;
            Debug.LogWarning($"Font '{nama}.ttf' belum ada di Assets/_Project/Art/Fonts/Resources/; memakai font bawaan.");
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        // Domain reload mati: kosongkan cache supaya font yang baru ditaruh tim terbaca di Play berikutnya.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Bersihkan()
        {
            font = null;
            fontTulisan = null;
        }

        public static Canvas BuatCanvas(string nama, int urutan)
        {
            var go = new GameObject(nama, typeof(Canvas), typeof(CanvasScaler));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = urutan;
            var skala = go.GetComponent<CanvasScaler>();
            skala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            skala.referenceResolution = new Vector2(1920, 1080);
            skala.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        // Canvas yang bisa diklik: menu utama, menu jeda, pengaturan.
        public static Canvas BuatCanvasMenu(string nama, int urutan)
        {
            var canvas = BuatCanvas(nama, urutan);
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            if (EventSystem.current == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            return canvas;
        }

        public static RectTransform Isi(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static Text BuatTeks(Transform induk, string nama, int ukuran, TextAnchor rata, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(nama, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(induk, false);
            Isi((RectTransform)go.transform, anchorMin, anchorMax);
            var t = go.GetComponent<Text>();
            t.font = Font;
            t.fontSize = ukuran;
            t.alignment = rata;
            t.color = WarnaTeks;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Truncate;
            t.raycastTarget = false;
            return t;
        }

        public static Image BuatPanel(Transform induk, string nama, Color warna, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(nama, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(induk, false);
            Isi((RectTransform)go.transform, anchorMin, anchorMax);
            var img = go.GetComponent<Image>();
            img.color = warna;
            img.raycastTarget = false;
            return img;
        }

        // Tombol menu: bisa diklik mouse dan dipilih dengan panah/WASD + Enter. Warna sorot = karat (palet style guide).
        public static Button BuatTombol(Transform induk, string label, Vector2 anchorMin, Vector2 anchorMax, UnityAction saatKlik)
        {
            var img = BuatPanel(induk, "Tombol " + label, Color.white, anchorMin, anchorMax);
            img.raycastTarget = true;
            var tombol = img.gameObject.AddComponent<Button>();
            tombol.targetGraphic = img;   // AddComponent saat runtime tidak mengisinya sendiri
            var warna = tombol.colors;
            warna.normalColor = WarnaTombol;
            warna.highlightedColor = WarnaTombolSorot;
            warna.selectedColor = WarnaTombolSorot;
            warna.pressedColor = WarnaTombolTekan;
            tombol.colors = warna;
            tombol.onClick.AddListener(MainkanKlik);
            tombol.onClick.AddListener(saatKlik);
            BuatTeks(img.transform, "Label", 30, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one).text = label;
            return tombol;
        }

        // Suara klik lewat grup SFX. AudioSource sementara hidup lewat pergantian scene (tombol yang memuat scene tidak memotong bunyinya).
        static void MainkanKlik()
        {
            var klip = Resources.Load<AudioClip>("ButtonClick-OldRadio");
            if (klip == null) return;
            var go = new GameObject("KlikTombol");
            Object.DontDestroyOnLoad(go);
            var sumber = go.AddComponent<AudioSource>();
            var mixer = Resources.Load<UnityEngine.Audio.AudioMixer>(PenyimpanPengaturan.JalurMixer);
            if (mixer != null)
            {
                var grup = mixer.FindMatchingGroups("SFX");
                if (grup.Length > 0) sumber.outputAudioMixerGroup = grup[0];
            }
            sumber.PlayOneShot(klip);
            Object.Destroy(go, klip.length + 0.2f);   // timeScale 0: dihapus setelah lanjut, tidak masalah
        }

        // Memindahkan pilihan keyboard ke tombol; null melepas pilihan.
        public static void Pilih(Button tombol)
        {
            if (EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(tombol != null ? tombol.gameObject : null);
        }
    }
}
