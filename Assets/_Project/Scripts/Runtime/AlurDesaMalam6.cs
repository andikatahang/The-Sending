using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Sidang Warga di Scene_Desa pada Malam 6 (Game Story, Malam 6): warga berobor di depan rumah Bu Sumi,
    // tiga tuduhan dijawab dengan Papan Bukti, lalu cabang berhasil (obor turun) atau gagal (rumah dibakar).
    // Aturan di JalannyaSidang (Core); sutradara hanya mengurutkan. Gerbang desa Malam 6 diurus AlurDesaMalam5.
    public class AlurDesaMalam6 : MonoBehaviour
    {
        public const string SinyalObor = "Obor", SinyalApi = "Api", SinyalTurunkanObor = "TurunkanObor";

        // Game Story, Malam 6 (dikutip persis).
        static readonly string[] Tuduhan =
        {
            "\"Rambut di sumur itu rambut Mbok Sumi!\"",
            "\"Dia dendam sama keluargamu sejak suaminya mati!\"",
            "\"Siapa lagi yang ngerti ilmu begituan di desa ini?\"",
        };

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] PanelSidang panel;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] DataJurnal jurnal;
        [SerializeField] GameObject kerumunan, buSumi;
        [SerializeField] Light[] lampuObor;
        [SerializeField] PuingRumahSumi rumahSumi;
        [SerializeField] GameObject api;
        [SerializeField] Transform titikSidang, bidikKerumunan, bidikBuSumi, bidikRumah;
        [SerializeField] PemutarTimeline timelineDatang, timelineBerhasil, timelineGagal;
        [SerializeField] DataDialog dialogPembuka, dialogBerhasil, dialogGagal, dialogDiam, dialogMenuduh;
        [SerializeField] DataDialog[] dialogTuduhan = new DataDialog[3], dialogBenar = new DataDialog[3], dialogSalah = new DataDialog[3];
        [SerializeField] Suara suaraKerumunan, suaraTeriak, suaraApi, musik;

        bool berjalan, mulai;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            timelineDatang.Sinyal += SaatSinyal;
            timelineBerhasil.Sinyal += SaatSinyal;
            timelineGagal.Sinyal += SaatSinyal;
        }

        void OnDisable()
        {
            timelineDatang.Sinyal -= SaatSinyal;
            timelineBerhasil.Sinyal -= SaatSinyal;
            timelineGagal.Sinyal -= SaatSinyal;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            kerumunan.SetActive(false);
            api.SetActive(false);
            if (gm == null || gm.State.MalamKe != 6) return;
            berjalan = true;
            if (SidangWarga.SaatnyaSidang(gm.State)) StartCoroutine(Sidang(true));   // datang dari scene lain (layar hitam)
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (berjalan && !mulai && gm != null && !gm.Dijeda && SidangWarga.SaatnyaSidang(gm.State))
                StartCoroutine(Sidang(false));   // Raka sudah di desa saat 21.00
        }

        void SaatSinyal(string nama)
        {
            if (nama == SinyalObor) suaraTeriak.Putar();
            else if (nama == SinyalApi)
            {
                api.SetActive(true);
                suaraApi.Putar();
            }
            else if (nama == SinyalTurunkanObor)
            {
                foreach (var l in lampuObor) l.enabled = false;
                suaraKerumunan.Hentikan();
            }
        }

        IEnumerator Sidang(bool dariHitam)
        {
            mulai = true;
            var s = State;
            adegan.Mulai(bidikKerumunan);
            if (!dariHitam) yield return kartu.StartCoroutine(kartu.KeHitam());
            pemain.Pindah(titikSidang);
            kerumunan.SetActive(true);
            musik.Putar();
            suaraKerumunan.Putar();
            yield return kartu.StartCoroutine(kartu.DariHitam());
            yield return timelineDatang.PutarDanTunggu();
            yield return dialog.MulaiDanTunggu(dialogPembuka);
            adegan.Bidik(bidikBuSumi);

            var j = new JalannyaSidang(s);
            for (int t = 0; t < SidangWarga.JumlahTuduhan; t++)
            {
                yield return dialog.MulaiDanTunggu(dialogTuduhan[t]);
                yield return panel.Tanya(Tuduhan[t], JalannyaSidang.PilihanJawaban(s), s, jurnal);
                if (panel.Jawaban == JawabanSidang.Diam)
                {
                    j.Diam();
                    yield return dialog.MulaiDanTunggu(dialogDiam);
                    break;
                }
                if (panel.Jawaban == JawabanSidang.Menuduh)
                {
                    j.IkutMenuduh();
                    yield return dialog.MulaiDanTunggu(dialogMenuduh);
                    break;
                }
                bool benar = j.Jawab(panel.Item);
                yield return dialog.MulaiDanTunggu(benar ? dialogBenar[t] : dialogSalah[t]);
            }

            if (s.HasilSidang == HasilSidang.Berhasil) yield return Berhasil();
            else yield return Gagal();
            yield return GantiMalam(s);
        }

        IEnumerator Berhasil()
        {
            yield return timelineBerhasil.PutarDanTunggu();
            yield return dialog.MulaiDanTunggu(dialogBerhasil);
            yield return kartu.StartCoroutine(kartu.Tampilkan("",
                "Pak Joko akan melapor ke Bhabinkamtibmas besok pagi. Bu Sumi ikut ke rumah Raka.", 5f));
        }

        IEnumerator Gagal()
        {
            adegan.Bidik(bidikRumah);
            yield return timelineGagal.PutarDanTunggu();
            yield return dialog.MulaiDanTunggu(dialogGagal);
            yield return kartu.StartCoroutine(kartu.KeHitam());
            rumahSumi.Terapkan(true);
            buSumi.SetActive(false);
            yield return kartu.StartCoroutine(kartu.Tampilkan("",
                "Rumah Bu Sumi terbakar habis. Bu Sumi selamat dengan luka bakar. Kerabatnya dari desa lain membawanya pergi malam itu.", 6f));
        }

        // K44-A: sampai Fase 7, akhir Malam 6 berhenti di kartu Malam 7 lalu menu utama.
        IEnumerator GantiMalam(GameState s)
        {
            var gm = GameManager.Instance;
            bool dimuat = false;
            try
            {
                yield return kartu.StartCoroutine(kartu.KeHitam());
                PergantianMalam.Lanjut(s);        // Malam 7, Kondisi Ibu -12
                if (s.Ending != Ending.Belum)
                {
                    Debug.LogWarning("Ending tercapai di Malam 6; layar ending dibuat di Fase 7.", this);
                    gm.MuatScene(GameManager.SceneMenuUtama);
                    dimuat = true;
                    yield break;
                }
                gm.SimpanCheckpoint();            // checkpoint awal Malam 7
                yield return kartu.StartCoroutine(kartu.Tampilkan("Malam 7: Malam Selasa Kliwon",
                    KalenderJawa.NamaMalam(7) + "\n\n(Bersambung. Malam 7 dibuat di Fase 7.)", 8f));
                yield return kartu.StartCoroutine(kartu.KeHitam());
                gm.MuatScene(GameManager.SceneMenuUtama);
                dimuat = true;
            }
            finally { if (!dimuat) adegan.Selesai(); }
        }
    }
}
