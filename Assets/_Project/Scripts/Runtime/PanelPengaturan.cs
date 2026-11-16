using System;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Panel pengaturan untuk menu utama dan menu jeda. Setiap perubahan langsung disimpan dan diterapkan.
    public class PanelPengaturan : MonoBehaviour
    {
        const float LangkahVolume = 0.1f, LangkahSensitivitas = 0.01f;
        const float AtasBaris = 0.86f, TinggiBaris = 0.09f, JarakBaris = 0.015f;
        static readonly string[] LabelGrup = { "Volume utama", "Musik", "Efek suara", "Suasana", "Dialog" };

        GameObject panel;
        Button tombolPertama;
        Action saatTutup, segarkan;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvasMenu("Pengaturan", 60).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.03f, 0.03f, 0.04f, 0.97f), new Vector2(0.2f, 0.08f), new Vector2(0.8f, 0.92f));
            latar.raycastTarget = true;   // menutup klik ke menu di belakangnya
            panel = latar.gameObject;
            var t = latar.transform;
            UiPabrik.BuatTeks(t, "Judul", 44, TextAnchor.MiddleCenter, new Vector2(0f, 0.88f), new Vector2(1f, 0.98f)).text = "PENGATURAN";

            int baris = 0;
            for (int i = 0; i < Pengaturan.NamaGrup.Length; i++)
            {
                int grup = i;
                BarisNilai(t, baris++, LabelGrup[i],
                    p => Mathf.RoundToInt(p.Volume[grup] * 100f) + "%",
                    (p, arah) => p.Volume[grup] += arah * LangkahVolume);
            }
            BarisNilai(t, baris++, "Sensitivitas mouse",
                p => Mathf.RoundToInt(p.Sensitivitas * 100f).ToString(),
                (p, arah) => p.Sensitivitas += arah * LangkahSensitivitas);
            BarisNilai(t, baris++, "Balik sumbu Y", p => p.BalikY ? "Ya" : "Tidak", (p, arah) => p.BalikY = !p.BalikY);
            BarisNilai(t, baris++, "Layar penuh", p => p.LayarPenuh ? "Ya" : "Tidak", (p, arah) => p.LayarPenuh = !p.LayarPenuh);

            UiPabrik.BuatTombol(t, "Kembali", new Vector2(0.35f, 0.03f), new Vector2(0.65f, 0.11f), Tutup);
            panel.SetActive(false);
        }

        // Satu baris: label, tombol kurang, nilai, tombol tambah.
        void BarisNilai(Transform induk, int baris, string label, Func<Pengaturan, string> baca, Action<Pengaturan, int> ubah)
        {
            float atas = AtasBaris - baris * TinggiBaris, bawah = atas - TinggiBaris + JarakBaris;
            UiPabrik.BuatTeks(induk, label, 30, TextAnchor.MiddleLeft, new Vector2(0.06f, bawah), new Vector2(0.54f, atas)).text = label;
            var kurang = UiPabrik.BuatTombol(induk, "<", new Vector2(0.56f, bawah), new Vector2(0.63f, atas), () => Ubah(ubah, -1));
            var nilai = UiPabrik.BuatTeks(induk, label + " (nilai)", 30, TextAnchor.MiddleCenter, new Vector2(0.64f, bawah), new Vector2(0.84f, atas));
            UiPabrik.BuatTombol(induk, ">", new Vector2(0.85f, bawah), new Vector2(0.92f, atas), () => Ubah(ubah, 1));
            if (tombolPertama == null) tombolPertama = kurang;
            segarkan += () => nilai.text = baca(GameManager.Instance.Pengaturan);
        }

        void Ubah(Action<Pengaturan, int> ubah, int arah)
        {
            var gm = GameManager.Instance;
            ubah(gm.Pengaturan, arah);
            gm.SimpanPengaturan();   // merapikan nilai (batas), menyimpan, dan menerapkan
            segarkan();
        }

        public void Buka(Action saatTutup = null)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("PanelPengaturan butuh GameManager di scene.", this);
                return;
            }
            this.saatTutup = saatTutup;
            panel.SetActive(true);
            segarkan();
            UiPabrik.Pilih(tombolPertama);
        }

        public void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            var selesai = saatTutup;
            saatTutup = null;
            selesai?.Invoke();
        }
    }
}
