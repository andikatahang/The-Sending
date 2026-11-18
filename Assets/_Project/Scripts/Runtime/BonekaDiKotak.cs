using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Game Story Malam 2: jika Raka menyimpan boneka sendiri, boneka disimpan di kotak kayu kamar Raka sampai dinetralkan.
    // Pasang di objek yang selalu aktif, bukan di boneka.
    public class BonekaDiKotak : MonoBehaviour
    {
        [SerializeField] GameObject boneka;

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || boneka == null) return;
            var s = gm.State;
            bool tampak = s.MediaDitemukan[Kode.M1] && !s.MediaDinetralkan[Kode.M1] && !s.MediaDiserahkan[Kode.M1]
                          && (s.MalamKe > 2 || TujuanMalam.Sudah(s, TujuanMalam.PilihanWiryo));
            if (boneka.activeSelf != tampak) boneka.SetActive(tampak);
        }
    }
}
