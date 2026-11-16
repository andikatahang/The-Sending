using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TheSending.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace TheSending.Tests
{
    public class BerkasSaveTests
    {
        string folder;

        [SetUp]
        public void Siapkan() => folder = Path.Combine(Path.GetTempPath(), "TheSendingTest_" + Guid.NewGuid().ToString("N"));

        [TearDown]
        public void Bersihkan()
        {
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
        }

        [Test]
        public void BacaTanpaFileMengembalikanNull()
        {
            Assert.IsFalse(BerkasSave.Ada(folder));
            Assert.IsNull(BerkasSave.Baca(folder));
        }

        [Test]
        public void TulisLaluBacaMengembalikanIsiYangSama()
        {
            BerkasSave.Tulis(folder, new GameState { MalamKe = 3, KondisiIbu = 41 });
            var s = BerkasSave.Baca(folder);
            Assert.AreEqual(3, s.MalamKe);
            Assert.AreEqual(41, s.KondisiIbu);
        }

        [Test]
        public void TulisUlangMenimpaTanpaMeninggalkanFileSementara()
        {
            BerkasSave.Tulis(folder, new GameState { MalamKe = 2 });
            BerkasSave.Tulis(folder, new GameState { MalamKe = 4 });
            Assert.AreEqual(4, BerkasSave.Baca(folder).MalamKe);
            Assert.IsFalse(File.Exists(BerkasSave.Jalur(folder) + ".tmp"));
        }

        [Test]
        public void FileRusakDiabaikanDenganPeringatan()
        {
            Directory.CreateDirectory(folder);
            File.WriteAllText(BerkasSave.Jalur(folder), "rusak");
            LogAssert.Expect(LogType.Warning, new Regex("Save rusak"));
            Assert.IsNull(BerkasSave.Baca(folder));
        }

        [Test]
        public void TertangkapMengurangiLimaDanPenaltiTersimpan()
        {
            Checkpoint.Simpan(folder, new GameState { MalamKe = 4, KondisiIbu = 40 });
            Assert.AreEqual(35, Checkpoint.MuatSetelahTertangkap(folder).KondisiIbu);
            Assert.AreEqual(30, Checkpoint.MuatSetelahTertangkap(folder).KondisiIbu);
            Assert.AreEqual(30, BerkasSave.Baca(folder).KondisiIbu);
        }

        [Test]
        public void TertangkapTanpaCheckpointMengembalikanNull()
        {
            Assert.IsNull(Checkpoint.MuatSetelahTertangkap(folder));
        }

        [Test]
        public void SimpanCheckpoint_FolderTidakBisaDitulis_KembalikanFalseTanpaException()
        {
            string jalurFile = folder + ".berkas";
            File.WriteAllText(jalurFile, "bukan folder");
            try
            {
                LogAssert.Expect(LogType.Warning, new Regex("Checkpoint gagal disimpan"));
                Assert.IsFalse(Checkpoint.Simpan(jalurFile, new GameState()));
            }
            finally { File.Delete(jalurFile); }
        }

        [Test]
        public void MuatSetelahTertangkap_CheckpointRusak_KembalikanNull()
        {
            Directory.CreateDirectory(folder);
            File.WriteAllText(BerkasSave.Jalur(folder), "rusak");
            LogAssert.Expect(LogType.Warning, new Regex("Save rusak"));
            Assert.IsNull(Checkpoint.MuatSetelahTertangkap(folder));
        }
    }
}
