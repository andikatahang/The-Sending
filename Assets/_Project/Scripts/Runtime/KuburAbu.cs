using UnityEngine;

namespace TheSending.Runtime
{
    // Langkah keenam Ritual Penangkal: abu dikubur dengan tangan di pekarangan (Game Story 6.5, desain 4.5).
    public class KuburAbu : Interaktif
    {
        [SerializeField] RitualDapur dapur;
        [SerializeField] Hud hud;

        public override bool BisaDiinteraksi => dapur != null && dapur.MenungguKubur;

        public override void Interaksi()
        {
            int media = dapur.Media;
            if (dapur.Kubur()) hud.Tampilkan($"Abu dikubur. Media M{media + 1} sudah dinetralkan.");
        }
    }
}
