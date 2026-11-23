using System;
using TheSending.Core;
using UnityEngine;
using UnityEngine.AI;

namespace TheSending.Runtime
{
    // Sosok Santet di scene: NavMeshAgent yang digerakkan PerilakuSantet (Core). Nyalakan objeknya untuk memulai patroli.
    // Kecepatan selidik dan kejar naik sesuai tingkat kekuatan (desain 4.6). Diam saat gim dijeda (Time.timeScale 0).
    // PembuatMalam2 memasang NavMeshAgent dalam keadaan mati; agen dinyalakan di sini setelah NavMesh siap.
    [RequireComponent(typeof(NavMeshAgent))]
    public class SosokSantet : MonoBehaviour
    {
        public const float KecepatanPatroli = 1.2f;
        const float JarakTiba = 0.4f, TinggiDada = 1.4f;
        const float SudutSenter = 25f;   // separuh spotAngle senter HP (50°, PembuatRig)

        [SerializeField] LantaiNavMesh lantai;
        [SerializeField] PemainGerak pemain;
        [SerializeField] SenterHP senter;
        [SerializeField] Transform[] titikPatroli = new Transform[0];
        [SerializeField] Suara gemerincing;   // gemerincing paku setiap kali bergerak (Game Story 4.2), loop 3D

        NavMeshAgent agen;
        PerilakuSantet otak = new PerilakuSantet();
        int titikKe;
        Vector3 sasaranSelidiki;

        public event Action Menangkap;
        public KeadaanSantet Keadaan => otak.Keadaan;

        void Awake() => agen = GetComponent<NavMeshAgent>();

        void OnEnable()
        {
            otak = new PerilakuSantet();
            titikKe = 0;
            if (lantai == null || pemain == null || titikPatroli.Length == 0)
            {
                Debug.LogError("SosokSantet butuh lantai, pemain, dan titik patroli.", this);
                enabled = false;
                return;
            }
            lantai.Siapkan(transform, pemain.transform);
            agen.agentTypeID = lantai.AgentTypeID;
            agen.enabled = true;
            if (!agen.Warp(transform.position))
            {
                Debug.LogError($"SosokSantet di {transform.position} tidak berada di NavMesh.", this);
                enabled = false;
                return;
            }
            agen.speed = KecepatanPatroli;
            agen.SetDestination(titikPatroli[0].position);
            if (gemerincing != null) gemerincing.Putar();
        }

        // Langsung mengejar tanpa harus mendeteksi Raka dulu (Mbah Wiryo, Malam 7). Panggil setelah objeknya dinyalakan:
        // OnEnable membuat otak baru.
        public void KejarLangsung() => otak.MulaiKejar();

        void OnDisable()
        {
            if (gemerincing != null) gemerincing.Hentikan();
            if (agen != null) agen.enabled = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.Dijeda || !agen.isOnNavMesh) return;
            var s = gm.State;
            var p = Amati(s);
            var sebelumnya = otak.Keadaan;
            if (otak.Tick(Time.deltaTime, p))
            {
                enabled = false;
                Menangkap?.Invoke();
                return;
            }
            if (PerilakuSantet.Mendeteksi(p)) sasaranSelidiki = Datar(pemain.transform.position);
            float cepat = KewaspadaanSantet.KecepatanKejar(KewaspadaanSantet.TingkatKekuatan(s));
            switch (otak.Keadaan)
            {
                case KeadaanSantet.Kejar:
                    agen.speed = cepat;
                    agen.SetDestination(Datar(pemain.transform.position));
                    break;
                case KeadaanSantet.Selidiki:
                    agen.speed = cepat;
                    agen.SetDestination(sasaranSelidiki);
                    break;
                default:
                    agen.speed = KecepatanPatroli;
                    if (sebelumnya != KeadaanSantet.Patroli) agen.SetDestination(titikPatroli[titikKe].position);
                    else if (!agen.pathPending && agen.remainingDistance <= JarakTiba)
                    {
                        titikKe = (titikKe + 1) % titikPatroli.Length;
                        agen.SetDestination(titikPatroli[titikKe].position);
                    }
                    break;
            }
        }

        // Titik pemain di ketinggian Sosok: titik dalam kolong ranjang berada di bawah lantai.
        Vector3 Datar(Vector3 v) => new Vector3(v.x, transform.position.y, v.z);

        Pengamatan Amati(GameState s)
        {
            Vector3 a = transform.position, b = pemain.transform.position;
            bool menyala = senter != null && senter.Menyala;
            return new Pengamatan
            {
                Jarak = new Vector2(a.x - b.x, a.z - b.z).magnitude,
                Gerak = pemain.GerakSekarang,
                TahanNapas = pemain.Napas.SedangMenahan,
                MinyakSereh = s.PunyaMinyakSereh,
                Bersembunyi = pemain.Bersembunyi,
                SenterMenyala = menyala,
                SenterMengarah = menyala && Disorot(),
            };
        }

        // Cahaya senter mengenai Sosok: ia di dalam kerucut senter dan tidak terhalang dinding.
        bool Disorot()
        {
            var mata = pemain.Kamera.position;
            var arah = transform.position + Vector3.up * TinggiDada - mata;
            if (Vector3.Angle(pemain.Kamera.forward, arah) > SudutSenter) return false;
            return !Physics.Raycast(mata, arah.normalized, out var kena, arah.magnitude, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
                   || kena.transform.IsChildOf(transform);
        }
    }
}
