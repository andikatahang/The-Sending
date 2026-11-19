using System;
using TheSending.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSending.Runtime
{
    // Malam 4 (K27-A): tahan E untuk menarik tali kerekan. Tempo naik selama ditahan; lewat
    // Core.KerekanSumur.KecepatanTarikMaks bungkusan jatuh lagi ke dasar. Dunia tetap berjalan.
    // Sutradara (AlurMalam4) mendengarkan BungkusanNaik untuk menandai tujuan dan menemukan M3.
    public class KerekanSumur : Interaktif
    {
        const float Kedalaman = 4f;     // meter tali sampai bungkusan di atas
        const float Akselerasi = 0.4f;  // tempo naik per detik selama E ditahan

        [SerializeField] Suara kerekanDerit, taliDitarik, bendaMasukAir;
        [SerializeField] Transform titikBungkusan;
        [SerializeField] Hud hud;

        public event Action BungkusanNaik;

        // shortcut: tempo berbasis progres linear, upgrade ke kurva jika terasa kaku (K27/R3).
        public float Progres => tertarik / Kedalaman;   // 0..1, untuk bar HUD
        public float Kecepatan => kecepatan;
        public bool Menarik => menarik;

        float tertarik, kecepatan;
        bool menarik;
        Vector3 posisiDasar;

        static bool PunyaAlat
        {
            get
            {
                var s = GameManager.Instance.State;
                return Inventaris.Punya(s, Inventaris.TaliTambang) && Inventaris.Punya(s, Inventaris.Pengait);
            }
        }

        public override string Prompt => PunyaAlat ? "Tahan E untuk menarik kerekan" : base.Prompt;

        public override bool BisaDiinteraksi
        {
            get
            {
                var gm = GameManager.Instance;
                return gm != null && gm.State.MalamKe == 4 && !menarik
                    && !TujuanMalam.Sudah(gm.State, TujuanMalam.AmbilBungkusanSumur);
            }
        }

        void Awake()
        {
            if (titikBungkusan != null) posisiDasar = titikBungkusan.localPosition;
        }

        public override void Interaksi()
        {
            if (!PunyaAlat)
            {
                if (hud != null) hud.Tampilkan("Aku butuh tali dan pengait. Pinjam di pos ronda desa.");
                return;
            }
            menarik = true;
            if (kerekanDerit != null) kerekanDerit.Putar();
        }

        void Update()
        {
            if (!menarik) return;
            var kb = Keyboard.current;
            if (kb == null || !kb.eKey.isPressed)
            {
                menarik = false;  // lepas: tempo kembali nol, tali tertahan di posisinya
                kecepatan = 0f;
                return;
            }

            kecepatan += Akselerasi * Time.deltaTime;
            if (!Core.KerekanSumur.TarikanAman(kecepatan))
            {
                Jatuh();
                return;
            }

            tertarik += kecepatan * Time.deltaTime;
            if (taliDitarik != null && kecepatan > 0f && UnityEngine.Random.value < Time.deltaTime) taliDitarik.Putar();
            GeserBungkusan();
            if (tertarik < Kedalaman) return;

            menarik = false;
            kecepatan = 0f;
            BungkusanNaik?.Invoke();
        }

        void Jatuh()
        {
            menarik = false;
            kecepatan = 0f;
            tertarik = 0f;
            GeserBungkusan();
            if (bendaMasukAir != null) bendaMasukAir.Putar();
            if (hud != null) hud.Tampilkan("Terlalu cepat! Bungkusannya jatuh lagi. Tarik lebih pelan.");
        }

        void GeserBungkusan()
        {
            if (titikBungkusan != null) titikBungkusan.localPosition = posisiDasar + Vector3.up * tertarik;
        }
    }
}
