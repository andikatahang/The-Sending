using System;

namespace TheSending.Core
{
    // Game Story 10.3. Halaman tertentu langsung menghasilkan bukti atau pengetahuan ritual.
    public static class BukuHarian
    {
        public static void Temukan(GameState s, int h)
        {
            if (h < 0 || h >= Kode.JumlahHalaman) throw new ArgumentOutOfRangeException(nameof(h), h, "Halaman harus 0..7.");
            if (s.HalamanDiary[h]) return;
            s.HalamanDiary[h] = true;
            s.Umumkan(Perubahan.Diary);

            if (s.HalamanDiary[Kode.H1] && s.HalamanDiary[Kode.H3]) PapanBukti.TambahBukti(s, Kode.B1);
            if (h == Kode.H4) RitualPenangkal.Pelajari(s, dariBuSumi: false);
            if (h == Kode.H7) PapanBukti.TambahBukti(s, Kode.B7);
        }
    }
}
