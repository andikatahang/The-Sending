using TheSending.Core;
using TheSending.Runtime;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Isi Malam 5 di Scene_RumahKeluarga (dipanggil PembuatRumahKeluarga.Buat): sutradara AlurMalam5, pohon asem di
    // pekarangan belakang (K31-A) dengan kendi M5 di kakinya dan kain kafan M6 di dahan, pacul dan galah di gudang,
    // teh di dapur, pintu kamar Laras, kalender, dan pintu depan ke desa. Jangan sunting scene dengan tangan.
    internal static class PembuatMalam5
    {
        const string Sfx = PembuatPengganti.FolderSfx;
        static readonly Vector3 Asem = new Vector3(2.2f, 0f, 18.2f);

        internal static void Pasang(Rig rig, GameObject sutradara)
        {
            var alur = sutradara.AddComponent<AlurMalam5>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "titikMulai", Titik("TitikMulaiMalam5", new Vector3(0f, 0.05f, 4f), new Vector3(0f, 0.05f, 0f)));
            Set(alur, "titikPulang", Titik("TitikPulangMalam5", new Vector3(0f, 0.05f, 0.9f), new Vector3(0f, 0.05f, 6f)));
            Set(alur, "dialogLaras", PembuatDataFase5.Muat(PembuatDataFase5.Laras));
            Set(alur, "dialogLarasTerkunci", PembuatDataFase5.Muat(PembuatDataFase5.LarasTerkunci));
            Set(alur, "dialogLarasTeh", PembuatDataFase5.Muat(PembuatDataFase5.LarasTeh));
            Set(alur, "suaraPintuDepan", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)"));
            Set(alur, "suaraGali", BuatSuara(rig, "GaliTanah", Sfx, "SFX", "(Suara: pacul menggali tanah.)"));
            Set(alur, "suaraGelas", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kamar dibuka pelan.)"));

            var kalender = PembuatRumahKeluarga.BendaDialog(rig, "Kalender", new Vector3(-1.5f, 1.25f, 8.13f),
                new Vector3(0.35f, 0.5f, 0.1f), 0f, true, PembuatDataFase5.Muat(PembuatDataFase5.KalenderMalam5), "Lihat kalender");
            PembuatMalam2.AktifPada(sutradara, 5, 5, kalender.gameObject);

            Alat(sutradara, "Pacul", Inventaris.Pacul, new Vector3(5.2f, 0f, 11.75f), new Vector3(0.3f, 1.2f, 0.2f), "Ambil pacul");
            Alat(sutradara, "Galah", Inventaris.Galah, new Vector3(4.8f, 0f, 11.75f), new Vector3(0.2f, 2.2f, 0.2f), "Ambil galah bambu");

            var teh = PembuatRumahKeluarga.BendaDialog(rig, "GelasTeh", new Vector3(-4.5f, 0.76f, 10.4f), new Vector3(0.2f, 0.15f, 0.2f),
                0f, false, PembuatDataFase5.Muat(PembuatDataFase5.AmbilTeh), "Buat teh hangat");
            SetBool(teh, "sekaliSaja", true);
            PembuatPengganti.Pasang("SisaMakanan", teh.transform, new Vector3(0.25f, 0f, 0f));
            PembuatMalam2.AktifPada(sutradara, 5, 5, teh.gameObject);

            PohonAsem(rig, sutradara, alur);
            KamarLaras(sutradara, alur);

            var depan = Logika("PintuDepanMalam5", new Vector3(0f, 0f, 0.12f), new Vector3(1.2f, 2.1f, 0.1f));
            var pintu = depan.AddComponent<InteraktifAlur>();
            SetTeks(pintu, "prompt", "Keluar ke desa");
            Set(alur, "pintuDepan", pintu);
            PembuatMalam2.AktifPada(sutradara, 5, 5, depan);
        }

        static void Alat(GameObject sutradara, string prefab, string barang, Vector3 posisi, Vector3 ukuran, string prompt)
        {
            var g = Logika(prefab, posisi, ukuran);
            PembuatPengganti.Pasang(prefab, g.transform, Vector3.zero);
            var ambil = g.AddComponent<AmbilBarang>();
            SetTeks(ambil, "barang", barang);
            SetTeks(ambil, "prompt", prompt);
            PembuatMalam2.AktifPada(sutradara, 5, 7, g);
        }

        // Pohon asem di ujung pekarangan (K31-A); pohon tetap ada sejak Malam 0, kendi dan kain kafan hanya Malam 5.
        static void PohonAsem(Rig rig, GameObject sutradara, AlurMalam5 alur)
        {
            PembuatPengganti.Pasang("PohonAsem", null, Asem);

            var media = new GameObject("MediaM5M6");
            media.transform.position = Asem;
            SetAngka(media.AddComponent<PenandaMedia>(), "media", Kode.M5);
            var penandaM6 = new GameObject("MediaM6");
            penandaM6.transform.SetParent(media.transform, false);
            penandaM6.transform.localPosition = new Vector3(0f, 3.5f, 0f);
            SetAngka(penandaM6.AddComponent<PenandaMedia>(), "media", Kode.M6);
            PembuatMalam2.AktifPada(sutradara, 5, 7, media);

            var gali = Logika("GaliKendi", Asem + new Vector3(-0.8f, 0f, -0.6f), new Vector3(0.9f, 0.4f, 0.9f));
            PembuatPengganti.Pasang("KendiTanahMerah", gali.transform, new Vector3(0f, -0.3f, 0f));
            var tumpukan = PembuatPengganti.Pasang("TumpukanTanah", gali.transform, Vector3.zero);
            Set(alur, "galiKendi", Alat(rig, gali, Inventaris.Pacul, "Gali tanah di bawah pohon asem",
                "Tanahnya keras. Aku butuh pacul dari gudang.", PembuatDataFase5.GaliKendi, tumpukan));
            PembuatMalam2.AktifPada(sutradara, 5, 5, gali);

            var kafan = Logika("KainKafanDahan", Asem + new Vector3(-0.6f, 3.5f, 0f), new Vector3(0.6f, 0.8f, 0.6f));
            var kain = PembuatPengganti.Pasang("KainKafan", kafan.transform, Vector3.zero);
            var pakai = Alat(rig, kafan, Inventaris.Galah, "Turunkan kain di dahan dengan galah",
                "Terlalu tinggi. Galah bambu di gudang mungkin sampai.", PembuatDataFase5.KainKafan, kain);
            Set(alur, "kainKafan", pakai);
            PembuatMalam2.AktifPada(sutradara, 5, 5, kafan);
        }

        static PakaiAlat Alat(Rig rig, GameObject g, string barang, string promptPunya, string pesanKurang, string dialog, GameObject hilang)
        {
            var p = g.AddComponent<PakaiAlat>();
            SetTeks(p, "barang", barang);
            SetTeks(p, "promptPunya", promptPunya);
            SetTeks(p, "pesanKurang", pesanKurang);
            SetTeks(p, "prompt", "Periksa");
            Set(p, "dialog", PembuatDataFase5.Muat(dialog));
            Set(p, "panel", rig.Dialog);
            Set(p, "hud", rig.Hud);
            SetDaftar(p, "hilangkan", new Object[] { hilang });
            return p;
        }

        // Pintu kamar Laras di sekat timur (x 3, z 5,5..6,5): tertutup selama Malam 5, Laras di dalam kamar.
        static void KamarLaras(GameObject sutradara, AlurMalam5 alur)
        {
            var g = Logika("PintuKamarLaras", new Vector3(3f, 0f, 6f), new Vector3(0.1f, 2.1f, 1f));
            Balok("DaunPintuLaras", new Vector3(3f, 1.05f, 6f), new Vector3(0.06f, 2.1f, 1f), "KayuGelap").transform.SetParent(g.transform, true);
            var pintu = g.AddComponent<InteraktifAlur>();
            SetTeks(pintu, "prompt", "Ketuk pintu kamar Laras");
            Set(alur, "pintuLaras", pintu);
            PembuatMalam2.AktifPada(sutradara, 5, 5, g);

            var laras = new GameObject("LarasMalam5");
            PembuatPengganti.Pasang("Laras", laras.transform, new Vector3(5f, 0f, 6.5f), -90f);
            PembuatMalam2.AktifPada(sutradara, 5, 5, laras);
        }
    }
}
