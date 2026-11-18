using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Bunyi langkah pemain. Jenis lantai dari Permukaan di bawah kaki; tanpa Permukaan memakai bawaan scene.
    [RequireComponent(typeof(PemainGerak), typeof(CharacterController))]
    public class LangkahKaki : MonoBehaviour
    {
        // Jeda antarlangkah: langkah sekitar 1,0 m saat jalan (2,2 m/s) dan 1,3 m saat lari (4,0 m/s).
        public const float JedaJalan = 1.0f / PemainGerak.KecepatanJalan, JedaLari = 1.3f / PemainGerak.KecepatanLari;
        const float JarakCekLantai = 0.5f;

        [SerializeField] AudioSource sumber;
        [SerializeField] JenisPermukaan bawaan;
        [SerializeField] AudioClip[] tanahJalan, tanahLari, kayuJalan, kayuLari;

        PemainGerak gerak;
        CharacterController cc;
        float waktu;
        AudioClip terakhir;

        void Awake()
        {
            gerak = GetComponent<PemainGerak>();
            cc = GetComponent<CharacterController>();
        }

        void Update()
        {
            // Diam juga berarti dijeda atau bersembunyi (PemainGerak). Pemain mati saat cutscene: nilainya bisa basi.
            var g = gerak.enabled ? gerak.GerakSekarang : Gerak.Diam;
            if (g == Gerak.Diam || !cc.enabled || !cc.isGrounded)
            {
                waktu = JedaJalan;   // langkah pertama langsung berbunyi
                return;
            }
            bool lari = g == Gerak.Lari;
            waktu += Time.deltaTime;
            if (waktu < (lari ? JedaLari : JedaJalan)) return;
            waktu = 0f;
            Putar(lari);
        }

        void Putar(bool lari)
        {
            bool kayu = PermukaanDiBawah() == JenisPermukaan.Kayu;
            var klip = kayu ? (lari ? kayuLari : kayuJalan) : (lari ? tanahLari : tanahJalan);
            if (sumber == null || klip == null || klip.Length == 0) return;

            // Acak, tidak sama dengan klip sebelumnya.
            var pilih = klip[Random.Range(0, klip.Length)];
            if (pilih == terakhir && klip.Length > 1) pilih = klip[(System.Array.IndexOf(klip, pilih) + 1) % klip.Length];
            terakhir = pilih;
            sumber.pitch = Random.Range(0.92f, 1.08f);
            sumber.PlayOneShot(pilih, Random.Range(0.75f, 1f));
        }

        JenisPermukaan PermukaanDiBawah()
        {
            var asal = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(asal, Vector3.down, out var hit, JarakCekLantai, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                var p = hit.collider.GetComponentInParent<Permukaan>();
                if (p != null) return p.Jenis;
            }
            return bawaan;
        }
    }
}
