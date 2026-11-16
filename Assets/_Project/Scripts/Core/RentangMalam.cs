namespace TheSending.Core
{
    public static class RentangMalam
    {
        public static bool Berisi(int malam, int dari, int sampai) => malam >= dari && malam <= sampai;
    }
}
