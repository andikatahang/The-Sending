using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Scene_Desa pada Malam 3 (K17-B): jalan malam dari rumah keluarga ke rumah Bu Sumi dan kembali.
    // Raka muncul di depan rumah keluarga (berangkat) atau di depan rumah Bu Sumi (keluar dari sana). Prolog diurus AlurProlog.
    public class AlurDesaMalam3 : MonoBehaviour
    {
        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] InteraktifAlur pintuRumah, pintuBuSumi;
        [SerializeField] Transform titikDepanRumah, titikDepanBuSumi;
        [SerializeField] Suara suaraPintu, suaraKetuk;

        bool berjalan, sibuk;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            pintuRumah.Ditekan += SaatPintuRumah;
            pintuBuSumi.Ditekan += SaatPintuBuSumi;
        }

        void OnDisable()
        {
            pintuRumah.Ditekan -= SaatPintuRumah;
            pintuBuSumi.Ditekan -= SaatPintuBuSumi;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.State.MalamKe != 3) return;
            var s = gm.State;
            berjalan = true;
            kartu.HitamSeketika();
            bool dariBuSumi = PetaScene.DiDepanRumahBuSumi(s);
            pemain.Pindah(dariBuSumi ? titikDepanBuSumi : titikDepanRumah);
            pintuBuSumi.UbahPrompt(dariBuSumi ? "Pintu rumah Bu Sumi" : null);
            StartCoroutine(Tiba(s, dariBuSumi));
        }

        IEnumerator Tiba(GameState s, bool dariBuSumi)
        {
            sibuk = true;
            yield return kartu.StartCoroutine(kartu.DariHitam());
            if (!dariBuSumi)
                hud.Tampilkan("Rumah Bu Sumi di sisi kanan jalan, ke arah jembatan. Jendelanya masih menyala.");
            else if (!KepercayaanSumi.PintuTerbuka(s))
            {
                hud.Tampilkan("Bu Sumi membanting pintu di belakang Raka.");
                hud.Tampilkan("Ritual Penangkal masih bisa dipelajari dari Halaman 4 buku harian Bapak.");
            }
            else hud.Tampilkan("Rumah keluarga ada di ujung utara jalan.");
            sibuk = false;
        }

        void SaatPintuBuSumi()
        {
            if (!berjalan || sibuk) return;
            if (TujuanMalam.Sudah(State, TujuanMalam.KunjungiSumi))
                hud.Tampilkan("Sudah larut. Bu Sumi tidak membuka pintunya lagi malam ini.");
            else
                StartCoroutine(Pindah(PetaScene.RumahBuSumi, suaraKetuk));
        }

        void SaatPintuRumah()
        {
            if (!berjalan || sibuk) return;
            if (!TujuanMalam.Sudah(State, TujuanMalam.KunjungiSumi))
                hud.Tampilkan("Raka belum menemui Bu Sumi.");
            else
                StartCoroutine(Pindah(PetaScene.RumahKeluarga, suaraPintu));
        }

        IEnumerator Pindah(string scene, Suara suara)
        {
            sibuk = true;
            adegan.Mulai();
            suara.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            GameManager.Instance.MuatScene(scene);   // jeda dilepas KameraAdegan.OnDisable saat scene dibongkar
        }
    }
}
