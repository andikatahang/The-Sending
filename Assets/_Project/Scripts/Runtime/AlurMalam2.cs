using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Malam 2 (Game Story, Malam 2): kompas di kamar Ibu, papan kopong, kunci gudang dari Laras, Sosok Santet
    // di celah gudang, boneka (M1), puzzle aksara, weton Ibu dan hitung mundur, Mbah Wiryo dan pilihannya, lalu sekuens
    // bersembunyi pertama sampai kamar Raka. Obat (PemberiObat, JadwalObat), kursi tunggu, linggis (AmbilBarang),
    // papan (PapanLantai), boneka di kotak (BonekaDiKotak), dan tempat sembunyi berjalan sendiri.
    public class AlurMalam2 : MonoBehaviour
    {
        const float LamaGemetar = 2f, JedaSebelumTutup = 3f;
        // Game Story, Malam 2 dan Malam 3, "Ringkasan siang" (dikutip persis).
        const string RingkasanMalam2 =
            "Ibu tetap menolak dibawa ke rumah sakit dan mengamuk setiap kali dipaksa. " +
            "Raka mulai mendengar bisik-bisik tetangga soal Bu Sumi.";
        const string RingkasanMalam3 =
            "Raka mencari informasi di desa. Sebagian warga menyapanya ramah, sebagian menghindar.";
        // Nama SignalAsset di Timeline Malam2_CelahGudang (PembuatMalam2 memakai konstanta yang sama).
        public const string SinyalStinger = "Stinger";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] PanelAksara aksara;
        [SerializeField] DataDialog dialogWeton, dialogWiryo, dialogBisikan;
        [SerializeField] Transform titikMulai, titikTamu, titikLarasTamu, titikKamarLaras, bidikWiryo, bidikLaras, bidikCelah;
        [SerializeField] PemutarTimeline timelineCelah;
        [SerializeField] InteraktifAlur pintuGudang, bacaBoneka;
        [SerializeField] PapanLantai papan;
        [SerializeField] ZonaPemicu zonaPapan, zonaGudang, zonaKamarRaka;
        [SerializeField] GameObject sosokCelah, mbahWiryo, larasMalam2;
        [SerializeField] Transform larasVisual;
        [SerializeField] SosokSantet sosok;
        [SerializeField] Suara suaraKopong, suaraBongkar, suaraKunci, suaraPintuGudang, suaraStinger, suaraKetuk, suaraDetak;
        [SerializeField] Suara musikTegang, gerimis;

        bool berjalan, sibuk, kopongSudah, celahSudah, sembunyiBerjalan, detakBerbunyi, gantiDimulai;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            timelineCelah.Sinyal += SaatSinyal;
            pintuGudang.Ditekan += SaatPintuGudang;
            bacaBoneka.Ditekan += BukaAksara;
            papan.Dibongkar += SaatDibongkar;
            zonaPapan.Masuk += SaatInjakPapan;
            zonaGudang.Masuk += SaatMasukGudang;
            zonaKamarRaka.Masuk += SaatSampaiKamarRaka;
            sosok.Menangkap += SaatTertangkap;
            if (GameManager.Instance != null) GameManager.Instance.Berubah += SaatBerubah;
        }

        void OnDisable()
        {
            timelineCelah.Sinyal -= SaatSinyal;
            pintuGudang.Ditekan -= SaatPintuGudang;
            bacaBoneka.Ditekan -= BukaAksara;
            papan.Dibongkar -= SaatDibongkar;
            zonaPapan.Masuk -= SaatInjakPapan;
            zonaGudang.Masuk -= SaatMasukGudang;
            zonaKamarRaka.Masuk -= SaatSampaiKamarRaka;
            sosok.Menangkap -= SaatTertangkap;
            if (GameManager.Instance != null) GameManager.Instance.Berubah -= SaatBerubah;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurMalam2 butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            if (s.MalamKe >= 3)
            {
                hud.Tampilkan("Malam 3 belum dibuat (Fase 3).");
                return;
            }
            if (s.MalamKe != 2) return;
            berjalan = true;
            kartu.HitamSeketika();
            KeadaanSesuaiState(s);
            gerimis.Putar();
            // Checkpoint sebelum sekuens bersembunyi (K12-A): tertangkap atau "Lanjutkan" mengulang sekuens itu saja.
            if (TujuanMalam.Sudah(s, TujuanMalam.PilihanWiryo) && !TujuanMalam.Sudah(s, TujuanMalam.SembunyiPertama))
                StartCoroutine(MulaiSembunyi(dariCheckpoint: true));
            else
                StartCoroutine(Pembuka());
        }

        // Isi rumah mengikuti state: awal malam, "Lanjutkan", atau muat ulang setelah tertangkap.
        void KeadaanSesuaiState(GameState s)
        {
            pintuGudang.gameObject.SetActive(!Inventaris.Punya(s, Inventaris.Linggis));
            PerbaruiPromptGudang(s);
            larasMalam2.SetActive(!TujuanMalam.Sudah(s, TujuanMalam.PilihanWiryo));
            bacaBoneka.gameObject.SetActive(s.MediaDitemukan[Kode.M1] && !TujuanMalam.Sudah(s, TujuanMalam.BacaAksara));
            mbahWiryo.SetActive(false);
            sosokCelah.SetActive(false);
            sosok.gameObject.SetActive(false);
        }

        void PerbaruiPromptGudang(GameState s) =>
            pintuGudang.UbahPrompt(Inventaris.Punya(s, Inventaris.KunciGudang) ? "Buka pintu gudang" : null);

        void SaatBerubah(Perubahan p)
        {
            if (berjalan && p == Perubahan.Barang) PerbaruiPromptGudang(State);
        }

        IEnumerator Pembuka()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Malam 2: Boneka di Bawah Lantai",
                KalenderJawa.NamaMalam(2) + "\n\n" + RingkasanMalam2 + "\n\nTujuan: Mencari sumber bau anyir di kamar Ibu", 8f));
            hud.Tampilkan("Kamar Ibu berbau anyir.");
            hud.Tampilkan("Tekan C untuk membuka kompas HP. Jarumnya berputar di dekat benda santet.");
            sibuk = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (!berjalan || sibuk || gm == null || gm.Dijeda) return;
            var s = gm.State;
            if (s.MalamKe != 2 || s.Ending != Ending.Belum) return;
            if (sembunyiBerjalan) AturDetak();
            if (TujuanMalam.Sudah(s, TujuanMalam.SembunyiPertama))
            {
                if (!gantiDimulai && PergantianMalam.BisaLanjut(s)) StartCoroutine(GantiMalam(s));
                return;
            }
            if (!sembunyiBerjalan && JamGim.SaatnyaWiryoDatang(s)) StartCoroutine(KunjunganWiryo(s));
        }

        // Detak jantung selama Raka bersembunyi dan Sosok Santet berkeliaran.
        void AturDetak()
        {
            if (pemain.Bersembunyi == detakBerbunyi) return;
            detakBerbunyi = pemain.Bersembunyi;
            if (detakBerbunyi) suaraDetak.Putar();
            else suaraDetak.Hentikan();
        }

        // Game Story Malam 2 butir 2: papan yang berbunyi kopong saat diinjak.
        void SaatInjakPapan()
        {
            if (!berjalan || sibuk || kopongSudah || State.MediaDitemukan[Kode.M1]) return;
            kopongSudah = true;
            suaraKopong.Putar();
            hud.Tampilkan("Papan lantai di dekat ranjang berbunyi kopong.");
        }

        // Game Story Malam 2 butir 3: Laras memberikan kunci gudang.
        void SaatPintuGudang()
        {
            if (!berjalan) return;
            if (!Inventaris.Punya(State, Inventaris.KunciGudang))
            {
                hud.Tampilkan("Pintu gudang terkunci. Laras mungkin menyimpan kuncinya.");
                return;
            }
            suaraKunci.Putar();
            suaraPintuGudang.Putar();
            pintuGudang.gameObject.SetActive(false);
            hud.Tampilkan("Gudang gelap. Nyalakan senter (F).");
        }

        void SaatMasukGudang()
        {
            if (!berjalan || sibuk || celahSudah) return;
            var s = State;
            if (Inventaris.Punya(s, Inventaris.Linggis) || TujuanMalam.Sudah(s, TujuanMalam.PilihanWiryo)) return;
            StartCoroutine(CelahGudang());
        }

        // Game Story Malam 2 butir 3: Sosok Santet terlihat pertama kali lewat celah dinding bambu (kejutan, bukan kejar).
        // Timeline Malam2_CelahGudang (4 detik): kamera mendekat ke celah, sosok lewat di luar, stinger di detik 1,9.
        IEnumerator CelahGudang()
        {
            sibuk = true;
            celahSudah = true;
            adegan.Mulai(bidikCelah);
            try
            {
                sosokCelah.SetActive(true);
                yield return timelineCelah.PutarDanTunggu();
            }
            finally
            {
                sosokCelah.SetActive(false);
                adegan.Selesai();
            }
            hud.Tampilkan("Ada sesuatu di balik dinding bambu.");
            hud.Tampilkan("Linggis bersandar di dinding gudang.");
            sibuk = false;
        }

        void SaatSinyal(string nama)
        {
            if (nama == SinyalStinger) suaraStinger.Putar();
            else Debug.LogWarning($"AlurMalam2: sinyal tidak dikenal '{nama}'.", this);
        }

        // Game Story Malam 2 butir 4: boneka kain di bawah papan (M1). Media otomatis difoto (desain 4.7).
        void SaatDibongkar()
        {
            if (!berjalan || State.MediaDitemukan[Kode.M1]) return;
            suaraBongkar.Putar();
            MediaSantet.Temukan(State, Kode.M1);
            bacaBoneka.gameObject.SetActive(true);
            hud.Tampilkan("Boneka itu otomatis difoto dan masuk Papan Bukti.");
            BukaAksara();
        }

        // Game Story Malam 2 butir 5: tulisan di boneka dicocokkan dengan tabel Halaman 2.
        void BukaAksara()
        {
            if (!berjalan || TujuanMalam.Sudah(State, TujuanMalam.BacaAksara)) return;
            aksara.Buka(SaatAksaraTerbaca);
        }

        void SaatAksaraTerbaca()
        {
            TujuanMalam.Selesaikan(State, TujuanMalam.BacaAksara);
            bacaBoneka.gameObject.SetActive(false);   // Raka mengangkat boneka dari lubang
            StartCoroutine(Weton());
        }

        // Game Story Malam 2 butir 6: RATMI dan SELASA KLIWON adalah nama dan weton Ibu, lima malam lagi.
        IEnumerator Weton()
        {
            sibuk = true;
            yield return dialog.MulaiDanTunggu(dialogWeton);
            var s = State;
            hud.Tampilkan($"Hitung mundur dimulai: Selasa Kliwon {KalenderJawa.SisaMalamKeSelasaKliwon(s.MalamKe)} malam lagi.");
            if (!JamGim.SaatnyaWiryoDatang(s))
                hud.Tampilkan($"Obat Ibu pukul {JamGim.Format(JamGim.MulaiJendelaObat(s.MalamKe))}. Kursi di kamar Ibu bisa dipakai menunggu.");
            sibuk = false;
        }

        // Game Story Malam 2 butir 7-8: Mbah Wiryo datang tanpa diundang, menawarkan diri membuang boneka,
        // pilihan pertama pemain, lalu berbisik kepada Laras. Laras tampak gemetar.
        IEnumerator KunjunganWiryo(GameState s)
        {
            sibuk = true;
            var gm = GameManager.Instance;
            suaraKetuk.Putar();
            hud.Tampilkan("Ada yang mengetuk pintu depan.");
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.KeHitam());
                pemain.Pindah(titikTamu);
                larasMalam2.SetActive(true);
                larasMalam2.transform.SetPositionAndRotation(titikLarasTamu.position, titikLarasTamu.rotation);
                mbahWiryo.SetActive(true);
                adegan.Bidik(bidikWiryo);
                yield return kartu.StartCoroutine(kartu.DariHitam());
                yield return dialog.MulaiDanTunggu(dialogWiryo);   // pilihan: serahkan (serahkan:M1) atau simpan
                adegan.Bidik(bidikLaras);
                yield return dialog.MulaiDanTunggu(dialogBisikan);
                yield return Gemetar(larasVisual, LamaGemetar);
                yield return kartu.StartCoroutine(kartu.KeHitam());
                mbahWiryo.SetActive(false);
                larasMalam2.SetActive(false);
            }
            finally { adegan.Selesai(); }
            if (KeMenuJikaEnding(gm)) yield break;   // menyerahkan boneka = Kondisi Ibu -8
            if (!TujuanMalam.Sudah(s, TujuanMalam.PilihanWiryo))
            {
                Debug.LogError("Dialog Mbah Wiryo selesai tanpa pilihan; dianggap boneka disimpan.", this);
                TujuanMalam.Selesaikan(s, TujuanMalam.PilihanWiryo);
            }
            hud.Tampilkan(s.MediaDiserahkan[Kode.M1] ? "Mbah Wiryo membawa boneka itu pergi." : "Boneka disimpan di kotak kayu di kamar Raka.");
            yield return MulaiSembunyi(dariCheckpoint: false);
        }

        // Pengganti animasi "Laras gemetar" sampai model beranimasi datang.
        static IEnumerator Gemetar(Transform t, float lama)
        {
            var awal = t.localPosition;
            for (float w = 0f; w < lama; w += Time.unscaledDeltaTime)
            {
                t.localPosition = awal + new Vector3(Random.Range(-0.015f, 0.015f), 0f, Random.Range(-0.015f, 0.015f));
                yield return null;
            }
            t.localPosition = awal;
        }

        // Game Story Malam 2 butir 9: Sosok Santet berpatroli di ruang tengah; Raka harus kembali ke kamarnya tanpa ketahuan.
        IEnumerator MulaiSembunyi(bool dariCheckpoint)
        {
            sibuk = true;
            var gm = GameManager.Instance;
            larasMalam2.SetActive(false);
            pemain.Pindah(titikKamarLaras);
            if (!dariCheckpoint) gm.SimpanCheckpoint();   // K12-A: checkpoint sebelum sekuens bersembunyi
            string teks = dariCheckpoint
                ? "Gemerincing paku terdengar dari ruang tengah."
                : "Laras masuk ke kamarnya. Raka menemaninya sampai Laras tenang.\n\nLalu terdengar gemerincing paku dari ruang tengah.";
            yield return kartu.StartCoroutine(kartu.Tampilkan(KalenderJawa.NamaMalam(2), teks, 5f));
            sembunyiBerjalan = true;
            sosok.gameObject.SetActive(true);
            musikTegang.Putar();
            hud.Tampilkan("Kembali ke kamar Raka tanpa ketahuan.");
            hud.Tampilkan("Sosok itu mendengar langkah. Jangan berlari.");
            hud.Tampilkan("Bersembunyi di lemari (E). Tahan Spasi untuk menahan napas.");
            sibuk = false;
        }

        void SaatSampaiKamarRaka()
        {
            if (!sembunyiBerjalan || sibuk) return;
            StartCoroutine(Selamat());
        }

        IEnumerator Selamat()
        {
            sibuk = true;
            sembunyiBerjalan = false;
            sosok.gameObject.SetActive(false);
            musikTegang.Hentikan();
            suaraDetak.Hentikan();
            detakBerbunyi = false;
            TujuanMalam.Selesaikan(State, TujuanMalam.SembunyiPertama);
            hud.Tampilkan("Gemerincing paku itu menjauh, lalu hilang.");
            for (float t = 0f; t < JedaSebelumTutup; t += Time.unscaledDeltaTime) yield return null;
            sibuk = false;
        }

        void SaatTertangkap()
        {
            if (!sembunyiBerjalan) return;
            sembunyiBerjalan = false;
            StartCoroutine(Tertangkap());
        }

        // Desain 4.2: muat checkpoint (sebelum sekuens ini, K12-A), lalu Kondisi Ibu -5. Scene dimuat ulang dari state itu.
        IEnumerator Tertangkap()
        {
            sibuk = true;
            var gm = GameManager.Instance;
            adegan.Mulai();
            musikTegang.Hentikan();
            suaraDetak.Hentikan();
            suaraStinger.Putar();
            yield return kartu.StartCoroutine(kartu.Tampilkan("", "Sosok itu menemukan Raka.", 2f));
            yield return kartu.StartCoroutine(kartu.KeHitam());
            gm.Tertangkap();
            // Jeda dilepas KameraAdegan.OnDisable saat scene dibongkar.
            gm.MuatScene(gm.State.Ending != Ending.Belum ? GameManager.SceneMenuUtama : gm.SceneUntukState);
        }

        // Ending sebelum Fase 7: kembali ke menu utama (layar ending dibuat di Fase 7).
        bool KeMenuJikaEnding(GameManager gm)
        {
            if (gm.State.Ending == Ending.Belum) return false;
            Debug.LogWarning("Ending tercapai di Malam 2; layar ending dibuat di Fase 7.", this);
            gm.MuatScene(GameManager.SceneMenuUtama);
            return true;
        }

        IEnumerator GantiMalam(GameState s)
        {
            gantiDimulai = true;
            sibuk = true;
            var gm = GameManager.Instance;
            bool dimuat = false;
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.KeHitam());
                gerimis.Hentikan();
                PergantianMalam.Lanjut(s);        // Malam 3, Kondisi Ibu -12
                if (KeMenuJikaEnding(gm))         // catatan 0B: -12 bisa langsung memicu ending
                {
                    dimuat = true;
                    yield break;
                }
                gm.SimpanCheckpoint();            // checkpoint awal Malam 3
                // Fase 3 mengganti bagian di bawah ini dengan gm.MuatScene(gm.SceneUntukState) untuk sutradara Malam 3.
                yield return kartu.StartCoroutine(kartu.Tampilkan(
                    "Malam 3: Tujuh Paku di Saka",
                    KalenderJawa.NamaMalam(3) + "\n\n" + RingkasanMalam3 + "\n\n(Bersambung. Malam 3 dibuat di Fase 3.)", 8f));
                yield return kartu.StartCoroutine(kartu.KeHitam());
                gm.MuatScene(GameManager.SceneMenuUtama);
                dimuat = true;
            }
            finally { if (!dimuat) adegan.Selesai(); }   // jeda dilepas KameraAdegan.OnDisable saat scene dibongkar
        }
    }
}
