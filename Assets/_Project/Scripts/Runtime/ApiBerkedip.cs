using UnityEngine;

namespace TheSending.Runtime
{
    // Nyala obor berkedip; waktu tanpa skala supaya tetap hidup saat jeda.
    public class ApiBerkedip : MonoBehaviour
    {
        [SerializeField] Light lampu;
        [SerializeField] float dasar = 1.5f, kuat = 0.4f, laju = 9f;

        float acak;

        void Awake() => acak = Random.value * 100f;

        void Update()
        {
            if (lampu == null) return;
            lampu.intensity = dasar + (Mathf.PerlinNoise(Time.unscaledTime * laju, acak) - 0.5f) * 2f * kuat;
        }
    }
}
