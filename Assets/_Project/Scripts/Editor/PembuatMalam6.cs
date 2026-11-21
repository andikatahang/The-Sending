using TheSending.Runtime;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Isi Malam 6 di Scene_RumahKeluarga (dipanggil PembuatRumahKeluarga.Buat): sutradara AlurMalam6, Ibu kerasukan
    // (K39-A), pintu depan ke desa, dan Timeline Laras berlari masuk saat 21.00. Jangan sunting scene dengan tangan.
    internal static class PembuatMalam6
    {
        const string Sfx = PembuatPengganti.FolderSfx;

        internal static void Pasang(Rig rig, GameObject sutradara, Transform ibu, PakaiAlat kainKafan)
        {
            var alur = new GameObject("Sutradara Malam 6").AddComponent<AlurMalam6>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "titikMulai", Titik("TitikMulaiMalam6", new Vector3(-4.3f, 0.05f, 5.4f), new Vector3(-6.2f, 0.05f, 6f)));
            Set(alur, "titikPulang", Titik("TitikPulangMalam6", new Vector3(0f, 0.05f, 0.9f), new Vector3(0f, 0.05f, 6f)));
            Set(alur, "bidikRanjang", Titik("BidikRanjangMalam6", new Vector3(-4.3f, 1.7f, 5.2f), new Vector3(-6.2f, 0.7f, 6f)));
            var bidikPintu = Titik("BidikPintuDepanMalam6", new Vector3(0.4f, 1.6f, 5f), new Vector3(0f, 1.3f, 0.5f));
            Set(alur, "bidikPintuDepan", bidikPintu);
            Set(alur, "ibu", ibu);
            Set(alur, "kainKafan", kainKafan);
            var lampu = GameObject.Find("Lampu kamar Ibu").GetComponent<Light>();
            SetDaftar(alur, "lampuKamar", new Object[] { lampu });
            Set(alur, "dialogKerasukan", PembuatDataFase6.Muat(PembuatDataFase6.Kerasukan));
            Set(alur, "dialogLarasMasuk", PembuatDataFase6.Muat(PembuatDataFase6.LarasMasuk));
            Set(alur, "dialogLarasMasukTanpaObat", PembuatDataFase6.Muat(PembuatDataFase6.LarasMasukTanpaObat));
            Set(alur, "suaraPintuDepan", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)"));
            Set(alur, "suaraGeram", BuatSuara(rig, "GeramIbu", Sfx, "SFX", "(Suara: geram berat dari ranjang Ibu.)"));
            Set(alur, "suaraLangkahLari", BuatSuara(rig, "LangkahLariKayu", Sfx, "SFX", "(Suara: langkah berlari.)"));

            var depan = Logika("PintuDepanMalam6", new Vector3(0f, 0f, 0.12f), new Vector3(1.2f, 2.1f, 0.1f));
            var pintu = depan.AddComponent<InteraktifAlur>();
            SetTeks(pintu, "prompt", "Keluar ke desa");
            Set(alur, "pintuDepan", pintu);
            PembuatMalam2.AktifPada(sutradara, 6, 6, depan);

            // Laras berlari dari luar pintu depan ke ruang tengah (AlurMalam6 menyalakannya saat 21.00).
            var dari = Titik("LarasMasukDari", new Vector3(0f, 0f, -1.5f), new Vector3(0f, 0f, 3f));
            var ke = Titik("LarasMasukKe", new Vector3(0f, 0f, 3f), new Vector3(0f, 0f, 6f));
            var laras = PembuatPengganti.Pasang("Laras", null, dari.position, 0f);
            laras.name = "LarasMasukMalam6";
            var c = PembuatTimeline.Buat("Malam6_LarasMasuk", 3f);
            PembuatTimeline.Gerak(c.Tl, c.Director, laras, (0f, dari), (2.2f, ke));
            PembuatTimeline.Sinyal(c.Tl, 0.1f, AlurMalam6.SinyalLangkahLaras);
            PembuatTimeline.Simpan(c.Tl);
            Set(alur, "timelineLarasMasuk", c.Pemutar);
            Set(alur, "larasMasuk", laras);
            laras.SetActive(false);
        }
    }
}
