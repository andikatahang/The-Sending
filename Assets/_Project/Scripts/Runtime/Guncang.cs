using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSending.Runtime
{
    // Guncangan pengganti animasi tokoh (Laras gemetar, Ibu menjerit) sampai model beranimasi datang.
    // Waktu tanpa skala, karena adegan menjeda gim.
    public static class Guncang
    {
        public static IEnumerator Posisi(Transform t, float lama, float kuat)
        {
            var awal = t.localPosition;
            for (float w = 0f; w < lama && !(Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && GameManager.Instance != null && GameManager.Instance.Sinematik); w += Time.unscaledDeltaTime)
            {
                t.localPosition = awal + new Vector3(Random.Range(-kuat, kuat), 0f, Random.Range(-kuat, kuat));
                yield return null;
            }
            t.localPosition = awal;
        }
    }
}
