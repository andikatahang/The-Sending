using System;
using System.Text;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Puzzle aksara Jawa (Game Story Malam 2): tulisan di boneka dibaca suku demi suku dengan tabel Halaman 2.
    // Tombol 1-4 memilih bacaan suku yang ditandai. Esc menutup tanpa kehilangan kemajuan (dibuka lagi dari boneka).
    // Gambar aksara belum ada (tools/render-aksara.sh belum dijalankan): suku tampil sebagai bacaannya dalam kurung
    // siku dan tabel sebagai teks, supaya alur tetap bisa diuji.
    public class PanelAksara : MonoBehaviour
    {
        static readonly Color WarnaTanda = new Color(0.353f, 0.102f, 0.071f, 1f);   // karat #5A1A12
        static readonly Color WarnaKotak = new Color(0.12f, 0.1f, 0.08f, 1f);
        const string Petunjuk = "[1-4] pilih bacaan aksara yang ditandai     [Esc] tutup";
        const string TabelPengganti =
            "ha  na  ca  ra  ka\nda  ta  sa  wa  la\npa  dha  ja  ya  nya\nma  ga  ba  tha  nga\n\n" +
            "wulu = i, taling = e, taling tarung = o,\npangkon = mati, pasangan = dua konsonan\n\n(gambar tabel belum dipasang)";

        [SerializeField] Texture2D tabel;
        [SerializeField] Texture2D[] gambarSuku = new Texture2D[0];   // urutan BacaanAksara.Kata, 9 gambar

        GameObject panel;
        Image[] bingkai;
        Text[] bacaanSuku;
        Text pilihan, pesan;
        readonly BacaanAksara bacaan = new BacaanAksara();
        string[] pilihanSekarang = new string[0];
        Action saatSelesai;
        int frameBuka;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Aksara", 16).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.05f, 0.04f, 0.03f, 0.96f), new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.92f));
            panel = latar.gameObject;
            UiPabrik.BuatTeks(latar.transform, "Judul", 30, TextAnchor.UpperLeft, new Vector2(0.03f, 0.9f), new Vector2(0.55f, 0.98f)).text =
                "TULISAN DI BONEKA";

            var min = new Vector2(0.58f, 0.04f);
            var maks = new Vector2(0.97f, 0.96f);
            if (tabel != null) Gambar(latar.transform, "Tabel", tabel, min, maks);
            else UiPabrik.BuatTeks(latar.transform, "TabelPengganti", 26, TextAnchor.UpperLeft, min, maks).text = TabelPengganti;

            bingkai = new Image[BacaanAksara.JumlahSuku];
            bacaanSuku = new Text[BacaanAksara.JumlahSuku];
            int i = 0;
            for (int k = 0; k < BacaanAksara.Kata.Length; k++)
                for (int n = 0; n < BacaanAksara.Kata[k].Length; n++, i++)
                    Suku(latar.transform, i, k, n, BacaanAksara.Kata[k][n]);

            pilihan = UiPabrik.BuatTeks(latar.transform, "Pilihan", 30, TextAnchor.UpperLeft, new Vector2(0.03f, 0.14f), new Vector2(0.55f, 0.34f));
            pesan = UiPabrik.BuatTeks(latar.transform, "Pesan", 24, TextAnchor.UpperLeft, new Vector2(0.03f, 0.03f), new Vector2(0.55f, 0.13f));
            panel.SetActive(false);
        }

        // Kata pertama di baris atas, kata kedua di baris bawah. Kotak berisi gambar aksara; bacaannya tampil di bawah kotak.
        void Suku(Transform induk, int i, int kata, int n, string suku)
        {
            float x = 0.03f + n * 0.088f, atas = kata == 0 ? 0.86f : 0.6f;
            bingkai[i] = UiPabrik.BuatPanel(induk, "Suku " + i, WarnaKotak, new Vector2(x, atas - 0.16f), new Vector2(x + 0.08f, atas));
            if (i < gambarSuku.Length && gambarSuku[i] != null)
                Gambar(bingkai[i].transform, "Aksara", gambarSuku[i], new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.95f));
            else
                UiPabrik.BuatTeks(bingkai[i].transform, "Pengganti", 28, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one).text = $"[{suku}]";
            bacaanSuku[i] = UiPabrik.BuatTeks(induk, "Bacaan " + i, 26, TextAnchor.UpperCenter, new Vector2(x, atas - 0.22f), new Vector2(x + 0.08f, atas - 0.16f));
        }

        static void Gambar(Transform induk, string nama, Texture tekstur, Vector2 min, Vector2 maks)
        {
            var go = new GameObject(nama, typeof(RectTransform), typeof(RawImage));
            go.transform.SetParent(induk, false);
            UiPabrik.Isi((RectTransform)go.transform, min, maks);
            var g = go.GetComponent<RawImage>();
            g.texture = tekstur;
            g.raycastTarget = false;
        }

        public void Buka(Action selesai)
        {
            var gm = GameManager.Instance;
            if (Terbuka || gm == null || gm.Dijeda || bacaan.Selesai) return;
            saatSelesai = selesai;
            frameBuka = Time.frameCount;   // tombol E yang membuka panel tidak ikut terbaca
            pesan.text = Petunjuk;
            panel.SetActive(true);
            gm.Jeda(true);
            Perbarui();
        }

        void Update()
        {
            if (!Terbuka || Time.frameCount == frameBuka) return;
            var gm = GameManager.Instance;
            if (gm == null) return;
            var k = gm.Kontrol;
            if (k.JedaMenu.WasPressedThisFrame())
            {
                Tutup();
                return;
            }
            for (int i = 0; i < pilihanSekarang.Length && i < k.PilihanAngka.Length; i++)
            {
                if (!k.PilihanAngka[i].WasPressedThisFrame()) continue;
                Pilih(pilihanSekarang[i]);
                return;
            }
        }

        void Pilih(string suku)
        {
            if (!bacaan.Pilih(suku))
            {
                pesan.text = "Bukan itu. Cocokkan bentuknya dengan tabel.\n" + Petunjuk;
                return;
            }
            pesan.text = Petunjuk;
            Perbarui();
            if (!bacaan.Selesai) return;
            Tutup();
            var selesai = saatSelesai;
            saatSelesai = null;
            selesai?.Invoke();
        }

        void Perbarui()
        {
            int i = 0;
            foreach (var kata in BacaanAksara.Kata)
                foreach (var suku in kata)
                {
                    bingkai[i].color = i == bacaan.Terbaca ? WarnaTanda : WarnaKotak;
                    bacaanSuku[i].text = i < bacaan.Terbaca ? suku : "";
                    i++;
                }
            if (bacaan.Selesai)
            {
                pilihanSekarang = new string[0];
                pilihan.text = "";
                return;
            }
            pilihanSekarang = AksaraJawa.Pilihan(bacaan.SukuSekarang);
            var sb = new StringBuilder("Aksara yang ditandai dibaca:\n");
            for (int n = 0; n < pilihanSekarang.Length; n++) sb.Append($"{n + 1}. {pilihanSekarang[n]}     ");
            pilihan.text = sb.ToString();
        }

        // Panel dimatikan atau dihancurkan saat terbuka (misalnya ganti scene): lepaskan jeda.
        void OnDisable() => Tutup();

        void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }
    }
}
