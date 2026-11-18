using System;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // GDD: wajah Ibu makin pucat saat Kondisi Ibu turun. Kulit diwarnai dari warna aslinya menuju pucat:
    // Kondisi Ibu awal (60) ke atas = warna asli, 0 = pucat penuh. Renderer kulit dipilih PembuatRumahKeluarga.
    public class WajahIbu : MonoBehaviour
    {
        static readonly int WarnaDasar = Shader.PropertyToID("_BaseColor");
        static readonly Color Pucat = new Color(0.78f, 0.8f, 0.76f);

        [SerializeField] Renderer[] kulit = new Renderer[0];

        MaterialPropertyBlock blok;
        Color[] asli;
        int terakhir = -1;

        void Awake()
        {
            blok = new MaterialPropertyBlock();
            asli = Array.ConvertAll(kulit, r => r.sharedMaterial.GetColor(WarnaDasar));
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.State.KondisiIbu == terakhir) return;
            terakhir = gm.State.KondisiIbu;
            float pucat = 1f - Mathf.Clamp01(terakhir / (float)KondisiIbu.Awal);
            for (int i = 0; i < kulit.Length; i++)
            {
                blok.SetColor(WarnaDasar, Color.Lerp(asli[i], Pucat, pucat));
                kulit[i].SetPropertyBlock(blok);
            }
        }
    }
}
