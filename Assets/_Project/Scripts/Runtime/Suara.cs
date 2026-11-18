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
            if (hud != null && !string.IsNullOrEmpty(teksPengganti)) hud.Tampilkan(teksPengganti);
        }

        // Memudar lalu berhenti. Jalankan lewat StartCoroutine milik pemanggil.
        public System.Collections.IEnumerator Pudar(float detik)
        {
            if (sumber == null || !sumber.isPlaying) yield break;
            float awal = sumber.volume;
            for (float t = 0f; t < detik; t += Time.unscaledDeltaTime)
            {
                sumber.volume = Mathf.Lerp(awal, 0f, t / detik);
                yield return null;
            }
            sumber.Stop();
            sumber.volume = awal;
        }

        public void Hentikan()
        {
            if (sumber != null) sumber.Stop();
        }
    }
}
