using System.Collections;
using TheSending.Core;
using UnityEngine;
using UnityEngine.AI;

namespace TheSending.Runtime
{
    // Sutradara Scene_RumahPakde (Malam 5): menyelinap melewati anjing penjaga, kunci laci di pot teras, laci memberi B6.
    // K33-A: anjing memakai SosokSantet; ketahuan = gonggong, layar hitam, dilempar ke desa, tanpa penalti Kondisi Ibu.
    // Sisa makanan di mangkuk menyibukkan anjing sampai Raka meninggalkan scene.
    public class AlurRumahPakde : MonoBehaviour
    {
        const float LamaKetahuan = 2f;

        [SerializeField] KartuTeks kartu;
        [SerializeField] KameraAdegan adegan;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] SosokSantet anjing;
        [SerializeField] PakaiAlat pot, laci, mangkuk;
        [SerializeField] InteraktifAlur pagarKeluar;
        [SerializeField] Transform titikMulai, titikMangkuk;
        [SerializeField] Suara suaraGonggong, suaraMakan, suaraPintu;

        bool sibuk;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            anjing.Menangkap += SaatKetahuan;
            mangkuk.Dipakai += SaatMangkuk;
            laci.Dipakai += SaatLaci;
            pagarKeluar.Ditekan += SaatKeluar;
        }

        void OnDisable()
        {
            anjing.Menangkap -= SaatKetahuan;
            mangkuk.Dipakai -= SaatMangkuk;
            laci.Dipakai -= SaatLaci;
            pagarKeluar.Ditekan -= SaatKeluar;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurRumahPakde butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            kartu.HitamSeketika();
            pemain.Pindah(titikMulai);
            kartu.StartCoroutine(kartu.DariHitam());
            if (s.MalamKe != 5)
            {
                anjing.gameObject.SetActive(false);
                hud.Tampilkan("Rumah Pakde gelap. Tidak ada yang bisa dilakukan di sini malam ini.");
                return;
            }
            anjing.gameObject.SetActive(!s.BuktiKunci[Kode.B6]);
            hud.Tampilkan("Anjing Pakde berpatroli di halaman. Kunci laci, kata Laras dulu, selalu Pakde selipkan di pot teras.");
        }

        void SaatKetahuan()
        {
            if (sibuk || this == null || !isActiveAndEnabled) return;
            StartCoroutine(Ketahuan());
        }

        // K33-A: tanpa GameManager.Tertangkap; Raka boleh mencoba lagi dari desa.
        IEnumerator Ketahuan()
        {
            sibuk = true;
            suaraGonggong.Putar();
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.Tampilkan("", "Anjing itu menggonggong keras. Raka lari keluar pagar.", LamaKetahuan));
                yield return kartu.StartCoroutine(kartu.KeHitam());
            }
            finally { adegan.Selesai(); }
            PetaScene.CatatAsal(State, PetaScene.RumahPakde);
            GameManager.Instance.MuatScene(PetaScene.Desa);
        }

        // Inventaris tidak punya hapus barang: sisa makanan tetap tercatat, mangkuk sekali pakai per kunjungan.
        void SaatMangkuk()
        {
            if (!anjing.isActiveAndEnabled) return;
            anjing.enabled = false;   // OnDisable SosokSantet mematikan agen; nyalakan lagi untuk berjalan ke mangkuk
            var agen = anjing.GetComponent<NavMeshAgent>();
            agen.enabled = true;
            if (agen.Warp(anjing.transform.position)) agen.SetDestination(titikMangkuk.position);
            suaraMakan.Putar();
            hud.Tampilkan("Anjing itu sibuk makan.");
        }

        void SaatLaci() =>
            hud.Tampilkan("Kuitansi uang muka dan catatan pembayaran untuk \"W\". Pakde menerima uang dari pembeli tanah.");

        void SaatKeluar()
        {
            if (sibuk) return;
            StartCoroutine(Keluar());
        }

        IEnumerator Keluar()
        {
            sibuk = true;
            adegan.Mulai();
            suaraPintu.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            PetaScene.CatatAsal(State, PetaScene.RumahPakde);
            GameManager.Instance.MuatScene(PetaScene.Desa);
        }
    }
}
