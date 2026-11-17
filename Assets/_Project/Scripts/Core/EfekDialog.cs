using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheSending.Core
{
    // Kode efek yang ditulis penulis dialog di Inspector, dipisah ';'.
    // Kode tidak dikenal -> FormatException, supaya salah ketik ketahuan saat tes, bukan diam-diam diabaikan.
    public static class EfekDialog
    {
        public static void Jalankan(GameState s, string kode)
        {
            if (string.IsNullOrWhiteSpace(kode)) return;
            // Urai semua bagian dulu, baru jalankan: kode salah di tengah tidak boleh mengubah sebagian state.
            var aksi = new List<Action<GameState>>();
            foreach (var bagian in kode.Split(';'))
            {
                var k = bagian.Trim();
                if (k.Length == 0) continue;
                int titikDua = k.IndexOf(':');
                string nama = (titikDua < 0 ? k : k.Substring(0, titikDua)).Trim();
                string isi = titikDua < 0 ? "" : k.Substring(titikDua + 1).Trim();
                aksi.Add(Urai(nama, isi, k));
            }
            foreach (var a in aksi) a(s);
        }

        static Action<GameState> Urai(string nama, string isi, string asli)
        {
            switch (nama)
            {
                case "tujuan": { var id = Wajib(isi, asli); if (!TujuanMalam.Dikenal(id)) throw new FormatException($"Efek '{asli}': tujuan '{id}' tidak dikenal."); return s => TujuanMalam.Selesaikan(s, id); }
                case "barang": { var id = Wajib(isi, asli); if (!Inventaris.Dikenal(id)) throw new FormatException($"Efek '{asli}': barang '{id}' tidak dikenal."); return s => Inventaris.Tambah(s, id); }
                case "bukti": { int n = Nomor(isi, 'B', Kode.JumlahBukti, asli); return s => PapanBukti.TambahBukti(s, n); }
                case "katanya": { int n = Nomor(isi, 'K', Kode.JumlahKatanya, asli); return s => PapanBukti.TambahKatanya(s, n); }
                case "halaman": { int n = Nomor(isi, 'H', Kode.JumlahHalaman, asli); return s => BukuHarian.Temukan(s, n); }
                case "media": { int n = Nomor(isi, 'M', Kode.JumlahMedia, asli); return s => MediaSantet.Temukan(s, n); }
                case "serahkan": { int n = Nomor(isi, 'M', Kode.JumlahMedia, asli); return s => MediaSantet.Serahkan(s, n); }
                case "kepercayaan": { var v = NilaiEnum<AksiSumi>(isi, asli); return s => KepercayaanSumi.Tambah(s, v); }
                case "tuduh-sumi": TanpaIsi(isi, asli); return s => KepercayaanSumi.Tuduh(s);
                case "ritual-dari-sumi": TanpaIsi(isi, asli); return s => RitualPenangkal.Pelajari(s, dariBuSumi: true);
                case "minyak-sereh": TanpaIsi(isi, asli); return s => KepercayaanSumi.TerimaMinyakSereh(s);
                case "pilihan-akhir": { var v = NilaiEnum<PilihanAkhir>(isi, asli); return s => PenentuEnding.Pilih(s, v); }
                default: throw new FormatException($"Efek dialog tidak dikenal: '{asli}'.");
            }
        }

        static void TanpaIsi(string isi, string asli)
        {
            if (isi.Length != 0) throw new FormatException($"Efek '{asli}' tidak memakai isi setelah ':'.");
        }

        static string Wajib(string isi, string asli)
        {
            if (isi.Length == 0) throw new FormatException($"Efek '{asli}' butuh isi setelah ':'.");
            return isi;
        }

        // "B5" -> indeks 4
        static int Nomor(string isi, char huruf, int jumlah, string asli)
        {
            if (isi.Length >= 2 && char.ToUpperInvariant(isi[0]) == huruf
                && int.TryParse(isi.Substring(1), NumberStyles.None, CultureInfo.InvariantCulture, out int n) && n >= 1 && n <= jumlah)
                return n - 1;
            throw new FormatException($"Efek '{asli}': harus {huruf}1..{huruf}{jumlah}.");
        }

        static T NilaiEnum<T>(string isi, string asli) where T : struct
        {
            if (isi.Length > 0 && char.IsLetter(isi[0]) && Enum.TryParse(isi, false, out T hasil) && Enum.IsDefined(typeof(T), hasil)) return hasil;
            throw new FormatException($"Efek '{asli}': nilai '{isi}' tidak dikenal.");
        }
    }
}
