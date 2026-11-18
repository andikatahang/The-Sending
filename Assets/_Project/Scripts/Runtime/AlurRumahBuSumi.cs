using System;
using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara interior rumah Bu Sumi pada Malam 3 (Game Story Malam 3 butir 7-8, Kepercayaan 6.4, K17-B dan K23-A).
    // Bu Sumi dingin di depan pintu; pilihan dialog: minta maaf (+1), menuduh (-1, Raka diusir dan pintu ditutup sampai
    // malam berikutnya), atau menawarkan bantuan. Teplok di meja dan sumur di halaman belakang (+1 setelah keduanya, lapor
    // ke Bu Sumi). Kepercayaan minimal 1: cerita tiga tahun lalu dan Ritual Penangkal di bale-bale. Pintu keluar ke desa.
    // Tidak ada checkpoint di sini; kemajuan kunjungan disimpan di GameState lewat efek dialog.
    public class AlurRumahBuSumi : MonoBehaviour
    {
        const float LamaKerja = 2.5f;

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] DataDialog dialogPintu, dialogMenunggu, dialogTerimaKasih, dialogPamit, dialogCerita;
        [SerializeField] InteraktifAlur buSumi, teplok, sumur, pintuKeluar;
        [SerializeField] GameObject cahayaTeplok;
        [SerializeField] Transform titikMulai, titikBale, titikSumiBale, bidikBale;
        [SerializeField] Suara suaraBanting, suaraTimba, suaraPintu;

        bool berjalan, sibuk, teplokBeres, airBeres, ceritaSudah, peringatanPulang;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            buSumi.Ditekan += SaatBuSumi;
            teplok.Ditekan += SaatTeplok;
            sumur.Ditekan += SaatSumur;
            pintuKeluar.Ditekan += SaatKeluar;
        }

        void OnDisable()
        {
            buSumi.Ditekan -= SaatBuSumi;
            teplok.Ditekan -= SaatTeplok;
            sumur.Ditekan -= SaatSumur;
            pintuKeluar.Ditekan -= SaatKeluar;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurRumahBuSumi butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            kartu.HitamSeketika();
            pemain.Pindah(titikMulai);
            cahayaTeplok.SetActive(false);
            if (s.MalamKe != 3)
            {
                // Isi Malam 4 ke atas dibuat di fase berikutnya; pintu keluar tetap bisa dipakai.
                hud.Tampilkan("Rumah Bu Sumi pada malam ini belum dibuat.");
                kartu.StartCoroutine(kartu.DariHitam());
                return;
            }
            berjalan = true;
            // Kepercayaan >= 1 dan bahan lengkap berarti cerita sudah didengar (hanya terjadi lewat save uji F5).
            ceritaSudah = KepercayaanSumi.BisaAjarkanRitual(s) && RitualPenangkal.BahanLengkap(s);
            StartCoroutine(Datang());
        }

        IEnumerator Datang()
        {
            sibuk = true;
            yield return kartu.StartCoroutine(kartu.DariHitam());
            hud.Tampilkan("Bu Sumi membukakan pintu dan berdiri menghalangi. Dapurnya gelap; teploknya mati.");
            sibuk = false;
        }

        void SaatBuSumi()
        {
            if (!berjalan || sibuk) return;
            var s = State;
            DataDialog d;
            if (!TujuanMalam.Sudah(s, TujuanMalam.KunjungiSumi)) d = dialogPintu;
            else if (s.AksiSumiDipakai[(int)AksiSumi.BantuLampuDanAir]) d = dialogPamit;
            else if (teplokBeres && airBeres) d = dialogTerimaKasih;
            else d = dialogMenunggu;
            StartCoroutine(Bicara(d));
        }

        IEnumerator Bicara(DataDialog d)
        {
            sibuk = true;
            yield return dialog.MulaiDanTunggu(d);
            var s = State;
            if (!KepercayaanSumi.PintuTerbuka(s))
            {
                // Game Story 6.4: menuduh langsung = -1 dan pintu ditutup sampai malam berikutnya. Raka diusir ke jalan.
                yield return Keluar(suaraBanting);
                yield break;
            }
            if (KepercayaanSumi.BisaAjarkanRitual(s) && !ceritaSudah) yield return Cerita();
            sibuk = false;
        }

        // Game Story Malam 3 butir 8: Kepercayaan minimal 1, Bu Sumi bercerita dan mengajarkan Ritual Penangkal.
        IEnumerator Cerita()
        {
            ceritaSudah = true;
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.KeHitam());
                pemain.Pindah(titikBale);
                buSumi.transform.SetPositionAndRotation(titikSumiBale.position, titikSumiBale.rotation);
                adegan.Bidik(bidikBale);
                yield return kartu.StartCoroutine(kartu.DariHitam());
                yield return dialog.MulaiDanTunggu(dialogCerita);   // efek ritual-dari-sumi: pengetahuan + satu ikat bahan
            }
            finally { adegan.Selesai(); }
            hud.Tampilkan("Bu Sumi memberi seikat garam, daun kelor, dan bilah bambu kuning.");
            hud.Tampilkan("Media santet dinetralkan di anglo dapur rumah.");
        }

        void SaatTeplok() => Bantu(teplokBeres, "Raka membersihkan sumbu dan menuang minyak. Teplok menyala lagi.", () =>
        {
            teplokBeres = true;
            cahayaTeplok.SetActive(true);
            teplok.UbahPrompt("Teplok sudah menyala");
        });

        void SaatSumur() => Bantu(airBeres, "Raka menimba air dan mengisi gentong Bu Sumi sampai penuh.", () =>
        {
            airBeres = true;
            suaraTimba.Putar();
            sumur.UbahPrompt("Gentong sudah penuh");
        });

        // K23-A: dua pekerjaan sederhana; Kepercayaan +1 diberikan dialog terima kasih setelah keduanya selesai.
        void Bantu(bool sudah, string teks, Action selesai)
        {
            if (!berjalan || sibuk || sudah) return;
            var s = State;
            if (!TujuanMalam.Sudah(s, TujuanMalam.KunjungiSumi))
                hud.Tampilkan("Raka belum bicara dengan Bu Sumi.");
            else if (!s.AksiSumiDipakai[(int)AksiSumi.BantuLampuDanAir])
                StartCoroutine(Kerja(teks, selesai));
        }

        IEnumerator Kerja(string teks, Action selesai)
        {
            sibuk = true;
            yield return kartu.StartCoroutine(kartu.Tampilkan("", teks, LamaKerja));
            selesai();
            if (teplokBeres && airBeres) hud.Tampilkan("Beri tahu Bu Sumi.");
            sibuk = false;
        }

        // R4: kunjungan sekali per malam. Keluar sebelum selesai membantu diberi peringatan satu kali.
        void SaatKeluar()
        {
            if (sibuk) return;
            if (berjalan)
            {
                var s = State;
                if (!TujuanMalam.Sudah(s, TujuanMalam.KunjungiSumi))
                {
                    hud.Tampilkan("Raka belum bicara dengan Bu Sumi.");
                    return;
                }
                if (!s.AksiSumiDipakai[(int)AksiSumi.BantuLampuDanAir] && !peringatanPulang)
                {
                    peringatanPulang = true;
                    hud.Tampilkan("Bu Sumi masih butuh bantuan (teplok, air). Raka tidak kembali lagi malam ini. Tekan E sekali lagi untuk keluar.");
                    return;
                }
            }
            StartCoroutine(Keluar(suaraPintu));
        }

        IEnumerator Keluar(Suara suara)
        {
            sibuk = true;
            adegan.Mulai();
            suara.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            GameManager.Instance.MuatScene(PetaScene.Desa);   // AlurDesaMalam3 menaruh Raka di depan rumah Bu Sumi
        }
    }
}
