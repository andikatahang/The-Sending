using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Membuat Scene_Epilog (K50-A): tanpa pemain. GameManager, satu kamera PSX (AudioListener) yang digerakkan Timeline,
    // KartuTeks, dan set-set kecil berjauhan (x berselang 50 m) dari prefab yang ada. AlurEpilog memilih bidikan menurut ending.
    // Teks yang ditandai "dikutip" persis Game Story 8.2-8.5; selebihnya DRAF. Jangan sunting scene dengan tangan.
    public static class PembuatEpilog
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.Epilog + ".unity";
        const float LamaBidikan = 5f;

        static GameObject kamera;

        [MenuItem("The Sending/Buat Scene Epilog")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();

            new GameObject("GameManager").AddComponent<GameManager>();
            kamera = new GameObject("Kamera") { tag = "MainCamera" };   // di akar scene: Timeline menggerakkannya (PembuatTimeline.Gerak)
            var cam = kamera.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            PembuatGayaPsx.SiapkanKamera(cam);
            kamera.AddComponent<AudioListener>();
            var kartu = new GameObject("UI").AddComponent<KartuTeks>();

            var pagi = new GameObject("CahayaPagi").AddComponent<Light>();
            pagi.type = LightType.Directional;
            pagi.color = Hex("FFE2B8");
            pagi.intensity = 1.1f;
            pagi.transform.rotation = Quaternion.Euler(35f, 140f, 0f);
            pagi.enabled = false;

            var alur = new GameObject("Sutradara").AddComponent<AlurEpilog>();
            Set(alur, "kartu", kartu);
            Set(alur, "kamera", cam);
            Set(alur, "cahayaPagi", pagi);
            Set(alur, "musik", Musik());

            Sejati(alur);
            Fitnah(alur);
            Balas(alur);
            Terlambat(alur);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene epilog dibuat: {JalurScene}");
        }

        // GS 8.2.
        static void Sejati(AlurEpilog alur)
        {
            var kamar = Kamar("KamarIbuPagi", 0f, malam: false);
            Pasang("Laras", kamar, new Vector3(1.1f, 0f, -0.9f), -90f);
            var teras = Teras(50f);
            var sumi = RumahSumi(100f);
            Isi(alur, "sejati",
                B(kamar, Bidik("Epilog_Sejati_1", kamar, new Vector3(1.8f, 1.6f, 1.8f), new Vector3(0f, 0.7f, -1.2f), new Vector3(1f, 1.3f, 0.6f), new Vector3(0f, 0.6f, -1.5f)), true,
                    "Ending Sejati: Terang",
                    "Kendi induk terbakar dan Sosok Santet menjerit lalu lenyap menjadi asap. Pagi harinya, Ibu membuka mata untuk pertama kali dalam beberapa hari dan meminta air."),   // dikutip
                B(teras, Bidik("Epilog_Sejati_2", teras, new Vector3(0f, 1.8f, 8f), new Vector3(0f, 1.2f, 2f), new Vector3(0.5f, 1.7f, 5.5f), new Vector3(0f, 1.2f, 2f)), true,
                    "", "Bidan Wulan datang dan Ibu akhirnya bersedia dirujuk ke rumah sakit. Pak Joko datang bersama Bripka Anton."),   // dikutip
                B(teras, Bidik("Epilog_Sejati_3", teras, new Vector3(1.8f, 1.6f, 3.6f), new Vector3(0.6f, 1.5f, 2.2f), new Vector3(1.4f, 1.6f, 3.1f), new Vector3(0.6f, 1.5f, 2.2f)), true,
                    "Bripka Anton",
                    "Sekarang ada pasal dalam KUHP baru tentang orang yang mengaku punya kekuatan gaib dan menawarkan jasa untuk mencelakai orang. Yang dibuktikan perbuatannya, bukan gaibnya. Catatan pesanan ini sangat membantu, Mas."),   // dikutip
                B(sumi, Bidik("Epilog_Sejati_4", sumi, new Vector3(3f, 1.6f, 7f), new Vector3(-1.5f, 1.2f, 4f), new Vector3(1.2f, 1.5f, 5.6f), new Vector3(-1.5f, 1.2f, 3.8f)), true,
                    "", "Nama baik Bu Sumi dipulihkan di depan warga. Raka datang ke rumahnya, meminta maaf atas nama keluarga, dan menawarkan bantuan."),   // dikutip
                B(null, null, false, "Raka",
                    "Yang membunuh Bapak bukan cuma ilmu hitam. Tapi juga keserakahan, diam, dan orang-orang yang lebih percaya gosip daripada bukti."),   // dikutip
                Pesan("Mencari kebenaran dengan bukti dan menempuh jalur hukum adalah cara yang benar untuk melawan kejahatan."));   // dikutip
        }

        // GS 8.3.
        static void Fitnah(AlurEpilog alur)
        {
            var makam = Pemakaman(150f);
            var puing = Lahan("PuingRumahSumi", 200f);
            Pasang("PuingRumahKecil", puing, Vector3.zero);
            Pasang("Raka", puing, new Vector3(0.6f, 0f, 4.5f), 180f);
            Isi(alur, "fitnah",
                B(makam, Bidik("Epilog_Fitnah_1", makam, new Vector3(0f, 2.6f, 5.5f), new Vector3(0f, 0.8f, -2f), new Vector3(0.8f, 2.2f, 3.8f), new Vector3(0f, 0.9f, -2f)), true,
                    "Ending Fitnah: Api", "Ibu meninggal tepat tengah malam. Beberapa hari kemudian, pemakaman berlangsung di bawah gerimis."),   // dikutip
                B(makam, Bidik("Epilog_Fitnah_2", makam, new Vector3(-1.1f, 1.6f, 0.4f), new Vector3(-1.4f, 1.5f, -1.6f), new Vector3(-1.2f, 1.6f, -0.3f), new Vector3(-1.4f, 1.6f, -1.6f)), true,
                    "Pakde Darmo", "Sudah, Le. Ikhlaskan. Urusan tanah biar Pakde yang bantu."),   // dikutip
                B(makam, Bidik("Epilog_Fitnah_3", makam, new Vector3(0.4f, 1.7f, -0.6f), new Vector3(1.6f, 1.4f, -5f), new Vector3(1.1f, 1.6f, -2.8f), new Vector3(1.6f, 1.5f, -5f)), true,
                    "", "Di barisan pelayat paling belakang, Mbah Wiryo berdiri sambil mengusap cincin akiknya."),   // dikutip
                B(puing, Bidik("Epilog_Fitnah_4", puing, new Vector3(1.4f, 1.7f, 7.5f), new Vector3(0f, 0.6f, 0f), new Vector3(0.9f, 1.6f, 6.2f), new Vector3(0f, 0.6f, 0f)), true,
                    "", "Bu Sumi tidak pernah kembali ke desa. Rumahnya tinggal puing hitam. Raka menatap rumah Bu Sumi yang hangus dan akhirnya sadar siapa yang sebenarnya bersalah, tetapi semuanya sudah terlambat."),   // dikutip
                Pesan("Menghakimi tanpa bukti tidak hanya melukai orang yang tidak bersalah, tetapi juga membiarkan pelaku sebenarnya lolos."));   // dikutip
        }

        // GS 8.4. Kematian Pakde hanya teks (tanpa gambar). Cermin tanpa pantulan nyata: Sosok berdiri di belakang Raka.
        static void Balas(AlurEpilog alur)
        {
            var mandi = KamarMandi(300f);
            Isi(alur, "balas",
                B(null, null, false, "Ending Balas Dendam: Santet Balik",
                    "Raka membacakan mantra pambalik. Ibu perlahan pulih. Tiga hari kemudian, Pakde Darmo ditemukan meninggal di rumahnya dengan mulut penuh paku. Warga mulai berbisik tentang Raka."),   // dikutip
                B(mandi, Bidik("Epilog_Balas_1", mandi, new Vector3(1.1f, 1.6f, 0.9f), new Vector3(0f, 1.5f, -1.3f), new Vector3(0.8f, 1.6f, 0.2f), new Vector3(0f, 1.5f, -1.3f)), false,
                    "", "Malam itu, Raka bercermin di kamar mandi. Hidungnya berdarah. Di cermin, Sosok Santet berdiri tepat di belakangnya."),   // dikutip
                B(mandi, Bidik("Epilog_Balas_2", mandi, new Vector3(0.5f, 1.7f, -0.2f), new Vector3(0f, 1.5f, -1.4f), new Vector3(0.3f, 1.6f, -0.7f), new Vector3(0f, 1.5f, -1.4f)), false,
                    "Suara Mbah Wiryo", "Sing wis nyilih, kudu mbayar, Le.\n(Yang sudah meminjam, harus membayar, Nak.)"),   // dikutip
                Pesan("Membalas kejahatan dengan kejahatan hanya membuat kita menjadi pelaku berikutnya."));   // dikutip
        }

        // GS 8.5.
        static void Terlambat(AlurEpilog alur)
        {
            var kamar = Kamar("KamarIbuMalam", 250f, malam: true);
            Pasang("Raka", kamar, new Vector3(1.0f, 0f, -0.9f), -90f);
            var laras = Kamar("KamarLaras", 350f, malam: false, ibu: false);
            Pasang("Laras", laras, new Vector3(1.0f, 0f, -1.0f), -90f);
            Pasang("Raka", laras, new Vector3(1.8f, 0f, 1.6f), 200f);
            Isi(alur, "terlambat",
                B(kamar, Bidik("Epilog_Terlambat_1", kamar, new Vector3(2f, 1.5f, 1.5f), new Vector3(0.3f, 0.8f, -1.2f), new Vector3(1.6f, 1.3f, 0.6f), new Vector3(0.3f, 0.8f, -1.3f)), false,
                    "Ending Terlambat: Malam Ketujuh", "Ibu mengembuskan napas terakhir. Raka terduduk memegang media santet yang belum sempat dinetralkan."),   // dikutip
                B(laras, Bidik("Epilog_Terlambat_2", laras, new Vector3(1.8f, 1.6f, 2.2f), new Vector3(1f, 1.2f, -1f), new Vector3(1.5f, 1.6f, 1.2f), new Vector3(1f, 1.2f, -1f)), true,
                    "", "Pagi harinya, terdengar suara Laras terbatuk keras dari kamarnya. Saat Raka membuka pintu, Laras memuntahkan segumpal rambut."),   // dikutip
                B(null, null, false, "Santet belum selesai.", ""),   // dikutip
                Pesan("Setiap keputusan dan setiap waktu yang terbuang punya akibat."));   // dikutip
        }

        // ---------- Set ----------

        static Transform Induk(string nama, float x)
        {
            var t = new GameObject("Set " + nama).transform;
            t.position = new Vector3(x, 0f, 0f);
            return t;
        }

        static void Dinding(Transform set, string nama, Vector3 posisi, Vector3 ukuran, string material) =>
            Balok(nama, posisi, ukuran, material).transform.SetParent(set, false);   // posisi dunia sementara jadi posisi lokal

        static void Cahaya(Transform set, string nama, Vector3 posisi, string hex, float jangkauan, float intensitas) =>
            Lampu(nama, posisi, hex, jangkauan, intensitas).transform.SetParent(set, false);

        static GameObject Pasang(string prefab, Transform set, Vector3 posisi, float putarY = 0f) =>
            PembuatPengganti.Pasang(prefab, set, posisi, putarY);

        // Kamar 5 x 5 m terbuka ke +Z (kamera di sisi itu); ranjang di dinding belakang, kepala ke -Z.
        static Transform Kamar(string nama, float x, bool malam, bool ibu = true)
        {
            var set = Induk(nama, x);
            Dinding(set, "Lantai", new Vector3(0f, -0.1f, 0f), new Vector3(5f, 0.2f, 5f), "Keramik");
            Dinding(set, "DindingBelakang", new Vector3(0f, 1.5f, -2.5f), new Vector3(5f, 3f, 0.2f), "DindingRumah");
            Dinding(set, "DindingKiri", new Vector3(-2.5f, 1.5f, 0f), new Vector3(0.2f, 3f, 5f), "DindingRumah");
            Dinding(set, "DindingKanan", new Vector3(2.5f, 1.5f, 0f), new Vector3(0.2f, 3f, 5f), "DindingRumah");
            Pasang("Ranjang", set, new Vector3(0f, 0f, -1.3f));
            if (ibu) Pasang("BuRatmi_Berbaring", set, new Vector3(0f, 0.54f, -1.3f));
            if (malam) Cahaya(set, "Teplok", new Vector3(-1.6f, 1.4f, -1.8f), "E8A050", 6f, 1.2f);
            else Cahaya(set, "CahayaJendela", new Vector3(1.8f, 2.2f, 0.5f), "FFE8C8", 8f, 1.2f);
            return set;
        }

        static Transform Lahan(string nama, float x)
        {
            var set = Induk(nama, x);
            Dinding(set, "Tanah", new Vector3(0f, -0.1f, 0f), new Vector3(24f, 0.2f, 24f), "Tanah");
            return set;
        }

        static Transform Teras(float x)
        {
            var set = Lahan("TerasRumah", x);
            Pasang("RumahKeluargaLuar", set, Vector3.zero);
            Pasang("Raka", set, new Vector3(-0.6f, 0f, 2.2f), 90f);
            Pasang("BripkaAnton", set, new Vector3(0.6f, 0f, 2.2f), -90f);
            Pasang("PakJoko", set, new Vector3(1.4f, 0f, 2.9f), -120f);
            Pasang("BidanWulan", set, new Vector3(-1.6f, 0f, 1.5f), 150f);
            return set;
        }

        static Transform RumahSumi(float x)
        {
            var set = Lahan("RumahSumiPagi", x);
            Pasang("RumahKecil", set, Vector3.zero);
            Pasang("BuSumi", set, new Vector3(-1.5f, 0f, 3.2f));
            Pasang("Raka", set, new Vector3(-1.5f, 0f, 4.4f), 180f);
            return set;
        }

        // Makam baru di tengah; pelayat di sisi -Z menghadap makam (+Z), Mbah Wiryo paling belakang. Gerimis partikel.
        static Transform Pemakaman(float x)
        {
            var set = Lahan("Pemakaman", x);
            Pasang("GundukanMakam", set, Vector3.zero);
            Pasang("NisanBatu", set, new Vector3(0f, 0f, 1.1f), 180f);
            Pasang("Kamboja", set, new Vector3(3.5f, 0f, 2.5f));
            foreach (var p in new[] { new Vector3(-3f, 0f, 1f), new Vector3(3f, 0f, -1f), new Vector3(-4f, 0f, 4f), new Vector3(2f, 0f, 5f) })
            {
                Pasang("GundukanMakam", set, p);
                Pasang("NisanKayu", set, p + new Vector3(0f, 0f, 1.1f), 180f);
            }
            Pasang("Raka", set, new Vector3(-0.7f, 0f, -1.6f));
            Pasang("PakdeDarmo", set, new Vector3(-1.4f, 0f, -1.7f), 20f);
            Pasang("Laras", set, new Vector3(0.4f, 0f, -1.8f));
            string[] warga = { "Warga1", "Warga2", "Warga3", "Warga4", "Warga5" };
            for (int i = 0; i < warga.Length; i++)
            {
                var w = Pasang(warga[i], set, new Vector3(-2f + i, 0f, -2.9f - (i % 2) * 0.4f));
                if (i % 2 == 0) Pasang("Payung", w.transform, new Vector3(0.25f, 1.05f, 0f));
            }
            Pasang("MbahWiryo", set, new Vector3(1.6f, 0f, -5f));
            Gerimis(set);
            return set;
        }

        static Transform KamarMandi(float x)
        {
            var set = Induk("KamarMandi", x);
            Dinding(set, "Lantai", new Vector3(0f, -0.1f, 0f), new Vector3(3f, 0.2f, 3f), "UbinMandi");
            Dinding(set, "DindingBelakang", new Vector3(0f, 1.4f, -1.5f), new Vector3(3f, 2.8f, 0.2f), "DindingMandi");
            Dinding(set, "DindingKiri", new Vector3(-1.5f, 1.4f, 0f), new Vector3(0.2f, 2.8f, 3f), "DindingMandi");
            Pasang("Cermin", set, new Vector3(0f, 1.55f, -1.38f));
            Pasang("Raka", set, new Vector3(0f, 0f, -0.9f), 180f);
            Pasang("SosokSantet", set, new Vector3(-0.2f, 0f, 0.1f), 180f);
            Cahaya(set, "Bohlam", new Vector3(0.3f, 2.5f, -0.3f), "E8D8A0", 5f, 0.8f);
            return set;
        }

        // Gerimis: partikel jatuh (bentuk kotak di atas set), waktu tak berskala supaya tetap turun saat kartu menjeda.
        static void Gerimis(Transform set)
        {
            var g = new GameObject("Gerimis");
            g.transform.SetParent(set, false);
            g.transform.localPosition = new Vector3(0f, 7f, -1f);
            var ps = g.AddComponent<ParticleSystem>();
            var m = ps.main;
            m.loop = true;
            m.prewarm = true;
            m.useUnscaledTime = true;
            m.startLifetime = 1.2f;
            m.startSpeed = 7f;
            m.startSize = 0.03f;
            m.maxParticles = 800;
            m.startColor = new Color(0.8f, 0.82f, 0.86f, 0.5f);
            m.simulationSpace = ParticleSystemSimulationSpace.World;
            var e = ps.emission;
            e.rateOverTime = 500f;
            var sh = ps.shape;
            sh.shapeType = ParticleSystemShapeType.Box;
            sh.rotation = new Vector3(90f, 0f, 0f);   // pancaran +Z diputar ke bawah
            sh.scale = new Vector3(14f, 14f, 0.1f);
            var r = g.GetComponent<ParticleSystemRenderer>();
            r.renderMode = ParticleSystemRenderMode.Stretch;
            r.lengthScale = 6f;
            r.sharedMaterial = PembuatPengganti.MaterialPartikel("PartikelAsap", false);
        }

        // ---------- Bidikan ----------

        // Kamera bergerak lurus pelan dari (dari, lihat lihatDari) ke (ke, lihat lihatKe); posisi relatif terhadap set.
        static PemutarTimeline Bidik(string nama, Transform set, Vector3 dari, Vector3 lihatDari, Vector3 ke, Vector3 lihatKe)
        {
            var (tl, director, pemutar) = PembuatTimeline.Buat(nama, LamaBidikan);
            var o = set.position;
            var a = Titik("a", o + dari, o + lihatDari);
            var b = Titik("b", o + ke, o + lihatKe);
            PembuatTimeline.Gerak(tl, director, kamera, (0f, a), (LamaBidikan, b));
            Object.DestroyImmediate(a.gameObject);
            Object.DestroyImmediate(b.gameObject);
            PembuatTimeline.Simpan(tl);
            return pemutar;
        }

        static (Transform set, PemutarTimeline tl, bool pagi, string judul, string teks) B(
            Transform set, PemutarTimeline tl, bool pagi, string judul, string teks) => (set, tl, pagi, judul, teks);

        static (Transform, PemutarTimeline, bool, string, string) Pesan(string teks) => (null, null, false, "Pesan", teks);

        static void Isi(AlurEpilog alur, string nama, params (Transform set, PemutarTimeline tl, bool pagi, string judul, string teks)[] daftar)
        {
            var so = new SerializedObject(alur);
            var p = so.FindProperty(nama);
            p.arraySize = daftar.Length;
            for (int i = 0; i < daftar.Length; i++)
            {
                var d = daftar[i];
                var e = p.GetArrayElementAtIndex(i);
                e.FindPropertyRelative("set").objectReferenceValue = d.set != null ? d.set.gameObject : null;
                e.FindPropertyRelative("timeline").objectReferenceValue = d.tl;
                e.FindPropertyRelative("pagi").boolValue = d.pagi;
                e.FindPropertyRelative("judul").stringValue = d.judul;
                e.FindPropertyRelative("teks").stringValue = d.teks;
                e.FindPropertyRelative("detik").floatValue = Mathf.Clamp(4f + d.teks.Length / 25f, 4f, 10f);   // kira-kira kecepatan baca
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static Suara Musik()
        {
            var g = new GameObject("Suara MusikPenutup");
            var sumber = g.AddComponent<AudioSource>();
            sumber.playOnAwake = false;
            sumber.loop = true;
            sumber.spatialBlend = 0f;
            sumber.outputAudioMixerGroup = GrupMixer("Music");
            sumber.clip = PembuatPengganti.Klip(PembuatPengganti.FolderMusik, "MusikPenutup");   // null = diam sampai filenya ada
            var suara = g.AddComponent<Suara>();
            Set(suara, "sumber", sumber);
            return suara;
        }
    }
}
