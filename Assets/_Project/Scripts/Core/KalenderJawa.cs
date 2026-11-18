using System;

namespace TheSending.Core
{
    // Pergantian hari Jawa dimulai saat magrib. Prolog dan Malam 1 = malam Rabu Wage (Game Story 3.1).
    public static class KalenderJawa
    {
        const int JumlahHari = 7, JumlahPasaran = 5;

        public static Hari HariMalam(int malam) =>
            (Hari)(((int)Hari.Rabu + Urutan(malam)) % JumlahHari);

        public static Pasaran PasaranMalam(int malam) =>
            (Pasaran)(((int)Pasaran.Wage + Urutan(malam)) % JumlahPasaran);

        public static string NamaMalam(int malam) => $"Malam {HariMalam(malam)} {PasaranMalam(malam)}";

        public static Hari[] UrutanPaku(int malam)
        {
            var hasil = new Hari[JumlahHari];
            int mulai = (int)HariMalam(malam);
            for (int i = 0; i < JumlahHari; i++) hasil[i] = (Hari)((mulai + i) % JumlahHari);
            return hasil;
        }

        public static Pasaran[] UrutanLilin(int malam)
        {
            var hasil = new Pasaran[JumlahPasaran];
            int mulai = (int)PasaranMalam(malam);
            for (int i = 0; i < JumlahPasaran; i++) hasil[i] = (Pasaran)((mulai + i) % JumlahPasaran);
            return hasil;
        }

        // Weton Ibu (Selasa Kliwon) jatuh pada Malam 7 (Game Story Malam 2 butir 6).
        public static int SisaMalamKeSelasaKliwon(int malam) => 6 - Urutan(malam);

        // Hitung mundur berjalan sejak tulisan boneka terbaca di Malam 2.
        public static bool HitungMundurBerjalan(GameState s) =>
            s.MalamKe >= 3 || (s.MalamKe == 2 && TujuanMalam.Sudah(s, TujuanMalam.BacaAksara));

        // Jarak (dalam malam) dari Malam 1. Prolog dihitung sama dengan Malam 1.
        static int Urutan(int malam)
        {
            if (malam < 0 || malam > 7) throw new ArgumentOutOfRangeException(nameof(malam), malam, "Malam harus 0..7.");
            return Math.Max(malam, 1) - 1;
        }
    }
}
