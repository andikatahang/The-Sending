using System.Text;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    public class PanelJurnal : MonoBehaviour
    {
        enum Tab { PapanBukti, BukuHarian, Catatan }

        static readonly EntriJurnal Kosong = new EntriJurnal { Judul = "(belum ditulis)", Isi = "" };

        [SerializeField] DataJurnal data;

        GameObject panel;
        Text judul, isi;
        Tab tab;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Jurnal", 15).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.09f, 0.07f, 0.05f, 0.95f), new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.92f));
            panel = latar.gameObject;
            judul = UiPabrik.BuatTeks(latar.transform, "Judul", 28, TextAnchor.UpperLeft, new Vector2(0.03f, 0.9f), new Vector2(0.97f, 0.98f));
            isi = UiPabrik.BuatTeks(latar.transform, "Isi", 26, TextAnchor.UpperLeft, new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.88f));
            panel.SetActive(false);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var k = gm.Kontrol;

            if (k.Jurnal.WasPressedThisFrame())
            {
                if (Terbuka)
                {
                    panel.SetActive(false);
                    gm.Jeda(false);
                }
                else if (!gm.Dijeda)
                {
                    panel.SetActive(true);
                    gm.Jeda(true);
                    Gambar(gm.State);
                }
            }
            if (!Terbuka) return;

            for (int i = 0; i < 3; i++)
            {
                if (!k.PilihanAngka[i].WasPressedThisFrame()) continue;
                tab = (Tab)i;
                Gambar(gm.State);
            }
        }

        // Panel dimatikan atau dihancurkan saat Jurnal terbuka (misalnya ganti scene): lepaskan jeda.
        void OnDisable()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }

        void Gambar(GameState s)
        {
            judul.text = "[1] Papan Bukti     [2] Buku Harian     [3] Catatan     [Tab] Tutup";
            isi.text = tab == Tab.PapanBukti ? TeksPapan(s)
                     : tab == Tab.BukuHarian ? TeksDiary(s)
                     : TeksCatatan();
        }

        string TeksPapan(GameState s)
        {
            var sb = new StringBuilder();
            for (int m = 0; m < Kode.JumlahMedia; m++)
            {
                if (!s.MediaDitemukan[m]) continue;
                string status = s.MediaDinetralkan[m] ? " (dinetralkan)" : s.MediaDiserahkan[m] ? " (diserahkan)" : "";
                Tulis(sb, $"[MEDIA] M{m + 1} ", Entri(data != null ? data.Media : null, m), status);
            }
            for (int b = 0; b < Kode.JumlahBukti; b++)
                if (s.BuktiKunci[b]) Tulis(sb, $"[BUKTI] B{b + 1} ", Entri(data != null ? data.Bukti : null, b), "");
            for (int k = 0; k < Kode.JumlahKatanya; k++)
                if (s.KatanyaDiketahui[k]) Tulis(sb, $"[{Label(s.StatusKatanya[k])}] K{k + 1} ", Entri(data != null ? data.Katanya : null, k), "");
            return sb.Length == 0 ? "Belum ada informasi." : sb.ToString();
        }

        string TeksDiary(GameState s)
        {
            var sb = new StringBuilder();
            for (int h = 0; h < Kode.JumlahHalaman; h++)
                if (s.HalamanDiary[h]) Tulis(sb, $"Halaman {h + 1}: ", Entri(data != null ? data.HalamanDiary : null, h), "");
            return sb.Length == 0 ? "Belum ada halaman yang ditemukan." : sb.ToString();
        }

        string TeksCatatan()
        {
            var sb = new StringBuilder();
            sb.AppendLine("CATATAN BAPAK");
            if (data != null) foreach (var e in data.CatatanBapak) Tulis(sb, "- ", e ?? Kosong, "");
            sb.AppendLine().AppendLine("CATATAN RAKA");
            if (data != null) foreach (var e in data.CatatanRaka) Tulis(sb, "- ", e ?? Kosong, "");
            return sb.ToString();
        }

        static void Tulis(StringBuilder sb, string awalan, EntriJurnal e, string akhiran)
        {
            sb.Append(awalan).Append(e.Judul).AppendLine(akhiran);
            if (!string.IsNullOrEmpty(e.Isi)) sb.Append("    ").AppendLine(e.Isi);
        }

        static EntriJurnal Entri(EntriJurnal[] daftar, int i) =>
            daftar != null && i < daftar.Length && daftar[i] != null ? daftar[i] : Kosong;

        static string Label(StatusInfo status) =>
            status == StatusInfo.Bukti ? "BUKTI" : status == StatusInfo.Terbantahkan ? "TERBANTAHKAN" : "KATANYA";
    }
}
