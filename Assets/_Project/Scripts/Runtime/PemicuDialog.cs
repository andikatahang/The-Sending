using UnityEngine;

namespace TheSending.Runtime
{
    public class PemicuDialog : Interaktif
    {
        [SerializeField] DataDialog dialog;
        [SerializeField] PanelDialog panel;
        [SerializeField] bool sekaliSaja;

        bool sudah;

        public override bool BisaDiinteraksi => dialog != null && panel != null && !(sekaliSaja && sudah);

        public override void Interaksi()
        {
            sudah = true;
            panel.Mulai(dialog);
        }
    }
}
