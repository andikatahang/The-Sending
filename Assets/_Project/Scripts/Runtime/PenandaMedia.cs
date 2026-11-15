using System.Collections.Generic;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Pasang di objek media santet (M1..M7 = 0..6) supaya kompas HP bereaksi sampai media ditemukan.
    public class PenandaMedia : MonoBehaviour
    {
        static readonly List<PenandaMedia> Semua = new List<PenandaMedia>();

        [SerializeField, Range(0, Kode.JumlahMedia - 1)] int media;

        void OnEnable() => Semua.Add(this);
        void OnDisable() => Semua.Remove(this);

        public static float JarakTerdekat(Vector3 posisi, GameState s)
        {
            float terdekat = float.PositiveInfinity;
            foreach (var p in Semua)
            {
                if (s.MediaDitemukan[p.media]) continue;
                terdekat = Mathf.Min(terdekat, Vector3.Distance(posisi, p.transform.position));
            }
            return terdekat;
        }
    }
}
