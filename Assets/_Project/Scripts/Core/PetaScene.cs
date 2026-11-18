namespace TheSending.Core
{
    // Scene yang dimuat "Mulai Baru" dan "Lanjutkan" untuk sebuah GameState (desain 2.7).
    public static class PetaScene
    {
        public const string Desa = "Scene_Desa", RumahKeluarga = "Scene_RumahKeluarga";

        // Prolog dimulai di jalan desa sampai Raka tiba di rumah. Sesudah itu setiap malam dimulai di rumah keluarga.
        public static string Untuk(GameState s) =>
            s.MalamKe == 0 && !TujuanMalam.Sudah(s, TujuanMalam.TibaDiRumah) ? Desa : RumahKeluarga;
    }
}
