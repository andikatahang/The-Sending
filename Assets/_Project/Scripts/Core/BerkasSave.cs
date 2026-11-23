using System;
using System.IO;
using UnityEngine;

namespace TheSending.Core
{
    // Satu slot save: <folder>/checkpoint.json.
    public static class BerkasSave
    {
        public const string NamaFile = "checkpoint.json";

        public static string Jalur(string folder) => Path.Combine(folder, NamaFile);
        public static bool Ada(string folder) => File.Exists(Jalur(folder));

        // Tulis ke .tmp dulu lalu ganti file lama, supaya save lama tidak rusak jika gim mati saat menulis.
        public static void Tulis(string folder, GameState s)
        {
            Directory.CreateDirectory(folder);
            string tujuan = Jalur(folder);
            string sementara = tujuan + ".tmp";
            File.WriteAllText(sementara, SaveData.KeJson(s));
            if (File.Exists(tujuan)) File.Replace(sementara, tujuan, null);
            else File.Move(sementara, tujuan);
        }

        // null jika belum ada save atau file rusak; menu lalu hanya menawarkan Mulai Baru.
        public static GameState Baca(string folder)
        {
            string jalur = Jalur(folder);
            if (!File.Exists(jalur)) return null;
            try
            {
                return SaveData.DariJson(File.ReadAllText(jalur));
            }
            catch (FormatException e)
            {
                Debug.LogWarning($"Save rusak, diabaikan: {e.Message}");
                return null;
            }
            catch (IOException e)
            {
                Debug.LogWarning($"Save tidak bisa dibaca: {e.Message}");
                return null;
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogWarning($"Save tidak bisa dibaca: {e.Message}");
                return null;
            }
        }

        public static void Hapus(string folder)
        {
            if (!Directory.Exists(folder)) return;
            File.Delete(Jalur(folder));   // aman jika file tidak ada
            File.Delete(Jalur(folder) + ".tmp");
        }
    }
}
