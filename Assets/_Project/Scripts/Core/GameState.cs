using System;
using System.Collections.Generic;

namespace TheSending.Core
{
    // Field publik agar bisa diserialisasi JsonUtility. Ubah hanya lewat kelas aturan di Core.
    [Serializable]
    public class GameState
    {
        public int MalamKe;                 // 0 = Prolog, 1..7 = Malam 1..7
        public float MenitGim = 1050f;      // = JamGim.MulaiProlog (17.30), dicek oleh JamGimTests
        public int KondisiIbu = 60;         // = KondisiIbu.Awal, dicek oleh KondisiIbuTests
        public float BateraiPersen = 100f;  // = BateraiSenter.Penuh, dicek oleh BateraiKompasTests

        public bool[] MediaDitemukan = new bool[Kode.JumlahMedia];
        public int[] MalamMediaDitemukan = new int[Kode.JumlahMedia];
        public bool[] MediaDinetralkan = new bool[Kode.JumlahMedia];
        public bool[] MediaDiserahkan = new bool[Kode.JumlahMedia];

        public bool[] BuktiKunci = new bool[Kode.JumlahBukti];
        public bool[] KatanyaDiketahui = new bool[Kode.JumlahKatanya];
        public StatusInfo[] StatusKatanya = new StatusInfo[Kode.JumlahKatanya];
        public bool[] HalamanDiary = new bool[Kode.JumlahHalaman];

        public int KepercayaanSumi;
        public bool[] AksiSumiDipakai = new bool[Kode.JumlahAksiSumi];
        public int PintuSumiTerbukaMulaiMalam;
        public bool PunyaMinyakSereh;

        public bool RitualDiketahui;
        public bool[] BahanRitual = new bool[Kode.JumlahBahanRitual];
        public int PakuDicabut;             // Malam 3: paku saka yang sudah dicabut berurutan (TujuhPaku)

        public HasilSidang HasilSidang;
        public PilihanAkhir PilihanAkhir;
        public bool RitualTerakhirSelesai;
        public bool LolosKejaranWiryo;

        public bool ObatSudahDiberikan;
        public List<string> TujuanSelesai = new List<string>();
        public List<string> Barang = new List<string>();            // id dari Inventaris
        public List<string> Foto = new List<string>();              // id foto petunjuk dari PapanBukti
        public Ending Ending;

        [field: NonSerialized] public event Action<Perubahan> Berubah;

        public bool JalurFitnah =>
            HasilSidang == HasilSidang.Gagal || HasilSidang == HasilSidang.Diam || HasilSidang == HasilSidang.Menuduh;

        public void Umumkan(Perubahan perubahan) => Berubah?.Invoke(perubahan);
    }
}
