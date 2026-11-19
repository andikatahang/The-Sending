using System;
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Aset dialog Malam 4 (pos ronda di desa, rumah keluarga) dan isi Jurnal yang terbuka di Malam 4.
    // Teks yang bukan kutipan Game Story adalah DRAF (R7): tim menyunting di file ini lalu Bangun Ulang Semua.
    // Batchmode: -executeMethod TheSending.EditorTools.PembuatDataFase4.Buat
    public static class PembuatDataFase4
    {
        const string Folder = "Assets/_Project/Data/Fase4";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        internal const string PakJoko = "PakJoko", Gosip = "Gosip", B2Cemani = "B2Cemani", WargaMenuduh = "WargaMenuduh",
            KalenderMalam4 = "KalenderMalam4", TelurCemani = "TelurCemani";

        [MenuItem("The Sending/Buat Data Fase 4")]
        public static void Buat()
        {
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();

            // Pos ronda (AlurDesaMalam4)
            Simpan(PakJoko,
                B("Raka", "Pak Joko, saya mau pinjam tali. Tali timba sumur kami putus."),
                B("Pak Joko", "Tali tambang ada di pos, Mas. Bawa juga pengait besi ini, siapa tahu timbanya nyangkut.",
                    efek: "barang:TaliTambang; barang:Pengait"));
            Simpan(Gosip,
                B("Warga 1", "Wong tuwo kae pancen dukun. Bojone mati, saiki males.",
                    "Perempuan tua itu memang dukun. Suaminya mati, sekarang dia membalas.", efek: "katanya:K1"),   // dikutip
                B("Warga 2", "Dulu dia sudah sumpahi keluarga Mas Raka. Semua orang dengar.", efek: "katanya:K2"),
                P(B("Pak Joko", "Mas Raka, kalau memang terbukti, warga yang akan bertindak. Desa ini harus aman."),   // dikutip
                    ("Belum ada bukti apa-apa, Pak. Jangan asal tuduh Bu Sumi.", "kepercayaan:BelaDiPosRonda", 3),
                    ("(Diam saja.)", "", 4)),
                B("Pak Joko", "Hm. Ya sudah, Mas. Kita lihat saja nanti.", akhir: true),
                B("Warga 1", "Tuh, Mas Raka saja tidak membantah."));
            Simpan(B2Cemani,
                B("Raka", "Pak, ada yang tahu siapa yang memelihara ayam cemani di sekitar sini?"),
                B("Warga 1", "Ayam cemani? Di sekitar sini cuma Mbah Wiryo yang punya, Mas. Di Watuireng.", efek: "bukti:B2"));

            // Rumah keluarga (AlurMalam4 dan benda di scene)
            Simpan(WargaMenuduh,
                B("Warga 2", "Lho, itu rambut Mbok Sumi! Siapa lagi yang rambutnya putih panjang begitu?", efek: "katanya:K3"),   // dikutip
                B("Warga 1", "Benar. Besok kita lapor Pak Joko."),
                B("Raka", "Belum tentu, Pak. Rambut uban bukan cuma milik Bu Sumi."));
            Simpan(KalenderMalam4,
                B("Raka", "Jumat. Malam ini malam Sabtu Pahing."),
                B("Raka", "Ada kertas terselip di balik kalender... halaman buku harian Bapak.", efek: "halaman:H5"));
            Simpan(TelurCemani,
                B("Raka", "Telur ayam hitam... di dalamnya jarum dan rambut. Rambut Ibu.", efek: "media:M4"),
                B("Raka", "Ayam hitam legam begini tidak dipelihara sembarang orang."));

            IsiJurnal();
            AssetDatabase.SaveAssets();
            Debug.Log($"Data Fase 4 dibuat: 6 dialog di {Folder}, isi Jurnal Malam 4.");
        }

        internal static DataDialog Muat(string nama) =>
            AssetDatabase.LoadAssetAtPath<DataDialog>($"{Folder}/{nama}.asset")
            ?? throw new FileNotFoundException($"Dialog Fase 4 belum ada: {Folder}/{nama}.asset. Jalankan PembuatDataFase4.Buat.");
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

        // Isi Jurnal yang terbuka di Malam 4 (draf R7).
        static void IsiJurnal()
        {
            var j = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");
            j.HalamanDiary[Kode.H5].Isi =
                "Pagi ini kutemukan sebatang paku di bawah bantalku. Berkarat, berukir nama hari. Ratmi bilang mungkin " +
                "jatuh dari usuk. Aku tidak percaya, tapi kubuang juga ke kali. Sejak malam itu badanku tidak enak.";
            j.Media[Kode.M3].Isi =
                "Bungkusan kain hitam dari dasar sumur, berisi rambut uban panjang dan kembang tujuh rupa yang membusuk.";
            j.Media[Kode.M4].Isi =
                "Telur ayam hitam di atas plafon dapur. Di dalamnya jarum dan rambut Ibu.";
            j.Bukti[Kode.B2].Isi =
                "Telur ayam cemani dari plafon dapur. Kata warga di pos ronda, di sekitar sini hanya Mbah Wiryo di " +
                "Watuireng yang memelihara ayam cemani.";
            j.Katanya[Kode.K1].Isi = "Kata warga di pos ronda, Bu Sumi itu dukun.";
            j.Katanya[Kode.K2].Isi = "Kata warga, Bu Sumi pernah mengutuk keluarga Raka.";
            j.Katanya[Kode.K3].Isi = "Kata warga yang lewat, rambut di dasar sumur itu rambut Bu Sumi.";
            EditorUtility.SetDirty(j);
        }
    }
}
