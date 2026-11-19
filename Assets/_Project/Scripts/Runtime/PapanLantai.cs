using System;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Papan lantai kopong di kamar Ibu (Game Story Malam 2 butir 2). Butuh linggis; sutradara yang menjalankan isinya.
    // Papan hilang selama boneka (M1) sudah ditemukan, termasuk setelah save dimuat.
    public class PapanLantai : Interaktif
    {
        [SerializeField] GameObject papan;
        [SerializeField] Hud hud;

        public event Action Dibongkar;

        static bool PunyaLinggis =>
            GameManager.Instance != null && Inventaris.Punya(GameManager.Instance.State, Inventaris.Linggis);

        public override string Prompt => PunyaLinggis ? "Bongkar papan lantai dengan linggis" : base.Prompt;

        public override bool BisaDiinteraksi
        {
            get
            {
                var gm = GameManager.Instance;
                return gm != null && gm.State.MalamKe >= 2 && !gm.State.MediaDitemukan[Kode.M1];
            }
        }

        public override void Interaksi()
        {
            if (PunyaLinggis) Dibongkar?.Invoke();
            else if (hud != null) hud.Tampilkan("Papan ini kopong. Butuh linggis untuk membongkarnya.");
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || papan == null) return;
            bool tampak = !gm.State.MediaDitemukan[Kode.M1];
            if (papan.activeSelf != tampak) papan.SetActive(tampak);
        }
    }
}
