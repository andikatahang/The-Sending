using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Malam 5 di rumah keluarga (Game Story, Malam 5): gali kendi di pohon asem (M5), kain kafan (M6),
    // bicara dengan Laras (teh jika ia mengunci diri), pintu depan ke desa (makam, rumah Pakde), lalu obat dan
    // pergantian malam. Media dan tujuan dicatat lewat efek dialog; sutradara hanya mengurutkan.
    public class AlurMalam5 : MonoBehaviour
    {
        // Game Story, Malam 5, "Ringkasan siang" (dikutip persis).
        const string RingkasanMalam5 =
            "Ibu makin lemah dan hanya bisa minum sedikit. Laras tidak berani keluar kamar.";
        const string TujuanBab = "Tujuan: Mencari asal media santet dan mengumpulkan kesaksian";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] Transform titikMulai, titikPulang;
        [SerializeField] InteraktifAlur pintuDepan, pintuLaras;
        [SerializeField] PakaiAlat galiKendi, kainKafan;
        [SerializeField] DataDialog dialogLaras, dialogLarasTerkunci, dialogLarasTeh;
        [SerializeField] Suara suaraPintuDepan, suaraGali, suaraGelas;

        bool berjalan, sibuk, petunjukObat, gantiDimulai;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            pintuDepan.Ditekan += SaatPintuDepan;
            pintuLaras.Ditekan += SaatPintuLaras;
            galiKendi.Dipakai += SaatGaliKendi;
            kainKafan.Dipakai += SaatKainKafan;
        }

        void OnDisable()
        {
            pintuDepan.Ditekan -= SaatPintuDepan;
            pintuLaras.Ditekan -= SaatPintuLaras;
            galiKendi.Dipakai -= SaatGaliKendi;
            kainKafan.Dipakai -= SaatKainKafan;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurMalam5 butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            if (s.MalamKe >= 6)
            {
                hud.Tampilkan("Malam 6 belum dibuat (Fase 6).");
                return;
            }
            if (s.MalamKe != 5) return;
            berjalan = true;
            kartu.HitamSeketika();
            pintuDepan.UbahPrompt("Keluar ke desa");
            if (string.IsNullOrEmpty(s.SceneAsal) && !TujuanMalam.Sudah(s, TujuanMalam.GaliKendi)) StartCoroutine(Pembuka());
            else StartCoroutine(Pulang());
        }

        IEnumerator Pembuka()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Malam 5: Tanah Merah", KalenderJawa.NamaMalam(5) + "\n\n" + RingkasanMalam5 + "\n\n" + TujuanBab, 8f));
            hud.Tampilkan("Malam kelima. Ibu makin lemah; aku harus cepat.");
            hud.Tampilkan("Jarum kompas bergetar ke arah pohon asem di ujung pekarangan belakang. Pacul ada di gudang.");
            sibuk = false;
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
            if (!berjalan || gm == null || gm.Dijeda) return;
            var s = gm.State;
            if (sibuk || s.MalamKe != 5 || s.Ending != Ending.Belum || !TujuanMalam.MalamSelesai(s)) return;
            if (!JamGim.ObatSelesai(s))
            {
                if (!petunjukObat)
                {
                    petunjukObat = true;
                    hud.Tampilkan($"Obat Ibu pukul {JamGim.Format(JamGim.MulaiJendelaObat(5))}. Duduk di kursi kamar Ibu [E] untuk menunggu.");
                }
                return;
            }
            if (!gantiDimulai && PergantianMalam.BisaLanjut(s)) StartCoroutine(GantiMalam(s));
        }

        // M5 dan tujuan GaliKendi dicatat efek dialog galiKendi (media:M5, tujuan:GaliKendi).
        void SaatGaliKendi()
        {
            if (!berjalan) return;
            suaraGali.Putar();
            hud.Tampilkan("Kendi dikubur di bawah pohon asem kami sendiri...");
            hud.Tampilkan("Tanah merah lengket. Tanah makam desa ini hitam berpasir, kata catatan Bapak. Bandingkan di makam (lewat pintu depan).");
        }

        // M6 dicatat efek dialog kainKafan (media:M6).
        void SaatKainKafan()
        {
            if (!berjalan) return;
            hud.Tampilkan("Kain kafan berajah. Netralkan di anglo dapur.");
        }

        void SaatPintuLaras()
        {
            if (!berjalan || sibuk) return;
            StartCoroutine(BicaraLaras());
        }

        IEnumerator BicaraLaras()
        {
            var s = State;
            if (TujuanMalam.Sudah(s, TujuanMalam.BicaraLaras))
            {
                hud.Tampilkan("Laras tertidur sambil memeluk lututnya.");
                yield break;
            }
            sibuk = true;
            if (!KesaksianLaras.BisaBicara(s))
            {
                yield return dialog.MulaiDanTunggu(dialogLarasTerkunci);
                hud.Tampilkan("Mungkin teh hangat dari dapur bisa menenangkannya.");
                sibuk = false;
                yield break;
            }
            if (KesaksianLaras.BeriTeh(s))   // K37-A: setelah teh, pilihan Laras ditawarkan lagi
            {
                suaraGelas.Putar();
                yield return dialog.MulaiDanTunggu(dialogLarasTeh);
            }
            yield return dialog.MulaiDanTunggu(dialogLaras);
            if (s.LarasMengunciDiri) hud.Tampilkan("Laras menangis dan mengunci pintu kamarnya.");
            sibuk = false;
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

        bool KeMenuJikaEnding(GameManager gm)
        {
            if (gm.State.Ending == Ending.Belum) return false;
            Debug.LogWarning("Ending tercapai di Malam 5; layar ending dibuat di Fase 7.", this);
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
                PergantianMalam.Lanjut(s);        // Malam 6, Kondisi Ibu -12
                if (KeMenuJikaEnding(gm))
                {
                    dimuat = true;
                    yield break;
                }
                gm.SimpanCheckpoint();            // checkpoint awal Malam 6
                // Fase 6 mengganti bagian di bawah ini dengan gm.MuatScene(gm.SceneUntukState) untuk sutradara Malam 6.
                yield return kartu.StartCoroutine(kartu.Tampilkan(
                    "Malam 6: Obor", KalenderJawa.NamaMalam(6) + "\n\n(Bersambung. Malam 6 dibuat di Fase 6.)", 8f));
                yield return kartu.StartCoroutine(kartu.KeHitam());
                gm.MuatScene(GameManager.SceneMenuUtama);
                dimuat = true;
            }
            finally { if (!dimuat) adegan.Selesai(); }
        }
    }
}
