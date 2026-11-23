using System.IO;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheSending.EditorTools
{
    // Malam 6: obor, api besar, dan asap. Seperti prefab pengganti lain, file yang sudah ada tidak ditimpa.
    public static partial class PembuatPengganti
    {
        static readonly Color Jingga = new Color(1f, 0.55f, 0.2f);

        static void BuatApi()
        {
            SimpanBaru("Obor", akar =>
            {
                BagianPrimitif(akar, PrimitiveType.Cylinder, "Bambu", new Vector3(0f, 0.45f, 0f), new Vector3(0.04f, 0.45f, 0.04f));
                BagianPrimitif(akar, PrimitiveType.Sphere, "KainObor", new Vector3(0f, 0.95f, 0f), new Vector3(0.1f, 0.14f, 0.1f));
                Lampu(akar, new Vector3(0f, 1.05f, 0f), 6f, 1.5f);
            });
            SimpanBaru("ApiBesar", akar =>
            {
                var ps = Partikel(akar, MaterialPartikel("PartikelApi", true), 40, 1.2f);
                var m = ps.main;
                m.startSize = new ParticleSystem.MinMaxCurve(0.6f, 1.2f);
                m.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2.5f);
                var sh = ps.shape;
                sh.shapeType = ParticleSystemShapeType.Box;
                sh.scale = new Vector3(3f, 0.5f, 3f);
                var warna = ps.colorOverLifetime;
                warna.enabled = true;
                var g = new Gradient();
                g.SetKeys(new[] { new GradientColorKey(Jingga, 0f), new GradientColorKey(new Color(0.35f, 0.05f, 0.02f), 1f) },
                          new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
                warna.color = g;
                Lampu(akar, new Vector3(0f, 1.5f, 0f), 14f, 1.5f);
            });
            SimpanBaru("AsapHitam", akar =>
            {
                var ps = Partikel(akar, MaterialPartikel("PartikelAsap", false), 20, 4f);
                var m = ps.main;
                m.startSize = new ParticleSystem.MinMaxCurve(1f, 2f);
                m.startSpeed = 1.5f;
                m.startColor = new Color(0.12f, 0.12f, 0.12f, 0.6f);
            });
        }

        static void SimpanBaru(string nama, System.Action<GameObject> isi)
        {
            string jalur = $"{FolderPrefab}/{nama}.prefab";
            if (File.Exists(jalur)) return;
            var akar = new GameObject(nama);
            isi(akar);
            PrefabUtility.SaveAsPrefabAsset(akar, jalur);
            Object.DestroyImmediate(akar);
        }

        static void BagianPrimitif(GameObject akar, PrimitiveType bentuk, string mat, Vector3 pos, Vector3 ukuran)
        {
            var g = GameObject.CreatePrimitive(bentuk);
            Object.DestroyImmediate(g.GetComponent<Collider>());
            g.name = mat;
            g.transform.SetParent(akar.transform, false);
            g.transform.localPosition = pos;
            g.transform.localScale = ukuran;
            g.GetComponent<Renderer>().sharedMaterial = Material(mat);
        }

        static void Lampu(GameObject akar, Vector3 pos, float jangkauan, float intensitas)
        {
            var g = new GameObject("Cahaya");
            g.transform.SetParent(akar.transform, false);
            g.transform.localPosition = pos;
            var l = g.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = Jingga;
            l.range = jangkauan;
            l.intensity = intensitas;
            var so = new SerializedObject(akar.AddComponent<ApiBerkedip>());
            so.FindProperty("lampu").objectReferenceValue = l;
            so.FindProperty("dasar").floatValue = intensitas;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static ParticleSystem Partikel(GameObject akar, Material mat, int maks, float umur)
        {
            var ps = akar.AddComponent<ParticleSystem>();
            ps.transform.localRotation = Quaternion.identity;
            var m = ps.main;
            m.maxParticles = maks;
            m.startLifetime = umur;
            m.loop = true;
            m.simulationSpace = ParticleSystemSimulationSpace.World;
            var e = ps.emission;
            e.rateOverTime = maks / umur;
            var sh = ps.shape;
            sh.shapeType = ParticleSystemShapeType.Cone;
            sh.angle = 10f;
            sh.rotation = new Vector3(-90f, 0f, 0f);   // ke atas
            akar.GetComponent<ParticleSystemRenderer>().sharedMaterial = mat;
            return ps;
        }

        // Unlit transparan dengan tekstur titik 16 px, filter Point. tambah = aditif (api), selain itu alpha (asap).
        internal static Material MaterialPartikel(string nama, bool tambah)
        {
            string jalur = $"{FolderMaterial}/{nama}.mat";
            var ada = AssetDatabase.LoadAssetAtPath<Material>(jalur);
            if (ada != null) return ada;
            var mat = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit")
                ?? throw new System.InvalidOperationException("Shader URP Particles/Unlit tidak ditemukan."));
            mat.SetTexture("_BaseMap", TeksturTitik());
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_Blend", tambah ? 2f : 0f);
            mat.SetFloat("_SrcBlend", (float)(tambah ? UnityEngine.Rendering.BlendMode.One : UnityEngine.Rendering.BlendMode.SrcAlpha));
            mat.SetFloat("_DstBlend", (float)(tambah ? UnityEngine.Rendering.BlendMode.One : UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha));
            mat.SetFloat("_ZWrite", 0f);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            AssetDatabase.CreateAsset(mat, jalur);
            return mat;
        }

        static Texture2D TeksturTitik()
        {
            string jalur = $"{FolderMaterial}/TitikPartikel.asset";
            var ada = AssetDatabase.LoadAssetAtPath<Texture2D>(jalur);
            if (ada != null) return ada;
            const int n = 16;
            var t = new Texture2D(n, n, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
            for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float d = new Vector2(x - 7.5f, y - 7.5f).magnitude / 7.5f;
                t.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(1f - d)));
            }
            t.Apply();
            AssetDatabase.CreateAsset(t, jalur);
            return t;
        }
    }
}
