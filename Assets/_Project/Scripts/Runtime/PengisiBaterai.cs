using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Stop kontak di kamar Raka: satu-satunya tempat mengisi baterai HP (GDD).
    public class PengisiBaterai : Interaktif
    {
        [SerializeField] Hud hud;

        public override void Interaksi()
        {
            BateraiSenter.Isi(GameManager.Instance.State);
            if (hud != null) hud.Tampilkan("Baterai HP penuh.");
        }
    }
}
