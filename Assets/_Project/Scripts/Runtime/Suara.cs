using UnityEngine;

namespace TheSending.Runtime
{
    // Satu bunyi adegan. Jika klipnya belum ada (aset belum didapat tim), teks pengganti tampil di HUD
    // supaya adegan tetap terbaca. Begitu klip dipasang, teks pengganti tidak tampil lagi.
    public class Suara : MonoBehaviour
    {
        [SerializeField] AudioSource sumber;
        [SerializeField] Hud hud;
        [SerializeField] string teksPengganti;

        public void Putar()
        {
            if (sumber != null && sumber.clip != null)
            {
                sumber.Play();
                return;
            }
            if (hud != null) hud.Tampilkan(teksPengganti);
        }

        public void Hentikan()
        {
            if (sumber != null) sumber.Stop();
        }
    }
}
