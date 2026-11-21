using System;
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Aset dialog Malam 5 (rumah keluarga, makam, rumah Bu Sumi, rumah Pakde) dan isi Jurnal yang terbuka di Malam 5.
    // Teks yang bukan kutipan Game Story adalah DRAF (R7): tim menyunting di file ini lalu Bangun Ulang Semua.
    // Batchmode: -executeMethod TheSending.EditorTools.PembuatDataFase5.Buat
    public static class PembuatDataFase5
    {
        const string Folder = "Assets/_Project/Data/Fase5";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        internal const string Laras = "Laras", LarasTerkunci = "LarasTerkunci", LarasTeh = "LarasTeh", GaliKendi = "GaliKendi",
            KainKafan = "KainKafan", KalenderMalam5 = "KalenderMalam5", AmbilTeh = "AmbilTeh", NisanHadi = "NisanHadi",
            PakLasno = "PakLasno", SumiDingin = "SumiDingin", SumiKesaksian = "SumiKesaksian", SumiMinyakSereh = "SumiMinyakSereh",
            SumiPamit = "SumiPamit", PotTeras = "PotTeras", LaciPakde = "LaciPakde";

        [MenuItem("The Sending/Buat Data Fase 5")]
        public static void Buat()
        {
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();

            // Rumah keluarga (AlurMalam5)
            Simpan(Laras,
                B("Raka", "Ras... boleh Mas masuk?"),
                B("Laras", "..."),
                P(B("Raka", "(Laras menatap lantai. Tangannya gemetar.)"),
                    ("Ras, kamu nggak salah apa-apa. Cerita saja, Mas di sini.", "", 3),   // dikutip GS 607
                    ("Kamu nyembunyiin sesuatu, kan? Ngomong sekarang!", "laras-kunci", 6)),   // dikutip GS 608
                B("Laras", "Aku lihat Mbah Wiryo di sumur, Mas. Tengah malam. Dia bilang kalau aku cerita, Ibu cepat mati..."),   // dikutip
                B("Laras", "Ini... aku nemu di lemari Ibu. Tulisan Bapak.", efek: "bukti:B4; halaman:H6; tujuan:BicaraLaras"),
                B("Raka", "Makasih, Ras. Sekarang Mas yang jaga.", akhir: true),
                B("Laras", "Aku nggak nyembunyiin apa-apa! Keluar, Mas!"));
            Simpan(LarasTerkunci, B("Laras", "(Dari balik pintu) Pergi, Mas. Aku nggak mau ngomong."));
            Simpan(LarasTeh,
                B("Raka", "Ras, Mas bawain teh anget. Mas minta maaf tadi kasar."),
                B("Laras", "(Pintu terbuka pelan. Laras menerima gelasnya.)"));
            Simpan(GaliKendi,
                B("Raka", "Ada yang keras di bawah akar... kendi kecil. Isinya tanah merah, lengket.", efek: "media:M5; tujuan:GaliKendi"),
                B("Raka", "Tanah di sini cokelat. Tanah merah ini dibawa dari tempat lain."));
            Simpan(KainKafan,
                B("Raka", "Kain kafan. Ada tulisan rajah dengan tinta hitam.", efek: "media:M6"));
            Simpan(KalenderMalam5, B("Raka", "Sabtu. Malam ini malam Minggu Pon."));
            Simpan(AmbilTeh, B("Raka", "Teh hangat dan sisa nasi. Laras belum makan dari siang.", efek: "barang:TehHangat; barang:SisaMakanan"));

            // Makam (AlurMakam)
            Simpan(NisanHadi,
                B("Raka", "Hadi Santoso bin Karto. Pak, aku pulang."),
                B("Raka", "Tanah di makam ini hitam berpasir. Persis kata catatan Bapak."));
            Simpan(PakLasno,
                B("Pak Lasno", "Mas Raka? Anaknya Pak Hadi to? Malam-malam kok di sini."),
                B("Raka", "Pak, tanah merah ini dari mana? Ditemukan di rumah kami."),
                B("Pak Lasno", "Lemah abang ngene iki dudu soko kene, Mas. Iki lemah makam Watuireng.",
                    "Tanah merah seperti ini bukan dari sini, Mas. Ini tanah makam Watuireng.", efek: "bukti:B3; tujuan:BandingkanTanah"),   // dikutip
                B("Pak Lasno", "Makam di sana tanahnya merah. Orang sini tahu semua."));

            // Rumah Bu Sumi Malam 5
            Simpan(SumiDingin,
                B("Bu Sumi", "Wis bengi, Le. Mulih o, jaganen ibumu.", "Sudah malam, Nak. Pulanglah, jaga ibumu."));
            Simpan(SumiKesaksian,
                B("Bu Sumi", "Sisirku ilang, Le. Sawise Mbah Wiryo mampir mrene sasi wingi.",
                    "Sisirku hilang, Nak. Setelah Mbah Wiryo mampir ke sini bulan lalu.", efek: "bukti:B5"),
                B("Bu Sumi", "Iki layang soko bapakmu. Simpenen.", "Ini surat dari bapakmu. Simpanlah.", efek: "halaman:H7"),
                B("Bu Sumi", "Bapakmu wis njaluk ngapura. Aku wis ngapura. Sing ora ngerti mung wong sak desa.",
                    "Bapakmu sudah minta maaf. Aku sudah memaafkan. Yang tidak tahu cuma orang sedesa."));   // dikutip
            Simpan(SumiMinyakSereh,
                B("Bu Sumi", "Gawanen lenga sereh iki. Oleske ing sikilmu, langkahmu ora gampang dirungu.",
                    "Bawalah minyak sereh ini. Oleskan di kakimu, langkahmu tidak mudah terdengar.", efek: "minyak-sereh"));
            Simpan(SumiPamit, B("Bu Sumi", "Ati-ati, Le.", "Hati-hati, Nak."));

            // Rumah Pakde
            Simpan(PotTeras, B("Raka", "Di bawah tanah pot... kunci kecil.", efek: "barang:KunciLaci"));
            Simpan(LaciPakde,
                B("Raka", "Kuitansi uang muka dari perusahaan tambang. Atas nama Pakde Darmo."),
                B("Raka", "Dan buku catatan: \"W, tiga juta, lunas setelah selesai.\" W... Wiryo?", efek: "bukti:B6"));

            IsiJurnal();
            AssetDatabase.SaveAssets();
            Debug.Log($"Data Fase 5 dibuat: 15 dialog di {Folder}, isi Jurnal Malam 5.");
        }

        internal static DataDialog Muat(string nama) =>
            AssetDatabase.LoadAssetAtPath<DataDialog>($"{Folder}/{nama}.asset")
            ?? throw new FileNotFoundException($"Dialog Fase 5 belum ada: {Folder}/{nama}.asset. Jalankan PembuatDataFase5.Buat.");
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

        // Isi Jurnal yang terbuka di Malam 5 (draf R7).
        static void IsiJurnal()
        {
            var j = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");
            j.HalamanDiary[Kode.H6].Isi =
                "Aku sudah bicara dengan notaris soal wasiat. Tanah di tepi sungai tidak akan kujual. Soal ini harus " +
                "diselesaikan lewat musyawarah keluarga, bukan dengan jalan lain. Wiryo sempat menawarkan 'jasanya' " +
                "untuk mengurus tanah itu; aku tidak mau berurusan dengannya.";
            j.HalamanDiary[Kode.H7].Isi =
                "Mbok Sumi, aku minta maaf atas kata-kataku waktu itu. Aku marah dan salah menuduh. Kalau sesuatu terjadi " +
                "padaku, tolong jangan benci anak-anakku.";
            j.Media[Kode.M5].Isi = "Kendi kecil berisi tanah merah, terkubur di bawah akar pohon asem di pekarangan.";
            j.Media[Kode.M6].Isi = "Kain kafan bertulis rajah dengan tinta hitam.";
            j.Bukti[Kode.B3].Isi = "Pak Lasno: tanah merah itu tanah makam Watuireng, bukan tanah desa ini.";
            j.Bukti[Kode.B4].Isi = "Laras melihat Mbah Wiryo di sumur tengah malam dan diancam supaya diam.";
            j.Bukti[Kode.B5].Isi = "Bu Sumi: sisirnya hilang sesudah Mbah Wiryo mampir bulan lalu.";
            j.Bukti[Kode.B6].Isi = "Kuitansi uang muka tambang atas nama Pakde Darmo, dan catatan 'W, tiga juta, lunas setelah selesai'.";
            TambahCatatanBapak(j, "Tanah makam",
                "Tanah makam Sumberwangi hitam berpasir, sisa letusan gunung dulu. Tanah merah hanya ada di makam desa seberang sawah.");
            EditorUtility.SetDirty(j);
        }

        // Aman diulang: hanya menambah jika judul belum ada.
        static void TambahCatatanBapak(DataJurnal j, string judul, string isi)
        {
            foreach (var e in j.CatatanBapak)
                if (e != null && e.Judul == judul) { e.Isi = isi; return; }
            Array.Resize(ref j.CatatanBapak, j.CatatanBapak.Length + 1);
            j.CatatanBapak[j.CatatanBapak.Length - 1] = new EntriJurnal { Judul = judul, Isi = isi };
        }
    }
}
