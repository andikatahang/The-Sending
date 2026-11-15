using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Jarum menunjuk utara (+Z dunia); di dekat media santet jarum berputar makin kencang.
    public class KompasHP : MonoBehaviour
    {
        [SerializeField] Transform pemain;

        GameObject panel;
        RectTransform jarum;
        float putaran;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Kompas", 11).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.05f, 0.05f, 0.06f, 0.85f), new Vector2(0.84f, 0.06f), new Vector2(0.96f, 0.27f));
            panel = latar.gameObject;
            var j = UiPabrik.BuatPanel(latar.transform, "Jarum", new Color(0.6f, 0.1f, 0.08f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            jarum = j.rectTransform;
            jarum.pivot = new Vector2(0.5f, 0f);
            jarum.sizeDelta = new Vector2(8f, 90f);
            UiPabrik.BuatTeks(latar.transform, "U", 22, TextAnchor.UpperCenter, new Vector2(0f, 0.82f), new Vector2(1f, 1f)).text = "U";
            panel.SetActive(false);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            if (!gm.Dijeda && gm.Kontrol.Kompas.WasPressedThisFrame()) panel.SetActive(!panel.activeSelf);
            if (!panel.activeSelf) return;

            float jarak = PenandaMedia.JarakTerdekat(pemain.position, gm.State);
            putaran = (putaran + Kompas.KecepatanPutar(jarak) * Time.deltaTime) % 360f;
            jarum.localEulerAngles = new Vector3(0f, 0f, pemain.eulerAngles.y + putaran);
        }
    }
}
