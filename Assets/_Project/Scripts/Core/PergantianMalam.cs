using System;

namespace TheSending.Core
{
    public static class PergantianMalam
    {
        public static bool BisaLanjut(GameState s) => s.Ending == Ending.Belum && TujuanMalam.MalamSelesai(s);

        public static void Lanjut(GameState s)
        {
            if (!BisaLanjut(s)) throw new InvalidOperationException($"Malam {s.MalamKe} belum bisa ditutup.");

            bool dariProlog = s.MalamKe == 0;
            s.MalamKe++;
            s.ObatSudahDiberikan = false;
            s.TujuanSelesai.Clear();
            PetaScene.CatatAsal(s, ""); // asal kunjungan malam lalu tidak boleh melewati pembuka malam baru
            // Prolog bersambung ke Malam 1 pada sore yang sama, jadi jam dan kondisi tidak berubah.
            if (!dariProlog) s.MenitGim = JamGim.MulaiMalam;
            s.Umumkan(Perubahan.Malam);
            if (!dariProlog) KondisiIbu.Ubah(s, KondisiIbu.PergantianMalam);
        }
    }
}
