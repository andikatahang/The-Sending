using System;

namespace TheSending.Core
{
    // Sosok Santet mengandalkan suara langkah dan cahaya senter (Game Story 4.2) dan menguat sesuai desain 4.6.
    // Semua angka adalah nilai awal; disetel saat playtest.
    public static class KewaspadaanSantet
    {
        public const float RadiusNapas = 1.5f, RadiusJalan = 4f, RadiusLari = 8f, FaktorMinyakSereh = 0.6f;
        public const float JarakCahaya = 12f;
        public const float KecepatanDasar = 3.5f, TambahKecepatanPerTingkat = 0.4f;
        public const float JedaDasar = 180f, KurangJedaPerTingkat = 30f, JedaMinimum = 60f;

        public static float RadiusSuara(Gerak gerak, bool tahanNapas, bool minyakSereh)
        {
            if (gerak == Gerak.Diam) return tahanNapas ? 0f : RadiusNapas;
            float r = gerak == Gerak.Lari ? RadiusLari : RadiusJalan;
            return minyakSereh ? r * FaktorMinyakSereh : r;
        }

        public static bool Terdengar(float jarak, Gerak gerak, bool tahanNapas, bool minyakSereh) =>
            jarak <= RadiusSuara(gerak, tahanNapas, minyakSereh);

        public static bool CahayaTerlihat(float jarak, bool senterMenyala, bool senterMengarahKeSantet) =>
            senterMenyala && senterMengarahKeSantet && jarak <= JarakCahaya;

        // Media diserahkan + media dari malam sebelumnya yang belum dinetralkan.
        public static int TingkatKekuatan(GameState s)
        {
            int tingkat = 0;
            for (int m = 0; m < Kode.JumlahMedia; m++)
            {
                if (s.MediaDiserahkan[m]) tingkat++;
                else if (s.MediaDitemukan[m] && !s.MediaDinetralkan[m] && s.MalamMediaDitemukan[m] < s.MalamKe) tingkat++;
            }
            return tingkat;
        }

        public static float KecepatanKejar(int tingkat) => KecepatanDasar + TambahKecepatanPerTingkat * tingkat;

        public static float JedaKemunculanDetik(int tingkat) =>
            Math.Max(JedaMinimum, JedaDasar - KurangJedaPerTingkat * tingkat);
    }
}
