using UnityEngine;

namespace TheSending.Runtime
{
    // Pasang di objek Sosok yang membuntuti di jalan sawah (Malam 7, tanpa NavMesh, tidak menyerang). Jalan sawah menuju +z:
    // Sosok berada jarakBelakang di belakang pemain dan offsetX ke samping (sisi sawah), menghadap pemain. Objek mati
    // saat pemain melewati zBerhenti. Diam saat gim dijeda.
    public class PembuntutSosok : MonoBehaviour
    {
        [SerializeField] PemainGerak pemain;
        [SerializeField] float jarakBelakang = 16f;
        [SerializeField] float offsetX = 6f;
        [SerializeField] float zBerhenti;
        [SerializeField] float kecepatan = 4f;   // m/detik: menyusul tanpa melompat saat pemain berlari

        void Update()
        {
            var gm = GameManager.Instance;
            if (pemain == null || gm == null || gm.Dijeda) return;
            var p = pemain.transform.position;
            if (p.z > zBerhenti)
            {
                gameObject.SetActive(false);
                return;
            }
            var sasaran = new Vector3(p.x + offsetX, transform.position.y, p.z - jarakBelakang);
            transform.position = Vector3.MoveTowards(transform.position, sasaran, kecepatan * Time.deltaTime);
            var arah = new Vector3(p.x - transform.position.x, 0f, p.z - transform.position.z);
            if (arah.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(arah);
        }
    }
}
