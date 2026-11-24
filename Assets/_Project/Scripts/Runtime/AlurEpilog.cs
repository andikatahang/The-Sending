using System;
using System.Collections;
using TheSending.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace TheSending.Runtime
{
    // Satu bidikan epilog: kartu teks (layar hitam) lalu set kecil dengan kamera bergerak (Timeline).
    [Serializable]
    public class BidikanEpilog
    {
        public GameObject set;              // null = kartu saja
        public PemutarTimeline timeline;    // null = tanpa bidikan
        public bool pagi;
        public string judul;
        [TextArea] public string teks;
        public float detik = 6f;
    }

    // Sutradara Scene_Epilog (K50-A): GameManager memuat scene ini begitu State.Ending terisi. Memutar bidikan ending itu,
    // pesan penutup Game Story 9.2, lalu menghapus save (K52) dan kembali ke menu utama. Tidak ada pemain.
    public class AlurEpilog : MonoBehaviour
    {
        public const string PesanPenutup = "Kepercayaan boleh dihormati, tetapi tuduhan harus dibuktikan. Jangan main hakim sendiri, dan jangan tinggalkan pengobatan medis.";   // dikutip
        static readonly Color KabutPagi = new Color32(0x9A, 0x9C, 0x98, 0xFF), AmbienPagi = new Color32(0x6A, 0x66, 0x5E, 0xFF);

        [SerializeField] BidikanEpilog[] sejati, fitnah, balas, terlambat;
        [SerializeField] KartuTeks kartu;
        [SerializeField] Suara musik;
        [SerializeField] Camera kamera;
        [SerializeField] Light cahayaPagi;

        Color kabutMalam, ambienMalam;

        void Start()
        {
            kartu.HitamSeketika();
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurEpilog butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            if (gm.State.Ending == Ending.Belum)
            {
                gm.MuatScene(GameManager.SceneMenuUtama);
                return;
            }
            kabutMalam = RenderSettings.fogColor;
            ambienMalam = RenderSettings.ambientLight;
            StartCoroutine(Putar(gm));
        }

        BidikanEpilog[] Daftar(Ending e) => e switch
        {
            Ending.Sejati => sejati,
            Ending.Fitnah => fitnah,
            Ending.BalasDendam => balas,
            _ => terlambat,
        };

        IEnumerator Putar(GameManager gm)
        {
            if (musik != null) musik.Putar();
            foreach (var b in Daftar(gm.State.Ending))
            {
                Siapkan(b);
                var director = b.timeline != null ? b.timeline.GetComponent<PlayableDirector>() : null;
                if (b.timeline != null) b.timeline.EvaluasiAwal();
                // Kartu memudar keluar ke set (atau ke latar hitam bila tanpa set).
                if (!string.IsNullOrEmpty(b.judul) || !string.IsNullOrEmpty(b.teks)) yield return kartu.Tampilkan(b.judul, b.teks, b.detik);
                else yield return kartu.DariHitam();
                if (b.timeline != null)
                {
                    yield return b.timeline.PutarDanTunggu();
                    director.time = director.duration;   // tahan pose akhir kamera selama memudar ke hitam
                    director.Evaluate();
                }
                yield return kartu.KeHitam();
                if (director != null) director.Stop();
            }
            Siapkan(new BidikanEpilog());
            yield return kartu.Tampilkan("", PesanPenutup, 8f);
            yield return kartu.KeHitam();
            gm.HapusSave();
            gm.MulaiBaru();
            gm.MuatScene(GameManager.SceneMenuUtama);
        }

        // Hanya set bidikan ini yang menyala; cahaya pagi atau malam. Tanpa set: latar hitam (kartu memudar tanpa terlihat).
        void Siapkan(BidikanEpilog b)
        {
            foreach (var daftar in new[] { sejati, fitnah, balas, terlambat })
                foreach (var x in daftar)
                    if (x.set != null) x.set.SetActive(x.set == b.set);
            if (b.set != null) b.set.SetActive(true);
            RenderSettings.fogColor = b.pagi ? KabutPagi : kabutMalam;
            RenderSettings.ambientLight = b.pagi ? AmbienPagi : ambienMalam;
            cahayaPagi.enabled = b.pagi;
            kamera.backgroundColor = b.set == null ? Color.black : RenderSettings.fogColor;
        }
    }
}
