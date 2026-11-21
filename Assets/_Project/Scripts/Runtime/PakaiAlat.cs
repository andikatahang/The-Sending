using System;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Benda yang butuh satu barang di inventaris (atau tanpa barang jika kosong): gali kendi, kain kafan, pot, laci,
    // mangkuk anjing, titik sampel makam. Sutradara yang menjalankan isinya lewat Dipakai.
    public class PakaiAlat : Interaktif
    {
        [SerializeField] string barang;
        [SerializeField] string promptPunya;
        [SerializeField] string pesanKurang;
        [SerializeField] DataDialog dialog;
        [SerializeField] PanelDialog panel;
        [SerializeField] Hud hud;
        [SerializeField] Suara suara;
        [SerializeField] GameObject[] hilangkan = new GameObject[0];
        [SerializeField] bool sekaliSaja = true;

        string promptDasar;

        public event Action Dipakai;
        public bool Sudah { get; private set; }

        bool Punya => string.IsNullOrEmpty(barang) ||
            (GameManager.Instance != null && Inventaris.Punya(GameManager.Instance.State, barang));

        public override string Prompt => Punya ? promptPunya : promptDasar ?? base.Prompt;
        public override bool BisaDiinteraksi => !Sudah;

        public void Atur(string promptBaru) => promptDasar = promptBaru;

        public override void Interaksi()
        {
            if (!Punya)
            {
                if (hud != null) hud.Tampilkan(pesanKurang);
                return;
            }
            if (sekaliSaja) Sudah = true;
            if (suara != null) suara.Putar();
            foreach (var go in hilangkan) if (go != null) go.SetActive(false);
            if (dialog != null && panel != null) StartCoroutine(panel.MulaiDanTunggu(dialog));
            Dipakai?.Invoke();
        }
    }
}
