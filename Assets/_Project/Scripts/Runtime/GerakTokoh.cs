using UnityEngine;

namespace TheSending.Runtime
{
    // Animasi tokoh manusia: Jalan saat tokoh berpindah (digerakkan sutradara atau Timeline), selain itu Diam.
    // Diukur per jendela waktu nyata, jadi Timeline unscaled tetap terhitung saat timeScale 0, sedangkan
    // guncangan Guncang.Posisi (bolak-balik di tempat, beberapa cm) tetap di bawah ambang. Posisi tidak diubah.
    public class GerakTokoh : MonoBehaviour
    {
        public const string ParameterJalan = "Jalan";
        const float Jendela = 0.2f, KecepatanJalan = 0.5f;   // detik nyata, meter per detik
        static readonly int Jalan = Animator.StringToHash(ParameterJalan);

        Animator animator;
        Vector3 acuan;
        float waktu;

        void Awake() => animator = GetComponentInChildren<Animator>();

        void OnEnable()
        {
            acuan = transform.position;
            waktu = 0f;
        }

        void Update()
        {
            waktu += Time.unscaledDeltaTime;
            if (waktu < Jendela) return;
            var geser = transform.position - acuan;
            geser.y = 0f;
            animator.SetBool(Jalan, geser.magnitude / waktu > KecepatanJalan);
            acuan = transform.position;
            waktu = 0f;
        }
    }
}
