using System;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Satu-satunya singleton. Menyimpan GameState, menjalankan jam gim, mengurus save, checkpoint, dan jeda.
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; }
        public KontrolPemain Kontrol { get; private set; }
        public bool Dijeda => jumlahJeda > 0;
        public bool AdaSave => BerkasSave.Ada(Folder);

        // Diteruskan dari GameState aktif. UI berlangganan di sini, bukan ke GameState,
        // karena GameState diganti setiap kali save dimuat (catatan 0B di CLAUDE.md).
        public event Action<Perubahan> Berubah;

        int jumlahJeda;
        static string Folder => Application.persistentDataPath;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);   // duplikat dari scene lain; yang pertama tetap hidup
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Kontrol = new KontrolPemain();
            Kontrol.Aktifkan();
            Pasang(new GameState());
        }

        void OnDestroy()
        {
            if (Instance != this) return;
            Kontrol?.Buang();
            Instance = null;
        }

        void Update()
        {
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

        public void SimpanCheckpoint() => Checkpoint.Simpan(Folder, State);

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
        public void Jeda(bool aktif) => jumlahJeda = Math.Max(0, jumlahJeda + (aktif ? 1 : -1));

        void Pasang(GameState s)
        {
            if (State != null) State.Berubah -= Teruskan;
            State = s;
            State.Berubah += Teruskan;
            Berubah?.Invoke(Perubahan.Malam);                       // minta UI menggambar ulang dari state baru
            if (State.Ending != Ending.Belum) Berubah?.Invoke(Perubahan.Ending);
        }

        void Teruskan(Perubahan p) => Berubah?.Invoke(p);
    }
}
