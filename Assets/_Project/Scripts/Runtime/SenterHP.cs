using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    public class SenterHP : MonoBehaviour
    {
        [SerializeField] Light cahaya;

        public bool Menyala => cahaya != null && cahaya.enabled;

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || cahaya == null) return;
            var s = gm.State;

            if (!gm.Dijeda && gm.Kontrol.Senter.WasPressedThisFrame())
                cahaya.enabled = !cahaya.enabled && BateraiSenter.BisaNyala(s);

            if (!cahaya.enabled) return;
            if (!gm.Dijeda) BateraiSenter.Pakai(s, Time.deltaTime);
            if (!BateraiSenter.BisaNyala(s)) cahaya.enabled = false;
        }
    }
}
