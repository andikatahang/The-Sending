using TheSending.Core;
using TheSending.Runtime;
using UnityEngine;
using UnityEngine.AI;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Isi Malam 2 di Scene_RumahKeluarga (dipanggil PembuatRumahKeluarga.Buat): sutradara AlurMalam2, panel aksara,
    // papan kopong dan boneka, gudang dan celahnya (Timeline), kotak kayu, tamu, tempat sembunyi, Sosok Santet, suara.
    // Denah rumah tetap (K7); angka di sini hanya menaruh benda di dalamnya. Jangan sunting scene dengan tangan.
    internal static class PembuatMalam2
    {
        const string Sfx = PembuatPengganti.FolderSfx, Musik = PembuatPengganti.FolderMusik;
        const string WarnaBulan = "6D86A8";
        const float TinggiMata = 1.6f;   // = posisi lokal kamera pemain di PembuatRig.Buat

        // Mengembalikan Sosok Santet supaya Malam 3 memakainya lagi (paku salah urutan).
        internal static SosokSantet Pasang(Rig rig, GameObject sutradara, InteraktifAlur pintuGudang, GameObject kalender1, GameObject laras1)
        {
            var alur = sutradara.AddComponent<AlurMalam2>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "dialogWeton", PembuatDataFase2.Muat(PembuatDataFase2.Weton));
            Set(alur, "dialogWiryo", PembuatDataFase2.Muat(PembuatDataFase2.WiryoDatang));
            Set(alur, "dialogBisikan", PembuatDataFase2.Muat(PembuatDataFase2.WiryoBisikan));
            Set(alur, "pintuGudang", pintuGudang);
            Set(alur, "titikMulai", Titik("TitikMulaiMalam2", new Vector3(-4.3f, 0.05f, 5.4f), new Vector3(-6.2f, 0.05f, 6f)));

            // Laras dan kalender Malam 1 diganti versi Malam 2 (Laras Malam 2 diatur AlurMalam2 sendiri).
            AktifPada(sutradara, 0, 1, kalender1, laras1);
            var kalender2 = PembuatRumahKeluarga.BendaDialog(rig, "Kalender", new Vector3(-1.5f, 1.25f, 8.13f),
                new Vector3(0.35f, 0.5f, 0.1f), 0f, true, PembuatDataFase2.Muat(PembuatDataFase2.KalenderMalam2), "Lihat kalender");
            AktifPada(sutradara, 2, 2, kalender2.gameObject);

            Aksara(rig, alur);
            KamarIbu(rig, sutradara, alur);
            Gudang(rig, alur);
            KamarRaka(sutradara);
            Tamu(rig, alur);
            var sosok = Sembunyi(rig, alur);
            SuaraMalam2(rig, alur);
            return sosok;
        }

        internal static void AktifPada(GameObject sutradara, int dari, int sampai, params GameObject[] sasaran)
        {
            var a = sutradara.AddComponent<AktifDiMalam>();
            SetAngka(a, "dari", dari);
            SetAngka(a, "sampai", sampai);
            SetDaftar(a, "sasaran", sasaran);
        }

        static void Aksara(Rig rig, AlurMalam2 alur)
        {
            var panel = rig.Hud.gameObject.AddComponent<PanelAksara>();
            var tabel = PembuatDataFase2.Tabel();
            var suku = PembuatDataFase2.GambarSuku();
            if (tabel != null) Set(panel, "tabel", tabel);
            if (suku != null) SetDaftar(panel, "gambarSuku", suku);
            if (tabel == null || suku == null)
                Debug.LogWarning("Gambar aksara belum lengkap (tools/render-aksara.sh); PanelAksara memakai teks pengganti.");
            Set(alur, "aksara", panel);
        }

        // Papan kopong di sisi timur ranjang Ibu, boneka (M1) di bawahnya, penanda kompas, dan kolong ranjang.
        static void KamarIbu(Rig rig, GameObject sutradara, AlurMalam2 alur)
        {
            var papan = Logika("PapanLantai", new Vector3(-5f, 0f, 6.6f), new Vector3(0.35f, 0.1f, 1.25f));
            PembuatPengganti.Pasang("LubangLantai", papan.transform, Vector3.zero);
            var visualPapan = PembuatPengganti.Pasang("PapanLantai", papan.transform, Vector3.zero);
            var skripPapan = papan.AddComponent<PapanLantai>();
            Set(skripPapan, "papan", visualPapan);
            Set(skripPapan, "hud", rig.Hud);
            SetTeks(skripPapan, "prompt", "Periksa papan lantai");
            Set(alur, "papan", skripPapan);
            Set(alur, "zonaPapan", Zona("ZonaPapan", new Vector3(-5f, 1.2f, 6.6f), new Vector3(0.6f, 2.4f, 1.3f)));

            // Kompas bereaksi mulai Malam 2 (Game Story Malam 2 butir 1); PenandaMedia mengabaikan media yang sudah ditemukan.
            var penanda = new GameObject("MediaM1");
            penanda.transform.position = new Vector3(-5f, 0f, 6.6f);
            SetAngka(penanda.AddComponent<PenandaMedia>(), "media", Kode.M1);
            AktifPada(sutradara, 2, 7, penanda);

            var boneka = Logika("BonekaDiLubang", new Vector3(-5f, 0f, 6.5f), new Vector3(0.3f, 0.2f, 0.45f));
            PembuatPengganti.Pasang("Boneka", boneka.transform, Vector3.zero);
            var baca = boneka.AddComponent<InteraktifAlur>();
            SetTeks(baca, "prompt", "Baca tulisan di boneka");
            Set(alur, "bacaBoneka", baca);
            boneka.SetActive(false);

            // Kolong ranjang: kamera 0,25 m dari lantai, jadi titik dalam berada di bawah lantai (CharacterController mati).
            Persembunyian(rig, "KolongRanjang", new Vector3(-5.55f, 0f, 6f), new Vector3(0.1f, 0.4f, 1.8f), "Bersembunyi di kolong ranjang",
                Titik("DalamKolong", new Vector3(-6.2f, 0.25f - TinggiMata, 6f), new Vector3(-4f, 0.25f - TinggiMata, 6f)),
                Titik("KeluarKolong", new Vector3(-4.8f, 0.05f, 6f), new Vector3(-6.2f, 0.05f, 6f)));
        }

        // Gudang (x 3..7, z 8..12): alat tani, tumpukan karung (tempat sembunyi), linggis, celah di dinding timur.
        static void Gudang(Rig rig, AlurMalam2 alur)
        {
            PembuatPengganti.Pasang("AlatTani", null, new Vector3(4.2f, 0f, 11.75f), 180f);
            Persembunyian(rig, "Karung", new Vector3(5.6f, 0f, 11.2f), new Vector3(1.4f, 0.95f, 0.8f), "Bersembunyi di balik karung",
                Titik("DalamKarung", new Vector3(6.5f, 0.9f - TinggiMata, 11.6f), new Vector3(5f, 0.9f - TinggiMata, 9f)),
                Titik("KeluarKarung", new Vector3(5.6f, 0.05f, 10.2f), new Vector3(5.6f, 0.05f, 9f)), "Karung");

            var linggis = Logika("Linggis", new Vector3(6.75f, 0f, 8.6f), new Vector3(0.25f, 1.2f, 0.25f));
            PembuatPengganti.Pasang("Linggis", linggis.transform, Vector3.zero);
            var ambil = linggis.AddComponent<AmbilBarang>();
            SetTeks(ambil, "barang", Inventaris.Linggis);
            SetTeks(ambil, "prompt", "Ambil linggis");

            Set(alur, "zonaGudang", Zona("ZonaGudang", new Vector3(5f, 1.2f, 10f), new Vector3(3.6f, 2.4f, 3.6f)));
            Celah(rig, alur);
        }

        // Celah dinding bambu: tanah dan cahaya bulan di luar, bidikan kamera, sosok yang lewat, dan Timeline-nya (4 detik).
        static void Celah(Rig rig, AlurMalam2 alur)
        {
            Balok("TanahLuar", new Vector3(9.5f, -0.1f, 10f), new Vector3(5f, 0.2f, 6f), "Tanah")
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Tanah;
            var bulan = new GameObject("Cahaya bulan").AddComponent<Light>();
            bulan.type = LightType.Spot;
            bulan.color = Hex(WarnaBulan);
            bulan.range = 5f;
            bulan.spotAngle = 60f;
            bulan.intensity = 2f;
            bulan.transform.SetPositionAndRotation(new Vector3(8.4f, 3.2f, 10f), Quaternion.Euler(90f, 0f, 0f));

            var bidik = Titik("BidikCelah", new Vector3(6.2f, 1.55f, 10f), new Vector3(8f, 1.55f, 10f));
            var dekat = Titik("BidikCelahDekat", new Vector3(6.75f, 1.55f, 10f), new Vector3(8f, 1.55f, 10f));
            var dari = Titik("SosokCelahDari", new Vector3(8.2f, 0f, 12.2f), new Vector3(8.2f, 0f, 7f));
            var ke = Titik("SosokCelahKe", new Vector3(8.2f, 0f, 7.8f), new Vector3(8.2f, 0f, 3f));
            var sosok = PembuatPengganti.Pasang("SosokSantet", null, dari.position, dari.eulerAngles.y);
            sosok.name = "SosokCelah";

            var c = PembuatTimeline.Buat("Malam2_CelahGudang", 4f);
            PembuatTimeline.Gerak(c.Tl, c.Director, rig.KameraCutscene.gameObject, (0f, bidik), (4f, dekat));
            PembuatTimeline.Gerak(c.Tl, c.Director, sosok, (0f, dari), (4f, ke));
            PembuatTimeline.Sinyal(c.Tl, 1.9f, AlurMalam2.SinyalStinger);   // sosok melintas di depan celah sekitar detik 2
            PembuatTimeline.Simpan(c.Tl);
            Set(alur, "timelineCelah", c.Pemutar);
            Set(alur, "bidikCelah", bidik);
            Set(alur, "sosokCelah", sosok);
            sosok.SetActive(false);
        }

        // Kotak kayu kamar Raka; boneka tampil di atasnya jika Raka menyimpannya (BonekaDiKotak).
        static void KamarRaka(GameObject sutradara)
        {
            PembuatPengganti.Pasang("KotakKayu", null, new Vector3(-6.2f, 0f, 3.55f));
            var boneka = PembuatPengganti.Pasang("Boneka", null, new Vector3(-6.2f, 0.33f, 3.55f), 90f);
            boneka.name = "BonekaDiKotak";
            Set(sutradara.AddComponent<BonekaDiKotak>(), "boneka", boneka);
            boneka.SetActive(false);
        }

        // Laras di ruang tengah (memberi kunci gudang) dan kunjungan Mbah Wiryo di pintu depan.
        static void Tamu(Rig rig, AlurMalam2 alur)
        {
            var laras = PembuatRumahKeluarga.BendaDialog(rig, "Laras", new Vector3(0.8f, 0f, 6.6f), new Vector3(0.5f, 1.7f, 0.5f), 180f, false,
                PembuatDataFase2.Muat(PembuatDataFase2.LarasKunciGudang), "Bicara dengan Laras");
            laras.name = "LarasMalam2";
            Set(alur, "larasMalam2", laras.gameObject);
            Set(alur, "larasVisual", laras.transform.GetChild(0));
            laras.gameObject.SetActive(false);

            var wiryo = Logika("MbahWiryo", new Vector3(0f, 0f, 0.9f), new Vector3(0.5f, 1.7f, 0.5f));
            PembuatPengganti.Pasang("MbahWiryo", wiryo.transform, Vector3.zero);
            Set(alur, "mbahWiryo", wiryo);
            wiryo.SetActive(false);

            Set(alur, "titikTamu", Titik("TitikTamu", new Vector3(0f, 0.05f, 2.8f), new Vector3(0f, 0.05f, 0f)));
            Set(alur, "titikLarasTamu", Titik("TitikLarasTamu", new Vector3(1f, 0f, 1.8f), new Vector3(0f, 0f, 0.9f)));
            Set(alur, "bidikWiryo", Titik("BidikWiryo", new Vector3(0.5f, 1.6f, 3f), new Vector3(0f, 1.5f, 0.9f)));
            Set(alur, "bidikLaras", Titik("BidikLaras", new Vector3(-0.3f, 1.55f, 2.9f), new Vector3(1f, 1.35f, 1.8f)));
        }

        // Sekuens bersembunyi pertama (Game Story Malam 2 butir 9): Raka di kamar Laras, Sosok Santet berpatroli di
        // ruang tengah, lemari pakaian di dinding barat untuk bersembunyi, kamar Raka sebagai tujuan.
        static SosokSantet Sembunyi(Rig rig, AlurMalam2 alur)
        {
            var lemari = Persembunyian(rig, "LemariPakaian", new Vector3(-2.55f, 0f, 4.8f), new Vector3(0.6f, 2f, 1f), "Bersembunyi di lemari",
                Titik("DalamLemari", new Vector3(-2.55f, 0.05f, 4.8f), new Vector3(0f, 0.05f, 4.8f)),
                Titik("KeluarLemari", new Vector3(-1.9f, 0.05f, 4.8f), new Vector3(0f, 0.05f, 4.8f)), "LemariPakaian", 90f);
            DindingDalamLemari(lemari.transform);
            Set(alur, "titikKamarLaras", Titik("TitikKamarLaras", new Vector3(6f, 0.05f, 6.5f), new Vector3(3f, 0.05f, 6f)));
            Set(alur, "zonaKamarRaka", Zona("ZonaKamarRaka", new Vector3(-5f, 1.2f, 2f), new Vector3(3.6f, 2.4f, 3.6f)));

            var lantai = new GameObject("NavMesh").AddComponent<LantaiNavMesh>();
            lantai.transform.position = new Vector3(0f, 0f, 6f);

            var sosok = new GameObject("SosokSantet");
            sosok.transform.SetPositionAndRotation(new Vector3(-0.5f, 0f, 10.5f), Quaternion.Euler(0f, 180f, 0f));
            PembuatPengganti.Pasang("SosokSantet", sosok.transform, Vector3.zero);
            var agen = sosok.AddComponent<NavMeshAgent>();
            agen.radius = LantaiNavMesh.RadiusAgen;
            agen.height = 2.2f;
            agen.speed = SosokSantet.KecepatanPatroli;
            agen.angularSpeed = 240f;
            agen.acceleration = 8f;
            agen.stoppingDistance = 0.2f;
            agen.enabled = false;   // dinyalakan SosokSantet setelah NavMesh dibangun
            var skrip = sosok.AddComponent<SosokSantet>();
            Set(skrip, "lantai", lantai);
            Set(skrip, "pemain", rig.Pemain);
            Set(skrip, "senter", rig.Senter);
            SetDaftar(skrip, "titikPatroli", new Object[]
            {
                Titik("Patroli 1", new Vector3(0f, 0f, 7f), new Vector3(1.6f, 0f, 6.8f)),
                Titik("Patroli 2", new Vector3(1.6f, 0f, 6.8f), new Vector3(1.6f, 0f, 1f)),
                Titik("Patroli 3", new Vector3(1.6f, 0f, 1f), new Vector3(-1.2f, 0f, 1f)),
                Titik("Patroli 4", new Vector3(-1.2f, 0f, 1f), new Vector3(-1.6f, 0f, 5f)),
                Titik("Patroli 5", new Vector3(-1.6f, 0f, 5f), new Vector3(0f, 0f, 7f)),
            });
            var gemerincing = BuatSuara(rig, "GemerincingPaku", Sfx, "SFX", "(Suara: gemerincing paku.)", ulang: true, ruang: true);
            gemerincing.transform.SetParent(sosok.transform, false);
            Set(skrip, "gemerincing", gemerincing);
            Set(alur, "sosok", skrip);
            sosok.SetActive(false);
            return skrip;
        }

        // Sisi kubus tidak tergambar dari dalam, jadi dari dalam lemari seluruh ruangan terlihat. Lapisan tipis di dalam
        // (tanpa collider) menutup pandangan; hanya celah 8 cm di antara dua daun pintu yang terbuka ke ruang tengah.
        static void DindingDalamLemari(Transform lemari)
        {
            (Vector3 pusat, Vector3 ukuran)[] sisi =
            {
                // Di dalam model lemari (tinggi 1,85 m, dalam 0,77 m, lebar 1,24 m).
                (new Vector3(-0.27f, 0.9f, 0f), new Vector3(0.02f, 1.8f, 0.96f)),        // belakang
                (new Vector3(0f, 0.9f, -0.48f), new Vector3(0.56f, 1.8f, 0.02f)),        // kiri
                (new Vector3(0f, 0.9f, 0.48f), new Vector3(0.56f, 1.8f, 0.02f)),         // kanan
                (new Vector3(0f, 1.79f, 0f), new Vector3(0.56f, 0.02f, 0.96f)),          // atap
                (new Vector3(0.28f, 0.9f, -0.255f), new Vector3(0.02f, 1.76f, 0.43f)),   // daun pintu
                (new Vector3(0.28f, 0.9f, 0.255f), new Vector3(0.02f, 1.76f, 0.43f)),
            };
            foreach (var (pusat, ukuran) in sisi)
            {
                var g = Balok("SisiDalamLemari", lemari.TransformPoint(pusat), ukuran, "KayuGelap");
                Object.DestroyImmediate(g.GetComponent<Collider>());
                g.transform.SetParent(lemari, true);
            }
        }

        static GameObject Persembunyian(Rig rig, string nama, Vector3 posisi, Vector3 ukuran, string prompt, Transform dalam, Transform keluar,
            string prefab = null, float putarY = 0f)
        {
            var g = Logika(nama, posisi, ukuran);
            if (prefab != null) PembuatPengganti.Pasang(prefab, g.transform, Vector3.zero, putarY);
            var t = g.AddComponent<TempatSembunyi>();
            Set(t, "pemain", rig.Pemain);
            Set(t, "titikDalam", dalam);
            Set(t, "titikKeluar", keluar);
            SetTeks(t, "prompt", prompt);
            return g;
        }

        static void SuaraMalam2(Rig rig, AlurMalam2 alur)
        {
            Set(alur, "suaraKopong", BuatSuara(rig, "PapanKopong", Sfx, "SFX", "(Suara: papan kayu berbunyi kopong.)"));
            Set(alur, "suaraBongkar", BuatSuara(rig, "PapanDibongkar", Sfx, "SFX", "(Suara: papan dicongkel linggis.)"));
            Set(alur, "suaraKunci", BuatSuara(rig, "KunciDibuka", Sfx, "SFX", "(Suara: kunci diputar.)"));
            Set(alur, "suaraPintuGudang", BuatSuara(rig, "PintuGudangBerderit", Sfx, "SFX", "(Suara: pintu gudang berderit.)"));
            Set(alur, "suaraStinger", BuatSuara(rig, "StingerKejutan", Sfx, "SFX", "(Suara: dentuman mengejutkan.)"));
            Set(alur, "suaraKetuk", BuatSuara(rig, "KetukPintu", Sfx, "SFX", "(Suara: ketukan di pintu depan.)"));
            Set(alur, "suaraDetak", BuatSuara(rig, "DetakJantung", Sfx, "SFX", "(Suara: detak jantung.)", ulang: true));
            string tegang = PembuatPengganti.AdaSuara(Musik, "MusikTegang") ? "MusikTegang" : "DroneHoror";
            Set(alur, "musikTegang", BuatSuara(rig, tegang, Musik, "Music", "(Musik tegang.)", ulang: true));
            Set(alur, "gerimis", BuatSuara(rig, "GerimisDiAtap", Sfx, "Ambience", "(Suara: gerimis di atap.)", ulang: true));
        }
    }
}
