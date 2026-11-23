using System;
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Aset dialog Malam 7 (rumah keluarga dan rumah Mbah Wiryo di Watuireng), Halaman 8, dan Catatan Bapak tentang sesaji.
    // Kutipan Game Story ditandai "dikutip"; sisanya DRAF: tim menyunting di file ini lalu Bangun Ulang Semua.
    // Batchmode: -executeMethod TheSending.EditorTools.PembuatDataFase7.Buat
    public static class PembuatDataFase7
    {
        const string Folder = "Assets/_Project/Data/Fase7";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        internal const string LingkaranGaram = "LingkaranGaram", RitualTerakhir = "RitualTerakhir",
            BukuCatatan = "BukuCatatan", KendiInduk = "KendiInduk", Lontar = "Lontar",
            WiryoMemergoki = "WiryoMemergoki", SesajiDilihat = "SesajiDilihat";

        [MenuItem("The Sending/Buat Data Fase 7")]
        public static void Buat()
        {
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();

            // Rumah keluarga (AlurMalam7). Pelajari ritual dipanggil sutradara lewat kode, bukan efek dialog.
            Simpan(LingkaranGaram,   // DRAF kecuali kutipan
                B("Bu Sumi", "Garam dan kelor sudah melingkari ranjang. Lingkaran ini tidak boleh terputus."),
                B("Laras", "Ibu kelihatan lebih tenang, Mas."),
                B("Bu Sumi", "Santet iki ono induke, Le. Nek induke ora diobong, kabeh mau mung ngurangi, ora mateni.",   // dikutip
                    "Santet ini ada induknya, Nak. Kalau induknya tidak dibakar, semua tadi cuma mengurangi, tidak mematikan."),
                B("Raka", "Di mana induknya, Bu?"),
                B("Bu Sumi", "Cari di rumah orang yang kirim. Watuireng, lewat jalan sawah. Aku jaga Ibumu di sini."),
                B("Raka", "Saya berangkat. Ras, jangan buka lingkarannya."));
            Simpan(RitualTerakhir,   // DRAF
                B("Bu Sumi", "Taruh kendinya di tengah lingkaran, Le. Pelan-pelan."),
                B("Raka", "Sudah, Bu."),
                B("Bu Sumi", "Garam, kelor, api, bambu. Urutannya seperti yang Bapakmu tulis. Jangan ada yang terlewat."),
                B("Raka", "Saya ikut, Bu. Terus kita kubur abunya."));

            // Watuireng (AlurWatuireng)
            Simpan(BukuCatatan,
                B("Raka", "Buku pesanan. Tulisan tangan, daftar nama dan tanggal."),
                B("Raka", "\"Darmo, Sumberwangi: Hadi (rampung), Ratmi, Raka, Laras.\"",   // dikutip
                    efek: "bukti:B8"),
                B("Raka", "Hadi itu Bapak. Pakde... jadi benar kamu."),
                B("Raka", "Ada selembar halaman buku harian Bapak terselip di dalamnya.", efek: "halaman:H8"));
            Simpan(KendiInduk,   // DRAF
                B("Raka", "Kendi hitam ini... berat. Di dalamnya ada foto keluarga kami, ditusuk jarum.", efek: "media:M7"),
                B("Raka", "Dan sisir Bu Sumi. Ini induknya."));
            Simpan(Lontar,   // DRAF; teks pilihan memparafrasekan tabel "Pilihan Terakhir" Game Story
                P(B("Raka", "Lontar di samping kendi. Judulnya \"pambalik\": mantra untuk mengembalikan santet ke pengirimnya."),
                    ("Bawa pulang kendi dan buku catatan, bakar dengan cara Bu Sumi, lalu laporkan.", "pilihan-akhir:Lapor", 1),
                    ("Bacakan mantra pambalik, kirim santet kembali kepada Pakde Darmo.", "pilihan-akhir:Balas", 2)),
                B("Raka", "Bapak bilang selesaikan dengan musyawarah. Aku bawa semuanya pulang.", akhir: true),
                B("Raka", "(Raka membaca mantranya pelan. Kendi mulai bergetar.)"));
            Simpan(WiryoMemergoki,
                B("Mbah Wiryo", "Bapakmu dulu juga keras kepala, Le. Lihat sekarang dia di mana."),   // dikutip
                B("Raka", "(Mbah Wiryo berdiri. Ayam cemani di kandang berkokok keras.)"));
            Simpan(SesajiDilihat,   // DRAF
                B("Raka", "Lima mangkuk sesaji, masing-masing bertanda nama pasaran. Lilinnya menyala."),
                B("Raka", "Salah urutan, Mbah Wiryo bisa terbangun. Catatan Bapak menyebut urutan pasaran mulai malam ini."));

            IsiJurnal();
            AssetDatabase.SaveAssets();
            Debug.Log($"Data Fase 7 dibuat: 7 dialog di {Folder}, Halaman 8, Catatan Bapak sesaji.");
        }

        internal static DataDialog Muat(string nama) =>
            AssetDatabase.LoadAssetAtPath<DataDialog>($"{Folder}/{nama}.asset")
            ?? throw new FileNotFoundException($"Dialog Fase 7 belum ada: {Folder}/{nama}.asset. Jalankan PembuatDataFase7.Buat.");

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

        // Aman diulang: Halaman/Media/Bukti diisi ulang di tempat, Catatan Bapak hanya ditambah jika judul belum ada.
        static void IsiJurnal()
        {
            var j = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");
            j.HalamanDiary[Kode.H8].Isi =   // DRAF
                "Semalam aku lihat Wiryo berdiri di pekarangan, lama sekali, menatap rumah. Ia pergi saat kutegur. " +
                "Paginya ada paku di bantalku. Kalau tulisan ini sampai terbaca, Wiryo orangnya, dan ia tidak bekerja sendiri.";
            j.Media[Kode.M7].Isi = "Kendi induk berisi foto keluarga yang ditusuk jarum dan sisir Bu Sumi.";
            j.Bukti[Kode.B8].Isi = "Buku pesanan Mbah Wiryo: \"Darmo, Sumberwangi: Hadi (rampung), Ratmi, Raka, Laras.\"";
            const string judul = "Sesaji dan hari pasaran";
            const string isi =   // DRAF
                "Lima pasaran berurutan: Kliwon, Legi, Pahing, Pon, Wage. Dalam sesaji, lilin dipadamkan menurut urutan itu, " +
                "mulai dari pasaran malam hajat dilakukan. Salah urutan berarti memanggil penjaganya.";
            foreach (var e in j.CatatanBapak)
                if (e != null && e.Judul == judul) { e.Isi = isi; EditorUtility.SetDirty(j); return; }
            Array.Resize(ref j.CatatanBapak, j.CatatanBapak.Length + 1);
            j.CatatanBapak[j.CatatanBapak.Length - 1] = new EntriJurnal { Judul = judul, Isi = isi };
            EditorUtility.SetDirty(j);
        }
    }
}
