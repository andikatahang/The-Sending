using System;

namespace TheSending.Core
{
    // Desain 4.3 dan 4.4. Jam disimpan sebagai menit sejak 00.00 hari itu; lewat tengah malam lanjut di atas 1440.
    // Runtime tidak memanggil Maju saat cutscene, dialog, Jurnal, inventaris, atau jeda terbuka.
    public static class JamGim
    {
        public const float DetikNyataPerJamGim = 125f;
        public const float DetikNyataPerJamGimMalam6 = 250f;   // K41-A: 18.00-21.00 sekitar 12,5 menit nyata
        public const float DetikNyataPerJamGimMalam7 = 250f;
        public static float DetikPerJamGim(int malam) => malam == 6 ? DetikNyataPerJamGimMalam6 : malam == 7 ? DetikNyataPerJamGimMalam7 : DetikNyataPerJamGim;
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
            Majukan(s, detikNyata * 60f / DetikPerJamGim(s.MalamKe));
        }

        // Tambah menit gim langsung (mis. lilin salah, kokok cemani), dijepit ke batas malam.
        public static void Majukan(GameState s, float menit)
        {
            if (menit <= 0f) return;
            float baru = Math.Min(s.MenitGim + menit, Batas(s.MalamKe));
            if (baru <= s.MenitGim) return;
            s.MenitGim = baru;
            s.Umumkan(Perubahan.Jam);
            if (s.MalamKe == 7)
            {
                ObatMalam7.Cek(s);
                PenentuEnding.Cek(s);
            }
        }

        // Malam 6: warga berkumpul (Sidang). Malam 7: tengah malam.
        public static bool BatasKerasTercapai(GameState s) =>
            (s.MalamKe == 6 || s.MalamKe == 7) && s.MenitGim >= Batas(s.MalamKe);

        // Desain 4.4: 21.00-22.00, Malam 6 dimajukan ke 20.00-21.00.
        public static float MulaiJendelaObat(int malam) => malam == 6 ? Jam20 : Jam21;
        public static float AkhirJendelaObat(int malam) => MulaiJendelaObat(malam) + LamaJendelaObat;

        public static bool DalamJendelaObat(GameState s) =>
            s.MalamKe >= 1 && s.MenitGim >= MulaiJendelaObat(s.MalamKe) && s.MenitGim < AkhirJendelaObat(s.MalamKe);

        // "Duduk menunggu" melompat ke kejadian berwaktu berikutnya.
        // Malam 1 (K4 opsi B): obat 21.00, cutscene 23.00. Malam 2 (K11-A): setelah tulisan boneka terbaca dan sebelum
        // Mbah Wiryo datang, menunggu jam obat 21.00. Malam 3 (K19-A): setelah semua tujuan wajib selesai, menunggu jam obat.
        // Null jika tidak ada yang ditunggu, atau obat sedang harus diberikan.
        public static float? KejadianBerikutnya(GameState s)
        {
            if (s.MalamKe == 6) return TungguMalam6(s);
            if (s.MalamKe == 2) return TungguMalam2(s);
            if (s.MalamKe >= 3 && s.MalamKe <= 5) return TujuanMalam.MalamSelesai(s) && !ObatSelesai(s) && s.MenitGim < Jam21 ? Jam21 : (float?)null;
            if (s.MalamKe != 1 || TujuanMalam.Sudah(s, TujuanMalam.Cutscene23)) return null;
            if (s.MenitGim < Jam21) return Jam21;
            if (DalamJendelaObat(s) && !s.ObatSudahDiberikan) return null;
            return s.MenitGim < Jam23 ? Jam23 : (float?)null;
        }

        // K41-A: kursi di Malam 6 menunggu jam obat 20.00, lalu (setelah obat) pukul 21.00 saat warga berkumpul.
        static float? TungguMalam6(GameState s)
        {
            if (s.HasilSidang != HasilSidang.Belum || s.MenitGim >= Jam21) return null;
            if (s.MenitGim < Jam20) return Jam20;
            return s.ObatSudahDiberikan ? Jam21 : (float?)null;
        }

        static float? TungguMalam2(GameState s) =>
            TujuanMalam.Sudah(s, TujuanMalam.BacaAksara) && !TujuanMalam.Sudah(s, TujuanMalam.PilihanWiryo) && s.MenitGim < Jam21
                ? Jam21 : (float?)null;

        // K11-A: Mbah Wiryo mengetuk pintu setelah tulisan boneka terbaca dan obat sudah diberikan (atau jam obat lewat).
        public static bool SaatnyaWiryoDatang(GameState s) =>
            s.MalamKe == 2 && TujuanMalam.Sudah(s, TujuanMalam.BacaAksara) && !TujuanMalam.Sudah(s, TujuanMalam.PilihanWiryo)
            && (s.ObatSudahDiberikan || s.MenitGim >= AkhirJendelaObat(2));

        // K19-A: malam tidak ditutup sebelum obat diberikan atau jendela obatnya lewat.
        public static bool ObatSelesai(GameState s) => s.ObatSudahDiberikan || s.MenitGim >= AkhirJendelaObat(s.MalamKe);

        // Malam 3 (Game Story): Ibu menjerit setiap kali jam dinding berdentang, yaitu setiap jam penuh.
        public static bool JamBerdentang(float sebelum, float sesudah) => Math.Floor(sesudah / 60f) > Math.Floor(sebelum / 60f);

        public static bool Tunggu(GameState s)
        {
            var sampai = KejadianBerikutnya(s);
            if (sampai == null) return false;
            s.MenitGim = sampai.Value;
            s.Umumkan(Perubahan.Jam);
            return true;
        }

        public static float SisaDetikMalam7(GameState s) =>
            s.MalamKe != 7 ? 0f : Math.Max(0f, (TengahMalam - s.MenitGim) * DetikPerJamGim(7) / 60f);

        public static string Format(float menit)
        {
            int m = (int)menit % (24 * 60);
            return $"{m / 60:00}.{m % 60:00}";
        }
    }
}
