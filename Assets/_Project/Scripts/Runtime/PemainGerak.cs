using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PemainGerak : MonoBehaviour
    {
        public const float KecepatanJalan = 2.2f, KecepatanLari = 4.0f;
        public const float BatasDongak = 80f;
        const float Gravitasi = -9.81f;

        [SerializeField] Transform kamera;

        CharacterController cc;
        float dongak, kecepatanJatuh;

        public Gerak GerakSekarang { get; private set; }
        public Napas Napas { get; } = new Napas();
        public Transform Kamera => kamera;

        void Awake()
        {
            cc = GetComponent<CharacterController>();
            if (kamera == null && Camera.main != null) kamera = Camera.main.transform;
            if (kamera == null)
            {
                Debug.LogError($"{GetType().Name} di '{name}' butuh referensi kamera.", this);
                enabled = false;
            }
        }

        // Kursor milik GameManager; pemain hanya memberi tahu bahwa ada yang dikendalikan.
        void OnEnable()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning($"PemainGerak di '{name}' tidak menemukan GameManager; pemain tidak bisa bergerak.", this);
                return;
            }
            GameManager.Instance.KunciKursor = true;
        }

        void OnDisable()
        {
            if (GameManager.Instance != null) GameManager.Instance.KunciKursor = false;
        }

        // Memindahkan pemain ke titik mulai (awal bab, muat checkpoint, adegan).
        // CharacterController dimatikan dulu supaya posisi baru tidak ditimpa.
        public void Pindah(Transform titik)
        {
            cc.enabled = false;
            transform.SetPositionAndRotation(titik.position, Quaternion.Euler(0f, titik.eulerAngles.y, 0f));
            dongak = 0f;
            kamera.localEulerAngles = Vector3.zero;
            cc.enabled = true;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.Dijeda)
            {
                GerakSekarang = Gerak.Diam;
                return;
            }

            var k = gm.Kontrol;
            var p = gm.Pengaturan;
            Vector2 lihat = k.Lihat.ReadValue<Vector2>() * p.Sensitivitas;
            transform.Rotate(0f, lihat.x, 0f);
            dongak = Mathf.Clamp(dongak + (p.BalikY ? lihat.y : -lihat.y), -BatasDongak, BatasDongak);
            kamera.localEulerAngles = new Vector3(dongak, 0f, 0f);

            Vector2 masuk = Vector2.ClampMagnitude(k.Gerak.ReadValue<Vector2>(), 1f);
            bool bergerak = masuk.sqrMagnitude > 0.01f;
            bool lari = bergerak && k.Lari.IsPressed();
            Vector3 arah = (transform.right * masuk.x + transform.forward * masuk.y) * (lari ? KecepatanLari : KecepatanJalan);
            kecepatanJatuh = cc.isGrounded ? -1f : kecepatanJatuh + Gravitasi * Time.deltaTime;
            arah.y = kecepatanJatuh;
            cc.Move(arah * Time.deltaTime);

            GerakSekarang = !bergerak ? Gerak.Diam : lari ? Gerak.Lari : Gerak.Jalan;
            Napas.Tick(Time.deltaTime, k.TahanNapas.IsPressed());
        }
    }
}
