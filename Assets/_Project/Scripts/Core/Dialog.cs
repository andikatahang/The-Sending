using System;

namespace TheSending.Core
{
    [Serializable]
    public class PilihanDialog
    {
        public string Teks;
        public string Efek;          // kode EfekDialog, boleh kosong
        public int LompatKe = -1;    // indeks baris berikutnya; -1 = percakapan selesai
    }

    [Serializable]
    public class BarisDialog
    {
        public string Pembicara;
        public string Teks;
        public string Terjemahan;    // terjemahan kalimat bahasa Jawa, boleh kosong
        public string Efek;          // dijalankan saat baris ini tampil
        public bool Akhir;           // percakapan selesai setelah baris ini
        public PilihanDialog[] Pilihan = new PilihanDialog[0];
    }
}
