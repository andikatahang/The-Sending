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
    // Membuat Scene_RumahBuSumi (K17-B): interior dapur jamu dan halaman belakang kecil dengan sumur. Dimuat dari pintu
    // rumah Bu Sumi di Scene_Desa pada Malam 3. Jangan sunting scene dengan tangan; ubah angka di sini lalu bangun ulang.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatRumahBuSumi.Buat
    public static class PembuatRumahBuSumi
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.RumahBuSumi + ".unity";
        const float Tinggi = 2.8f, Tebal = 0.15f;
        const string Sfx = PembuatPengganti.FolderSfx;
        const string WarnaBulan = "6D86A8", WarnaTeplok = "FFB46B";

        [MenuItem("The Sending/Buat Scene Rumah Bu Sumi")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();
            var rig = PembuatRig.Buat(new Vector3(0f, 0.05f, 0.8f));
            SetAngka(rig.Langkah, "bawaan", (int)JenisPermukaan.Kayu);

            var alur = new GameObject("Sutradara").AddComponent<AlurRumahBuSumi>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "dialogPintu", PembuatDataFase3.Muat(PembuatDataFase3.SumiPintu));
            Set(alur, "dialogMenunggu", PembuatDataFase3.Muat(PembuatDataFase3.SumiMenunggu));
            Set(alur, "dialogTerimaKasih", PembuatDataFase3.Muat(PembuatDataFase3.SumiTerimaKasih));
            Set(alur, "dialogPamit", PembuatDataFase3.Muat(PembuatDataFase3.SumiPamit));
            Set(alur, "dialogCerita", PembuatDataFase3.Muat(PembuatDataFase3.SumiCerita));
            Set(alur, "dialogMalam5Dingin", PembuatDataFase5.Muat(PembuatDataFase5.SumiDingin));
            Set(alur, "dialogMalam5Kesaksian", PembuatDataFase5.Muat(PembuatDataFase5.SumiKesaksian));
            Set(alur, "dialogMalam5MinyakSereh", PembuatDataFase5.Muat(PembuatDataFase5.SumiMinyakSereh));
            Set(alur, "dialogMalam5Pamit", PembuatDataFase5.Muat(PembuatDataFase5.SumiPamit));
            Set(alur, "titikMulai", Titik("TitikMulai", new Vector3(0f, 0.05f, 0.8f), new Vector3(0f, 0.05f, 5f)));

            Ruang(rig, alur);
            HalamanBelakang(rig, alur);

            var pemanggil = new GameObject("PemanggilSidang").AddComponent<PemanggilSidang>();   // Malam 6, 21.00: ke sidang di desa
            Set(pemanggil, "kartu", rig.Kartu);
            Set(pemanggil, "adegan", rig.Adegan);
            SetTeks(pemanggil, "sceneIni", PetaScene.RumahBuSumi);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene rumah Bu Sumi dibuat: {JalurScene}");
        }

        // Dinding lurus sejajar sumbu, dari (x1, z1) ke (x2, z2).
        static void Dinding(float x1, float z1, float x2, float z2)
        {
            var ukuran = new Vector3(Mathf.Abs(x2 - x1) + Tebal, Tinggi, Mathf.Abs(z2 - z1) + Tebal);
            Balok("Dinding", new Vector3((x1 + x2) / 2f, Tinggi / 2f, (z1 + z2) / 2f), ukuran, "Gedek");
        }

        // Dapur jamu 8 x 5 m: pintu depan (keluar ke desa), pintu belakang ke halaman, teplok mati, bale-bale, rak, lumpang.
        static void Ruang(Rig rig, AlurRumahBuSumi alur)
        {
            Balok("Lantai", new Vector3(0f, -0.1f, 2.5f), new Vector3(8f, 0.2f, 5f), "KayuTua");
            Balok("Plafon", new Vector3(0f, Tinggi + 0.1f, 2.5f), new Vector3(8f, 0.2f, 5f), "KayuGelap");
            Dinding(-4f, 0f, -0.6f, 0f);
            Dinding(0.6f, 0f, 4f, 0f);
            Balok("AmbangPintuDepan", new Vector3(0f, 2.45f, 0f), new Vector3(1.2f, 0.7f, Tebal), "Gedek");
            Dinding(-4f, 5f, 2f, 5f);
            Dinding(3f, 5f, 4f, 5f);
            Balok("AmbangPintuBelakang", new Vector3(2.5f, 2.45f, 5f), new Vector3(1f, 0.7f, Tebal), "Gedek");
            Dinding(-4f, 0f, -4f, 5f);
            Dinding(4f, 0f, 4f, 5f);
            Balok("PintuDepanTertutup", new Vector3(0f, 1.05f, -0.08f), new Vector3(1.2f, 2.1f, 0.06f), "KayuGelap");   // daun pintu di luar ambang

            var keluar = Logika("PintuKeluar", new Vector3(0f, 0f, 0.1f), new Vector3(1.2f, 2.1f, 0.1f));
            var k = keluar.AddComponent<InteraktifAlur>();
            SetTeks(k, "prompt", "Keluar ke jalan");
            Set(alur, "pintuKeluar", k);
            Set(alur, "suaraPintu", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)"));
            Set(alur, "suaraBanting", BuatSuara(rig, "PintuDibanting", Sfx, "SFX", "(Suara: pintu dibanting.)"));

            // Teplok mati di meja: dapur hanya diterangi cahaya redup dari pintu belakang sampai teplok diperbaiki.
            PembuatPengganti.Pasang("MejaKayu", null, new Vector3(-2.5f, 0f, 1.2f));
            var teplok = Logika("TeplokMeja", new Vector3(-2.5f, 0.76f, 1.2f), new Vector3(0.3f, 0.35f, 0.3f));
            PembuatPengganti.Pasang("Teplok", teplok.transform, Vector3.zero);
            var perbaiki = teplok.AddComponent<InteraktifAlur>();
            SetTeks(perbaiki, "prompt", "Perbaiki lampu teplok");
            Set(alur, "teplok", perbaiki);
            var cahaya = Lampu("CahayaTeplok", new Vector3(-2.5f, 1.1f, 1.2f), WarnaTeplok, 7f, 2f);
            Set(alur, "cahayaTeplok", cahaya.gameObject);
            cahaya.gameObject.SetActive(false);
            Lampu("CahayaRedup", new Vector3(1f, 2.4f, 2.5f), WarnaBulan, 6f, 0.5f);

            PembuatPengganti.Pasang("BaleBale", null, new Vector3(-2.4f, 0f, 3.8f));
            PembuatPengganti.Pasang("TekoTeh", null, new Vector3(-2.4f, 0.45f, 3.8f));
            PembuatPengganti.Pasang("Lilin", null, new Vector3(-2.15f, 0.76f, 1.2f));   // di samping collider teplok (x -2,65..-2,35)
            PembuatPengganti.Pasang("Lemari", null, new Vector3(0.6f, 0f, 4.7f), 180f);   // dinding belakang, jauh dari pintu belakang (x 2..3)
            PembuatPengganti.Pasang("RakBotol", null, new Vector3(2.6f, 0f, 4.7f), 180f);
            PembuatPengganti.Pasang("Lumpang", null, new Vector3(1.2f, 0f, 2.6f));

            var sumi = Logika("BuSumi", new Vector3(0f, 0f, 2.2f), new Vector3(0.5f, 1.65f, 0.5f));
            PembuatPengganti.Pasang("BuSumi", sumi.transform, Vector3.zero, 180f);
            var bicara = sumi.AddComponent<InteraktifAlur>();
            SetTeks(bicara, "prompt", "Bicara dengan Bu Sumi");
            Set(alur, "buSumi", bicara);

            Set(alur, "titikBale", Titik("TitikBale", new Vector3(-1.4f, 0.05f, 2.8f), new Vector3(-2.4f, 0.05f, 3.8f)));
            Set(alur, "titikSumiBale", Titik("TitikSumiBale", new Vector3(-2.4f, 0f, 4.1f), new Vector3(-1.4f, 0f, 2.8f)));
            Set(alur, "bidikBale", Titik("BidikBale", new Vector3(-0.6f, 1.5f, 1.6f), new Vector3(-2.4f, 1.2f, 4.1f)));
        }

        // Halaman belakang 8 x 6 m berpagar bambu dengan sumur timba, diterangi bulan.
        static void HalamanBelakang(Rig rig, AlurRumahBuSumi alur)
        {
            Balok("Halaman", new Vector3(0f, -0.1f, 8f), new Vector3(8f, 0.2f, 6f), "Tanah")
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Tanah;
            Batas("Batas", new Vector3(-4.1f, 1.5f, 8f), new Vector3(0.2f, 3f, 6f));
            Batas("Batas", new Vector3(4.1f, 1.5f, 8f), new Vector3(0.2f, 3f, 6f));
            Batas("Batas", new Vector3(0f, 1.5f, 11.1f), new Vector3(8f, 3f, 0.2f));
            foreach (float x in new[] { -3.4f, 0f, 3.4f })
                PembuatPengganti.Pasang("RumpunBambu", null, new Vector3(x, 0f, 10.7f));
            var bulan = new GameObject("Cahaya bulan").AddComponent<Light>();
            bulan.type = LightType.Spot;
            bulan.color = Hex(WarnaBulan);
            bulan.range = 9f;
            bulan.spotAngle = 80f;
            bulan.intensity = 1.3f;
            bulan.transform.SetPositionAndRotation(new Vector3(1f, 6f, 8f), Quaternion.Euler(90f, 0f, 0f));

            var sumur = Logika("Sumur", new Vector3(1.5f, 0f, 8.5f), new Vector3(1.2f, 1f, 1.2f));
            PembuatPengganti.Pasang("Sumur", sumur.transform, Vector3.zero, -90f);
            var timba = sumur.AddComponent<InteraktifAlur>();
            SetTeks(timba, "prompt", "Timba air untuk Bu Sumi");
            Set(alur, "sumur", timba);
            Set(alur, "suaraTimba", BuatSuara(rig, "AirDitimba", Sfx, "SFX", "(Suara: air ditimba dari sumur.)"));
        }
    }
}
