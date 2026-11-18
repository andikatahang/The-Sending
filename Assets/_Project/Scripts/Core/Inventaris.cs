using System;

namespace TheSending.Core
{
    // Barang yang dibawa Raka (tombol I). Barang tidak habis dipakai: obat dari Bidan Wulan cukup untuk semua malam.
    public static class Inventaris
    {
        public const string ObatIbu = "ObatIbu", BukuHarianAyah = "BukuHarianAyah", MapSertifikat = "MapSertifikat";
        public const string KunciGudang = "KunciGudang", Linggis = "Linggis";

        // id, nama, keterangan
        static readonly string[][] Daftar =
        {
            new[] { ObatIbu, "Obat dan oralit", "Dari Bidan Wulan. Diminumkan kepada Ibu pukul 21.00." },
            new[] { BukuHarianAyah, "Buku harian Bapak", "Banyak halamannya sobek. Halaman yang sudah ditemukan bisa dibaca di Jurnal." },
            new[] { MapSertifikat, "Map fotokopi sertifikat tanah", "Fotokopi sertifikat tanah keluarga di tepi sungai." },
            new[] { KunciGudang, "Kunci gudang", "Dari Laras. Membuka pintu gudang di sebelah dapur." },
            new[] { Linggis, "Linggis", "Dari gudang. Cukup kuat untuk membongkar papan lantai." },
        };

        public static bool Dikenal(string id) => Cari(id) != null;
        public static string Nama(string id) => Wajib(id)[1];
        public static string Keterangan(string id) => Wajib(id)[2];
        public static bool Punya(GameState s, string id) => s.Barang.Contains(id);

        public static void Tambah(GameState s, string id)
        {
            Wajib(id);
            if (s.Barang.Contains(id)) return;
            s.Barang.Add(id);
            s.Umumkan(Perubahan.Barang);
        }

        static string[] Cari(string id)
        {
            foreach (var b in Daftar)
                if (b[0] == id) return b;
            return null;
        }

        static string[] Wajib(string id) =>
            Cari(id) ?? throw new ArgumentException($"Barang '{id}' tidak dikenal.", nameof(id));
    }
}
