using System;

namespace TheSending.Core
{
    // Game Story 6.3 dan desain 4.7.
    public static class PapanBukti
    {
        public static void TambahBukti(GameState s, int b)
        {
            if (b < 0 || b >= Kode.JumlahBukti) throw new ArgumentOutOfRangeException(nameof(b), b, "Bukti harus 0..7.");
            if (s.BuktiKunci[b]) return;
            s.BuktiKunci[b] = true;
            s.Umumkan(Perubahan.Bukti);
            PerbaruiKatanya(s);
        }

        public const string FotoPakuMalam1 = "PakuMalam1";

        // Foto petunjuk yang bukan Bukti Kunci (desain 4.7). Tampil di Papan Bukti berlabel FOTO, tidak dihitung di Sidang Warga.
        // id, judul
        static readonly string[][] DaftarFoto =
        {
            new[] { FotoPakuMalam1, "Tiga paku berkarat yang dimuntahkan Ibu pada Malam 1" },
        };

        public static bool FotoDikenal(string id) => CariFoto(id) != null;

        public static string JudulFoto(string id) =>
            (CariFoto(id) ?? throw new ArgumentException($"Foto '{id}' tidak dikenal.", nameof(id)))[1];

        public static void TambahFoto(GameState s, string id)
        {
            if (!FotoDikenal(id)) throw new ArgumentException($"Foto '{id}' tidak dikenal.", nameof(id));
            if (s.Foto.Contains(id)) return;
            s.Foto.Add(id);
            s.Umumkan(Perubahan.Foto);
        }

        static string[] CariFoto(string id)
        {
            foreach (var f in DaftarFoto)
                if (f[0] == id) return f;
            return null;
        }

        public static void TambahKatanya(GameState s, int k)
        {
            if (k < 0 || k >= Kode.JumlahKatanya) throw new ArgumentOutOfRangeException(nameof(k), k, "Katanya harus 0..3.");
            if (s.KatanyaDiketahui[k]) return;
            s.KatanyaDiketahui[k] = true;
            s.StatusKatanya[k] = StatusInfo.Katanya;
            s.Umumkan(Perubahan.Katanya);
            PerbaruiKatanya(s);
        }

        static void PerbaruiKatanya(GameState s)
        {
            bool[] b = s.BuktiKunci;
            bool berubah = false;
            berubah |= Tetapkan(s, Kode.K1, b[Kode.B2] || b[Kode.B3], StatusInfo.Terbantahkan);
            berubah |= Tetapkan(s, Kode.K2, b[Kode.B7], StatusInfo.Terbantahkan);
            berubah |= Tetapkan(s, Kode.K3, b[Kode.B4] || b[Kode.B5], StatusInfo.Terbantahkan);
            berubah |= Tetapkan(s, Kode.K4, b[Kode.B6], StatusInfo.Bukti);
            if (berubah) s.Umumkan(Perubahan.Katanya);
        }

        static bool Tetapkan(GameState s, int k, bool syarat, StatusInfo status)
        {
            if (!s.KatanyaDiketahui[k] || !syarat || s.StatusKatanya[k] == status) return false;
            s.StatusKatanya[k] = status;
            return true;
        }
    }
}
