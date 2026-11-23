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
    // Membuat Scene_RumahKeluarga (denah K7) untuk akhir Prolog, Malam 1, Malam 2 (PembuatMalam2), dan Malam 3 (PembuatMalam3).
    // Jangan sunting scene dengan tangan; ubah angka di sini lalu bangun ulang.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatRumahKeluarga.Buat
    public static class PembuatRumahKeluarga
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.RumahKeluarga + ".unity";
        internal const float Tinggi = 3f, Tebal = 0.15f;
        const string Sfx = PembuatPengganti.FolderSfx, Musik = PembuatPengganti.FolderMusik;
        const string WarnaLampuRumah = "FFD9A0", WarnaTeplok = "FFB46B";
        const string JalurFont = "Assets/_Project/Art/Fonts/Resources/VT323-Regular.ttf";

        [MenuItem("The Sending/Buat Scene Rumah Keluarga")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();
            var rig = PembuatRig.Buat(new Vector3(0f, 0.05f, 1.2f));
            SetAngka(rig.Langkah, "bawaan", (int)JenisPermukaan.Kayu);

            var sutradara = new GameObject("Sutradara");
            var prolog = sutradara.AddComponent<AlurPrologRumah>();
            var malam1 = sutradara.AddComponent<AlurMalam1>();
            foreach (var alur in new Object[] { prolog, malam1 })
            {
                Set(alur, "adegan", rig.Adegan);
                Set(alur, "kartu", rig.Kartu);
                Set(alur, "dialog", rig.Dialog);
                Set(alur, "hud", rig.Hud);
                Set(alur, "pemain", rig.Pemain);
            }
            Set(prolog, "dialogLaras", PembuatDataFase1.Muat(PembuatDataFase1.LarasSambutan));
            Set(prolog, "dialogIbu", PembuatDataFase1.Muat(PembuatDataFase1.IbuProlog));
            Set(prolog, "malam1", malam1);
            Set(prolog, "titikMulai", Titik("TitikMulaiProlog", new Vector3(0f, 0.05f, 1.2f), new Vector3(0f, 0.05f, 6f)));
            Set(malam1, "dialogPenutup", PembuatDataFase1.Muat(PembuatDataFase1.PenutupMalam1));

            var (pintuGudang, pintuBelakang) = Bangunan();
            // Kunci gudang hanya ada di Malam 2; mulai Malam 3 gudang selalu terbuka (pintunya tidak ada).
            var gudangTerkunci = sutradara.AddComponent<AktifDiMalam>();
            SetAngka(gudangTerkunci, "dari", 0);
            SetAngka(gudangTerkunci, "sampai", 2);
            SetSatuObjek(gudangTerkunci, "sasaran", pintuGudang.gameObject);
            var kalender = Perabot(rig);
            Ukiran(rig);
            SetDaftar(malam1, "lampuListrik", LampuListrik());
            var ibu = KamarIbu(rig, prolog, malam1);
            BendaAdegan(malam1);
            var laras = Tokoh(rig, sutradara, prolog, malam1);
            SuaraDanJadwal(rig, sutradara, malam1);
            var sosok = PembuatMalam2.Pasang(rig, sutradara, pintuGudang, kalender, laras);
            PembuatMalam3.Pasang(rig, sutradara, sosok, pintuGudang, pintuBelakang, ibu);
            PembuatMalam4.Pasang(rig, sutradara, sosok);
            var kainKafan = PembuatMalam5.Pasang(rig, sutradara);
            PembuatMalam6.Pasang(rig, sutradara, ibu, kainKafan);
            PembuatMalam7.Pasang(rig, sutradara, sosok, ibu);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene rumah keluarga dibuat: {JalurScene}");
        }

        // Dinding lurus sejajar sumbu, dari (x1, z1) ke (x2, z2).
        static void Dinding(float x1, float z1, float x2, float z2, string bahan = "DindingRumah")
        {
            var ukuran = new Vector3(Mathf.Abs(x2 - x1) + Tebal, Tinggi, Mathf.Abs(z2 - z1) + Tebal);
            Balok("Dinding", new Vector3((x1 + x2) / 2f, Tinggi / 2f, (z1 + z2) / 2f), ukuran, bahan);
        }

        static (InteraktifAlur pintuGudang, GameObject pintuBelakang) Bangunan()
        {
            // Lantai dipecah: ruang tengah dan kamar keramik putih, dapur (x -7..3, z 8..12) berlinoleum tua, gudang kayu.
            Balok("Lantai", new Vector3(0f, -0.1f, 4f), new Vector3(14f, 0.2f, 8f), "Keramik");
            Balok("Lantai", new Vector3(5f, -0.1f, 10f), new Vector3(4f, 0.2f, 4f), "KayuTua");
            Balok("LantaiDapur", new Vector3(-2f, -0.1f, 10f), new Vector3(10f, 0.2f, 4f), "Linoleum");
            // Plafon: dapur dan gudang (z 8..12) berlubang 1 x 1 m di (1,5; 10) sebagai jalan naik Malam 4 (NaikPlafon).
            float yPlafon = Tinggi + 0.1f;
            Balok("Plafon", new Vector3(0f, yPlafon, 4f), new Vector3(14f, 0.2f, 8f), "KayuGelap");
            Balok("Plafon", new Vector3(-3f, yPlafon, 10f), new Vector3(8f, 0.2f, 4f), "KayuGelap");
            Balok("Plafon", new Vector3(4.5f, yPlafon, 10f), new Vector3(5f, 0.2f, 4f), "KayuGelap");
            Balok("Plafon", new Vector3(1.5f, yPlafon, 8.75f), new Vector3(1f, 0.2f, 1.5f), "KayuGelap");
            Balok("Plafon", new Vector3(1.5f, yPlafon, 11.25f), new Vector3(1f, 0.2f, 1.5f), "KayuGelap");

            // Dinding luar. Pintu depan (selatan) tertutup: di Fase 1 pemain tidak keluar rumah lagi.
            Dinding(-7f, 0f, -0.6f, 0f);
            Dinding(0.6f, 0f, 7f, 0f);
            // Dinding utara dengan pintu belakang dapur (x -4,6..-3,4) ke pekarangan: tertutup di Malam 1-2, terbuka mulai Malam 3 (K18).
            Dinding(-7f, 12f, -4.6f, 12f, "Gedek");   // dinding dapur/gudang tetap gedek
            Dinding(-3.4f, 12f, 7f, 12f, "Gedek");
            Balok("AmbangPintuBelakang", new Vector3(-4f, 2.55f, 12f), new Vector3(1.2f, 0.9f, Tebal), "Gedek");
            var pintuBelakang = Balok("PintuBelakang", new Vector3(-4f, 1.05f, 12f), new Vector3(1.2f, 2.1f, 0.1f), "KayuGelap");
            Dinding(-7f, 0f, -7f, 12f);
            // Dinding timur dengan celah 0,3 x 0,5 m di gudang (z 10, tinggi 1,3-1,8 m): Sosok Santet terlihat lewat sini di Malam 2.
            Dinding(7f, 0f, 7f, 9.775f);
            Dinding(7f, 10.225f, 7f, 12f);
            Balok("Dinding", new Vector3(7f, 0.65f, 10f), new Vector3(Tebal, 1.3f, 0.3f), "Gedek");
            Balok("Dinding", new Vector3(7f, 2.4f, 10f), new Vector3(Tebal, 1.2f, 0.3f), "Gedek");
            Balok("PintuDepan", new Vector3(0f, 1.05f, 0f), new Vector3(1.2f, 2.1f, 0.1f), "KayuGelap");
            Ambang(new Vector3(0f, 0f, 0f), new Vector3(1.061f, 0f, Tebal));   // edit manual tim: ambang plester seperti pintu kamar

            // Sekat barat (x = -3): pintu kamar Raka z 1.5-2.5, pintu kamar Ibu z 5.5-6.5.
            Dinding(-3f, 0f, -3f, 1.367f);   // celah pintu dilebarkan tim (edit manual 23 Nov): sisi gagang
            Dinding(-3f, 2.5f, -3f, 5.367f);
            Dinding(-3f, 6.5f, -3f, 8f);
            // Sekat timur (x = 3): pintu kamar mandi z 1.5-2.5, kamar Laras z 5.5-6.5, gudang z 9.5-10.5.
            Dinding(3f, 0f, 3f, 1.367f);
            Dinding(3f, 2.5f, 3f, 5.367f);
            Dinding(3f, 6.5f, 3f, 9.429f);   // celah gudang dilebarkan tim (edit manual 23 Nov)
            Dinding(3f, 10.5786f, 3f, 12f);
            // Sekat antarkamar (z = 4) dan sekat dapur (z = 8) dengan pintu dapur x -0.6..0.6.
            Dinding(-7f, 4f, -3f, 4f);
            Dinding(3f, 4f, 7f, 4f);
            Dinding(-7f, 8f, -0.6f, 8f);
            Dinding(0.6f, 8f, 7f, 8f);

            // Gudang terkunci sampai Malam 2: AlurMalam2 membukanya dengan kunci dari Laras.
            var pintuGudang = Balok("PintuGudang", new Vector3(3f, 1.05f, 10f), new Vector3(0.1f, 2.1f, 1f), "KayuGelap")
                .AddComponent<InteraktifAlur>();
            SetTeks(pintuGudang, "prompt", "Pintu gudang (terkunci)");

            // Empat saka guru dan blandar ruang tengah.
            foreach (float x in new[] { -1.5f, 1.5f })
                foreach (float z in new[] { 2.5f, 5.5f })
                    PembuatPengganti.Pasang("SakaGuru", null, new Vector3(x, 0f, z));
            Balok("Blandar", new Vector3(0f, 2.75f, 5.497f), new Vector3(6f, 0.3f, 0.3f), "KayuGelap");
            return (pintuGudang, pintuBelakang);
        }

        // Benda atau tokoh yang membuka dialog: objek logika + PemicuDialog + prefab visual.
        // tengah = true untuk benda dinding (titik nol prefab di tengah benda).
        internal static PemicuDialog BendaDialog(Rig rig, string prefab, Vector3 posisi, Vector3 ukuran, float putarY, bool tengah,
            DataDialog dialog, string prompt)
        {
            var g = Logika(prefab, posisi, ukuran);
            PembuatPengganti.Pasang(prefab, g.transform, tengah ? new Vector3(0f, ukuran.y / 2f, 0f) : Vector3.zero, putarY);
            var pemicu = g.AddComponent<PemicuDialog>();
            Set(pemicu, "dialog", dialog);
            Set(pemicu, "panel", rig.Dialog);
            SetTeks(pemicu, "prompt", prompt);
            return pemicu;
        }

        static GameObject Perabot(Rig rig)
        {
            // Ruang tengah
            PembuatPengganti.Pasang("Meja", null, new Vector3(-2.621f, 0f, 3.41f), 90f);
            PembuatPengganti.Pasang("Kursi", null, new Vector3(-2.326f, 0f, 3.533f), 90f);
            // Sofa tua di dinding selatan, menghadap ke tengah ruang (Pakde datang di Malam 3).
            PembuatPengganti.Pasang("Sofa", null, new Vector3(-0.243f, 0f, 4.014f), 90.33f);   // edit manual tim
            PembuatPengganti.Pasang("JamDinding", null, new Vector3(2.906f, 1.9f, 3.971f), -90f);   // edit manual tim: di atas TV
            BendaDialog(rig, "FotoKeluarga", new Vector3(-1.8f, 1.275f, 7.87f), new Vector3(0.6f, 0.45f, 0.1f), 180f, true,
                PembuatDataFase1.Muat(PembuatDataFase1.FotoKeluarga), "Periksa foto keluarga");
            var stopKontak = Logika("StopKontak", new Vector3(2.88f, 0.35f, 3.5f), new Vector3(0.15f, 0.15f, 0.15f));
            PembuatPengganti.Pasang("StopKontak", stopKontak.transform, new Vector3(0f, 0.075f, 0f), -90f);   // satu stop kontak
            var pengisi = stopKontak.AddComponent<PengisiBaterai>();
            Set(pengisi, "hud", rig.Hud);
            SetTeks(pengisi, "prompt", "Isi baterai HP");

            // Kamar Raka, kamar Laras, kamar mandi, dapur
            PembuatPengganti.Pasang("RanjangKayu", null, new Vector3(-6.2f, 0f, 2f));
            PembuatPengganti.Pasang("Kabinet", null, new Vector3(-5.35f, 0f, 1.25f));
            PembuatPengganti.Pasang("Meja", null, new Vector3(-4.2f, 0f, 0.5f));
            PembuatPengganti.Pasang("RanjangKayu", null, new Vector3(6.409f, 0f, 5.127f));
            PembuatPengganti.Pasang("Kabinet", null, new Vector3(5.516f, 0f, 4.279f));
            PembuatPengganti.Pasang("Laci", null, new Vector3(4.737f, 0f, 4.33f));
            Dekor();
            return BendaDialog(rig, "Kalender", new Vector3(-1.5f, 1.25f, 8.13f), new Vector3(0.35f, 0.5f, 0.1f), 0f, true,
                PembuatDataFase1.Muat(PembuatDataFase1.Kalender), "Lihat kalender").gameObject;
        }

        // Dekor tanpa interaksi (props). Semua >= 0,9 m dari garis tengah pintu, di luar zona terlarang anglo/pintu belakang/Sosok.
        static void Dekor()
        {
            // Kamar mandi (x 3..7, z 0..4), pintu di x = 3, z 1,5-2,5. Ubin dinding 1 cm di depan muka dalam dinding luar.
            Balok("BakMandi", new Vector3(6.435f, 0.4f, 0.579f), new Vector3(1f, 0.8f, 1f), "UbinMandi");
            Balok("LantaiMandi", new Vector3(5f, 0.005f, 2f), new Vector3(3.9f, 0.01f, 3.9f), "UbinMandi");
            Balok("DindingMandiTimur", new Vector3(6.915f, 0.75f, 2f), new Vector3(0.02f, 1.5f, 3.85f), "DindingMandi");
            Balok("DindingMandiSelatan", new Vector3(5f, 0.75f, 0.085f), new Vector3(3.85f, 1.5f, 0.02f), "DindingMandi");
            PembuatPengganti.Pasang("Kloset", null, new Vector3(6.6f, 0f, 3.2f), -90f);
            PembuatPengganti.Pasang("Wastafel", null, new Vector3(4.2f, 0f, 3.62f), 180f);
            PembuatPengganti.Pasang("AksesorisMandi", null, new Vector3(3.452f, 0.009f, 3.88f), 180f);   // edit manual tim

            // Ruang tengah: TV di atas laci di dinding timur, karpet di depan sofa, sakelar dekat pintu depan.
            PembuatPengganti.Pasang("Karpet", null, new Vector3(0.269f, -0.0015f, 3.969f), -89.169f).transform.localScale = Vector3.one * 1.4508f;
            PembuatPengganti.Pasang("BaleBale", null, new Vector3(0.76f, 0.058f, 3.969f), 90f).transform.localScale = Vector3.one * 0.73483f;
            var laci = PembuatPengganti.Pasang("Laci", null, new Vector3(2.7f, 0.001f, 3.978f), -90f);
            laci.transform.localScale = new Vector3(1.9933892f, 0.67456f, 1f);   // edit manual tim
            PembuatPengganti.Pasang("TvLed", null, new Vector3(2.804f, 0.568f, 3.974f), -90f).transform.localScale = Vector3.one * 1.2003f;
            PembuatPengganti.Pasang("RemoteTv", null, new Vector3(0.65f, 0.386f, 4.089f), 83.19f);
            PembuatPengganti.Pasang("Sakelar", null, new Vector3(0.95f, 1.3f, 0.085f));
            PembuatPengganti.Pasang("KursiPlastik", null, new Vector3(2.4f, 0f, 7.3f), 200f);

            // Dapur: meja cor L di sudut barat daya, kulkas di sudut barat laut, dispenser dan meja makan di sisi timur pintu dapur.
            MejaCor();
            // Dekor dapur, teko, kulkas, dispenser, kompor, meja makan di atas karpet: edit manual tim (23 Nov) dipertahankan persis.
            var dekor = PembuatPengganti.Pasang("DekorDapur", null, new Vector3(-5.969f, AtasMejaCor, 8.493f)).transform;
            Anak(dekor, "Plate").localPosition = new Vector3(-0.61f, 0.19273636f, 0.43f);
            var piring2 = Object.Instantiate(Anak(dekor, "Plate").gameObject, Anak(dekor, "Plate").parent, false);
            piring2.name = "Plate (1)";
            piring2.transform.localPosition = new Vector3(-1.7f, 0.19273636f, 0.44f);
            Anak(dekor, "Bowl").localPosition = new Vector3(-2.07f, 0.199f, 1.18f);
            var talenan = Anak(dekor, "CuttingWood");
            talenan.localPosition = new Vector3(1.13f, 0.042384453f, 1.011f);
            talenan.localRotation = new Quaternion(0.22781433f, 0.66940325f, 0.6694032f, -0.22781429f);
            PembuatPengganti.Pasang("TabungGas", null, new Vector3(-4.994f, 0f, 8.38f));   // edit manual tim
            PembuatPengganti.Pasang("KomporGas", null, new Vector3(-4.209f, 0.81649f, 8.465f), 180.237f).transform.localScale = Vector3.one * 1.486031f;
            PembuatPengganti.Pasang("Kulkas", null, new Vector3(-2.546f, 0f, 8.458f));
            PembuatPengganti.Pasang("Dispenser", null, new Vector3(-6.687f, 0.002f, 10.37f), 90f).transform.localScale = Vector3.one * 1.9282811f;
            PembuatPengganti.Pasang("Karpet", null, new Vector3(-1.036f, -0.0014712f, 10.673f)).transform.localScale = Vector3.one * 1.4537f;
            PembuatPengganti.Pasang("MejaMakan", null, new Vector3(MejaMakan.x, 0f, MejaMakan.z));
            var teko = PembuatPengganti.Pasang("TekoTeh", null, new Vector3(-6.015f, AtasMejaCor, 8.254f));
            var tekoIsi = Anak(teko.transform, "defaultMaterial");
            tekoIsi.localPosition = new Vector3(488.799f, -3.5187967f, -44.256565f);
            tekoIsi.localRotation = new Quaternion(0f, -0.30879653f, 0f, 0.9511282f);
            tekoIsi.localScale = Vector3.one * 1.1709f;
            PembuatPengganti.Pasang("JamDinding", null, new Vector3(-3.5f, 1.9f, 8.14f));

            // Kamar: lemari kayu tua di kamar Raka, lilin di laci kamar Laras.
            PembuatPengganti.Pasang("Lemari", null, new Vector3(-4.2f, 0f, 3.7f), 180f);
            PembuatPengganti.Pasang("Lilin", null, new Vector3(4.6f, Atas(laci), 4.33f));

            // Pintu kamar berengsel (PintuKamar, E buka/tutup, mulai terbuka 90° ke dalam kamar). Gudang dilewati: PintuGudang
            // terkunci di Malam 0-2. Engsel di tepi celah; daun tertutup memanjang ke `tutup`, membuka ke arah `buka`.
            PintuKamar("PintuKamarRaka", "DaunPintu", new Vector3(-3f, 0f, 2.5f), Vector3.back, Vector3.left, 1f);
            PintuKamar("PintuKamarIbu", "DaunPintu", new Vector3(-3f, 0f, 6.5f), Vector3.back, Vector3.left, 1f);
            PintuKamar("PintuKamarMandi", "DaunPintu", new Vector3(3f, 0f, 2.5f), Vector3.back, Vector3.right, 1f);
            PintuLaras = PintuKamar("DaunPintuKamarLaras", "DaunPintu", new Vector3(3f, 0f, 6.5f), Vector3.back, Vector3.right, 1f);
            var ambangSekat = new Vector3(Tebal, 0f, 1.0276f);
            foreach (var c in new[] { new Vector3(-3f, 0f, 1.9448f), new Vector3(-3f, 0f, 5.9448f), new Vector3(3f, 0f, 1.9448f), new Vector3(3f, 0f, 5.9448f), new Vector3(3f, 0f, 10.008f) })
                Ambang(c, ambangSekat);
            Ambang(new Vector3(0f, 0f, 8f), new Vector3(1.2f + Tebal, 0f, Tebal));

            // Sakelar tunggal di sisi gagang setiap pintu kamar, di dalam ruangan: tinggi 1,3 m, 0,15 m dari kusen. Dekor saja.
            foreach (var (pos, putar) in new[]
            {
                (new Vector3(-3.08f, 1.3f, 1.25f), -90f), (new Vector3(-3.08f, 1.3f, 5.25f), -90f),   // Raka, Ibu
                (new Vector3(3.08f, 1.3f, 1.25f), 90f), (new Vector3(3.08f, 1.3f, 5.25f), 90f),       // kamar mandi, Laras
                (new Vector3(3.08f, 1.3f, 9.38f), 90f), (new Vector3(-0.72f, 1.3f, 8.08f), 0f),       // gudang, dapur
            })
                PembuatPengganti.Pasang("Sakelar", null, pos, putar);
        }

        // Daun pintu `prefab` dilebarkan pas `lebar` celah; collider padat (bukan trigger) supaya bisa dibidik E dan menghalangi
        // saat tertutup. Derit: Audio/SFX/PintuBerderit jika ada (belum ada = senyap).
        // Pintu kamar Laras: PembuatMalam5 menggantinya dengan pintu terkunci di Malam 5.
        internal static GameObject PintuLaras;

        static GameObject PintuKamar(string nama, string prefab, Vector3 engsel, Vector3 tutup, Vector3 buka, float lebar)
        {
            var g = new GameObject(nama);
            g.transform.SetPositionAndRotation(engsel, Quaternion.LookRotation(Vector3.Cross(tutup, Vector3.up), Vector3.up));   // tegak: FromToRotation(right, left) membalik pintu dapur
            var daun = PembuatPengganti.Pasang(prefab, g.transform, new Vector3(lebar / 2f, 0f, 0f), 180f);   // gagang jauh dari engsel
            BentukPintuRaka(daun.transform);   // semua pintu kamar berbentuk sama (pintu Laras/Raka)
            var b = Batas(daun);
            var kotak = g.AddComponent<BoxCollider>();
            var ukuranLokal = g.transform.InverseTransformVector(b.size);
            kotak.center = g.transform.InverseTransformPoint(b.center);
            kotak.size = new Vector3(Mathf.Abs(ukuranLokal.x), Mathf.Abs(ukuranLokal.y), 0.06f);
            var pintu = g.AddComponent<PintuKamar>();
            var so = new SerializedObject(pintu);
            so.FindProperty("sudutBuka").floatValue = g.transform.InverseTransformDirection(buka).z > 0f ? -90f : 90f;
            so.ApplyModifiedPropertiesWithoutUndo();
            var derit = g.AddComponent<AudioSource>();
            derit.playOnAwake = false;
            derit.spatialBlend = 1f;
            derit.maxDistance = 14f;
            derit.rolloffMode = AudioRolloffMode.Linear;
            derit.outputAudioMixerGroup = GrupMixer("SFX");
            derit.clip = PembuatPengganti.Klip(Sfx, "PintuBerderit");
            Set(pintu, "derit", derit);
            return g;
        }

        // Bentuk pintu kamar Raka hasil edit manual tim (23 Nov): acuan semua pintu kamar ber-prefab DaunPintu.
        static void BentukPintuRaka(Transform daun)
        {
            daun.localPosition = new Vector3(0.725f, 1.42844f, -0.015f);
            daun.localRotation = Quaternion.identity;
            daun.localScale = Vector3.one;
            foreach (var n in new[] { "Cube.002", "Cube.007" }) Anak(daun, n).localPosition = new Vector3(-1.643f, -1.027f, -0.1f);
            Anak(daun, "Cube.003").localPosition = new Vector3(-1.6430004f, -1.0269995f, 0.0999999f);
            Anak(daun, "Cube.006").localPosition = new Vector3(-1.6430004f, -1.1369997f, 0f);
            Anak(daun, "Cube.001").localPosition = new Vector3(-0.451f, -0.05f, 0f);
        }

        // Ambang dinding di atas celah pintu kamar (acuan: ambang kamar Raka, bawah 2,04 m sampai plafon).
        static void Ambang(Vector3 tengahCelah, Vector3 ukuranDatar) =>
            Balok("Dinding", new Vector3(tengahCelah.x, 2.5185f, tengahCelah.z), new Vector3(ukuranDatar.x, 0.963f, ukuranDatar.z), "DindingRumah");

        internal static Transform Anak(Transform t, string nama)
        {
            foreach (var c in t.GetComponentsInChildren<Transform>(true)) if (c.name == nama) return c;
            throw new System.InvalidOperationException($"Anak {nama} tidak ada di {t.name}");
        }

        static Bounds Batas(GameObject g)
        {
            var rs = g.GetComponentsInChildren<Renderer>();
            var b = rs[0].bounds;
            foreach (var r in rs) b.Encapsulate(r.bounds);
            return b;
        }

        const float AtasMejaCor = 0.85f;
        // Meja makan di atas karpet dapur (edit manual tim 23 Nov); PembuatMalam3/5 menaruh benda di atasnya.
        internal static readonly Vector3 MejaMakan = new Vector3(-1.006f, 0f, 10.6f);
        internal const float AtasMejaMakan = 0.78f;

        // Meja dapur cor bentuk L menempel dinding barat (x -7) dan sekat dapur (z 8): kaki cor 0,3 m, papan cor 7 cm berlapis
        // ubin putih, rongga terbuka di antara kaki. Ujung timur x -3: jauh dari pintu dapur (x -0,6), anglo,
        // pintu belakang, lubang plafon, dan titik muncul Sosok. Balok ber-BoxCollider: penghalang padat.
        static void MejaCor()
        {
            const float xd = -6.92f, zd = 8.08f, dalam = 0.6f, dalamSelatan = 0.723f, yPapan = 0.77f, tebalPapan = 0.07f;
            void Cor(string nama, float x1, float x2, float z1, float z2, float y1, float y2, string bahan) =>
                Balok(nama, new Vector3((x1 + x2) / 2f, (y1 + y2) / 2f, (z1 + z2) / 2f), new Vector3(x2 - x1, y2 - y1, z2 - z1), bahan);
            // Lengan barat (z 8,08..10) dan lengan selatan (x .. -3).
            Cor("MejaCorPapanBarat", xd, xd + dalam, zd, 10f, yPapan, yPapan + tebalPapan, "Beton");
            Cor("MejaCorPapanSelatan", xd + dalam, -3f, zd, zd + dalamSelatan, yPapan, yPapan + tebalPapan, "Beton");
            Cor("MejaCorUbinBarat", xd, xd + dalam, zd, 10f, yPapan + tebalPapan, AtasMejaCor, "UbinMandi");
            Cor("MejaCorUbinSelatan", xd + dalam, -3f, zd, zd + dalamSelatan, yPapan + tebalPapan, AtasMejaCor, "UbinMandi");
            Cor("MejaCorKakiUtara", xd, xd + dalam, 9.7f, 10f, 0f, yPapan, "Beton");
            Cor("MejaCorKakiTengah", -4.8f, -4.5f, zd, zd + dalamSelatan, 0f, yPapan, "Beton");
            Cor("MejaCorKakiTimur", -3.3f, -3f, zd, zd + dalamSelatan, 0f, yPapan, "Beton");   // rongga x -4,5..-3,3: tabung gas
        }

        // Puncak renderer benda (untuk menaruh benda kecil di atasnya).
        static float Atas(GameObject g)
        {
            float y = float.MinValue;
            foreach (var r in g.GetComponentsInChildren<Renderer>()) y = Mathf.Max(y, r.bounds.max.y);
            return y;
        }

        // Lampu listrik yang padam pada cutscene 23.00.
        static Light[] LampuListrik() => new[]
        {
            Listrik("Lampu kamar Ibu", new Vector3(-5f, 2.6f, 6f)),
            Listrik("Lampu kamar Raka", new Vector3(-5f, 2.6f, 2f)),
            Listrik("Lampu kamar Laras", new Vector3(5f, 2.6f, 6f)),
            // Edit manual tim (23 Nov): lampu plafon ruang tengah dihapus; dapur tiga lampu, gudang satu lampu kecil.
            Listrik("Lampu dapur 1", new Vector3(1f, 2.6f, 10f)),
            Listrik("Lampu dapur 2", new Vector3(-2f, 2.6f, 10f)),
            Listrik("Lampu dapur 3", new Vector3(-5f, 2.6f, 10f)),
            Listrik("Lampu gudang", new Vector3(5f, 2.6f, 10f), 0.5f, 3f),
            Listrik("Lampu kamar mandi", new Vector3(5f, 2.6f, 2f)),   // lampu plafon bulat (dulu bohlam)
            BohlamGantung("Lampu bohlam ruang tengah", new Vector3(0.21f, 2.036f, 4.006f)),   // edit manual tim
        };

        // Bohlam (skala 3, edit manual tim) dengan cahaya tepat di bawahnya.
        static Light BohlamGantung(string nama, Vector3 posisi)
        {
            PembuatPengganti.Pasang("Bohlam", null, posisi).transform.localScale = Vector3.one * 3f;
            return Lampu(nama, posisi + Vector3.down * 0.2f, WarnaLampuRumah, 6f, 1.4f);
        }

        // Lampu neon di bawah plafon tepat di atas cahayanya. Terpisah dari Light supaya tetap tampak saat listrik padam.
        static Light Listrik(string nama, Vector3 posisi, float skala = 1f, float jangkauan = 7f)
        {
            PembuatPengganti.Pasang("LampuPlafon", null, new Vector3(posisi.x, Tinggi - 0.1f, posisi.z)).transform.localScale = Vector3.one * skala;   // D2
            return Lampu(nama, posisi, WarnaLampuRumah, jangkauan, 1.6f);
        }

        // Angka tahun di sisi selatan blandar. Hanya terbaca saat disorot senter (TerlihatSaatDisorot).
        static void Ukiran(Rig rig)
        {
            var ukiran = new GameObject("Ukiran blandar");
            ukiran.transform.position = new Vector3(0f, 2.75f, 5.322f);
            var teks = ukiran.AddComponent<TextMesh>();
            var font = AssetDatabase.LoadAssetAtPath<Font>(JalurFont) ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var tampilan = ukiran.GetComponent<MeshRenderer>();
            teks.font = font;
            tampilan.sharedMaterial = font.material;
            teks.text = GembokLemari.KodeBenar;
            teks.anchor = TextAnchor.MiddleCenter;
            teks.alignment = TextAlignment.Center;
            teks.fontSize = 64;
            teks.characterSize = 0.04f;
            teks.color = Hex("E6E0D1");

            var sorot = ukiran.AddComponent<TerlihatSaatDisorot>();
            Set(sorot, "senter", rig.Senter);
            Set(sorot, "kamera", rig.Kamera);
            Set(sorot, "tampilan", tampilan);
            Set(sorot, "hud", rig.Hud);
            SetTeks(sorot, "pesan", "Ada ukiran angka di blandar.");
        }

        static Transform KamarIbu(Rig rig, AlurPrologRumah prolog, AlurMalam1 malam1)
        {
            PembuatPengganti.Pasang("Ranjang", null, new Vector3(-6.2f, 0f, 6f));
            PembuatPengganti.Pasang("Meja", null, new Vector3(-6.3f, 0f, 4.45f));
            // K4 opsi B: duduk menunggu sampai kejadian Malam 1 berikutnya (obat 21.00, cutscene 23.00).
            var kursi = PembuatPengganti.Pasang("Kursi", null, new Vector3(-3.6f, 0f, 4.6f), 135f).AddComponent<KursiTunggu>();   // diputar tim
            Set(kursi, "kartu", rig.Kartu);
            SetTeks(kursi, "prompt", "Kursi");

            // Ibu: objek logika di atas kasur (permukaan kasur y = 0.54). Adegan hanya mengguncang visualnya.
            var ibu = Logika("Ibu", new Vector3(-6.2f, 0.54f, 6f), new Vector3(1.1f, 0.5f, 1.9f));
            var visualIbu = PembuatPengganti.Pasang("BuRatmi_Berbaring", ibu.transform, Vector3.zero);
            var obat = ibu.AddComponent<PemberiObat>();
            Set(obat, "hud", rig.Hud);
            Set(obat, "panel", rig.Dialog);
            Set(obat, "dialogSetelahObat", PembuatDataFase1.Muat(PembuatDataFase1.IbuSetelahObat));
            Set(malam1, "ibu", visualIbu.transform);
            // K14-A: wajah Ibu memucat mengikuti Kondisi Ibu. Hanya renderer kepala model tokoh.
            var kulit = System.Array.FindAll(visualIbu.GetComponentsInChildren<Renderer>(), r => r.name == "head");
            if (kulit.Length > 0) SetDaftar(visualIbu.AddComponent<WajahIbu>(), "kulit", kulit);
            else Debug.LogWarning("Model Ibu tanpa renderer kepala: WajahIbu tidak dipasang.");

            // Lemari Bapak di dinding utara kamar, menghadap selatan; gembok di depan pintunya.
            // Edit manual tim (23 Nov): lemari 0,389 m ke timur, gembok lebih besar tanpa kunci.
            var lemari = Logika("LemariAyah", new Vector3(-3.811f, 0f, 7.6f), new Vector3(1.2f, 2f, 0.6f));
            PembuatPengganti.Pasang("LemariAyah", lemari.transform, Vector3.zero, 180f);
            var gembok = PembuatPengganti.Pasang("Gembok", lemari.transform, new Vector3(0f, 0.94425f, -0.2605f), 180f);
            gembok.transform.localScale = Vector3.one * 1.9240447f;
            var kunci = gembok.GetComponentsInChildren<Transform>(true);
            foreach (var t in kunci) if (t.name == "Key") Object.DestroyImmediate(t.gameObject);
            var skripLemari = lemari.AddComponent<LemariAyah>();
            Set(skripLemari, "panel", rig.Gembok);
            Set(skripLemari, "gembok", gembok);
            SetTeks(skripLemari, "prompt", "Lemari Bapak");

            Set(prolog, "zonaKamarIbu", Zona("ZonaKamarIbu", new Vector3(-4.2f, 1.2f, 6f), new Vector3(1.4f, 2.4f, 2.4f)));
            Set(malam1, "titikMulai", Titik("TitikSampingRanjang", new Vector3(-4.6f, 0.05f, 6f), new Vector3(-6.2f, 0.05f, 6f)));
            var bidikIbu = Titik("BidikIbu", new Vector3(-4.3f, 1.7f, 5.2f), new Vector3(-6.2f, 0.7f, 6f));
            Set(prolog, "bidikIbu", bidikIbu);
            Set(malam1, "bidikIbu", bidikIbu);
            Cutscene23(rig, malam1, bidikIbu);
            return visualIbu.transform;   // Malam 3: Ibu menjerit (diguncang AlurMalam3)
        }

        // Timeline Malam1_Cutscene23 (11,5 detik), urutan sama dengan Game Story Malam 1:
        // 0 kejang + batuk (3 detik), 3 paku + darah, 5 listrik padam, 6,5 potong ke plafon + langkah di atap.
        static void Cutscene23(Rig rig, AlurMalam1 malam1, Transform bidikIbu)
        {
            var dekat = Titik("BidikIbuDekat", new Vector3(-4.8f, 1.6f, 5.5f), new Vector3(-6.2f, 0.7f, 6f));
            var atap = Titik("BidikAtap", new Vector3(-5f, 1f, 5.5f), new Vector3(-5.5f, 3f, 6.5f));
            var c = PembuatTimeline.Buat("Malam1_Cutscene23", 11.5f);
            // Kamera mendekat pelan ke ranjang, lalu potong ke plafon dan diam di sana.
            PembuatTimeline.Gerak(c.Tl, c.Director, rig.KameraCutscene.gameObject,
                (0f, bidikIbu), (6.49f, dekat), (6.5f, atap), (11.5f, atap));
            PembuatTimeline.Sinyal(c.Tl, 0.05f, AlurMalam1.SinyalKejang);
            PembuatTimeline.Sinyal(c.Tl, 3f, AlurMalam1.SinyalPaku);
            PembuatTimeline.Sinyal(c.Tl, 5f, AlurMalam1.SinyalPadam);
            PembuatTimeline.Sinyal(c.Tl, 6.5f, AlurMalam1.SinyalLangkah);
            PembuatTimeline.Simpan(c.Tl);
            Set(malam1, "timeline23", c.Pemutar);
        }

        // Objek yang baru dinyalakan AlurMalam1 pada cutscene 23.00. Semuanya dibuat nonaktif.
        static void BendaAdegan(AlurMalam1 malam1)
        {
            // Paku tergeletak di tengah bercak darah, dan sasaran foto menutupi seluruh bercak: pemain membidik
            // bercak (benda yang paling terlihat dalam gelap), bukan paku setebal 2 cm. Setinggi 0,12 m supaya menutupi collider
            // PapanLantai Malam 2 (tinggi 0,1, selalu ada) yang melintang di bercak; tanpa itu sinar Q kena papan, bukan paku.
            var paku = Logika("Paku", new Vector3(-5.1f, 0f, 6.3f), new Vector3(0.9f, 0.12f, 0.9f));
            PembuatPengganti.Pasang("Paku", paku.transform, Vector3.zero);
            var foto = paku.AddComponent<PetunjukFoto>();
            SetTeks(foto, "efek", "tujuan:PotretPaku; foto:PakuMalam1");
            SetTeks(foto, "pesan", "Paku dipotret.");

            var bercak = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bercak.name = "Bercak";
            bercak.transform.position = new Vector3(-5.1f, 0.005f, 6.3f);
            bercak.transform.localScale = new Vector3(0.6f, 0.005f, 0.6f);
            bercak.GetComponent<Renderer>().sharedMaterial = PembuatPengganti.Material("Karat");
            Object.DestroyImmediate(bercak.GetComponent<Collider>());

            var teplok = new GameObject("Teplok");
            teplok.transform.position = new Vector3(-6.3f, 0.76f, 4.45f);
            PembuatPengganti.Pasang("Teplok", teplok.transform, Vector3.zero);
            Lampu("CahayaTeplok", new Vector3(-6.3f, 1f, 4.45f), WarnaTeplok, 6f, 2f).transform.SetParent(teplok.transform, true);

            var larasBerdoa = PembuatPengganti.Pasang("Laras_Berdoa", null, new Vector3(-5.2f, 0f, 4.9f), -60f);

            Set(malam1, "paku", paku);
            Set(malam1, "darah", bercak);
            Set(malam1, "teplok", teplok);
            Set(malam1, "larasBerdoa", larasBerdoa);
            foreach (var g in new[] { paku, bercak, teplok, larasBerdoa }) g.SetActive(false);
        }

        static GameObject Tokoh(Rig rig, GameObject sutradara, AlurPrologRumah prolog, AlurMalam1 malam1)
        {
            var laras = BendaDialog(rig, "Laras", new Vector3(0.8f, 0f, 6.6f), new Vector3(0.5f, 1.7f, 0.5f), 180f, false,
                PembuatDataFase1.Muat(PembuatDataFase1.LarasKronologi), "Bicara dengan Laras");
            Set(malam1, "larasRuangTengah", laras.gameObject);

            // Bidan Wulan berdiri di ambang pintu kamar Ibu dan menutup jalan sampai ia pamit (AlurPrologRumah).
            var bidan = BendaDialog(rig, "BidanWulan", new Vector3(-3f, 0f, 6f), new Vector3(0.5f, 1.8f, 1f), 90f, false,
                PembuatDataFase1.Muat(PembuatDataFase1.BidanWulan), "Bicara dengan Bidan Wulan");
            SetBool(bidan, "sekaliSaja", true);
            bidan.transform.Find("BidanWulan").localPosition = new Vector3(0.194f, 0f, -0.09f);   // edit manual tim
            Set(prolog, "bidanWulan", bidan.gameObject);

            // Bidan hanya ada di Prolog: "Lanjutkan" di Malam 1 memuat scene ini tanpa dia.
            var hanyaProlog = sutradara.AddComponent<AktifDiMalam>();
            SetAngka(hanyaProlog, "dari", 0);
            SetAngka(hanyaProlog, "sampai", 0);
            SetSatuObjek(hanyaProlog, "sasaran", bidan.gameObject);
            return laras.gameObject;
        }

        static void SuaraDanJadwal(Rig rig, GameObject sutradara, AlurMalam1 malam1)
        {
            var jadwal = sutradara.AddComponent<JadwalObat>();
            Set(jadwal, "hud", rig.Hud);
            Set(jadwal, "panel", rig.Dialog);
            Set(jadwal, "teguranLaras", PembuatDataFase1.Muat(PembuatDataFase1.TeguranObat));
            Set(jadwal, "dentang", BuatSuara(rig, "DentangJam", Sfx, "SFX", "(Suara: jam dinding berdentang.)"));

            Set(malam1, "suaraBatuk", BuatSuara(rig, "SuaraMauMuntah", Sfx, "SFX", "(Suara: Ibu terbatuk.)"));
            Set(malam1, "suaraPaku", BuatSuara(rig, "PakuJatuh", Sfx, "SFX", "(Suara: logam jatuh ke lantai.)"));
            Set(malam1, "suaraListrik", BuatSuara(rig, "ListrikPadam", Sfx, "SFX", "(Suara: listrik padam.)"));
            Set(malam1, "suaraLangkah", BuatSuara(rig, "LangkahDiAtap", Sfx, "SFX", "(Suara: langkah di atap.)"));
            Set(malam1, "musikDrone", BuatSuara(rig, PembuatPengganti.AdaSuara(Musik, "MusikMalam") ? "MusikMalam" : "DroneHoror", Musik, "Music", "(Musik latar.)", ulang: true));
        }
    }
}
