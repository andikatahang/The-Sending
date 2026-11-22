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

        public override void Interaksi() => Atur(!terbuka);

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
