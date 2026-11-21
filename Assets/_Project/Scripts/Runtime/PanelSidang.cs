using System.Collections;
using System.Collections.Generic;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    public enum JawabanSidang { Item, Diam, Menuduh }

    // Sidang Warga (Malam 6): satu tuduhan, pemain memilih item Papan Bukti, diam, atau ikut menuduh. Esc tidak menutup.
    public class PanelSidang : MonoBehaviour
    {
        const int MaksSatuKolom = 8;

        GameObject panel;
        Text judul;
        RectTransform daftar;
        bool dijawab;

        public bool Terbuka => panel != null && panel.activeSelf;
        public JawabanSidang Jawaban { get; private set; }
        public ItemPapan Item { get; private set; }

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Sidang", 22).transform;   // di atas panel dialog
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.05f, 0.04f, 0.03f, 0.96f), new Vector2(0.1f, 0.08f), new Vector2(0.9f, 0.92f));
            panel = latar.gameObject;
            judul = UiPabrik.BuatTeks(latar.transform, "Tuduhan", 30, TextAnchor.UpperLeft, new Vector2(0.04f, 0.84f), new Vector2(0.96f, 0.97f));
            daftar = UiPabrik.BuatPanel(latar.transform, "Daftar", Color.clear, new Vector2(0.04f, 0.03f), new Vector2(0.96f, 0.82f)).rectTransform;
            panel.SetActive(false);
        }

        public IEnumerator Tanya(string tuduhan, IReadOnlyList<ItemPapan> pilihan, GameState s, DataJurnal jurnal)
        {
            var gm = GameManager.Instance;
            if (Terbuka || gm == null) yield break;
            foreach (Transform anak in daftar) Destroy(anak.gameObject);
            judul.text = tuduhan;
            dijawab = false;

            int jumlah = pilihan.Count + 2;
            int kolom = jumlah > MaksSatuKolom ? 2 : 1;
            int baris = Mathf.CeilToInt(jumlah / (float)kolom);
            Button pertama = null;
            for (int i = 0; i < jumlah; i++)
            {
                float x0 = (i / baris) / (float)kolom, x1 = (i / baris + 1) / (float)kolom;
                float y1 = 1f - (i % baris) / (float)baris, y0 = 1f - (i % baris + 1) / (float)baris;
                string label;
                UnityEngine.Events.UnityAction klik;
                if (i < pilihan.Count)
                {
                    var item = pilihan[i];
                    label = Label(item, s, jurnal);
                    klik = () => Jawab(JawabanSidang.Item, item);
                }
                else if (i == pilihan.Count) { label = "Diam saja"; klik = () => Jawab(JawabanSidang.Diam, default); }
                else { label = "Ikut menuduh"; klik = () => Jawab(JawabanSidang.Menuduh, default); }
                var tombol = UiPabrik.BuatTombol(daftar, label, new Vector2(x0 + 0.01f, y0 + 0.01f), new Vector2(x1 - 0.01f, y1 - 0.01f), klik);
                var teks = tombol.GetComponentInChildren<Text>();
                teks.resizeTextForBestFit = true;   // judul panjang: teks mengecil sampai muat
                teks.resizeTextMinSize = 12;
                teks.resizeTextMaxSize = 26;
                if (pertama == null) pertama = tombol;
            }

            panel.SetActive(true);
            gm.Jeda(true);
            UiPabrik.Pilih(pertama);
            while (!dijawab) yield return null;
        }

        static string Label(ItemPapan item, GameState s, DataJurnal jurnal)
        {
            if (!item.IsKatanya) return $"[BUKTI] B{item.Indeks + 1} {Judul(jurnal != null ? jurnal.Bukti : null, item.Indeks)}";
            string status = s.StatusKatanya[item.Indeks] switch
            {
                StatusInfo.Bukti => "BUKTI",
                StatusInfo.Terbantahkan => "TERBANTAHKAN",
                _ => "KATANYA",
            };
            return $"[{status}] K{item.Indeks + 1} {Judul(jurnal != null ? jurnal.Katanya : null, item.Indeks)}";
        }

        static string Judul(EntriJurnal[] entri, int i) =>
            entri != null && i < entri.Length && entri[i] != null && !string.IsNullOrEmpty(entri[i].Judul) ? entri[i].Judul : "(belum ditulis)";

        void Jawab(JawabanSidang jawaban, ItemPapan item)
        {
            if (!Terbuka) return;
            Jawaban = jawaban;
            Item = item;
            Tutup();
        }

        // Panel dimatikan atau dihancurkan saat terbuka (misalnya ganti scene): lepaskan jeda.
        void OnDisable() => Tutup();

        void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            UiPabrik.Pilih(null);
            dijawab = true;
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }
    }
}
