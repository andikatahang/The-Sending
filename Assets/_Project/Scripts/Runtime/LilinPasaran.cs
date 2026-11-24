using System;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Satu lilin sesaji bertanda pasaran (Malam 7, Watuireng). E memberi tahu sutradara; AlurWatuireng memeriksa urutan
    // (LilinSesaji) lalu memadamkan atau menyalakan lagi lewat Nyalakan. Butuh collider biasa (bukan trigger).
    public class LilinPasaran : Interaktif
    {
        public Pasaran Pasaran;
        [SerializeField] Light cahaya;        // Light anak
        [SerializeField] GameObject nyala;    // visual api lilin

        bool menyala = true;

        public event Action<LilinPasaran> Dipadamkan;

        public override string Prompt => $"Padamkan lilin {Pasaran}";
        public override bool BisaDiinteraksi => menyala;

        public override void Interaksi() => Dipadamkan?.Invoke(this);

        public void Nyalakan(bool nyala)
        {
            menyala = nyala;
            if (cahaya != null) cahaya.enabled = nyala;
            if (this.nyala != null) this.nyala.SetActive(nyala);
        }
    }
}
