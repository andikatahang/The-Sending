using System;
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Aset dialog Malam 3 (rumah keluarga dan rumah Bu Sumi) dan isi Jurnal yang terbuka di Malam 3.
    // Tim menyunting teks DI FILE INI lalu menjalankan The Sending > Bangun Ulang Semua (K24).
    // Batchmode: -executeMethod TheSending.EditorTools.PembuatDataFase3.Buat
    public static class PembuatDataFase3
    {
        const string Folder = "Assets/_Project/Data/Fase3";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        internal const string PakdeDatang = "PakdeDatang", KotakSepatu = "KotakSepatu", Halaman4 = "Halaman4",
            KalenderMalam3 = "KalenderMalam3", SumiPintu = "SumiPintu", SumiMenunggu = "SumiMenunggu",
            SumiTerimaKasih = "SumiTerimaKasih", SumiPamit = "SumiPamit", SumiCerita = "SumiCerita";

        [MenuItem("The Sending/Buat Data Fase 3")]
        public static void Buat()
        {
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();

            // Rumah keluarga
            Simpan(PakdeDatang,
                B("Pakde Darmo", "Le, Raka! Pakde dengar ibumu sakit. Ini, Pakde bawakan buah."),
                B("Raka", "Terima kasih, Pakde."),
                B("Pakde Darmo", "Ibumu harus dibawa ke rumah sakit kota. Biayanya besar, Le. Pakde sudah ada pembeli untuk tanah di tepi sungai itu. Orangnya sudah siap bayar.",
                    efek: "katanya:K4"),
                B("Raka", "Itu tanah peninggalan Bapak, Pakde. Bapak dulu nggak mau menjualnya."),
                B("Pakde Darmo", "Kamu itu masih anak kecil, Le. Nggak ngerti keadaan. Tanah itu dijual, ibumu bisa berobat, kalian bisa hidup enak."),   // dikutip
                B("Raka", "Maaf, Pakde. Tanahnya nggak akan dijual."),
                B("Pakde Darmo", "(menghela napas keras) Ya sudah. Nanti kamu menyesal sendiri.", efek: "tujuan:PakdeDatang"));
            Simpan(KotakSepatu,
                B("Raka", "Kotak sepatu lama... isinya surat-surat Bapak. Di dasarnya nyelip selembar halaman buku harian.", efek: "halaman:H3"),
                B("Raka", "Pakde Darmo marah besar, bilang Bapak bakal menyesal... Persis omongan Pakde tadi."));
            Simpan(Halaman4,
                B("Raka", "Ada halaman buku harian Bapak nyelip di rak alat.", efek: "halaman:H4"),
                B("Raka", "Bapak nulis cara mencabut paku santet dan menetralkan medianya. Bisa kubaca lagi di Jurnal."));
            Simpan(KalenderMalam3,
                B("Raka", "Kamis. Malam ini malam Jumat Legi."));

            // Rumah Bu Sumi
            Simpan(SumiPintu,
                B("Bu Sumi", "Arep apa kowe mrene bengi-bengi?", "Mau apa kamu ke sini malam-malam?"),   // Raka sudah di ambang pintu dalam
                B("Raka", "Bu Sumi... saya Raka, anaknya Bu Ratmi."),
                P(B("Bu Sumi", "Aku ngerti. Kabeh wong desa wis ngomongke aku.", "Aku tahu. Semua orang desa sudah membicarakan aku."),
                    ("Bu, saya minta maaf soal yang dulu.", "kepercayaan:MintaMaaf; tujuan:KunjungiSumi", 3),
                    ("Ibu yang bikin ibu saya sakit, kan?", "tuduh-sumi; tujuan:KunjungiSumi", 5),
                    ("Dapur Ibu gelap. Ada yang bisa saya bantu?", "tujuan:KunjungiSumi", 7)),
                B("Bu Sumi", "...Wis suwe aku ngenteni ana sing ngomong ngono.", "...Sudah lama aku menunggu ada yang bilang begitu."),
                B("Bu Sumi", "Teplokku mati, gentongku kosong. Aku wis tuwa, Le.", "Teplokku mati, gentongku kosong. Aku sudah tua, Nak.", akhir: true),
                B("Bu Sumi", "Kowe padha wae karo wong-wong kuwi.", "Kamu sama saja dengan orang-orang itu."),
                B("Bu Sumi", "Metu! Aja bali mrene bengi iki.", "Pergi! Jangan kembali ke sini malam ini.", akhir: true),
                B("Bu Sumi", "Hm. Sumbune kudu diresiki. Banyune njupuk saka sumur.", "Hm. Sumbunya harus dibersihkan. Airnya ambil dari sumur."),
                B("Bu Sumi", "Teplok ana ing meja, sumur ana ing mburi omah.", "Teplok ada di meja, sumur ada di belakang rumah."));
            Simpan(SumiMenunggu,
                B("Bu Sumi", "Teplok ana ing meja, sumur ana ing mburi omah.", "Teplok ada di meja, sumur ada di belakang rumah."));
            Simpan(SumiTerimaKasih,
                B("Bu Sumi", "Matur nuwun, Le. Wis suwe ora ana sing gelem nulungi aku.", "Terima kasih, Nak. Sudah lama tidak ada yang mau menolongku.",
                    efek: "kepercayaan:BantuLampuDanAir"));
            Simpan(SumiPamit,
                B("Bu Sumi", "Wis bengi, Le. Muliha, ibumu ngenteni.", "Sudah malam, Nak. Pulanglah, ibumu menunggu."));
            Simpan(SumiCerita,
                B("Bu Sumi", "Telung taun kepungkur bojoku lara banget. Aku nyilih dhuwit marang ibumu, nanging ditolak.",
                    "Tiga tahun lalu suamiku sakit keras. Aku meminjam uang kepada ibumu, tetapi ditolak."),
                B("Raka", "Waktu itu uangnya kepakai buat biaya kuliah saya, Bu."),
                B("Bu Sumi", "Bojoku seda. Aku mung kandha \"Gusti Allah mboten sare\". Wong-wong banjur nganggep aku ngipat-ipati kulawargamu.",
                    "Suamiku meninggal. Aku cuma bilang \"Tuhan tidak tidur\". Orang-orang lalu menganggap aku mengutuk keluargamu."),
                B("Bu Sumi", "Aku ora tau nyantet sapa-sapa. Aku mung tau nangis.", "Aku tidak pernah menyantet siapa pun. Aku cuma pernah menangis."),   // dikutip
                B("Bu Sumi", "Simbokku biyen mulang carane nawarake barang ala. Rungokna.", "Ibuku dulu mengajari cara menawarkan barang jahat. Dengarkan."),
                B("Bu Sumi", "Barange diselehke ing anglo, disawuri uyah, ditutupi godhong kelor, diobong, diudheg nganggo bilah pring kuning karo ndonga. Awune dipendhem ing pekarangan.",
                    "Barangnya diletakkan di anglo, ditaburi garam, ditutup daun kelor, dibakar, diaduk dengan bilah bambu kuning sambil berdoa. Abunya dikubur di pekarangan.",
                    efek: "ritual-dari-sumi"),
                B("Bu Sumi", "Iki uyah, godhong kelor, lan pring kuning. Gawanen.", "Ini garam, daun kelor, dan bambu kuning. Bawalah."),
                B("Bu Sumi", "Wong sing teka tanpa diundang, ojo dikei apa-apa.", "Orang yang datang tanpa diundang, jangan diberi apa pun."));   // dikutip

            IsiJurnal();
            AssetDatabase.SaveAssets();
            Debug.Log($"Data Fase 3 dibuat: 9 dialog di {Folder}, isi Jurnal Malam 3.");
        }

        internal static DataDialog Muat(string nama) =>
            AssetDatabase.LoadAssetAtPath<DataDialog>($"{Folder}/{nama}.asset")
            ?? throw new FileNotFoundException($"Dialog Fase 3 belum ada: {Folder}/{nama}.asset. Jalankan PembuatDataFase3.Buat.");

        static BarisDialog B(string pembicara, string teks, string terjemahan = "", string efek = "", bool akhir = false) =>
            new BarisDialog { Pembicara = pembicara, Teks = teks, Terjemahan = terjemahan, Efek = efek, Akhir = akhir };

        // Baris dengan pilihan: (teks, kode efek, indeks baris berikutnya).
        static BarisDialog P(BarisDialog baris, params (string teks, string efek, int lompatKe)[] pilihan)
        {
            baris.Pilihan = Array.ConvertAll(pilihan, p => new PilihanDialog { Teks = p.teks, Efek = p.efek, LompatKe = p.lompatKe });
            return baris;
        }

        // Baris terakhir selalu menutup percakapan. Aset yang sudah ada diisi ulang di tempat supaya GUID tetap.
        static void Simpan(string nama, params BarisDialog[] baris)
        {
            foreach (var b in baris)
                if (string.IsNullOrWhiteSpace(b.Teks)) throw new InvalidDataException($"Dialog '{nama}': ada baris tanpa teks.");
            baris[baris.Length - 1].Akhir = true;
            var masalah = PemeriksaDialog.Periksa(baris);
            if (masalah.Count > 0) throw new InvalidDataException($"Dialog '{nama}': {string.Join("; ", masalah)}");

            string jalur = $"{Folder}/{nama}.asset";
            var data = AssetDatabase.LoadAssetAtPath<DataDialog>(jalur);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<DataDialog>();
                AssetDatabase.CreateAsset(data, jalur);
            }
            data.Baris = baris;
            EditorUtility.SetDirty(data);
        }

        // Jurnal.asset dibuat PembuatSandbox (0B). Halaman 4 sengaja menyebut aturan, bukan urutan paku (itu jawaban puzzle).
        static void IsiJurnal()
        {
            var j = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");
            j.HalamanDiary[Kode.H3].Isi =
                "Darmo datang lagi bersama orang dari tambang pasir. Kutolak di depan mereka. Darmo marah besar, " +
                "membanting gelas, lalu bilang aku akan menyesal. Kakakku sendiri. Aku hampir tidak mengenalinya lagi.";
            j.HalamanDiary[Kode.H4].Isi =
                "Kalau menemukan paku di tiang rumah, jangan dicabut sembarangan. Paku santet dicabut mulai dari paku yang " +
                "berukir hari ini, lalu urut hari berikutnya sampai ketujuhnya. Ingat, hari Jawa berganti saat magrib: " +
                "Kamis malam sudah terhitung malam Jumat.\n\n" +
                "Media santet dinetralkan di anglo: letakkan medianya, taburkan garam, tutup dengan daun kelor, nyalakan api, " +
                "aduk dengan bilah bambu kuning sampai menjadi abu sambil berdoa, lalu kubur abunya di pekarangan. " +
                "Kalau urutannya salah, ulangi dari awal.\n\n" +
                "Garam ada di rak bumbu dapur, daun kelor di pohon pekarangan belakang, bambu kuning di gudang.";
            j.Media[Kode.M2].Isi =
                "Tujuh paku berkarat tertanam berbaris di tiang kayu besar ruang tengah, tertutup dempul hitam. Setiap paku " +
                "berukir nama hari. Sama dengan paku yang dimuntahkan Ibu.";
            j.Bukti[Kode.B1].Isi =
                "Halaman 1 dan 3 buku harian Bapak: Darmo menyampaikan tawaran pembeli tanah dan Bapak menolak, lalu Darmo " +
                "marah besar dan berkata Bapak akan menyesal.";
            j.Katanya[Kode.K4].Isi = "Kata Pakde Darmo, pembeli tanah keluarga di tepi sungai sudah siap membayar.";
            j.CatatanBapak[2].Isi =
                "Rumah limasan beratap seperti limas yang terpancung dan bertumpu pada empat tiang utama di tengah rumah, " +
                "yang disebut saka guru. Saka guru dianggap jantung rumah dan biasanya dibuat dari kayu terbaik, misalnya " +
                "jati. Ruang yang dinaungi keempat saka itu dipakai keluarga untuk berkumpul.";
            j.CatatanBapak[5].Isi =
                "Dalam kepercayaan sebagian masyarakat Jawa, daun kelor dan bambu kuning dipercaya menolak hal-hal jahat, " +
                "sehingga sering dipakai dalam upacara tolak bala. Di luar kepercayaan itu, daun kelor adalah sayuran bergizi " +
                "yang biasa dimasak sebagai sayur bening. Ritual Penangkal dalam gim ini fiksi, bukan panduan nyata.";
            EditorUtility.SetDirty(j);
        }
    }
}
