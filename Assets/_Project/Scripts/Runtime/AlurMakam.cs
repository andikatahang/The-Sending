using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Scene_Makam (Malam 5): tiga sampel tanah dibandingkan dengan tanah kendi; setelah ketiganya tidak cocok,
    // Pak Lasno datang dan memberi B3 (tanah makam Watuireng). Gerbang keluar kembali ke desa.
    public class AlurMakam : MonoBehaviour
    {
        static readonly string[] Kendi = { "Merah bata", "Lengket, liat, menggumpal", "Anyir, seperti karat" };
        static readonly string[][] Sampel =
        {
            new[] { "Hitam", "Berpasir, lepas di jari", "Tanah basah biasa" },
            new[] { "Hitam keabuan", "Berpasir kasar", "Bau akar kamboja" },
            new[] { "Hitam pekat", "Berpasir halus", "Bau daun busuk" },
        };

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] PanelTanah panel;
        [SerializeField] PakaiAlat[] titikSampel = new PakaiAlat[3];
        [SerializeField] InteraktifAlur nisanHadi, gerbangKeluar;
        [SerializeField] GameObject pakLasno;
        [SerializeField] Transform titikMulai, titikLasno, bidikLasno;
        [SerializeField] DataDialog dialogNisan, dialogLasno;
        [SerializeField] Suara suasana, suaraGali, suaraPintu;

        readonly PerbandinganTanah perbandingan = new PerbandinganTanah();
        readonly System.Action[] saatSampel = new System.Action[3];
        bool berjalan, sibuk, nisanSudah;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            for (int i = 0; i < titikSampel.Length; i++)
            {
                int n = i;
                saatSampel[i] = () => SaatSampel(n);
                titikSampel[i].Dipakai += saatSampel[i];
            }
            nisanHadi.Ditekan += SaatNisan;
            gerbangKeluar.Ditekan += SaatKeluar;
        }

        void OnDisable()
        {
            for (int i = 0; i < titikSampel.Length; i++) titikSampel[i].Dipakai -= saatSampel[i];
            nisanHadi.Ditekan -= SaatNisan;
            gerbangKeluar.Ditekan -= SaatKeluar;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurMakam butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            kartu.HitamSeketika();
            pemain.Pindah(titikMulai);
            suasana.Putar();
            bool sudahBanding = TujuanMalam.Sudah(s, TujuanMalam.BandingkanTanah);
            pakLasno.SetActive(s.MalamKe == 5 && sudahBanding);
            if (s.MalamKe != 5)
            {
                foreach (var t in titikSampel) t.gameObject.SetActive(false);
                nisanHadi.gameObject.SetActive(false);
                hud.Tampilkan("Makam sepi.");
                kartu.StartCoroutine(kartu.DariHitam());
                return;
            }
            berjalan = true;
            if (sudahBanding) foreach (var t in titikSampel) t.gameObject.SetActive(false);
            StartCoroutine(Datang(sudahBanding));
        }

        IEnumerator Datang(bool sudahBanding)
        {
            sibuk = true;
            yield return kartu.StartCoroutine(kartu.DariHitam());
            if (!sudahBanding) hud.Tampilkan("Makam Bapak di bawah kamboja besar. Ambil sampel tanah di tiga titik [E].");
            sibuk = false;
        }

        void SaatSampel(int i)
        {
            if (!berjalan) return;
            suaraGali.Putar();
            panel.Buka(i, Kendi, Sampel[i], () => SetelahBanding(i));
        }

        // Bisa terpanggil dari OnDisable PanelTanah saat scene dibongkar: abaikan jika sutradara sudah tidak aktif.
        void SetelahBanding(int i)
        {
            if (!berjalan || this == null || !isActiveAndEnabled) return;
            perbandingan.Bandingkan(i);
            if (perbandingan.SemuaSudahDibandingkan)
            {
                if (!TujuanMalam.Sudah(State, TujuanMalam.BandingkanTanah)) StartCoroutine(PakLasno());
                return;
            }
            int sisa = 0;
            for (int t = 0; t < PerbandinganTanah.JumlahTitik; t++) if (!titikSampel[t].Sudah) sisa++;
            hud.Tampilkan($"Tidak cocok. Masih ada {sisa} titik lagi.");
        }

        IEnumerator PakLasno()
        {
            sibuk = true;
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.KeHitam());
                pakLasno.transform.SetPositionAndRotation(titikLasno.position, titikLasno.rotation);
                pakLasno.SetActive(true);
                adegan.Bidik(bidikLasno);
                yield return kartu.StartCoroutine(kartu.DariHitam());
                yield return dialog.MulaiDanTunggu(dialogLasno);   // efek bukti:B3; tujuan:BandingkanTanah
            }
            finally { adegan.Selesai(); }
            hud.Tampilkan("Tanah makam Watuireng. Kembali ke rumah dan bicara dengan Laras.");
            sibuk = false;
        }

        void SaatNisan()
        {
            if (!berjalan || sibuk || nisanSudah) return;
            nisanSudah = true;
            StartCoroutine(Nisan());
        }

        IEnumerator Nisan()
        {
            sibuk = true;
            yield return dialog.MulaiDanTunggu(dialogNisan);
            sibuk = false;
        }

        void SaatKeluar()
        {
            if (sibuk) return;
            StartCoroutine(Keluar());
        }

        IEnumerator Keluar()
        {
            sibuk = true;
            adegan.Mulai();
            suaraPintu.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            PetaScene.CatatAsal(State, PetaScene.Makam);
            GameManager.Instance.MuatScene(PetaScene.Desa);
        }
    }
}
