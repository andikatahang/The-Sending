using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Scene_Desa pada Malam 5 (K32-A): gerbang ke makam, rumah Bu Sumi, rumah Pakde, dan pulang.
    // Titik muncul dari GameState.SceneAsal. Tidak ada checkpoint di desa.
    public class AlurDesaMalam5 : MonoBehaviour
    {
        [SerializeField] KartuTeks kartu;
        [SerializeField] KameraAdegan adegan;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] InteraktifAlur pintuRumah, gerbangMakam, pintuBuSumi, pagarPakde;
        [SerializeField] Transform titikDepanRumah, titikGerbangMakam, titikDepanBuSumi, titikDepanPakde;
        [SerializeField] Suara suaraPintu, suaraKetuk, suasana;

        bool berjalan, sibuk, pesanPakde;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            pintuRumah.Ditekan += SaatPintuRumah;
            gerbangMakam.Ditekan += SaatGerbangMakam;
            pintuBuSumi.Ditekan += SaatPintuBuSumi;
            pagarPakde.Ditekan += SaatPagarPakde;
        }

        void OnDisable()
        {
            pintuRumah.Ditekan -= SaatPintuRumah;
            gerbangMakam.Ditekan -= SaatGerbangMakam;
            pintuBuSumi.Ditekan -= SaatPintuBuSumi;
            pagarPakde.Ditekan -= SaatPagarPakde;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null || (gm.State.MalamKe != 5 && gm.State.MalamKe != 6) || SidangWarga.SaatnyaSidang(gm.State)) return;   // sidang diurus AlurDesaMalam6
            berjalan = true;
            suasana.Putar();
            kartu.HitamSeketika();
            var asal = gm.State.SceneAsal;
            pemain.Pindah(asal == PetaScene.Makam ? titikGerbangMakam
                : asal == PetaScene.RumahBuSumi ? titikDepanBuSumi
                : asal == PetaScene.RumahPakde ? titikDepanPakde
                : titikDepanRumah);
            StartCoroutine(Tiba(asal == PetaScene.RumahKeluarga || string.IsNullOrEmpty(asal)));
        }

        IEnumerator Tiba(bool dariRumah)
        {
            sibuk = true;
            yield return kartu.StartCoroutine(kartu.DariHitam());
            if (dariRumah && State.MalamKe == 6) hud.Tampilkan("Jalan desa sepi. Dari arah balai dusun terdengar orang ramai.");
            else if (dariRumah) hud.Tampilkan("Makam desa di ujung utara jalan. Rumah Bu Sumi di selatan, rumah Pakde di seberangnya.");
            sibuk = false;
        }

        bool Siap => berjalan && !sibuk && !SidangWarga.SaatnyaSidang(State);

        void SaatGerbangMakam()
        {
            if (!Siap) return;
            if (State.MalamKe == 6) hud.Tampilkan("Tidak ada lagi yang perlu kulakukan di makam.");
            else if (!TujuanMalam.Sudah(State, TujuanMalam.GaliKendi)) hud.Tampilkan("Belum ada yang perlu kubandingkan di makam.");
            else StartCoroutine(Pindah(PetaScene.Makam, suaraPintu));
        }

        void SaatPintuBuSumi()
        {
            if (!Siap) return;
            if (State.MalamKe == 6) hud.Tampilkan("Dari balik pintu terdengar Bu Sumi menangis. Ia tidak membukakan pintu.");
            else if (!KepercayaanSumi.PintuTerbuka(State)) hud.Tampilkan("Pintu Bu Sumi tertutup rapat. Tidak ada jawaban.");
            else StartCoroutine(Pindah(PetaScene.RumahBuSumi, suaraKetuk));
        }

        void SaatPagarPakde()
        {
            if (!Siap) return;
            if (!pesanPakde)
            {
                pesanPakde = true;
                hud.Tampilkan(State.MalamKe == 6
                    ? "Pakde ikut berkumpul di balai dusun. Rumahnya kosong, hanya anjingnya."
                    : "Pakde sedang di pos ronda. Rumahnya kosong, hanya anjingnya.");
            }
            StartCoroutine(Pindah(PetaScene.RumahPakde, suaraPintu));
        }

        void SaatPintuRumah()
        {
            if (Siap) StartCoroutine(Pindah(PetaScene.RumahKeluarga, suaraPintu));
        }

        IEnumerator Pindah(string scene, Suara suara)
        {
            sibuk = true;
            adegan.Mulai();
            suara.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            PetaScene.CatatAsal(State, PetaScene.Desa);
            GameManager.Instance.MuatScene(scene);   // jeda dilepas KameraAdegan.OnDisable saat scene dibongkar
        }
    }
}
