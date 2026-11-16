using System;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    [Serializable]
    public class EntriJurnal
    {
        public string Judul;
        [TextArea(2, 8)] public string Isi;
    }

    // Teks Jurnal. Indeks mengikuti Kode (B1 = 0, K1 = 0, M1 = 0, Halaman 1 = 0).
    [CreateAssetMenu(menuName = "The Sending/Jurnal", fileName = "Jurnal")]
    public class DataJurnal : ScriptableObject
    {
        public EntriJurnal[] Bukti = new EntriJurnal[Kode.JumlahBukti];
        public EntriJurnal[] Katanya = new EntriJurnal[Kode.JumlahKatanya];
        public EntriJurnal[] Media = new EntriJurnal[Kode.JumlahMedia];
        public EntriJurnal[] HalamanDiary = new EntriJurnal[Kode.JumlahHalaman];
        public EntriJurnal[] CatatanBapak = new EntriJurnal[0];
        public EntriJurnal[] CatatanRaka = new EntriJurnal[0];
    }
}
