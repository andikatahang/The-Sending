using System;

namespace TheSending.Core
{
    // Game Story 6.1 dan desain 4.1, 4.2. Nama kelas sama dengan field GameState.KondisiIbu;
    // di dalam GameState dipakai literal yang dicek oleh KondisiIbuTests.
    public static class KondisiIbu
    {
        public const int Awal = 60, Min = 0, Maks = 100, MinJalurFitnah = 1;
        public const int PergantianMalam = -12, Obat = 2, MediaDinetralkan = 8, MediaDiserahkan = -8;
        public const int Tertangkap = -5, SalahUrutanPaku = -5;

        public static void Ubah(GameState s, int delta)
        {
            int min = s.JalurFitnah ? MinJalurFitnah : Min;
            s.KondisiIbu = Math.Clamp(s.KondisiIbu + delta, min, Maks);
            s.Umumkan(Perubahan.Kondisi);
            PenentuEnding.Cek(s);
        }

        // Panggil SETELAH checkpoint dimuat, supaya penalti tidak hilang.
        public static void SetelahTertangkap(GameState s) => Ubah(s, Tertangkap);

        // Desain 4.4. Di Malam 7 jalur utama, Runtime memanggil ini saat Laras memberi obat.
        public static bool BeriObat(GameState s)
        {
            if (s.ObatSudahDiberikan || !JamGim.DalamJendelaObat(s)) return false;
            s.ObatSudahDiberikan = true;
            Ubah(s, Obat);
            return true;
        }
    }
}
