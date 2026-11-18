using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Lemari ayah di kamar Ibu: E membuka panel gembok. Setelah terbuka, gemboknya hilang dan lemari tidak bisa diinteraksi lagi.
    public class LemariAyah : Interaktif
    {
        [SerializeField] PanelGembok panel;
        [SerializeField] GameObject gembok;

        // Buku harian hanya didapat dari lemari ini, dan inventaris tidak dikosongkan saat malam berganti.
        static bool Terbuka =>
            GameManager.Instance != null && Inventaris.Punya(GameManager.Instance.State, Inventaris.BukuHarianAyah);

        public override string Prompt => "Buka gembok lemari Bapak";
        public override bool BisaDiinteraksi => panel != null && !Terbuka;

        public override void Interaksi() => panel.Buka();

        // State bisa berganti kapan saja (muat save, tertangkap), jadi tampilan gembok selalu mengikuti state.
        void Update()
        {
            if (gembok != null && gembok.activeSelf == Terbuka) gembok.SetActive(!Terbuka);
        }
    }
}
