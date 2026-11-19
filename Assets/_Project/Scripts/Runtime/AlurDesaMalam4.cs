using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Scene_Desa pada Malam 4 (K26-A): pos ronda (pinjam tali, gosip K1/K2, bela atau diam), pulang lewat pintu rumah.
    // Efek (barang, katanya, kepercayaan, bukti) ada di DataDialog; sutradara hanya memilih dialog dan memindah scene.
    public class AlurDesaMalam4 : MonoBehaviour
    {
        [SerializeField] KartuTeks kartu;
        [SerializeField] KameraAdegan adegan;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] PanelDialog panel;
        [SerializeField] InteraktifAlur pintuRumah, pakJoko, warga;
        [SerializeField] Transform titikDepanRumah;
        [SerializeField] Suara suaraPintu;
        [SerializeField] DataDialog dialogPakJoko;   // pinjam tali + pengait
        [SerializeField] DataDialog dialogGosip;     // K1/K2 + pilihan bela/diam
        [SerializeField] DataDialog dialogB2;        // ayam cemani, hanya jika M4 sudah ditemukan

        bool berjalan, sibuk, sudahJoko, sudahGosip, sudahB2;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            pintuRumah.Ditekan += SaatPintuRumah;
            pakJoko.Ditekan += SaatPakJoko;
            warga.Ditekan += SaatWarga;
        }

        void OnDisable()
        {
            pintuRumah.Ditekan -= SaatPintuRumah;
            pakJoko.Ditekan -= SaatPakJoko;
            warga.Ditekan -= SaatWarga;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.State.MalamKe != 4) return;
            berjalan = true;
            kartu.HitamSeketika();
            pemain.Pindah(titikDepanRumah);
            StartCoroutine(Tiba());
        }

        IEnumerator Tiba()
        {
            sibuk = true;
            yield return kartu.StartCoroutine(kartu.DariHitam());
            hud.Tampilkan("Kentongan terdengar dari pos ronda di ujung jalan.");
            sibuk = false;
        }

        void SaatPakJoko()
        {
            if (!berjalan || sibuk) return;
            if (sudahJoko) { hud.Tampilkan("Pak Joko sudah meminjamkan tali dan pengait."); return; }
            sudahJoko = true;
            StartCoroutine(Bicara(dialogPakJoko));
        }

        void SaatWarga()
        {
            if (!berjalan || sibuk) return;
            // Syarat B2 dicek di sini: DataDialog/PemeriksaDialog belum punya syarat tampil.
            if (!sudahGosip) { sudahGosip = true; StartCoroutine(Bicara(dialogGosip)); }
            else if (!sudahB2 && State.MediaDitemukan[Kode.M4] && !State.BuktiKunci[Kode.B2]) { sudahB2 = true; StartCoroutine(Bicara(dialogB2)); }
            else hud.Tampilkan("Para warga kembali berbisik di antara mereka.");
        }

        IEnumerator Bicara(DataDialog data)
        {
            sibuk = true;
            yield return panel.MulaiDanTunggu(data);
            sibuk = false;
        }

        void SaatPintuRumah()
        {
            if (!berjalan || sibuk) return;
            StartCoroutine(Pulang());
        }

        IEnumerator Pulang()
        {
            sibuk = true;
            adegan.Mulai();
            suaraPintu.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            GameManager.Instance.MuatScene(PetaScene.RumahKeluarga);   // jeda dilepas KameraAdegan.OnDisable saat scene dibongkar
        }
    }
}
