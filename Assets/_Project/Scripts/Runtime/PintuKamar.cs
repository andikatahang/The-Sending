using UnityEngine;

namespace TheSending.Runtime
{
    // Daun pintu kamar berengsel: objek ini = engsel (sumbu Y), daun dan collider padatnya anak/komponen di sini.
    // E membuka/menutup. Mulai terbuka. Sosok Santet (juga anjing Pakde) dalam 1,5 m membuka pintu yang tertutup.
    // Collider pintu diabaikan LantaiNavMesh supaya NavMesh tidak terpotong di ambang.
    public class PintuKamar : Interaktif
    {
        const float KecepatanSudut = 180f, JarakOtomatis = 1.5f, JedaCek = 0.3f;

        [SerializeField] float sudutBuka = 90f;
        [SerializeField] AudioSource derit;

        Quaternion tertutup;
        bool terbuka = true;
        float sudut, cekBerikut;

        public override string Prompt => terbuka ? "Tutup pintu" : "Buka pintu";

        void Awake()
        {
            tertutup = transform.localRotation;
            sudut = sudutBuka;
            transform.localRotation = tertutup * Quaternion.Euler(0f, sudut, 0f);
        }

        // Tidak menutup selama pemain (CharacterController) berada di daerah ayunan daun, supaya tidak terjepit.
        public override void Interaksi()
        {
            if (terbuka && PemainDiAyunan()) return;
            Atur(!terbuka);
        }

        bool PemainDiAyunan()
        {
            var kotak = GetComponent<BoxCollider>();
            if (kotak == null) return false;
            float w = kotak.size.x, sisi = sudutBuka > 0f ? -1f : 1f;   // +90° memutar +X lokal ke -Z lokal
            var putar = (transform.parent != null ? transform.parent.rotation : Quaternion.identity) * tertutup;
            var pusat = transform.position + putar * new Vector3(w / 2f, kotak.center.y, sisi * w / 2f);
            foreach (var c in Physics.OverlapBox(pusat, new Vector3(w / 2f, kotak.size.y / 2f, w / 2f + 0.1f), putar, ~0,
                QueryTriggerInteraction.Ignore))
                if (c is CharacterController) return true;
            return false;
        }

        void Atur(bool buka)
        {
            terbuka = buka;
            if (derit != null && derit.clip != null) derit.Play();
        }

        void Update()
        {
            float target = terbuka ? sudutBuka : 0f;
            if (!Mathf.Approximately(sudut, target))
            {
                sudut = Mathf.MoveTowards(sudut, target, KecepatanSudut * Time.deltaTime);
                transform.localRotation = tertutup * Quaternion.Euler(0f, sudut, 0f);
            }
            if (terbuka || Time.time < cekBerikut) return;
            cekBerikut = Time.time + JedaCek;
            foreach (var s in FindObjectsByType<SosokSantet>(FindObjectsSortMode.None))
                if ((s.transform.position - transform.position).sqrMagnitude < JarakOtomatis * JarakOtomatis)
                {
                    Atur(true);
                    return;
                }
        }
    }
}
