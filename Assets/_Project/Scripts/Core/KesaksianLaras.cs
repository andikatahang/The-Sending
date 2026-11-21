namespace TheSending.Core
{
    // Game Story Malam 5: dibentak, Laras menangis dan mengunci diri. Raka bisa mencoba lagi setelah membawakan teh hangat.
    public static class KesaksianLaras
    {
        public static void Bentak(GameState s)
        {
            if (s.LarasMengunciDiri) return;
            s.LarasMengunciDiri = true;
            s.Umumkan(Perubahan.Tujuan);
        }

        public static bool BisaBicara(GameState s) => !s.LarasMengunciDiri || Inventaris.Punya(s, Inventaris.TehHangat);

        // true jika teh diberikan: teh habis dan Laras membuka pintu.
        public static bool BeriTeh(GameState s)
        {
            if (!s.LarasMengunciDiri || !Inventaris.Punya(s, Inventaris.TehHangat)) return false;
            s.Barang.Remove(Inventaris.TehHangat);
            s.LarasMengunciDiri = false;
            s.Umumkan(Perubahan.Barang);
            return true;
        }
    }
}
