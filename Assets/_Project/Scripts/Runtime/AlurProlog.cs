using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Prolog di Scene_Desa (Game Story, Prolog butir 1-5): telepon Laras, turun dari mobil,
    // tutorial jalan/lari/senter, azan dan kabut turun, Bu Sumi di jendela, sampai pintu rumah.
    public class AlurProlog : MonoBehaviour
    {
        public const float KabutSebelumMagrib = 0.02f, DetikKabutTurun = 20f, DetikAzan = 5f, DetikPudarAzan = 0.5f;
        // Ditulis pengembang dari ringkasan GDD (Laras menelepon sambil menangis; Ibu dua minggu sakit, memuntahkan rambut dan paku).
        public const string TeksTelepon =
            "\"Mas... Mas Raka, pulang, Mas.\"\n\n" +
            "\"Ibu sakit. Sudah dua minggu. Tadi Ibu muntah lagi... ada rambutnya, ada pakunya, Mas.\"\n\n" +
            "\"Aku takut.\"";
        // Nama SignalAsset di Timeline Prolog (PembuatDesa memakai konstanta yang sama).
        public const string SinyalMotorPergi = "MotorPergi", SinyalTiraiDitutup = "TiraiDitutup";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] DataDialog dialogOjek;
        [SerializeField] Transform bidikGapura, bidikJendela, ojek;
        [SerializeField] PemutarTimeline timelineOjek, timelineBuSumi;
        [SerializeField] ZonaPemicu zonaSenter, zonaAzan, zonaBuSumi;
        [SerializeField] InteraktifAlur pintuRumah;
        [SerializeField] GameObject siluetBuSumi, tiraiJendela;
        [SerializeField] Light lampuJendela;
        [SerializeField] Suara suaraMotor, suaraAzan, suaraPintu;   // tanpa suara telepon: suara tokoh ditunda, telepon hanya teks

        float kabutAkhir;
        bool sudahSenter, sudahAzan, sudahBuSumi, sedangMasuk;

        void OnEnable()
        {
            zonaSenter.Masuk += SaatZonaSenter;
            zonaAzan.Masuk += SaatZonaAzan;
            zonaBuSumi.Masuk += SaatZonaBuSumi;
            pintuRumah.Ditekan += SaatPintu;
            timelineOjek.Sinyal += SaatSinyal;
            timelineBuSumi.Sinyal += SaatSinyal;
        }

        void OnDisable()
        {
            zonaSenter.Masuk -= SaatZonaSenter;
            zonaAzan.Masuk -= SaatZonaAzan;
            zonaBuSumi.Masuk -= SaatZonaBuSumi;
            pintuRumah.Ditekan -= SaatPintu;
            timelineOjek.Sinyal -= SaatSinyal;
            timelineBuSumi.Sinyal -= SaatSinyal;
        }

        void SaatSinyal(string nama)
        {
            switch (nama)
            {
                case SinyalMotorPergi: suaraMotor.Putar(); break;
                case SinyalTiraiDitutup: TutupTirai(); break;
                default: Debug.LogWarning($"AlurProlog: sinyal tidak dikenal '{nama}'.", this); break;
            }
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurProlog butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            if (s.MalamKe != 0 || TujuanMalam.Sudah(s, TujuanMalam.TibaDiRumah))
            {
                // Scene desa dibuka di luar Prolog (fase berikutnya): tidak ada adegan pembuka.
                ojek.gameObject.SetActive(false);
                enabled = false;
                return;
            }
            kabutAkhir = RenderSettings.fogDensity;
            RenderSettings.fogDensity = KabutSebelumMagrib;
            kartu.HitamSeketika();
            StartCoroutine(Pembuka());
        }

        IEnumerator Pembuka()
        {
            adegan.Mulai(bidikGapura);
            try
            {
                yield return kartu.StartCoroutine(kartu.Tampilkan("Telepon dari Laras", TeksTelepon, 8f));
                yield return dialog.MulaiDanTunggu(dialogOjek);
                yield return timelineOjek.PutarDanTunggu();   // ojek menjauh; sinyal MotorPergi memutar suara motor
                ojek.gameObject.SetActive(false);
            }
            finally { adegan.Selesai(); }
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Prolog: Pulang", "Selasa sore menjelang magrib\n\nTujuan: Sampai di rumah dan bertemu Ibu", 4f));
            hud.Tampilkan("W A S D untuk berjalan. Tahan Shift untuk berlari.");
        }

        void SaatZonaSenter()
        {
            if (sudahSenter) return;
            sudahSenter = true;
            hud.Tampilkan("Kebun bambu gelap. Tekan F untuk menyalakan senter HP.");
        }

        void SaatZonaAzan()
        {
            if (sudahAzan) return;
            sudahAzan = true;
            suaraAzan.Putar();
            hud.Tampilkan("Azan magrib terdengar dari kejauhan. Hari sudah magrib.");
            StartCoroutine(AzanSingkat());
            StartCoroutine(KabutTurun());
        }

        // Hanya awal azan yang terdengar (keputusan tim), lalu memudar.
        IEnumerator AzanSingkat()
        {
            yield return new WaitForSeconds(DetikAzan - DetikPudarAzan);
            yield return suaraAzan.Pudar(DetikPudarAzan);
        }

        IEnumerator KabutTurun()
        {
            for (float t = 0f; t < DetikKabutTurun; t += Time.deltaTime)
            {
                RenderSettings.fogDensity = Mathf.Lerp(KabutSebelumMagrib, kabutAkhir, t / DetikKabutTurun);
                yield return null;
            }
            RenderSettings.fogDensity = kabutAkhir;
        }

        void SaatZonaBuSumi()
        {
            if (sudahBuSumi) return;
            sudahBuSumi = true;
            StartCoroutine(BuSumiDiJendela());
        }

        // Game Story: sosok perempuan tua di balik jendela menatap Raka, lalu menutup tirai.
        IEnumerator BuSumiDiJendela()
        {
            adegan.Mulai(bidikJendela);
            try
            {
                yield return timelineBuSumi.PutarDanTunggu();   // kamera mendekat; sinyal TiraiDitutup pada 2,5 detik
                TutupTirai();                                   // cadangan jika sinyal tidak terpicu
            }
            finally { adegan.Selesai(); }
        }

        void TutupTirai()
        {
            if (!siluetBuSumi.activeSelf) return;   // sekali saja: jangan meredupkan lampu dua kali
            siluetBuSumi.SetActive(false);
            tiraiJendela.SetActive(true);
            lampuJendela.intensity *= 0.3f;
        }

        void SaatPintu()
        {
            if (!sedangMasuk) StartCoroutine(MasukRumah());
        }

        IEnumerator MasukRumah()
        {
            sedangMasuk = true;
            var gm = GameManager.Instance;
            bool dimuat = false;
            adegan.Mulai();
            try
            {
                suaraPintu.Putar();
                yield return kartu.StartCoroutine(kartu.KeHitam());
                TujuanMalam.Selesaikan(gm.State, TujuanMalam.TibaDiRumah);
                gm.SimpanCheckpoint();              // "Lanjutkan" sesudah ini dimuat di rumah (PetaScene)
                gm.MuatScene(gm.SceneUntukState);   // jeda adegan dilepas KameraAdegan.OnDisable saat scene dibongkar
                dimuat = true;
            }
            finally { if (!dimuat) adegan.Selesai(); }   // gagal sebelum pindah scene: jangan biarkan gim terjeda
        }
    }
}
