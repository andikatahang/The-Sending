using System;

namespace TheSending.Core
{
    public readonly struct ItemPapan
    {
        public readonly bool IsKatanya;
        public readonly int Indeks;

        ItemPapan(bool isKatanya, int indeks)
        {
            IsKatanya = isKatanya;
            Indeks = indeks;
        }

        public static ItemPapan Bukti(int b) => new ItemPapan(false, b);
        public static ItemPapan Katanya(int k) => new ItemPapan(true, k);
    }

    // Game Story Malam 6 dan desain 4.8.
    public static class SidangWarga
    {
        public const int JumlahTuduhan = 3, MinimalBenar = 2;

        static readonly int[][] BuktiValid =
        {
            new[] { Kode.B4, Kode.B5 },             // "Rambut di sumur itu rambut Mbok Sumi!"
            new[] { Kode.B1, Kode.B6, Kode.B7 },    // "Dia dendam sama keluargamu..."
            new[] { Kode.B2, Kode.B3 },             // "Siapa lagi yang ngerti ilmu begituan...?"
        };

        public static bool JawabanBenar(GameState s, int tuduhan, ItemPapan item)
        {
            if (tuduhan < 0 || tuduhan >= JumlahTuduhan) throw new ArgumentOutOfRangeException(nameof(tuduhan));
            if (item.IsKatanya) return false;
            if (item.Indeks < 0 || item.Indeks >= Kode.JumlahBukti) throw new ArgumentOutOfRangeException(nameof(item));
            return s.BuktiKunci[item.Indeks] && Array.IndexOf(BuktiValid[tuduhan], item.Indeks) >= 0;
        }

        public static HasilSidang Selesaikan(GameState s, int jumlahBenar) =>
            Tetapkan(s, jumlahBenar >= MinimalBenar ? HasilSidang.Berhasil : HasilSidang.Gagal);

        public static HasilSidang Diam(GameState s) => Tetapkan(s, HasilSidang.Diam);
        public static HasilSidang IkutMenuduh(GameState s) => Tetapkan(s, HasilSidang.Menuduh);

        static HasilSidang Tetapkan(GameState s, HasilSidang hasil)
        {
            if (s.HasilSidang != HasilSidang.Belum) throw new InvalidOperationException("Sidang Warga sudah selesai.");
            s.HasilSidang = hasil;
            s.Umumkan(Perubahan.Sidang);
            return hasil;
        }
    }
}
