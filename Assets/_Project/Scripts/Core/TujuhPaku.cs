using System;

namespace TheSending.Core
{
    public enum HasilCabut { Ditolak, Benar, Salah }

    // Malam 3 (Game Story): mengetuk empat saka guru, lalu mencabut tujuh paku berukir nama hari dari saka yang benar.
    // Urutan mulai hari malam ini (Kamis malam = malam Jumat), lalu hari-hari berikutnya. Salah urutan: Kondisi Ibu -5,
    // kemajuan tetap; Sosok Santet yang muncul diurus Runtime. Kemajuan disimpan di GameState supaya selamat dari checkpoint.
    public static class TujuhPaku
    {
        public const int Jumlah = 7;
        public const int SakaBenar = 3;   // saka timur laut ruang tengah; nomor saka 0..3 dipasang PembuatMalam3
        public const int MalamPaku = 3;

        static Hari[] Urutan => KalenderJawa.UrutanPaku(MalamPaku);

        // true = saka berbunyi berongga; paku di baliknya ditemukan (M2 otomatis masuk Papan Bukti, desain 4.7).
        public static bool Ketuk(GameState s, int saka)
        {
            if (saka < 0 || saka > 3) throw new ArgumentOutOfRangeException(nameof(saka), saka, "Saka guru harus 0..3.");
            if (saka != SakaBenar) return false;
            MediaSantet.Temukan(s, Kode.M2);
            return true;
        }

        public static bool Selesai(GameState s) => s.PakuDicabut >= Jumlah;

        public static bool SudahDicabut(GameState s, Hari hari) => Array.IndexOf(Urutan, hari) < s.PakuDicabut;

        public static HasilCabut Cabut(GameState s, Hari hari)
        {
            if (!s.MediaDitemukan[Kode.M2] || Selesai(s) || SudahDicabut(s, hari)) return HasilCabut.Ditolak;
            if (Urutan[s.PakuDicabut] != hari)
            {
                KondisiIbu.Ubah(s, KondisiIbu.SalahUrutanPaku);
                return HasilCabut.Salah;
            }
            s.PakuDicabut++;
            if (Selesai(s)) TujuanMalam.Selesaikan(s, TujuanMalam.CabutPaku);
            return HasilCabut.Benar;
        }
    }
}
