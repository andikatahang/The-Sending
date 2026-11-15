using System;
using System.Collections.Generic;

namespace TheSending.Core
{
    // Puzzle "pilih berurutan": aksara, paku, lilin, dan langkah ritual.
    public class PuzzleUrutan<T>
    {
        readonly T[] kunci;
        readonly bool ulangDariAwalSaatSalah;

        public int Kemajuan { get; private set; }
        public bool Selesai => Kemajuan == kunci.Length;

        public PuzzleUrutan(IList<T> kunci, bool ulangDariAwalSaatSalah)
        {
            if (kunci == null || kunci.Count == 0) throw new ArgumentException("Kunci puzzle kosong.", nameof(kunci));
            this.kunci = new List<T>(kunci).ToArray();
            this.ulangDariAwalSaatSalah = ulangDariAwalSaatSalah;
        }

        // true jika item benar. Salah: kembali ke awal jika ulangDariAwalSaatSalah, selain itu kemajuan tetap.
        public bool Pilih(T item)
        {
            if (Selesai) return false;
            if (EqualityComparer<T>.Default.Equals(item, kunci[Kemajuan]))
            {
                Kemajuan++;
                return true;
            }
            if (ulangDariAwalSaatSalah) Kemajuan = 0;
            return false;
        }
    }
}
