using UnityEngine;

namespace TheSending.Runtime
{
    public class PemainInteraksi : MonoBehaviour
    {
        public const float Jangkauan = 2.2f;

        [SerializeField] Transform kamera;

        public Interaktif Sasaran { get; private set; }
        PemainGerak gerak;

        void Awake()
        {
            gerak = GetComponent<PemainGerak>();
            if (kamera == null && Camera.main != null) kamera = Camera.main.transform;
            if (kamera == null)
            {
                Debug.LogError($"{GetType().Name} di '{name}' butuh referensi kamera.", this);
                enabled = false;
            }
        }

        void Update()
        {
            Sasaran = null;
            var gm = GameManager.Instance;
            if (gm == null || gm.Dijeda) return;
            if (gerak != null && gerak.Bersembunyi) return;   // TempatSembunyi sendiri yang membaca E untuk keluar

            // Trigger (zona pemicu) tidak pernah menghalangi E; benda Interaktif wajib memakai collider biasa.
            if (Physics.Raycast(kamera.position, kamera.forward, out var hit, Jangkauan, ~0, QueryTriggerInteraction.Ignore))
            {
                var benda = hit.collider.GetComponentInParent<Interaktif>();
                if (benda != null && benda.isActiveAndEnabled && benda.BisaDiinteraksi) Sasaran = benda;
            }
            if (Sasaran != null && gm.Kontrol.Interaksi.WasPressedThisFrame()) Sasaran.Interaksi();
        }
    }
}
