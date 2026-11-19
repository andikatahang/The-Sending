using System.Collections;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Sutradara Malam 4 di rumah keluarga (Game Story, Malam 4): pinjam tali dan pengait di pos ronda (pintu depan ke
    // Scene_Desa, AlurDesaMalam4), tarik bungkusan dari sumur (KerekanSumur), Sosok keluar dari arah sumur dan mengejar
    // (K29-A: lolos = masuk pintu belakang dapur), dua warga menuduh, lalu obat dan pergantian malam. Plafon (NaikPlafon),
    // ritual, dan obat berjalan sendiri.
    public class AlurMalam4 : MonoBehaviour
    {
        // Game Story, Malam 4 dan Malam 5, "Ringkasan siang" (dikutip persis).
        const string RingkasanMalam4 =
            "Air sumur berbau busuk. Kabar bahwa keluarga Raka kena santet sudah menyebar ke seluruh desa.";
        const string TujuanBab = "Tujuan: Mengangkat apa yang ada di dasar sumur";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] DataDialog dialogWargaMenuduh;   // efek katanya:K3 (Task 7)
        [SerializeField] Transform titikMulai, titikPulang, titikMunculSosok, titikWarga;
        [SerializeField] GameObject warga;                // dua warga di pintu belakang; mati di luar adegan
        [SerializeField] KerekanSumur kerekan;
        [SerializeField] Text teksTempo;                  // bacaan tempo tarikan; boleh kosong
        [SerializeField] InteraktifAlur pintuDepan;
        [SerializeField] ZonaPemicu pintuBelakang;        // trigger di ambang pintu belakang dapur: lolos dari kejaran
        [SerializeField] SosokSantet sosok;
        [SerializeField] PemutarTimeline cutsceneKejar;   // boleh kosong: Sosok langsung muncul
        [SerializeField] Suara suaraPintuDepan, suaraStinger, suaraDetak, musikTegang;

        bool berjalan, sibuk, petunjukObat, gantiDimulai, kejarBerjalan, detakBerbunyi;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            kerekan.BungkusanNaik += SaatBungkusanNaik;
            pintuDepan.Ditekan += SaatPintuDepan;
            pintuBelakang.Masuk += SaatMasukPintuBelakang;
            sosok.Menangkap += SaatTertangkap;
        }

        void OnDisable()
        {
            kerekan.BungkusanNaik -= SaatBungkusanNaik;
            pintuDepan.Ditekan -= SaatPintuDepan;
            pintuBelakang.Masuk -= SaatMasukPintuBelakang;
            sosok.Menangkap -= SaatTertangkap;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurMalam4 butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            if (s.MalamKe >= 5)
            {
                hud.Tampilkan("Malam 5 belum dibuat (Fase 5).");
                return;
            }
            if (s.MalamKe != 4) return;
            berjalan = true;
            kartu.HitamSeketika();
            sosok.gameObject.SetActive(false);
            warga.SetActive(false);
            if (teksTempo != null) teksTempo.text = "";
            PerbaruiPromptPintu(s);
            // Checkpoint sebelum kejar (K29-A) menyimpan bungkusan sudah naik: kejaran diulang dari awal.
            if (TujuanMalam.Sudah(s, TujuanMalam.AmbilBungkusanSumur) && !TujuanMalam.Sudah(s, TujuanMalam.LolosKejaranSumur))
                StartCoroutine(Kejaran(dariCheckpoint: true));
            // shortcut: "sudah meminjam tali" dipakai sebagai tanda pulang dari desa (tanpa static); ke desa tanpa
            // meminjam lalu pulang menampilkan kartu pembuka lagi. Upgrade jika tim merasa mengganggu.
            else if (!Inventaris.Punya(s, Inventaris.TaliTambang)) StartCoroutine(Pembuka());
            else StartCoroutine(Pulang());
        }

        void PerbaruiPromptPintu(GameState s) =>
            pintuDepan.UbahPrompt(Inventaris.Punya(s, Inventaris.TaliTambang) ? "Keluar ke desa" : "Pergi ke pos ronda");

        IEnumerator Pembuka()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Malam 4: Rambut di Dasar Sumur", KalenderJawa.NamaMalam(4) + "\n\n" + RingkasanMalam4 + "\n\n" + TujuanBab, 8f));
            hud.Tampilkan("Ada sesuatu di dasar sumur belakang. Butuh tali dan pengait; para peronda di pos ronda punya.");
            sibuk = false;
        }

        // Kembali dari desa (K26-A, boleh bolak-balik), "Lanjutkan", atau muat ulang.
        IEnumerator Pulang()
        {
            sibuk = true;
            pemain.Pindah(titikPulang);
            yield return kartu.StartCoroutine(kartu.DariHitam());
            if (Inventaris.Punya(State, Inventaris.Pengait) && !TujuanMalam.Sudah(State, TujuanMalam.AmbilBungkusanSumur))
                hud.Tampilkan("Tali dan pengait sudah ada. Sumur di pekarangan belakang, lewat pintu dapur.");
            sibuk = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (!berjalan || gm == null || gm.Dijeda) return;
            if (teksTempo != null)
                teksTempo.text = kerekan.Menarik
                    ? $"Tarik: {Mathf.RoundToInt(kerekan.Progres * 100f)}%  Tempo: {kerekan.Kecepatan:0.0}" : "";
            var s = gm.State;
            if (sibuk || s.MalamKe != 4 || s.Ending != Ending.Belum) return;
            if (kejarBerjalan) AturDetak();
            if (kejarBerjalan || !TujuanMalam.MalamSelesai(s)) return;
            if (!JamGim.ObatSelesai(s))
            {
                if (!petunjukObat)
                {
                    petunjukObat = true;
                    hud.Tampilkan($"Obat Ibu pukul {JamGim.Format(JamGim.MulaiJendelaObat(4))}. Kursi di kamar Ibu bisa dipakai menunggu.");
                }
                return;
            }
            if (!gantiDimulai && PergantianMalam.BisaLanjut(s)) StartCoroutine(GantiMalam(s));
        }

        // Dipanggil KerekanSumur saat bungkusan sampai di atas pada tempo aman.
        void SaatBungkusanNaik()
        {
            if (!berjalan) return;
            var s = State;
            if (!s.MediaDitemukan[Kode.M3]) MediaSantet.Temukan(s, Kode.M3);   // M3 otomatis terfoto
            TujuanMalam.Selesaikan(s, TujuanMalam.AmbilBungkusanSumur);
            hud.Tampilkan("Bungkusan kain hitam berisi gumpalan rambut dan tanah. Media santet ketiga.");
            StartCoroutine(Kejaran(dariCheckpoint: false));
        }

        // K29-A dan desain 2.6: checkpoint dulu, cutscene pendek, lalu Sosok keluar dari arah sumur.
        IEnumerator Kejaran(bool dariCheckpoint)
        {
            sibuk = true;
            var gm = GameManager.Instance;
            if (dariCheckpoint)
            {
                pemain.Pindah(titikPulang);   // shortcut: muat ulang mulai di titik pulang, bukan di sumur
                yield return kartu.StartCoroutine(kartu.DariHitam());
            }
            else gm.SimpanCheckpoint();
            if (cutsceneKejar != null)
            {
                adegan.Mulai();
                try { yield return cutsceneKejar.PutarDanTunggu(); }
                finally { adegan.Selesai(); }
            }
            suaraStinger.Putar();
            sosok.transform.SetPositionAndRotation(titikMunculSosok.position, titikMunculSosok.rotation);
            sosok.gameObject.SetActive(true);   // Kejar dipicu PerilakuSantet saat Sosok melihat/mendengar Raka
            musikTegang.Putar();
            kejarBerjalan = true;
            hud.Tampilkan("Gemerincing paku dari arah sumur. Lari ke pintu belakang dapur!");
            sibuk = false;
        }

        void AturDetak()
        {
            if (pemain.Bersembunyi == detakBerbunyi) return;
            detakBerbunyi = pemain.Bersembunyi;
            if (detakBerbunyi) suaraDetak.Putar();
            else suaraDetak.Hentikan();
        }

        void SaatMasukPintuBelakang()
        {
            if (!berjalan || !kejarBerjalan) return;
            kejarBerjalan = false;
            sosok.gameObject.SetActive(false);
            musikTegang.Hentikan();
            suaraDetak.Hentikan();
            detakBerbunyi = false;
            TujuanMalam.Selesaikan(State, TujuanMalam.LolosKejaranSumur);
            StartCoroutine(WargaMenuduh());
        }

        // Game Story Malam 4: warga yang mendengar keributan melihat bungkusan dan menuduh Bu Sumi (K3).
        IEnumerator WargaMenuduh()
        {
            sibuk = true;
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.KeHitam());
                warga.SetActive(true);
                adegan.Bidik(titikWarga);
                yield return kartu.StartCoroutine(kartu.DariHitam());
                if (dialogWargaMenuduh != null) yield return dialog.MulaiDanTunggu(dialogWargaMenuduh);
                else Debug.LogError("AlurMalam4: dialogWargaMenuduh belum diisi.", this);
                yield return kartu.StartCoroutine(kartu.KeHitam());
                warga.SetActive(false);
            }
            finally { adegan.Selesai(); }
            yield return kartu.StartCoroutine(kartu.DariHitam());
            hud.Tampilkan("Bungkusan itu bisa dinetralkan di anglo dapur.");
            sibuk = false;
        }

        void SaatTertangkap()
        {
            if (!kejarBerjalan) return;
            kejarBerjalan = false;
            StartCoroutine(Tertangkap());
        }

        // Desain 4.2: muat checkpoint sebelum kejar, lalu Kondisi Ibu -5.
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

        // K26-A: pintu depan ke pos ronda kapan saja di Malam 4, kecuali saat dikejar.
        void SaatPintuDepan()
        {
            if (!berjalan || sibuk) return;
            if (kejarBerjalan) hud.Tampilkan("Gemerincing paku masih terdengar. Jangan keluar sekarang.");
            else StartCoroutine(KeDesa());
        }

        IEnumerator KeDesa()
        {
            sibuk = true;
            adegan.Mulai();
            suaraPintuDepan.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            GameManager.Instance.MuatScene(PetaScene.Desa);   // AlurDesaMalam4 menaruh Raka di depan rumah keluarga
        }

        bool KeMenuJikaEnding(GameManager gm)
        {
            if (gm.State.Ending == Ending.Belum) return false;
            Debug.LogWarning("Ending tercapai di Malam 4; layar ending dibuat di Fase 7.", this);
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
                PergantianMalam.Lanjut(s);        // Malam 5, Kondisi Ibu -12
                if (KeMenuJikaEnding(gm))
                {
                    dimuat = true;
                    yield break;
                }
                gm.SimpanCheckpoint();            // checkpoint awal Malam 5
                // Fase 5 mengganti bagian di bawah ini dengan gm.MuatScene(gm.SceneUntukState) untuk sutradara Malam 5.
                yield return kartu.StartCoroutine(kartu.Tampilkan(
                    "Malam 5: Tanah Merah", KalenderJawa.NamaMalam(5) + "\n\n(Bersambung. Malam 5 dibuat di Fase 5.)", 8f));
                yield return kartu.StartCoroutine(kartu.KeHitam());
                gm.MuatScene(GameManager.SceneMenuUtama);
                dimuat = true;
            }
            finally { if (!dimuat) adegan.Selesai(); }
        }
    }
}
