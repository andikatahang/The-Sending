using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Membuat Scene_Watuireng (Malam 7): jalan sawah lurus 70 m ke utara (z 0..70) diapit sawah, halaman dengan kandang
    // cemani (z 70..75), rumah Mbah Wiryo 8 x 6 m (z 75..81) berisi sesaji, lima lilin pasaran, kendi induk, lontar, dan buku
    // catatan, lalu kebun berpagar (z 81..96) dengan gerbang di sudut barat laut (zona lolos kejaran). Sutradara AlurWatuireng.
    // Jangan sunting scene dengan tangan; ubah angka di sini lalu bangun ulang.
    public static class PembuatWatuireng
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.Watuireng + ".unity";
        const float Kabut = 0.1f, Tinggi = 2.6f, Tebal = 0.12f;
        const float UjungJalan = 70f, Rumah0 = 75f, Rumah1 = 81f, UjungKebun = 96f, LebarHalaman = 7f;
        const string Sfx = PembuatPengganti.FolderSfx, Musik = PembuatPengganti.FolderMusik;
        const string WarnaBulan = "6D86A8", WarnaLilin = "FFB060";
        static readonly Vector3 Mulai = new Vector3(0f, 0.05f, 1.5f);
        static readonly Vector3 Kandang = new Vector3(3.8f, 0f, 72f);   // 3,8 m dari jalan tengah: lewat lurus aman (KokokCemani.Jarak 3)
        static readonly Vector3 Kendi = new Vector3(0f, 0f, 78.7f);
        static readonly Vector3 Semadi = new Vector3(0f, 0f, 79.9f);
        // Urutan fisik acak (bukan urutan jawaban LilinSesaji), kiri ke kanan dilihat dari pintu depan.
        static readonly Pasaran[] UrutanLilin = { Pasaran.Pon, Pasaran.Kliwon, Pasaran.Legi, Pasaran.Wage, Pasaran.Pahing };

        [MenuItem("The Sending/Buat Scene Watuireng")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();
            RenderSettings.fogDensity = Kabut;
            var rig = PembuatRig.Buat(Mulai);
            SetAngka(rig.Langkah, "bawaan", (int)JenisPermukaan.Tanah);

            var alur = new GameObject("Sutradara").AddComponent<AlurWatuireng>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "titikMulai", Titik("TitikMulai", Mulai, new Vector3(0f, 0.05f, 10f)));
            Set(alur, "titikKejar", Titik("TitikKejar", new Vector3(0f, 0.05f, 77f), new Vector3(0f, 0.05f, 74f)));
            Set(alur, "dialogKendi", PembuatDataFase7.Muat(PembuatDataFase7.KendiInduk));
            Set(alur, "dialogLontar", PembuatDataFase7.Muat(PembuatDataFase7.Lontar));
            Set(alur, "dialogMemergoki", PembuatDataFase7.Muat(PembuatDataFase7.WiryoMemergoki));
            Set(alur, "suaraKokok", BuatSuara(rig, "KokokCemani", Sfx, "SFX", "(Suara: ayam cemani berkokok ribut.)"));
            Set(alur, "suaraLilin", BuatSuara(rig, "LilinPadam", Sfx, "SFX", "(Suara: lilin padam.)"));
            Set(alur, "suaraStinger", BuatSuara(rig, "StingerKejutan", Sfx, "SFX", "(Suara: dentuman mengejutkan.)"));
            string kejar = PembuatPengganti.AdaSuara(Musik, "MusikKejarWiryo") ? "MusikKejarWiryo"
                : PembuatPengganti.AdaSuara(Musik, "MusikKejar") ? "MusikKejar" : "DroneHoror";
            Set(alur, "musikKejar", BuatSuara(rig, kejar, Musik, "Music", "(Musik kejar.)", ulang: true));

            Lahan();
            Sawah(rig, alur);
            Halaman(alur);
            RumahWiryo(rig, alur);
            Kebun(alur);
            PembuatDekor.Pasang("Watuireng", Dekor);
            Wiryo(rig, alur);
            Memergoki(rig, alur);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene Watuireng dibuat: {JalurScene}");
        }

        // Tanah, batas tak terlihat (jalan sawah selebar 2,4 m, lalu halaman dan kebun 14 m), cahaya bulan dingin.
        static void Lahan()
        {
            Balok("Tanah", new Vector3(0f, -0.1f, 48f), new Vector3(40f, 0.2f, 104f), "Tanah")
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Tanah;
            Batas("BatasSelatan", new Vector3(0f, 1.5f, -0.5f), new Vector3(3f, 3f, 0.2f));
            Batas("BatasJalanBarat", new Vector3(-1.2f, 1.5f, UjungJalan / 2f), new Vector3(0.2f, 3f, UjungJalan + 1f));
            Batas("BatasJalanTimur", new Vector3(1.2f, 1.5f, UjungJalan / 2f), new Vector3(0.2f, 3f, UjungJalan + 1f));
            float sisi = (LebarHalaman - 1.2f) / 2f + 1.2f;
            Batas("BatasHalamanBarat", new Vector3(-sisi, 1.5f, UjungJalan), new Vector3(LebarHalaman - 1.2f, 3f, 0.2f));
            Batas("BatasHalamanTimur", new Vector3(sisi, 1.5f, UjungJalan), new Vector3(LebarHalaman - 1.2f, 3f, 0.2f));
            float tengah = (UjungJalan + UjungKebun) / 2f, panjang = UjungKebun - UjungJalan;
            Batas("BatasBarat", new Vector3(-LebarHalaman, 1.5f, tengah), new Vector3(0.2f, 3f, panjang));
            Batas("BatasTimur", new Vector3(LebarHalaman, 1.5f, tengah), new Vector3(0.2f, 3f, panjang));
            Batas("BatasUtara", new Vector3(0f, 1.5f, UjungKebun + 1.2f), new Vector3(LebarHalaman * 2f, 3f, 0.2f));   // di balik gerbang

            var bulan = new GameObject("CahayaBulan").AddComponent<Light>();
            bulan.type = LightType.Directional;
            bulan.intensity = 0.25f;
            bulan.color = Hex(WarnaBulan);
            bulan.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        // Petak sawah rendah di kedua sisi jalan, pematang melintang tiap 10 m, dan Sosok yang membuntuti dari sisi timur.
        static void Sawah(Rig rig, AlurWatuireng alur)
        {
            var induk = new GameObject("Sawah").transform;
            foreach (float x in new[] { -8.6f, 8.6f })
            {
                var petak = Balok("PetakSawah", new Vector3(x, 0.04f, UjungJalan / 2f), new Vector3(15f, 0.08f, UjungJalan), "Daun");
                petak.transform.SetParent(induk, true);
                for (float z = 10f; z < UjungJalan; z += 10f)
                    Balok("Pematang", new Vector3(x, 0.1f, z), new Vector3(15f, 0.12f, 0.4f), "Tanah").transform.SetParent(induk, true);
            }

            var sosok = PembuatPengganti.Pasang("SosokSantet", null, new Vector3(6f, 0.08f, Mulai.z - 16f));
            sosok.name = "SosokPembuntut";
            foreach (var c in sosok.GetComponentsInChildren<Collider>()) Object.DestroyImmediate(c);   // hanya penampakan
            var pembuntut = sosok.AddComponent<PembuntutSosok>();
            Set(pembuntut, "pemain", rig.Pemain);
            var so = new SerializedObject(pembuntut);
            so.FindProperty("zBerhenti").floatValue = UjungJalan - 6f;
            so.ApplyModifiedPropertiesWithoutUndo();
            Set(alur, "pembuntut", pembuntut);
        }

        // Halaman depan rumah dengan kandang cemani di sisi timur (AlurWatuireng mengukur jarak ke pusat kandang).
        static void Halaman(AlurWatuireng alur)
        {
            var kandang = new GameObject("KandangCemani").transform;
            kandang.position = Kandang;
            PembuatPengganti.Pasang("KandangAyam", kandang, Vector3.zero, -90f);
            PembuatPengganti.Pasang("AyamCemani", kandang, new Vector3(0f, 0.06f, 0f), -90f);
            Set(alur, "kandang", kandang);
        }

        // Dekor di prefab Dekor_Watuireng (PembuatDekor): pagar bambu visual di sisi halaman dan kebun (batas sebenarnya
        // tak terlihat), rumpun bambu dan kelor di kebun.
        static void Dekor(Transform r)
        {
            var pagar = new GameObject("Pagar").transform;
            pagar.SetParent(r, false);
            float tengah = (UjungJalan + UjungKebun) / 2f, panjang = UjungKebun - UjungJalan;
            foreach (float x in new[] { -LebarHalaman, LebarHalaman })
                foreach (float y in new[] { 0.4f, 0.9f })
                    Balok("PalangPagar", new Vector3(x, y, tengah), new Vector3(0.06f, 0.06f, panjang), "Bambu").transform.SetParent(pagar, true);

            var induk = new GameObject("Kebun").transform;
            induk.SetParent(r, false);
            foreach (var p in new[] { new Vector3(-4.5f, 0f, 85f), new Vector3(2.5f, 0f, 86.5f), new Vector3(-1f, 0f, 89.5f),
                         new Vector3(5f, 0f, 91f), new Vector3(-5.2f, 0f, 92.5f), new Vector3(1.5f, 0f, 94f) })
                PembuatPengganti.Pasang("RumpunBambu", induk, p);
            foreach (var p in new[] { new Vector3(4.8f, 0f, 83.5f), new Vector3(-2.5f, 0f, 83f), new Vector3(-3f, 0f, 94.8f) })
                PembuatPengganti.Pasang("PohonKelor", induk, p);
        }

        // Dinding lurus sejajar sumbu, dari (x1, z1) ke (x2, z2).
        static void Dinding(string nama, float x1, float z1, float x2, float z2)
        {
            var ukuran = new Vector3(Mathf.Abs(x2 - x1) + Tebal, Tinggi, Mathf.Abs(z2 - z1) + Tebal);
            Balok(nama, new Vector3((x1 + x2) / 2f, Tinggi / 2f, (z1 + z2) / 2f), ukuran, "Gedek");
        }

        // Rumah 8 x 6 m (x -4..4, z 75..81): pintu depan terbuka di tengah dinding selatan, pintu belakang di timur dinding utara.
        // Wiryo bersemadi menghadap selatan; di depannya kendi induk, lontar, nampan sesaji, dan lima lilin berderet.
        static void RumahWiryo(Rig rig, AlurWatuireng alur)
        {
            const float l = 4f;
            Balok("LantaiRumah", new Vector3(0f, 0.02f, (Rumah0 + Rumah1) / 2f), new Vector3(2f * l, 0.04f, Rumah1 - Rumah0), "KayuTua");
            Balok("AtapRumah", new Vector3(0f, Tinggi + 0.1f, (Rumah0 + Rumah1) / 2f), new Vector3(2f * l + 0.6f, 0.2f, Rumah1 - Rumah0 + 0.6f), "Atap");
            Dinding("DindingBarat", -l, Rumah0, -l, Rumah1);
            Dinding("DindingTimur", l, Rumah0, l, Rumah1);
            Dinding("DindingDepanBarat", -l, Rumah0, -0.6f, Rumah0);
            Dinding("DindingDepanTimur", 0.6f, Rumah0, l, Rumah0);
            Dinding("DindingBelakangBarat", -l, Rumah1, 2.4f, Rumah1);
            Dinding("DindingBelakangTimur", 3.6f, Rumah1, l, Rumah1);
            Balok("AmbangDepan", new Vector3(0f, 2.35f, Rumah0), new Vector3(1.2f, 0.5f, Tebal), "Gedek");
            Balok("AmbangBelakang", new Vector3(3f, 2.35f, Rumah1), new Vector3(1.2f, 0.5f, Tebal), "Gedek");

            var semadi = PembuatPengganti.Pasang("MbahWiryo", null, Semadi, 180f);
            semadi.name = "WiryoSemadi";
            Set(alur, "wiryoSemadi", semadi);
            Set(alur, "titikMunculWiryo", Titik("TitikMunculWiryo", Semadi, new Vector3(0f, 0f, Rumah0)));

            var kendi = Logika("KendiInduk", Kendi, new Vector3(0.5f, 0.6f, 0.5f));
            PembuatPengganti.Pasang("KendiInduk", kendi.transform, Vector3.zero);
            var aKendi = kendi.AddComponent<InteraktifAlur>();
            SetTeks(aKendi, "prompt", "Ambil kendi induk");
            Set(alur, "kendi", aKendi);

            var lontar = Logika("Lontar", Kendi + new Vector3(1.1f, 0f, 0f), new Vector3(0.6f, 0.2f, 0.3f));
            PembuatPengganti.Pasang("Lontar", lontar.transform, Vector3.zero);
            var aLontar = lontar.AddComponent<InteraktifAlur>();
            SetTeks(aLontar, "prompt", "Baca lontar");
            Set(alur, "lontar", aLontar);

            var nampan = Logika("NampanSesaji", Kendi + new Vector3(-1.2f, 0f, 0f), new Vector3(0.7f, 0.25f, 0.7f));
            PembuatPengganti.Pasang("NampanSesaji", nampan.transform, Vector3.zero);
            var lihat = nampan.AddComponent<PemicuDialog>();
            Set(lihat, "dialog", PembuatDataFase7.Muat(PembuatDataFase7.SesajiDilihat));
            Set(lihat, "panel", rig.Dialog);
            SetTeks(lihat, "prompt", "Lihat sesaji");

            var daftar = new Object[UrutanLilin.Length];
            for (int i = 0; i < UrutanLilin.Length; i++)
            {
                var p = new Vector3(-1f + 0.5f * i, 0f, Kendi.z - 0.7f);
                var g = Logika("Lilin" + UrutanLilin[i], p, new Vector3(0.3f, 0.35f, 0.3f));
                var visual = PembuatPengganti.Pasang("LilinSesaji", g.transform, Vector3.zero);
                var cahaya = Lampu("Cahaya", p + new Vector3(0f, 0.3f, 0f), WarnaLilin, 1.8f, 0.5f);
                cahaya.transform.SetParent(g.transform, true);
                var lilin = g.AddComponent<LilinPasaran>();
                SetAngka(lilin, "Pasaran", (int)UrutanLilin[i]);
                Set(lilin, "cahaya", cahaya);
                var api = visual.transform.Find("Api");   // bagian pengganti diberi nama materialnya
                if (api != null) Set(lilin, "nyala", api.gameObject);
                daftar[i] = lilin;
            }
            SetDaftar(alur, "lilin", daftar);

            // Meja dengan buku catatan (B8, sekali saja) dan teplok redup di sudut barat daya.
            var meja = Logika("MejaBuku", new Vector3(-3.2f, 0f, 76.2f), new Vector3(0.6f, 0.8f, 1f));
            PembuatPengganti.Pasang("Meja", meja.transform, Vector3.zero, 90f);
            PembuatPengganti.Pasang("BukuCatatan", meja.transform, new Vector3(0f, 0.76f, 0.15f), 90f);
            PembuatPengganti.Pasang("Teplok", meja.transform, new Vector3(0f, 0.76f, -0.3f));
            var buku = meja.AddComponent<PemicuDialog>();
            Set(buku, "dialog", PembuatDataFase7.Muat(PembuatDataFase7.BukuCatatan));
            Set(buku, "panel", rig.Dialog);
            SetTeks(buku, "prompt", "Baca buku catatan");
            SetBool(buku, "sekaliSaja", true);
            Lampu("CahayaTeplok", new Vector3(-3.2f, 1.2f, 75.9f), WarnaLilin, 5f, 0.5f);

            var kemenyan = BuatSuara(rig, "Kemenyan", Sfx, "Ambience", "", ulang: true, ruang: true);   // tanpa file: diam
            kemenyan.transform.position = Kendi + Vector3.up;
            kemenyan.GetComponent<AudioSource>().playOnAwake = true;
        }

        // Kebun di belakang rumah (tanaman di Dekor): gerbang di sudut barat laut (zona lolos menjauh dari rumah).
        static void Kebun(AlurWatuireng alur)
        {
            var gerbang = new Vector3(-5f, 0f, UjungKebun);
            foreach (float dx in new[] { -1.1f, 1.1f })
                Balok("TiangGerbang", gerbang + new Vector3(dx, 0.8f, 0f), new Vector3(0.12f, 1.6f, 0.12f), "Bambu");
            foreach (float y in new[] { 0.4f, 0.9f })
            {
                Balok("PalangUtaraBarat", new Vector3(-6.6f, y, UjungKebun), new Vector3(0.8f, 0.06f, 0.06f), "Bambu");
                Balok("PalangUtaraTimur", new Vector3(1.4f, y, UjungKebun), new Vector3(10.6f, 0.06f, 0.06f), "Bambu");
            }
            Batas("BatasUtaraTimur", new Vector3(1.4f, 1.5f, UjungKebun), new Vector3(10.6f, 3f, 0.2f));
            Batas("BatasUtaraBarat", new Vector3(-6.6f, 1.5f, UjungKebun), new Vector3(0.8f, 3f, 0.2f));
            Set(alur, "gerbangKebun", Zona("ZonaGerbangKebun", gerbang + new Vector3(0f, 1.2f, 0.6f), new Vector3(2f, 2.4f, 0.8f)));
        }

        // Wiryo pengejar (K53): SosokSantet + NavMeshAgent tanpa gemerincing, mati sampai AlurWatuireng menyalakannya.
        static void Wiryo(Rig rig, AlurWatuireng alur)
        {
            var lantai = new GameObject("NavMesh").AddComponent<LantaiNavMesh>();
            lantai.transform.position = new Vector3(0f, 0f, 84f);   // 30 x 30 m bawaan: halaman, rumah, dan kebun

            var wiryo = new GameObject("WiryoPengejar");
            wiryo.transform.SetPositionAndRotation(Semadi, Quaternion.Euler(0f, 180f, 0f));
            PembuatPengganti.Pasang("MbahWiryo", wiryo.transform, Vector3.zero);
            var agen = wiryo.AddComponent<NavMeshAgent>();
            agen.radius = LantaiNavMesh.RadiusAgen;
            agen.height = 1.7f;
            agen.speed = SosokSantet.KecepatanPatroli;
            agen.angularSpeed = 300f;
            agen.acceleration = 8f;
            agen.stoppingDistance = 0.2f;
            agen.enabled = false;   // dinyalakan SosokSantet setelah NavMesh dibangun
            var skrip = wiryo.AddComponent<SosokSantet>();
            Set(skrip, "lantai", lantai);
            Set(skrip, "pemain", rig.Pemain);
            Set(skrip, "senter", rig.Senter);
            SetDaftar(skrip, "titikPatroli", new Object[]
            {
                Titik("PatroliWiryo 1", new Vector3(3f, 0f, 84f), new Vector3(-3f, 0f, 88f)),
                Titik("PatroliWiryo 2", new Vector3(-3f, 0f, 88f), new Vector3(3f, 0f, 92f)),
                Titik("PatroliWiryo 3", new Vector3(3f, 0f, 92f), new Vector3(3f, 0f, 84f)),
            });
            Set(alur, "wiryo", skrip);
            wiryo.SetActive(false);
        }

        // Cutscene Wiryo memergoki (sekitar 3 detik): kamera dari dekat lontar mendongak ke Wiryo yang membuka mata.
        static void Memergoki(Rig rig, AlurWatuireng alur)
        {
            var dari = Titik("BidikMemergokiDari", new Vector3(1.2f, 1.1f, 77.6f), Semadi + Vector3.up * 1.0f);
            var ke = Titik("BidikMemergokiKe", new Vector3(0.4f, 1.5f, 78.4f), Semadi + Vector3.up * 1.6f);
            var c = PembuatTimeline.Buat("Watuireng_Memergoki", 3f);
            PembuatTimeline.Gerak(c.Tl, c.Director, rig.KameraCutscene.gameObject, (0f, dari), (3f, ke));
            PembuatTimeline.Sinyal(c.Tl, 0.6f, AlurWatuireng.SinyalMemergoki);
            PembuatTimeline.Simpan(c.Tl);
            Set(alur, "timelineMemergoki", c.Pemutar);
        }
    }
}
