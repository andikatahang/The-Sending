using System;
using System.IO;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Referensi ke bagian rig yang dibutuhkan pembuat scene untuk menyambungkan sutradara dan benda.
    internal sealed class Rig
    {
        public PemainGerak Pemain;
        public Transform Kamera;
        public SenterHP Senter;
        public Hud Hud;
        public PanelDialog Dialog;
        public KartuTeks Kartu;
        public PanelGembok Gembok;
        public KameraAdegan Adegan;
        public Camera KameraCutscene;
        public LangkahKaki Langkah;
    }

    // Bagian yang sama di setiap scene gameplay: GameManager, pemain + HP, kamera adegan, UI, dan alat bantu tata letak.
    // Dipanggil SETELAH EditorSceneManager.NewScene.
    internal static class PembuatRig
    {
        const string JalurMixer = "Assets/_Project/Audio/Resources/MixerUtama.mixer";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";
        const string FolderLangkah = "Assets/_Project/Audio/ThirdParty/FootstepsEssentials";

        // Pemain selalu mulai menghadap +Z; sutradara memindahkannya lewat PemainGerak.Pindah bila perlu.
        internal static Rig Buat(Vector3 posisiPemain)
        {
            var jurnal = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");

            new GameObject("GameManager").AddComponent<GameManager>();

            var pemain = new GameObject("Pemain");
            pemain.transform.position = posisiPemain;
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

            var gerak = pemain.AddComponent<PemainGerak>();
            Set(gerak, "kamera", kamera.transform);
            var interaksi = pemain.AddComponent<PemainInteraksi>();
            Set(interaksi, "kamera", kamera.transform);
            var senterHp = pemain.AddComponent<SenterHP>();
            Set(senterHp, "cahaya", senter);
            Set(pemain.AddComponent<KompasHP>(), "pemain", pemain.transform);
            var langkah = Langkah(pemain);

            // Kamera adegan: mati sampai KameraAdegan.Mulai. Tanpa AudioListener (pendengar tetap di kamera pemain).
            // Tetap di akar scene: Timeline menggerakkannya dengan posisi dunia (PembuatTimeline.Gerak).
            var camAdegan = new GameObject("KameraAdegan").AddComponent<Camera>();
            camAdegan.nearClipPlane = 0.05f;
            PembuatGayaPsx.SiapkanKamera(camAdegan);
            camAdegan.enabled = false;

            var ui = new GameObject("UI");
            var hud = ui.AddComponent<Hud>();
            Set(hud, "interaksi", interaksi);
            Set(hud, "pemain", gerak);
            var kameraHp = pemain.AddComponent<KameraHP>();
            Set(kameraHp, "kamera", kamera.transform);
            Set(kameraHp, "hud", hud);
            var dialog = ui.AddComponent<PanelDialog>();
            Set(ui.AddComponent<PanelJurnal>(), "data", jurnal);
            var kartu = ui.AddComponent<KartuTeks>();   // tanpa kartu pembuka: sutradara scene yang menampilkan kartu
            var pengaturan = ui.AddComponent<PanelPengaturan>();
            Set(ui.AddComponent<MenuJeda>(), "pengaturan", pengaturan);
            Set(ui.AddComponent<AlatUji>(), "hud", hud);
            ui.AddComponent<PanelInventaris>();
            var gembok = ui.AddComponent<PanelGembok>();
            Set(gembok, "hud", hud);
            var adegan = ui.AddComponent<KameraAdegan>();
            Set(adegan, "kameraPemain", cam);
            Set(adegan, "kameraAdegan", camAdegan);

            var rig = new Rig
            {
                Pemain = gerak, Kamera = kamera.transform, Senter = senterHp, Hud = hud,
                Dialog = dialog, Kartu = kartu, Gembok = gembok, Adegan = adegan, KameraCutscene = camAdegan, Langkah = langkah,
            };
            // Klik senter dan suasana malam di setiap scene gameplay. Tanpa teks pengganti: diam sampai filenya ada di Audio/SFX.
            Set(senterHp, "klik", BuatSuara(rig, "KlikSenter", PembuatPengganti.FolderSfx, "SFX", ""));
            Set(kameraHp, "bunyiFlash", BuatSuara(rig, "FlashKamera", PembuatPengganti.FolderSfx, "SFX", ""));
            // Semua raycast berawal dari pemain; kapsulnya sendiri tidak boleh menghalangi (foto ke lantai mengenai kapsul).
            pemain.layer = 2;   // Ignore Raycast
            BuatSuara(rig, "SuasanaMalam", PembuatPengganti.FolderSfx, "Ambience", "", ulang: true)
                .GetComponent<AudioSource>().playOnAwake = true;
            return rig;
        }

        // Langkah kaki pemain: satu AudioSource 2D di grup SFX. Jenis lantai bawaan diatur pembuat scene (SetAngka "bawaan").
        static LangkahKaki Langkah(GameObject pemain)
        {
            var sumber = pemain.AddComponent<AudioSource>();
            sumber.playOnAwake = false;
            sumber.spatialBlend = 0f;
            sumber.outputAudioMixerGroup = GrupMixer("SFX");
            var langkah = pemain.AddComponent<LangkahKaki>();
            Set(langkah, "sumber", sumber);
            SetDaftar(langkah, "tanahJalan", KlipDi("Footsteps_DirtyGround/Footsteps_DirtyGround_Walk"));
            SetDaftar(langkah, "tanahLari", KlipDi("Footsteps_DirtyGround/Footsteps_DirtyGround_Run"));
            SetDaftar(langkah, "kayuJalan", KlipDi("Footsteps_Wood/Footsteps_Wood_Walk"));
            SetDaftar(langkah, "kayuLari", KlipDi("Footsteps_Wood/Footsteps_Wood_Run"));
            return langkah;
        }

        static AudioClip[] KlipDi(string subfolder)
        {
            string folder = $"{FolderLangkah}/{subfolder}";
            var klip = Array.ConvertAll(AssetDatabase.FindAssets("t:AudioClip", new[] { folder }),
                id => AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(id)));
            if (klip.Length == 0) throw new FileNotFoundException($"Tidak ada klip langkah di {folder}.");
            return klip;
        }

        static AudioMixerGroup GrupMixer(string grup)
        {
            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(JalurMixer)
                ?? throw new FileNotFoundException($"{JalurMixer} belum ada (dibuat tim di 0C).");
            var daftar = mixer.FindMatchingGroups(grup);
            if (daftar.Length == 0) throw new ArgumentException($"Grup mixer '{grup}' tidak ada di MixerUtama.");
            return daftar[0];
        }

        // Satu bunyi adegan. Klip null = aset belum ada: komponen Suara menampilkan teks pengganti di HUD.
        // ruang = suara 3D yang melekat pada benda (misalnya gemerincing paku Sosok Santet); selain itu 2D.
        internal static Suara BuatSuara(Rig rig, string nama, string folder, string grup, string teksPengganti,
            bool ulang = false, bool ruang = false)
        {
            var g = new GameObject("Suara " + nama);
            var sumber = g.AddComponent<AudioSource>();
            sumber.playOnAwake = false;
            sumber.loop = ulang;
            sumber.spatialBlend = ruang ? 1f : 0f;
            if (ruang)
            {
                sumber.rolloffMode = AudioRolloffMode.Linear;
                sumber.minDistance = 1f;
                sumber.maxDistance = 14f;
            }
            sumber.outputAudioMixerGroup = GrupMixer(grup);
            sumber.clip = PembuatPengganti.Klip(folder, nama);
            var suara = g.AddComponent<Suara>();
            Set(suara, "sumber", sumber);
            Set(suara, "hud", rig.Hud);
            SetTeks(suara, "teksPengganti", teksPengganti);
            return suara;
        }

        // Titik berdiri pemain atau bidikan kamera adegan: berada di `posisi`, menghadap ke `menghadap`.
        internal static Transform Titik(string nama, Vector3 posisi, Vector3 menghadap)
        {
            var t = new GameObject(nama).transform;
            t.position = posisi;
            t.LookAt(menghadap);
            return t;
        }

        internal static ZonaPemicu Zona(string nama, Vector3 pusat, Vector3 ukuran)
        {
            var g = new GameObject(nama);
            g.transform.position = pusat;
            var kotak = g.AddComponent<BoxCollider>();
            kotak.isTrigger = true;
            kotak.size = ukuran;
            return g.AddComponent<ZonaPemicu>();
        }

        internal static Light Lampu(string nama, Vector3 posisi, string hex, float jangkauan, float intensitas)
        {
            var lampu = new GameObject(nama).AddComponent<Light>();
            lampu.type = LightType.Point;
            lampu.color = Hex(hex);
            lampu.range = jangkauan;
            lampu.intensity = intensitas;
            lampu.transform.position = posisi;
            return lampu;
        }

        // Kotak bermaterial pengganti, dengan collider (dinding, lantai, pintu). UV dalam meter: tekstur berulang, tidak melar.
        internal static GameObject Balok(string nama, Vector3 posisi, Vector3 ukuran, string material)
        {
            var g = Kotak(nama, posisi, ukuran);
            g.GetComponent<MeshFilter>().sharedMesh = PembuatAsetAsli.KubusMeter(ukuran);
            g.GetComponent<Renderer>().sharedMaterial = PembuatPengganti.Material(material);
            return g;
        }

        // Objek logika: collider kotak (alas di titik nol) tempat skrip Interaktif dipasang. Visualnya prefab anak.
        internal static GameObject Logika(string nama, Vector3 posisi, Vector3 ukuranCollider)
        {
            var g = new GameObject(nama);
            g.transform.position = posisi;
            var kotak = g.AddComponent<BoxCollider>();
            kotak.size = ukuranCollider;
            kotak.center = new Vector3(0f, ukuranCollider.y / 2f, 0f);
            return g;
        }

        // Dinding tak terlihat. Layer Ignore Raycast: menahan badan pemain, tapi tidak menghalangi E dan foto (Q)
        // ke benda di baliknya (misalnya pintu Bu Sumi di luar batas timur jalan desa).
        internal static void Batas(string nama, Vector3 pusat, Vector3 ukuran)
        {
            var g = new GameObject(nama) { layer = 2 };
            g.transform.position = pusat;
            g.AddComponent<BoxCollider>().size = ukuran;
        }

        internal static Color Hex(string hex) =>
            ColorUtility.TryParseHtmlString("#" + hex, out var warna) ? warna : throw new FormatException($"Warna salah: {hex}");
    }
}
