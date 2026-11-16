using System;
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheSending.EditorTools
{
    // Membuat scene uji Fase 0 (0B + 0C).
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatSandbox.Buat
    public static class PembuatSandbox
    {
        internal const string JalurScene = "Assets/_Project/Scenes/Scene_Sandbox0B.unity";
        const string JalurDialog = "Assets/_Project/Data/Sandbox/DialogContoh.asset";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        [MenuItem("The Sending/Buat Scene Sandbox 0B")]
        public static void Buat()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            Directory.CreateDirectory(Path.GetDirectoryName(JalurDialog));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var dialog = BuatDialogContoh();
            var jurnal = AmbilAtauBuatJurnal();
            PembuatGayaPsx.TerapkanKeScene();

            var bulan = new GameObject("CahayaBulan").AddComponent<Light>();
            bulan.type = LightType.Directional;
            bulan.intensity = 0.15f;
            bulan.color = new Color(0.43f, 0.53f, 0.66f);
            bulan.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var lantai = GameObject.CreatePrimitive(PrimitiveType.Plane);
            lantai.name = "Lantai";
            lantai.transform.localScale = new Vector3(3f, 1f, 3f);
            Kotak("Dinding", new Vector3(0f, 1.5f, 8f), new Vector3(16f, 3f, 0.2f));

            new GameObject("GameManager").AddComponent<GameManager>();

            var pemain = new GameObject("Pemain");
            pemain.transform.position = new Vector3(0f, 0.05f, -4f);
            var cc = pemain.AddComponent<CharacterController>();
            cc.height = 1.7f;
            cc.radius = 0.3f;
            cc.center = new Vector3(0f, 0.85f, 0f);
            var kamera = new GameObject("Kamera");
            kamera.tag = "MainCamera";
            kamera.transform.SetParent(pemain.transform, false);
            kamera.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            var cam = kamera.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            PembuatGayaPsx.SiapkanKamera(cam);
            kamera.AddComponent<AudioListener>();
            var senterGo = new GameObject("Senter");
            senterGo.transform.SetParent(kamera.transform, false);
            var senter = senterGo.AddComponent<Light>();
            senter.type = LightType.Spot;
            senter.range = 15f;
            senter.spotAngle = 50f;
            senter.intensity = 3f;
            senter.shadows = LightShadows.Soft;
            senter.enabled = false;

            Set(pemain.AddComponent<PemainGerak>(), "kamera", kamera.transform);
            var interaksi = pemain.AddComponent<PemainInteraksi>();
            Set(interaksi, "kamera", kamera.transform);
            Set(pemain.AddComponent<SenterHP>(), "cahaya", senter);
            Set(pemain.AddComponent<KompasHP>(), "pemain", pemain.transform);

            var ui = new GameObject("UI");
            var hud = ui.AddComponent<Hud>();
            Set(hud, "interaksi", interaksi);
            var kameraHp = pemain.AddComponent<KameraHP>();
            Set(kameraHp, "kamera", kamera.transform);
            Set(kameraHp, "hud", hud);
            var panelDialog = ui.AddComponent<PanelDialog>();
            Set(ui.AddComponent<PanelJurnal>(), "data", jurnal);
            var kartu = ui.AddComponent<KartuTeks>();
            SetTeks(kartu, "judulSaatMulai", "Sandbox Fase 0");
            SetTeks(kartu, "isiSaatMulai", "WASD jalan, Shift lari, E interaksi, F senter, Q kamera, C kompas, tahan Spasi menahan napas, Tab Jurnal, Esc jeda. Uji: F5 simpan checkpoint, F9 tertangkap.");
            var panelPengaturan = ui.AddComponent<PanelPengaturan>();
            Set(ui.AddComponent<MenuJeda>(), "pengaturan", panelPengaturan);
            Set(ui.AddComponent<AlatUji>(), "hud", hud);

            var meja = Kotak("Meja (dialog)", new Vector3(-2f, 0.4f, 0f), new Vector3(1.2f, 0.8f, 0.6f));
            var pemicu = meja.AddComponent<PemicuDialog>();
            Set(pemicu, "dialog", dialog);
            Set(pemicu, "panel", panelDialog);
            SetTeks(pemicu, "prompt", "Bicara dengan Bu Sumi");

            var foto = Kotak("Foto keluarga (Q)", new Vector3(2f, 1.4f, 7.85f), new Vector3(0.6f, 0.4f, 0.05f));
            var petunjuk = foto.AddComponent<PetunjukFoto>();
            SetTeks(petunjuk, "efek", "katanya:K1");
            SetTeks(petunjuk, "pesan", "Foto keluarga dipotret.");

            var stopKontak = Kotak("Stop kontak (isi baterai)", new Vector3(4f, 0.5f, 7.85f), new Vector3(0.15f, 0.15f, 0.05f));
            var pengisi = stopKontak.AddComponent<PengisiBaterai>();
            Set(pengisi, "hud", hud);
            SetTeks(pengisi, "prompt", "Isi baterai HP");

            var boneka = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boneka.name = "Media M1 (kompas)";
            boneka.transform.position = new Vector3(6f, 0.2f, 4f);
            boneka.transform.localScale = Vector3.one * 0.3f;
            boneka.AddComponent<PenandaMedia>();   // media = 0 = M1

            var khusus = Kotak("Hanya Malam 2-3", new Vector3(-5f, 0.5f, 4f), Vector3.one);
            var pengatur = new GameObject("AktifDiMalam (Malam 2-3)").AddComponent<AktifDiMalam>();
            SetAngka(pengatur, "dari", 2);
            SetAngka(pengatur, "sampai", 3);
            SetSatuObjek(pengatur, "sasaran", khusus);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene sandbox dibuat: {JalurScene}");
        }

        static DataDialog BuatDialogContoh()
        {
            var d = ScriptableObject.CreateInstance<DataDialog>();
            d.Baris = new[]
            {
                new BarisDialog
                {
                    Pembicara = "Bu Sumi", Teks = "Opo maneh? Wis bengi.", Terjemahan = "Apa lagi? Sudah malam.",
                    Pilihan = new[]
                    {
                        new PilihanDialog { Teks = "Bu, saya minta maaf soal yang dulu.", Efek = "kepercayaan:MintaMaaf", LompatKe = 1 },
                        new PilihanDialog { Teks = "Ibu yang bikin ibu saya sakit, kan?", Efek = "tuduh-sumi", LompatKe = 2 },
                    },
                },
                new BarisDialog
                {
                    Pembicara = "Bu Sumi", Teks = "Aku ora tau nyantet sapa-sapa. Aku mung tau nangis.",
                    Terjemahan = "Aku tidak pernah menyantet siapa pun. Aku cuma pernah menangis.",
                    Efek = "bukti:B5", Akhir = true,
                },
                new BarisDialog { Pembicara = "Bu Sumi", Teks = "Lungo!", Terjemahan = "Pergi!", Akhir = true },
            };
            AssetDatabase.DeleteAsset(JalurDialog);   // data contoh selalu dibuat ulang
            AssetDatabase.CreateAsset(d, JalurDialog);
            return d;
        }

        static DataJurnal AmbilAtauBuatJurnal()
        {
            var ada = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal);
            if (ada != null) return ada;   // jangan timpa isi yang sudah ditulis tim

            var j = ScriptableObject.CreateInstance<DataJurnal>();
            j.Media = Entri(
                "Boneka kain beraksara Jawa", "Tujuh paku berkarat", "Bungkusan rambut uban dan kembang busuk",
                "Telur ayam cemani berisi jarum", "Tanah kuburan merah dalam kendi", "Kain kafan bertulis rajah",
                "Kendi induk berisi foto keluarga dan sisir Bu Sumi");
            j.Bukti = Entri(
                "Halaman buku harian ayah tentang tekanan Pakde Darmo",
                "Telur ayam cemani; cemani hanya dipelihara di Watuireng",
                "Sampel tanah merah dan kesaksian Pak Lasno",
                "Kesaksian Laras soal Mbah Wiryo di sumur",
                "Kesaksian Bu Sumi soal sisir yang hilang",
                "Kuitansi uang muka dan catatan pembayaran Pakde untuk \"W\"",
                "Surat permintaan maaf ayah kepada Bu Sumi",
                "Buku catatan pesanan Mbah Wiryo");
            j.Katanya = Entri(
                "Bu Sumi itu dukun", "Bu Sumi pernah mengutuk keluarga Raka",
                "Rambut di sumur milik Bu Sumi", "Pembeli tanah sudah siap");
            j.HalamanDiary = Entri(
                "Tawaran pembeli tanah lewat Darmo, ayah menolak", "Tabel aksara Jawa",
                "Darmo marah dan berkata ayah akan menyesal", "Cara mencabut paku santet dan Ritual Penangkal",
                "Ayah menemukan paku di bantal sebelum sakit",
                "Rencana wasiat di notaris; selesaikan masalah tanah dengan musyawarah",
                "Surat permintaan maaf ayah kepada Bu Sumi",
                "Ayah melihat Wiryo di pekarangan pada malam sebelum ia sakit");
            j.CatatanBapak = Entri(
                "Weton dan hari pasaran", "Tabel aksara Jawa", "Rumah limasan dan saka guru", "Sumur timba dan kerekan",
                "Ayam cemani", "Daun kelor dan bambu kuning dalam kepercayaan Jawa", "Tradisi larung");
            j.CatatanRaka = Entri(
                "Tanda dehidrasi dan pentingnya cairan", "Kapan harus dirujuk ke rumah sakit", "Pasal 252 KUHP baru",
                "Main hakim sendiri adalah tindak pidana", "Cara yang benar menyelesaikan sengketa waris",
                "Cara membedakan fakta dan gosip");
            AssetDatabase.CreateAsset(j, JalurJurnal);
            return j;
        }

        static EntriJurnal[] Entri(params string[] judul) =>
            Array.ConvertAll(judul, t => new EntriJurnal { Judul = t, Isi = "" });

        static GameObject Kotak(string nama, Vector3 posisi, Vector3 ukuran)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = nama;
            g.transform.position = posisi;
            g.transform.localScale = ukuran;
            return g;
        }

        static SerializedProperty Properti(SerializedObject so, string nama) =>
            so.FindProperty(nama) ?? throw new MissingFieldException(so.targetObject.GetType().Name, nama);

        internal static void Set(Object komponen, string nama, Object nilai)
        {
            if (nilai == null) throw new ArgumentNullException(nama);
            var so = new SerializedObject(komponen);
            Properti(so, nama).objectReferenceValue = nilai;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetTeks(Object komponen, string nama, string nilai)
        {
            var so = new SerializedObject(komponen);
            Properti(so, nama).stringValue = nilai;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetAngka(Object komponen, string nama, int nilai)
        {
            var so = new SerializedObject(komponen);
            Properti(so, nama).intValue = nilai;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetSatuObjek(Object komponen, string nama, Object nilai)
        {
            if (nilai == null) throw new ArgumentNullException(nama);
            var so = new SerializedObject(komponen);
            var p = Properti(so, nama);
            p.arraySize = 1;
            p.GetArrayElementAtIndex(0).objectReferenceValue = nilai;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
