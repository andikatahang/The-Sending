using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Scene_Watuireng (Malam 7, Game Story Malam 7 butir 3-8): jalan sawah dengan Sosok membuntuti, kandang
    // cemani, buku catatan (PemicuDialog terpisah), lilin sesaji, kendi induk (M7), lontar (pilihan terakhir), lalu checkpoint
    // dan kejaran Mbah Wiryo sampai gerbang kebun. Lolos: Balas = epilog (GameManager), Lapor = kartu pulang lalu rumah.
    public class AlurWatuireng : MonoBehaviour
    {
        // Nama SignalAsset di Timeline Watuireng_Memergoki (PembuatWatuireng memakai konstanta yang sama).
        public const string SinyalMemergoki = "Memergoki";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] Transform titikMulai, titikKejar, titikMunculWiryo;
        [SerializeField] Transform kandang;                 // pusat kandang cemani: jarak ke pemain dibandingkan KokokCemani.Jarak
        [SerializeField] LilinPasaran[] lilin = new LilinPasaran[5];
        [SerializeField] InteraktifAlur kendi, lontar;
        [SerializeField] ZonaPemicu gerbangKebun;          // trigger di gerbang kebun: lolos dari kejaran
        [SerializeField] PembuntutSosok pembuntut;          // objek Sosok di jalan sawah
        [SerializeField] GameObject wiryoSemadi;            // Wiryo duduk bersemadi (diam); mati saat kejaran
        [SerializeField] SosokSantet wiryo;                 // Wiryo pengejar (NavMesh)
        [SerializeField] PemutarTimeline timelineMemergoki; // boleh kosong: langsung dialog
        [SerializeField] DataDialog dialogKendi, dialogLontar, dialogMemergoki;
        [SerializeField] Suara suaraKokok, suaraLilin, suaraStinger, musikKejar;

        PuzzleUrutan<Pasaran> puzzle = LilinSesaji.Buat(7);
        bool berjalan, sibuk, kejarBerjalan, obatTercatat;
        float kokokBerikut;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            foreach (var l in lilin) l.Dipadamkan += SaatLilin;
            kendi.Ditekan += SaatKendi;
            lontar.Ditekan += SaatLontar;
            gerbangKebun.Masuk += SaatGerbang;
            wiryo.Menangkap += SaatTertangkap;
            if (timelineMemergoki != null) timelineMemergoki.Sinyal += SaatSinyal;
        }

        void OnDisable()
        {
            foreach (var l in lilin) l.Dipadamkan -= SaatLilin;
            kendi.Ditekan -= SaatKendi;
            lontar.Ditekan -= SaatLontar;
            gerbangKebun.Masuk -= SaatGerbang;
            wiryo.Menangkap -= SaatTertangkap;
            if (timelineMemergoki != null) timelineMemergoki.Sinyal -= SaatSinyal;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurWatuireng butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            kartu.HitamSeketika();
            wiryo.gameObject.SetActive(false);
            if (s.MalamKe != 7 || s.Ending != Ending.Belum)
            {
                Debug.LogWarning("AlurWatuireng: scene dimuat di luar Malam 7.", this);
                kartu.StartCoroutine(kartu.DariHitam());
                return;
            }
            berjalan = true;
            obatTercatat = s.ObatSudahDiberikan;
            bool dariCheckpoint = s.PilihanAkhir != PilihanAkhir.Belum && !s.LolosKejaranWiryo;
            foreach (var l in lilin) l.Nyalakan(!dariCheckpoint);
            kendi.gameObject.SetActive(!dariCheckpoint);
            pembuntut.gameObject.SetActive(!dariCheckpoint);
            if (dariCheckpoint) StartCoroutine(Kejaran(dariCheckpoint: true));
            else StartCoroutine(Datang());
        }

        IEnumerator Datang()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            yield return kartu.StartCoroutine(kartu.DariHitam());
            hud.Tampilkan("Jalan sawah ke Watuireng. Jangan sampai Mbah Wiryo tahu aku datang.");
            sibuk = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (!berjalan || gm == null || gm.Dijeda) return;
            var s = gm.State;
            if (s.Ending != Ending.Belum) return;
            // ObatMalam7.Cek (di JamGim) memberi obat otomatis pukul 21.00 di jalur utama.
            if (!obatTercatat && s.ObatSudahDiberikan)
            {
                obatTercatat = true;
                hud.Tampilkan("Laras memberi Ibu obat pukul 21.00.");
            }
            if (!sibuk && !kejarBerjalan) CekKokok(s);
        }

        // Time.time berhenti saat dijeda (timeScale 0), jadi jeda kokok dihitung dalam waktu nyata bermain.
        void CekKokok(GameState s)
        {
            Vector3 a = pemain.transform.position, b = kandang.position;
            float jarak = new Vector2(a.x - b.x, a.z - b.z).magnitude;
            if (!KokokCemani.Berkokok(jarak) || Time.time < kokokBerikut) return;
            kokokBerikut = Time.time + KokokCemani.JedaDetik;
            suaraKokok.Putar();
            JamGim.Majukan(s, KokokCemani.Menit);
            hud.Tampilkan("Ayam cemani berkokok ribut! Mbah Wiryo bergerak dalam semadinya.");
        }

        void SaatLilin(LilinPasaran l)
        {
            if (!berjalan || sibuk || kejarBerjalan || puzzle.Selesai) return;
            if (puzzle.Pilih(l.Pasaran))
            {
                l.Nyalakan(false);
                suaraLilin.Putar();
                if (puzzle.Selesai) hud.Tampilkan("Lilin terakhir padam. Kendi induk bisa diambil [E].");
                return;
            }
            JamGim.Majukan(State, LilinSesaji.MenitSalah);
            foreach (var x in lilin) x.Nyalakan(true);
            puzzle = LilinSesaji.Buat(7);
            suaraStinger.Putar();
            hud.Tampilkan("Salah urutan. Mbah Wiryo makin gelisah. Ulangi dari Kliwon.");
        }

        void SaatKendi()
        {
            if (!berjalan || sibuk || kejarBerjalan) return;
            if (!puzzle.Selesai) hud.Tampilkan("Lilin sesaji masih menyala di sekeliling kendi. Padamkan dulu sesuai urutan pasaran.");
            else StartCoroutine(AmbilKendi());
        }

        IEnumerator AmbilKendi()
        {
            sibuk = true;
            yield return dialog.MulaiDanTunggu(dialogKendi);   // efek media:M7 (terfoto otomatis)
            kendi.gameObject.SetActive(false);
            hud.Tampilkan("Ada lontar di samping tempat kendi tadi [E].");
            sibuk = false;
        }

        void SaatLontar()
        {
            if (!berjalan || sibuk || kejarBerjalan || State.PilihanAkhir != PilihanAkhir.Belum) return;
            if (!State.MediaDitemukan[Kode.M7]) hud.Tampilkan("Lontar tua bertulisan Jawa. Kendi induk dulu.");
            else StartCoroutine(BacaLontar());
        }

        IEnumerator BacaLontar()
        {
            sibuk = true;
            yield return dialog.MulaiDanTunggu(dialogLontar);   // efek pilihan-akhir:Lapor/Balas
            if (State.PilihanAkhir == PilihanAkhir.Belum)
            {
                sibuk = false;
                yield break;
            }
            GameManager.Instance.SimpanCheckpoint();   // checkpoint sebelum kejaran Wiryo
            yield return Kejaran(dariCheckpoint: false);
        }

        // Pola AlurMalam4.Kejaran: cutscene Wiryo memergoki, lalu Wiryo langsung mengejar.
        IEnumerator Kejaran(bool dariCheckpoint)
        {
            sibuk = true;
            if (dariCheckpoint)
            {
                pemain.Pindah(titikKejar);
                yield return kartu.StartCoroutine(kartu.DariHitam());
            }
            adegan.Mulai();
            try
            {
                if (timelineMemergoki != null) yield return timelineMemergoki.PutarDanTunggu();
                yield return dialog.MulaiDanTunggu(dialogMemergoki);
            }
            finally { adegan.Selesai(); }
            if (wiryoSemadi != null) wiryoSemadi.SetActive(false);
            wiryo.transform.SetPositionAndRotation(titikMunculWiryo.position, titikMunculWiryo.rotation);
            wiryo.gameObject.SetActive(true);
            wiryo.KejarLangsung();
            musikKejar.Putar();
            kejarBerjalan = true;
            hud.Tampilkan("Lari! Keluar lewat kebun ke gerbang!");
            sibuk = false;
        }

        void SaatSinyal(string nama)
        {
            if (nama == SinyalMemergoki)
            {
                suaraStinger.Putar();
                suaraKokok.Putar();
            }
            else Debug.LogWarning($"AlurWatuireng: sinyal tidak dikenal '{nama}'.", this);
        }

        void SaatGerbang()
        {
            if (!berjalan || !kejarBerjalan) return;
            kejarBerjalan = false;
            StartCoroutine(Lolos());
        }

        IEnumerator Lolos()
        {
            sibuk = true;
            var gm = GameManager.Instance;
            wiryo.gameObject.SetActive(false);
            musikKejar.Hentikan();
            adegan.Mulai();   // jeda dilepas KameraAdegan.OnDisable saat scene dibongkar
            PenentuEnding.LolosDariWiryo(gm.State);
            if (gm.State.Ending != Ending.Belum) yield break;   // Balas Dendam: GameManager memuat epilog
            gm.SimpanCheckpoint();   // checkpoint setelah lolos (Lapor)
            yield return kartu.StartCoroutine(kartu.Tampilkan("",
                "Raka berlari pulang menembus sawah. Kendi induk dan buku catatan itu ia dekap erat.", 5f));
            yield return kartu.StartCoroutine(kartu.KeHitam());
            gm.MuatScene(gm.SceneUntukState);
        }

        void SaatTertangkap()
        {
            if (!kejarBerjalan) return;
            kejarBerjalan = false;
            StartCoroutine(Tertangkap());
        }

        // Pola AlurMalam4.Tertangkap: muat checkpoint sebelum kejaran, Kondisi Ibu -5.
        IEnumerator Tertangkap()
        {
            sibuk = true;
            var gm = GameManager.Instance;
            adegan.Mulai();
            musikKejar.Hentikan();
            suaraStinger.Putar();
            yield return kartu.StartCoroutine(kartu.Tampilkan("", "Mbah Wiryo menangkap Raka.", 2f));
            yield return kartu.StartCoroutine(kartu.KeHitam());
            gm.Tertangkap();
            // Jeda dilepas KameraAdegan.OnDisable saat scene dibongkar.
            gm.MuatScene(gm.SceneUntukState);
        }
    }
}
