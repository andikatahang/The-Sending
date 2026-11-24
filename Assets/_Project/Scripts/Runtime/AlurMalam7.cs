using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Malam 7 di rumah keluarga (Game Story, Malam 7). Jalur utama: lingkaran garam dan kelor bersama Bu Sumi dan
    // Laras, pintu depan ke Watuireng; pulang setelah lolos (Lapor) = ritual terakhir di anglo, Netralkan(M7), Ending Sejati.
    // Jalur Fitnah: tanpa Bu Sumi, Sosok berpatroli seperti Malam 2, Raka bertahan sampai 00.00 (GameManager memuat epilog).
    public class AlurMalam7 : MonoBehaviour
    {
        // Nama SignalAsset di Timeline Malam7_RitualTerakhir (PembuatMalam7 memakai konstanta yang sama).
        public const string SinyalApi = "ApiInduk", SinyalJerit = "JeritSosok", SinyalLenyap = "SosokLenyap";

        const string JudulBab = "Malam 7: Malam Selasa Kliwon";
        // DRAF: ringkasan untuk dicek tim.
        const string RingkasanUtama =
            "Sore hari Bu Sumi datang membawa garam dan seikat daun kelor. Laras tidak lagi takut padanya.";
        const string TujuanUtama = "Tujuan: Membakar media induk sebelum tengah malam";
        const string RingkasanFitnah =
            "Tanpa Bu Sumi, Raka tidak tahu apa-apa soal induk santet. Ia hanya bisa bertahan di dalam rumah " +
            "dan menjaga Ibu sampai tengah malam.";
        const string TujuanFitnah = "Tujuan: Menjaga Ibu sampai tengah malam";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] Transform titikMulai, titikPulang, bidikRanjang, bidikAnglo, titikBuSumiDapur;
        [SerializeField] InteraktifAlur pintuDepan, buSumi;   // buSumi: tokoh dengan collider, dipindah ke dapur saat pulang
        [SerializeField] GameObject laras, lingkaranGaram, kendiDiAnglo, apiInduk, sosokJendela;
        [SerializeField] SosokSantet sosok;                    // Sosok Malam 2 (jalur Fitnah)
        [SerializeField] PemutarTimeline timelineRitual;
        [SerializeField] DataDialog dialogLingkaranGaram, dialogRitualTerakhir;
        [SerializeField] Suara suaraPintuDepan, suaraApi, suaraStinger, suaraDetak, musikTegang;

        bool berjalan, sibuk, patroliBerjalan, detakBerbunyi, obatTercatat;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            pintuDepan.Ditekan += SaatPintuDepan;
            buSumi.Ditekan += SaatBuSumi;
            timelineRitual.Sinyal += SaatSinyal;
            sosok.Menangkap += SaatTertangkap;
        }

        void OnDisable()
        {
            pintuDepan.Ditekan -= SaatPintuDepan;
            buSumi.Ditekan -= SaatBuSumi;
            timelineRitual.Sinyal -= SaatSinyal;
            sosok.Menangkap -= SaatTertangkap;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurMalam7 butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            if (s.MalamKe != 7 || s.Ending != Ending.Belum) return;
            berjalan = true;
            obatTercatat = s.ObatSudahDiberikan;
            kartu.HitamSeketika();
            sosok.gameObject.SetActive(false);
            sosokJendela.SetActive(false);
            apiInduk.SetActive(false);
            kendiDiAnglo.SetActive(false);
            // Bu Sumi bergabung hanya jika sidang berhasil (jalur utama).
            buSumi.gameObject.SetActive(!s.JalurFitnah);
            laras.SetActive(!s.JalurFitnah);
            lingkaranGaram.SetActive(false);
            if (s.JalurFitnah)
            {
                pintuDepan.UbahPrompt("Pintu depan");
                StartCoroutine(Fitnah());
            }
            else if (s.LolosKejaranWiryo)
            {
                pintuDepan.UbahPrompt("Pintu depan");
                StartCoroutine(Pulang());
            }
            else
            {
                pintuDepan.UbahPrompt("Berangkat ke Watuireng");
                StartCoroutine(Pembuka());
            }
        }

        // Jalur utama, awal malam: kartu bab lalu lingkaran garam dan kelor (Game Story Malam 7 butir 1-2).
        IEnumerator Pembuka()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                JudulBab, KalenderJawa.NamaMalam(7) + "\n\n" + RingkasanUtama + "\n\n" + TujuanUtama, 8f));
            adegan.Mulai(bidikRanjang);
            try
            {
                lingkaranGaram.SetActive(true);
                yield return dialog.MulaiDanTunggu(dialogLingkaranGaram);
            }
            finally { adegan.Selesai(); }
            hud.Tampilkan("Berangkat ke Watuireng lewat pintu depan.");
            sibuk = false;
        }

        // Jalur utama, pulang dari Watuireng dengan kendi induk (pilihan Lapor): Bu Sumi menunggu di dapur.
        IEnumerator Pulang()
        {
            sibuk = true;
            lingkaranGaram.SetActive(true);
            kendiDiAnglo.SetActive(true);
            buSumi.transform.SetPositionAndRotation(titikBuSumiDapur.position, titikBuSumiDapur.rotation);
            buSumi.UbahPrompt("Bakar kendi induk bersama Bu Sumi");
            pemain.Pindah(titikPulang);
            yield return kartu.StartCoroutine(kartu.DariHitam());
            hud.Tampilkan("Bu Sumi menunggu di dapur. Kendi induk harus dibakar sebelum tengah malam.");
            sibuk = false;
        }

        void SaatBuSumi()
        {
            if (!berjalan || sibuk) return;
            if (State.LolosKejaranWiryo) StartCoroutine(RitualTerakhir());
            else hud.Tampilkan("Bu Sumi menyuruhku segera berangkat ke Watuireng. Waktunya tidak banyak.");
        }

        // Game Story Malam 7 butir 9: Ritual Penangkal terakhir bersama Bu Sumi. Checkpoint dulu, lalu cutscene.
        IEnumerator RitualTerakhir()
        {
            sibuk = true;
            var gm = GameManager.Instance;
            var s = gm.State;
            gm.SimpanCheckpoint();
            // Catatan 0B: Bu Sumi melengkapi pengetahuan dan bahan ritual jika belum.
            if (!s.RitualDiketahui || !RitualPenangkal.BahanLengkap(s)) RitualPenangkal.Pelajari(s, dariBuSumi: true);
            adegan.Mulai(bidikAnglo);
            yield return dialog.MulaiDanTunggu(dialogRitualTerakhir);
            yield return timelineRitual.PutarDanTunggu();
            sosokJendela.SetActive(false);
            if (!s.MediaDitemukan[Kode.M7]) MediaSantet.Temukan(s, Kode.M7);   // jaga-jaga: kendi dicatat di Watuireng
            if (MediaSantet.BisaDinetralkan(s, Kode.M7)) MediaSantet.Netralkan(s, Kode.M7);   // Ending Sejati: GameManager memuat epilog
            if (s.Ending != Ending.Belum) yield break;   // jeda dilepas KameraAdegan.OnDisable saat scene dibongkar
            Debug.LogError("AlurMalam7: ritual terakhir selesai tanpa ending.", this);
            adegan.Selesai();
            sibuk = false;
        }

        void SaatSinyal(string nama)
        {
            switch (nama)
            {
                case SinyalApi: apiInduk.SetActive(true); suaraApi.Putar(); break;
                case SinyalJerit: suaraStinger.Putar(); sosokJendela.SetActive(true); break;
                case SinyalLenyap: sosokJendela.SetActive(false); break;
                default: Debug.LogWarning($"AlurMalam7: sinyal tidak dikenal '{nama}'.", this); break;
            }
        }

        // Jalur Fitnah (desain Fase 7): Sosok Malam 2 berpatroli, Raka bersembunyi dan menjaga Ibu sampai 00.00.
        // shortcut: kartu panjang tampil lagi setelah tertangkap (checkpoint awal malam); ganti kartu pendek jika tim terganggu.
        IEnumerator Fitnah()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                JudulBab, KalenderJawa.NamaMalam(7) + "\n\n" + RingkasanFitnah + "\n\n" + TujuanFitnah, 8f));
            patroliBerjalan = true;
            sosok.gameObject.SetActive(true);
            musikTegang.Putar();
            hud.Tampilkan("Gemerincing paku lagi. Dia ada di dalam rumah.");
            hud.Tampilkan("Jangan lari [Shift]. Sembunyi di lemari atau kolong ranjang [E], tahan napas [Spasi].");
            sibuk = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (!berjalan || gm == null || gm.Dijeda) return;
            var s = gm.State;
            if (s.Ending != Ending.Belum) return;
            if (patroliBerjalan) AturDetak();
            // ObatMalam7.Cek (di JamGim) memberi obat otomatis pukul 21.00 di jalur utama.
            if (!obatTercatat && s.ObatSudahDiberikan)
            {
                obatTercatat = true;
                if (!s.JalurFitnah) hud.Tampilkan("Laras memberi Ibu obat pukul 21.00.");
            }
        }

        void AturDetak()
        {
            if (pemain.Bersembunyi == detakBerbunyi) return;
            detakBerbunyi = pemain.Bersembunyi;
            if (detakBerbunyi) suaraDetak.Putar();
            else suaraDetak.Hentikan();
        }

        void SaatTertangkap()
        {
            if (!patroliBerjalan) return;
            patroliBerjalan = false;
            StartCoroutine(Tertangkap());
        }

        // Pola AlurMalam2.Tertangkap: muat checkpoint awal Malam 7, Kondisi Ibu -5 (minimal 1 di jalur Fitnah).
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
            gm.MuatScene(gm.SceneUntukState);
        }

        void SaatPintuDepan()
        {
            if (!berjalan || sibuk) return;
            var s = State;
            if (s.JalurFitnah) hud.Tampilkan("Aku tidak boleh meninggalkan Ibu malam ini.");
            else if (s.LolosKejaranWiryo) hud.Tampilkan("Bu Sumi menunggu di dapur. Kendi induk harus dibakar sekarang.");
            else StartCoroutine(KeWatuireng());
        }

        IEnumerator KeWatuireng()
        {
            sibuk = true;
            adegan.Mulai();
            suaraPintuDepan.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            GameManager.Instance.MuatScene(PetaScene.Watuireng);
        }
    }
}
