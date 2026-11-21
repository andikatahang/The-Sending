using UnityEngine;

namespace TheSending.Runtime
{
    // Rumah Bu Sumi utuh atau puing (jalur Fitnah). Dipasang di objek yang selalu aktif, bukan di dalam utuh/puing.
    public class PuingRumahSumi : MonoBehaviour
    {
        [SerializeField] GameObject[] utuh, puing;

        void OnEnable() => Terapkan(GameManager.Instance != null && GameManager.Instance.State.JalurFitnah);

        public void Terapkan(bool terbakar)
        {
            foreach (var go in utuh) if (go != null) go.SetActive(!terbakar);
            foreach (var go in puing) if (go != null) go.SetActive(terbakar);
        }
    }
}
