using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Scene_MainMenu: peringatan konten dulu, lalu Lanjutkan / Mulai Baru / Pengaturan / Keluar.
    public class MenuUtama : MonoBehaviour
    {
        // Topik dari GDD (darah, paku, kekerasan massa, kematian; nama fiksi; ritual bukan panduan). Susunan kalimat oleh pengembang.
        public const string TeksPeringatan =
            "Gim ini memuat darah, paku, kekerasan massa, dan kematian.\n\n" +
            "Seluruh nama desa dan tokoh adalah fiksi.\nRitual dalam gim tidak disajikan sebagai panduan nyata.";

        [SerializeField] PanelPengaturan pengaturan;

        GameObject layarPeringatan, layarKonfirmasi;
        CanvasGroup grupMenu;
        Button tombolMengerti, tombolLanjutkan, tombolMulaiBaru, tombolBatal;
        Text pesan;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvasMenu("MenuUtama", 50).transform;
            akar.SetParent(transform, false);

            var menu = UiPabrik.BuatPanel(akar, "Menu", Color.clear, Vector2.zero, Vector2.one);
            grupMenu = menu.gameObject.AddComponent<CanvasGroup>();
            var m = menu.transform;
            UiPabrik.BuatTeks(m, "Judul", 120, TextAnchor.MiddleCenter, new Vector2(0.1f, 0.62f), new Vector2(0.9f, 0.88f)).text = "THE SENDING";
            tombolLanjutkan = UiPabrik.BuatTombol(m, "Lanjutkan", new Vector2(0.38f, 0.46f), new Vector2(0.62f, 0.54f), Lanjutkan);
            tombolMulaiBaru = UiPabrik.BuatTombol(m, "Mulai Baru", new Vector2(0.38f, 0.36f), new Vector2(0.62f, 0.44f), MulaiBaru);
            UiPabrik.BuatTombol(m, "Pengaturan", new Vector2(0.38f, 0.26f), new Vector2(0.62f, 0.34f), BukaPengaturan);
            UiPabrik.BuatTombol(m, "Keluar", new Vector2(0.38f, 0.16f), new Vector2(0.62f, 0.24f), Keluar);
            pesan = UiPabrik.BuatTeks(m, "Pesan", 26, TextAnchor.MiddleCenter, new Vector2(0.1f, 0.05f), new Vector2(0.9f, 0.12f));

            layarKonfirmasi = BuatLayar(akar, "Konfirmasi", "Save lama akan ditimpa.\nMulai permainan baru?");
            UiPabrik.BuatTombol(layarKonfirmasi.transform, "Ya, mulai baru", new Vector2(0.28f, 0.30f), new Vector2(0.48f, 0.38f), Mulai);
            tombolBatal = UiPabrik.BuatTombol(layarKonfirmasi.transform, "Batal", new Vector2(0.52f, 0.30f), new Vector2(0.72f, 0.38f), TutupKonfirmasi);

            layarPeringatan = BuatLayar(akar, "Peringatan", "PERINGATAN KONTEN\n\n" + TeksPeringatan);
            tombolMengerti = UiPabrik.BuatTombol(layarPeringatan.transform, "Lanjut", new Vector2(0.40f, 0.20f), new Vector2(0.60f, 0.28f), TutupPeringatan);
        }

        // Layar penuh hitam di atas menu; menutup klik ke tombol di belakangnya.
        static GameObject BuatLayar(Transform akar, string nama, string teks)
        {
            var latar = UiPabrik.BuatPanel(akar, nama, Color.black, Vector2.zero, Vector2.one);
            latar.raycastTarget = true;
            UiPabrik.BuatTeks(latar.transform, "Teks", 36, TextAnchor.MiddleCenter, new Vector2(0.15f, 0.42f), new Vector2(0.85f, 0.85f)).text = teks;
            latar.gameObject.SetActive(false);
            return latar.gameObject;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("MenuUtama butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            tombolLanjutkan.gameObject.SetActive(gm.AdaSave);
            if (gm.PeringatanSudahTampil)
            {
                FokusMenu();
                return;
            }
            grupMenu.interactable = false;
            layarPeringatan.SetActive(true);
            UiPabrik.Pilih(tombolMengerti);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || !gm.Kontrol.JedaMenu.WasPressedThisFrame()) return;
            if (pengaturan != null && pengaturan.Terbuka) pengaturan.Tutup();
            else if (layarKonfirmasi.activeSelf) TutupKonfirmasi();
        }

        void FokusMenu()
        {
            grupMenu.interactable = true;
            UiPabrik.Pilih(tombolLanjutkan.gameObject.activeSelf ? tombolLanjutkan : tombolMulaiBaru);
        }

        void TutupPeringatan()
        {
            GameManager.Instance.PeringatanSudahTampil = true;
            layarPeringatan.SetActive(false);
            FokusMenu();
        }

        void Lanjutkan()
        {
            var gm = GameManager.Instance;
            if (gm.Lanjutkan()) gm.MuatScene(GameManager.SceneAwal);
            else pesan.text = "Save tidak bisa dibaca. Pilih Mulai Baru.";
        }

        void MulaiBaru()
        {
            if (!GameManager.Instance.AdaSave)
            {
                Mulai();
                return;
            }
            grupMenu.interactable = false;
            layarKonfirmasi.SetActive(true);
            UiPabrik.Pilih(tombolBatal);
        }

        void TutupKonfirmasi()
        {
            layarKonfirmasi.SetActive(false);
            FokusMenu();
        }

        void Mulai()
        {
            var gm = GameManager.Instance;
            gm.MulaiBaru();
            gm.SimpanCheckpoint();   // langsung menimpa save lama, supaya Lanjutkan tidak memuat permainan sebelumnya
            gm.MuatScene(GameManager.SceneAwal);
        }

        void BukaPengaturan()
        {
            if (pengaturan == null)
            {
                Debug.LogWarning("MenuUtama tanpa PanelPengaturan.", this);
                return;
            }
            grupMenu.interactable = false;
            pengaturan.Buka(FokusMenu);
        }

        void Keluar()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
