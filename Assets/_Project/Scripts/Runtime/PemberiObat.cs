using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Ibu di ranjang. E memberi obat pada jendela obat (desain 4.4); di luar jendela hanya memberi tahu pemain.
    public class PemberiObat : Interaktif
    {
        [SerializeField] Hud hud;
        [SerializeField] PanelDialog panel;
        [SerializeField] DataDialog dialogSetelahObat;

        public override string Prompt => "Beri obat Ibu";

        public override bool BisaDiinteraksi
        {
            get
            {
                var gm = GameManager.Instance;
                return gm != null && gm.State.MalamKe >= 1 && Inventaris.Punya(gm.State, Inventaris.ObatIbu);
            }
        }

        public override void Interaksi()
        {
            var s = GameManager.Instance.State;
            if (KondisiIbu.BeriObat(s))
            {
                Beri("Ibu meminum obat dan oralit.");
                if (panel != null && dialogSetelahObat != null) panel.Mulai(dialogSetelahObat);
                return;
            }
            float mulai = JamGim.MulaiJendelaObat(s.MalamKe);
            Beri(s.ObatSudahDiberikan ? "Obat malam ini sudah diberikan."
                : s.MenitGim < mulai ? $"Belum waktunya. Obat diminum pukul {JamGim.Format(mulai)}."
                : "Jam obat sudah lewat.");
        }

        void Beri(string pesan)
        {
            if (hud != null) hud.Tampilkan(pesan);
        }
    }
}
