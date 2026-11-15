namespace TheSending.Core
{
    // Aturan ending final, dokumen desain Bagian 4.1.
    public static class PenentuEnding
    {
        public const float TengahMalam = 24 * 60f;

        public static Ending Hitung(GameState s)
        {
            if (s.Ending != Ending.Belum) return s.Ending;

            bool waktuHabis = s.MalamKe == 7 && s.MenitGim >= TengahMalam;

            if (s.JalurFitnah) return waktuHabis ? Ending.Fitnah : Ending.Belum;
            if (s.KondisiIbu <= 0) return Ending.Terlambat;
            if (waktuHabis) return Ending.Terlambat;
            if (s.PilihanAkhir == PilihanAkhir.Balas && s.LolosKejaranWiryo) return Ending.BalasDendam;
            if (s.RitualTerakhirSelesai && s.PilihanAkhir == PilihanAkhir.Lapor && s.HasilSidang == HasilSidang.Berhasil)
                return Ending.Sejati;
            return Ending.Belum;
        }

        // Dipanggil setiap ada perubahan yang bisa memicu ending. Ending pertama yang tercapai bersifat tetap.
        public static Ending Cek(GameState s)
        {
            var e = Hitung(s);
            if (e != Ending.Belum && s.Ending == Ending.Belum)
            {
                s.Ending = e;
                s.Umumkan(Perubahan.Ending);
            }
            return e;
        }
    }
}
