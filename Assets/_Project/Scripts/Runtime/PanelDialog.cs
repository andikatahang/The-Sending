using System;
using System.Collections;
using System.Text;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Kotak dialog di bawah layar. E atau klik kiri melanjutkan; tombol 1-4 memilih jawaban.
    // Jalan setelah PemainInteraksi: di frame tutup gim masih di-pause saat E dibaca, jadi E tidak membuka dialog lagi.
    [DefaultExecutionOrder(100)]
    public class PanelDialog : MonoBehaviour
    {
        const int MaksPilihan = PemeriksaDialog.MaksPilihan;

        GameObject panel;
        Text pembicara, teks, terjemahan, pilihan;
        JalannyaDialog dialog;
        DataDialog sumber;
        Action saatSelesai;
        int frameMulai;

        public bool Terbuka => dialog != null;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Dialog", 20).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.03f, 0.03f, 0.04f, 0.88f), new Vector2(0.1f, 0.03f), new Vector2(0.9f, 0.33f));
            panel = latar.gameObject;
            pembicara = UiPabrik.BuatTeks(latar.transform, "Pembicara", 30, TextAnchor.UpperLeft, new Vector2(0.03f, 0.78f), new Vector2(0.97f, 0.95f));
            teks = UiPabrik.BuatTeks(latar.transform, "Teks", 30, TextAnchor.UpperLeft, new Vector2(0.03f, 0.45f), new Vector2(0.97f, 0.78f));
            terjemahan = UiPabrik.BuatTeks(latar.transform, "Terjemahan", 24, TextAnchor.UpperLeft, new Vector2(0.03f, 0.3f), new Vector2(0.97f, 0.45f));
            terjemahan.fontStyle = FontStyle.Italic;
            pilihan = UiPabrik.BuatTeks(latar.transform, "Pilihan", 26, TextAnchor.UpperLeft, new Vector2(0.05f, 0.03f), new Vector2(0.97f, 0.3f));
            panel.SetActive(false);
        }

        // Panel dimatikan atau dihancurkan saat dialog terbuka (misalnya ganti scene): lepaskan jeda tanpa memanggil callback.
        void OnDisable()
        {
            if (!Terbuka) return;
            dialog = null;
            saatSelesai = null;
            if (panel != null) panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }

        public bool Mulai(DataDialog data, Action saatSelesai = null)
        {
            if (Terbuka || data == null) return false;
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("PanelDialog butuh GameManager di scene.", this);
                return false;
            }
            sumber = data;
            this.saatSelesai = saatSelesai;
            try
            {
                dialog = new JalannyaDialog(gm.State, data.Baris);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException || e is InvalidOperationException)
            {
                Gagal(e);
                return false;
            }
            frameMulai = Time.frameCount;   // tombol E yang membuka dialog tidak ikut melanjutkannya
            gm.Jeda(true);
            panel.SetActive(true);
            Gambar();
            return true;
        }

        // Untuk adegan berskrip: yield return panel.MulaiDanTunggu(data);
        public IEnumerator MulaiDanTunggu(DataDialog data)
        {
            if (!Mulai(data)) yield break;
            while (Terbuka) yield return null;
        }

        void Update()
        {
            if (!Terbuka || Time.frameCount == frameMulai) return;
            var k = GameManager.Instance.Kontrol;
            try
            {
                if (dialog.MenungguPilihan)
                {
                    int jumlah = Math.Min(dialog.Sekarang.Pilihan.Length, MaksPilihan);
                    for (int i = 0; i < jumlah; i++)
                    {
                        if (!k.PilihanAngka[i].WasPressedThisFrame()) continue;
                        dialog.Pilih(i);
                        break;
                    }
                }
                else if (k.Interaksi.WasPressedThisFrame() || k.Klik.WasPressedThisFrame())
                {
                    dialog.Lanjut();
                }
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException || e is InvalidOperationException)
            {
                Gagal(e);
                return;
            }

            if (dialog.Selesai) Tutup();
            else Gambar();
        }

        void Gambar()
        {
            var b = dialog.Sekarang;
            pembicara.text = b.Pembicara;
            teks.text = b.Teks;
            terjemahan.text = string.IsNullOrEmpty(b.Terjemahan) ? "" : $"({b.Terjemahan})";
            if (!dialog.MenungguPilihan)
            {
                pilihan.text = "[E] Lanjut";
                return;
            }
            if (b.Pilihan.Length > MaksPilihan)
                Debug.LogWarning($"Dialog '{sumber.name}' baris {dialog.Indeks}: {b.Pilihan.Length} pilihan, hanya {MaksPilihan} yang tampil.", sumber);
            var sb = new StringBuilder();
            for (int i = 0; i < b.Pilihan.Length && i < MaksPilihan; i++) sb.AppendLine($"{i + 1}. {b.Pilihan[i].Teks}");
            pilihan.text = sb.ToString();
        }

        void Tutup()
        {
            dialog = null;
            panel.SetActive(false);
            GameManager.Instance.Jeda(false);
            var selesai = saatSelesai;
            saatSelesai = null;
            selesai?.Invoke();
        }

        // Data dialog salah adalah kesalahan penulisan: catat jelas, tutup dialog, gim tetap jalan.
        void Gagal(Exception e)
        {
            Debug.LogError($"Dialog '{(sumber != null ? sumber.name : "?")}' berisi data salah: {e.Message}", sumber);
            if (dialog != null)
            {
                Tutup();
                return;
            }
            var selesai = saatSelesai;
            saatSelesai = null;
            selesai?.Invoke();
        }
    }
}
