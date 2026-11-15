using System;

namespace TheSending.Core
{
    // Desain 4.3 dan 4.4. Jam disimpan sebagai menit sejak 00.00 hari itu; lewat tengah malam lanjut di atas 1440.
    // Runtime tidak memanggil Maju saat cutscene, dialog, Jurnal, inventaris, atau jeda terbuka.
    public static class JamGim
    {
        public const float DetikNyataPerJamGim = 250f;
        public const float MulaiProlog = 17 * 60 + 30f;
        public const float MulaiMalam = 18 * 60f;
        public const float Jam20 = 20 * 60f, Jam21 = 21 * 60f, Jam22 = 22 * 60f, Jam23 = 23 * 60f;
        public const float TengahMalam = 24 * 60f;
        public const float BatasLunak = 24 * 60 + 30f;
        const float LamaJendelaObat = 60f;

        public static float Batas(int malam) => malam == 6 ? Jam21 : malam == 7 ? TengahMalam : BatasLunak;

        public static void Maju(GameState s, float detikNyata)
        {
            if (detikNyata <= 0f) return;
            float baru = Math.Min(s.MenitGim + detikNyata * 60f / DetikNyataPerJamGim, Batas(s.MalamKe));
            if (baru <= s.MenitGim) return;
            s.MenitGim = baru;
            s.Umumkan(Perubahan.Jam);
            if (s.MalamKe == 7) PenentuEnding.Cek(s);
        }

        // Malam 6: warga berkumpul (Sidang). Malam 7: tengah malam.
        public static bool BatasKerasTercapai(GameState s) =>
            (s.MalamKe == 6 || s.MalamKe == 7) && s.MenitGim >= Batas(s.MalamKe);

        public static bool DalamJendelaObat(GameState s)
        {
            if (s.MalamKe < 1) return false;
            float mulai = s.MalamKe == 6 ? Jam20 : Jam21;
            return s.MenitGim >= mulai && s.MenitGim < mulai + LamaJendelaObat;
        }

        public static float SisaDetikMalam7(GameState s) =>
            s.MalamKe != 7 ? 0f : Math.Max(0f, (TengahMalam - s.MenitGim) * DetikNyataPerJamGim / 60f);

        public static string Format(float menit)
        {
            int m = (int)menit % (24 * 60);
            return $"{m / 60:00}.{m % 60:00}";
        }
    }
}
