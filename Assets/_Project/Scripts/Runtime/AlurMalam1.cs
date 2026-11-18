using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Malam 1 (Game Story, Malam 1): kartu bab, cutscene pukul 23.00, tutorial memotret, penutup bab.
    // Obat (PemberiObat, JadwalObat) dan gembok (LemariAyah) berjalan sendiri.
    public class AlurMalam1 : MonoBehaviour
    {
        const float LamaKejang = 3f;
        // Nama SignalAsset di Timeline Malam1_Cutscene23 (PembuatRumahKeluarga memakai konstanta yang sama).
        public const string SinyalKejang = "Kejang", SinyalPaku = "Paku", SinyalPadam = "Padam", SinyalLangkah = "Langkah";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] DataDialog dialogPenutup;
        [SerializeField] Transform titikMulai, bidikIbu, ibu;
        [SerializeField] PemutarTimeline timeline23;
        [SerializeField] GameObject larasRuangTengah, larasBerdoa, paku, darah, teplok;
        [SerializeField] Light[] lampuListrik = new Light[0];
        [SerializeField] Suara suaraBatuk, suaraPaku, suaraListrik, suaraLangkah, musikDrone;   // tanpa bisikan doa: suara tokoh ditunda

        bool berjalan, sibuk, penutupSudah;

        void OnEnable() => timeline23.Sinyal += SaatSinyal;
        void OnDisable() => timeline23.Sinyal -= SaatSinyal;

        void SaatSinyal(string nama)
        {
            switch (nama)
            {
                case SinyalKejang: suaraBatuk.Putar(); StartCoroutine(Kejang()); break;
                case SinyalPaku: darah.SetActive(true); paku.SetActive(true); suaraPaku.Putar(); break;
                case SinyalPadam: suaraListrik.Putar(); Padamkan(); break;
                case SinyalLangkah: suaraLangkah.Putar(); break;
                default: Debug.LogWarning($"AlurMalam1: sinyal tidak dikenal '{nama}'.", this); break;
            }
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurMalam1 butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            if (gm.State.MalamKe == 1)
            {
                kartu.HitamSeketika();   // "Lanjutkan" di Malam 1
                Mulai();
            }
        }

        // Dipanggil saat layar hitam: dari akhir Prolog, atau saat scene dimuat di Malam 1.
        public void Mulai()
        {
            if (berjalan) return;
            berjalan = true;
            StartCoroutine(Pembuka());
        }

        IEnumerator Pembuka()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            // Save yang dibuat setelah cutscene (tombol uji F5): rumah langsung dalam keadaan sesudahnya.
            if (TujuanMalam.Sudah(GameManager.Instance.State, TujuanMalam.Cutscene23)) KeadaanSetelahAdegan();
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Malam 1: Paku Pertama",
                "Malam Rabu Wage\n\nTujuan: Mengenal rumah, memberi obat, dan mencari tahu apa yang terjadi", 5f));
            hud.Tampilkan("Obat Ibu diberikan pukul 21.00. Jam ada di kanan atas.");
            sibuk = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (!berjalan || sibuk || gm == null || gm.Dijeda) return;
            var s = gm.State;
            if (s.MalamKe != 1 || s.Ending != Ending.Belum) return;

            if (!TujuanMalam.Sudah(s, TujuanMalam.Cutscene23))
            {
                if (s.MenitGim >= JamGim.Jam23) StartCoroutine(Cutscene23(s));
                return;
            }
            if (!TujuanMalam.Sudah(s, TujuanMalam.PotretPaku)) return;
            if (!penutupSudah) StartCoroutine(Penutup());
            else if (PergantianMalam.BisaLanjut(s)) StartCoroutine(GantiMalam(s));
        }

        // Game Story: pukul 23.00 Ibu kejang lalu memuntahkan darah dan tiga paku berkarat. Listrik padam.
        // Terdengar langkah berat di atap. Urutan dan kamera ada di Timeline Malam1_Cutscene23 (Task 13):
        // 0 kejang + batuk, 3 paku + darah, 5 listrik padam, 6,5 potong ke plafon + langkah, selesai 11,5 detik.
        IEnumerator Cutscene23(GameState s)
        {
            sibuk = true;
            adegan.Mulai(bidikIbu);
            try
            {
                pemain.Pindah(titikMulai);   // Raka sudah di samping ranjang saat adegan selesai
                musikDrone.Putar();
                yield return timeline23.PutarDanTunggu();
                KeadaanSetelahAdegan();      // juga menjadi cadangan jika ada sinyal yang tidak terpicu
            }
            finally { adegan.Selesai(); }
            TujuanMalam.Selesaikan(s, TujuanMalam.Cutscene23);
            hud.Tampilkan("Arahkan ke paku di lantai, lalu tekan Q untuk memotretnya.");
            hud.Tampilkan("Foto petunjuk masuk ke Papan Bukti di Jurnal (Tab).");
            sibuk = false;
        }

        // Pengganti animasi kejang sampai model Mixamo datang: tubuh Ibu berguncang.
        IEnumerator Kejang()
        {
            var awal = ibu.localRotation;
            for (float t = 0f; t < LamaKejang; t += Time.unscaledDeltaTime)
            {
                ibu.localRotation = awal * Quaternion.Euler(Random.Range(-6f, 6f), 0f, Random.Range(-6f, 6f));
                yield return null;
            }
            ibu.localRotation = awal;
        }

        void KeadaanSetelahAdegan()
        {
            darah.SetActive(true);
            paku.SetActive(true);
            larasRuangTengah.SetActive(false);
            larasBerdoa.SetActive(true);   // Laras menangis sambil berdoa di samping ranjang
            Padamkan();
        }

        void Padamkan()
        {
            foreach (var lampu in lampuListrik) lampu.enabled = false;
            teplok.SetActive(true);
        }

        IEnumerator Penutup()
        {
            sibuk = true;
            penutupSudah = true;
            yield return dialog.MulaiDanTunggu(dialogPenutup);
            if (!PergantianMalam.BisaLanjut(GameManager.Instance.State))
                hud.Tampilkan("Masih ada yang belum diperiksa: lemari Bapak di kamar Ibu.");
            sibuk = false;
        }

        IEnumerator GantiMalam(GameState s)
        {
            sibuk = true;
            var gm = GameManager.Instance;
            bool dimuat = false;
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.KeHitam());
                musikDrone.Hentikan();
                PergantianMalam.Lanjut(s);        // Malam 2, Kondisi Ibu -12
                if (s.Ending != Ending.Belum)     // catatan 0B: -12 bisa langsung memicu ending
                {
                    Debug.LogWarning("Ending tercapai saat pergantian malam; layar ending dibuat di Fase 7.", this);
                    gm.MuatScene(GameManager.SceneMenuUtama);
                    dimuat = true;
                    yield break;
                }
                gm.SimpanCheckpoint();            // checkpoint awal Malam 2
                // Rumah dimuat ulang untuk Malam 2: lampu menyala lagi, paku dan teplok hilang. Kartu bab ditampilkan AlurMalam2.
                gm.MuatScene(gm.SceneUntukState);
                dimuat = true;
            }
            finally { if (!dimuat) adegan.Selesai(); }   // jeda dilepas KameraAdegan.OnDisable saat scene dibongkar
        }
    }
}
