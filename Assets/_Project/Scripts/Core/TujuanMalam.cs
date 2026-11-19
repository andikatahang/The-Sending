using System;
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
        public const string PakdeDatang = "PakdeDatang", CabutPaku = "CabutPaku", KunjungiSumi = "KunjungiSumi";
        public const string RitualPertama = "RitualPertama";   // K20-A: tutorial Ritual Penangkal wajib
        public const string AmbilBungkusanSumur = "AmbilBungkusanSumur", LolosKejaranSumur = "LolosKejaranSumur";
        public const string GaliKendi = "GaliKendi", BandingkanTanah = "BandingkanTanah", BicaraLaras = "BicaraLaras";
        public const string SidangWarga = "SidangWarga";

        static readonly string[][] Wajib =
        {
            new[] { TibaDiRumah, BicaraBidan, MasukKamarIbu },        // Prolog
            new[] { BukaLemari, Cutscene23, PotretPaku },             // Malam 1
            new[] { BacaAksara, PilihanWiryo, SembunyiPertama },      // Malam 2
            new[] { PakdeDatang, CabutPaku, KunjungiSumi, RitualPertama },   // Malam 3
            new[] { AmbilBungkusanSumur, LolosKejaranSumur },         // Malam 4
            new[] { GaliKendi, BandingkanTanah, BicaraLaras },        // Malam 5
            new[] { SidangWarga },                                    // Malam 6
            new string[0],                                            // Malam 7
        };

        public static IReadOnlyList<string> DaftarWajib(int malam) => Wajib[malam];

        public static bool Dikenal(string id)
        {
            foreach (var malam in Wajib)
                if (Array.IndexOf(malam, id) >= 0) return true;
            return false;
        }

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

        // Tujuan wajib pertama malam ini yang belum selesai; null jika semua selesai (atau Malam 7).
        public static string BerikutnyaBelumSelesai(GameState s)
        {
            foreach (var id in Wajib[s.MalamKe])
                if (!s.TujuanSelesai.Contains(id)) return id;
            return null;
        }

        public static string Teks(string id)
        {
            switch (id)
            {
                case TibaDiRumah: return "Pulang: masuk ke rumah di ujung jalan";
                case BicaraBidan: return "Bicara dengan Bidan Wulan di depan kamar Ibu";
                case MasukKamarIbu: return "Masuk ke kamar Ibu";
                case BukaLemari: return "Buka lemari Bapak di kamar Ibu";
                case Cutscene23: return "Tunggu pukul 23.00 (duduk di kursi kamar Ibu)";
                case PotretPaku: return "Potret paku di lantai kamar Ibu [Q]";
                case BacaAksara: return "Bongkar papan dekat ranjang Ibu, baca tulisan boneka";
                case PilihanWiryo: return "Temui Mbah Wiryo di pintu depan";
                case SembunyiPertama: return "Kembali ke kamar Raka tanpa ketahuan Sosok";
                case PakdeDatang: return "Temui Pakde Darmo";
                case CabutPaku: return "Cabut tujuh paku di saka";
                case KunjungiSumi: return "Temui Bu Sumi di desa (lewat pintu depan)";
                case RitualPertama: return "Netralkan media di anglo dapur, kubur abunya";
                case AmbilBungkusanSumur: return "Ambil bungkusan di dasar sumur belakang";
                case LolosKejaranSumur: return "Lari dari Sosok ke pintu belakang dapur";
                case GaliKendi: return "Gali kendi";
                case BandingkanTanah: return "Bandingkan tanah";
                case BicaraLaras: return "Bicara dengan Laras";
                case SidangWarga: return "Hadapi sidang warga";
                default: return "";
            }
        }

        // Satu baris untuk HUD: tujuan berikutnya, atau petunjuk obat jika semua tujuan selesai. Kosong jika tidak ada.
        public static string BarisHud(GameState s)
        {
            var id = BerikutnyaBelumSelesai(s);
            if (id == CabutPaku)
                return s.MediaDitemukan[Kode.M2]
                    ? "Ambil tang di gudang, lalu cabut tujuh paku sesuai urutan hari"
                    : "Ketuk saka guru di ruang tengah [E], cari yang berongga";
            if (id != null) return Teks(id);
            if (s.MalamKe < 1 || s.MalamKe > 6 || JamGim.ObatSelesai(s)) return "";
            return $"Beri obat Ibu pukul {JamGim.Format(JamGim.MulaiJendelaObat(s.MalamKe))} di kamarnya";
        }
}
}
