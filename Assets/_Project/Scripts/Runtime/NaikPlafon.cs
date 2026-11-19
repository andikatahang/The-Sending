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

        public bool DiAtas { get; private set; }

        public override string Prompt => DiAtas ? "Turun dari plafon" : "Naik ke plafon";

        public override bool BisaDiinteraksi => GameManager.Instance != null && !kartu.Tampil;

        public override void Interaksi()
        {
            if (!Inventaris.Punya(GameManager.Instance.State, Inventaris.TanggaBambu))
            {
                if (hud != null) hud.Tampilkan("Butuh tangga dari gudang.");
                return;
            }
            kartu.StartCoroutine(Pindah());
        }

        IEnumerator Pindah()
        {
            yield return kartu.StartCoroutine(kartu.KeHitam());
            DiAtas = !DiAtas;
            pemain.Pindah(DiAtas ? titikAtas : titikBawah);
            yield return kartu.StartCoroutine(kartu.DariHitam());
        }
    }
}
