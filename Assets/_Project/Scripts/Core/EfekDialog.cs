using System;

namespace TheSending.Core
{
    // Kode efek yang ditulis penulis dialog di Inspector, dipisah ';'.
    // Kode tidak dikenal -> FormatException, supaya salah ketik ketahuan saat tes, bukan diam-diam diabaikan.
    public static class EfekDialog
    {
        public static void Jalankan(GameState s, string kode)
        {
            if (string.IsNullOrWhiteSpace(kode)) return;
            foreach (var bagian in kode.Split(';'))
            {
                var k = bagian.Trim();
                if (k.Length == 0) continue;
                int titikDua = k.IndexOf(':');
                string nama = (titikDua < 0 ? k : k.Substring(0, titikDua)).Trim();
                string isi = titikDua < 0 ? "" : k.Substring(titikDua + 1).Trim();
                JalankanSatu(s, nama, isi, k);
            }
        }

        static void JalankanSatu(GameState s, string nama, string isi, string asli)
        {
            switch (nama)
            {
                case "tujuan": TujuanMalam.Selesaikan(s, Wajib(isi, asli)); break;
                case "bukti": PapanBukti.TambahBukti(s, Nomor(isi, 'B', Kode.JumlahBukti, asli)); break;
                case "katanya": PapanBukti.TambahKatanya(s, Nomor(isi, 'K', Kode.JumlahKatanya, asli)); break;
                case "halaman": BukuHarian.Temukan(s, Nomor(isi, 'H', Kode.JumlahHalaman, asli)); break;
                case "media": MediaSantet.Temukan(s, Nomor(isi, 'M', Kode.JumlahMedia, asli)); break;
                case "serahkan": MediaSantet.Serahkan(s, Nomor(isi, 'M', Kode.JumlahMedia, asli)); break;
                case "kepercayaan": KepercayaanSumi.Tambah(s, NilaiEnum<AksiSumi>(isi, asli)); break;
                case "tuduh-sumi": KepercayaanSumi.Tuduh(s); break;
                case "ritual-dari-sumi": RitualPenangkal.Pelajari(s, dariBuSumi: true); break;
                case "minyak-sereh": KepercayaanSumi.TerimaMinyakSereh(s); break;
                case "pilihan-akhir": PenentuEnding.Pilih(s, NilaiEnum<PilihanAkhir>(isi, asli)); break;
                default: throw new FormatException($"Efek dialog tidak dikenal: '{asli}'.");
            }
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
                && int.TryParse(isi.Substring(1), out int n) && n >= 1 && n <= jumlah)
                return n - 1;
            throw new FormatException($"Efek '{asli}': harus {huruf}1..{huruf}{jumlah}.");
        }

        static T NilaiEnum<T>(string isi, string asli) where T : struct
        {
            if (Enum.TryParse(isi, false, out T hasil) && Enum.IsDefined(typeof(T), hasil)) return hasil;
            throw new FormatException($"Efek '{asli}': nilai '{isi}' tidak dikenal.");
        }
    }
}
