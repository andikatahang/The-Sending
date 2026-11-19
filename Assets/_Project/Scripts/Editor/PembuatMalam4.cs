using TheSending.Core;
using TheSending.Runtime;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Isi Malam 4 di Scene_RumahKeluarga (dipanggil PembuatRumahKeluarga.Buat): sutradara AlurMalam4, sumur dan kerekan di
    // pekarangan belakang (M3), loteng dapur dengan telur cemani (M4), tangga bambu di gudang, kalender Halaman 5, kejaran
    // pertama (lolos = masuk pintu belakang dapur, K29-A), dua warga, dan pintu depan ke pos ronda. Jangan sunting scene dengan tangan.
    internal static class PembuatMalam4
    {
        const string Sfx = PembuatPengganti.FolderSfx, Musik = PembuatPengganti.FolderMusik;
        static readonly Vector3 Sumur = new Vector3(1.5f, 0f, 15.5f);
        static readonly Vector3 Lubang = new Vector3(1.5f, 0f, 10f);   // lubang plafon dari PembuatRumahKeluarga.Bangunan
        const float LantaiLoteng = 3.2f;

        internal static void Pasang(Rig rig, GameObject sutradara, SosokSantet sosok)
        {
            var alur = sutradara.AddComponent<AlurMalam4>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "dialogWargaMenuduh", PembuatDataFase4.Muat(PembuatDataFase4.WargaMenuduh));
            Set(alur, "sosok", sosok);
            // Bacaan tempo tarikan kerekan (K27-A): canvas sendiri di bawah sutradara, kosong selain saat menarik.
            var kanvasTempo = UiPabrik.BuatCanvas("TempoUI", 10);
            kanvasTempo.transform.SetParent(sutradara.transform, false);
            Set(alur, "teksTempo", UiPabrik.BuatTeks(kanvasTempo.transform, "Tempo", 28, TextAnchor.LowerCenter,
                new Vector2(0.3f, 0.1f), new Vector2(0.7f, 0.16f)));
            Set(alur, "titikMulai", Titik("TitikMulaiMalam4", new Vector3(0f, 0.05f, 4f), new Vector3(0f, 0.05f, 0f)));
            Set(alur, "titikPulang", Titik("TitikPulangMalam4", new Vector3(0f, 0.05f, 0.9f), new Vector3(0f, 0.05f, 6f)));

            var kalender4 = PembuatRumahKeluarga.BendaDialog(rig, "Kalender", new Vector3(-1.5f, 1.25f, 8.13f),
                new Vector3(0.35f, 0.5f, 0.1f), 0f, true, PembuatDataFase4.Muat(PembuatDataFase4.KalenderMalam4), "Lihat kalender");
            PembuatMalam2.AktifPada(sutradara, 4, 4, kalender4.gameObject);

            var tangga = Logika("TanggaBambu", new Vector3(5.6f, 0f, 11.75f), new Vector3(0.7f, 2.5f, 0.2f));
            PembuatPengganti.Pasang("TanggaBambu", tangga.transform, Vector3.zero);
            var ambil = tangga.AddComponent<AmbilBarang>();
            SetTeks(ambil, "barang", Inventaris.TanggaBambu);
            SetTeks(ambil, "prompt", "Ambil tangga bambu");
            PembuatMalam2.AktifPada(sutradara, 4, 7, tangga);

            Loteng(rig, sutradara);
            SumurDanKejaran(rig, alur);
            PintuDepan(sutradara, alur);
            Set(alur, "suaraPintuDepan", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)"));
            Set(alur, "suaraStinger", BuatSuara(rig, "StingerKejutan", Sfx, "SFX", "(Suara: dentuman mengejutkan.)"));
            Set(alur, "suaraDetak", BuatSuara(rig, "DetakJantung", Sfx, "SFX", "(Suara: detak jantung.)", ulang: true));
            string tegang = PembuatPengganti.AdaSuara(Musik, "MusikKejar") ? "MusikKejar" : "DroneHoror";
            Set(alur, "musikTegang", BuatSuara(rig, tegang, Musik, "Music", "(Musik tegang.)", ulang: true));
        }

        // Loteng di atas dapur dan gudang (K28-A): NaikPlafon memindahkan Raka lewat lubang plafon; telur cemani M4 di pojok barat.
        static void Loteng(Rig rig, GameObject sutradara)
        {
            PembuatPengganti.Pasang("PlafonAnyaman", null, new Vector3(Lubang.x, LantaiLoteng - 0.22f, Lubang.z));
            const float tinggi = 2.2f, yDinding = LantaiLoteng + tinggi / 2f;
            Balok("DindingLoteng", new Vector3(0f, yDinding, 8f), new Vector3(14f, tinggi, 0.15f), "Gedek");
            Balok("DindingLoteng", new Vector3(0f, yDinding, 12f), new Vector3(14f, tinggi, 0.15f), "Gedek");
            Balok("DindingLoteng", new Vector3(-7f, yDinding, 10f), new Vector3(0.15f, tinggi, 4f), "Gedek");
            Balok("DindingLoteng", new Vector3(7f, yDinding, 10f), new Vector3(0.15f, tinggi, 4f), "Gedek");
            Balok("AtapLoteng", new Vector3(0f, LantaiLoteng + tinggi + 0.1f, 10f), new Vector3(14f, 0.2f, 4f), "KayuGelap");

            // Tutup lubang plafon 1 x 1 m sampai Malam 4 (Malam 0-3).
            var tutup = Balok("TutupLubangPlafon", new Vector3(Lubang.x, PembuatRumahKeluarga.Tinggi + 0.1f, Lubang.z), new Vector3(1f, 0.2f, 1f), "KayuGelap");
            PembuatMalam2.AktifPada(sutradara, 0, 3, tutup);

            // Collider logika mengisi lubang (y 2..3,5): terbidik dari dapur (mendongak) dan dari loteng (menunduk).
            var g = Logika("NaikPlafon", new Vector3(Lubang.x, 2.4f, Lubang.z), new Vector3(0.9f, 1.5f, 0.9f));
            var naik = g.AddComponent<NaikPlafon>();
            Set(naik, "titikAtas", Titik("TitikAtasPlafon", new Vector3(Lubang.x, LantaiLoteng + 0.05f, 8.8f),
                new Vector3(-3f, LantaiLoteng + 0.05f, 10.5f)));
            Set(naik, "titikBawah", Titik("TitikBawahPlafon", new Vector3(Lubang.x, 0.05f, 8.9f), new Vector3(Lubang.x, 0.05f, 11f)));
            Set(naik, "kartu", rig.Kartu);
            Set(naik, "pemain", rig.Pemain);
            Set(naik, "hud", rig.Hud);
            PembuatMalam2.AktifPada(sutradara, 4, 4, g);

            var telur = PembuatRumahKeluarga.BendaDialog(rig, "TelurCemani", new Vector3(-3f, LantaiLoteng, 10.5f), new Vector3(0.3f, 0.2f, 0.3f),
                0f, false, PembuatDataFase4.Muat(PembuatDataFase4.TelurCemani), "Periksa telur hitam");
            SetBool(telur, "sekaliSaja", true);
            PembuatMalam2.AktifPada(sutradara, 4, 4, telur.gameObject);
        }

        // Sumur di pekarangan belakang (Game Story Malam 4), Sosok keluar dari arah sumur, warga di luar pintu belakang.
        static void SumurDanKejaran(Rig rig, AlurMalam4 alur)
        {
            var sumur = Logika("Sumur", Sumur, new Vector3(1.3f, 1.8f, 1.3f));
            PembuatPengganti.Pasang("SumurTimba", sumur.transform, Vector3.zero);
            var bungkusan = PembuatPengganti.Pasang("BungkusanHitam", sumur.transform, new Vector3(0f, -3.2f, 0f));   // naik 4 m ke bibir sumur
            var kerekan = sumur.AddComponent<TheSending.Runtime.KerekanSumur>();
            Set(kerekan, "kerekanDerit", BuatSuara(rig, "KerekanDerit", Sfx, "SFX", "(Suara: kerekan berderit.)"));
            Set(kerekan, "taliDitarik", BuatSuara(rig, "TaliDitarik", Sfx, "SFX", "(Suara: tali ditarik.)"));
            Set(kerekan, "bendaMasukAir", BuatSuara(rig, "BendaMasukAir", Sfx, "SFX", "(Suara: benda jatuh ke air.)"));
            Set(kerekan, "titikBungkusan", bungkusan.transform);
            Set(kerekan, "hud", rig.Hud);
            SetTeks(kerekan, "prompt", "Periksa sumur");
            Set(alur, "kerekan", kerekan);

            Set(alur, "titikSumur", Titik("TitikSumurMalam4", new Vector3(0.5f, 0.05f, 14.3f), new Vector3(-4f, 0.05f, 12f)));
            var muncul = Titik("TitikMunculSosokMalam4", new Vector3(1.5f, 0f, 18f), new Vector3(-4f, 0f, 12f));
            Set(alur, "titikMunculSosok", muncul);
            Set(alur, "pintuBelakang", Zona("ZonaLolosPintuBelakang", new Vector3(-4f, 1.2f, 11.5f), new Vector3(1.2f, 2.4f, 0.6f)));

            // Cutscene kejar: kamera dari dekat sumur mendongak ke arah Sosok muncul.
            var dari = Titik("BidikKejarDari", new Vector3(0f, 1.6f, 14f), Sumur + Vector3.up * 1.2f);
            var ke = Titik("BidikKejarKe", new Vector3(0.6f, 1.5f, 14.8f), muncul.position + Vector3.up * 1.8f);
            var c = PembuatTimeline.Buat("Malam4_Kejar", 3f);
            PembuatTimeline.Gerak(c.Tl, c.Director, rig.KameraCutscene.gameObject, (0f, dari), (3f, ke));
            PembuatTimeline.Simpan(c.Tl);
            Set(alur, "cutsceneKejar", c.Pemutar);

            var warga = new GameObject("WargaMalam4");
            PembuatPengganti.Pasang("Warga1", warga.transform, new Vector3(-4.6f, 0f, 13.4f), 180f);
            PembuatPengganti.Pasang("Warga2", warga.transform, new Vector3(-3.4f, 0f, 13.6f), 180f);
            Set(alur, "warga", warga);
            warga.SetActive(false);
            Set(alur, "titikWarga", Titik("BidikWarga", new Vector3(-4f, 1.6f, 11f), new Vector3(-4f, 1.5f, 13.5f)));
        }

        // Pintu depan Malam 4 ke pos ronda (K26-A); PintuDepanMalam3 hanya aktif di Malam 3.
        static void PintuDepan(GameObject sutradara, AlurMalam4 alur)
        {
            var g = Logika("PintuDepanMalam4", new Vector3(0f, 0f, 0.12f), new Vector3(1.2f, 2.1f, 0.1f));
            var pintu = g.AddComponent<InteraktifAlur>();
            SetTeks(pintu, "prompt", "Pintu depan");
            Set(alur, "pintuDepan", pintu);
            PembuatMalam2.AktifPada(sutradara, 4, 4, g);
        }
    }
}
