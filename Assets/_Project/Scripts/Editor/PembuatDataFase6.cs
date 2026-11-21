using System;
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Aset dialog Malam 6 (rumah keluarga dan sidang warga) dan Catatan Raka tentang main hakim sendiri.
    // Kutipan Game Story ditandai "dikutip"; sisanya DRAF R6: tim menyunting di file ini lalu Bangun Ulang Semua.
    // Batchmode: -executeMethod TheSending.EditorTools.PembuatDataFase6.Buat
    public static class PembuatDataFase6
    {
        const string Folder = "Assets/_Project/Data/Fase6";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        internal const string Kerasukan = "Kerasukan", LarasMasuk = "LarasMasuk", LarasMasukTanpaObat = "LarasMasukTanpaObat",
            SidangPembuka = "SidangPembuka", Tuduhan1 = "Tuduhan1", Tuduhan2 = "Tuduhan2", Tuduhan3 = "Tuduhan3",
            Benar1 = "Benar1", Benar2 = "Benar2", Benar3 = "Benar3", Salah1 = "Salah1", Salah2 = "Salah2", Salah3 = "Salah3",
            SidangBerhasil = "SidangBerhasil", SidangGagal = "SidangGagal", SidangDiam = "SidangDiam", SidangMenuduh = "SidangMenuduh";

        [MenuItem("The Sending/Buat Data Fase 6")]
        public static void Buat()
        {
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();

            // Rumah keluarga (AlurMalam6)
            Simpan(Kerasukan,
                B("Ibu (suara berat)", "Ojo ngganggu. Omah iki wis dadi duwekku.", "Jangan mengganggu. Rumah ini sudah jadi milikku."),
                B("Laras", "Mas! Itu bukan suara Ibu!"),
                B("Ibu (suara berat)", "Selasa Kliwon... kabeh rampung.", "Selasa Kliwon... semuanya selesai."),
                B("Raka", "(Ibu terkulai. Napasnya kembali pelan.) Bu... Ibu dengar aku?"));
            Simpan(LarasMasuk,
                B("Laras", "Mas! Mas Raka! Warga bawa obor ke rumah Bu Sumi!"),
                B("Laras", "Pak Joko di depan. Mereka teriak mau bakar rumahnya!"),
                B("Raka", "Ras, jaga Ibu. Mas ke sana sekarang."));
            Simpan(LarasMasukTanpaObat,
                B("Laras", "Mas! Mas Raka! Warga bawa obor ke rumah Bu Sumi!"),
                B("Laras", "Obat Ibu belum diminum, biar aku saja. Mas cepat ke sana!"),
                B("Raka", "Ras, jaga Ibu. Mas ke sana sekarang."));

            // Sidang (AlurDesaMalam6)
            Simpan(SidangPembuka,
                B("Warga", "Bakar! Bakar saja rumahnya!"),   // dikutip
                B("Bu Sumi", "Aku ora ngerti apa-apa... tulung...", "Aku tidak tahu apa-apa... tolong..."),
                B("Raka", "Tunggu! Pak Joko, beri saya waktu bicara!"),
                B("Pak Joko", "Mas Raka. Warga sudah tidak sabar. Kalau Mas punya bukti, tunjukkan sekarang."));
            Simpan(Tuduhan1, B("Warga 2", "Rambut di sumur itu rambut Mbok Sumi!"));   // dikutip
            Simpan(Tuduhan2, B("Warga 1", "Dia dendam sama keluargamu sejak suaminya mati!"));   // dikutip
            Simpan(Tuduhan3, B("Warga 3", "Siapa lagi yang ngerti ilmu begituan di desa ini?"));   // dikutip
            Simpan(Benar1,
                B("Raka", "Rambut itu dari sisir Bu Sumi yang dicuri. Adik saya lihat sendiri siapa yang ada di sumur malam itu."),   // dikutip
                B("Pak Joko", "...Lanjutkan, Mas."));
            Simpan(Benar2,
                B("Raka", "Bapak saya sudah minta maaf, dan Bu Sumi sudah memaafkan. Yang untung dari sakitnya Ibu bukan Bu Sumi."),
                B("Warga 1", "...Lho, terus siapa?"));
            Simpan(Benar3,
                B("Raka", "Tanah di kendi itu tanah merah Watuireng, kata Pak Lasno. Ayam cemaninya juga dibeli orang Watuireng. Bukan dari desa ini."),
                B("Pak Joko", "Watuireng... itu bukan rumah Bu Sumi."));
            Simpan(Salah1, B("Warga 2", "Halah! Itu cuma omongan! Mana buktinya?"));
            Simpan(Salah2, B("Warga 1", "Nggak nyambung, Mas! Jangan bela dukun!"));
            Simpan(Salah3, B("Warga 3", "Katanya, katanya! Kami juga bisa ngomong begitu!"));
            Simpan(SidangBerhasil,
                B("Pak Joko", "Turunkan obornya! Kalau begini, kita lapor resmi. Main hakim sendiri itu juga melanggar hukum, kita semua bisa dipidana."),   // dikutip
                B("Bu Sumi", "Matur nuwun, Le. Aku melu kowe. Ibumu kudu ditulungi.", "Terima kasih, Nak. Aku ikut denganmu. Ibumu harus ditolong."),
                B("Raka", "Terima kasih, Bu."));
            Simpan(SidangGagal,
                B("Warga", "Bakar! Bakar!"),
                B("Bu Sumi", "Aku ora salah... Aku ora salah...", "Aku tidak salah... Aku tidak salah..."));   // dikutip
            Simpan(SidangDiam,
                B("Raka", "(Raka menunduk. Kata-katanya hilang.)"),
                B("Pak Joko", "Mas Raka saja diam. Sudah jelas."));
            Simpan(SidangMenuduh,
                B("Raka", "...Mungkin memang benar. Semua mulai sejak Bu Sumi..."),
                B("Warga", "Nah! Anaknya sendiri bilang begitu!"));

            IsiJurnal();
            AssetDatabase.SaveAssets();
            Debug.Log($"Data Fase 6 dibuat: 17 dialog di {Folder}, Catatan Raka main hakim sendiri.");
        }

        internal static DataDialog Muat(string nama) =>
            AssetDatabase.LoadAssetAtPath<DataDialog>($"{Folder}/{nama}.asset")
            ?? throw new FileNotFoundException($"Dialog Fase 6 belum ada: {Folder}/{nama}.asset. Jalankan PembuatDataFase6.Buat.");
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

        // Aman diulang: hanya menambah jika judul belum ada.
        static void IsiJurnal()
        {
            var j = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");
            const string judul = "Main hakim sendiri (DRAF, dicek ulang Fase 8)";
            const string isi =
                "Menghakimi orang tanpa proses hukum (membakar rumah, memukuli) adalah tindak pidana: perusakan, penganiayaan, " +
                "pembakaran, bahkan pembunuhan. Ikut beramai-ramai tidak menghapus tanggung jawab.\n\n" +
                "KUHP baru (UU 1/2023, berlaku sejak 2 Januari 2026) Pasal 252 mempidana orang yang mengaku punya kekuatan gaib " +
                "lalu menawarkan jasa untuk mencelakai orang lain. Yang dihukum adalah perbuatannya, lewat proses hukum, " +
                "bukan oleh warga.\n\nJika ada tuduhan santet: kumpulkan bukti, lapor ke Bhabinkamtibmas, Babinsa, atau polsek.";
            bool ada = false;
            foreach (var e in j.CatatanRaka)
                if (e != null && e.Judul == judul) { e.Isi = isi; ada = true; }
            if (!ada)
            {
                Array.Resize(ref j.CatatanRaka, j.CatatanRaka.Length + 1);
                j.CatatanRaka[j.CatatanRaka.Length - 1] = new EntriJurnal { Judul = judul, Isi = isi };
            }
            EditorUtility.SetDirty(j);
        }
    }
}
