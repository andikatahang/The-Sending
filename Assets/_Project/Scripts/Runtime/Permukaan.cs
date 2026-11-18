using UnityEngine;

namespace TheSending.Runtime
{
    public enum JenisPermukaan { Tanah, Kayu }

    // Penanda jenis lantai untuk LangkahKaki. Dipasang di objek (atau induknya) yang diinjak pemain.
    public class Permukaan : MonoBehaviour
    {
        public JenisPermukaan Jenis;
    }
}
