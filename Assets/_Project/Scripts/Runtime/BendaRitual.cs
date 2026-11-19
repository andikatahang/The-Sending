using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Satu langkah Ritual Penangkal di dapur (K25-B): anglo, garam, daun kelor, korek api, atau bilah bambu kuning.
    public class BendaRitual : Interaktif
    {
        [SerializeField] RitualDapur dapur;
        [SerializeField] LangkahRitual langkah;

        public override string Prompt
        {
            get
            {
                var gm = GameManager.Instance;
                if (gm == null || langkah != LangkahRitual.LetakkanMedia || dapur.Media >= 0) return base.Prompt;
                int m = RitualPenangkal.MediaBerikutnya(gm.State);
                return m < 0 ? base.Prompt : $"Letakkan {RitualDapur.NamaMedia(m)} di anglo";
            }
        }

        public override bool BisaDiinteraksi =>
            GameManager.Instance != null && GameManager.Instance.State.MalamKe >= 3 && dapur != null && !dapur.MenungguKubur;

        public override void Interaksi() => dapur.Lakukan(langkah);
    }
}
