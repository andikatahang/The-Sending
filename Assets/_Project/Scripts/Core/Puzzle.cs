using System;

namespace TheSending.Core
{
    // Malam 1: kode gembok = tahun rumah berdiri, terukir di blandar.
    public static class GembokLemari
    {
        public const string KodeBenar = "2003";
        public static bool Cek(string kode) => kode == KodeBenar;
    }

    // Malam 2: satu entri = satu aksara (dengan sandhangan) yang dipilih dari tabel Halaman 2.
    public static class AksaraJawa
    {
        public static readonly string[] Ratmi = { "RA", "T", "MI" };
        public static readonly string[] SelasaKliwon = { "SE", "LA", "SA", "KLI", "WO", "N" };

        public static PuzzleUrutan<string> Buat(string[] kata) => new PuzzleUrutan<string>(kata, false);
    }

    // Malam 3: salah urutan tidak mengulang, tetapi Runtime memberi penalti KondisiIbu.SalahUrutanPaku.
    public static class UrutanPaku
    {
        public static PuzzleUrutan<Hari> Buat(int malam) => new PuzzleUrutan<Hari>(KalenderJawa.UrutanPaku(malam), false);
    }

    // Malam 7: salah urutan membuat Mbah Wiryo bangun lebih cepat (diurus Runtime).
    public static class LilinSesaji
    {
        public static PuzzleUrutan<Pasaran> Buat(int malam) => new PuzzleUrutan<Pasaran>(KalenderJawa.UrutanLilin(malam), false);
    }

    // Malam 4: tali yang ditarik lebih cepat dari batas membuat bungkusan jatuh kembali.
    public static class KerekanSumur
    {
        public const float KecepatanTarikMaks = 0.6f; // meter per detik, disetel saat playtest
        public static bool TarikanAman(float kecepatanTarik) => kecepatanTarik <= KecepatanTarikMaks;
    }

    // Malam 5: tiga sampel tanah makam dibandingkan dengan tanah kendi, tidak ada yang cocok.
    public class PerbandinganTanah
    {
        public const int JumlahTitik = 3;
        readonly bool[] sudah = new bool[JumlahTitik];

        public bool SemuaSudahDibandingkan => Array.TrueForAll(sudah, x => x);

        public bool Bandingkan(int titik)
        {
            if (titik < 0 || titik >= JumlahTitik) throw new ArgumentOutOfRangeException(nameof(titik));
            sudah[titik] = true;
            return false; // tanah makam Sumberwangi hitam berpasir, tidak pernah cocok
        }
    }
}
