using System;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Satu dari tujuh paku di saka guru (Game Story Malam 3), berukir nama hari. Butuh tang. Aturannya di TujuhPaku (Core);
    // sutradara menangani bunyi dan Sosok Santet setelah salah urutan. Paku yang sudah dicabut hilang, juga setelah save dimuat.
    public class PakuDiSaka : Interaktif
    {
        [SerializeField] Hari hari;
        [SerializeField] GameObject visual;
        [SerializeField] Hud hud;

        public event Action<HasilCabut> Dicabut;

        static bool PunyaTang =>
            GameManager.Instance != null && Inventaris.Punya(GameManager.Instance.State, Inventaris.Tang);

        public override string Prompt => (PunyaTang ? "Cabut paku berukir " : "Paku berukir ") + hari;

        public override bool BisaDiinteraksi
        {
            get
            {
                var gm = GameManager.Instance;
                return gm != null && gm.State.MalamKe == 3 && gm.State.MediaDitemukan[Kode.M2] && !TujuhPaku.SudahDicabut(gm.State, hari);
            }
        }

        public override void Interaksi()
        {
            if (!PunyaTang)
            {
                hud.Tampilkan("Pakunya tertanam dalam. Butuh tang dari gudang.");
                return;
            }
            var hasil = TujuhPaku.Cabut(GameManager.Instance.State, hari);
            if (hasil != HasilCabut.Ditolak) Dicabut?.Invoke(hasil);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || visual == null) return;
            bool tampak = !TujuhPaku.SudahDicabut(gm.State, hari);
            if (visual.activeSelf != tampak) visual.SetActive(tampak);
        }
    }
}
