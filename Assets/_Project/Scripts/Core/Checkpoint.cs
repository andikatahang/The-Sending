using System;
using System.IO;
using UnityEngine;

namespace TheSending.Core
{
    public static class Checkpoint
    {
        // false jika gagal menulis (file terkunci, folder read-only, disk penuh); gameplay jalan terus.
        public static bool Simpan(string folder, GameState s)
        {
            try
            {
                BerkasSave.Tulis(folder, s);
                return true;
            }
            catch (IOException e)
            {
                Debug.LogWarning($"Checkpoint gagal disimpan: {e.Message}");
                return false;
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogWarning($"Checkpoint gagal disimpan: {e.Message}");
                return false;
            }
        }

        // Desain 4.2: muat checkpoint, kurangi 5, lalu simpan ulang supaya penalti tidak hilang
        // saat pemain keluar gim. null jika belum ada checkpoint.
        public static GameState MuatSetelahTertangkap(string folder)
        {
            var s = BerkasSave.Baca(folder);
            if (s == null) return null;
            KondisiIbu.SetelahTertangkap(s);
            Simpan(folder, s);
            return s;
        }
    }
}
