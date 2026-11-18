using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheSending.EditorTools
{
    // Prefab aset asli di Prefabs/Asli/<nama sama dengan pengganti>.prefab, dibangun dari paket di Art/ThirdParty.
    // PembuatPengganti.Pasang memakai prefab Asli bila ada. Prefab Asli SELALU dibuat ulang (jangan disunting tangan);
    // ubah angka di sini. Ukuran, depan (+Z), dan titik nol (lantai) sama dengan prefab pengganti.
    // Juga memberi tekstur pada material pengganti dan menyediakan kubus ber-UV meter (PembuatRig.Balok).
    // Batchmode: -executeMethod TheSending.EditorTools.PembuatAsetAsli.Buat
    public static class PembuatAsetAsli
    {
        internal const string FolderPrefab = "Assets/_Project/Prefabs/Asli";
        const string FolderMesh = "Assets/_Project/Art/Models/Asli";
        const string Pihak3 = "Assets/_Project/Art/ThirdParty";
        internal const string Alam = Pihak3 + "/EssentialNaturePack/Prefabs/";
        const string Interior = Pihak3 + "/HouseInteriorPack/Prefabs/(Prb)";
        const string Kit = Pihak3 + "/TraditionalHouseModular/Prefabs/Modular/";
        const string TeksturAlam = Pihak3 + "/EssentialNaturePack/Materials/Textures/";
        const string TeksturKit = Pihak3 + "/TraditionalHouseModular/Material/";
        const string TeksturBeton = Pihak3 + "/YughuesConcrete/Textures/T_YFCM_";
        const string Pocong = Pihak3 + "/pocong-scary/";
        const string SofaTua = Pihak3 + "/old-sofas/";
        // Grup OBJ sofa tiga dudukan (old-sofas berisi empat sofa).
        static readonly string[] SofaTigaDudukan = { "couchinbase_low", "couchin_low", "Couchinl_low", "Couchinm_low", "Couchinr_low" };

        // Tekstur untuk material pengganti: (material, tekstur, warna pengali, meter per ulangan). Palet tetap gelap.
        static readonly (string bahan, string tekstur, string hex, float meter)[] DaftarTekstur =
        {
            ("KayuTua", TeksturKit + "tileable-1/512 - native/T1-A 512.png", "9A8270", 2f),
            ("KayuGelap", TeksturKit + "tileable-1/512 - native/T1-A 512.png", "54443A", 2f),
            ("Gedek", TeksturKit + "tileable-3/512 - native/T3-A 512.png", "9C8C70", 1.5f),
            ("Tanah", TeksturAlam + "Forest_Ground.png", "6E6458", 3f),
            ("Genteng", TeksturBeton + "RevealingBricks_d.tga", "8C5A48", 2f),
            ("Bambu", TeksturAlam + "Bamboo_wood.png", "A09C78", 1f),
            ("Kain", TeksturBeton + "Plastered_d.tga", "B9AE96", 2f),
            ("Atap", TeksturKit + "tileable-4/512 - native/T4-A 512.PNG", "7A6A5A", 2.5f),
            ("Tembok", TeksturBeton + "Moldy_d.tga", "8A887E", 2f),
            ("TanahHutan", TeksturAlam + "Forest_Leaves.png", "5A5248", 4f),
            ("Jalan", TeksturAlam + "Forest_Road.png", "6A6058", 4f),
            ("Kafan", Pocong + "textures/Baked_BaseColor.png", "4A4440", 1f),
            ("Sofa", SofaTua + "textures/low_old sofa_BaseColor.png", "8A7A6A", 1f),
        };

        [MenuItem("The Sending/Buat Aset Asli")]
        public static void Buat()
        {
            Directory.CreateDirectory(FolderPrefab);
            Directory.CreateDirectory(FolderMesh);
            AssetDatabase.Refresh();
            Teksturkan();
            // Perabot nappin berwarna modern: semua materialnya diganti kayu supaya cocok dengan rumah desa 2003.
            Prefab("Meja", r => Kayu(Taruh(Interior + "Desk.prefab", r, Vector3.zero, new Vector3(1.0f, 0.75f, 0.6f), 90f), "KayuTua"), true);
            Prefab("Kursi", r => Kayu(Taruh(Interior + "DiningChair.prefab", r, Vector3.zero, new Vector3(0.45f, 0.95f, 0.45f), 90f), "KayuTua"), true);
            Prefab("LemariAyah", r => Kayu(Taruh(Interior + "Drawer1.prefab", r, Vector3.zero, new Vector3(1.2f, 2.0f, 0.6f), -90f), "KayuGelap"), true);
            Prefab("Laci", r => Kayu(Taruh(Interior + "Storage1.prefab", r, Vector3.zero, new Vector3(0.9f, 0.85f, 0.45f), -90f), "KayuGelap"), true);
            Prefab("SakaGuru", SakaGuru, true);
            Prefab("RumpunBambu", RumpunBambu, false);
            Prefab("RumahKecil", RumahKecil, false);
            Prefab("RumahKeluargaLuar", RumahKeluargaLuar, false);
            // Sosok Santet (K9-A): pocong-scary, sekitar 30.000 segitiga, tanpa rig; meluncur tanpa animasi.
            Prefab("SosokSantet", r => Kayu(Tokoh(Pocong + "Model/pocong.fbx", r, 2.2f, 0f, -90f), "Kafan"), false);   // FBX berbaring (sumbu Z ke atas): ditegakkan
            // Sofa ruang tengah (K22-A): sofa tiga dudukan dari old-sofas, satu tekstur.
            Prefab("Sofa", r => Kayu(Taruh(SofaTua + "Model/old sofas.obj", r, Vector3.zero, new Vector3(1.9f, 0.8f, 0.8f), 0f,
                hanya: SofaTigaDudukan), "Sofa"), true);
            AssetDatabase.SaveAssets();
            Debug.Log("Aset asli siap.");
        }

        static void Teksturkan()
        {
            foreach (var (bahan, jalur, hex, meter) in DaftarTekstur)
            {
                var mat = PembuatPengganti.Material(bahan);
                var tekstur = AssetDatabase.LoadAssetAtPath<Texture2D>(jalur)
                    ?? throw new FileNotFoundException($"Tekstur untuk material {bahan} tidak ada: {jalur}");
                mat.SetTexture("_BaseMap", tekstur);
                mat.SetTextureScale("_BaseMap", Vector2.one / meter);   // UV kubus dan atap dalam meter
                mat.SetColor("_BaseColor", PembuatRig.Hex(hex));
                mat.SetFloat("_Smoothness", 0f);
                EditorUtility.SetDirty(mat);
            }
        }

        static void Prefab(string nama, Action<Transform> isi, bool colliderKotak)
        {
            var akar = new GameObject(nama);
            isi(akar.transform);
            if (colliderKotak)
            {
                var b = Batas(akar.transform);
                var kotak = akar.AddComponent<BoxCollider>();
                kotak.center = b.center;
                kotak.size = b.size;
            }
            PrefabUtility.SaveAsPrefabAsset(akar, $"{FolderPrefab}/{nama}.prefab");
            Object.DestroyImmediate(akar);
        }

        // Batas gabungan semua renderer (akar prefab berada di titik nol dunia saat dibangun).
        static Bounds Batas(Transform t)
        {
            var rs = t.GetComponentsInChildren<Renderer>();
            if (rs.Length == 0) throw new InvalidOperationException($"{t.name} tidak punya renderer.");
            var b = rs[0].bounds;
            foreach (var r in rs) b.Encapsulate(r.bounds);
            return b;
        }

        // Model vendor diputar, diskalakan ke `ukuran` (per sumbu), lalu alas tengahnya ditaruh di `alas`. Collider vendor dibuang.
        // hanya: nama anak langsung model yang dipakai (misalnya satu sofa dari OBJ berisi empat); null = semua.
        internal static GameObject Taruh(string jalur, Transform induk, Vector3 alas, Vector3 ukuran, float putarY, bool collider = false,
            string[] hanya = null)
        {
            var asal = AssetDatabase.LoadAssetAtPath<GameObject>(jalur) ?? throw new FileNotFoundException(jalur);
            var bungkus = new GameObject(asal.name).transform;
            bungkus.SetParent(induk, false);
            var model = (GameObject)PrefabUtility.InstantiatePrefab(asal);
            PrefabUtility.UnpackPrefabInstance(model, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            if (hanya != null) Saring(model.transform, hanya, jalur);
            foreach (var c in model.GetComponentsInChildren<Collider>(true)) Object.DestroyImmediate(c);
            model.transform.SetParent(bungkus, false);
            model.transform.localRotation = Quaternion.Euler(0f, putarY, 0f);

            var b = Batas(bungkus);
            bungkus.localScale = new Vector3(ukuran.x / b.size.x, ukuran.y / b.size.y, ukuran.z / b.size.z);
            b = Batas(bungkus);
            bungkus.position += induk.TransformPoint(alas) - new Vector3(b.center.x, b.min.y, b.center.z);
            if (collider)
            {
                var kotak = bungkus.gameObject.AddComponent<BoxCollider>();
                kotak.center = bungkus.InverseTransformPoint(Batas(bungkus).center);
                kotak.size = Vector3.Scale(ukuran, new Vector3(1f / bungkus.lossyScale.x, 1f / bungkus.lossyScale.y, 1f / bungkus.lossyScale.z));
            }
            return bungkus.gameObject;
        }

        static void Saring(Transform model, string[] hanya, string jalur)
        {
            var ada = new List<string>();
            for (int i = model.childCount - 1; i >= 0; i--)
            {
                var anak = model.GetChild(i);
                ada.Add(anak.name);
                if (Array.IndexOf(hanya, anak.name) < 0) Object.DestroyImmediate(anak.gameObject);
            }
            if (model.childCount == 0)
                throw new InvalidOperationException($"{jalur}: tidak ada anak bernama {string.Join(", ", hanya)}. Anak yang ada: {string.Join(", ", ada)}");
        }

        // Tokoh: diskalakan seragam sampai setinggi `tinggi` (Taruh menskalakan per sumbu dan membuat tokoh gepeng),
        // alas tengahnya di titik nol induk. Collider vendor dibuang.
        static GameObject Tokoh(string jalur, Transform induk, float tinggi, float putarY, float putarX = 0f)
        {
            var asal = AssetDatabase.LoadAssetAtPath<GameObject>(jalur) ?? throw new FileNotFoundException(jalur);
            var bungkus = new GameObject(asal.name).transform;
            bungkus.SetParent(induk, false);
            var model = (GameObject)PrefabUtility.InstantiatePrefab(asal);
            PrefabUtility.UnpackPrefabInstance(model, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            foreach (var c in model.GetComponentsInChildren<Collider>(true)) Object.DestroyImmediate(c);
            model.transform.SetParent(bungkus, false);
            model.transform.localRotation = Quaternion.Euler(putarX, putarY, 0f);
            var b = Batas(bungkus);
            bungkus.localScale = Vector3.one * (tinggi / b.size.y);
            b = Batas(bungkus);
            bungkus.position += induk.position - new Vector3(b.center.x, b.min.y, b.center.z);
            return bungkus.gameObject;
        }

        // ---------- Perabot dan alam ----------

        static void Kayu(GameObject g, string bahan)
        {
            var mat = PembuatPengganti.Material(bahan);
            foreach (var r in g.GetComponentsInChildren<Renderer>())
                r.sharedMaterials = Enumerable.Repeat(mat, r.sharedMaterials.Length).ToArray();
        }

        static void SakaGuru(Transform r)
        {
            Kotak(r, "Umpak", "Tembok", new Vector3(0f, 0.1f, 0f), new Vector3(0.45f, 0.2f, 0.45f), false);
            Taruh(Kit + "pe3.prefab", r, new Vector3(0f, 0.2f, 0f), new Vector3(0.3f, 2.8f, 0.3f), 0f);
        }

        static void RumpunBambu(Transform r)
        {
            // Lima batang bambu tinggi dan dua rumpun kecil di kakinya; satu collider di pangkal.
            (float x, float z, float putar)[] batang = { (0f, 0f, 0f), (0.7f, 0.4f, 70f), (-0.6f, 0.3f, 150f), (0.2f, -0.7f, 220f), (-0.5f, -0.5f, 300f) };
            foreach (var (x, z, putar) in batang) Vendor(Alam + "Bamboo.prefab", r, new Vector3(x, 0f, z), putar, 1f);
            Vendor(Alam + "LittleBamboo.prefab", r, new Vector3(0.5f, 0f, -0.3f), 30f, 1.3f);
            Vendor(Alam + "LittleBamboo.prefab", r, new Vector3(-0.5f, 0f, 0.4f), 200f, 1.3f);
            Vendor(Alam + "LittleBamboo.prefab", r, new Vector3(0.1f, 0f, 0.6f), 120f, 1.1f);
            var kapsul = r.gameObject.AddComponent<CapsuleCollider>();
            kapsul.radius = 0.5f;
            kapsul.height = 4f;
            kapsul.center = new Vector3(0f, 2f, 0f);
        }

        const float BatasLod0 = 0.5f;   // dengan lodBias 2 (kualitas PC)

        // Prefab vendor tanpa collider. Dipakai juga untuk hutan di PembuatDesa.
        internal static GameObject Vendor(string jalur, Transform induk, Vector3 posisi, float putarY, float skala)
        {
            var asal = AssetDatabase.LoadAssetAtPath<GameObject>(jalur) ?? throw new FileNotFoundException(jalur);
            var g = (GameObject)PrefabUtility.InstantiatePrefab(asal);
            foreach (var c in g.GetComponentsInChildren<Collider>(true)) Object.DestroyImmediate(c);
            g.transform.SetParent(induk, false);
            g.transform.localPosition = posisi;
            g.transform.localRotation = Quaternion.Euler(0f, putarY, 0f);
            g.transform.localScale = Vector3.one * skala;
            // LOD0 hanya sampai sekitar 20 m untuk pohon 6 m; lebih jauh kabut menutupinya, jadi papan silang LOD1 cukup.
            var lod = g.GetComponent<LODGroup>();
            if (lod != null && lod.lodCount > 1)
            {
                var lods = lod.GetLODs();
                lods[0].screenRelativeTransitionHeight = Mathf.Max(lods[0].screenRelativeTransitionHeight, BatasLod0);
                lod.SetLODs(lods);
            }
            return g;
        }

        // ---------- Rumah gaya limasan dari kit TraditionalHouseModular ----------

        // Rumah Bu Sumi: badan 6 x 5 m (x -3..3, z -2.5..2.5), depan +Z, jendela di (1.2, 1.5, 2.5), pintu di x -1.5.
        static void RumahKecil(Transform r)
        {
            const float tinggi = 2.8f;
            Badan(r, -3f, 3f, -2.5f, 2.5f, tinggi, "Gedek", "Gedek");
            // Jendela: kaca gelap dan bingkai. Cahaya, siluet, dan tirai dipasang PembuatDesa di depannya.
            Kotak(r, "KacaJendela", "Gelap", new Vector3(1.2f, 1.5f, 2.51f), new Vector3(1.1f, 1.1f, 0.02f), false);
            Bingkai(r, new Vector3(1.2f, 1.5f, 2.55f), 1.1f, 1.1f);
            Pintu(r, new Vector3(-1.5f, 0f, 2.5f));
            Taruh(Kit + "pd4.prefab", r, new Vector3(1.2f, 2.15f, 2.56f), new Vector3(0.9f, 0.35f, 0.05f), 0f);
            Atap(r, "AtapRumahKecil", new Vector3(0f, tinggi, 0f), 3.5f, 3.0f, 0.8f);
        }

        // Rumah keluarga: titik nol di ambang pintu depan, badan 14 x 12 m ke -Z, emper 8 x 2,2 m ke +Z.
        // PembuatDesa menaruh balok PintuRumah (interaktif) di depan dinding depan, jadi dinding depan tetap utuh.
        static void RumahKeluargaLuar(Transform r)
        {
            const float tinggi = 3f;
            Badan(r, -7f, 7f, -12f, 0f, tinggi, "KayuTua", "Gedek");
            Bingkai(r, new Vector3(0f, 1.05f, 0.27f), 1.1f, 2.1f);
            Taruh(Kit + "pd4.prefab", r, new Vector3(0f, 2.3f, 0.05f), new Vector3(1.1f, 0.45f, 0.06f), 0f);
            foreach (float x in new[] { -4f, 4f })
            {
                Kotak(r, "KacaJendela", "Gelap", new Vector3(x, 1.5f, 0.01f), new Vector3(1.0f, 1.0f, 0.02f), false);
                Taruh(Kit + "pd4.prefab", r, new Vector3(x, 1.0f, 0.05f), new Vector3(1.0f, 1.0f, 0.06f), 0f);
                Bingkai(r, new Vector3(x, 1.5f, 0.06f), 1.0f, 1.0f);
            }
            // Emper: lantai, empat tiang, pagar kisi di kiri-kanan, atap sorong.
            Kotak(r, "LantaiEmper", "Tembok", new Vector3(0f, 0.025f, 1.1f), new Vector3(8.4f, 0.05f, 2.2f), false);
            foreach (float x in new[] { -3.9f, -1.6f, 1.6f, 3.9f })
                Taruh(Kit + "pe3.prefab", r, new Vector3(x, 0f, 1.9f), new Vector3(0.2f, 2.55f, 0.2f), 0f, collider: true);
            foreach (float x in new[] { -2.75f, 2.75f })
                Taruh(Kit + "pf2.prefab", r, new Vector3(x, 0f, 1.9f), new Vector3(2.1f, 0.8f, 0.1f), 0f, collider: true);
            var emper = new MeshDuaSisi();
            emper.Segiempat(new Vector3(-4.4f, 2.95f, 0f), new Vector3(4.4f, 2.95f, 0f), new Vector3(4.4f, 2.5f, 2.4f), new Vector3(-4.4f, 2.5f, 2.4f));
            Pasang(r, "AtapEmper", emper.Simpan("AtapEmper"), "Atap");
            Atap(r, "AtapRumahKeluarga", new Vector3(0f, tinggi, -6f), 7.6f, 6.6f, 1.8f);
        }

        // Badan rumah: dinding (bahan depan dan bahan sisi lain), alas tembok lumut, plafon, dan tiang sudut dari kit.
        static void Badan(Transform r, float x1, float x2, float z1, float z2, float tinggi, string depan, string sisi)
        {
            const float tebal = 0.15f, alas = 0.3f;
            float lebar = x2 - x1, dalam = z2 - z1, cx = (x1 + x2) / 2f, cz = (z1 + z2) / 2f, ty = alas + (tinggi - alas) / 2f;
            Kotak(r, "DindingDepan", depan, new Vector3(cx, ty, z2 - tebal / 2f), new Vector3(lebar, tinggi - alas, tebal), true);
            Kotak(r, "DindingBelakang", sisi, new Vector3(cx, ty, z1 + tebal / 2f), new Vector3(lebar, tinggi - alas, tebal), true);
            Kotak(r, "DindingKiri", sisi, new Vector3(x1 + tebal / 2f, ty, cz), new Vector3(tebal, tinggi - alas, dalam), true);
            Kotak(r, "DindingKanan", sisi, new Vector3(x2 - tebal / 2f, ty, cz), new Vector3(tebal, tinggi - alas, dalam), true);
            Kotak(r, "Alas", "Tembok", new Vector3(cx, alas / 2f, cz), new Vector3(lebar + 0.04f, alas, dalam + 0.04f), true);
            Kotak(r, "Plafon", "KayuGelap", new Vector3(cx, tinggi + 0.03f, cz), new Vector3(lebar, 0.06f, dalam), false);
            foreach (float x in new[] { x1, x2 })
                foreach (float z in new[] { z1, z2 })
                    Taruh(Kit + "pe3.prefab", r, new Vector3(x, 0f, z), new Vector3(0.22f, tinggi, 0.22f), 0f);
        }

        static void Bingkai(Transform r, Vector3 pusat, float lebar, float tinggi)
        {
            const float t = 0.08f;
            Kotak(r, "Bingkai", "KayuGelap", pusat + new Vector3(0f, (tinggi + t) / 2f, 0f), new Vector3(lebar + 2f * t, t, t), false);
            Kotak(r, "Bingkai", "KayuGelap", pusat - new Vector3(0f, (tinggi + t) / 2f, 0f), new Vector3(lebar + 2f * t, t, t), false);
            Kotak(r, "Bingkai", "KayuGelap", pusat + new Vector3((lebar + t) / 2f, 0f, 0f), new Vector3(t, tinggi, t), false);
            Kotak(r, "Bingkai", "KayuGelap", pusat - new Vector3((lebar + t) / 2f, 0f, 0f), new Vector3(t, tinggi, t), false);
        }

        static void Pintu(Transform r, Vector3 alas)
        {
            Kotak(r, "Pintu", "KayuGelap", alas + new Vector3(0f, 1f, 0.02f), new Vector3(0.9f, 2f, 0.04f), false);
            Bingkai(r, alas + new Vector3(0f, 1f, 0.05f), 0.9f, 2f);
        }

        // Atap limasan dua tingkat: penanggap landai (25°) selebar `landai` m, lalu brunjung curam (40°) sampai bubungan.
        // `setX`, `setZ` = setengah ukuran tepi atap (sudah termasuk teritis); bubungan sejajar X.
        static void Atap(Transform r, string nama, Vector3 tepi, float setX, float setZ, float landai)
        {
            float setZAtas = setZ - landai;
            float y2 = tepi.y + landai * Mathf.Tan(25f * Mathf.Deg2Rad);
            float y3 = y2 + setZAtas * Mathf.Tan(40f * Mathf.Deg2Rad);
            float ax = setX - landai, ridge = ax - setZAtas;
            var m = new MeshDuaSisi();
            m.Limas(tepi, setX, setZ, y2, ax, setZAtas);
            m.Limas(new Vector3(tepi.x, y2, tepi.z), ax, setZAtas, y3, Mathf.Max(ridge, 0f), 0f);
            Pasang(r, nama, m.Simpan(nama), "Atap");
        }

        static void Pasang(Transform r, string nama, Mesh mesh, string bahan)
        {
            var g = new GameObject(nama);
            g.transform.SetParent(r, false);
            g.AddComponent<MeshFilter>().sharedMesh = mesh;
            g.AddComponent<MeshRenderer>().sharedMaterial = PembuatPengganti.Material(bahan);
        }

        static GameObject Kotak(Transform r, string nama, string bahan, Vector3 pusat, Vector3 ukuran, bool collider)
        {
            var g = new GameObject(nama);
            g.transform.SetParent(r, false);
            g.transform.localPosition = pusat;
            g.transform.localScale = ukuran;
            g.AddComponent<MeshFilter>().sharedMesh = KubusMeter(ukuran);
            g.AddComponent<MeshRenderer>().sharedMaterial = PembuatPengganti.Material(bahan);
            if (collider) g.AddComponent<BoxCollider>();
            return g;
        }

        // ---------- Mesh ----------

        const string FolderKubus = "Assets/_Project/Art/Models/Kubus";
        static readonly Dictionary<Vector3, Mesh> CacheKubus = new Dictionary<Vector3, Mesh>();

        // Kubus satuan (dipakai dengan localScale = ukuran) yang UV-nya dalam meter untuk ukuran itu, sehingga tekstur
        // berulang per meter dan tidak melar. Disimpan sebagai aset per ukuran supaya scene dan prefab bisa memakainya.
        internal static Mesh KubusMeter(Vector3 ukuran)
        {
            var kunci = new Vector3(Mathf.Round(ukuran.x * 100f) / 100f, Mathf.Round(ukuran.y * 100f) / 100f, Mathf.Round(ukuran.z * 100f) / 100f);
            if (CacheKubus.TryGetValue(kunci, out var ada) && ada != null) return ada;
            string jalur = $"{FolderKubus}/Kubus_{kunci.x:0.##}x{kunci.y:0.##}x{kunci.z:0.##}.asset";
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(jalur);
            if (mesh == null)
            {
                Directory.CreateDirectory(FolderKubus);
                mesh = BuatKubus(kunci);
                mesh.name = Path.GetFileNameWithoutExtension(jalur);
                AssetDatabase.CreateAsset(mesh, jalur);
            }
            CacheKubus[kunci] = mesh;
            return mesh;
        }

        static Mesh BuatKubus(Vector3 s)
        {
            var v = new List<Vector3>();
            var uv = new List<Vector2>();
            var tri = new List<int>();
            // Tiap sisi: normal, sumbu u, sumbu v (semuanya sumbu satuan).
            Vector3[,] sisi =
            {
                { Vector3.right, Vector3.forward, Vector3.up }, { Vector3.left, Vector3.back, Vector3.up },
                { Vector3.up, Vector3.right, Vector3.forward }, { Vector3.down, Vector3.right, Vector3.back },
                { Vector3.forward, Vector3.left, Vector3.up }, { Vector3.back, Vector3.right, Vector3.up },
            };
            for (int i = 0; i < 6; i++)
            {
                Vector3 n = sisi[i, 0], u = sisi[i, 1], w = sisi[i, 2];
                int awal = v.Count;
                foreach (var (a, b) in new[] { (-1f, -1f), (-1f, 1f), (1f, 1f), (1f, -1f) })
                {
                    var p = (n + a * u + b * w) * 0.5f;
                    v.Add(p);
                    uv.Add(new Vector2(Vector3.Dot(p, Vector3.Scale(u, s)), Vector3.Dot(p, Vector3.Scale(w, s))));
                }
                tri.AddRange(new[] { awal, awal + 1, awal + 2, awal, awal + 2, awal + 3 });
            }
            var m = new Mesh();
            m.SetVertices(v);
            m.SetUVs(0, uv);
            m.SetTriangles(tri, 0);
            m.RecalculateNormals();
            m.RecalculateBounds();
            return m;
        }

        // Mesh bidang dua sisi (atap terlihat dari bawah emper) dengan UV dalam meter.
        sealed class MeshDuaSisi
        {
            readonly List<Vector3> v = new List<Vector3>();
            readonly List<Vector2> uv = new List<Vector2>();
            readonly List<int> tri = new List<int>();

            // p0, p1 = tepi bawah; p2, p3 = tepi atas (boleh berimpit: segitiga).
            public void Segiempat(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
            {
                var u = (p1 - p0).normalized;
                var miring = p3 - p0 + (p2 - p1);
                var w = (miring - u * Vector3.Dot(miring, u)).normalized;
                for (int sisi = 0; sisi < 2; sisi++)
                {
                    int a = v.Count;
                    foreach (var p in new[] { p0, p1, p2, p3 })
                    {
                        v.Add(p);
                        uv.Add(new Vector2(Vector3.Dot(p - p0, u), Vector3.Dot(p - p0, w)));
                    }
                    if (sisi == 0) tri.AddRange(new[] { a, a + 3, a + 2, a, a + 2, a + 1 });
                    else tri.AddRange(new[] { a, a + 1, a + 2, a, a + 2, a + 3 });
                }
            }

            // Limas terpancung: persegi bawah (setengah ax, az) di `bawah`, persegi atas (bx, bz) di ketinggian yAtas.
            public void Limas(Vector3 bawah, float ax, float az, float yAtas, float bx, float bz)
            {
                Vector3 B(float x, float z) => new Vector3(bawah.x + x, bawah.y, bawah.z + z);
                Vector3 A(float x, float z) => new Vector3(bawah.x + x, yAtas, bawah.z + z);
                Segiempat(B(-ax, az), B(ax, az), A(bx, bz), A(-bx, bz));
                Segiempat(B(ax, -az), B(-ax, -az), A(-bx, -bz), A(bx, -bz));
                Segiempat(B(ax, az), B(ax, -az), A(bx, -bz), A(bx, bz));
                Segiempat(B(-ax, -az), B(-ax, az), A(-bx, bz), A(-bx, -bz));
            }

            public Mesh Simpan(string nama)
            {
                var m = new Mesh { name = nama };
                m.SetVertices(v);
                m.SetUVs(0, uv);
                m.SetTriangles(tri, 0);
                m.RecalculateNormals();
                m.RecalculateBounds();
                string jalur = $"{FolderMesh}/{nama}.asset";
                var lama = AssetDatabase.LoadAssetAtPath<Mesh>(jalur);
                if (lama == null) { AssetDatabase.CreateAsset(m, jalur); return m; }
                EditorUtility.CopySerialized(m, lama);   // tetap GUID yang sama
                Object.DestroyImmediate(m);
                return lama;
            }
        }
    }
}
