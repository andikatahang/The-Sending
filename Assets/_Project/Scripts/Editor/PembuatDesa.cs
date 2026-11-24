using System.IO;
using System.Linq;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Membuat Scene_Desa: jalan masuk desa untuk Prolog (denah K7) dan jalan malam ke rumah Bu Sumi di Malam 3 (K17-B).
    // Jangan sunting scene dengan tangan; ubah angka di sini.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatDesa.Buat
    public static class PembuatDesa
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.Desa + ".unity";
        const float SetengahLebar = 6.5f, UjungJalan = 100f;
        const string Sfx = PembuatPengganti.FolderSfx;

        [MenuItem("The Sending/Buat Scene Desa")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();
            var rig = PembuatRig.Buat(new Vector3(0f, 0.05f, 2f));
            SetAngka(rig.Langkah, "bawaan", (int)JenisPermukaan.Tanah);

            Lingkungan();
            var sutradara = new GameObject("Sutradara Prolog").AddComponent<AlurProlog>();
            Set(sutradara, "adegan", rig.Adegan);
            Set(sutradara, "kartu", rig.Kartu);
            Set(sutradara, "dialog", rig.Dialog);
            Set(sutradara, "hud", rig.Hud);
            Set(sutradara, "dialogOjek", PembuatDataFase1.Muat(PembuatDataFase1.Ojek));

            // Gapura dan mobil yang mengantar Raka, parkir di luar gapura menghadap ke selatan.
            DekorDesa();
            var ojek = BuatMobil(new Vector3(1.2f, 0f, -3.2f));
            Set(sutradara, "ojek", ojek.transform);
            Set(sutradara, "bidikGapura", Titik("BidikGapura", new Vector3(-2.5f, 1.7f, 7f), new Vector3(0.5f, 1.2f, 0f)));
            // Cutscene mobil pergi (3,5 detik): mobil lurus ke z = -25 dalam 3 detik, suara mesin sejak awal.
            var ojekPergi = PembuatTimeline.Buat("Prolog_OjekPergi", 3.5f);
            PembuatTimeline.Gerak(ojekPergi.Tl, ojekPergi.Director, ojek,
                (0f, ojek.transform), (3f, Titik("TitikOjekPergi", new Vector3(1.5f, ojek.transform.position.y, -25f), new Vector3(1.5f, ojek.transform.position.y, -30f))));
            PembuatTimeline.Sinyal(ojekPergi.Tl, 0.05f, AlurProlog.SinyalMotorPergi);
            PembuatTimeline.Simpan(ojekPergi.Tl);
            Set(sutradara, "timelineOjek", ojekPergi.Pemutar);

            // Zona tutorial dan azan
            var selebarJalan = new Vector3(SetengahLebar * 2f, 3f, 1.5f);
            Set(sutradara, "zonaSenter", Zona("ZonaSenter", new Vector3(0f, 1.5f, 18f), selebarJalan));
            Set(sutradara, "zonaAzan", Zona("ZonaAzan", new Vector3(0f, 1.5f, 58f), selebarJalan));
            Set(sutradara, "zonaBuSumi", Zona("ZonaBuSumi", new Vector3(0f, 1.5f, 72f), selebarJalan));

            // Rumah Bu Sumi di kanan jalan, menghadap jalan. Jendela prefab (1.2, 1.5, 2.5) jatuh di (7.5, 1.5, 76.2).
            // Semua bagian rumah utuh (termasuk pintu Bu Sumi tiap malam) di satu induk: PuingRumahSumi mematikannya di jalur Fitnah.
            rumahSumiUtuh = new GameObject("RumahBuSumi Utuh").transform;
            PembuatPengganti.Pasang("RumahKecil", rumahSumiUtuh, new Vector3(10f, 0f, 75f), -90f);
            // Tirai di ceruk bingkai (x 7,41-7,49, kaca di 7,49). Siluet berdiri di dalam rumah 0,4 m di belakang kaca,
            // menghadap jalan, dengan lampu dan bidang cahaya di belakangnya; dari ambang (0,92) ke bawah tertutup dinding.
            Balok("AmbangJendela", new Vector3(7.42f, 0.92f, 76.2f), new Vector3(0.14f, 0.06f, 1.3f), "KayuGelap");
            Balok("CahayaJendela", new Vector3(8.4f, 1.5f, 76.2f), new Vector3(0.01f, 2f, 2.4f), "Kain");
            var siluet = PembuatPengganti.Pasang("BuSumi_Siluet", null, new Vector3(7.9f, -0.08f, 76.2f), -90f);
            siluet.transform.localScale = new Vector3(1f, 1f, 0.05f);
            var tirai = Balok("Tirai", new Vector3(7.44f, 1.5f, 76.2f), new Vector3(0.01f, 1.1f, 1.1f), "Kain");
            tirai.SetActive(false);
            Set(sutradara, "siluetBuSumi", siluet);
            Set(sutradara, "tiraiJendela", tirai);
            var lampuJendela = Lampu("LampuJendela", new Vector3(8.2f, 1.7f, 76.2f), "FFB46B", 4f, 2f);
            Set(sutradara, "lampuJendela", lampuJendela);
            // K47-A: kaca gelap di lubang jendela, di luar Prolog (siluet Prolog terlihat tanpa kaca).
            var kaca = Balok("KacaGelap", new Vector3(7.47f, 1.5f, 76.2f), new Vector3(0.02f, 1.2f, 1.3f), "KacaGelap");
            PembuatMalam2.AktifPada(sutradara.gameObject, 1, 7, kaca);
            foreach (var g in new[] { GameObject.Find("AmbangJendela"), GameObject.Find("CahayaJendela"), siluet, tirai, lampuJendela.gameObject, kaca })
                g.transform.SetParent(rumahSumiUtuh, true);
            var bidikJendela = Titik("BidikJendela", new Vector3(2.5f, 1.6f, 74.5f), new Vector3(7.4f, 1.5f, 76.2f));
            Set(sutradara, "bidikJendela", bidikJendela);
            // Cutscene Bu Sumi (4 detik): kamera adegan mendekat pelan ke jendela, tirai menutup pada 2,5 detik.
            var buSumi = PembuatTimeline.Buat("Prolog_BuSumi", 4f);
            PembuatTimeline.Gerak(buSumi.Tl, buSumi.Director, rig.KameraCutscene.gameObject,
                (0f, bidikJendela), (4f, Titik("BidikJendelaDekat", new Vector3(3.5f, 1.6f, 75f), new Vector3(7.4f, 1.5f, 76.2f))));
            PembuatTimeline.Sinyal(buSumi.Tl, 2.5f, AlurProlog.SinyalTiraiDitutup);
            PembuatTimeline.Simpan(buSumi.Tl);
            Set(sutradara, "timelineBuSumi", buSumi.Pemutar);

            // Rumah keluarga: prefab menghadap selatan (ke arah pemain), pintunya memindahkan ke Scene_RumahKeluarga.
            // Kayu: yang bisa diinjak dari luar hanya lantai emper (teras).
            Lampu("LampuEmper", new Vector3(0f, 2.4f, UjungJalan - 1f), "FFB46B", 6f, 1.2f);   // di bawah lampu teras prefab
            PembuatPengganti.Pasang("RumahKeluargaLuar", null, new Vector3(0f, 0f, UjungJalan), 180f)
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Kayu;
            var pintu = Balok("PintuRumah", new Vector3(0f, 1.05f, UjungJalan - 0.15f), new Vector3(1.1f, 2.1f, 0.2f), "KayuGelap");
            var alurPintu = pintu.AddComponent<InteraktifAlur>();
            SetTeks(alurPintu, "prompt", "Masuk rumah");
            Set(sutradara, "pintuRumah", alurPintu);

            // Suara. Azan tidak punya pengganti sintetis; selama filenya belum ada hanya teks ini yang tampil.
            // Tidak ada suara telepon: suara tokoh ditunda, telepon Laras hanya kartu teks.
            Set(sutradara, "suaraMotor", BuatSuara(rig, "MobilPergi", Sfx, "SFX", "(Suara: mobil menjauh.)"));
            Set(sutradara, "suaraAzan", BuatSuara(rig, "AzanMagrib", Sfx, "Ambience", "(Suara: azan magrib dari kejauhan.)"));
            var suaraPintu = BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)");
            Set(sutradara, "suaraPintu", suaraPintu);
            Malam3(rig, alurPintu, suaraPintu);
            Malam4(rig, alurPintu, suaraPintu);
            Malam5(rig, alurPintu, suaraPintu);
            Malam6(rig);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene desa dibuat: {JalurScene}");
        }

        // Malam 3 (K17-B): Raka berjalan dari rumah keluarga ke rumah Bu Sumi (pintu di (7,5; 73,5), menghadap jalan) dan kembali.
        // Pintu rumah keluarga dipakai bersama AlurProlog; AlurProlog mati di luar Prolog dan melepas langganannya.
        static void Malam3(Rig rig, InteraktifAlur pintuRumah, Suara suaraPintu)
        {
            var alur = new GameObject("Sutradara Malam 3").AddComponent<AlurDesaMalam3>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "pintuRumah", pintuRumah);
            Set(alur, "suaraPintu", suaraPintu);
            Set(alur, "suaraKetuk", BuatSuara(rig, "KetukPintu", Sfx, "SFX", "(Suara: ketukan di pintu.)"));

            var ketuk = PintuBuSumi("PintuBuSumi");
            var pintu = ketuk.gameObject;
            Set(alur, "pintuBuSumi", ketuk);
            PembuatMalam2.AktifPada(alur.gameObject, 3, 3, pintu);
            Set(alur, "titikDepanRumah", Titik("TitikDepanRumahMalam3", new Vector3(0f, 0.1f, 96.8f), new Vector3(0f, 0.1f, 80f)));
            Set(alur, "titikDepanBuSumi", Titik("TitikDepanBuSumi", new Vector3(6.2f, 0.05f, 73.5f), new Vector3(0f, 0.05f, 73.5f)));
        }

        // Tebal dan maju dari dinding depan (x 7,5) supaya sinar E tidak lebih dulu kena collider dinding/bingkai pintu.
        // Tiap malam punya salinannya sendiri: AktifDiMalam ganda pada satu objek akan saling menimpa.
        static InteraktifAlur PintuBuSumi(string nama)
        {
            var ketuk = Logika(nama, new Vector3(7.05f, 0f, 73.5f), new Vector3(0.7f, 2.2f, 1.4f)).AddComponent<InteraktifAlur>();
            ketuk.transform.SetParent(rumahSumiUtuh, true);
            SetTeks(ketuk, "prompt", "Ketuk pintu Bu Sumi");
            return ketuk;
        }

        // Malam 5 (K32-A): gerbang makam dan rumah Pakde di kiri (barat) jalan. Ujung utara jalan sudah ditempati rumah
        // keluarga, jadi gerbang makam di sisi barat dekat ujung utara. Collider logika di dalam BatasBarat (x -6,5).
        static void Malam5(Rig rig, InteraktifAlur pintuRumah, Suara suaraPintu)
        {
            var alur = new GameObject("Sutradara Malam 5").AddComponent<AlurDesaMalam5>();
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "pintuRumah", pintuRumah);
            Set(alur, "suaraPintu", suaraPintu);
            Set(alur, "suaraKetuk", BuatSuara(rig, "KetukPintu", Sfx, "SFX", "(Suara: ketukan di pintu.)"));
            Set(alur, "suasana", BuatSuara(rig, "SuasanaDesaMalam5", Sfx, "Ambience", "", ulang: true));   // tanpa file: diam (SuasanaMalam rig sudah berbunyi)

            PembuatPengganti.Pasang("GerbangMakam", null, new Vector3(-7f, 0f, 92f), 90f);
            var gerbang = Logika("GerbangMakam Malam 5", new Vector3(-6.2f, 0f, 92f), new Vector3(0.4f, 2.2f, 2.4f));
            var aGerbang = gerbang.AddComponent<InteraktifAlur>();
            SetTeks(aGerbang, "prompt", "Masuk makam desa");
            Set(alur, "gerbangMakam", aGerbang);

            PembuatPengganti.Pasang("RumahPakdeLuar", null, new Vector3(-13f, 0f, 74f), 90f);
            Lampu("LampuTerasPakde", new Vector3(-9f, 2.6f, 74f), "FFD9A0", 6f, 0.8f);   // teras prefab di x -9 (lokal z 4)
            PembuatPengganti.Pasang("PagarBesi", null, new Vector3(-7f, 0f, 74f), 90f);
            var pagar = Logika("PagarPakde Malam 5", new Vector3(-6.2f, 0f, 74f), new Vector3(0.4f, 1.4f, 2.2f));
            var aPagar = pagar.AddComponent<InteraktifAlur>();
            SetTeks(aPagar, "prompt", "Masuk halaman rumah Pakde");
            Set(alur, "pagarPakde", aPagar);

            var ketuk = PintuBuSumi("PintuBuSumi Malam 5");
            Set(alur, "pintuBuSumi", ketuk);
            PembuatMalam2.AktifPada(alur.gameObject, 5, 6, gerbang, pagar, ketuk.gameObject);   // K46-A

            Set(alur, "titikDepanRumah", Titik("TitikDepanRumahMalam5", new Vector3(0f, 0.1f, 96.8f), new Vector3(0f, 0.1f, 80f)));
            Set(alur, "titikGerbangMakam", Titik("TitikGerbangMakam", new Vector3(-5f, 0.05f, 92f), new Vector3(0f, 0.05f, 92f)));
            Set(alur, "titikDepanBuSumi", Titik("TitikDepanBuSumiMalam5", new Vector3(6.2f, 0.05f, 73.5f), new Vector3(0f, 0.05f, 73.5f)));
            Set(alur, "titikDepanPakde", Titik("TitikDepanPakde", new Vector3(-5f, 0.05f, 74f), new Vector3(0f, 0.05f, 74f)));
        }

        static Transform rumahSumiUtuh;

        // Malam 6: sidang warga ber-obor di jalan depan rumah Bu Sumi (pintu x 7,5; z 73,5).
        static void Malam6(Rig rig)
        {
            var alur = new GameObject("Sutradara Malam 6").AddComponent<AlurDesaMalam6>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "jurnal", AssetDatabase.LoadAssetAtPath<DataJurnal>("Assets/_Project/Data/Jurnal.asset"));
            Set(alur, "panel", new GameObject("PanelSidang").AddComponent<PanelSidang>());
            Set(alur, "titikSidang", Titik("TitikSidang", new Vector3(2.5f, 0.05f, 73.5f), new Vector3(10f, 0.05f, 73.5f)));
            Set(alur, "bidikKerumunan", Titik("BidikKerumunan", new Vector3(-1f, 2.2f, 66f), new Vector3(4f, 1.4f, 73.5f)));
            Set(alur, "bidikBuSumi", Titik("BidikBuSumiSidang", new Vector3(4.5f, 1.6f, 73.5f), new Vector3(7.2f, 1.5f, 73.5f)));
            Set(alur, "bidikRumah", Titik("BidikRumahSumi", new Vector3(-2f, 3f, 70f), new Vector3(10f, 2f, 75f)));

            // Kerumunan di induk bertransform identitas (posisi lokal = dunia), mati di awal; tokoh menghadap rumah (timur).
            var kerumunan = new GameObject("Kerumunan Malam 6");
            var buSumi = PembuatPengganti.Pasang("BuSumi", kerumunan.transform, new Vector3(6.8f, 0f, 73.5f), -90f);
            var tokoh = new (string nama, Vector3 pos, bool obor)[]
            {
                ("PakJoko_Obor", new Vector3(4.2f, 0f, 72f), false),   // obornya sudah di prefab
                ("Warga1", new Vector3(4.5f, 0f, 70f), true),
                ("Warga2", new Vector3(3.5f, 0f, 71.3f), true),
                ("Warga3", new Vector3(3.5f, 0f, 75.7f), true),
                ("Warga4", new Vector3(4.5f, 0f, 77f), true),
                ("Warga5", new Vector3(5.5f, 0f, 76f), false),
            };
            var datang = PembuatTimeline.Buat("Malam6_WargaDatang", 6f);
            var orang = new GameObject[tokoh.Length];
            for (int i = 0; i < tokoh.Length; i++)
            {
                var (nama, pos, obor) = tokoh[i];
                orang[i] = PembuatPengganti.Pasang(nama, kerumunan.transform, pos, 90f);
                if (obor) PembuatPengganti.Pasang("Obor", PembuatPengganti.Tulang(orang[i], "hand.R"), Vector3.zero);
                PembuatTimeline.Gerak(datang.Tl, datang.Director, orang[i],
                    (0f, Titik($"Datang {nama} Awal", pos + Vector3.forward * 15f, pos + Vector3.forward * 14f)),
                    (5f, Titik($"Datang {nama} Akhir", pos, pos + Vector3.right)));
            }
            PembuatTimeline.Sinyal(datang.Tl, 1f, AlurDesaMalam6.SinyalObor);
            PembuatTimeline.Simpan(datang.Tl);
            Set(alur, "timelineDatang", datang.Pemutar);
            SetDaftar(alur, "lampuObor", kerumunan.GetComponentsInChildren<Light>(true));

            var berhasil = PembuatTimeline.Buat("Malam6_Berhasil", 4f);
            PembuatTimeline.Gerak(berhasil.Tl, berhasil.Director, orang[0],
                (0f, Titik("Berhasil Joko Awal", tokoh[0].pos, tokoh[0].pos + Vector3.right)),
                (2f, Titik("Berhasil Joko Akhir", tokoh[0].pos + Vector3.right, tokoh[0].pos + Vector3.right * 2f)));
            PembuatTimeline.Sinyal(berhasil.Tl, 2.5f, AlurDesaMalam6.SinyalTurunkanObor);
            PembuatTimeline.Simpan(berhasil.Tl);
            Set(alur, "timelineBerhasil", berhasil.Pemutar);

            var gagal = PembuatTimeline.Buat("Malam6_Gagal", 5f);
            foreach (var (i, z) in new[] { (1, 72f), (4, 75f) })
            {
                var tujuan = new Vector3(7f, 0f, z);
                PembuatTimeline.Gerak(gagal.Tl, gagal.Director, orang[i],
                    (0f, Titik($"Gagal {tokoh[i].nama} Awal", tokoh[i].pos, tujuan)),
                    (3f, Titik($"Gagal {tokoh[i].nama} Akhir", tujuan, tujuan + Vector3.right)));
            }
            PembuatTimeline.Sinyal(gagal.Tl, 3f, AlurDesaMalam6.SinyalApi);
            PembuatTimeline.Simpan(gagal.Tl);
            Set(alur, "timelineGagal", gagal.Pemutar);

            var api = new GameObject("Api RumahSumi");
            foreach (float z in new[] { 72f, 74.5f, 77f })
                PembuatPengganti.Pasang("ApiBesar", api.transform, new Vector3(8f, 0f, z));
            PembuatPengganti.Pasang("AsapHitam", api.transform, new Vector3(9f, 3f, 75f));
            Lampu("LampuApi", new Vector3(7f, 2f, 74.5f), "FF7A2A", 16f, 3f).transform.SetParent(api.transform, true);
            api.SetActive(false);
            Set(alur, "api", api);

            var rumah = new GameObject("RumahSumi Utuh/Puing").AddComponent<PuingRumahSumi>();
            SetDaftar(rumah, "utuh", new[] { rumahSumiUtuh.gameObject });
            SetDaftar(rumah, "puing", new[] { PembuatPengganti.Pasang("PuingRumahKecil", null, new Vector3(10f, 0f, 75f), -90f) });
            Set(alur, "rumahSumi", rumah);

            Set(alur, "dialogPembuka", PembuatDataFase6.Muat(PembuatDataFase6.SidangPembuka));
            SetDaftar(alur, "dialogTuduhan", new[] { PembuatDataFase6.Tuduhan1, PembuatDataFase6.Tuduhan2, PembuatDataFase6.Tuduhan3 }.Select(PembuatDataFase6.Muat).ToArray());
            SetDaftar(alur, "dialogBenar", new[] { PembuatDataFase6.Benar1, PembuatDataFase6.Benar2, PembuatDataFase6.Benar3 }.Select(PembuatDataFase6.Muat).ToArray());
            SetDaftar(alur, "dialogSalah", new[] { PembuatDataFase6.Salah1, PembuatDataFase6.Salah2, PembuatDataFase6.Salah3 }.Select(PembuatDataFase6.Muat).ToArray());
            Set(alur, "dialogBerhasil", PembuatDataFase6.Muat(PembuatDataFase6.SidangBerhasil));
            Set(alur, "dialogGagal", PembuatDataFase6.Muat(PembuatDataFase6.SidangGagal));
            Set(alur, "dialogDiam", PembuatDataFase6.Muat(PembuatDataFase6.SidangDiam));
            Set(alur, "dialogMenuduh", PembuatDataFase6.Muat(PembuatDataFase6.SidangMenuduh));

            var kerumunanSuara = BuatSuara(rig, "KerumunanMarah", Sfx, "SFX", "(Suara: kerumunan warga marah.)", ulang: true, ruang: true);
            kerumunanSuara.transform.SetParent(kerumunan.transform, false);
            kerumunanSuara.transform.localPosition = new Vector3(4.5f, 1.5f, 73.5f);
            Set(alur, "suaraKerumunan", kerumunanSuara);
            Set(alur, "suaraTeriak", BuatSuara(rig, "TeriakBakar", Sfx, "SFX", "(Suara: warga berteriak \"Bakar!\")"));
            var suaraApi = BuatSuara(rig, "RumahTerbakar", Sfx, "SFX", "(Suara: api melalap rumah.)", ulang: true, ruang: true);
            suaraApi.transform.position = new Vector3(8f, 1.5f, 74.5f);
            Set(alur, "suaraApi", suaraApi);
            const string Musik = PembuatPengganti.FolderMusik;
            string musik = PembuatPengganti.AdaSuara(Musik, "MusikSidang") ? "MusikSidang"
                : PembuatPengganti.AdaSuara(Musik, "MusikTegang") ? "MusikTegang" : "DroneHoror";
            Set(alur, "musik", BuatSuara(rig, musik, Musik, "Music", "(Musik tegang.)", ulang: true));

            Set(alur, "kerumunan", kerumunan);
            Set(alur, "buSumi", buSumi);
            kerumunan.SetActive(false);
        }

        // Malam 4 (K26-A): pos ronda di kiri jalan, sekitar 12 m di selatan rumah keluarga. Bangunannya selalu ada; orang dan suasananya hanya Malam 4.
        static void Malam4(Rig rig, InteraktifAlur pintuRumah, Suara suaraPintu)
        {
            var alur = new GameObject("Sutradara Malam 4").AddComponent<AlurDesaMalam4>();
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "panel", rig.Dialog);
            Set(alur, "pintuRumah", pintuRumah);
            Set(alur, "suaraPintu", suaraPintu);
            Set(alur, "dialogPakJoko", PembuatDataFase4.Muat(PembuatDataFase4.PakJoko));
            Set(alur, "dialogGosip", PembuatDataFase4.Muat(PembuatDataFase4.Gosip));
            Set(alur, "dialogB2", PembuatDataFase4.Muat(PembuatDataFase4.B2Cemani));
            Set(alur, "titikDepanRumah", Titik("TitikDepanRumahMalam4", new Vector3(0f, 0.1f, 96.8f), new Vector3(0f, 0.1f, 80f)));

            var pos = new GameObject("PosRonda Malam 4");
            PembuatPengganti.Pasang("PosRonda", null, new Vector3(-4.5f, 0f, 88f), 90f);
            PembuatPengganti.Pasang("Kentongan", null, new Vector3(-3.05f, 1.6f, 87f), 90f);
            PembuatPengganti.Pasang("KursiPlastik", null, new Vector3(-3.3f, 0f, 87.5f), -90f);   // menghadap pos (-X)
            PembuatPengganti.Pasang("KursiPlastik", null, new Vector3(-3.3f, 0f, 89f), -90f);   // menghadap pos (-X)
            var joko = Logika("PakJoko", new Vector3(-2.6f, 0f, 88.6f), new Vector3(0.6f, 1.8f, 0.6f));
            joko.transform.SetParent(pos.transform, true);
            PembuatPengganti.Pasang("PakJoko", joko.transform, Vector3.zero, 90f);
            var aJoko = joko.AddComponent<InteraktifAlur>();
            SetTeks(aJoko, "prompt", "Bicara dengan Pak Joko");
            Set(alur, "pakJoko", aJoko);
            var warga = Logika("Warga", new Vector3(-4.5f, 0f, 86.2f), new Vector3(2f, 1.8f, 0.8f));
            warga.transform.SetParent(pos.transform, true);
            PembuatPengganti.Pasang("Warga1", warga.transform, new Vector3(-0.5f, 0f, 0f), 0f);
            PembuatPengganti.Pasang("Warga2", warga.transform, new Vector3(0.5f, 0f, 0f), 0f);
            var aWarga = warga.AddComponent<InteraktifAlur>();
            SetTeks(aWarga, "prompt", "Dengarkan obrolan warga");
            Set(alur, "warga", aWarga);
            Lampu("LampuPosRonda", new Vector3(-4.5f, 2.4f, 88f), "FFB46B", 6f, 1.5f);
            var suasana = BuatSuara(rig, "SuasanaPosRonda", Sfx, "Ambience", "(Suara: suasana pos ronda, kentongan dan jangkrik.)", ulang: true);
            suasana.transform.SetParent(pos.transform, false);
            Set(alur, "suasana", suasana);
            PembuatMalam2.AktifPada(alur.gameObject, 4, 4, pos);
        }

        // Dekor desa di prefab Prefabs/Dekor/Dekor_Desa (PembuatDekor), dipakai juga latar menu utama.
        static void DekorDesa() => PembuatDekor.Pasang("Desa", r =>
        {
            PembuatPengganti.Pasang("Gapura", r, Vector3.zero);
            DekorJalan(r);
        });

        // Lampu jalan, kebun bambu, pagar kayu, dan hutan.
        internal static void DekorJalan(Transform r)
        {
            // Selang-seling barat/timur; di luar kebun bambu (z 20-42) dan bidikan jendela Bu Sumi (z 72-76).
            float[] zLampu = { 10f, 44f, 62f, 80f, 90f };
            for (int i = 0; i < zLampu.Length; i++)
            {
                float x = i % 2 == 0 ? -3f : 3f;
                PembuatPengganti.Pasang("LampuJalan", r, new Vector3(x, 0f, zLampu[i]), x < 0f ? 0f : 180f);
                Lampu("CahayaLampuJalan", new Vector3(x < 0f ? x + 0.7f : x - 0.7f, 2.8f, zLampu[i]), "FFB46B", 8f, 1.5f)   // di bawah kepala lampu
                    .transform.SetParent(r, true);
            }

            // Kebun bambu z 20-40: enam rumpun di tiap sisi, selang-seling jaraknya dari jalan.
            for (int i = 0; i < 6; i++)
            {
                float z = 20f + i * 4f, x = 3.5f + i % 2;
                PembuatPengganti.Pasang("RumpunBambu", r, new Vector3(-x, 0f, z), i * 37f);
                PembuatPengganti.Pasang("RumpunBambu", r, new Vector3(x, 0f, z + 2f), i * 53f);
            }

            // Pagar kayu (visual, tanpa collider) tepat di luar BatasBarat/BatasTimur: halaman rumah keluarga dan sisi rumah Bu Sumi,
            // menjauhi gerbang makam (z 92), pagar Pakde (z 74), serta pintu (z 73,5) dan jendela (z 76,2) Bu Sumi.
            foreach (float z in new[] { 97f, 99f }) PembuatPengganti.Pasang("PagarKayu", r, new Vector3(-SetengahLebar - 0.1f, 0f, z), 90f);
            foreach (float z in new[] { 95f, 97f, 99f }) PembuatPengganti.Pasang("PagarKayu", r, new Vector3(SetengahLebar + 0.1f, 0f, z), 90f);
            foreach (float z in new[] { 70f, 80f }) PembuatPengganti.Pasang("PagarKayu", r, new Vector3(SetengahLebar + 0.1f, 0f, z), 90f);
            Hutan(r);
        }

        // Latar Scene_MainMenu: desa malam tanpa pemain dan tanpa logika (hanya tampilan).
        internal static void Latar()
        {
            Lingkungan();
            DekorDesa();
            PembuatPengganti.Pasang("RumahKecil", null, new Vector3(10f, 0f, 75f), -90f);
            Balok("CahayaJendela", new Vector3(7.47f, 1.5f, 76.2f), new Vector3(0.02f, 1.1f, 1.1f), "Kain");
            Lampu("LampuJendela", new Vector3(8.2f, 1.7f, 76.2f), "FFB46B", 4f, 2f);
            PembuatPengganti.Pasang("RumahKeluargaLuar", null, new Vector3(0f, 0f, UjungJalan), 180f);
        }

        static void Lingkungan()
        {
            var bulan = new GameObject("CahayaBulan").AddComponent<Light>();
            bulan.type = LightType.Directional;
            bulan.intensity = 0.15f;
            bulan.color = Hex("6D86A8");
            bulan.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Tanah hutan sampai sekitar 40 m dari jalan (kabut menelan sisanya), dan jalan tanah di koridor.
            Balok("Tanah", new Vector3(0f, -0.1f, 55f), new Vector3(100f, 0.2f, 200f), "TanahHutan");
            Jalan("Jalan", -45f, 48.5f);
            Jalan("Jalan", 55.5f, 100f);
            Batas("BatasBarat", new Vector3(-SetengahLebar, 2f, 50f), new Vector3(0.2f, 4f, 140f));
            Batas("BatasTimur", new Vector3(SetengahLebar, 2f, 50f), new Vector3(0.2f, 4f, 140f));
            Batas("BatasSelatan", new Vector3(0f, 2f, -1.5f), new Vector3(SetengahLebar * 2f, 4f, 0.2f));
            // R12: ujung utara di balik rumah keluarga, kalau-kalau collider prefab rumah menyisakan celah.
            Batas("BatasUtara", new Vector3(0f, 2f, 104f), new Vector3(SetengahLebar * 2f, 4f, 0.2f));

            // Jembatan bambu z 48-56. Air hanya tampilan; kedua sisi jembatan ditutup pembatas.
            // Sungai berlanjut ke dalam hutan di kedua sisi.
            var air = Balok("Air", new Vector3(0f, 0.01f, 52f), new Vector3(100f, 0.02f, 6f), "Gelap");
            Object.DestroyImmediate(air.GetComponent<Collider>());
            PembuatPengganti.Pasang("JembatanBambu", null, new Vector3(0f, 0f, 52f))
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Kayu;
            Batas("BatasAirBarat", new Vector3(-3.75f, 1f, 52f), new Vector3(5.5f, 2f, 6.2f));
            Batas("BatasAirTimur", new Vector3(3.75f, 1f, 52f), new Vector3(5.5f, 2f, 6.2f));
        }

        static void Jalan(string nama, float z1, float z2)
        {
            var g = Balok(nama, new Vector3(0f, 0.005f, (z1 + z2) / 2f), new Vector3(5f, 0.01f, z2 - z1), "Jalan");
            Object.DestroyImmediate(g.GetComponent<Collider>());
        }

        const string Alam = PembuatAsetAsli.Alam;
        static readonly string[] Pohon = { "SawtoothOak", "PlaneTree", "Ginkgo_wood" };
        static readonly string[] Semak = { "JapaneseLaurel", "JapaneseEurya", "Mugwort", "ViolaGrypoceras" };
        static readonly string[] Batu = { "Rock_A", "Rock_B", "Rock_C", "Rock_D" };
        static readonly string[] Kayu = { "Decayed_wood_A", "Decayed_wood_B", "Decayed_wood_C", "Decayed_wood_D", "Decayed_wood_E" };

        // Hutan di luar dinding koridor (|x| > 6.5): pohon berdaun lebar, semak, batu, kayu lapuk, dan daun gugur.
        // Seed tetap: hasilnya sama setiap kali dibangun. Tanpa collider (pemain tidak bisa keluar koridor).
        // Tetap kosong: jalan mobil, sungai, rumah Bu Sumi dan halamannya, rumah keluarga.
        static void Hutan(Transform r)
        {
            var induk = new GameObject("Hutan").transform;
            induk.SetParent(r, false);
            var acak = new System.Random(2003);
            float Acak(float a, float b) => a + (float)acak.NextDouble() * (b - a);
            string Pilih(string[] daftar) => daftar[acak.Next(daftar.Length)];

            const float jarak = 5f;
            for (float z = -45f; z <= 150f; z += jarak)
                for (float jauh = 7f; jauh <= 45f; jauh += jarak)
                    foreach (float sisi in new[] { -1f, 1f })
                    {
                        float x = sisi * (jauh + Acak(0f, jarak - 1f)), pz = z + Acak(0f, jarak - 1f);
                        if (Terlarang(x, pz)) continue;
                        double jenis = acak.NextDouble();
                        float putar = Acak(0f, 360f);
                        var posisi = new Vector3(x, 0f, pz);
                        if (jenis < 0.55) PembuatAsetAsli.Vendor(Alam + Pilih(Pohon) + ".prefab", induk, posisi, putar, Acak(0.85f, 1.3f));
                        else if (jenis < 0.8) PembuatAsetAsli.Vendor(Alam + Pilih(Semak) + ".prefab", induk, posisi, putar, Acak(0.8f, 1.4f));
                        else if (jenis < 0.9) PembuatAsetAsli.Vendor(Alam + Pilih(Batu) + ".prefab", induk, posisi, putar, Acak(0.7f, 1.3f));
                        else PembuatAsetAsli.Vendor(Alam + Pilih(Kayu) + ".prefab", induk, posisi, putar, 1f);
                        if (acak.NextDouble() < 0.4)
                            PembuatAsetAsli.Vendor(Alam + "FallenLeaves.prefab", induk, posisi + new Vector3(Acak(-2f, 2f), 0f, Acak(-2f, 2f)), Acak(0f, 360f), Acak(1f, 1.8f));
                    }

            // Di tepi jalan (dalam koridor) hanya daun gugur dan bunga kecil setinggi mata kaki.
            for (float z = 4f; z <= 92f; z += 6f)
                foreach (float sisi in new[] { -1f, 1f })
                {
                    float x = sisi * Acak(4.5f, 6.2f), pz = z + Acak(0f, 4f);
                    if (pz > 46f && pz < 58f) continue;
                    var nama = acak.NextDouble() < 0.6 ? "FallenLeaves" : "ViolaGrypoceras";
                    PembuatAsetAsli.Vendor(Alam + nama + ".prefab", induk, new Vector3(x, 0f, pz), Acak(0f, 360f), 1f);
                }
        }

        static bool Terlarang(float x, float z) =>
            Mathf.Abs(x) < SetengahLebar + 0.5f
            || (z > 46.5f && z < 57.5f)                              // sungai
            || (x > 6f && x < 16f && z > 67f && z < 83f)            // rumah Bu Sumi
            || (x < -6f && x > -18f && z > 68f && z < 80f)          // rumah Pakde
            || (x < -6f && x > -10f && z > 89f && z < 95f)          // gerbang makam
            || (Mathf.Abs(x) < 11f && z > 95f && z < 116f);         // rumah keluarga

        // Model mobil Sketchfab (diimpor glTFast, sudah berskala meter, depan di +Z). Tanpa model: pakai prefab pengganti.
        const string ModelMobil = "Assets/_Project/Art/Models/Desa/MobilLMPV.glb";

        static GameObject BuatMobil(Vector3 posisi)
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelMobil);
            if (model == null) Debug.LogWarning($"{ModelMobil} belum terimpor (paket glTFast?); memakai prefab pengganti Ojek.");
            var mobil = model != null ? (GameObject)PrefabUtility.InstantiatePrefab(model) : PembuatPengganti.Pasang("Ojek", null, Vector3.zero);
            mobil.name = "Ojek";
            mobil.transform.SetPositionAndRotation(posisi, Quaternion.Euler(0f, 180f, 0f));
            // Pivot glb ada di tengah tinggi mobil: angkat supaya roda di posisi.y. Kunci Timeline memakai tinggi ini juga.
            float bawah = float.MaxValue;
            foreach (var r in mobil.GetComponentsInChildren<Renderer>()) bawah = Mathf.Min(bawah, r.bounds.min.y);
            if (bawah != float.MaxValue) mobil.transform.position += Vector3.up * (posisi.y - bawah);
            return mobil;
        }
    }
}
