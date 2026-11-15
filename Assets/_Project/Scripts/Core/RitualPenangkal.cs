using System;

namespace TheSending.Core
{
    // Game Story 6.5 dan desain 4.5. Salah langkah = ritual gagal, ulang dari langkah pertama.
    public class RitualPenangkal
    {
        static readonly LangkahRitual[] Urutan =
        {
            LangkahRitual.LetakkanMedia, LangkahRitual.TaburGaram, LangkahRitual.TutupKelor,
            LangkahRitual.NyalakanApi, LangkahRitual.AdukBambu, LangkahRitual.KuburAbu,
        };

        readonly GameState s;
        readonly int media;
        readonly PuzzleUrutan<LangkahRitual> langkah = new PuzzleUrutan<LangkahRitual>(Urutan, true);

        public int Kemajuan => langkah.Kemajuan;
        public bool Selesai => langkah.Selesai;

        public RitualPenangkal(GameState s, int media)
        {
            if (!BisaMulai(s, media)) throw new InvalidOperationException($"Ritual belum bisa dimulai untuk media {media + 1}.");
            this.s = s;
            this.media = media;
        }

        public bool Lakukan(LangkahRitual l)
        {
            if (Selesai) return false;
            bool benar = langkah.Pilih(l);
            if (Selesai) MediaSantet.Netralkan(s, media);
            return benar;
        }

        // Dari Bu Sumi: pengetahuan + satu ikat bahan. Dari Halaman 4: pengetahuan saja.
        public static void Pelajari(GameState s, bool dariBuSumi)
        {
            s.RitualDiketahui = true;
            if (dariBuSumi)
                for (int i = 0; i < s.BahanRitual.Length; i++) s.BahanRitual[i] = true;
            s.Umumkan(Perubahan.Ritual);
        }

        public static void AmbilBahan(GameState s, BahanRitual bahan)
        {
            s.BahanRitual[(int)bahan] = true;
            s.Umumkan(Perubahan.Ritual);
        }

        public static bool BahanLengkap(GameState s) => Array.TrueForAll(s.BahanRitual, x => x);

        public static bool BisaMulai(GameState s, int media) =>
            s.RitualDiketahui && BahanLengkap(s) && MediaSantet.BisaDinetralkan(s, media);
    }
}
