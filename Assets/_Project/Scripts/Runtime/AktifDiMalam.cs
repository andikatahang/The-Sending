using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Menyalakan objek sasaran hanya pada malam dari..sampai (0 = Prolog). Dipakai untuk perubahan isi scene antarmalam.
    public class AktifDiMalam : MonoBehaviour
    {
        [SerializeField, Range(0, 7)] int dari;
        [SerializeField, Range(0, 7)] int sampai = 7;
        [SerializeField] GameObject[] sasaran = new GameObject[0];

        void OnEnable()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.Berubah += SaatBerubah;
            Terapkan();
        }

        void OnDisable()
        {
            if (GameManager.Instance != null) GameManager.Instance.Berubah -= SaatBerubah;
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
