using UnityEngine;

namespace TheSending.Runtime
{
    // Adegan dan cutscene. Selama adegan: gim dijeda (jam berhenti, input pemain mati), kursor tetap terkunci,
    // dan gambar diambil dari kamera adegan jika ada bidikan. Gerak kamera di cutscene diatur Timeline (PemutarTimeline).
    public class KameraAdegan : MonoBehaviour
    {
        [SerializeField] Camera kameraPemain, kameraAdegan;

        public bool Aktif { get; private set; }

        void Awake()
        {
            if (kameraPemain == null || kameraAdegan == null)
            {
                Debug.LogError("KameraAdegan butuh kamera pemain dan kamera adegan.", this);
                enabled = false;
                return;
            }
            kameraAdegan.enabled = false;
        }

        // Tanpa bidikan: gambar tetap dari mata pemain (misalnya saat layar hitam).
        public void Mulai(Transform bidikan = null)
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("KameraAdegan butuh GameManager di scene.", this);
                return;
            }
            if (!Aktif)
            {
                Aktif = true;
                gm.Sinematik = true;
                gm.Jeda(true);
            }
            if (bidikan != null) Bidik(bidikan);
        }

        public void Bidik(Transform bidikan)
        {
            kameraAdegan.transform.SetPositionAndRotation(bidikan.position, bidikan.rotation);
            kameraPemain.enabled = false;
            kameraAdegan.enabled = true;
        }

        public void Selesai()
        {
            if (!Aktif) return;
            Aktif = false;
            if (kameraAdegan != null) kameraAdegan.enabled = false;
            if (kameraPemain != null) kameraPemain.enabled = true;
            var gm = GameManager.Instance;
            if (gm == null) return;
            gm.Sinematik = false;
            gm.Jeda(false);
        }

        // Scene dibongkar di tengah adegan: lepaskan jeda.
        void OnDisable() => Selesai();
    }
}
