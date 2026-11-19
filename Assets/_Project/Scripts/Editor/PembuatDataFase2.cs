using System;
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Aset dialog Malam 2, isi Jurnal yang terbuka di Malam 2, dan impor gambar aksara (tools/render-aksara.sh).
    // Tim menyunting teks DI FILE INI lalu menjalankan The Sending > Bangun Ulang Semua (K15).
    // Batchmode: -executeMethod TheSending.EditorTools.PembuatDataFase2.Buat
    public static class PembuatDataFase2
    {
        const string Folder = "Assets/_Project/Data/Fase2";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";
        const string FolderAksara = "Assets/_Project/Art/UI/Aksara";
        const int UkuranTabel = 1024;   // tabel Halaman 2 harus terbaca; batas PSX 256 hanya untuk tekstur dunia

        internal const string LarasKunciGudang = "LarasKunciGudang", Weton = "Weton", WiryoDatang = "WiryoDatang",
            WiryoBisikan = "WiryoBisikan", KalenderMalam2 = "KalenderMalam2";

        [MenuItem("The Sending/Buat Data Fase 2")]
        public static void Buat()
        {
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();

            Simpan(LarasKunciGudang,
                B("Laras", "Mas, kamar Ibu bau anyir dari tadi sore. Aku nggak berani masuk."),
                B("Raka", "Papan lantai dekat ranjang bunyinya kopong. Kita punya linggis, Ras?"),
                B("Laras", "Ada di gudang. Ini kuncinya, Mas.", efek: "barang:KunciGudang"));
            Simpan(Weton,
                B("Raka", "Ra... t... mi. Ratmi. Ini nama Ibu."),
                B("Raka", "Dan yang ini... Se-la-sa Kli-won."),
                B("Laras", "Selasa Kliwon... itu weton Ibu, Mas."),                       // dikutip
                B("Raka", "Malam ini Kamis Kliwon. Selasa Kliwon... lima malam lagi."));
            Simpan(WiryoDatang,
                B("Mbah Wiryo", "Kulonuwun... Mbah dengar ibumu sakit, Le.", "Permisi... Mbah dengar ibumu sakit, Nak."),
                B("Raka", "Mbah Wiryo? Malam-malam begini?"),
                P(B("Mbah Wiryo", "Barang begini jangan disimpan di rumah, Le. Biar Mbah yang larung ke kali, supaya ibumu tenang."),   // dikutip
                    ("Serahkan boneka kepada Mbah Wiryo", "serahkan:M1; tujuan:PilihanWiryo", 3),
                    ("Simpan boneka sendiri", "tujuan:PilihanWiryo", 5)),
                B("Raka", "Iya, Mbah. Tolong dibuang saja."),
                B("Mbah Wiryo", "Nah, begitu. Ibumu biar Mbah yang mendoakan.", akhir: true),
                B("Raka", "Nggak usah, Mbah. Biar saya simpan dulu."),
                B("Mbah Wiryo", "(tersenyum tipis) Yo wis, sakarepmu, Le.", "Ya sudah, terserah kamu, Nak."));
            Simpan(WiryoBisikan,
                B("Mbah Wiryo (kepada Laras)", "Ingat ya, Nduk.", "Ingat ya, Nak."));                   // dikutip
            Simpan(KalenderMalam2,
                B("Raka", "Rabu, Malam Kamis Kliwon."));

            IsiJurnal();
            SiapkanGambarAksara();
            AssetDatabase.SaveAssets();
            Debug.Log($"Data Fase 2 dibuat: 5 dialog di {Folder}, isi Jurnal Malam 2.");
        }

        internal static DataDialog Muat(string nama) =>
            AssetDatabase.LoadAssetAtPath<DataDialog>($"{Folder}/{nama}.asset")
            ?? throw new FileNotFoundException($"Dialog Fase 2 belum ada: {Folder}/{nama}.asset. Jalankan PembuatDataFase2.Buat.");

        // null jika gambar belum dibuat (tools/render-aksara.sh): PanelAksara memakai teks pengganti.
        internal static Texture2D Tabel() => AssetDatabase.LoadAssetAtPath<Texture2D>($"{FolderAksara}/Tabel.png");

        internal static Texture2D[] GambarSuku()
        {
            var hasil = new Texture2D[BacaanAksara.JumlahSuku];
            for (int i = 0; i < hasil.Length; i++)
            {
                hasil[i] = AssetDatabase.LoadAssetAtPath<Texture2D>($"{FolderAksara}/Suku_{i}.png");
                if (hasil[i] == null) return null;
            }
            return hasil;
        }

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

        // Jurnal.asset dibuat PembuatSandbox (0B). Isi Media M1 sengaja tidak menyebut bacaan aksara (itu jawaban puzzle).
        static void IsiJurnal()
        {
            var j = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");
            j.Media[Kode.M1].Isi =
                "Ditemukan di bawah papan lantai kamar Ibu. Kain putih kumal berlilit benang merah, jarum menusuk " +
                "kepala dan perutnya. Ada tulisan aksara Jawa di tubuhnya.";
            j.CatatanBapak[0].Isi =
                "Orang Jawa memakai dua hitungan hari sekaligus: tujuh hari (Senin sampai Minggu) dan lima hari pasaran " +
                "(Legi, Pahing, Pon, Wage, Kliwon). Gabungan hari dan pasaran saat seseorang lahir disebut weton, misalnya " +
                "Selasa Kliwon. Gabungan yang sama berulang setiap 35 hari. Weton dipakai dalam tradisi, misalnya untuk " +
                "menentukan hari baik hajatan.";
            j.CatatanBapak[1].Isi =
                "Aksara Jawa (hanacaraka) punya 20 aksara dasar. Setiap aksara berbunyi konsonan dengan vokal a: ha, na, " +
                "ca, ra, ka, dan seterusnya. Sandhangan mengubah bunyinya: wulu menjadi i, taling menjadi e, taling tarung " +
                "menjadi o. Pangkon mematikan vokal, dan pasangan dipakai jika konsonan mati diikuti konsonan lain.";
            j.CatatanBapak[6].Isi =
                "Larung berarti menghanyutkan sesaji ke laut atau sungai. Di banyak daerah di Jawa, larung adalah upacara " +
                "syukur dan doa keselamatan bersama. Tradisi ini dihormati masyarakat dan tidak ada hubungannya dengan santet.";
            EditorUtility.SetDirty(j);
        }

        static void SiapkanGambarAksara()
        {
            if (!(AssetImporter.GetAtPath($"{FolderAksara}/Tabel.png") is TextureImporter imp) || imp.maxTextureSize == UkuranTabel) return;
            imp.maxTextureSize = UkuranTabel;
            imp.SaveAndReimport();
        }
    }
}
