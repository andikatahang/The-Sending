using System.IO;
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
            Prefab("Gembok", r => Bahan(Taruh(Props + "door-padlock-key/source/padlock.fbx", r, Vector3.zero,
                new Vector3(0.08f, 0.12f, 0.04f), 0f), "door-padlock-key", "textures/padlock_Padlock_BaseColor.png"), false);
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
            Prefab("RemoteTv", r => Nyata(Props + "props-lepas/tv_remote.glb", r, Vector3.zero, 2, 0.17f), false);
            Prefab("Dispenser", r => Bahan(Nyata(Props + "water-dispenser/source/Waterdispenser.fbx", r, Vector3.zero, 1, 1.0f), "water-dispenser", "textures/BakedWaterDispenser.png"), true);
            Prefab("KursiPlastik", r => Nyata(Props + "plastic-chair/source/Plastic Chair.fbx", r, Vector3.zero, 1, 0.85f), true);
            Prefab("Sakelar", r => Bahan(Taruh(Props + "power-outlet-light-switch/source/Untitled 2.fbx", r, Vector3.zero,
                new Vector3(0.08f, 0.12f, 0.03f), 0f), "power-outlet-light-switch", "textures/BaseColor.png"), false);
            Prefab("LampuPlafon", r => Kayu(Seragam(Props + "lampu-plafon/CeilingLightRound.fbx", r, Vector3.zero, 0.35f), "Kertas"), false);
            Prefab("Bohlam", r => Kayu(Seragam(Props + "bohlam/BbLght.obj", r, Vector3.zero, 0.12f), "Kertas"), false);
            Prefab("Kloset", r => Bahan(Nyata(Props + "toilet/source/Toilet.fbx", r, Vector3.zero, 1, 0.76f),
                "toilet", "textures/Toilet_BaseColor.png"), true);
            Prefab("Wastafel", r => Bahan(Nyata(Props + "sink-set/source/sink2.fbx", r, Vector3.zero, 1, 0.9f, -90f,
                new[] { "sink", "sink_bot_hole", "sink_bolt", "sink_sd_hole", "tap", "tap_handle", "sink_hot_cold" }),
                "sink-set", "textures/Fabric020_1K_Color.jpeg"), false);
            Prefab("AksesorisMandi", r => Nyata(Props + "bathroom_accesories/scene.gltf", r, Vector3.zero, 0, 0.45f), false);
            Prefab("MejaMakan", r => Bahan(Nyata(Props + "wooden-kitchen-table-chair/source/Table_and_Chair.fbx", r, Vector3.zero, 1, 0.845f,
                hanya: new[] { "Table", "Chair", "Chair.001", "Chair.002", "Chair.003" }), "wooden-kitchen-table-chair", "textures/Table_BaseColor.png"), true);
            Prefab("TekoTeh", r => Nyata(Props + "teapot/scene.gltf", r, Vector3.zero, 1, 0.15f), false);
            Prefab("DekorDapur", r => Nyata(Props + "kitchen-decor-pack/source/KitchenDecorPack.fbx", r, Vector3.zero, 0, 0.9f, hanya: new[] { "CuttingWood", "SpiceJars", "Plate", "TallSpiceBottles", "Bowl" }), false);
            Prefab("Karpet", r => Bahan(Nyata(Props + "carpet/source/Carpet_Sketchfab.fbx", r, Vector3.zero, 0, 2f, hanya: new[] { "SM_Carpet_01" }),
                "carpet", "textures/TX_Carpet_0102_albedo.jpg"), false);
            Prefab("Gorden", r =>
            {
                var g = Taruh(Props + "window-curtains/source/curtains-animated-512.glb", r, Vector3.zero, new Vector3(1.2f, 1.6f, 0.1f), 0f);
                foreach (var a in g.GetComponentsInChildren<Animator>(true)) Object.DestroyImmediate(a);
                foreach (var a in g.GetComponentsInChildren<Animation>(true)) Object.DestroyImmediate(a);
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
            Prefab("Kulkas", r => Kayu(Nyata(Props + "kulkas-fridge/Fridge.fbx", r, Vector3.zero, 1, 1.5f), "Kertas"), true);
            // Tulisan timbul dan katup rinci (sekitar 26k segitiga) dibuang.
            Prefab("TabungGas", r => Bahan(Nyata(Props + "gas-lpg-3kg/source/gasssskeyn.obj", r, Vector3.zero, 1, 0.36f,
                hanya: new[] { "g_Capsule01_Capsule01", "g_ChamferBox03_ChamferBox03", "g_Cylinder31_Cylinder31", "g_Torus10_Torus10", "g_Sphere102_Sphere102" }),
                "gas-lpg-3kg", "textures/wire_088177027_Base_Color.png"), false);
            Prefab("KomporGas", r => Kayu(Nyata(Props + "kompor-gas-rinnai/Kompor-Gas-Rinnai.skp", r, Vector3.zero, 0, 0.7f), "Logam"), false);
            Prefab("PengisiHp", r => Kayu(Nyata(Props + "charger/charge.fbx", r, Vector3.zero, 1, 0.07f,
                hanya: new[] { "Cube", "Cube.001", "Cube.002" }), "Kertas"), false);   // kabel spiral (42k segitiga) dibuang

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
            Kayu(Taruh(Props + "photo-frame/Frames.FBX", r, new Vector3(0f, -0.225f, 0f), new Vector3(0.6f, 0.45f, 0.04f), 0f), "KayuGelap");
            var foto = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(foto.GetComponent<Collider>());
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

        // Material URP Lit dari satu tekstur warna; dipasang ke semua renderer (material bawaan FBX hanya-baca).
        static void Bahan(GameObject g, string folder, string baseColor, string warnaHex = "FFFFFF")
        {
            string jalurTex = Props + folder + "/" + baseColor;
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(jalurTex) ?? throw new FileNotFoundException(jalurTex);
            string jalurMat = $"{Props}{folder}/Bahan_{Path.GetFileNameWithoutExtension(baseColor)}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(jalurMat);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, jalurMat);
            }
            mat.SetTexture("_BaseMap", tex);
            mat.SetColor("_BaseColor", PembuatRig.Hex(warnaHex));
            mat.SetFloat("_Smoothness", 0f);
            EditorUtility.SetDirty(mat);
            foreach (var r in g.GetComponentsInChildren<Renderer>())
                r.sharedMaterials = System.Array.ConvertAll(r.sharedMaterials, _ => mat);
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
