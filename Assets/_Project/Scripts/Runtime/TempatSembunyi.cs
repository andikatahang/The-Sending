using UnityEngine;

namespace TheSending.Runtime
{
    // Lemari, kolong ranjang, atau balik tumpukan karung (Game Story 6.6). E masuk, E lagi keluar.
    // Jalan setelah PemainInteraksi (urutan 50): E yang mengeluarkan pemain tidak ikut memasukkannya lagi.
    [DefaultExecutionOrder(50)]
    public class TempatSembunyi : Interaktif
    {
        [SerializeField] PemainGerak pemain;
        [SerializeField] Transform titikDalam, titikKeluar;

        bool dipakai;
        int frameMasuk;

        public override bool BisaDiinteraksi =>
            pemain != null && titikDalam != null && titikKeluar != null && !pemain.Bersembunyi;

        public override void Interaksi()
        {
            pemain.Sembunyi(titikDalam);
            dipakai = true;
            frameMasuk = Time.frameCount;   // E yang dipakai masuk tidak ikut mengeluarkan
        }

        void Update()
        {
            if (!dipakai) return;
            if (!pemain.Bersembunyi)        // dipindah sutradara (adegan, muat ulang)
            {
                dipakai = false;
                return;
            }
            var gm = GameManager.Instance;
            if (gm == null || gm.Dijeda || Time.frameCount == frameMasuk || !gm.Kontrol.Interaksi.WasPressedThisFrame()) return;
            dipakai = false;
            pemain.Pindah(titikKeluar);
        }
    }
}
