namespace TheSending.Core
{
    // Scene yang dimuat "Mulai Baru" dan "Lanjutkan" untuk sebuah GameState (desain 2.7).
    public static class PetaScene
    {
        public const string Desa = "Scene_Desa", RumahKeluarga = "Scene_RumahKeluarga";
        // Malam 3 (K17-B): dimuat dari pintu rumah Bu Sumi di Scene_Desa. Desa dan rumah Bu Sumi tidak punya checkpoint,
        // jadi Untuk() tidak pernah memilih keduanya di Malam 3.
        public const string RumahBuSumi = "Scene_RumahBuSumi";

        // Malam 5 (K32-A): makam dan rumah Pakde dimuat dari gerbang di Scene_Desa, tanpa checkpoint.
        public const string Makam = "Scene_Makam", RumahPakde = "Scene_RumahPakde";

        public const string Watuireng = "Scene_Watuireng", Epilog = "Scene_Epilog";

        // Dicatat sebelum kembali ke Scene_Desa; sutradara desa memilih titik muncul dari sini (tanpa field static).
        public static void CatatAsal(GameState s, string scene) => s.SceneAsal = scene ?? "";

        // Titik muncul di Scene_Desa pada Malam 3: depan rumah Bu Sumi setelah berkunjung, selain itu depan rumah keluarga.
        public static bool DiDepanRumahBuSumi(GameState s) => s.MalamKe == 3 && TujuanMalam.Sudah(s, TujuanMalam.KunjungiSumi);

        // Prolog dimulai di jalan desa sampai Raka tiba di rumah. Sesudah itu setiap malam dimulai di rumah keluarga.
        public static string Untuk(GameState s)
        {
            if (s.Ending != Ending.Belum) return Epilog;
            if (s.MalamKe == 7 && s.PilihanAkhir != PilihanAkhir.Belum && !s.LolosKejaranWiryo) return Watuireng;
            return Awal(s);
        }

        static string Awal(GameState s) =>
            s.MalamKe == 0 && !TujuanMalam.Sudah(s, TujuanMalam.TibaDiRumah) ? Desa : RumahKeluarga;
    }
}
