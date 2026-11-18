using UnityEngine;

namespace TheSending.Runtime
{
    // Ukiran yang baru terbaca saat disorot senter HP dari dekat (Game Story Malam 1: angka tahun di blandar).
    // Pasang di objek teks ukiran; objek ini tidak boleh punya collider.
    public class TerlihatSaatDisorot : MonoBehaviour
    {
        const float Jangkauan = 7f, SudutMaks = 20f;

        [SerializeField] SenterHP senter;
        [SerializeField] Transform kamera;
        [SerializeField] Renderer tampilan;
        [SerializeField] Hud hud;
        [SerializeField] string pesan;

        bool sudahDiberitahu;

        void Awake()
        {
            if (senter == null || kamera == null || tampilan == null)
            {
                Debug.LogError($"TerlihatSaatDisorot di '{name}' butuh senter, kamera, dan tampilan.", this);
                enabled = false;
                return;
            }
            tampilan.enabled = false;
        }

        void Update()
        {
            Vector3 ke = transform.position - kamera.position;
            // Disorot = senter menyala, cukup dekat, diarahkan ke ukiran, dan tidak terhalang dinding.
            bool disorot = senter.Menyala && ke.magnitude <= Jangkauan && Vector3.Angle(kamera.forward, ke) <= SudutMaks
                && !Physics.Linecast(kamera.position, transform.position, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            tampilan.enabled = disorot;
            if (!disorot || sudahDiberitahu) return;
            sudahDiberitahu = true;
            if (hud != null) hud.Tampilkan(pesan);
        }
    }
}
