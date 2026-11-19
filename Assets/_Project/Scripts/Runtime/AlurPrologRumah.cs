using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara akhir Prolog di rumah (Game Story, Prolog butir 5-7): disambut Laras, Bidan Wulan memberi obat,
    // tutorial inventaris dan Jurnal, lalu masuk kamar Ibu dan bab berganti ke Malam 1.
    public class AlurPrologRumah : MonoBehaviour
    {
        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] DataDialog dialogLaras, dialogIbu;
        [SerializeField] Transform titikMulai, bidikIbu;
        [SerializeField] GameObject bidanWulan;
        [SerializeField] ZonaPemicu zonaKamarIbu;
        [SerializeField] AlurMalam1 malam1;

        bool bidanPamit, masukKamar;

        void OnEnable()
        {
            zonaKamarIbu.Masuk += SaatMasukKamarIbu;
            if (GameManager.Instance != null) GameManager.Instance.Berubah += SaatBerubah;
        }

        void OnDisable()
        {
            zonaKamarIbu.Masuk -= SaatMasukKamarIbu;
            if (GameManager.Instance != null) GameManager.Instance.Berubah -= SaatBerubah;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurPrologRumah butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            if (gm.State.MalamKe != 0) return;   // Malam 1 dan seterusnya diurus sutradara malam
            // Scene di-Play langsung dari editor dengan state baru: anggap Raka baru tiba.
            TujuanMalam.Selesaikan(gm.State, TujuanMalam.TibaDiRumah);
            pemain.Pindah(titikMulai);
            kartu.HitamSeketika();
            StartCoroutine(Sambutan());
        }

        IEnumerator Sambutan()
        {
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.DariHitam());
                yield return dialog.MulaiDanTunggu(dialogLaras);
            }
            finally { adegan.Selesai(); }
        }

        // Efek "tujuan:BicaraBidan" ada di baris terakhir dialog Bidan Wulan.
        void SaatBerubah(Perubahan p)
        {
            var s = GameManager.Instance.State;
            if (p != Perubahan.Tujuan || bidanPamit || s.MalamKe != 0 || !TujuanMalam.Sudah(s, TujuanMalam.BicaraBidan)) return;
            bidanPamit = true;
            StartCoroutine(BidanPamit());
        }

        IEnumerator BidanPamit()
        {
            while (dialog.Terbuka) yield return null;
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.KeHitam());
                bidanWulan.SetActive(false);   // ia berdiri di ambang pintu kamar Ibu; setelah pamit pintunya bebas
                yield return kartu.StartCoroutine(kartu.DariHitam());
            }
            finally { adegan.Selesai(); }
            hud.Tampilkan("Bidan Wulan pamit pulang.");
            hud.Tampilkan("Tekan [I] untuk melihat inventaris.");
            hud.Tampilkan("Tekan [Tab] untuk membuka Jurnal: tujuan, bukti, dan catatan.");
        }

        void SaatMasukKamarIbu()
        {
            var gm = GameManager.Instance;
            if (masukKamar || gm == null || gm.State.MalamKe != 0) return;
            if (!TujuanMalam.Sudah(gm.State, TujuanMalam.BicaraBidan)) return;
            masukKamar = true;
            StartCoroutine(MasukKamarIbu());
        }

        // Game Story: Raka masuk ke kamar Ibu, Ibu menatap kosong dan berbisik pelan, bab berlanjut ke Malam 1.
        IEnumerator MasukKamarIbu()
        {
            var gm = GameManager.Instance;
            var s = gm.State;
            TujuanMalam.Selesaikan(s, TujuanMalam.MasukKamarIbu);
            if (!PergantianMalam.BisaLanjut(s))
            {
                Debug.LogError("Prolog belum bisa ditutup: ada tujuan wajib yang belum selesai.", this);
                yield break;
            }
            adegan.Mulai(bidikIbu);
            try
            {
                yield return dialog.MulaiDanTunggu(dialogIbu);
                yield return kartu.StartCoroutine(kartu.KeHitam());
            }
            finally { adegan.Selesai(); }
            PergantianMalam.Lanjut(s);   // Prolog -> Malam 1: jam dan Kondisi Ibu tidak berubah
            gm.SimpanCheckpoint();       // checkpoint awal Malam 1
            malam1.Mulai();
        }
    }
}
