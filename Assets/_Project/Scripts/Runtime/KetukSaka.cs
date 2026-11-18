using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Puzzle mencari saka (Game Story Malam 3): E mengetuk saka guru. Saka yang benar berbunyi berongga (TujuhPaku.Ketuk)
    // dan paku di baliknya ditemukan; sutradara menampilkannya lewat Perubahan.Media.
    public class KetukSaka : Interaktif
    {
        [SerializeField, Range(0, 3)] int nomor;
        [SerializeField] Hud hud;
        [SerializeField] Suara padat, berongga;

        public override bool BisaDiinteraksi
        {
            get
            {
                var gm = GameManager.Instance;
                return gm != null && gm.State.MalamKe == 3 && !gm.State.MediaDitemukan[Kode.M2];
            }
        }

        public override void Interaksi()
        {
            if (TujuhPaku.Ketuk(GameManager.Instance.State, nomor))
            {
                berongga.Putar();
                return;
            }
            padat.Putar();
            hud.Tampilkan("Saka guru ini berbunyi padat.");
        }
    }
}
