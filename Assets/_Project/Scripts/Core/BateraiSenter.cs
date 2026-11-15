using System;

namespace TheSending.Core
{
    // GDD: baterai senter HP berkurang perlahan dan hanya bisa diisi di kamar Raka.
    public static class BateraiSenter
    {
        public const float Penuh = 100f;
        public const float DetikNyalaSampaiHabis = 600f; // nilai awal, disetel saat playtest

        public static bool BisaNyala(GameState s) => s.BateraiPersen > 0f;

        public static void Pakai(GameState s, float detik)
        {
            if (detik <= 0f || s.BateraiPersen <= 0f) return;
            s.BateraiPersen = Math.Max(0f, s.BateraiPersen - detik * Penuh / DetikNyalaSampaiHabis);
        }

        public static void Isi(GameState s) => s.BateraiPersen = Penuh;
    }
}
