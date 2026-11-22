using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Props dari "Props TheSending" (pindahan ke Art/ThirdParty). Nama prefab yang sama dengan prefab Pengganti
    // otomatis menggantikannya (PembuatPengganti.Pasang memuat Prefabs/Asli lebih dulu); logika tidak berubah.
    public static partial class PembuatAsetAsli
    {
        const string Props = Pihak3 + "/";

        static void BuatProps()
        {
            // --- Penggantian visual (Task 2) ---
            // Benda dinding (StopKontak, JamDinding, FotoKeluarga): titik nol di tengah, sama dengan pengganti.
            Prefab("Gapura", r => Kayu(Nyata(Props + "gapura/2Gapura.fbx", r, Vector3.zero, 1, 4f), "BatuPutih"), false);
            // Sumur tanpa "Ground" (alas tanah 9,8 m bawaan model).
            string[] sumur = { "Cube.001", "Cube.002", "Cube.003", "Cube.004", "Cylinder", "Cylinder.001", "Cylinder.002", "Cylinder.004", "Plane.001", "Tali" };
            Prefab("SumurTimba", r => Bahan(Nyata(Props + "well-building-sumur/source/Sumur_Baked.fbx", r, Vector3.zero, 1, 2.4f, hanya: sumur),
                "well-building-sumur", "textures/Diffuse_Bake.jpg"), false);
            Prefab("Sumur", r => Bahan(Nyata(Props + "well-building-sumur/source/Sumur_Baked.fbx", r, Vector3.zero, 1, 2.4f, hanya: sumur),
                "well-building-sumur", "textures/Diffuse_Bake.jpg"), false);
            Prefab("TanggaBambu", r => Bahan(Nyata(Props + "low-poly-ladder/source/ladr.fbx", r, Vector3.zero, 1, 2.5f),
                "low-poly-ladder", "textures/towercolours4.png"), false);
            Prefab("Boneka", r => Bahan(Taruh(Props + "voodoo-doll/source/Doll(stab).fbx", r, Vector3.zero,
                new Vector3(0.3f, 0.12f, 0.4f), 0f), "voodoo-doll", "textures/voodoo_-_base.jpg"), false);
            Prefab("Gembok", r => Bahan(Nyata(Props + "door-padlock-key/source/padlock.fbx", r, Vector3.zero, 1, 0.08f), "door-padlock-key", "textures/padlock_Padlock_BaseColor.png"), false);
            Prefab("StopKontak", r => Bahan(Taruh(Props + "power-outlet-light-switch/source/Untitled 2.fbx", r, new Vector3(0f, -0.05f, 0f),
                new Vector3(0.1f, 0.1f, 0.03f), 0f), "power-outlet-light-switch", "textures/BaseColor.png"), false);
            Prefab("JamDinding", r => Bahan(Nyata(Props + "antique-wall-clock/source/clock.obj", r, new Vector3(0f, -0.165f, 0f), 0, 0.33f), "antique-wall-clock", "textures/clock_low_clock_BaseColor.png"), false);
            Prefab("LemariPakaian", r => Bahan(Nyata(Props + "wooden-wardrobe/source/FurnitureDresser001a_substanceuv.fbx", r, Vector3.zero, 1, 1.85f), "wooden-wardrobe", "textures/FurnitureDresser001a_color.jpg"), false);
            Prefab("Linggis", r => Bahan(Taruh(Props + "tools-asset/source/Tools.fbx", r, Vector3.zero, new Vector3(0.05f, 1.2f, 0.12f), 0f,
                hanya: new[] { "Fomka1" }), "tools-asset", "textures/Fomka1_baseColor.jpg"), false);
            Prefab("Tang", r => Bahan(Taruh(Props + "tools-asset/source/Tools.fbx", r, Vector3.zero, new Vector3(0.22f, 0.03f, 0.08f), 0f,
                hanya: new[] { "Pliers" }), "tools-asset", "textures/PliersMat_baseColor.png"), false);
            Prefab("FotoKeluarga", FotoKeluarga, false);

            // --- Dekor rumah keluarga (Task 3). Kulkas tidak dipakai (D5). ---
            Prefab("TvLed", r => Bahan(Nyata(Props + "led-tv/source/led tv/led tv.fbx", r, Vector3.zero, 0, 0.9f),
                "led-tv", "textures/phong1_Base_Color.jpeg"), false);
            Prefab("RemoteTv", r => Nyata(Props + "props-lepas/tv_remote.glb", r, Vector3.zero, 2, 0.17f), false);   // material glb sudah bertekstur
            Prefab("Dispenser", r => Bahan(Nyata(Props + "water-dispenser/source/Waterdispenser.fbx", r, Vector3.zero, 1, 1.0f), "water-dispenser", "textures/BakedWaterDispenser.png"), true);
            Prefab("KursiPlastik", r => Bahan(Nyata(Props + "plastic-chair/source/Plastic Chair.fbx", r, Vector3.zero, 1, 0.85f), "plastic-chair", null), true);
            // Satu sakelar tunggal (light_switch1_4), skala seragam: pelat 14 cm model -> 8,6 cm. Titik nol di tengah, muka di +Z.
            Prefab("Sakelar", r => Bahan(Nyata(Props + "power-outlet-light-switch/source/Untitled 2.fbx", r, new Vector3(0f, -0.043f, 0f), 0, 0.086f,
                hanya: new[] { "01_4/light_switch1_4" }), "power-outlet-light-switch", "textures/BaseColor.png"), false);
            Prefab("LampuPlafon", r => Bahan(Seragam(Props + "lampu-plafon/CeilingLightRound.fbx", r, Vector3.zero, 0.35f), "lampu-plafon", null), false);
            Prefab("Bohlam", r => Bahan(Seragam(Props + "bohlam/BbLght.obj", r, Vector3.zero, 0.12f), "bohlam", null), false);
            Prefab("Kloset", r => Bahan(Nyata(Props + "toilet/source/Toilet.fbx", r, Vector3.zero, 1, 0.76f),
                "toilet", "textures/Toilet_BaseColor.png"), true);
            Prefab("Wastafel", r => Bahan(Nyata(Props + "sink-set/source/sink2.fbx", r, Vector3.zero, 1, 0.9f, -90f,
                new[] { "sink", "sink_bot_hole", "sink_bolt", "sink_sd_hole", "tap", "tap_handle", "sink_hot_cold" }),
                "sink-set", "textures/Fabric020_1K_Color.jpeg"), false);
            Prefab("AksesorisMandi", r => Bahan(Nyata(Props + "bathroom_accesories/scene.gltf", r, Vector3.zero, 0, 0.45f), "bathroom_accesories", null), false);
            Prefab("MejaMakan", r => Bahan(Nyata(Props + "wooden-kitchen-table-chair/source/Table_and_Chair.fbx", r, Vector3.zero, 1, 0.845f,
                hanya: new[] { "Table", "Chair", "Chair.001", "Chair.002", "Chair.003" }), "wooden-kitchen-table-chair", "textures/Table_BaseColor.png"), true);
            Prefab("TekoTeh", r => Bahan(Nyata(Props + "teapot/scene.gltf", r, Vector3.zero, 1, 0.19f), "teapot", null), false);
            Prefab("DekorDapur", r => Bahan(Nyata(Props + "kitchen-decor-pack/source/KitchenDecorPack.fbx", r, Vector3.zero, 0, 0.9f, hanya: new[] { "CuttingWood", "SpiceJars", "Plate", "TallSpiceBottles", "Bowl" }), "kitchen-decor-pack", null), false);
            Prefab("Karpet", r => Bahan(Nyata(Props + "carpet/source/Carpet_Sketchfab.fbx", r, Vector3.zero, 0, 2f, hanya: new[] { "SM_Carpet_01" }),
                "carpet", "textures/TX_Carpet_0102_albedo.jpg"), false);
            Prefab("Gorden", r =>
            {
                var g = Taruh(Props + "window-curtains/source/curtains-animated-512.glb", r, Vector3.zero, new Vector3(1.2f, 1.6f, 0.1f), 0f);
                foreach (var a in g.GetComponentsInChildren<Animator>(true)) UnityEngine.Object.DestroyImmediate(a);
                foreach (var a in g.GetComponentsInChildren<Animation>(true)) UnityEngine.Object.DestroyImmediate(a);
                Bahan(g, "window-curtains", "textures/A23DTEX_Albedo_1.jpeg");
            }, false);
            Prefab("Lemari", r => Bahan(Nyata(Props + "wooden-cabinet/source/Cabinet_old.fbx", r, Vector3.zero, 0, 1.0f, -90f,
                new[] { "Circle.003", "Circle.004", "Cube.031", "Cube.032", "Cube.035", "Plane.002" }),
                "wooden-cabinet", "textures/Wood_aged_Base_color.png"), true);
            Prefab("Lilin", r => Bahan(Nyata(Props + "candle-low/source/CandleLow.fbx", r, Vector3.zero, 1, 0.15f),
                "candle-low", "textures/Candle_Base_Color.png"), false);
            // Daun pintu saja (tanpa kusen), tinggi 2,05 m; collider dipasang PintuKamar di scene.
            Prefab("DaunPintu", r => Bahan(Nyata(Props + "low-poly-door/source/PortalDoor.fbx", r, Vector3.zero, 1, 2.05f,
                hanya: new[] { "Cube.001", "Cube.002", "Cube.003", "Cube.006", "Cube.007" }),
                "low-poly-door", "textures/Wood027_2K_Color.jpeg"), false);

            Prefab("DaunPintuMandi", r => Bahan(Nyata(Props + "pintu-kamar-mandi/source/Pintu Kamar Mandi.fbx", r, Vector3.zero, 1, 2.05f,
                hanya: new[] { "Cube.003" }), "pintu-kamar-mandi", "textures/pintu kamar mandi.png"), false);

            // --- Dapur cor (dapur-L): kulkas, tabung LPG di rongga meja, kompor di atasnya, pengisi HP di stop kontak. ---
            Prefab("Kulkas", r => Bahan(Nyata(Props + "kulkas-fridge/Fridge.fbx", r, Vector3.zero, 1, 1.5f), "kulkas-fridge", null), true);
            // Tulisan timbul dan katup rinci (sekitar 26k segitiga) dibuang.
            Prefab("TabungGas", r => Bahan(Nyata(Props + "gas-lpg-3kg/source/gasssskeyn.obj", r, Vector3.zero, 1, 0.36f,
                hanya: new[] { "g_Capsule01_Capsule01", "g_ChamferBox03_ChamferBox03", "g_Cylinder31_Cylinder31", "g_Torus10_Torus10", "g_Sphere102_Sphere102" }),
                "gas-lpg-3kg", "textures/wire_088177027_Base_Color.png"), false);
            Prefab("KomporGas", r => Bahan(Nyata(Props + "kompor-gas-rinnai/Kompor-Gas-Rinnai.skp", r, Vector3.zero, 0, 0.7f), "kompor-gas-rinnai", null), false);
            Prefab("PengisiHp", r => Bahan(Nyata(Props + "charger/charge.fbx", r, Vector3.zero, 1, 0.07f,
                hanya: new[] { "Cube", "Cube.001", "Cube.002" }), "charger", null), false);   // kabel spiral (42k segitiga) dibuang

            // --- Scene lain (Task 4) --- Pagar dekor tanpa collider: batas tak terlihat di scene yang membatasi.
            Prefab("PagarKayu", r => Bahan(Nyata(Props + "simple-wood-fence/source/Simple Wood Fence/model/obj/fance.obj", r, Vector3.zero, 0, 2f, 90f),
                "simple-wood-fence", "textures/Diffuse.png"), false);   // model memanjang di Z: diputar supaya segmen 2 m sejajar X lokal
        }

        // Jendela dan pintu kayu vendor untuk rumah Asli (dipanggil saat membangun prefab rumah). Tanpa collider.
        static void JendelaKayu(Transform r, Vector3 alas) => Bahan(Nyata(Props + "wooden-window/source/window_Wooden.fbx", r, alas, 0, 1.16f,
            hanya: new[] { "01" }), "wooden-window", "textures/BaseColor.png");   // FBX berisi dua jendela; satu saja

        static void PintuKayu(Transform r, Vector3 alas) => Bahan(Nyata(Props + "low-poly-door/source/PortalDoor.fbx", r, alas, 1, 2.08f), "low-poly-door", "textures/Wood027_2K_Color.jpeg");

        // Bingkai photo-frame + foto (props-lepas/foto-keluarga.jpg). Titik nol di tengah seperti pengganti (0,6 x 0,45 x 0,04).
        static void FotoKeluarga(Transform r)
        {
            Bahan(Taruh(Props + "photo-frame/Frames.FBX", r, new Vector3(0f, -0.225f, 0f), new Vector3(0.6f, 0.45f, 0.04f), 0f), "photo-frame", null);
            var foto = GameObject.CreatePrimitive(PrimitiveType.Quad);
            UnityEngine.Object.DestroyImmediate(foto.GetComponent<Collider>());
            foto.name = "Foto";
            foto.transform.SetParent(r, false);
            foto.transform.localPosition = new Vector3(0f, 0f, 0.021f);   // sisi +Z; pengganti diputar 180° di pembuat
            foto.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            foto.transform.localScale = new Vector3(0.57f, 0.41f, 1f);   // bingkai vendor tipis
            Bahan(foto, "props-lepas", "foto-keluarga.jpg");
        }

        // Skala SERAGAM (proporsi asli model) sampai sisi `sumbu` (0 x, 1 y, 2 z, sesudah diputar) = `panjang` m; rotasi impor
        // (misalnya FBX sumbu Z ke atas) dipertahankan, lalu diputar `putarY`. Ukuran akhir dicatat ke log "[Ukuran]".
        internal static GameObject Nyata(string jalur, Transform induk, Vector3 alas, int sumbu, float panjang, float putarY = 0f,
            string[] hanya = null)
        {
            var asal = AssetDatabase.LoadAssetAtPath<GameObject>(jalur) ?? throw new FileNotFoundException(jalur);
            var bungkus = new GameObject(asal.name).transform;
            bungkus.SetParent(induk, false);
            var model = (GameObject)PrefabUtility.InstantiatePrefab(asal);
            PrefabUtility.UnpackPrefabInstance(model, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            if (hanya != null) Saring(model.transform, hanya, jalur);
            Bersihkan(model);
            model.transform.SetParent(bungkus, false);
            model.transform.localRotation = Quaternion.Euler(0f, putarY, 0f) * asal.transform.localRotation;
            var b = Batas(bungkus);
            bungkus.localScale = Vector3.one * (panjang / b.size[sumbu]);
            b = Batas(bungkus);
            bungkus.position += induk.TransformPoint(alas) - new Vector3(b.center.x, b.min.y, b.center.z);
            Debug.Log($"[Ukuran] {induk.root.name}/{asal.name}: {b.size.x:F2} x {b.size.y:F2} x {b.size.z:F2} m");
            return bungkus.gameObject;
        }

        // Material asli per slot (URP Lit): tekstur bawaan slot, atau tekstur folder yang namanya diawali nama material, atau
        // `baseColor` cadangan (null = warna impor slot). Normal, metallic, dan AO dicari di folder yang sama dengan awalan
        // tekstur warna. Material bawaan FBX hanya-baca: setiap kombinasi disimpan sebagai .mat di folder props.
        static void Bahan(GameObject g, string folder, string baseColor, string warnaHex = "FFFFFF")
        {
            string akar = Props + folder;
            Texture2D cadangan = null;
            if (baseColor != null)
                cadangan = AssetDatabase.LoadAssetAtPath<Texture2D>(akar + "/" + baseColor) ?? throw new FileNotFoundException(akar + "/" + baseColor);
            var semua = AssetDatabase.FindAssets("t:Texture2D", new[] { akar }).Select(AssetDatabase.GUIDToAssetPath).ToArray();
            foreach (var r in g.GetComponentsInChildren<Renderer>())
                r.sharedMaterials = r.sharedMaterials.Select(m => MaterialAsli(m, akar, semua, cadangan, warnaHex)).ToArray();
        }

        static readonly string[] KataWarna = { "basecolor", "base_color", "albedo", "diffuse", "color", "col" };

        static Material MaterialAsli(Material asal, string akar, string[] semua, Texture2D cadangan, string warnaHex)
        {
            string namaAsal = asal != null ? asal.name : "Default";
            var tex = asal != null ? asal.mainTexture as Texture2D : null;
            if (tex == null)
            {
                string kunci = Bersih(namaAsal);
                string cocok = kunci.Length < 2 ? null : semua.FirstOrDefault(j => Bersih(Path.GetFileNameWithoutExtension(j)).StartsWith(kunci)
                    && KataWarna.Any(k => Path.GetFileNameWithoutExtension(j).ToLower().Contains(k)));
                tex = cocok != null ? AssetDatabase.LoadAssetAtPath<Texture2D>(cocok) : cadangan;
            }
            Color warna = PembuatRig.Hex(warnaHex);
            if (tex == null && asal != null)
                warna = asal.HasProperty("_BaseColor") ? asal.GetColor("_BaseColor") : asal.HasProperty("_Color") ? asal.GetColor("_Color") : Color.gray;
            string id = tex != null ? Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(tex)) : "Warna_" + Bersih(namaAsal);
            if (warnaHex != "FFFFFF") id += "_" + warnaHex;
            string jalurMat = $"{akar}/Bahan_{id}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(jalurMat);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, jalurMat);
            }
            mat.SetColor("_BaseColor", warna);
            mat.SetFloat("_Smoothness", 0f);
            mat.SetTexture("_BaseMap", tex);
            if (tex != null)
            {
                string jalurTex = AssetDatabase.GetAssetPath(tex);
                SetelTeksturProps(jalurTex, false, true);
                string awalan = Awalan(Path.GetFileNameWithoutExtension(jalurTex));
                string dir = Path.GetDirectoryName(jalurTex).Replace('\\', '/');
                Peta(mat, Cari(semua, dir, awalan, "normal"), "_BumpMap", "_NORMALMAP", true);
                Peta(mat, Cari(semua, dir, awalan, "metal"), "_MetallicGlossMap", "_METALLICSPECGLOSSMAP", false);
                Peta(mat, Cari(semua, dir, awalan, "ao", "occlusion"), "_OcclusionMap", "_OCCLUSIONMAP", false);
            }
            EditorUtility.SetDirty(mat);
            return mat;
        }

        static void Peta(Material mat, string jalur, string slot, string keyword, bool normal)
        {
            if (jalur == null) { mat.SetTexture(slot, null); mat.DisableKeyword(keyword); return; }
            SetelTeksturProps(jalur, normal, false);
            mat.SetTexture(slot, AssetDatabase.LoadAssetAtPath<Texture2D>(jalur));
            mat.EnableKeyword(keyword);
        }

        // Peta pendamping di folder yang sama: awalan sama dan nama memuat salah satu kata (mis. "_Normal", "_AO").
        static string Cari(string[] semua, string dir, string awalan, params string[] kata) => semua.FirstOrDefault(j =>
        {
            if (Path.GetDirectoryName(j).Replace('\\', '/') != dir) return false;
            string n = Path.GetFileNameWithoutExtension(j);
            if (!Bersih(n).StartsWith(awalan)) return false;
            string sisa = n.ToLower().Substring(Math.Min(n.Length, awalan.Length));
            return kata.Any(k => k == "ao" ? System.Text.RegularExpressions.Regex.IsMatch(sisa, @"(^|[_\s.-])(mixed_)?ao($|[_\s.-])") : sisa.Contains(k))
                && !(kata[0] == "normal" && sisa.Contains("normaldx") && semua.Any(o => o.ToLower().Contains(awalan) && o.ToLower().Contains("normalgl")));
        });

        static string Bersih(string s) => new string(s.ToLower().Where(char.IsLetterOrDigit).ToArray());

        // "phong1_Base_Color" -> "phong1"; "Wood027_2K_Color" -> "wood0272k".
        static string Awalan(string namaTex)
        {
            string n = namaTex.ToLower();
            foreach (var k in KataWarna)
            {
                int i = n.LastIndexOf(k);
                if (i > 0) return Bersih(namaTex.Substring(0, i));
            }
            return Bersih(namaTex);
        }

        // Tekstur props tidak ikut turun ke 256 px PSX: resolusi asli dibatasi 1024 px, filter bilinear (efek PSX layar tetap).
        internal const int UkuranProps = 1024;

        internal static bool TeksturProps(string jalur) => FolderProps.Any(f => jalur.StartsWith(Props + f + "/"));

        static readonly string[] FolderProps =
        {
            "gapura", "well-building-sumur", "low-poly-ladder", "voodoo-doll", "door-padlock-key", "power-outlet-light-switch",
            "antique-wall-clock", "wooden-wardrobe", "tools-asset", "led-tv", "water-dispenser", "plastic-chair", "lampu-plafon",
            "bohlam", "toilet", "sink-set", "bathroom_accesories", "wooden-kitchen-table-chair", "teapot", "kitchen-decor-pack",
            "carpet", "window-curtains", "wooden-cabinet", "candle-low", "low-poly-door", "pintu-kamar-mandi", "kulkas-fridge",
            "gas-lpg-3kg", "kompor-gas-rinnai", "charger", "simple-wood-fence", "wooden-window", "photo-frame",
        };

        static void SetelTeksturProps(string jalur, bool normal, bool srgb)
        {
            if (!TeksturProps(jalur) || AssetImporter.GetAtPath(jalur) is not TextureImporter imp) return;
            var tipe = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            bool srgbAkhir = !normal && srgb;
            if (imp.maxTextureSize == UkuranProps && imp.filterMode == FilterMode.Bilinear && imp.mipmapEnabled
                && imp.textureType == tipe && imp.sRGBTexture == srgbAkhir) return;
            imp.maxTextureSize = UkuranProps;
            imp.filterMode = FilterMode.Bilinear;
            imp.mipmapEnabled = true;
            imp.textureType = tipe;
            imp.sRGBTexture = srgbAkhir;
            imp.SaveAndReimport();
        }

        // Jumlah segitiga per prefab Asli, untuk mencatat model berat (Global Constraints).
        static void LaporSegitiga()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { FolderPrefab }))
            {
                var p = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                int n = 0;
                foreach (var mf in p.GetComponentsInChildren<MeshFilter>(true))
                    if (mf.sharedMesh != null) n += mf.sharedMesh.triangles.Length / 3;
                foreach (var sm in p.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    if (sm.sharedMesh != null) n += sm.sharedMesh.triangles.Length / 3;
                Debug.Log($"[Segitiga] {p.name}: {n}");
            }
        }
    }
}
