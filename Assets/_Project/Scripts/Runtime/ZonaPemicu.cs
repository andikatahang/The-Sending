using System;
using UnityEngine;

namespace TheSending.Runtime
{
    // Zona (collider trigger) yang memberi tahu setiap kali pemain masuk. Pelanggan yang menentukan apa yang terjadi
    // dan menjaga supaya hanya sekali. Tidak terpicu saat gim dijeda (fisika berhenti).
    [RequireComponent(typeof(Collider))]
    public class ZonaPemicu : MonoBehaviour
    {
        public event Action Masuk;

        void OnTriggerEnter(Collider lain)
        {
            if (lain.GetComponent<PemainGerak>() != null) Masuk?.Invoke();
        }
    }
}
