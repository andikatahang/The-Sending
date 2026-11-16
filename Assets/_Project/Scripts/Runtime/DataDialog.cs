using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Satu percakapan. Penulis mengisi baris, pilihan, dan kode efek (lihat EfekDialog) di Inspector.
    [CreateAssetMenu(menuName = "The Sending/Dialog", fileName = "Dialog")]
    public class DataDialog : ScriptableObject
    {
        public BarisDialog[] Baris = new BarisDialog[0];
    }
}
