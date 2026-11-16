using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Menyalakan objek sasaran hanya pada malam dari..sampai (0 = Prolog). Dipakai untuk perubahan isi scene antarmalam.
    // Pasang di objek yang selalu aktif, jangan di sasaran atau anak sasaran, supaya tidak mematikan dirinya sendiri.
    public class AktifDiMalam : MonoBehaviour
    {
        [SerializeField, Range(0, 7)] int dari;
        [SerializeField, Range(0, 7)] int sampai = 7;
        [SerializeField] GameObject[] sasaran = new GameObject[0];

        void OnEnable()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogWarning($"AktifDiMalam di '{name}' tidak menemukan GameManager; sasaran tidak diatur.", this);
                return;
            }
            foreach (var g in sasaran)
                if (g != null && transform.IsChildOf(g.transform))
                    Debug.LogWarning($"AktifDiMalam di '{name}' berada di dalam sasarannya ('{g.name}') dan akan mematikan dirinya sendiri.", this);
            gm.Berubah += SaatBerubah;
            gm.StateDiganti += Terapkan;
            Terapkan();
        }

        void OnDisable()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            gm.Berubah -= SaatBerubah;
            gm.StateDiganti -= Terapkan;
        }

        void SaatBerubah(Perubahan p)
        {
            if (p == Perubahan.Malam) Terapkan();
        }

        void Terapkan()
        {
            bool aktif = RentangMalam.Berisi(GameManager.Instance.State.MalamKe, dari, sampai);
            foreach (var g in sasaran)
                if (g != null) g.SetActive(aktif);
        }
    }
}
