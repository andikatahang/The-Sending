namespace TheSending.Core
{
    public static class Checkpoint
    {
        public static void Simpan(string folder, GameState s) => BerkasSave.Tulis(folder, s);

        // Desain 4.2: muat checkpoint, kurangi 5, lalu simpan ulang supaya penalti tidak hilang
        // saat pemain keluar gim. null jika belum ada checkpoint.
        public static GameState MuatSetelahTertangkap(string folder)
        {
            var s = BerkasSave.Baca(folder);
            if (s == null) return null;
            KondisiIbu.SetelahTertangkap(s);
            BerkasSave.Tulis(folder, s);
            return s;
        }
    }
}
