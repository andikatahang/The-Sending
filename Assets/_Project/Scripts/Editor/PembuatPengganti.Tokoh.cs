using System;
using System.IO;
using System.Linq;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheSending.EditorTools
{
    // Tokoh manusia: model low-poly PSX (JashiPSX, Art/ThirdParty/tokoh-dasar) dengan tekstur per tokoh yang dilukis skrip
    // di atas tata letak UV character_256.png, plus kotak low-poly di kepala (kerudung, sanggul, peci, blangkon).
    // Prefab tokoh SELALU dibuat ulang (milik skrip ini, bukan aset tukaran tim). Tekstur dan material hanya dibuat jika
    // belum ada: hapus file di Art/Textures/Tokoh untuk melukis ulang setelah mengubah gaya di bawah.
    public static partial class PembuatPengganti
    {
        const string FolderDasar = "Assets/_Project/Art/ThirdParty/tokoh-dasar";
        const string JalurModel = FolderDasar + "/Simple_Character_PSX/model/fbx/simple-character-psx.fbx";
        const string JalurTeksturDasar = FolderDasar + "/Simple_Character_PSX/texture/character_256.png";
        const string JalurDiam = FolderDasar + "/Animasi-Idle.fbx", JalurJalan = FolderDasar + "/Animasi-walking.fbx";
        const string FolderTeksturTokoh = "Assets/_Project/Art/Textures/Tokoh", FolderMaterialTokoh = "Assets/_Project/Art/Materials/Tokoh";
        const string JalurKontroler = "Assets/_Project/Art/Animasi/Tokoh.controller";
        const float SkalaModel = 0.81f;   // tinggi tokoh sekitar 1,65 m saat animasi diam
        const string TulangKepala = "spine.005";

        enum Pola { Polos, Batik, Bunga, Kotak }
        enum Pose { Berdiri, Berdoa, Berbaring, Siluet }

        sealed class Tokoh
        {
            public string Nama;
            public string Baju, Bawah, Rambut, Kulit = "C69C7B", Sepatu = "3A3028", Aksen = "D9C08A";
            public Pola PolaBaju, PolaBawah;
            public bool Wanita, Tua, Pucat, Kacamata, Siluet, Obor;   // Obor = prefab Obor di tangan kanan
            public Pose Pose;
            public Bagian[] Kepala = new Bagian[0];   // di koordinat prefab (meter), ditempel ke tulang kepala
        }

        // Aksesori kepala: posisi relatif terhadap tengah mesh kepala (tinggi kepala sekitar 0,3 m, lebar 0,19, dalam 0,22).
        static Bagian RambutPanjang(string m) => K(m, 0f, -0.2f, -0.1f, 0.17f, 0.5f, 0.02f, -6f);   // terurai sampai punggung
        static Bagian Peci() => K("Gelap", 0f, 0.12f, 0.005f, 0.19f, 0.07f, 0.21f);

        static readonly Tokoh[] DaftarTokoh =
        {
            new Tokoh { Nama = "Laras", Baju = "7A4650", Bawah = "2E2A36", Rambut = "141010", Wanita = true, Kepala = new[] { RambutPanjang("Gelap") } },
            new Tokoh { Nama = "Laras_Berdoa", Baju = "7A4650", Bawah = "2E2A36", Rambut = "141010", Wanita = true, Kepala = new[] { RambutPanjang("Gelap") }, Pose = Pose.Berdoa },
            new Tokoh { Nama = "BidanWulan", Baju = "E6E4DC", Bawah = "D4D2C8", Rambut = "141010", Sepatu = "DAD6CC", Wanita = true, Kepala = new[] { RambutPanjang("Gelap") } },
            new Tokoh { Nama = "BuRatmi_Berbaring", Baju = "4E5A78", Bawah = "4E5A78", Aksen = "C8B88A", PolaBaju = Pola.Bunga, PolaBawah = Pola.Bunga,
                Rambut = "3A3633", Kulit = "C8AE98", Wanita = true, Tua = true, Pucat = true, Kepala = new[] { RambutPanjang("Gelap") }, Pose = Pose.Berbaring },
            new Tokoh { Nama = "BuSumi", Baju = "8A7E68", Bawah = "5A3A24", Aksen = "B89A6A", PolaBawah = Pola.Batik, Rambut = "E6E2DA", Kulit = "B08A6C",
                Wanita = true, Tua = true, Kepala = new[] { RambutPanjang("Kertas") } },
            new Tokoh { Nama = "BuSumi_Siluet", Siluet = true, Pose = Pose.Siluet, Kepala = new[] { RambutPanjang("Gelap") } },
            new Tokoh { Nama = "PakdeDarmo", Baju = "6B3A26", Bawah = "2E2E30", Rambut = "3C3C3C", PolaBaju = Pola.Batik, Kacamata = true },
            new Tokoh { Nama = "MbahWiryo", Baju = "1E1E24", Bawah = "3A2A1E", Aksen = "8A6A42", PolaBawah = Pola.Batik, Rambut = "8A8680", Kulit = "A07858", Tua = true,
                Kepala = new[] { K("Batik", 0f, 0.11f, 0.005f, 0.2f, 0.08f, 0.22f), K("Batik", 0f, 0.05f, -0.115f, 0.09f, 0.07f, 0.04f) } },
            new Tokoh { Nama = "PakJoko", Baju = "5E6A52", Bawah = "2A2A2C", Rambut = "1E1814", Kepala = new[] { Peci() } },
            new Tokoh { Nama = "PakJoko_Obor", Baju = "5E6A52", Bawah = "2A2A2C", Rambut = "1E1814", Kepala = new[] { Peci() }, Obor = true },
            new Tokoh { Nama = "Warga1", Baju = "5A3420", Bawah = "18181A", Rambut = "1E1814", PolaBaju = Pola.Batik },
            new Tokoh { Nama = "Warga2", Baju = "A8A290", Bawah = "3E4E34", Aksen = "6E2A22", PolaBawah = Pola.Kotak, Rambut = "1E1814", Sepatu = "3A2A1E" },
            new Tokoh { Nama = "Warga3", Baju = "3E4A5E", Bawah = "2A2A2C", Rambut = "1E1814", PolaBaju = Pola.Kotak, Kepala = new[] { Peci() } },
            new Tokoh { Nama = "Warga4", Baju = "7A5A30", Bawah = "3A2A1E", Aksen = "C8B88A", PolaBawah = Pola.Batik, Rambut = "2A2622", Kulit = "A87E5C", Tua = true },
            new Tokoh { Nama = "Warga5", Baju = "6E2A3A", Bawah = "3A2E36", PolaBawah = Pola.Batik, Rambut = "141010", Wanita = true, Kepala = new[] { RambutPanjang("Gelap") } },
            new Tokoh { Nama = "PakLasno", Baju = "6E6048", Bawah = "4A3A2A", Rambut = "2A2622", Kulit = "A87E5C", Tua = true, Kepala = new[] { Peci() } },
        };

        static void BuatTokoh()
        {
            foreach (var f in new[] { FolderTeksturTokoh, FolderMaterialTokoh, Path.GetDirectoryName(JalurKontroler) })
                Directory.CreateDirectory(f);
            SiapkanImpor();
            var kontroler = Kontroler();
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(JalurModel)
                ?? throw new FileNotFoundException($"Model tokoh tidak ada: {JalurModel}");
            foreach (var t in DaftarTokoh) BuatPrefabTokoh(t, model, kontroler);
        }

        // Rig Humanoid untuk model (pemetaan manual: nama tulang gaya Rigify) dan dua animasi Mixamo (berulang, di tempat).
        static readonly (string manusia, string tulang)[] PetaTulang =
        {
            ("Hips", "spine"), ("Spine", "spine.001"), ("Chest", "spine.002"), ("UpperChest", "spine.003"), ("Neck", "spine.004"), ("Head", TulangKepala),
            ("LeftShoulder", "shoulder.L"), ("LeftUpperArm", "upper_arm.L"), ("LeftLowerArm", "forearm.L"), ("LeftHand", "hand.L"),
            ("RightShoulder", "shoulder.R"), ("RightUpperArm", "upper_arm.R"), ("RightLowerArm", "forearm.R"), ("RightHand", "hand.R"),
            ("LeftUpperLeg", "thigh.L"), ("LeftLowerLeg", "shin.L"), ("LeftFoot", "foot.L"), ("LeftToes", "toe.L"),
            ("RightUpperLeg", "thigh.R"), ("RightLowerLeg", "shin.R"), ("RightFoot", "foot.R"), ("RightToes", "toe.R"),
        };

        static void SiapkanImpor()
        {
            var imp = (ModelImporter)AssetImporter.GetAtPath(JalurModel);
            if (imp.animationType != ModelImporterAnimationType.Human || imp.humanDescription.human.Length != PetaTulang.Length)
            {
                imp.materialImportMode = ModelImporterMaterialImportMode.None;
                imp.animationType = ModelImporterAnimationType.Human;
                imp.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                imp.SaveAndReimport();   // impor pertama sebagai Human mengisi daftar kerangka
                var hd = imp.humanDescription;
                hd.human = PetaTulang.Select(p => new HumanBone { humanName = p.manusia, boneName = p.tulang, limit = new HumanLimit { useDefaultValues = true } }).ToArray();
                imp.humanDescription = hd;
                imp.SaveAndReimport();
            }
            var avatar = AssetDatabase.LoadAllAssetsAtPath(JalurModel).OfType<Avatar>().FirstOrDefault();
            if (avatar == null || !avatar.isHuman) Debug.LogError($"Avatar Humanoid tokoh gagal dibuat: {JalurModel}");

            SiapkanAnimasi(JalurDiam, "Diam");
            SiapkanAnimasi(JalurJalan, "Jalan");
        }

        static void SiapkanAnimasi(string jalur, string nama)
        {
            var imp = (ModelImporter)AssetImporter.GetAtPath(jalur);
            if (imp.animationType == ModelImporterAnimationType.Human && imp.clipAnimations.Length == 1 && imp.clipAnimations[0].name == nama) return;
            imp.materialImportMode = ModelImporterMaterialImportMode.None;
            imp.animationType = ModelImporterAnimationType.Human;
            imp.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            imp.SaveAndReimport();
            var klip = imp.defaultClipAnimations[0];
            klip.name = nama;
            klip.loopTime = true;
            klip.lockRootRotation = klip.lockRootHeightY = klip.lockRootPositionXZ = true;   // di tempat: posisi diatur scene
            klip.keepOriginalOrientation = klip.keepOriginalPositionY = klip.keepOriginalPositionXZ = true;
            imp.clipAnimations = new[] { klip };
            imp.SaveAndReimport();
        }

        static AnimationClip Klip(string jalur) =>
            AssetDatabase.LoadAllAssetsAtPath(jalur).OfType<AnimationClip>().First(k => !k.name.StartsWith("__preview__"));

        static AnimatorController Kontroler()
        {
            var ada = AssetDatabase.LoadAssetAtPath<AnimatorController>(JalurKontroler);
            if (ada != null) return ada;
            var k = AnimatorController.CreateAnimatorControllerAtPath(JalurKontroler);
            k.AddParameter(GerakTokoh.ParameterJalan, AnimatorControllerParameterType.Bool);
            var mesin = k.layers[0].stateMachine;
            var diam = mesin.AddState("Diam");
            diam.motion = Klip(JalurDiam);
            var jalan = mesin.AddState("Jalan");
            jalan.motion = Klip(JalurJalan);
            mesin.defaultState = diam;
            Transisi(diam, jalan, AnimatorConditionMode.If);
            Transisi(jalan, diam, AnimatorConditionMode.IfNot);
            AssetDatabase.SaveAssets();
            return k;
        }

        static void Transisi(AnimatorState dari, AnimatorState ke, AnimatorConditionMode syarat)
        {
            var t = dari.AddTransition(ke);
            t.hasExitTime = false;
            t.duration = 0.2f;
            t.AddCondition(syarat, 0f, GerakTokoh.ParameterJalan);
        }

        static void BuatPrefabTokoh(Tokoh t, GameObject sumber, RuntimeAnimatorController kontroler)
        {
            var akar = new GameObject(t.Nama);
            var model = Object.Instantiate(sumber, akar.transform);
            model.name = "Model";
            model.transform.localScale = Vector3.one * SkalaModel;

            var mat = MaterialTokoh(t);
            foreach (var r in model.GetComponentsInChildren<SkinnedMeshRenderer>())
                // Hanya Pakde berkacamata: tanpa material kedua, submesh lensa di kepala tidak digambar.
            {
                if (r.name == "head" && !t.Kacamata) r.sharedMesh = KepalaTanpaLensa(r.sharedMesh);   // buang submesh lensa
                r.sharedMaterials = r.name == "head" && t.Kacamata ? new[] { mat, Material("Logam") } : new[] { mat };
            }

            var kepala = Tulang(model, TulangKepala);
            var tengahKepala = TengahKepala(model);
            foreach (var b in t.Kepala)
            {
                var g = GameObject.CreatePrimitive(b.Bentuk);
                Object.DestroyImmediate(g.GetComponent<Collider>());
                g.name = b.Material;
                g.transform.SetPositionAndRotation(tengahKepala + b.Posisi, Quaternion.Euler(b.Putar));
                g.transform.localScale = b.Ukuran;
                g.transform.SetParent(kepala, true);
                g.GetComponent<Renderer>().sharedMaterial = t.Siluet ? mat : Material(b.Material);
            }

            if (t.Obor) Pasang("Obor", Tulang(model, "hand.R"), Vector3.zero);

            var animator = model.GetComponent<Animator>();
            if (animator == null) animator = model.AddComponent<Animator>();
            if (t.Pose == Pose.Berdiri)
            {
                animator.runtimeAnimatorController = kontroler;
                animator.applyRootMotion = false;
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;   // cutscene Timeline berjalan saat timeScale 0
                animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                akar.AddComponent<GerakTokoh>();
                var kapsul = akar.AddComponent<CapsuleCollider>();   // pengganti kotak lama: menghalangi pemain dan NavMesh
                kapsul.center = new Vector3(0f, 0.83f, 0f);
                kapsul.radius = 0.22f;
                kapsul.height = 1.66f;
            }
            else
            {
                Object.DestroyImmediate(animator);   // pose statis: tulang diputar di sini, tanpa Animator
                if (t.Pose == Pose.Berdoa) PoseBerdoa(model);
                else if (t.Pose == Pose.Berbaring) PoseBerbaring(model, akar);
                else PoseSiluet(model, akar);
            }

            PrefabUtility.SaveAsPrefabAsset(akar, $"{FolderPrefab}/{t.Nama}.prefab");
            Object.DestroyImmediate(akar);
        }

        // Siluet di jendela (akar dipipihkan PembuatDesa): tanpa Animator, karena animasi Humanoid di induk berskala
        // tidak seragam menggeser tubuh ke atas. Lengan turun, kaki di y 0 seperti prefab lama.
        static void PoseSiluet(GameObject model, GameObject akar)
        {
            Lengan(model, new Vector3(-0.15f, -1f, 0f), new Vector3(-0.05f, -1f, 0.05f));
            var kapsul = akar.AddComponent<CapsuleCollider>();
            kapsul.center = new Vector3(0f, 0.83f, 0f);
            kapsul.radius = 0.22f;
            kapsul.height = 1.66f;
        }

        const string JalurKepalaTanpaLensa = "Assets/_Project/Art/Models/Tokoh/KepalaTanpaLensa.asset";

        static Mesh KepalaTanpaLensa(Mesh asal)
        {
            var ada = AssetDatabase.LoadAssetAtPath<Mesh>(JalurKepalaTanpaLensa);
            if (ada != null) return ada;
            Directory.CreateDirectory(Path.GetDirectoryName(JalurKepalaTanpaLensa));
            var m = Object.Instantiate(asal);
            m.subMeshCount = 1;
            AssetDatabase.CreateAsset(m, JalurKepalaTanpaLensa);
            return m;
        }

        static Vector3 TengahKepala(GameObject model)
        {
            var r = model.GetComponentsInChildren<SkinnedMeshRenderer>().First(x => x.name == "head");
            var m = new Mesh();
            r.BakeMesh(m, true);
            var b = GeometryUtility.CalculateBounds(m.vertices, r.transform.localToWorldMatrix);
            Object.DestroyImmediate(m);
            return b.center;
        }

        internal static Transform Tulang(GameObject model, string nama) =>
            model.GetComponentsInChildren<Transform>().FirstOrDefault(x => x.name == nama)
            ?? throw new InvalidOperationException($"Tulang '{nama}' tidak ada di model tokoh.");

        // Memutar tulang (arah ke tulang anak) supaya menunjuk arah dunia. Model menghadap +Z, kiri tokoh = -X.
        static void Arahkan(GameObject model, string tulang, string anak, Vector3 arah)
        {
            var a = Tulang(model, tulang);
            a.rotation = Quaternion.FromToRotation(Tulang(model, anak).position - a.position, arah) * a.rotation;
        }

        static void Lengan(GameObject model, Vector3 atas, Vector3 bawah)
        {
            Arahkan(model, "upper_arm.L", "forearm.L", atas);
            Arahkan(model, "forearm.L", "hand.L", bawah);
            Arahkan(model, "upper_arm.R", "forearm.R", new Vector3(-atas.x, atas.y, atas.z));
            Arahkan(model, "forearm.R", "hand.R", new Vector3(-bawah.x, bawah.y, bawah.z));
        }

        // Bersimpuh (duduk di atas tumit) dengan tangan menangkup di depan dada, kepala menunduk. Menghadap +Z.
        static void PoseBerdoa(GameObject model)
        {
            Lengan(model, new Vector3(-0.25f, -1f, 0.35f), new Vector3(0.75f, 0.55f, 0.6f));
            foreach (var s in new[] { "L", "R" })
            {
                Arahkan(model, "thigh." + s, "shin." + s, new Vector3(0f, -0.3f, 1f));
                Arahkan(model, "shin." + s, "foot." + s, new Vector3(0f, -0.15f, -1f));
                Arahkan(model, "foot." + s, "toe." + s, new Vector3(0f, -0.2f, -1f));
            }
            Arahkan(model, "spine.004", TulangKepala, new Vector3(0f, 1f, 0.45f));
            float terendah = new[] { "shin.L", "shin.R", "foot.L", "foot.R", "toe.L", "toe.R" }.Min(n => Tulang(model, n).position.y);
            model.transform.position += Vector3.up * (0.06f - terendah);
            var kotak = model.transform.parent.gameObject.AddComponent<BoxCollider>();
            kotak.center = new Vector3(0f, 0.5f, 0f);
            kotak.size = new Vector3(0.45f, 1f, 0.7f);
        }

        // Telentang di kasur (akar di permukaan kasur), kepala ke -Z.
        static void PoseBerbaring(GameObject model, GameObject akar)
        {
            Lengan(model, new Vector3(-0.15f, -1f, 0f), new Vector3(-0.05f, -1f, 0.1f));
            model.transform.SetPositionAndRotation(new Vector3(0f, 0.12f, 0.79f), Quaternion.Euler(-90f, 0f, 0f));
            var kotak = akar.AddComponent<BoxCollider>();
            kotak.center = new Vector3(0f, 0.13f, 0f);
            kotak.size = new Vector3(0.5f, 0.26f, 1.75f);
        }

        static Material MaterialTokoh(Tokoh t)
        {
            string jalurTeks = $"{FolderTeksturTokoh}/{t.Nama}.png", jalurMat = $"{FolderMaterialTokoh}/{t.Nama}.mat";
            if (!File.Exists(jalurTeks))
            {
                File.WriteAllBytes(jalurTeks, Lukis(t).EncodeToPNG());
                AssetDatabase.ImportAsset(jalurTeks);   // PrasetelTeksturPsx: point, tanpa mipmap, 256
            }
            var ada = AssetDatabase.LoadAssetAtPath<Material>(jalurMat);
            if (ada != null) return ada;
            if (t.Siluet)   // hitam rata: Unlit, tidak diterangi lampu jendela
            {
                var hitam = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                hitam.SetColor("_BaseColor", Color.black);
                AssetDatabase.CreateAsset(hitam, jalurMat);
                return hitam;
            }
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(jalurTeks));
            mat.SetFloat("_Smoothness", 0f);
            AssetDatabase.CreateAsset(mat, jalurMat);
            return mat;
        }
    }
}
