using System;

namespace TheSending.Core
{
    // GDD: menahan napas mengurangi suara, tetapi tidak bisa terlalu lama.
    public class Napas
    {
        public const float BatasDetik = 8f;
        public const float PulihPerDetik = 0.5f;

        bool harusLepasDulu;

        public float Sisa { get; private set; } = BatasDetik;
        public bool SedangMenahan { get; private set; }

        public void Tick(float detik, bool inginMenahan)
        {
            if (!inginMenahan) harusLepasDulu = false;

            if (inginMenahan && !harusLepasDulu && Sisa > 0f)
            {
                SedangMenahan = true;
                Sisa = Math.Max(0f, Sisa - detik);
                if (Sisa <= 0f)
                {
                    SedangMenahan = false;
                    harusLepasDulu = true;
                }
                return;
            }

            SedangMenahan = false;
            Sisa = Math.Min(BatasDetik, Sisa + PulihPerDetik * detik);
        }
    }
}
