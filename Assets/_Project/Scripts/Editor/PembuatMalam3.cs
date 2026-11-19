using TheSending.Core;
using TheSending.Runtime;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Isi Malam 3 di Scene_RumahKeluarga (dipanggil PembuatRumahKeluarga.Buat): sutradara AlurMalam3, Pakde Darmo, kotak
    // sepatu (Halaman 3), saka guru dan tujuh paku, tang dan Halaman 4 di gudang, bahan dan anglo Ritual Penangkal,
    // pekarangan belakang (pohon kelor, tempat mengubur abu), pintu depan ke jalan desa, dan suara.
    // Denah tetap (K7) kecuali pintu belakang dapur (K18). Jangan sunting scene dengan tangan.
    internal static class PembuatMalam3
    {
        const string Sfx = PembuatPengganti.FolderSfx, Musik = PembuatPengganti.FolderMusik;
        const string WarnaBulan = "6D86A8", WarnaApi = "FF8A3D";
        // Saka guru dari PembuatRumahKeluarga.Bangunan; nomor = indeks (TujuhPaku.SakaBenar = 3).
        static readonly Vector3[] Saka =
        {
            new Vector3(-1.5f, 0f, 2.5f), new Vector3(-1.5f, 0f, 5.5f), new Vector3(1.5f, 0f, 2.5f), new Vector3(1.5f, 0f, 5.5f),
        };
        // Ukiran tujuh paku dari atas ke bawah; sengaja tidak urut supaya pemain membaca ukirannya.
        static readonly Hari[] UkiranPaku = { Hari.Selasa, Hari.Jumat, Hari.Minggu, Hari.Kamis, Hari.Sabtu, Hari.Rabu, Hari.Senin };

        internal static void Pasang(Rig rig, GameObject sutradara, SosokSantet sosok, InteraktifAlur pintuGudang, GameObject pintuBelakang,
            Transform ibu)
        {
            var alur = sutradara.AddComponent<AlurMalam3>();
            Set(alur, "adegan", rig.Adegan);
            Set(alur, "kartu", rig.Kartu);
            Set(alur, "dialog", rig.Dialog);
            Set(alur, "hud", rig.Hud);
            Set(alur, "pemain", rig.Pemain);
            Set(alur, "dialogPakde", PembuatDataFase3.Muat(PembuatDataFase3.PakdeDatang));
            Set(alur, "pintuGudang", pintuGudang);
            Set(alur, "sosok", sosok);
            Set(alur, "ibu", ibu);
            Set(alur, "titikMulai", Titik("TitikMulaiMalam3", new Vector3(0f, 0.05f, 4f), new Vector3(0f, 0.05f, 0f)));
            Set(alur, "titikRuangTengah", Titik("TitikRuangTengahMalam3", new Vector3(0f, 0.05f, 3.2f), new Vector3(1.5f, 0.05f, 5.5f)));
            Set(alur, "titikPulang", Titik("TitikPulangMalam3", new Vector3(0f, 0.05f, 0.9f), new Vector3(0f, 0.05f, 6f)));
            Set(alur, "titikMunculSosok", Titik("TitikMunculSosokMalam3", new Vector3(-0.5f, 0f, 10.5f), new Vector3(-0.5f, 0f, 6f)));
            Set(alur, "bidikIbu", Titik("BidikIbuMalam3", new Vector3(-4.3f, 1.7f, 5.2f), new Vector3(-6.2f, 0.7f, 6f)));

            PembuatMalam2.AktifPada(sutradara, 0, 2, pintuBelakang);   // pintu belakang terbuka mulai Malam 3 (K18)
            var kalender3 = PembuatRumahKeluarga.BendaDialog(rig, "Kalender", new Vector3(-1.5f, 1.25f, 8.13f),
                new Vector3(0.35f, 0.5f, 0.1f), 0f, true, PembuatDataFase3.Muat(PembuatDataFase3.KalenderMalam3), "Lihat kalender");
            PembuatMalam2.AktifPada(sutradara, 3, 3, kalender3.gameObject);

            Pakde(alur);
            KamarRaka(rig, sutradara, alur);
            SakaDanPaku(rig, sutradara, alur);
            Gudang(rig, sutradara);
            Ritual(rig);
            PintuDepan(sutradara, alur);
            SuaraMalam3(rig, alur);
        }

        // Pakde Darmo berdiri di samping sofa, buah tangannya di meja ruang tengah (Game Story Malam 3 butir 1).
        static void Pakde(AlurMalam3 alur)
        {
            var pakde = Logika("PakdeDarmo", new Vector3(-1.2f, 0f, 1.5f), new Vector3(0.5f, 1.7f, 0.5f));
            PembuatPengganti.Pasang("PakdeDarmo", pakde.transform, Vector3.zero, 50f);   // menghadap Raka di TitikTamuPakde
            Set(alur, "pakde", pakde);
            pakde.SetActive(false);
            var buah = PembuatPengganti.Pasang("BuahTangan", null, new Vector3(-2.2f, 0.76f, 3.6f), 90f);
            Set(alur, "buahTangan", buah);
            buah.SetActive(false);
            Set(alur, "titikTamu", Titik("TitikTamuPakde", new Vector3(0.2f, 0.05f, 2.6f), new Vector3(-1.2f, 0.05f, 1.5f)));
            Set(alur, "bidikPakde", Titik("BidikPakde", new Vector3(0.4f, 1.6f, 2.9f), new Vector3(-1.2f, 1.45f, 1.5f)));
        }

        // Kotak sepatu lama di kamar Raka berisi Halaman 3 (Game Story Malam 3 butir 2).
        static void KamarRaka(Rig rig, GameObject sutradara, AlurMalam3 alur)
        {
            var kotak = PembuatRumahKeluarga.BendaDialog(rig, "KotakSepatu", new Vector3(-4.4f, 0f, 3.55f), new Vector3(0.4f, 0.2f, 0.3f), 0f, false,
                PembuatDataFase3.Muat(PembuatDataFase3.KotakSepatu), "Buka kotak sepatu lama");
            SetBool(kotak, "sekaliSaja", true);
            PembuatMalam2.AktifPada(sutradara, 3, 3, kotak.gameObject);
            Set(alur, "titikKamarRaka", Titik("TitikKamarRakaMalam3", new Vector3(-4.2f, 0.05f, 2.2f), new Vector3(-4.4f, 0.05f, 3.55f)));
        }

        // Puzzle mencari saka dan tujuh paku (Game Story Malam 3 butir 3-6).
        static void SakaDanPaku(Rig rig, GameObject sutradara, AlurMalam3 alur)
        {
            var padat = BuatSuara(rig, "KetukPadat", Sfx, "SFX", "(Suara: ketukan kayu padat.)");
            var berongga = BuatSuara(rig, "KetukBerongga", Sfx, "SFX", "(Suara: ketukan kayu berongga.)");
            for (int i = 0; i < Saka.Length; i++)
            {
                // Sedikit lebih besar dari collider saka (0,3 m) supaya sinar E mengenai logika ini lebih dulu.
                var g = Logika("KetukSaka " + i, Saka[i], new Vector3(0.4f, 2.6f, 0.4f));
                var k = g.AddComponent<KetukSaka>();
                SetAngka(k, "nomor", i);
                Set(k, "hud", rig.Hud);
                Set(k, "padat", padat);
                Set(k, "berongga", berongga);
                SetTeks(k, "prompt", "Ketuk saka guru");
            }

            // Kompas mengarah ke saka yang benar mulai Malam 3; PenandaMedia mengabaikan media yang sudah ditemukan.
            var benar = Saka[TujuhPaku.SakaBenar];
            var penanda = new GameObject("MediaM2");
            penanda.transform.position = benar + Vector3.up * 1.3f;
            SetAngka(penanda.AddComponent<PenandaMedia>(), "media", Kode.M2);
            PembuatMalam2.AktifPada(sutradara, 3, 7, penanda);

            // Dempul hitam di sisi barat saka (menghadap tengah ruang); tujuh paku di baliknya, menonjol 7 cm.
            float x = benar.x - 0.16f;
            Set(alur, "dempul", PembuatPengganti.Pasang("Dempul", null, new Vector3(x, 1.35f, benar.z)));
            var kelompok = new GameObject("TujuhPaku");
            var daftar = new Object[UkiranPaku.Length];
            for (int i = 0; i < UkiranPaku.Length; i++)
            {
                float y = 1.8f - i * 0.15f;
                var g = Logika("Paku " + UkiranPaku[i], new Vector3(x - 0.02f, y - 0.05f, benar.z), new Vector3(0.12f, 0.1f, 0.22f));
                g.transform.SetParent(kelompok.transform, true);
                var visual = PembuatPengganti.Pasang("PakuSaka", g.transform, new Vector3(0f, 0.05f, 0f));
                var p = g.AddComponent<PakuDiSaka>();
                SetAngka(p, "hari", (int)UkiranPaku[i]);
                Set(p, "visual", visual);
                Set(p, "hud", rig.Hud);
                daftar[i] = p;
            }
            SetDaftar(alur, "paku", daftar);
            Set(alur, "pakuSaka", kelompok);
            kelompok.SetActive(false);
        }

        // Tang dan Halaman 4 di rak alat gudang (Game Story Malam 3 butir 5), bilah bambu kuning bersandar di dinding (desain 4.5).
        static void Gudang(Rig rig, GameObject sutradara)
        {
            var tang = Logika("Tang", new Vector3(4.4f, 1.03f, 11.65f), new Vector3(0.25f, 0.12f, 0.3f));
            PembuatPengganti.Pasang("Tang", tang.transform, Vector3.zero);
            var ambil = tang.AddComponent<AmbilBarang>();
            SetTeks(ambil, "barang", Inventaris.Tang);
            SetTeks(ambil, "prompt", "Ambil tang");

            var halaman = PembuatRumahKeluarga.BendaDialog(rig, "HalamanSobek", new Vector3(3.9f, 1.03f, 11.65f), new Vector3(0.2f, 0.08f, 0.25f), 0f,
                false, PembuatDataFase3.Muat(PembuatDataFase3.Halaman4), "Ambil halaman buku harian");
            SetBool(halaman, "sekaliSaja", true);
            PembuatMalam2.AktifPada(sutradara, 3, 3, halaman.gameObject);

            Bahan(rig, "BambuKuning", BahanRitual.BambuKuning, new Vector3(6.75f, 0f, 9.3f), new Vector3(0.3f, 1.2f, 0.3f),
                "Ambil bilah bambu kuning", "Bilah bambu kuning diambil.");
        }

        // Ritual Penangkal (Game Story 6.5, desain 4.5, K25-B): garam diambil dari rak bumbu dapur; anglo dan meja kecil di
        // sampingnya berisi benda-benda langkah ritual; pekarangan belakang (K18-A) untuk kelor dan mengubur abu.
        static void Ritual(Rig rig)
        {
            Bahan(rig, "RakBumbu", BahanRitual.Garam, new Vector3(-5f, 0.76f, 10.5f), new Vector3(0.5f, 0.2f, 0.3f),
                "Ambil garam", "Garam diambil dari rak bumbu.");

            var dapur = new GameObject("RitualDapur").AddComponent<RitualDapur>();
            Set(dapur, "hud", rig.Hud);
            Set(dapur, "suaraApi", BuatSuara(rig, "ApiAnglo", Sfx, "SFX", "(Suara: api arang menyala.)", ulang: true));

            var anglo = Logika("Anglo", new Vector3(-2.6f, 0f, 11.3f), new Vector3(0.5f, 0.45f, 0.5f));
            PembuatPengganti.Pasang("Anglo", anglo.transform, Vector3.zero);
            PasangLangkah(dapur, anglo, LangkahRitual.LetakkanMedia, "Anglo");
            var api = PembuatPengganti.Pasang("ApiAnglo", anglo.transform, Vector3.zero);
            Lampu("CahayaApi", new Vector3(-2.6f, 0.8f, 11.3f), WarnaApi, 4f, 1.8f).transform.SetParent(api.transform, true);
            Set(dapur, "api", api);
            api.SetActive(false);

            // Meja kecil di samping anglo: garam, kelor, korek di atasnya, bambu kuning bersandar. Tampak setelah bahan lengkap.
            PembuatPengganti.Pasang("Meja", null, new Vector3(-1.5f, 0f, 11.4f));
            var perlengkapan = new GameObject("PerlengkapanRitual");
            (string prefab, LangkahRitual langkah, Vector3 posisi, Vector3 ukuran, string prompt)[] benda =
            {
                ("GaramToples", LangkahRitual.TaburGaram, new Vector3(-1.85f, 0.76f, 11.4f), new Vector3(0.2f, 0.2f, 0.2f), "Taburkan garam"),
                ("IkatKelor", LangkahRitual.TutupKelor, new Vector3(-1.55f, 0.76f, 11.4f), new Vector3(0.25f, 0.15f, 0.25f), "Tutup dengan daun kelor"),
                ("KorekApi", LangkahRitual.NyalakanApi, new Vector3(-1.25f, 0.76f, 11.4f), new Vector3(0.2f, 0.1f, 0.2f), "Nyalakan api"),
                ("BambuKuning", LangkahRitual.AdukBambu, new Vector3(-0.85f, 0f, 11.55f), new Vector3(0.25f, 1.2f, 0.25f), "Aduk dengan bilah bambu kuning"),
            };
            foreach (var (prefab, langkah, posisi, ukuran, prompt) in benda)
            {
                var g = Logika(prefab + "Ritual", posisi, ukuran);
                PembuatPengganti.Pasang(prefab, g.transform, Vector3.zero);
                PasangLangkah(dapur, g, langkah, prompt);
                g.transform.SetParent(perlengkapan.transform, true);
            }
            Set(dapur, "perlengkapan", perlengkapan);
            perlengkapan.SetActive(false);

            Pekarangan(rig, dapur);
        }

        static void PasangLangkah(RitualDapur dapur, GameObject g, LangkahRitual langkah, string prompt)
        {
            var b = g.AddComponent<BendaRitual>();
            Set(b, "dapur", dapur);
            SetAngka(b, "langkah", (int)langkah);
            SetTeks(b, "prompt", prompt);
        }

        // Pekarangan belakang 10 x 7 m (x -7..3, z 12..19) berpagar bambu, diterangi bulan. Fase 4 menaruh sumur di sini.
        static void Pekarangan(Rig rig, RitualDapur dapur)
        {
            Balok("TanahPekarangan", new Vector3(-2f, -0.1f, 15.5f), new Vector3(10f, 0.2f, 7f), "Tanah")
                .AddComponent<Permukaan>().Jenis = JenisPermukaan.Tanah;
            Batas("BatasPekarangan", new Vector3(-7.1f, 1.5f, 15.5f), new Vector3(0.2f, 3f, 7f));
            Batas("BatasPekarangan", new Vector3(3.1f, 1.5f, 15.5f), new Vector3(0.2f, 3f, 7f));
            Batas("BatasPekarangan", new Vector3(-2f, 1.5f, 19.1f), new Vector3(10f, 3f, 0.2f));
            foreach (float px in new[] { -6.5f, -3f, 0.5f, 2.6f })
                PembuatPengganti.Pasang("RumpunBambu", null, new Vector3(px, 0f, 18.7f));
            var bulan = new GameObject("Cahaya bulan pekarangan").AddComponent<Light>();
            bulan.type = LightType.Spot;
            bulan.color = Hex(WarnaBulan);
            bulan.range = 9f;
            bulan.spotAngle = 80f;
            bulan.intensity = 1.5f;
            bulan.transform.SetPositionAndRotation(new Vector3(-3f, 6f, 15f), Quaternion.Euler(90f, 0f, 0f));

            Bahan(rig, "PohonKelor", BahanRitual.Kelor, new Vector3(-5.5f, 0f, 16f), new Vector3(0.8f, 2.6f, 0.8f),
                "Petik daun kelor", "Daun kelor dipetik.");

            var kubur = Logika("TanahKuburAbu", new Vector3(-1.5f, 0f, 15f), new Vector3(1f, 0.15f, 1f));
            Balok("GundukanTanah", new Vector3(-1.5f, 0.03f, 15f), new Vector3(0.9f, 0.06f, 0.9f), "Tanah").transform.SetParent(kubur.transform, true);
            var k = kubur.AddComponent<KuburAbu>();
            Set(k, "dapur", dapur);
            Set(k, "hud", rig.Hud);
            SetTeks(k, "prompt", "Kubur abu di sini");
        }

        static void Bahan(Rig rig, string prefab, BahanRitual bahan, Vector3 posisi, Vector3 ukuran, string prompt, string pesan)
        {
            var g = Logika(prefab, posisi, ukuran);
            PembuatPengganti.Pasang(prefab, g.transform, Vector3.zero);
            var a = g.AddComponent<AmbilBahan>();
            SetAngka(a, "bahan", (int)bahan);
            Set(a, "hud", rig.Hud);
            SetTeks(a, "pesan", pesan);
            SetTeks(a, "prompt", prompt);
        }

        // Pintu depan hanya bisa dipakai di Malam 3 (K17-B, ke jalan desa): logika tipis di depan daun pintu yang tertutup.
        static void PintuDepan(GameObject sutradara, AlurMalam3 alur)
        {
            var g = Logika("PintuDepanMalam3", new Vector3(0f, 0f, 0.12f), new Vector3(1.2f, 2.1f, 0.1f));
            var pintu = g.AddComponent<InteraktifAlur>();
            SetTeks(pintu, "prompt", "Pintu depan");
            Set(alur, "pintuDepan", pintu);
            PembuatMalam2.AktifPada(sutradara, 3, 3, g);
        }

        static void SuaraMalam3(Rig rig, AlurMalam3 alur)
        {
            Set(alur, "suaraMotorDatang", BuatSuara(rig, "MotorDatang", Sfx, "SFX", "(Suara: motor berhenti di depan rumah.)"));
            Set(alur, "suaraMotorPergi", BuatSuara(rig, "MotorPergi", Sfx, "SFX", "(Suara: motor menjauh.)"));
            Set(alur, "suaraKetuk", BuatSuara(rig, "KetukPintu", Sfx, "SFX", "(Suara: ketukan di pintu depan.)"));
            Set(alur, "suaraDentang", BuatSuara(rig, "DentangJam", Sfx, "SFX", "(Suara: jam dinding berdentang.)"));
            Set(alur, "suaraJerit", BuatSuara(rig, "IbuMenjerit", Sfx, "SFX", "(Suara: Ibu menjerit dari kamarnya.)"));
            Set(alur, "suaraCabut", BuatSuara(rig, "PakuDicabut", Sfx, "SFX", "(Suara: paku dicabut.)"));
            Set(alur, "suaraPintuDepan", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)"));
            Set(alur, "suaraStinger", BuatSuara(rig, "StingerKejutan", Sfx, "SFX", "(Suara: dentuman mengejutkan.)"));
            Set(alur, "suaraDetak", BuatSuara(rig, "DetakJantung", Sfx, "SFX", "(Suara: detak jantung.)", ulang: true));
            string tegang = PembuatPengganti.AdaSuara(Musik, "MusikTegang") ? "MusikTegang" : "DroneHoror";
            Set(alur, "musikTegang", BuatSuara(rig, tegang, Musik, "Music", "(Musik tegang.)", ulang: true));
        }
    }
}
