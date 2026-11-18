namespace TheSending.Core
{
    // Membaca tulisan di boneka (Game Story Malam 2): RATMI lalu SELASA KLIWON, suku demi suku.
    // Salah pilih tidak mengulang dan tidak ada penalti.
    public class BacaanAksara
    {
        public static readonly string[][] Kata = { AksaraJawa.Ratmi, AksaraJawa.SelasaKliwon };
        public static readonly int JumlahSuku = AksaraJawa.Ratmi.Length + AksaraJawa.SelasaKliwon.Length;

        PuzzleUrutan<string> puzzle = AksaraJawa.Buat(AksaraJawa.Ratmi);

        public int KataKe { get; private set; }
        public bool Selesai => KataKe >= Kata.Length;
        // Jumlah suku yang sudah terbaca dari awal; juga indeks gambar suku yang sedang ditandai.
        public int Terbaca { get; private set; }
        public string SukuSekarang => Selesai ? null : Kata[KataKe][puzzle.Kemajuan];

        public bool Pilih(string suku)
        {
            if (Selesai || !puzzle.Pilih(suku)) return false;
            Terbaca++;
            if (puzzle.Selesai && ++KataKe < Kata.Length) puzzle = AksaraJawa.Buat(Kata[KataKe]);
            return true;
        }
    }
}
