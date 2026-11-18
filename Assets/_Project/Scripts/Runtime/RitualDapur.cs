using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Ritual Penangkal di dapur (Game Story 6.5, desain 4.5, K25-B): setiap langkah adalah benda (BendaRitual), urutannya
    // dicek RitualPenangkal (Core). Langkah keenam (kubur abu) di pekarangan lewat KuburAbu. Salah langkah = gagal, ulang
    // dari anglo. Instans ritual tidak disimpan: pindah scene di tengah ritual berarti mulai lagi.
    public class RitualDapur : MonoBehaviour
    {
        public const int LangkahDiDapur = 5;
        const int LangkahApi = 4;   // NyalakanApi adalah langkah keempat
        // Game Story 6.2.
        static readonly string[] Nama =
        {
            "boneka kain", "tujuh paku", "bungkusan rambut", "telur ayam cemani", "kendi tanah merah", "kain kafan rajah", "kendi induk",
        };
        static readonly string[] PesanLangkah =
        {
            "Media diletakkan di atas anglo.", "Garam ditaburkan.", "Media ditutup daun kelor.", "Api dinyalakan.",
            "Raka mengaduk dengan bilah bambu kuning sambil berdoa. Media menjadi abu. Kubur abunya di pekarangan belakang.",
        };

        [SerializeField] Hud hud;
        [SerializeField] GameObject api, perlengkapan;   // perlengkapan: garam, kelor, korek, bambu di samping anglo
        [SerializeField] Suara suaraApi;

        RitualPenangkal ritual;

        public int Media { get; private set; } = -1;
        public bool MenungguKubur => ritual != null && ritual.Kemajuan == LangkahDiDapur;

        public static string NamaMedia(int m) => Nama[m];

        public void Lakukan(LangkahRitual langkah)
        {
            var s = GameManager.Instance.State;
            if (ritual == null && !Mulai(s, langkah)) return;
            if (!ritual.Lakukan(langkah))
            {
                ritual = null;
                Media = -1;
                hud.Tampilkan("Ritual gagal: urutannya salah. Media diangkat dari anglo; ulangi dari awal.");
                return;
            }
            hud.Tampilkan(PesanLangkah[ritual.Kemajuan - 1]);
        }

        bool Mulai(GameState s, LangkahRitual langkah)
        {
            if (!s.RitualDiketahui) hud.Tampilkan("Raka belum tahu cara menetralkan media santet.");
            else if (!RitualPenangkal.BahanLengkap(s)) hud.Tampilkan("Bahan ritual belum lengkap: garam, daun kelor, dan bilah bambu kuning.");
            else if (langkah != LangkahRitual.LetakkanMedia) hud.Tampilkan("Belum ada media di atas anglo.");
            else if (RitualPenangkal.MediaBerikutnya(s) < 0) hud.Tampilkan("Tidak ada media yang bisa dinetralkan sekarang.");
            else
            {
                Media = RitualPenangkal.MediaBerikutnya(s);
                ritual = new RitualPenangkal(s, Media);
                return true;
            }
            return false;
        }

        // Dipanggil KuburAbu di pekarangan: langkah keenam. Core menetralkan media (+8) dan menandai RitualPertama.
        public bool Kubur()
        {
            if (!MenungguKubur) return false;
            ritual.Lakukan(LangkahRitual.KuburAbu);
            ritual = null;
            Media = -1;
            return true;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var s = gm.State;
            bool siap = s.MalamKe >= 3 && s.RitualDiketahui && RitualPenangkal.BahanLengkap(s);
            if (perlengkapan.activeSelf != siap) perlengkapan.SetActive(siap);
            bool nyala = ritual != null && ritual.Kemajuan >= LangkahApi;
            if (api.activeSelf == nyala) return;
            api.SetActive(nyala);
            if (nyala) suaraApi.Putar();
            else suaraApi.Hentikan();
        }
    }
}
