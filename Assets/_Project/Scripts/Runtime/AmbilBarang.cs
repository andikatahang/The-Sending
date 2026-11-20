using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Benda yang dibawa Raka saat E ditekan (misalnya linggis di gudang). Hilang dari scene setelah masuk inventaris,
    // juga setelah save dimuat.
    public class AmbilBarang : Interaktif
    {
        [SerializeField] string barang;
        [SerializeField, Range(0, 7)] int dariMalam;   // benda tampil sejak Prolog, baru bisa diambil mulai malam ini

        public override bool BisaDiinteraksi =>
            GameManager.Instance != null && GameManager.Instance.State.MalamKe >= dariMalam && Inventaris.Dikenal(barang);

        void Start()
        {
            if (!Inventaris.Dikenal(barang)) Debug.LogError($"AmbilBarang '{name}': barang '{barang}' tidak dikenal.", this);
            Sinkron();
        }

        public override void Interaksi()
        {
            Inventaris.Tambah(GameManager.Instance.State, barang);
            Sinkron();
        }

        void Sinkron()
        {
            var gm = GameManager.Instance;
            if (gm != null && Inventaris.Punya(gm.State, barang)) gameObject.SetActive(false);
        }
    }
}
