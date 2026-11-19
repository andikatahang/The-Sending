using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Bahan Ritual Penangkal di rumah (desain 4.5): garam di rak bumbu dapur, daun kelor di pohon pekarangan, bilah bambu
    // kuning di gudang. Bisa diambil setelah Raka tahu ritualnya (Halaman 4 atau Bu Sumi). Bendanya tetap di tempat.
    public class AmbilBahan : Interaktif
    {
        [SerializeField] BahanRitual bahan;
        [SerializeField] Hud hud;
        [SerializeField] string pesan;

        public override bool BisaDiinteraksi
        {
            get
            {
                var gm = GameManager.Instance;
                return gm != null && gm.State.MalamKe >= 3 && gm.State.RitualDiketahui && !gm.State.BahanRitual[(int)bahan];
            }
        }

        public override void Interaksi()
        {
            RitualPenangkal.AmbilBahan(GameManager.Instance.State, bahan);
            hud.Tampilkan(pesan);
        }
    }
}
