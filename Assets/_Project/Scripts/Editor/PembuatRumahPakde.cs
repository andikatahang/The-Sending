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
    // Membuat Scene_RumahPakde (Malam 5): halaman 14 x 10 m berpagar besi, gerbang di selatan (z 0), rumah di utara
    // dengan teras dan pot (kunci laci), ruang kerja 4 x 4 m di sudut timur dengan meja berlaci (B6), mangkuk anjing di
    // sudut barat daya, dan anjing penjaga (K33-A: SosokSantet + NavMesh runtime). Dimuat dari pagar Pakde di Scene_Desa.
    // Jangan sunting scene dengan tangan; ubah angka di sini lalu bangun ulang.
    public static class PembuatRumahPakde
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.RumahPakde + ".unity";
        const float Lebar = 14f, Panjang = 10f, Tinggi = 2.8f, Tebal = 0.15f;
        const string Sfx = PembuatPengganti.FolderSfx;
        const string WarnaBulan = "6D86A8", WarnaLampu = "FFD9A0";
        static readonly Vector3 Mulai = new Vector3(0f, 0.05f, 1.2f);

        [MenuItem("The Sending/Buat Scene Rumah Pakde")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();
            var rig = PembuatRig.Buat(Mulai);
            SetAngka(rig.Langkah, "bawaan", (int)JenisPermukaan.Tanah);

            var alur = new GameObject("Sutradara").AddComponent<AlurRumahPakde>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "titikMulai", Titik("TitikMulai", Mulai, new Vector3(0f, 0.05f, 8f)));
            Set(alur, "suaraGonggong", BuatSuara(rig, "AnjingMenggonggong", Sfx, "SFX", "(Suara: anjing menggonggong keras.)"));
            Set(alur, "suaraMakan", BuatSuara(rig, "AnjingMakan", Sfx, "SFX", "(Suara: anjing mengunyah.)"));
            Set(alur, "suaraPintu", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: gerbang besi berderit.)"));

            Halaman();
            Rumah(rig, alur);
            RuangKerja(rig, alur);
            Mangkuk(rig, alur);
            Anjing(rig, alur);

            var pemanggil = new GameObject("PemanggilSidang").AddComponent<PemanggilSidang>();   // Malam 6, 21.00: ke sidang di desa
            Set(pemanggil, "kartu", rig.Kartu);
            Set(pemanggil, "adegan", rig.Adegan);
            SetTeks(pemanggil, "sceneIni", PetaScene.RumahPakde);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene rumah Pakde dibuat: {JalurScene}");
        }

        // Tanah, pagar besi pengganti (visual) + batas tak terlihat (Ignore Raycast), cahaya bulan.
        static void Halaman()
        {
            Balok("Tanah", new Vector3(0f, -0.1f, Panjang / 2f), new Vector3(Lebar + 2f, 0.2f, Panjang + 8f), "Tanah")
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Tanah;
            Batas("BatasBarat", new Vector3(-Lebar / 2f, 1.5f, Panjang / 2f), new Vector3(0.2f, 3f, Panjang));
            Batas("BatasTimur", new Vector3(Lebar / 2f, 1.5f, Panjang / 2f), new Vector3(0.2f, 3f, Panjang));
            Batas("BatasSelatan", new Vector3(0f, 1.5f, -0.1f), new Vector3(Lebar, 3f, 0.2f));
            Batas("BatasUtara", new Vector3(0f, 1.5f, Panjang + 0.1f), new Vector3(Lebar, 3f, 0.2f));   // di belakang teras

            var pagar = new GameObject("Pagar").transform;
            for (float x = -Lebar / 2f + 1f; x < Lebar / 2f; x += 2f)
                PembuatPengganti.Pasang("PagarBesi", pagar, new Vector3(x, 0f, -0.2f));
            for (float z = 1f; z < Panjang; z += 2f)
            {
                PembuatPengganti.Pasang("PagarBesi", pagar, new Vector3(-Lebar / 2f - 0.1f, 0f, z), 90f);
                PembuatPengganti.Pasang("PagarBesi", pagar, new Vector3(Lebar / 2f + 0.1f, 0f, z), 90f);
            }

            var bulan = new GameObject("CahayaBulan").AddComponent<Light>();
            bulan.type = LightType.Directional;
            bulan.intensity = 0.25f;
            bulan.color = Hex(WarnaBulan);
            bulan.transform.rotation = Quaternion.Euler(50f, 30f, 0f);
        }

        // Rumah menghadap selatan (teras di z 8–10), pot di teras, lampu teras, gerbang keluar di selatan.
        static void Rumah(Rig rig, AlurRumahPakde alur)
        {
            PembuatPengganti.Pasang("RumahPakdeLuar", null, new Vector3(0f, 0f, 13f), 180f);
            Lampu("LampuTeras", new Vector3(0f, 2.6f, 9f), WarnaLampu, 6f, 0.8f);

            var pot = Logika("PotTeras", new Vector3(-2.5f, 0.2f, 9f), new Vector3(0.6f, 1.1f, 0.6f));
            PembuatPengganti.Pasang("PotBunga", pot.transform, Vector3.zero);
            var p = pot.AddComponent<PakaiAlat>();
            SetTeks(p, "prompt", "Pot bunga di teras");
            SetTeks(p, "promptPunya", "Periksa pot bunga");
            Set(p, "dialog", PembuatDataFase5.Muat(PembuatDataFase5.PotTeras));   // efek barang:KunciLaci
            Set(p, "panel", rig.Dialog);
            Set(p, "hud", rig.Hud);
            Set(alur, "pot", p);

            var keluar = Logika("GerbangKeluar", new Vector3(0f, 0f, 0.4f), new Vector3(2f, 2f, 0.3f));   // di dalam BatasSelatan
            var k = keluar.AddComponent<InteraktifAlur>();
            SetTeks(k, "prompt", "Keluar ke jalan desa");
            Set(alur, "pagarKeluar", k);
        }

        // Dinding lurus sejajar sumbu, dari (x1, z1) ke (x2, z2).
        static void Dinding(float x1, float z1, float x2, float z2)
        {
            var ukuran = new Vector3(Mathf.Abs(x2 - x1) + Tebal, Tinggi, Mathf.Abs(z2 - z1) + Tebal);
            Balok("Dinding", new Vector3((x1 + x2) / 2f, Tinggi / 2f, (z1 + z2) / 2f), ukuran, "Tembok");
        }

        // Ruang kerja 4 x 4 m (x 2,5–6,5, z 3–7), pintu terbuka di dinding barat, meja berlaci menempel dinding timur.
        static void RuangKerja(Rig rig, AlurRumahPakde alur)
        {
            Balok("LantaiRuangKerja", new Vector3(4.5f, 0.02f, 5f), new Vector3(4f, 0.04f, 4f), "Tembok");
            Balok("AtapRuangKerja", new Vector3(4.5f, Tinggi + 0.1f, 5f), new Vector3(4.3f, 0.2f, 4.3f), "Genteng");
            Dinding(2.5f, 3f, 6.5f, 3f);
            Dinding(2.5f, 7f, 6.5f, 7f);
            Dinding(6.5f, 3f, 6.5f, 7f);
            Dinding(2.5f, 3f, 2.5f, 4.4f);
            Dinding(2.5f, 5.6f, 2.5f, 7f);
            Balok("AmbangPintu", new Vector3(2.5f, 2.45f, 5f), new Vector3(Tebal, 0.7f, 1.2f), "Tembok");
            Lampu("LampuRuangKerja", new Vector3(4.5f, 2.4f, 5f), WarnaLampu, 5f, 0.6f);

            var meja = Logika("LaciPakde", new Vector3(5.9f, 0f, 5f), new Vector3(0.6f, 0.8f, 1.2f));
            PembuatPengganti.Pasang("MejaKerjaLaci", meja.transform, Vector3.zero, -90f);   // laci (+Z prefab) menghadap pintu
            var laci = meja.AddComponent<PakaiAlat>();
            SetTeks(laci, "prompt", "Laci meja (terkunci)");
            SetTeks(laci, "barang", Inventaris.KunciLaci);
            SetTeks(laci, "promptPunya", "Buka laci dengan kunci kecil");
            SetTeks(laci, "pesanKurang", "Lacinya terkunci. Kuncinya pasti disembunyikan di sekitar rumah.");
            Set(laci, "dialog", PembuatDataFase5.Muat(PembuatDataFase5.LaciPakde));   // efek bukti:B6
            Set(laci, "panel", rig.Dialog);
            Set(laci, "hud", rig.Hud);
            Set(laci, "suara", BuatSuara(rig, "LaciDibuka", Sfx, "SFX", "(Suara: laci kayu ditarik.)"));
            Set(alur, "laci", laci);
        }

        // Mangkuk di sudut barat daya. AlurRumahPakde.SaatMangkuk tidak memeriksa barang, jadi PakaiAlat yang memeriksa.
        static void Mangkuk(Rig rig, AlurRumahPakde alur)
        {
            var g = Logika("Mangkuk", new Vector3(-6.2f, 0f, 0.8f), new Vector3(0.6f, 0.4f, 0.6f));
            PembuatPengganti.Pasang("MangkukAnjing", g.transform, Vector3.zero);
            var m = g.AddComponent<PakaiAlat>();
            SetTeks(m, "prompt", "Mangkuk anjing");
            SetTeks(m, "barang", Inventaris.SisaMakanan);
            SetTeks(m, "promptPunya", "Lempar sisa makanan ke mangkuk");
            SetTeks(m, "pesanKurang", "Mangkuknya kosong. Sisa makanan dari dapur bisa menyibukkan anjing itu.");
            Set(m, "hud", rig.Hud);
            Set(alur, "mangkuk", m);
            Set(alur, "titikMangkuk", Titik("TitikMangkuk", new Vector3(-5.6f, 0f, 1.3f), new Vector3(-6.2f, 0f, 0.8f)));
        }

        // Anjing penjaga (K33-A): SosokSantet + NavMeshAgent, mati sampai AlurRumahPakde menyalakannya; patroli 4 sudut.
        static void Anjing(Rig rig, AlurRumahPakde alur)
        {
            var lantai = new GameObject("NavMesh").AddComponent<LantaiNavMesh>();
            lantai.transform.position = new Vector3(0f, 0f, Panjang / 2f);

            var anjing = new GameObject("Anjing");
            anjing.transform.SetPositionAndRotation(new Vector3(-5.5f, 0f, 7.5f), Quaternion.Euler(0f, 90f, 0f));
            PembuatPengganti.Pasang("Anjing", anjing.transform, Vector3.zero);
            var agen = anjing.AddComponent<NavMeshAgent>();
            agen.radius = LantaiNavMesh.RadiusAgen;
            agen.height = 0.8f;
            agen.speed = SosokSantet.KecepatanPatroli;
            agen.angularSpeed = 300f;
            agen.acceleration = 8f;
            agen.stoppingDistance = 0.2f;
            agen.enabled = false;   // dinyalakan SosokSantet setelah NavMesh dibangun
            var skrip = anjing.AddComponent<SosokSantet>();
            Set(skrip, "lantai", lantai);
            Set(skrip, "pemain", rig.Pemain);
            Set(skrip, "senter", rig.Senter);
            SetDaftar(skrip, "titikPatroli", new Object[]
            {
                Titik("Patroli 1", new Vector3(-5.5f, 0f, 7.5f), new Vector3(-5.5f, 0f, 1.8f)),
                Titik("Patroli 2", new Vector3(-5.5f, 0f, 1.8f), new Vector3(5.5f, 0f, 1.8f)),
                Titik("Patroli 3", new Vector3(5.5f, 0f, 1.8f), new Vector3(1.5f, 0f, 7.5f)),
                Titik("Patroli 4", new Vector3(1.5f, 0f, 7.5f), new Vector3(-5.5f, 0f, 7.5f)),
            });
            var geram = BuatSuara(rig, "AnjingMenggeram", Sfx, "SFX", "(Suara: anjing menggeram pelan.)", ulang: true, ruang: true);
            geram.transform.SetParent(anjing.transform, false);
            Set(skrip, "gemerincing", geram);
            Set(alur, "anjing", skrip);
            anjing.SetActive(false);
        }
    }
}
