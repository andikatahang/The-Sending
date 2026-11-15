using System.Collections.Generic;

namespace TheSending.Core
{
    // Tujuan wajib per malam (Game Story bab 7). Kejadian berwaktu termasuk tujuan wajib.
    // Malam 7 berakhir lewat PenentuEnding, bukan lewat tujuan.
    public static class TujuanMalam
    {
        public const string TibaDiRumah = "TibaDiRumah", BicaraBidan = "BicaraBidan", MasukKamarIbu = "MasukKamarIbu";
        public const string BukaLemari = "BukaLemari", Cutscene23 = "Cutscene23", PotretPaku = "PotretPaku";
        public const string BacaAksara = "BacaAksara", PilihanWiryo = "PilihanWiryo", SembunyiPertama = "SembunyiPertama";
        public const string CabutPaku = "CabutPaku", KunjungiSumi = "KunjungiSumi";
        public const string AmbilBungkusanSumur = "AmbilBungkusanSumur", LolosKejaranSumur = "LolosKejaranSumur";
        public const string GaliKendi = "GaliKendi", BandingkanTanah = "BandingkanTanah", BicaraLaras = "BicaraLaras";
        public const string SidangWarga = "SidangWarga";

        static readonly string[][] Wajib =
        {
            new[] { TibaDiRumah, BicaraBidan, MasukKamarIbu },        // Prolog
            new[] { BukaLemari, Cutscene23, PotretPaku },             // Malam 1
            new[] { BacaAksara, PilihanWiryo, SembunyiPertama },      // Malam 2
            new[] { CabutPaku, KunjungiSumi },                        // Malam 3
            new[] { AmbilBungkusanSumur, LolosKejaranSumur },         // Malam 4
            new[] { GaliKendi, BandingkanTanah, BicaraLaras },        // Malam 5
            new[] { SidangWarga },                                    // Malam 6
            new string[0],                                            // Malam 7
        };

        public static IReadOnlyList<string> DaftarWajib(int malam) => Wajib[malam];

        public static bool Sudah(GameState s, string id) => s.TujuanSelesai.Contains(id);

        public static void Selesaikan(GameState s, string id)
        {
            if (s.TujuanSelesai.Contains(id)) return;
            s.TujuanSelesai.Add(id);
            s.Umumkan(Perubahan.Tujuan);
        }

        public static bool MalamSelesai(GameState s)
        {
            if (s.MalamKe == 7) return false;
            foreach (var id in Wajib[s.MalamKe])
                if (!s.TujuanSelesai.Contains(id)) return false;
            return true;
        }
    }
}
