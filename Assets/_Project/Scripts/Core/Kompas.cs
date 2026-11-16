namespace TheSending.Core
{
    // GDD: jarum kompas berputar tidak karuan di dekat media santet, makin dekat makin kencang.
    public static class Kompas
    {
        public const float JarakDeteksi = 12f;
        public const float PutaranMin = 90f, PutaranMaks = 1080f; // derajat per detik

        public static float KecepatanPutar(float jarakMediaTerdekat)
        {
            if (float.IsNaN(jarakMediaTerdekat) || jarakMediaTerdekat < 0f || jarakMediaTerdekat >= JarakDeteksi)
                return 0f;
            float dekat = 1f - jarakMediaTerdekat / JarakDeteksi;
            return PutaranMin + (PutaranMaks - PutaranMin) * dekat;
        }
    }
}
