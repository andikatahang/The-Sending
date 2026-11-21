using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Malam 4 (K28-A): naik/turun plafon dapur dengan layar hitam singkat, tanpa animasi panjat.
    // Butuh tangga bambu dari gudang. M4 di atas plafon diambil lewat Interaktif lain.
    public class NaikPlafon : Interaktif
    {
        [SerializeField] Transform titikAtas, titikBawah;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PemainGerak pemain;
        [SerializeField] Hud hud;
        [SerializeField] AlurMalam4 alur;   // boleh kosong

        bool sibuk;

        public bool DiAtas { get; private set; }

        public override string Prompt => DiAtas ? "Turun dari plafon" : "Naik ke plafon";

        public override bool BisaDiinteraksi => GameManager.Instance != null && !sibuk && !kartu.Tampil;

        public override void Interaksi()
        {
            if (alur != null && alur.SedangDikejar)
            {
                if (hud != null) hud.Tampilkan("Tidak sempat naik sekarang!");
                return;
            }
            if (!Inventaris.Punya(GameManager.Instance.State, Inventaris.TanggaBambu))
            {
                if (hud != null) hud.Tampilkan("Terlalu tinggi. Ambil tangga bambu di gudang.");
                return;
            }
            kartu.StartCoroutine(Pindah());
        }

        IEnumerator Pindah()
        {
            sibuk = true;
            yield return kartu.StartCoroutine(kartu.KeHitam());
            DiAtas = !DiAtas;
            pemain.Pindah(DiAtas ? titikAtas : titikBawah);
            yield return kartu.StartCoroutine(kartu.DariHitam());
            sibuk = false;
        }
    }
}
