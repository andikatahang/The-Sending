using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Malam 6 pukul 21.00 (K43-A): di mana pun Raka berada, Laras memanggil dan sidang dimuat di desa.
    public class PemanggilSidang : MonoBehaviour
    {
        [SerializeField] KartuTeks kartu;
        [SerializeField] KameraAdegan adegan;
        [SerializeField] string sceneIni;

        bool mulai;

        void Update()
        {
            var gm = GameManager.Instance;
            if (mulai || gm == null || gm.Dijeda || !SidangWarga.SaatnyaSidang(gm.State)) return;
            mulai = true;
            StartCoroutine(Panggil(gm));
        }

        IEnumerator Panggil(GameManager gm)
        {
            if (adegan != null) adegan.Mulai();
            if (kartu != null)
            {
                yield return kartu.Tampilkan("", "Laras berlari menyusul, terengah-engah: \"Mas! Warga bawa obor ke rumah Bu Sumi!\"", 4f);
                yield return kartu.KeHitam();
            }
            PetaScene.CatatAsal(gm.State, sceneIni);
            gm.SimpanCheckpoint();
            gm.MuatScene(PetaScene.Desa);
        }
    }
}
