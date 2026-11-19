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
    // Membuat Scene_Makam (Malam 5): tanah lapang 20 x 24 m berpagar tak terlihat, gerbang di selatan (z 0), kamboja besar
    // di tengah dengan makam Pak Hadi, tiga titik sampel tanah, dan Pak Lasno. Dimuat dari gerbang makam di Scene_Desa.
    // Jangan sunting scene dengan tangan; ubah angka di sini lalu bangun ulang.
    public static class PembuatMakam
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.Makam + ".unity";
        const float Lebar = 20f, Panjang = 24f, KabutMakam = 0.09f;
        const string Sfx = PembuatPengganti.FolderSfx;
        const string WarnaBulan = "6D86A8";
        static readonly Vector3 Kamboja = new Vector3(0f, 0f, 13f);
        static readonly Vector3[] TitikSampel = { new Vector3(-6f, 0f, 6f), new Vector3(6.5f, 0f, 17f), new Vector3(-6.5f, 0f, 20f) };

        [MenuItem("The Sending/Buat Scene Makam")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();
            RenderSettings.fogDensity = KabutMakam;
            var rig = PembuatRig.Buat(new Vector3(0f, 0.05f, 1.5f));
            SetAngka(rig.Langkah, "bawaan", (int)JenisPermukaan.Tanah);

            var alur = new GameObject("Sutradara").AddComponent<AlurMakam>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "panel", rig.Hud.gameObject.AddComponent<PanelTanah>());
            Set(alur, "dialogNisan", PembuatDataFase5.Muat(PembuatDataFase5.NisanHadi));
            Set(alur, "dialogLasno", PembuatDataFase5.Muat(PembuatDataFase5.PakLasno));
            Set(alur, "titikMulai", Titik("TitikMulai", new Vector3(0f, 0.05f, 1.5f), new Vector3(0f, 0.05f, 10f)));
            Set(alur, "suasana", BuatSuara(rig, "SuasanaMakam", Sfx, "Ambience", "", ulang: true));   // tanpa file: diam
            Set(alur, "suaraGali", BuatSuara(rig, "GaliTanah", Sfx, "SFX", "(Suara: pacul menggali tanah.)"));
            Set(alur, "suaraPintu", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: gerbang besi berderit.)"));

            Lahan();
            Makam(alur);
            Sampel(rig, alur);
            Lasno(alur);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene makam dibuat: {JalurScene}");
        }

        // Tanah, pagar tak terlihat (Ignore Raycast), gerbang keluar di selatan, dan cahaya bulan dingin.
        static void Lahan()
        {
            Balok("Tanah", new Vector3(0f, -0.1f, Panjang / 2f), new Vector3(Lebar, 0.2f, Panjang), "Tanah")
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Tanah;
            Batas("BatasBarat", new Vector3(-Lebar / 2f, 1.5f, Panjang / 2f), new Vector3(0.2f, 3f, Panjang));
            Batas("BatasTimur", new Vector3(Lebar / 2f, 1.5f, Panjang / 2f), new Vector3(0.2f, 3f, Panjang));
            Batas("BatasSelatan", new Vector3(0f, 1.5f, -0.1f), new Vector3(Lebar, 3f, 0.2f));
            Batas("BatasUtara", new Vector3(0f, 1.5f, Panjang), new Vector3(Lebar, 3f, 0.2f));

            var bulan = new GameObject("CahayaBulan").AddComponent<Light>();
            bulan.type = LightType.Directional;
            bulan.intensity = 0.25f;
            bulan.color = Hex(WarnaBulan);
            bulan.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Lampu("CahayaKamboja", Kamboja + new Vector3(0f, 4f, -1f), WarnaBulan, 10f, 0.8f);
        }

        // Kamboja besar, makam Pak Hadi di sisinya, dan sekitar 12 makam lain (seed tetap, menjauhi jalur dan titik sampel).
        static void Makam(AlurMakam alur)
        {
            PembuatPengganti.Pasang("GerbangMakam", null, new Vector3(0f, 0f, 0.1f));
            var keluar = Logika("GerbangKeluar", new Vector3(0f, 0f, 0.4f), new Vector3(2f, 2.2f, 0.3f));   // di dalam BatasSelatan
            var aKeluar = keluar.AddComponent<InteraktifAlur>();
            SetTeks(aKeluar, "prompt", "Keluar ke jalan desa");
            Set(alur, "gerbangKeluar", aKeluar);

            PembuatPengganti.Pasang("Kamboja", null, Kamboja).transform.localScale = Vector3.one * 1.6f;
            PembuatPengganti.Pasang("GundukanMakam", null, new Vector3(2f, 0f, 11.5f));
            var nisan = Logika("NisanHadi", new Vector3(2f, 0f, 12.6f), new Vector3(0.5f, 1f, 0.4f));
            PembuatPengganti.Pasang("NisanHadi", nisan.transform, Vector3.zero, 180f);
            var baca = nisan.AddComponent<InteraktifAlur>();
            SetTeks(baca, "prompt", "Baca nisan");
            Set(alur, "nisanHadi", baca);

            var induk = new GameObject("Makam").transform;
            var acak = new System.Random(1955);
            float Acak(float a, float b) => a + (float)acak.NextDouble() * (b - a);
            for (int n = 0, coba = 0; n < 12 && coba < 500; coba++)
            {
                var p = new Vector3(Acak(-8.5f, 8.5f), 0f, Acak(4f, 22f));
                if (Terlarang(p)) continue;
                PembuatPengganti.Pasang("GundukanMakam", induk, p);
                PembuatPengganti.Pasang(acak.NextDouble() < 0.5 ? "NisanBatu" : "NisanKayu", induk, p + new Vector3(0f, 0f, 1.1f), 180f);
                n++;
            }
        }

        // Jalur tengah dari gerbang, sekitar kamboja dan makam Pak Hadi, titik sampel. Cek kasar per baris makam (2,5 m).
        static bool Terlarang(Vector3 p)
        {
            if (Mathf.Abs(p.x) < 2f && p.z < 10f) return true;
            if (Mathf.Abs(p.x - Kamboja.x) < 4f && Mathf.Abs(p.z - Kamboja.z) < 3.5f) return true;
            foreach (var t in TitikSampel)
                if (Mathf.Abs(p.x - t.x) < 2f && Mathf.Abs(p.z - t.z) < 3f) return true;
            // shortcut: tidak memeriksa tumpang tindih antar makam acak; ubah seed bila ada yang bertabrakan.
            return false;
        }

        // Tiga titik tanah gembur. Sekali saja (hitungan "masih N titik" di AlurMakam memakai PakaiAlat.Sudah).
        static void Sampel(Rig rig, AlurMakam alur)
        {
            var daftar = new Object[TitikSampel.Length];
            for (int i = 0; i < TitikSampel.Length; i++)
            {
                var g = Logika("TitikSampel" + (i + 1), TitikSampel[i], new Vector3(0.9f, 0.4f, 0.9f));
                var tanah = Balok("TanahGembur", TitikSampel[i] + new Vector3(0f, 0.05f, 0f), new Vector3(0.8f, 0.1f, 0.8f), "Tanah");
                Object.DestroyImmediate(tanah.GetComponent<Collider>());
                tanah.transform.SetParent(g.transform, true);
                var p = g.AddComponent<PakaiAlat>();
                SetTeks(p, "prompt", "Tanah gembur");
                SetTeks(p, "barang", Inventaris.Pacul);
                SetTeks(p, "promptPunya", "Ambil sampel tanah");
                SetTeks(p, "pesanKurang", "Butuh pacul untuk mengambil sampel.");
                SetBool(p, "sekaliSaja", true);
                Set(p, "hud", rig.Hud);
                daftar[i] = p;
            }
            SetDaftar(alur, "titikSampel", daftar);
        }

        // Pak Lasno (mati sampai ketiga sampel dibandingkan) datang dari arah gerbang; kamera adegan membidiknya.
        static void Lasno(AlurMakam alur)
        {
            var lasno = PembuatPengganti.Pasang("PakLasno", null, new Vector3(-1.5f, 0f, 8f));
            lasno.SetActive(false);
            Set(alur, "pakLasno", lasno);
            Set(alur, "titikLasno", Titik("TitikLasno", new Vector3(-1.5f, 0f, 8f), new Vector3(-1.5f, 0f, 3f)));
            Set(alur, "bidikLasno", Titik("BidikLasno", new Vector3(-0.5f, 1.6f, 5f), new Vector3(-1.5f, 1.5f, 8f)));
        }
    }
}
