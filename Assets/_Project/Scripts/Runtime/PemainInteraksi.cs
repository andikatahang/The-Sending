using UnityEngine;

namespace TheSending.Runtime
{
    public class PemainInteraksi : MonoBehaviour
    {
        public const float Jangkauan = 2.2f;

        [SerializeField] Transform kamera;

        public Interaktif Sasaran { get; private set; }

        void Update()
        {
            Sasaran = null;
            var gm = GameManager.Instance;
            if (gm == null || gm.Dijeda) return;

            if (Physics.Raycast(kamera.position, kamera.forward, out var hit, Jangkauan, ~0, QueryTriggerInteraction.Collide))
            {
                var benda = hit.collider.GetComponentInParent<Interaktif>();
                if (benda != null && benda.isActiveAndEnabled && benda.BisaDiinteraksi) Sasaran = benda;
            }
            if (Sasaran != null && gm.Kontrol.Interaksi.WasPressedThisFrame()) Sasaran.Interaksi();
        }
    }
}
