namespace TheSending.Core
{
    // Malam 7 jalur utama (sidang berhasil): obat Ibu diberikan otomatis pukul 21.00, sekali.
    public static class ObatMalam7
    {
        public static bool Cek(GameState s)
        {
            if (s.MalamKe != 7 || s.HasilSidang != HasilSidang.Berhasil || s.ObatSudahDiberikan
                || s.MenitGim < JamGim.Jam21 || s.Ending != Ending.Belum) return false;
            KondisiIbu.Ubah(s, KondisiIbu.Obat);
            s.ObatSudahDiberikan = true;
            return true;
        }
    }
}
