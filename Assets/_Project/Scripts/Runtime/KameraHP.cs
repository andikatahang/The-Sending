using System;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    public class KameraHP : MonoBehaviour
    {
        public const float JangkauanFoto = 6f;
        const float LamaKilatan = 0.15f;

        [SerializeField] Transform kamera;
        [SerializeField] Hud hud;
        [SerializeField] Suara bunyiFlash;

        Image kilatan;
        float sisaKilatan;

        void Awake()
        {
            if (kamera == null && Camera.main != null) kamera = Camera.main.transform;
            if (kamera == null) { Debug.LogError($"KameraHP di '{name}' butuh referensi kamera.", this); enabled = false; return; }
            var akar = UiPabrik.BuatCanvas("KilatanKamera", 30).transform;
            akar.SetParent(transform, false);
            kilatan = UiPabrik.BuatPanel(akar, "Kilatan", new Color(1f, 1f, 1f, 0f), Vector2.zero, Vector2.one);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (sisaKilatan > 0f)
            {
                sisaKilatan -= Time.unscaledDeltaTime;
                kilatan.color = new Color(1f, 1f, 1f, Mathf.Clamp01(sisaKilatan / LamaKilatan) * 0.8f);
            }

            if (gm.Dijeda || !gm.Kontrol.Kamera.WasPressedThisFrame()) return;
            sisaKilatan = LamaKilatan;
            if (bunyiFlash != null) bunyiFlash.Putar();

            // Trigger dan pemain sendiri (layer Ignore Raycast) tidak menghalangi foto; PetunjukFoto wajib memakai collider biasa.
            if (!Physics.Raycast(kamera.position, kamera.forward, out var hit, JangkauanFoto, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) return;
            var petunjuk = hit.collider.GetComponentInParent<PetunjukFoto>();
            if (petunjuk == null) return;
            try
            {
                petunjuk.Dipotret(gm.State);
                if (hud != null) hud.Tampilkan(petunjuk.Pesan);
            }
            catch (FormatException e)
            {
                Debug.LogError($"Efek foto salah di '{petunjuk.name}': {e.Message}", petunjuk);
            }
        }
    }
}
