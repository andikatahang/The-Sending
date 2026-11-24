using TheSending.Runtime;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Isi Malam 7 di Scene_RumahKeluarga (dipanggil PembuatRumahKeluarga.Buat): sutradara AlurMalam7, Bu Sumi dan Laras
    // (jalur utama), lingkaran garam dan kelor di sekeliling ranjang Ibu, pintu depan ke Watuireng, Timeline ritual terakhir
    // di anglo dapur. Jalur Fitnah memakai Sosok Malam 2. Jangan sunting scene dengan tangan.
    internal static class PembuatMalam7
    {
        const string Sfx = PembuatPengganti.FolderSfx, Musik = PembuatPengganti.FolderMusik;
        static readonly Vector3 Ranjang = new Vector3(-6.2f, 0f, 6f);       // ranjang Ibu (PembuatRumahKeluarga.KamarIbu)
        static readonly Vector3 Anglo = new Vector3(2.612f, 0f, 11.3f);    // anglo dapur (PembuatMalam3.Ritual)
        static readonly Vector3 PintuBelakang = new Vector3(-4f, 0f, 12f);

        internal static void Pasang(Rig rig, GameObject sutradara, SosokSantet sosok, Transform ibu)
        {
            var alur = new GameObject("Sutradara Malam 7").AddComponent<AlurMalam7>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "sosok", sosok);
            Set(alur, "titikMulai", Titik("TitikMulaiMalam7", new Vector3(-4.3f, 0.05f, 5.4f), Ranjang));
            Set(alur, "titikPulang", Titik("TitikPulangMalam7", new Vector3(0f, 0.05f, 5f), new Vector3(0f, 0.05f, 8f)));
            Set(alur, "bidikRanjang", Titik("BidikRanjangMalam7", new Vector3(-3.9f, 1.8f, 6f), ibu.position));
            Set(alur, "dialogLingkaranGaram", PembuatDataFase7.Muat(PembuatDataFase7.LingkaranGaram));
            Set(alur, "dialogRitualTerakhir", PembuatDataFase7.Muat(PembuatDataFase7.RitualTerakhir));

            Tokoh(sutradara, alur);
            LingkaranGaram(alur);
            Ritual(rig, alur);

            var depan = Logika("PintuDepanMalam7", new Vector3(0f, 0f, 0.12f), new Vector3(1.2f, 2.1f, 0.1f));
            var pintu = depan.AddComponent<InteraktifAlur>();
            SetTeks(pintu, "prompt", "Berangkat ke Watuireng");
            Set(alur, "pintuDepan", pintu);
            PembuatMalam2.AktifPada(sutradara, 7, 7, depan);

            Set(alur, "suaraPintuDepan", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)"));
            Set(alur, "suaraApi", BuatSuara(rig, "ApiBesar", Sfx, "SFX", "(Suara: api berkobar besar.)", ulang: true));
            Set(alur, "suaraStinger", BuatSuara(rig, "StingerKejutan", Sfx, "SFX", "(Suara: dentuman mengejutkan.)"));
            Set(alur, "suaraDetak", BuatSuara(rig, "DetakJantung", Sfx, "SFX", "(Suara: detak jantung.)", ulang: true));
            string tegang = PembuatPengganti.AdaSuara(Musik, "MusikTegang") ? "MusikTegang" : "DroneHoror";
            Set(alur, "musikTegang", BuatSuara(rig, tegang, Musik, "Music", "(Musik tegang.)", ulang: true));
        }

        // Bu Sumi (bisa diajak bicara; dipindah AlurMalam7 ke dapur saat pulang) dan Laras di kamar Ibu. Malam 7 saja;
        // AlurMalam7 mematikan keduanya di jalur Fitnah.
        static void Tokoh(GameObject sutradara, AlurMalam7 alur)
        {
            var sumi = Logika("BuSumiMalam7", new Vector3(-5f, 0f, 7.55f), new Vector3(0.5f, 1.7f, 0.5f));
            sumi.transform.LookAt(Ranjang);
            PembuatPengganti.Pasang("BuSumi", sumi.transform, Vector3.zero);
            var bicara = sumi.AddComponent<InteraktifAlur>();
            SetTeks(bicara, "prompt", "Bicara dengan Bu Sumi");
            Set(alur, "buSumi", bicara);
            Set(alur, "titikBuSumiDapur", Titik("TitikBuSumiDapur", new Vector3(2.6f, 0f, 10.5f), Anglo));

            var laras = new GameObject("LarasMalam7");
            laras.transform.position = new Vector3(-4.8f, 0f, 4.55f);
            laras.transform.LookAt(Ranjang);
            PembuatPengganti.Pasang("Laras", laras.transform, Vector3.zero);
            Set(alur, "laras", laras);
            PembuatMalam2.AktifPada(sutradara, 7, 7, sumi, laras);
        }

        // Game Story Malam 7 butir 1: lingkaran garam (kubus putih kecil) dan ikat kelor di lantai sekeliling ranjang Ibu.
        // Ranjang rapat ke dinding barat, jadi titik di balik dinding ditarik ke x -6,9. Tampil sejak adegan (AlurMalam7).
        static void LingkaranGaram(AlurMalam7 alur)
        {
            var lingkaran = new GameObject("LingkaranGaram");
            const int jumlah = 14;
            for (int i = 0; i < jumlah; i++)
            {
                float a = i * Mathf.PI * 2f / jumlah;
                var p = new Vector3(Mathf.Max(Ranjang.x + 0.95f * Mathf.Cos(a), -6.9f), 0.025f, Ranjang.z + 1.3f * Mathf.Sin(a));
                var garam = Balok("Garam", p, new Vector3(0.05f, 0.05f, 0.05f), "BatuPutih");
                Object.DestroyImmediate(garam.GetComponent<Collider>());   // tidak mengganjal langkah atau NavMesh
                garam.transform.SetParent(lingkaran.transform, true);
            }
            foreach (float a in new[] { 0f, 90f, 270f, 315f, 45f })
            {
                float r = a * Mathf.Deg2Rad;
                var p = new Vector3(Mathf.Max(Ranjang.x + 1.05f * Mathf.Cos(r), -6.85f), 0f, Ranjang.z + 1.4f * Mathf.Sin(r));
                PembuatPengganti.Pasang("IkatKelor", lingkaran.transform, p, a);
            }
            Set(alur, "lingkaranGaram", lingkaran);
            lingkaran.SetActive(false);
        }

        // Ritual terakhir di anglo dapur (sekitar 8 detik): kamera ke anglo, api induk menyala, Sosok menjerit di ambang
        // pintu belakang dapur (dapur tanpa jendela), lalu lenyap; kamera kembali ke anglo.
        static void Ritual(Rig rig, AlurMalam7 alur)
        {
            var kendi = PembuatPengganti.Pasang("KendiInduk", null, Anglo + new Vector3(0f, 0.31f, 0f));
            kendi.transform.localScale = Vector3.one * 0.6f;
            Set(alur, "kendiDiAnglo", kendi);
            kendi.SetActive(false);

            var api = PembuatPengganti.Pasang("ApiAnglo", null, Anglo + new Vector3(0f, 0.25f, 0f));
            api.name = "ApiInduk";
            api.transform.localScale = Vector3.one * 1.8f;
            Lampu("CahayaApiInduk", Anglo + new Vector3(0f, 1f, 0f), "FF8A3A", 5f, 2.5f).transform.SetParent(api.transform, true);
            Set(alur, "apiInduk", api);
            api.SetActive(false);

            var sosok = PembuatPengganti.Pasang("SosokSantet", null, PintuBelakang + new Vector3(0f, 0f, 0.9f), 180f);
            sosok.name = "SosokJendelaDapur";
            Set(alur, "sosokJendela", sosok);
            sosok.SetActive(false);

            var bidikAnglo = Titik("BidikAngloMalam7", new Vector3(1.2f, 1.7f, 9.3f), Anglo + Vector3.up * 0.4f);
            Set(alur, "bidikAnglo", bidikAnglo);
            var dekat = Titik("BidikAngloDekat", new Vector3(1.6f, 1.2f, 10.2f), Anglo + Vector3.up * 0.4f);
            var pintu = Titik("BidikPintuBelakang", new Vector3(-2f, 1.6f, 9.5f), PintuBelakang + new Vector3(0f, 1.6f, 0.9f));
            var c = PembuatTimeline.Buat("Malam7_RitualTerakhir", 8f);
            PembuatTimeline.Gerak(c.Tl, c.Director, rig.KameraCutscene.gameObject,
                (0f, bidikAnglo), (3.99f, dekat), (4f, pintu), (6.5f, pintu), (6.51f, dekat), (8f, bidikAnglo));
            PembuatTimeline.Sinyal(c.Tl, 1.5f, AlurMalam7.SinyalApi);
            PembuatTimeline.Sinyal(c.Tl, 4.05f, AlurMalam7.SinyalJerit);
            PembuatTimeline.Sinyal(c.Tl, 6.4f, AlurMalam7.SinyalLenyap);
            PembuatTimeline.Simpan(c.Tl);
            Set(alur, "timelineRitual", c.Pemutar);
        }
    }
}
