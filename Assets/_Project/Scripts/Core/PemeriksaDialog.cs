using System;
using System.Collections.Generic;

namespace TheSending.Core
{
    // Memeriksa data satu percakapan tanpa menjalankannya, supaya salah ketik ketahuan di test, bukan di Play Mode.
    public static class PemeriksaDialog
    {
        public const int MaksPilihan = 4;   // tombol 1-4 di PanelDialog

        // Daftar masalah; kosong berarti dialog sehat.
        public static List<string> Periksa(IReadOnlyList<BarisDialog> baris)
        {
            var masalah = new List<string>();
            if (baris == null || baris.Count == 0)
            {
                masalah.Add("Dialog kosong.");
                return masalah;
            }
            for (int i = 0; i < baris.Count; i++)
            {
                var b = baris[i];
                if (b == null)
                {
                    masalah.Add($"Baris {i}: kosong.");
                    continue;
                }
                CekEfek(masalah, $"Baris {i}", b.Efek);
                var pilihan = b.Pilihan ?? new PilihanDialog[0];
                if (pilihan.Length > MaksPilihan)
                    masalah.Add($"Baris {i}: {pilihan.Length} pilihan, maksimal {MaksPilihan}.");
                for (int p = 0; p < pilihan.Length; p++)
                {
                    CekEfek(masalah, $"Baris {i} pilihan {p + 1}", pilihan[p].Efek);
                    if (pilihan[p].LompatKe >= baris.Count)
                        masalah.Add($"Baris {i} pilihan {p + 1}: LompatKe {pilihan[p].LompatKe} di luar daftar baris.");
                }
            }
            return masalah;
        }

        static void CekEfek(List<string> masalah, string tempat, string efek)
        {
            try { EfekDialog.Periksa(efek); }
            catch (FormatException e) { masalah.Add($"{tempat}: {e.Message}"); }
        }
    }
}
