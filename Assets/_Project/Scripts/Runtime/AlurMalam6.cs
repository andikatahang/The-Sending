using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Malam 6 di rumah keluarga: Ibu kerasukan (K39-A), persiapan bukti, lalu pukul 21.00 Laras
    // memanggil Raka ke sidang di desa (checkpoint K43-A). Obat dan kursi memakai JadwalObat/PemberiObat/KursiTunggu.
    public class AlurMalam6 : MonoBehaviour
    {
        public const string SinyalLangkahLaras = "LangkahLaras";

        // Game Story, Malam 6, "Ringkasan siang" (dikutip persis).
        const string RingkasanMalam6 =
            "Warga berkumpul di balai dusun. Nama Bu Sumi disebut-sebut. Raka tidak diajak bicara.";
        const string TujuanBab = "Tujuan: Mencegah warga main hakim sendiri";
        const float LamaKerasukan = 3f, JedaKedip = 0.12f;

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] Transform titikMulai, titikPulang, bidikRanjang, bidikPintuDepan;
        [SerializeField] Transform ibu;
        [SerializeField] Light[] lampuKamar;
        [SerializeField] InteraktifAlur pintuDepan;
        [SerializeField] PakaiAlat kainKafan;
        [SerializeField] PemutarTimeline timelineLarasMasuk;
        [SerializeField] GameObject larasMasuk;
        [SerializeField] DataDialog dialogKerasukan, dialogLarasMasuk, dialogLarasMasukTanpaObat;
        [SerializeField] Suara suaraPintuDepan, suaraGeram, suaraLangkahLari;

        bool berjalan, sibuk, sidangDimulai;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            pintuDepan.Ditekan += SaatPintuDepan;
            kainKafan.Dipakai += SaatKainKafan;
            timelineLarasMasuk.Sinyal += SaatSinyal;
        }

        void OnDisable()
        {
            pintuDepan.Ditekan -= SaatPintuDepan;
            kainKafan.Dipakai -= SaatKainKafan;
            timelineLarasMasuk.Sinyal -= SaatSinyal;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurMalam6 butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            if (s.MalamKe >= 7)
            {
                hud.Tampilkan("Malam 7 belum dibuat (Fase 7).");
                return;
            }
            if (s.MalamKe != 6) return;
            berjalan = true;
            kartu.HitamSeketika();
            larasMasuk.SetActive(false);
            pintuDepan.UbahPrompt("Keluar ke desa");
            if (SidangWarga.SaatnyaSidang(s)) StartCoroutine(KeSidang(false));   // lanjut dari checkpoint 21.00
            else if (string.IsNullOrEmpty(s.SceneAsal)) StartCoroutine(Pembuka());
            else StartCoroutine(Pulang());
        }

        IEnumerator Pembuka()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Malam 6: Obor", KalenderJawa.NamaMalam(6) + "\n\n" + RingkasanMalam6 + "\n\n" + TujuanBab, 8f));
            yield return Kerasukan();
            hud.Tampilkan("Ibu sudah tenang lagi. Pukul 21.00 warga berkumpul. Aku harus menyiapkan bukti (Jurnal [Tab]).");
            hud.Tampilkan("Rumah Pakde masih bisa kudatangi lewat pintu depan.");
            sibuk = false;
        }

        // K39-A: Ibu kerasukan, ranjang berguncang dan lampu kamar berkedip.
        IEnumerator Kerasukan()
        {
            adegan.Mulai(bidikRanjang);
            try
            {
                suaraGeram.Putar();
                StartCoroutine(KedipLampu());
                yield return StartCoroutine(Guncang.Posisi(ibu, LamaKerasukan, 0.04f));
                yield return dialog.MulaiDanTunggu(dialogKerasukan);
            }
            finally { adegan.Selesai(); }
        }

        IEnumerator KedipLampu()
        {
            bool nyala = true;
            for (float t = 0f, kedip = 0f; t < LamaKerasukan; t += Time.unscaledDeltaTime, kedip += Time.unscaledDeltaTime)
            {
                if (kedip >= JedaKedip)
                {
                    kedip = 0f;
                    nyala = !nyala;
                    foreach (var l in lampuKamar) l.enabled = nyala;
                }
                yield return null;
            }
            foreach (var l in lampuKamar) l.enabled = true;
        }

        // Kembali dari desa, "Lanjutkan", atau muat ulang.
        IEnumerator Pulang()
        {
            sibuk = true;
            pemain.Pindah(titikPulang);
            yield return kartu.StartCoroutine(kartu.DariHitam());
            sibuk = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (!berjalan || gm == null || gm.Dijeda || sibuk) return;
            if (!sidangDimulai && SidangWarga.SaatnyaSidang(gm.State)) StartCoroutine(KeSidang(true));
        }

        // M6 dicatat efek dialog kainKafan (media:M6).
        void SaatKainKafan()
        {
            if (!berjalan) return;
            hud.Tampilkan("Kain kafan berajah. Netralkan di anglo dapur.");
        }

        void SaatSinyal(string nama)
        {
            if (nama == SinyalLangkahLaras) suaraLangkahLari.Putar();
        }

        void SaatPintuDepan()
        {
            if (!berjalan || sibuk) return;
            StartCoroutine(KeDesa());
        }

        IEnumerator KeDesa()
        {
            sibuk = true;
            adegan.Mulai();
            suaraPintuDepan.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            PetaScene.CatatAsal(State, PetaScene.RumahKeluarga);
            GameManager.Instance.MuatScene(PetaScene.Desa);
        }

        IEnumerator KeSidang(bool adeganLaras)
        {
            sidangDimulai = sibuk = true;
            var gm = GameManager.Instance;
            var s = gm.State;
            adegan.Mulai(bidikPintuDepan);
            if (adeganLaras)
            {
                larasMasuk.SetActive(true);
                yield return timelineLarasMasuk.PutarDanTunggu();
                yield return dialog.MulaiDanTunggu(s.ObatSudahDiberikan ? dialogLarasMasuk : dialogLarasMasukTanpaObat);
            }
            yield return kartu.StartCoroutine(kartu.KeHitam());
            PetaScene.CatatAsal(s, PetaScene.RumahKeluarga);
            gm.SimpanCheckpoint();   // K43-A
            gm.MuatScene(PetaScene.Desa);
        }
    }
}
