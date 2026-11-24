using System;

namespace TheSending.Core
{
    // Malam 1: kode gembok = tahun rumah berdiri, terukir di blandar.
    public static class GembokLemari
    {
        public const string KodeBenar = "2003";
        public static bool Cek(string kode) => kode == KodeBenar;

        // Kode benar: Raka mendapat buku harian ayah (Halaman 1 dan 2, Game Story 10.3) dan map sertifikat.
        // Aman dipanggil ulang: semua langkahnya tidak menggandakan apa pun.
        public static bool Buka(GameState s, string kode)
        {
            if (!Cek(kode)) return false;
            Inventaris.Tambah(s, Inventaris.BukuHarianAyah);
            Inventaris.Tambah(s, Inventaris.MapSertifikat);
            BukuHarian.Temukan(s, Kode.H1);
            BukuHarian.Temukan(s, Kode.H2);
            TujuanMalam.Selesaikan(s, TujuanMalam.BukaLemari);
            return true;
        }
    }

    // Malam 2: satu entri = satu aksara (dengan sandhangan) yang dipilih dari tabel Halaman 2.
    public static class AksaraJawa
    {
        public static readonly string[] Ratmi = { "RA", "T", "MI" };
        public static readonly string[] SelasaKliwon = { "SE", "LA", "SA", "KLI", "WO", "N" };

        public static PuzzleUrutan<string> Buat(string[] kata) => new PuzzleUrutan<string>(kata, false);

        // Empat bacaan untuk tombol 1-4: jawaban dan tiga pengecoh yang bentuk atau bunyinya mirip. Urutan tetap.
        static readonly (string suku, string[] pilihan)[] DaftarPilihan =
        {
            ("RA", new[] { "KA", "RA", "HA", "NA" }),
            ("T", new[] { "N", "K", "T", "D" }),
            ("MI", new[] { "MA", "ME", "NI", "MI" }),
            ("SE", new[] { "SE", "SA", "SI", "LE" }),
            ("LA", new[] { "NA", "LA", "WA", "TA" }),
            ("SA", new[] { "PA", "YA", "SA", "SE" }),
            ("KLI", new[] { "KI", "LI", "KRI", "KLI" }),
            ("WO", new[] { "WA", "WO", "WE", "PO" }),
            ("N", new[] { "NG", "M", "T", "N" }),
        };

        public static string[] Pilihan(string suku)
        {
            foreach (var (s, pilihan) in DaftarPilihan)
                if (s == suku) return (string[])pilihan.Clone();
            throw new ArgumentException($"Suku aksara '{suku}' tidak dikenal.", nameof(suku));
        }
    }

    // Malam 7: salah urutan membuat Mbah Wiryo bangun lebih cepat (diurus Runtime).
    public static class LilinSesaji
    {
        public const float MenitSalah = 15f;   // Malam 7: salah menyalakan lilin memakan menit gim
        public static PuzzleUrutan<Pasaran> Buat(int malam) => new PuzzleUrutan<Pasaran>(KalenderJawa.UrutanLilin(malam), false);
    }

    // Malam 7: ayam cemani berkokok jika Raka dekat kandang; tiap kokok memakan menit gim.
    public static class KokokCemani
    {
        public const float Jarak = 3f, Menit = 5f, JedaDetik = 20f;
        public static bool Berkokok(float jarak) => jarak <= Jarak;
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
