using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Malam 3 (Game Story, Malam 3): Pakde Darmo datang membujuk menjual tanah, Halaman 3 di kotak sepatu,
    // Ibu menjerit setiap jam berdentang, kompas ke saka guru, tujuh paku (salah urutan: Sosok Santet muncul), pintu depan
    // ke jalan desa menuju rumah Bu Sumi, lalu Ritual Penangkal. Ketukan saka (KetukSaka), paku (PakuDiSaka), tang
    // (AmbilBarang), bahan (AmbilBahan), ritual (RitualDapur, BendaRitual), abu (KuburAbu), obat, dan kursi berjalan sendiri.
    public class AlurMalam3 : MonoBehaviour
    {
        const float LamaJerit = 2.5f, KuatJerit = 0.02f;
        const float LamaSosok = 40f;   // detik waktu gim; nilai awal, disetel dari daftar cek (R5)
        // Game Story, Malam 3 dan Malam 4, "Ringkasan siang" dan tujuan bab (dikutip persis).
        const string RingkasanMalam3 =
            "Raka mencari informasi di desa. Sebagian warga menyapanya ramah, sebagian menghindar.";
        const string RingkasanMalam4 =
            "Air sumur berbau busuk. Kabar bahwa keluarga Raka kena santet sudah menyebar ke seluruh desa.";
        const string TujuanBab = "Tujuan: Menghentikan jeritan Ibu dan mencari sekutu";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] DataDialog dialogPakde;
        [SerializeField] Transform titikMulai, titikTamu, titikKamarRaka, titikRuangTengah, titikPulang, titikMunculSosok, bidikPakde, bidikIbu;
        [SerializeField] Transform ibu;   // visual Ibu di ranjang; diguncang saat menjerit
        [SerializeField] GameObject pakde, buahTangan, dempul, pakuSaka;
        [SerializeField] PakuDiSaka[] paku = new PakuDiSaka[0];
        [SerializeField] InteraktifAlur pintuGudang, pintuDepan;
        [SerializeField] SosokSantet sosok;
        [SerializeField] Suara suaraMotorDatang, suaraMotorPergi, suaraKetuk, suaraDentang, suaraJerit, suaraCabut, suaraPintuDepan;
        [SerializeField] Suara suaraStinger, suaraDetak, musikTegang;

        bool berjalan, sibuk, jeritanPertama, petunjukBuSumi, petunjukObat, gantiDimulai, sosokBerjalan, detakBerbunyi;
        float menitTerakhir, sisaSosok;

        static GameState State => GameManager.Instance.State;

        void OnEnable()
        {
            foreach (var p in paku) p.Dicabut += SaatPakuDicabut;
            pintuDepan.Ditekan += SaatPintuDepan;
            sosok.Menangkap += SaatTertangkap;
            if (GameManager.Instance != null) GameManager.Instance.Berubah += SaatBerubah;
        }

        void OnDisable()
        {
            foreach (var p in paku) p.Dicabut -= SaatPakuDicabut;
            pintuDepan.Ditekan -= SaatPintuDepan;
            sosok.Menangkap -= SaatTertangkap;
            if (GameManager.Instance != null) GameManager.Instance.Berubah -= SaatBerubah;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurMalam3 butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            if (s.MalamKe >= 4)
            {
                hud.Tampilkan("Malam 4 belum dibuat (Fase 4).");
                return;
            }
            if (s.MalamKe != 3) return;
            berjalan = true;
            kartu.HitamSeketika();
            KeadaanSesuaiState(s);
            if (!TujuanMalam.Sudah(s, TujuanMalam.PakdeDatang)) StartCoroutine(Pembuka());
            else if (TujuanMalam.Sudah(s, TujuanMalam.KunjungiSumi)) StartCoroutine(Pulang(s));
            else StartCoroutine(Lanjut());
        }

        // Isi rumah mengikuti state: awal malam, kembali dari rumah Bu Sumi, "Lanjutkan", atau muat ulang setelah tertangkap.
        void KeadaanSesuaiState(GameState s)
        {
            pintuGudang.gameObject.SetActive(false);   // gudang sudah dibuka di Malam 2
            pakde.SetActive(false);
            buahTangan.SetActive(TujuanMalam.Sudah(s, TujuanMalam.PakdeDatang));
            TampilkanPaku(s.MediaDitemukan[Kode.M2]);
            jeritanPertama = s.HalamanDiary[Kode.H3];  // sudah terjadi sebelum save ini
            petunjukBuSumi = TujuanMalam.Sudah(s, TujuanMalam.CabutPaku);
            sosok.gameObject.SetActive(false);
            menitTerakhir = s.MenitGim;
            PerbaruiPromptPintu(s);
        }

        void TampilkanPaku(bool ditemukan)
        {
            dempul.SetActive(!ditemukan);
            pakuSaka.SetActive(ditemukan);
        }

        void PerbaruiPromptPintu(GameState s) =>
            pintuDepan.UbahPrompt(TujuanMalam.Sudah(s, TujuanMalam.CabutPaku) && !TujuanMalam.Sudah(s, TujuanMalam.KunjungiSumi)
                ? "Pergi ke rumah Bu Sumi" : null);

        void SaatBerubah(Perubahan p)
        {
            if (!berjalan) return;
            var s = State;
            // Game Story Malam 3 butir 4: di saka yang benar, tujuh paku berkarat tertutup dempul hitam.
            if (p == Perubahan.Media && s.MediaDitemukan[Kode.M2] && !pakuSaka.activeSelf)
            {
                TampilkanPaku(true);
                hud.Tampilkan("Di balik dempul hitam tertanam tujuh paku berkarat, masing-masing berukir nama hari.");
                hud.Tampilkan("Paku-paku itu otomatis difoto dan masuk Papan Bukti.");
                hud.Tampilkan("Paku santet tidak boleh dicabut sembarangan. Mungkin Bapak menulis sesuatu; periksa gudang.");
            }
            if (p == Perubahan.Tujuan) PerbaruiPromptPintu(s);
        }

        IEnumerator Pembuka()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Malam 3: Tujuh Paku di Saka", KalenderJawa.NamaMalam(3) + "\n\n" + RingkasanMalam3 + "\n\n" + TujuanBab, 8f));
            yield return PakdeDatang(State);
            sibuk = false;
        }

        // Game Story Malam 3 butir 1: Pakde Darmo datang membawa buah, bersikap sangat peduli, lalu membujuk Raka menjual
        // tanah "untuk biaya rumah sakit". Raka menolak. Desain Bagian 5: adegan berskrip (dialog).
        IEnumerator PakdeDatang(GameState s)
        {
            suaraMotorDatang.Putar();
            adegan.Mulai();
            try
            {
                suaraKetuk.Putar();
                yield return kartu.StartCoroutine(kartu.KeHitam());
                pemain.Pindah(titikTamu);
                pakde.SetActive(true);
                buahTangan.SetActive(true);
                adegan.Bidik(bidikPakde);
                yield return kartu.StartCoroutine(kartu.DariHitam());
                yield return dialog.MulaiDanTunggu(dialogPakde);   // efek: katanya:K4, tujuan:PakdeDatang
                yield return kartu.StartCoroutine(kartu.KeHitam());
                pakde.SetActive(false);
                suaraMotorPergi.Putar();
                pemain.Pindah(titikKamarRaka);
            }
            finally { adegan.Selesai(); }
            if (!TujuanMalam.Sudah(s, TujuanMalam.PakdeDatang))
            {
                Debug.LogError("Dialog Pakde selesai tanpa efek tujuan:PakdeDatang; ditandai sekarang.", this);
                TujuanMalam.Selesaikan(s, TujuanMalam.PakdeDatang);
            }
            yield return kartu.StartCoroutine(kartu.DariHitam());
            hud.Tampilkan("Pakde pergi dengan wajah masam. Raka masuk ke kamarnya.");
            hud.Tampilkan("Ada kotak sepatu lama di dekat dinding kamar.");
        }

        // "Lanjutkan" atau muat ulang setelah tertangkap, sesudah Pakde pergi.
        IEnumerator Lanjut()
        {
            sibuk = true;
            pemain.Pindah(titikRuangTengah);
            yield return kartu.StartCoroutine(kartu.Tampilkan(KalenderJawa.NamaMalam(3), TujuanBab, 3f));
            sibuk = false;
        }

        // Kembali dari jalan desa setelah berkunjung ke Bu Sumi (K17-B; desa dan rumah Bu Sumi tidak punya checkpoint).
        IEnumerator Pulang(GameState s)
        {
            sibuk = true;
            pemain.Pindah(titikPulang);
            yield return kartu.StartCoroutine(kartu.DariHitam());
            if (!s.RitualDiketahui) hud.Tampilkan("Raka belum tahu cara menetralkan paku itu. Mungkin Bapak menulisnya; periksa gudang.");
            else if (!RitualPenangkal.BahanLengkap(s)) hud.Tampilkan("Bahan ritual belum lengkap. Halaman 4 menyebut tempatnya (Jurnal, Tab).");
            else hud.Tampilkan("Bahan ritual sudah lengkap. Anglo ada di dapur.");
            sibuk = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (!berjalan || sibuk || gm == null || gm.Dijeda) return;
            var s = gm.State;
            if (s.MalamKe != 3 || s.Ending != Ending.Belum) return;
            if (sosokBerjalan) AturSosok();
            JamBerdentang(s);
            if (!jeritanPertama && s.HalamanDiary[Kode.H3])
            {
                StartCoroutine(JeritanPertama());
                return;
            }
            if (!petunjukBuSumi && TujuanMalam.Sudah(s, TujuanMalam.CabutPaku))
            {
                petunjukBuSumi = true;
                hud.Tampilkan("Paku ketujuh tercabut. Jeritan Ibu berhenti.");
                hud.Tampilkan("Raka teringat Bu Sumi, peracik jamu di dekat jembatan. Keluar lewat pintu depan.");
            }
            if (sosokBerjalan || !TujuanMalam.MalamSelesai(s)) return;
            if (!JamGim.ObatSelesai(s))   // K19-A
            {
                if (!petunjukObat)
                {
                    petunjukObat = true;
                    hud.Tampilkan($"Obat Ibu pukul {JamGim.Format(JamGim.MulaiJendelaObat(3))}. Kursi di kamar Ibu bisa dipakai menunggu.");
                }
                return;
            }
            if (!gantiDimulai && PergantianMalam.BisaLanjut(s)) StartCoroutine(GantiMalam(s));
        }

        // Game Story Malam 3 butir 3: setiap kali jam dinding berdentang, Ibu menjerit seperti ditusuk (sampai paku dicabut).
        void JamBerdentang(GameState s)
        {
            bool dentang = JamGim.JamBerdentang(menitTerakhir, s.MenitGim);
            menitTerakhir = s.MenitGim;
            if (!dentang || !jeritanPertama || TujuanMalam.Sudah(s, TujuanMalam.CabutPaku)) return;
            suaraDentang.Putar();
            suaraJerit.Putar();
            hud.Tampilkan("Jam berdentang. Ibu menjerit seperti ditusuk.");
        }

        // Jeritan pertama setelah Halaman 3 dibaca (desain Bagian 5: adegan berskrip). Jam dentang berikutnya hanya bunyi.
        IEnumerator JeritanPertama()
        {
            sibuk = true;
            jeritanPertama = true;
            adegan.Mulai(bidikIbu);
            try
            {
                suaraDentang.Putar();
                suaraJerit.Putar();
                yield return Guncang.Posisi(ibu, LamaJerit, KuatJerit);
            }
            finally { adegan.Selesai(); }
            hud.Tampilkan("Setiap kali jam dinding berdentang, Ibu menjerit seperti ditusuk.");
            hud.Tampilkan("Buka kompas (C). Jarumnya berputar ke arah saka guru di ruang tengah.");
            sibuk = false;
        }

        // Game Story Malam 3: salah urutan membuat Sosok Santet muncul. Kondisi Ibu -5 sudah diberikan TujuhPaku.Cabut.
        void SaatPakuDicabut(HasilCabut hasil)
        {
            if (!berjalan) return;
            if (hasil == HasilCabut.Benar)
            {
                suaraCabut.Putar();
                return;
            }
            var gm = GameManager.Instance;
            hud.Tampilkan("Paku itu tidak mau lepas. Ibu mengerang dari kamarnya.");
            if (KeMenuJikaEnding(gm)) return;
            // K21-A: checkpoint sesudah penalti dan sebelum Sosok muncul (desain 2.6), supaya penalti dan paku yang sudah
            // dicabut tidak hilang saat tertangkap.
            gm.SimpanCheckpoint();
            if (!sosokBerjalan) SosokMuncul();
        }

        void SosokMuncul()
        {
            sosokBerjalan = true;
            sisaSosok = LamaSosok;
            suaraStinger.Putar();
            sosok.transform.SetPositionAndRotation(titikMunculSosok.position, titikMunculSosok.rotation);
            sosok.gameObject.SetActive(true);   // SosokSantet.OnEnable menaruhnya di NavMesh dan mulai berpatroli
            musikTegang.Putar();
            hud.Tampilkan("Gemerincing paku terdengar dari dapur. Bersembunyi (E di lemari) dan tahan napas (Spasi).");
        }

        // Sosok pergi setelah LamaSosok detik waktu gim (berhenti saat jeda). Detak jantung selama Raka bersembunyi.
        void AturSosok()
        {
            if (pemain.Bersembunyi != detakBerbunyi)
            {
                detakBerbunyi = pemain.Bersembunyi;
                if (detakBerbunyi) suaraDetak.Putar();
                else suaraDetak.Hentikan();
            }
            sisaSosok -= Time.deltaTime;
            if (sisaSosok > 0f) return;
            sosokBerjalan = false;
            sosok.gameObject.SetActive(false);
            musikTegang.Hentikan();
            suaraDetak.Hentikan();
            detakBerbunyi = false;
            hud.Tampilkan("Gemerincing paku itu menjauh, lalu hilang.");
        }

        void SaatTertangkap()
        {
            if (!sosokBerjalan) return;
            sosokBerjalan = false;
            StartCoroutine(Tertangkap());
        }

        // Desain 4.2: muat checkpoint (salah cabut terakhir, K21-A), lalu Kondisi Ibu -5. Scene dimuat ulang dari state itu.
        IEnumerator Tertangkap()
        {
            sibuk = true;
            var gm = GameManager.Instance;
            adegan.Mulai();
            musikTegang.Hentikan();
            suaraDetak.Hentikan();
            suaraStinger.Putar();
            yield return kartu.StartCoroutine(kartu.Tampilkan("", "Sosok itu menemukan Raka.", 2f));
            yield return kartu.StartCoroutine(kartu.KeHitam());
            gm.Tertangkap();
            // Jeda dilepas KameraAdegan.OnDisable saat scene dibongkar.
            gm.MuatScene(gm.State.Ending != Ending.Belum ? GameManager.SceneMenuUtama : gm.SceneUntukState);
        }

        // Game Story Malam 3 butir 7: Raka pergi ke rumah Bu Sumi (K17-B: berjalan lewat Scene_Desa).
        void SaatPintuDepan()
        {
            if (!berjalan || sibuk) return;
            var s = State;
            if (!TujuanMalam.Sudah(s, TujuanMalam.CabutPaku))
                hud.Tampilkan("Raka tidak bisa pergi selama Ibu terus menjerit.");
            else if (TujuanMalam.Sudah(s, TujuanMalam.KunjungiSumi))
                hud.Tampilkan("Sudah larut. Raka tidak keluar lagi malam ini.");
            else if (sosokBerjalan)
                hud.Tampilkan("Gemerincing paku masih terdengar. Jangan keluar sekarang.");
            else
                StartCoroutine(KeRumahBuSumi());
        }

        IEnumerator KeRumahBuSumi()
        {
            sibuk = true;
            adegan.Mulai();
            suaraPintuDepan.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            GameManager.Instance.MuatScene(PetaScene.Desa);   // AlurDesaMalam3 menaruh Raka di depan rumah keluarga
        }

        // Ending sebelum Fase 7: kembali ke menu utama (layar ending dibuat di Fase 7).
        bool KeMenuJikaEnding(GameManager gm)
        {
            if (gm.State.Ending == Ending.Belum) return false;
            Debug.LogWarning("Ending tercapai di Malam 3; layar ending dibuat di Fase 7.", this);
            gm.MuatScene(GameManager.SceneMenuUtama);
            return true;
        }

        IEnumerator GantiMalam(GameState s)
        {
            gantiDimulai = true;
            sibuk = true;
            var gm = GameManager.Instance;
            bool dimuat = false;
            adegan.Mulai();
            try
            {
                yield return kartu.StartCoroutine(kartu.KeHitam());
                PergantianMalam.Lanjut(s);        // Malam 4, Kondisi Ibu -12
                if (KeMenuJikaEnding(gm))         // catatan 0B: -12 bisa langsung memicu ending
                {
                    dimuat = true;
                    yield break;
                }
                gm.SimpanCheckpoint();            // checkpoint awal Malam 4
                // Fase 4 mengganti bagian di bawah ini dengan gm.MuatScene(gm.SceneUntukState) untuk sutradara Malam 4.
                yield return kartu.StartCoroutine(kartu.Tampilkan(
                    "Malam 4: Rambut di Dasar Sumur",
                    KalenderJawa.NamaMalam(4) + "\n\n" + RingkasanMalam4 + "\n\n(Bersambung. Malam 4 dibuat di Fase 4.)", 8f));
                yield return kartu.StartCoroutine(kartu.KeHitam());
                gm.MuatScene(GameManager.SceneMenuUtama);
                dimuat = true;
            }
            finally { if (!dimuat) adegan.Selesai(); }   // jeda dilepas KameraAdegan.OnDisable saat scene dibongkar
        }
    }
}
