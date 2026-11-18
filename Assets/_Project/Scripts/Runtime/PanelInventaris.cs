using System.Text;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Inventaris (tombol I): daftar barang beserta keterangannya. Menjeda gim seperti Jurnal.
    public class PanelInventaris : MonoBehaviour
    {
        GameObject panel;
        Text isi;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Inventaris", 15).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.09f, 0.07f, 0.05f, 0.95f), new Vector2(0.2f, 0.15f), new Vector2(0.8f, 0.85f));
            panel = latar.gameObject;
            UiPabrik.BuatTeks(latar.transform, "Judul", 28, TextAnchor.UpperLeft, new Vector2(0.04f, 0.88f), new Vector2(0.96f, 0.97f)).text =
                "INVENTARIS     [I/Esc] Tutup";
            isi = UiPabrik.BuatTeks(latar.transform, "Isi", 26, TextAnchor.UpperLeft, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.86f));
            panel.SetActive(false);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var k = gm.Kontrol;

            bool tombol = k.Inventaris.WasPressedThisFrame();
            if (Terbuka && (tombol || k.JedaMenu.WasPressedThisFrame())) Tutup();
            else if (tombol && !gm.Dijeda)
            {
                panel.SetActive(true);
                gm.Jeda(true);
                Gambar(gm.State);
            }
        }

        // Panel dimatikan atau dihancurkan saat terbuka (misalnya ganti scene): lepaskan jeda.
        void OnDisable() => Tutup();

        void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }

        void Gambar(GameState s)
        {
            var sb = new StringBuilder();
            foreach (var id in s.Barang)
                sb.AppendLine(Inventaris.Nama(id)).Append("    ").AppendLine(Inventaris.Keterangan(id)).AppendLine();
            isi.text = sb.Length == 0 ? "Belum membawa apa-apa." : sb.ToString();
        }
    }
}
