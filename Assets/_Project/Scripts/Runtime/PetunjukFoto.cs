using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Petunjuk yang masuk Papan Bukti saat dipotret dengan Q (desain 4.7). Butuh Collider.
    public class PetunjukFoto : MonoBehaviour
    {
        [SerializeField] string efek;
        [SerializeField] string pesan = "Dipotret.";

        public string Pesan => pesan;

        public void Dipotret(GameState s) => EfekDialog.Jalankan(s, efek);
    }
}
