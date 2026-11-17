using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Aset dialog Fase 1 (Prolog + Malam 1) dan isi Jurnal yang terbuka di Fase 1.
    // Tim menyunting teks DI FILE INI lalu menjalankan The Sending > Bangun Ulang Semua (K5).
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatDataFase1.Buat
    public static class PembuatDataFase1
    {
        const string Folder = "Assets/_Project/Data/Fase1";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        internal const string Ojek = "Ojek", LarasSambutan = "LarasSambutan", BidanWulan = "BidanWulan",
            IbuProlog = "IbuProlog", LarasKronologi = "LarasKronologi", IbuSetelahObat = "IbuSetelahObat",
            TeguranObat = "TeguranObat", FotoKeluarga = "FotoKeluarga", PenutupMalam1 = "PenutupMalam1",
            Kalender = "Kalender";

        // Game Story 10.3: Halaman 2 adalah tabel aksara Jawa. Bentuk aksaranya (gambar atau font) datang di Fase 2
        // bersama puzzle aksara; di Fase 1 halaman ini berisi urutan bacanya saja.
        const string IsiHalaman2 =
            "ha  na  ca  ra  ka\n" +
            "da  ta  sa  wa  la\n" +
            "pa  dha  ja  ya  nya\n" +
            "ma  ga  ba  tha  nga";

        [MenuItem("The Sending/Buat Data Fase 1")]
        public static void Buat()
        {
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();

            // Prolog
            Simpan(Ojek,
                B("Tukang Ojek", "Sampai sini saja, Mas. Sudah mau magrib, saya nggak berani lewat kebun bambu."));
            Simpan(LarasSambutan,
                B("Laras", "Mas Raka, akhirnya sampai juga. Kok lama sekali?"),
                B("Raka", "Maaf, Ras. Ibu gimana?"),
                B("Laras", "Bu Bidan Wulan lagi periksa Ibu di kamar. Temui beliau dulu, Mas."));
            Simpan(BidanWulan,
                B("Bidan Wulan", "Tensi Ibu rendah, kurang minum dan kurang makan. Saya sudah sarankan dirawat di rumah sakit kecamatan, tapi Ibu menolak. Obat ini diminum jam sembilan malam, jangan sampai lupa ya, Mas.", efek: "barang:ObatIbu"),
                B("Raka", "Muntah paku itu nggak mungkin, Bu Bidan. Pasti ada penjelasannya."),
                B("Bidan Wulan", "Saya juga belum bisa menjelaskan, Mas. Tapi apa pun penyebabnya, Ibu tetap harus minum obat dan cukup cairan.", efek: "tujuan:BicaraBidan"));
            Simpan(IbuProlog,
                B("Raka", "Bu, ini Raka. Aku pulang."),
                B("Ibu", "..."),
                B("Raka", "Ibu kok diam saja... Ya sudah, Bu, istirahat dulu."));

            // Malam 1
            Simpan(LarasKronologi,
                B("Laras", "Ibu mulai sakit dua minggu lalu, Mas. Awalnya cuma lemas, terus nggak mau makan."),
                B("Raka", "Mungkin cuma kecapekan, Ras. Kamu juga kurang tidur."),
                B("Laras", "Bukan cuma itu. Sejak Ibu sakit, orang-orang desa mulai bisik-bisik soal Bu Sumi."),
                B("Raka", "Gosip lagi. Aku nggak percaya hal begitu."),
                B("Laras", "Terserah Mas. Tapi lihat sendiri dulu keadaan rumah ini, baru bilang."));
            Simpan(IbuSetelahObat,
                B("Ibu", "Hadi... ojo didol...", "Hadi... jangan dijual..."));
            Simpan(TeguranObat,
                B("Laras", "Mas, ini sudah lewat jam sembilan. Obat Ibu belum diminum."),
                B("Raka", "Astaga, aku lupa. Maaf, Ras."));
            Simpan(FotoKeluarga,
                B("Raka", "Di balik bingkai foto ini ada tulisan tangan."),
                B("Tulisan Bapak", "Kodenya adalah tahun rumah ini berdiri."));
            Simpan(PenutupMalam1,
                B("Laras", "Mas, Bapak dulu juga begini. Tujuh malam, terus..."),
                B("Raka", "Ras, sudah. Besok kita bawa Ibu ke rumah sakit, mau nggak mau."));
            Simpan(Kalender,
                B("Raka", "Selasa, Malam Rabu Wage. Hari pertama aku di rumah lagi."));

            IsiJurnal();
            AssetDatabase.SaveAssets();
            Debug.Log($"Data Fase 1 dibuat: 10 dialog di {Folder}, 4 isi Jurnal.");
        }

        internal static DataDialog Muat(string nama) =>
            AssetDatabase.LoadAssetAtPath<DataDialog>($"{Folder}/{nama}.asset")
            ?? throw new FileNotFoundException($"Dialog Fase 1 belum ada: {Folder}/{nama}.asset. Jalankan PembuatDataFase1.Buat.");

        static BarisDialog B(string pembicara, string teks, string terjemahan = "", string efek = "") =>
            new BarisDialog { Pembicara = pembicara, Teks = teks, Terjemahan = terjemahan, Efek = efek };

        // Baris terakhir selalu menutup percakapan. Aset yang sudah ada diisi ulang di tempat supaya GUID tetap.
        static void Simpan(string nama, params BarisDialog[] baris)
        {
            foreach (var b in baris)
            {
                if (string.IsNullOrWhiteSpace(b.Teks)) throw new InvalidDataException($"Dialog '{nama}': ada baris tanpa teks.");
                TanpaPenanda(nama, b.Teks, b.Terjemahan);
            }
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

        // Jurnal.asset dibuat PembuatSandbox (0B). Di sini hanya empat isi yang terbuka di Fase 1 yang diisi.
        static void IsiJurnal()
        {
            var j = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");
            string h1 = "Kang Darmo datang lagi sore ini membawa tawaran dari seorang pembeli tanah. Harganya besar, katanya. Aku tolak. Tanah ini warisan, bukan barang dagangan. Darmo pergi dengan muka masam, dan aku tahu ini belum selesai.", c1 = "Dehidrasi terjadi saat tubuh kekurangan cairan. Tandanya mulut kering, lemas, pusing, dan buang air kecil sedikit. Cairan seperti oralit perlu diberikan sedikit demi sedikit tapi rutin.", c2 = "Orang sakit perlu dirujuk ke rumah sakit bila tidak mau makan atau minum, terus lemas, atau keadaannya memburuk meski sudah dirawat di rumah. Jangan menunda. Bawa ke puskesmas atau rumah sakit.";
            TanpaPenanda("Jurnal", h1, c1, c2);
            j.HalamanDiary[Kode.H1].Isi = h1;
            j.HalamanDiary[Kode.H2].Isi = IsiHalaman2;
            j.CatatanRaka[0].Isi = c1;
            j.CatatanRaka[1].Isi = c2;
            EditorUtility.SetDirty(j);
        }

        static void TanpaPenanda(string nama, params string[] teks)
        {
            foreach (var t in teks)
                if (t != null && t.Contains("[["))
                    throw new InvalidDataException($"'{nama}' masih berisi penanda: \"{t}\". Lihat Task 11 Step 2.");
        }
    }
}
