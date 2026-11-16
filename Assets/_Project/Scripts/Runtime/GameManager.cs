using System;
using TheSending.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheSending.Runtime
{
    // Satu-satunya singleton. Menyimpan GameState dan Pengaturan, menjalankan jam gim,
    // mengurus save, checkpoint, jeda (Time.timeScale), kursor, dan pindah scene.
    // Setiap scene punya objek GameManager supaya bisa di-Play langsung; duplikat menghancurkan diri.
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public const string SceneMenuUtama = "Scene_MainMenu";
        // Scene yang dimuat "Mulai Baru" dan "Lanjutkan". Fase 1 menggantinya dengan scene Prolog.
        public const string SceneAwal = "Scene_Sandbox0B";

        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; }
        public KontrolPemain Kontrol { get; private set; }
        public Pengaturan Pengaturan { get; private set; }
        public bool Dijeda => jumlahJeda > 0;
        public bool AdaSave => BerkasSave.Ada(Folder);

        // Diisi PemainGerak: true selama ada pemain yang dikendalikan. Scene tanpa pemain (menu, ending) membiarkannya false.
        public bool KunciKursor { get; set; }
        // Peringatan konten cukup tampil sekali per sesi gim.
        public bool PeringatanSudahTampil { get; set; }

        // Diteruskan dari GameState aktif. UI berlangganan di sini, bukan ke GameState,
        // karena GameState diganti setiap kali save dimuat.
        public event Action<Perubahan> Berubah;
        // GameState diganti (Mulai Baru, Lanjutkan, Tertangkap). Pelanggan menggambar ulang dari State baru.
        public event Action StateDiganti;

        int jumlahJeda;
        static string Folder => Application.persistentDataPath;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                enabled = false;
                Destroy(gameObject);   // duplikat dari scene lain; yang pertama tetap hidup
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SaatSceneDimuat;
            Kontrol = new KontrolPemain();
            Kontrol.Aktifkan();
            Pengaturan = PenyimpanPengaturan.Muat();
            Pasang(new GameState());
        }

        void Start()
        {
            if (Instance != this) return;
            // AudioMixer.SetFloat belum berlaku di Awake, jadi pengaturan diterapkan di Start.
            PenyimpanPengaturan.Terapkan(Pengaturan);
            if (!PenyimpanPengaturan.AdaMixer)
                Debug.LogWarning("MixerUtama belum ada di Assets/_Project/Audio/Resources/. Hanya volume utama yang berlaku.", this);
        }

        void OnDestroy()
        {
            if (Instance != this) return;
            SceneManager.sceneLoaded -= SaatSceneDimuat;
            Kontrol?.Buang();
            Time.timeScale = 1f;                      // domain reload mati: jangan tinggalkan editor dalam keadaan beku
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Instance = null;
        }

        void Update()
        {
            if (Instance != this) return;
            bool kunci = KunciKursor && !Dijeda;
            Cursor.lockState = kunci ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !kunci;
            if (!Dijeda && State.Ending == Ending.Belum) JamGim.Maju(State, Time.deltaTime);
        }

        public void MulaiBaru() => Pasang(new GameState());

        public bool Lanjutkan()
        {
            var s = BerkasSave.Baca(Folder);
            if (s == null) return false;
            Pasang(s);
            return true;
        }

        public bool SimpanCheckpoint() => Checkpoint.Simpan(Folder, State);

        // Desain 4.2. Posisi pemain dikembalikan oleh scene malam (titik mulai checkpoint), bukan di sini.
        public void Tertangkap()
        {
            var s = Checkpoint.MuatSetelahTertangkap(Folder);
            if (s != null)
            {
                Pasang(s);
                return;
            }
            KondisiIbu.SetelahTertangkap(State);
            SimpanCheckpoint();
        }

        // Dialog, Jurnal, kartu teks, dan menu memanggil Jeda(true) saat dibuka dan Jeda(false) saat ditutup.
        // Selama ada yang menjeda, Time.timeScale = 0: fisika, NavMesh, Animator, audio, dan WaitForSeconds berhenti.
        public void Jeda(bool aktif)
        {
            if (!aktif && jumlahJeda == 0)
            {
                Debug.LogWarning("Jeda(false) dipanggil tanpa pasangan Jeda(true).", this);
                return;
            }
            jumlahJeda += aktif ? 1 : -1;
            Time.timeScale = Dijeda ? 0f : 1f;
        }

        // Dipanggil panel pengaturan setelah mengubah nilai: rapikan, simpan ke PlayerPrefs, terapkan.
        public void SimpanPengaturan()
        {
            Pengaturan.Rapikan();
            PenyimpanPengaturan.Simpan(Pengaturan);
            PenyimpanPengaturan.Terapkan(Pengaturan);
        }

        // Panel yang sedang menjeda melepas jedanya sendiri di OnDisable saat scene lama dibongkar.
        public void MuatScene(string nama) => SceneManager.LoadScene(nama);

        // Aman: pada load Single, OnDisable panel lama (melepas jeda) jalan sebelum sceneLoaded, dan Start scene baru jalan sesudahnya.
        void SaatSceneDimuat(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single) return;
            if (jumlahJeda != 0)
            {
                Debug.LogWarning($"Jeda tidak dilepas sebelum pindah scene ({jumlahJeda}); direset.");
                jumlahJeda = 0;
            }
            Time.timeScale = 1f;
        }

        void Pasang(GameState s)
        {
            int malamLama = State != null ? State.MalamKe : s.MalamKe;
            if (State != null) State.Berubah -= Teruskan;
            State = s;
            State.Berubah += Teruskan;
            StateDiganti?.Invoke();
            if (State.MalamKe != malamLama) Berubah?.Invoke(Perubahan.Malam);
            if (State.Ending != Ending.Belum) Berubah?.Invoke(Perubahan.Ending);
        }

        void Teruskan(Perubahan p) => Berubah?.Invoke(p);
    }
}
