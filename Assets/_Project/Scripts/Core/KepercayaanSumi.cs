using System;

namespace TheSending.Core
{
    // Game Story 6.4. Kelas ini hanya menyimpan nilai dan gerbang; hadiahnya diberikan lewat dialog di Runtime.
    public static class KepercayaanSumi
    {
        public const int Maks = 3, AmbangRitual = 1, AmbangKesaksian = 2, AmbangMinyakSereh = 3;

        public static bool Tambah(GameState s, AksiSumi aksi)
        {
            int i = (int)aksi;
            if (s.AksiSumiDipakai[i]) return false;
            s.AksiSumiDipakai[i] = true;
            s.KepercayaanSumi = Math.Min(Maks, s.KepercayaanSumi + 1);
            s.Umumkan(Perubahan.Kepercayaan);
            return true;
        }

        public static void Tuduh(GameState s)
        {
            s.KepercayaanSumi = Math.Max(0, s.KepercayaanSumi - 1);
            s.PintuSumiTerbukaMulaiMalam = s.MalamKe + 1;
            s.Umumkan(Perubahan.Kepercayaan);
        }

        public static bool PintuTerbuka(GameState s) => s.MalamKe >= s.PintuSumiTerbukaMulaiMalam;
        public static bool BisaAjarkanRitual(GameState s) => s.KepercayaanSumi >= AmbangRitual;
        public static bool BisaBeriKesaksian(GameState s) => s.KepercayaanSumi >= AmbangKesaksian;
        public static bool BisaBeriMinyakSereh(GameState s) => s.KepercayaanSumi >= AmbangMinyakSereh;

        public static void TerimaMinyakSereh(GameState s)
        {
            if (!BisaBeriMinyakSereh(s)) throw new InvalidOperationException("Kepercayaan Bu Sumi belum 3.");
            s.PunyaMinyakSereh = true;
            s.Umumkan(Perubahan.Kepercayaan);
        }
    }
}
