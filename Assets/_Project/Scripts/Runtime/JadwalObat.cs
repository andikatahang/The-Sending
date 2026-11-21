using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Pengingat obat Malam 1-6 (desain 4.4): notifikasi HP dan dentang jam saat jendela obat dibuka,
    // teguran Laras jika jendela lewat tanpa obat. Malam 7 diurus adegannya sendiri (Laras memberi obat).
    public class JadwalObat : MonoBehaviour
    {
        [SerializeField] Hud hud;
        [SerializeField] PanelDialog panel;
        [SerializeField] DataDialog teguranLaras;
        [SerializeField] Suara dentang;

        int malamDiingatkan = -1, malamDitegur = -1;

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.Dijeda) return;
            var s = gm.State;
            if (s.MalamKe < 1 || s.MalamKe > 6 || s.ObatSudahDiberikan) return;

            float mulai = JamGim.MulaiJendelaObat(s.MalamKe), akhir = JamGim.AkhirJendelaObat(s.MalamKe);
            if (malamDiingatkan != s.MalamKe && s.MenitGim >= mulai && s.MenitGim < akhir)
            {
                malamDiingatkan = s.MalamKe;
                if (dentang != null) dentang.Putar();
                if (hud != null) hud.Tampilkan($"HP: pukul {JamGim.Format(mulai)}. Waktunya memberi obat Ibu di kamarnya.");
            }
            if (s.MalamKe == 6) return;   // Malam 6: pukul 21.00 sudah sidang, tanpa teguran Laras
            if (malamDitegur != s.MalamKe && s.MenitGim >= akhir)
            {
                malamDitegur = s.MalamKe;
                if (panel != null && teguranLaras != null) panel.Mulai(teguranLaras);
            }
        }
    }
}
