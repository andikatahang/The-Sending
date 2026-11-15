using UnityEngine;

namespace TheSending.Runtime
{
    // Basis semua benda yang bisa diinteraksi dengan E. Butuh Collider di objek ini atau anaknya.
    public abstract class Interaktif : MonoBehaviour
    {
        [SerializeField] string prompt = "Periksa";

        public virtual string Prompt => prompt;
        public virtual bool BisaDiinteraksi => true;
        public abstract void Interaksi();
    }
}
