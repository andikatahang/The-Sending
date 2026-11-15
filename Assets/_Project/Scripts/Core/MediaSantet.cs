using System;

namespace TheSending.Core
{
    // Game Story 6.2. Netralkan dipanggil oleh RitualPenangkal setelah enam langkah benar.
    public static class MediaSantet
    {
        public static void Temukan(GameState s, int m)
        {
            CekIndeks(m);
            if (s.MediaDitemukan[m]) return;
            s.MediaDitemukan[m] = true;
            s.MalamMediaDitemukan[m] = s.MalamKe;
            s.Umumkan(Perubahan.Media);
        }

        public static bool BisaDiserahkan(GameState s, int m) => BisaDinetralkan(s, m);

        public static void Serahkan(GameState s, int m)
        {
            if (!BisaDiserahkan(s, m)) throw new InvalidOperationException($"Media {m + 1} tidak bisa diserahkan.");
            s.MediaDiserahkan[m] = true;
            s.Umumkan(Perubahan.Media);
            KondisiIbu.Ubah(s, KondisiIbu.MediaDiserahkan);
        }

        public static bool BisaDinetralkan(GameState s, int m)
        {
            CekIndeks(m);
            return s.MediaDitemukan[m] && !s.MediaDinetralkan[m] && !s.MediaDiserahkan[m];
        }

        public static void Netralkan(GameState s, int m)
        {
            if (!BisaDinetralkan(s, m)) throw new InvalidOperationException($"Media {m + 1} tidak bisa dinetralkan.");
            s.MediaDinetralkan[m] = true;
            if (m == Kode.M7) s.RitualTerakhirSelesai = true;
            s.Umumkan(Perubahan.Media);
            KondisiIbu.Ubah(s, KondisiIbu.MediaDinetralkan);
        }

        static void CekIndeks(int m)
        {
            if (m < 0 || m >= Kode.JumlahMedia) throw new ArgumentOutOfRangeException(nameof(m), m, "Media harus 0..6.");
        }
    }
}
