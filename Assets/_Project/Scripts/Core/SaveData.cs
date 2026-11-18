using System;
using UnityEngine;

namespace TheSending.Core
{
    public static class SaveData
    {
        public static string KeJson(GameState s) => JsonUtility.ToJson(s);

        // Isi file save tidak dipercaya: rusak atau di luar rentang -> FormatException.
        public static GameState DariJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new FormatException("Data save kosong.");

            GameState s;
            try { s = JsonUtility.FromJson<GameState>(json); }
            catch (ArgumentException e) { throw new FormatException("Data save rusak: " + e.Message, e); }
            if (s == null) throw new FormatException("Data save rusak.");

            Validasi(s);
            return s;
        }

        static void Validasi(GameState s)
        {
            Rentang(s.MalamKe, 0, 7, nameof(s.MalamKe));
            Rentang(s.KondisiIbu, 0, 100, nameof(s.KondisiIbu));
            Rentang(s.KepercayaanSumi, 0, 3, nameof(s.KepercayaanSumi));
            if (!(s.MenitGim >= 0f && s.MenitGim <= JamGim.BatasLunak))
                throw new FormatException($"{nameof(s.MenitGim)} = {s.MenitGim}, harus 0..{JamGim.BatasLunak}.");
            if (!(s.BateraiPersen >= 0f && s.BateraiPersen <= 100f))
                throw new FormatException($"BateraiPersen = {s.BateraiPersen}, harus 0..100.");
            Panjang(s.MediaDitemukan, Kode.JumlahMedia, nameof(s.MediaDitemukan));
            Panjang(s.MalamMediaDitemukan, Kode.JumlahMedia, nameof(s.MalamMediaDitemukan));
            Panjang(s.MediaDinetralkan, Kode.JumlahMedia, nameof(s.MediaDinetralkan));
            Panjang(s.MediaDiserahkan, Kode.JumlahMedia, nameof(s.MediaDiserahkan));
            Panjang(s.BuktiKunci, Kode.JumlahBukti, nameof(s.BuktiKunci));
            Panjang(s.KatanyaDiketahui, Kode.JumlahKatanya, nameof(s.KatanyaDiketahui));
            Panjang(s.StatusKatanya, Kode.JumlahKatanya, nameof(s.StatusKatanya));
            Panjang(s.HalamanDiary, Kode.JumlahHalaman, nameof(s.HalamanDiary));
            Panjang(s.AksiSumiDipakai, Kode.JumlahAksiSumi, nameof(s.AksiSumiDipakai));
            Panjang(s.BahanRitual, Kode.JumlahBahanRitual, nameof(s.BahanRitual));
            if (s.TujuanSelesai == null) throw new FormatException("TujuanSelesai tidak ada.");
            if (s.TujuanSelesai.Contains(null)) throw new FormatException("TujuanSelesai berisi elemen kosong.");
            if (s.Barang == null) throw new FormatException("Barang tidak ada.");
            foreach (var id in s.Barang)
                if (!Inventaris.Dikenal(id)) throw new FormatException($"Barang '{id}' tidak dikenal.");
            if (s.Foto == null) throw new FormatException("Foto tidak ada.");
            foreach (var id in s.Foto)
                if (!PapanBukti.FotoDikenal(id)) throw new FormatException($"Foto '{id}' tidak dikenal.");

            EnumValid(s.HasilSidang, nameof(s.HasilSidang));
            EnumValid(s.PilihanAkhir, nameof(s.PilihanAkhir));
            EnumValid(s.Ending, nameof(s.Ending));
            foreach (var status in s.StatusKatanya) EnumValid(status, nameof(s.StatusKatanya));
            Rentang(s.PintuSumiTerbukaMulaiMalam, 0, 8, nameof(s.PintuSumiTerbukaMulaiMalam));
            foreach (var malam in s.MalamMediaDitemukan) Rentang(malam, 0, 7, nameof(s.MalamMediaDitemukan));
        }

        static void Rentang(int nilai, int min, int maks, string nama)
        {
            if (nilai < min || nilai > maks) throw new FormatException($"{nama} = {nilai}, harus {min}..{maks}.");
        }

        static void EnumValid<T>(T nilai, string nama) where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), nilai)) throw new FormatException($"{nama} = {nilai} tidak dikenal.");
        }

        static void Panjang(Array a, int n, string nama)
        {
            if (a == null || a.Length != n) throw new FormatException($"{nama} harus berisi {n} elemen.");
        }
    }
}
