using UnityEngine;

namespace TheSending.Runtime
{
    // Latar menu utama: kamera bergeser pelan ke kiri-kanan dan naik-turun sambil tetap membidik satu titik.
    // Waktu unscaled supaya tetap bergerak walau ada yang menjeda.
    public class KameraMenuMelayang : MonoBehaviour
    {
        [SerializeField] Vector3 bidikan;
        [SerializeField] float geserSamping = 3f, geserTinggi = 0.6f, detikPutaran = 40f;

        Vector3 awal, kanan;

        void Start()
        {
            awal = transform.position;
            kanan = transform.right;
        }

        void LateUpdate()
        {
            float sudut = Time.unscaledTime * 2f * Mathf.PI / detikPutaran;
            transform.position = awal + kanan * (Mathf.Sin(sudut) * geserSamping)
                                      + Vector3.up * (Mathf.Sin(sudut * 0.5f) * geserTinggi);
            transform.LookAt(bidikan);
        }
    }
}
