using System;
using System.Collections.Generic;

namespace TheSending.Core
{
    // Urutan Sidang Warga (desain 4.8, Game Story Malam 6): tiga tuduhan berurutan, satu item Papan Bukti per tuduhan.
    // "Diam saja" atau "Ikut menuduh" kapan pun langsung gagal. Tujuan SidangWarga ditandai hanya setelah hasil ditetapkan.
    // Instans per sidang, tidak disimpan (R7): keluar di tengah sidang = ulang dari checkpoint 21.00.
    public sealed class JalannyaSidang
    {
        readonly GameState s;

        public int Tuduhan { get; private set; }
        public int Benar { get; private set; }
        public bool Selesai => s.HasilSidang != HasilSidang.Belum;

        public JalannyaSidang(GameState s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
            if (Selesai) throw new InvalidOperationException("Sidang Warga sudah selesai.");
        }

        // true jika jawaban untuk tuduhan saat ini benar. Tuduhan ketiga menetapkan hasil.
        public bool Jawab(ItemPapan item)
        {
            if (Selesai) throw new InvalidOperationException("Sidang Warga sudah selesai.");
            bool benar = SidangWarga.JawabanBenar(s, Tuduhan, item);
            if (benar) Benar++;
            Tuduhan++;
            if (Tuduhan == SidangWarga.JumlahTuduhan)
            {
                SidangWarga.Selesaikan(s, Benar);
                TujuanMalam.Selesaikan(s, TujuanMalam.SidangWarga);
            }
            return benar;
        }

        public void Diam()
        {
            SidangWarga.Diam(s);
            TujuanMalam.Selesaikan(s, TujuanMalam.SidangWarga);
        }

        public void IkutMenuduh()
        {
            SidangWarga.IkutMenuduh(s);
            TujuanMalam.Selesaikan(s, TujuanMalam.SidangWarga);
        }

        // Isi panel sidang: Bukti Kunci yang dimiliki (urut B1..B8), lalu Katanya yang diketahui (urut K1..K4).
        public static List<ItemPapan> PilihanJawaban(GameState s)
        {
            var daftar = new List<ItemPapan>();
            for (int b = 0; b < Kode.JumlahBukti; b++)
                if (s.BuktiKunci[b]) daftar.Add(ItemPapan.Bukti(b));
            for (int k = 0; k < Kode.JumlahKatanya; k++)
                if (s.KatanyaDiketahui[k]) daftar.Add(ItemPapan.Katanya(k));
            return daftar;
        }
    }
}
