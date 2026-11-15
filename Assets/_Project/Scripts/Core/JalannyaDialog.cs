using System;
using System.Collections.Generic;

namespace TheSending.Core
{
    // Alur satu percakapan. Baris berjalan berurutan; baris yang punya pilihan menunggu Pilih().
    public class JalannyaDialog
    {
        readonly IReadOnlyList<BarisDialog> baris;
        readonly GameState s;

        public int Indeks { get; private set; }
        public bool Selesai { get; private set; }
        public BarisDialog Sekarang => Selesai ? null : baris[Indeks];
        public bool MenungguPilihan => !Selesai && baris[Indeks].Pilihan != null && baris[Indeks].Pilihan.Length > 0;

        public JalannyaDialog(GameState s, IReadOnlyList<BarisDialog> baris)
        {
            if (baris == null || baris.Count == 0) throw new ArgumentException("Dialog kosong.", nameof(baris));
            this.s = s ?? throw new ArgumentNullException(nameof(s));
            this.baris = baris;
            Masuk(0);
        }

        public void Lanjut()
        {
            if (Selesai) return;
            if (MenungguPilihan) throw new InvalidOperationException("Baris ini menunggu pilihan.");
            if (baris[Indeks].Akhir || Indeks + 1 >= baris.Count) Selesai = true;
            else Masuk(Indeks + 1);
        }

        public void Pilih(int nomor)
        {
            if (!MenungguPilihan) throw new InvalidOperationException("Tidak ada pilihan di baris ini.");
            var pilihan = baris[Indeks].Pilihan;
            if (nomor < 0 || nomor >= pilihan.Length) throw new ArgumentOutOfRangeException(nameof(nomor));
            var p = pilihan[nomor];
            EfekDialog.Jalankan(s, p.Efek);
            if (p.LompatKe < 0) Selesai = true;
            else Masuk(p.LompatKe);
        }

        void Masuk(int i)
        {
            if (i < 0 || i >= baris.Count) throw new ArgumentOutOfRangeException(nameof(i), i, "Lompatan dialog di luar daftar baris.");
            Indeks = i;
            EfekDialog.Jalankan(s, baris[i].Efek);
        }
    }
}
