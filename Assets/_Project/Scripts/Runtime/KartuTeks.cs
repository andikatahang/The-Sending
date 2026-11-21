using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Layar hitam dengan judul dan teks: ringkasan siang di awal malam, transisi, dan dasar cutscene.
    public class KartuTeks : MonoBehaviour
    {
        const float LamaPudar = 0.8f;

        [SerializeField, TextArea] string judulSaatMulai;
        [SerializeField, TextArea(3, 8)] string isiSaatMulai;
        [SerializeField] float detikSaatMulai = 4f;

        CanvasGroup grup;
        Text judul, isi;
        int nomorTerakhir;
        // Token kartu yang memegang jeda (0 = tidak ada); coroutine lama tidak boleh menyentuh kartu baru.
        int kartuAktif;

        public bool Tampil { get; private set; }

        // Musik kartu pergantian malam ("Malam N: ..."), dari Audio/Resources (sp-conjuring).
        AudioSource musik;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("KartuTeks", 40).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", Color.black, Vector2.zero, Vector2.one);
            grup = latar.gameObject.AddComponent<CanvasGroup>();
            grup.alpha = 0f;
            judul = UiPabrik.BuatTeks(latar.transform, "Judul", 54, TextAnchor.MiddleCenter, new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.75f));
            isi = UiPabrik.BuatTeks(latar.transform, "Isi", 30, TextAnchor.UpperCenter, new Vector2(0.15f, 0.2f), new Vector2(0.85f, 0.52f));
        }

        void Start()
        {
            if (!string.IsNullOrEmpty(judulSaatMulai))
                StartCoroutine(Tampilkan(judulSaatMulai, isiSaatMulai, detikSaatMulai));
        }

        // Kartu dimatikan atau dihancurkan saat tampil: sembunyikan dan lepaskan jeda tepat sekali.
        void OnDisable()
        {
            if (grup != null) grup.alpha = 0f;
            if (musik != null) musik.Stop();
            LepasJeda();
        }

        // Melepas jeda tepat satu kali, dari jalur normal maupun OnDisable.
        void LepasJeda()
        {
            if (kartuAktif == 0) return;
            kartuAktif = 0;
            Tampil = false;
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }

        // Kartu memudar dari kondisi layar saat ini, tahan, lalu memudar ke transparan.
        // Cutscene yang butuh layar tetap hitam sesudahnya memanggil KeHitam() lagi.
        public IEnumerator Tampilkan(string teksJudul, string teksIsi, float detik)
        {
            if (Tampil)
            {
                Debug.LogWarning($"KartuTeks sedang tampil; kartu '{teksJudul}' diabaikan.", this);
                yield break;
            }
            if (GameManager.Instance == null)
            {
                Debug.LogError("KartuTeks butuh GameManager di scene.", this);
                yield break;
            }
            int kartu = ++nomorTerakhir;
            kartuAktif = kartu;
            Tampil = true;
            GameManager.Instance.Jeda(true);
            judul.text = teksJudul;
            isi.text = teksIsi;
            bool kartuMalam = teksJudul.StartsWith("Malam ") && teksJudul.Contains(":");
            if (kartuMalam) PutarMusik();
            yield return Pudar(grup.alpha, 1f, kartu);
            yield return null; // Spasi dari frame terakhir memudar masuk tidak dihitung
            // Spasi melewati sisa waktu tahan kartu (memudar keluar tetap berjalan).
            for (float t = 0f; t < detik && kartuAktif == kartu && !(Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame); t += Time.unscaledDeltaTime) yield return null;
            if (kartuAktif == kartu) yield return Pudar(1f, 0f, kartu, kartuMalam ? musik : null);
            if (kartuMalam && musik != null && kartuAktif == kartu) musik.Stop();
            if (kartuAktif == kartu) LepasJeda();
        }

        void PutarMusik()
        {
            if (musik == null)
            {
                var klip = Resources.Load<AudioClip>("MusikKartuMalam");
                if (klip == null) return;
                musik = gameObject.AddComponent<AudioSource>();
                musik.clip = klip;
                musik.loop = true;
                musik.playOnAwake = false;
                var mixer = Resources.Load<UnityEngine.Audio.AudioMixer>(PenyimpanPengaturan.JalurMixer);
                var grupMusik = mixer != null ? mixer.FindMatchingGroups("Music") : null;
                if (grupMusik != null && grupMusik.Length > 0) musik.outputAudioMixerGroup = grupMusik[0];
            }
            musik.volume = 1f;
            musik.Play();
        }

        // Layar langsung hitam tanpa memudar. Dipanggil sutradara di Start, sebelum frame pertama tergambar.
        public void HitamSeketika()
        {
            judul.text = "";
            isi.text = "";
            grup.alpha = 1f;
        }

        public IEnumerator KeHitam()
        {
            while (Tampil) yield return null;
            judul.text = "";
            isi.text = "";
            yield return Pudar(grup.alpha, 1f);
        }

        public IEnumerator DariHitam()
        {
            while (Tampil) yield return null;
            yield return Pudar(grup.alpha, 0f);
        }

        // suara (opsional) ikut memudar ke senyap bersama layar.
        IEnumerator Pudar(float dari, float ke, int kartu = 0, AudioSource suara = null)
        {
            for (float t = 0f; t < LamaPudar; t += Time.unscaledDeltaTime)
            {
                if (grup == null || (kartu != 0 && kartu != kartuAktif)) yield break;
                grup.alpha = Mathf.Lerp(dari, ke, t / LamaPudar);
                if (suara != null) suara.volume = 1f - t / LamaPudar;
                yield return null;
            }
            if (grup == null || (kartu != 0 && kartu != kartuAktif)) yield break;
            grup.alpha = ke;
        }
    }
}
