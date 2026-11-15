# Fase 0A: Setup Proyek dan Logika Core — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Menyiapkan repo dan paket Unity, lalu membangun seluruh aturan gim The Sending sebagai C# murni di assembly `TheSending.Core` yang lulus EditMode test.

**Architecture:** Satu kelas data `GameState` (field publik, bisa diserialisasi `JsonUtility`) ditambah kelas statis kecil per aturan (`KondisiIbu`, `PenentuEnding`, `JamGim`, dan seterusnya) yang membaca dan mengubah `GameState` lalu mengumumkan perubahan lewat satu event `GameState.Berubah`. Tidak ada MonoBehaviour di Core. Runtime (rencana 0B) hanya memanggil fungsi-fungsi ini.

**Tech Stack:** Unity 6000.6.4f1, C# (.NET Standard 2.1), Unity Test Framework 1.8.0 (NUnit), Git LFS.

Fase 0 dipecah menjadi tiga rencana: **0A (ini)** setup dan Core, **0B** fondasi Runtime (pemain, interaksi, HP, Jurnal, dialog, save ke file), **0C** gaya PSX, `docs/style-guide.md`, dan `Scene_MainMenu`.

## Global Constraints

- Unity **6000.6.4f1**, URP 17.6. Paket ditambahkan dengan versi persis: `com.unity.test-framework` **1.8.0**, `com.unity.ai.navigation` **2.0.14**, `com.unity.timeline` **6.6.0**, `com.unity.probuilder` **6.1.2**.
- Target build final **hanya Windows** (Mono). Pengembangan di Mac.
- Kode Core di `Assets/_Project/Scripts/Core`, namespace `TheSending.Core`, assembly `TheSending.Core`. **Tanpa MonoBehaviour.**
- Test di `Assets/_Project/Tests/EditMode`, namespace `TheSending.Tests`, assembly `TheSending.Tests.EditMode`.
- Nama domain bahasa Indonesia sesuai GDD (`KondisiIbu`, `MalamKe`, `PapanBukti`), istilah teknis bahasa Inggris.
- Angka aturan mengikuti Game Story 6.1 dan 6.4 serta dokumen desain Bagian 4 (`docs/superpowers/specs/2026-11-15-the-sending-design.md`). Desain mengalahkan Game Story untuk aturan di Bagian 4.
- `GameState` sengaja **mutable** (field publik) karena `JsonUtility` membutuhkannya dan demi kesederhanaan. Ubah state hanya lewat kelas aturan di Core.
- Indeks array berbasis 0: M1 = 0, B1 = 0, K1 = 0, Halaman 1 = 0. Selalu pakai konstanta `Kode.*`, jangan angka mentah.
- TDD: test ditulis dan dilihat gagal dulu, baru kode.
- Commit memakai format `<type>: <deskripsi>` dan diakhiri baris `Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ`. **Jangan pernah menambahkan baris `Co-Authored-By` Claude.**

## Cara Menjalankan Test

Semua task memakai skrip `tools/run-editmode-tests.sh` (dibuat di Task 1).

- **Editor Unity harus tertutup.** Jika terbuka, Unity batchmode gagal dengan pesan "another Unity instance is running". Minta tim menutup editor, atau minta tim menjalankan Window > General > Test Runner > EditMode > **Run All** lalu melaporkan hasilnya.
- Run pertama bisa makan waktu beberapa menit karena Unity mengimpor paket. Pakai timeout 10 menit.
- `bash tools/run-editmode-tests.sh` menjalankan semua test. `bash tools/run-editmode-tests.sh TheSending.Tests.KalenderJawaTests` hanya satu kelas. Beberapa kelas dipisah dengan `;`.
- Fase merah (RED) biasanya berupa **compile error** (`error CS0246: The type or namespace name ... could not be found`), karena tipenya belum ada. Itu dihitung sebagai gagal yang diharapkan.

## Struktur File

```
.gitattributes                                   Git LFS untuk aset biner
Packages/manifest.json                           + 4 paket
tools/run-editmode-tests.sh                      skrip test CLI
Assets/_Project/Scripts/Core/
  TheSending.Core.asmdef
  Kode.cs              indeks M/B/K/Halaman
  Enums.cs             semua enum domain
  GameState.cs         data state + event Berubah
  SaveData.cs          JSON <-> GameState + validasi
  KalenderJawa.cs      hari/pasaran, urutan paku & lilin
  PuzzleUrutan.cs      validator urutan generik
  Puzzle.cs            GembokLemari, AksaraJawa, UrutanPaku, LilinSesaji, KerekanSumur, PerbandinganTanah
  PenentuEnding.cs     aturan ending final
  KondisiIbu.cs        perubahan kondisi + obat + tertangkap
  JamGim.cs            jam gim, batas waktu, jendela obat
  TujuanMalam.cs       tujuan wajib per malam
  PergantianMalam.cs   pindah malam
  MediaSantet.cs       temukan/serahkan/netralkan media
  RitualPenangkal.cs   pelajari, bahan, urutan 6 langkah
  PapanBukti.cs        bukti, Katanya, status
  BukuHarian.cs        halaman diary -> B1, B7, ritual
  KepercayaanSumi.cs   nilai 0..3, tuduh, ambang
  SidangWarga.cs       penilaian sidang + ItemPapan
  KewaspadaanSantet.cs suara, cahaya, tingkat kekuatan
Assets/_Project/Tests/EditMode/
  TheSending.Tests.EditMode.asmdef
  <Nama>Tests.cs       satu file test per area Core
  AlurCeritaTests.cs   test integrasi alur ending
```

---

### Task 1: Setup repo, paket, assembly, dan KalenderJawa

Task ini menyiapkan seluruh pipeline (LFS, paket, assembly, skrip test) dan membuktikannya dengan kelas Core pertama.

**Files:**
- Create: `.gitattributes`, `tools/run-editmode-tests.sh`
- Modify: `Packages/manifest.json`
- Create: `Assets/_Project/Scripts/Core/TheSending.Core.asmdef`, `Assets/_Project/Scripts/Core/Enums.cs`, `Assets/_Project/Scripts/Core/KalenderJawa.cs`
- Create: `Assets/_Project/Tests/EditMode/TheSending.Tests.EditMode.asmdef`
- Test: `Assets/_Project/Tests/EditMode/KalenderJawaTests.cs`

**Interfaces:**
- Produces: semua enum di `Enums.cs` (`StatusInfo`, `HasilSidang`, `PilihanAkhir`, `Ending`, `Perubahan`, `Hari`, `Pasaran`, `AksiSumi`, `BahanRitual`, `LangkahRitual`, `Gerak`); `KalenderJawa.HariMalam(int) : Hari`, `KalenderJawa.PasaranMalam(int) : Pasaran`, `KalenderJawa.NamaMalam(int) : string`, `KalenderJawa.UrutanPaku(int) : Hari[]`, `KalenderJawa.UrutanLilin(int) : Pasaran[]`.

- [ ] **Step 1: Buat `.gitattributes` dan normalisasi file biner yang sudah ter-commit**

`.gitattributes`:
```gitattributes
* text=auto

# Aset biner lewat Git LFS
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.jpeg filter=lfs diff=lfs merge=lfs -text
*.tga filter=lfs diff=lfs merge=lfs -text
*.tif filter=lfs diff=lfs merge=lfs -text
*.psd filter=lfs diff=lfs merge=lfs -text
*.exr filter=lfs diff=lfs merge=lfs -text
*.hdr filter=lfs diff=lfs merge=lfs -text
*.fbx filter=lfs diff=lfs merge=lfs -text
*.obj filter=lfs diff=lfs merge=lfs -text
*.blend filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.aif filter=lfs diff=lfs merge=lfs -text
*.ttf filter=lfs diff=lfs merge=lfs -text
*.otf filter=lfs diff=lfs merge=lfs -text
*.mp4 filter=lfs diff=lfs merge=lfs -text
```

Run:
```bash
git lfs install && git add .gitattributes && git add --renormalize . && git lfs ls-files
```
Expected: daftar file `.png` dari `Assets/TutorialInfo` muncul sebagai file LFS.

- [ ] **Step 2: Tambahkan 4 paket ke `Packages/manifest.json`**

Tambahkan baris berikut di dalam objek `"dependencies"` (urutan bebas, jaga koma JSON tetap valid):
```json
    "com.unity.ai.navigation": "2.0.14",
    "com.unity.probuilder": "6.1.2",
    "com.unity.test-framework": "1.8.0",
    "com.unity.timeline": "6.6.0",
```

Run: `python3 -m json.tool Packages/manifest.json > /dev/null && echo JSON OK`
Expected: `JSON OK`

- [ ] **Step 3: Buat skrip test `tools/run-editmode-tests.sh`**

```bash
#!/usr/bin/env bash
# Menjalankan EditMode test The Sending lewat command line. Editor Unity harus tertutup.
# Pemakaian: bash tools/run-editmode-tests.sh [testFilter]
set -euo pipefail
UNITY="/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity"
PROJECT="$(cd "$(dirname "$0")/.." && pwd)"
OUT="${TMPDIR:-/tmp}/the-sending-editmode.xml"
LOG="${TMPDIR:-/tmp}/the-sending-editmode.log"
rm -f "$OUT"

args=(-batchmode -projectPath "$PROJECT" -runTests -testPlatform EditMode -testResults "$OUT" -logFile "$LOG")
if [ -n "${1:-}" ]; then args+=(-testFilter "$1"); fi

set +e
"$UNITY" "${args[@]}"
code=$?
set -e

if [ -f "$OUT" ]; then
  grep -o -m1 'result="[^"]*" total="[0-9]*" passed="[0-9]*" failed="[0-9]*"' "$OUT"
  grep -A4 '<test-case.*result="Failed"' "$OUT" | grep -E 'fullname=|<message>' | sed 's/^ *//' | head -40 || true
else
  echo "Tidak ada hasil test (exit $code). Penyebab umum: editor masih terbuka atau compile error. Log: $LOG"
  grep -E 'error CS|another Unity instance' "$LOG" | sort -u | head -20 || true
fi
exit $code
```

Run: `chmod +x tools/run-editmode-tests.sh`

- [ ] **Step 4: Buat dua file asmdef**

`Assets/_Project/Scripts/Core/TheSending.Core.asmdef`:
```json
{
    "name": "TheSending.Core",
    "rootNamespace": "TheSending.Core",
    "references": [],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

`Assets/_Project/Tests/EditMode/TheSending.Tests.EditMode.asmdef`:
```json
{
    "name": "TheSending.Tests.EditMode",
    "rootNamespace": "TheSending.Tests",
    "references": [
        "UnityEngine.TestRunner",
        "UnityEditor.TestRunner",
        "TheSending.Core"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": true,
    "precompiledReferences": [
        "nunit.framework.dll"
    ],
    "autoReferenced": false,
    "defineConstraints": [
        "UNITY_INCLUDE_TESTS"
    ],
    "versionDefines": [],
    "noEngineReferences": false
}
```

- [ ] **Step 5: Tulis test KalenderJawa yang gagal**

`Assets/_Project/Tests/EditMode/KalenderJawaTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class KalenderJawaTests
    {
        // Tabel Game Story 3.1
        [TestCase(0, Hari.Rabu, Pasaran.Wage)]   // Prolog = malam yang sama dengan Malam 1
        [TestCase(1, Hari.Rabu, Pasaran.Wage)]
        [TestCase(2, Hari.Kamis, Pasaran.Kliwon)]
        [TestCase(3, Hari.Jumat, Pasaran.Legi)]
        [TestCase(4, Hari.Sabtu, Pasaran.Pahing)]
        [TestCase(5, Hari.Minggu, Pasaran.Pon)]
        [TestCase(6, Hari.Senin, Pasaran.Wage)]
        [TestCase(7, Hari.Selasa, Pasaran.Kliwon)]
        public void HariDanPasaranSesuaiTabelGameStory(int malam, Hari hari, Pasaran pasaran)
        {
            Assert.AreEqual(hari, KalenderJawa.HariMalam(malam));
            Assert.AreEqual(pasaran, KalenderJawa.PasaranMalam(malam));
        }

        [Test]
        public void NamaMalam7AdalahMalamSelasaKliwon()
        {
            Assert.AreEqual("Malam Selasa Kliwon", KalenderJawa.NamaMalam(7));
        }

        [Test]
        public void UrutanPakuMalam3DimulaiDariJumat()
        {
            CollectionAssert.AreEqual(
                new[] { Hari.Jumat, Hari.Sabtu, Hari.Minggu, Hari.Senin, Hari.Selasa, Hari.Rabu, Hari.Kamis },
                KalenderJawa.UrutanPaku(3));
        }

        [Test]
        public void UrutanLilinMalam7DimulaiDariKliwon()
        {
            CollectionAssert.AreEqual(
                new[] { Pasaran.Kliwon, Pasaran.Legi, Pasaran.Pahing, Pasaran.Pon, Pasaran.Wage },
                KalenderJawa.UrutanLilin(7));
        }

        [TestCase(-1)]
        [TestCase(8)]
        public void MalamDiLuarRentangDitolak(int malam)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => KalenderJawa.HariMalam(malam));
        }
    }
}
```

- [ ] **Step 6: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.KalenderJawaTests` (timeout 10 menit)
Expected: tidak ada hasil test, log berisi `error CS0103` atau `error CS0246` untuk `KalenderJawa`/`Hari`.

- [ ] **Step 7: Buat `Enums.cs` dan `KalenderJawa.cs`**

`Assets/_Project/Scripts/Core/Enums.cs`:
```csharp
namespace TheSending.Core
{
    public enum StatusInfo { Katanya, Bukti, Terbantahkan }
    public enum HasilSidang { Belum, Berhasil, Gagal, Diam, Menuduh }
    public enum PilihanAkhir { Belum, Lapor, Balas }
    public enum Ending { Belum, Terlambat, Fitnah, BalasDendam, Sejati }
    public enum Perubahan { Kondisi, Media, Bukti, Katanya, Kepercayaan, Ritual, Diary, Sidang, Jam, Tujuan, Malam, Ending }
    public enum Hari { Minggu, Senin, Selasa, Rabu, Kamis, Jumat, Sabtu }
    public enum Pasaran { Legi, Pahing, Pon, Wage, Kliwon }
    public enum AksiSumi { MintaMaaf, BantuLampuDanAir, BelaDiPosRonda }
    public enum BahanRitual { Garam, Kelor, BambuKuning }
    public enum LangkahRitual { LetakkanMedia, TaburGaram, TutupKelor, NyalakanApi, AdukBambu, KuburAbu }
    public enum Gerak { Diam, Jalan, Lari }
}
```

`Assets/_Project/Scripts/Core/KalenderJawa.cs`:
```csharp
using System;

namespace TheSending.Core
{
    // Pergantian hari Jawa dimulai saat magrib. Prolog dan Malam 1 = malam Rabu Wage (Game Story 3.1).
    public static class KalenderJawa
    {
        const int JumlahHari = 7, JumlahPasaran = 5;

        public static Hari HariMalam(int malam) =>
            (Hari)(((int)Hari.Rabu + Urutan(malam)) % JumlahHari);

        public static Pasaran PasaranMalam(int malam) =>
            (Pasaran)(((int)Pasaran.Wage + Urutan(malam)) % JumlahPasaran);

        public static string NamaMalam(int malam) => $"Malam {HariMalam(malam)} {PasaranMalam(malam)}";

        public static Hari[] UrutanPaku(int malam)
        {
            var hasil = new Hari[JumlahHari];
            int mulai = (int)HariMalam(malam);
            for (int i = 0; i < JumlahHari; i++) hasil[i] = (Hari)((mulai + i) % JumlahHari);
            return hasil;
        }

        public static Pasaran[] UrutanLilin(int malam)
        {
            var hasil = new Pasaran[JumlahPasaran];
            int mulai = (int)PasaranMalam(malam);
            for (int i = 0; i < JumlahPasaran; i++) hasil[i] = (Pasaran)((mulai + i) % JumlahPasaran);
            return hasil;
        }

        // Jarak (dalam malam) dari Malam 1. Prolog dihitung sama dengan Malam 1.
        static int Urutan(int malam)
        {
            if (malam < 0 || malam > 7) throw new ArgumentOutOfRangeException(nameof(malam), malam, "Malam harus 0..7.");
            return Math.Max(malam, 1) - 1;
        }
    }
}
```

- [ ] **Step 8: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.KalenderJawaTests`
Expected: `result="Passed" total="13" passed="13" failed="0"`

- [ ] **Step 9: Commit (termasuk file `.meta` yang dibuat Unity dan `packages-lock.json`)**

```bash
git add .gitattributes Packages/ tools/ Assets/_Project Assets/_Project.meta
git status --short   # pastikan tidak ada file Library/ atau Temp/
git commit -m "chore: set up LFS, packages, Core and test assemblies with KalenderJawa"
```

---

### Task 2: Kode, GameState, dan SaveData

**Files:**
- Create: `Assets/_Project/Scripts/Core/Kode.cs`, `Assets/_Project/Scripts/Core/GameState.cs`, `Assets/_Project/Scripts/Core/SaveData.cs`
- Test: `Assets/_Project/Tests/EditMode/SaveDataTests.cs`

**Interfaces:**
- Consumes: semua enum dari Task 1.
- Produces:
  - `Kode.M1..M7`, `Kode.B1..B8`, `Kode.K1..K4`, `Kode.H1..H8` (int berbasis 0), `Kode.JumlahMedia = 7`, `JumlahBukti = 8`, `JumlahKatanya = 4`, `JumlahHalaman = 8`, `JumlahAksiSumi = 3`, `JumlahBahanRitual = 3`.
  - `GameState` dengan field: `int MalamKe`, `float MenitGim`, `int KondisiIbu`, `bool[] MediaDitemukan`, `int[] MalamMediaDitemukan`, `bool[] MediaDinetralkan`, `bool[] MediaDiserahkan`, `bool[] BuktiKunci`, `bool[] KatanyaDiketahui`, `StatusInfo[] StatusKatanya`, `bool[] HalamanDiary`, `int KepercayaanSumi`, `bool[] AksiSumiDipakai`, `int PintuSumiTerbukaMulaiMalam`, `bool PunyaMinyakSereh`, `bool RitualDiketahui`, `bool[] BahanRitual`, `HasilSidang HasilSidang`, `PilihanAkhir PilihanAkhir`, `bool RitualTerakhirSelesai`, `bool LolosKejaranWiryo`, `bool ObatSudahDiberikan`, `List<string> TujuanSelesai`, `Ending Ending`; properti `bool JalurFitnah`; event `Action<Perubahan> Berubah`; metode `void Umumkan(Perubahan)`.
  - `SaveData.KeJson(GameState) : string`, `SaveData.DariJson(string) : GameState` (melempar `FormatException` untuk data kosong, rusak, atau di luar rentang).

Catatan: nilai awal `MenitGim` (1050 = 17.30) dan `KondisiIbu` (60) ditulis langsung di `GameState` dengan komentar. Konstanta resminya dibuat di `KondisiIbu.Awal` (Task 5) dan `JamGim.MulaiProlog` (Task 6), dan test di task-task itu memastikan nilainya sama.

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/SaveDataTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class SaveDataTests
    {
        [Test]
        public void StateBaruDimulaiDiPrologDenganKondisi60()
        {
            var s = new GameState();
            Assert.AreEqual(0, s.MalamKe);
            Assert.AreEqual(60, s.KondisiIbu);
            Assert.AreEqual(1050f, s.MenitGim);
            Assert.AreEqual(Kode.JumlahMedia, s.MediaDitemukan.Length);
            Assert.AreEqual(Ending.Belum, s.Ending);
            Assert.IsFalse(s.JalurFitnah);
        }

        [Test]
        public void SimpanLaluMuatMengembalikanIsiYangSama()
        {
            var s = new GameState { MalamKe = 4, KondisiIbu = 37, MenitGim = 1290f, KepercayaanSumi = 2 };
            s.MediaDitemukan[Kode.M3] = true;
            s.BuktiKunci[Kode.B4] = true;
            s.StatusKatanya[Kode.K3] = StatusInfo.Terbantahkan;
            s.TujuanSelesai.Add("CabutPaku");

            var hasil = SaveData.DariJson(SaveData.KeJson(s));

            Assert.AreEqual(4, hasil.MalamKe);
            Assert.AreEqual(37, hasil.KondisiIbu);
            Assert.AreEqual(1290f, hasil.MenitGim);
            Assert.AreEqual(2, hasil.KepercayaanSumi);
            Assert.IsTrue(hasil.MediaDitemukan[Kode.M3]);
            Assert.IsTrue(hasil.BuktiKunci[Kode.B4]);
            Assert.AreEqual(StatusInfo.Terbantahkan, hasil.StatusKatanya[Kode.K3]);
            CollectionAssert.AreEqual(new[] { "CabutPaku" }, hasil.TujuanSelesai);
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        [TestCase("ini bukan json")]
        public void DataKosongAtauRusakDitolak(string json)
        {
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void MalamDiLuarRentangDitolak()
        {
            var json = SaveData.KeJson(new GameState()).Replace("\"MalamKe\":0", "\"MalamKe\":9");
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void KondisiDiLuarRentangDitolak()
        {
            var json = SaveData.KeJson(new GameState()).Replace("\"KondisiIbu\":60", "\"KondisiIbu\":150");
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void PanjangArraySalahDitolak()
        {
            var json = SaveData.KeJson(new GameState()).Replace(
                "\"BuktiKunci\":[false,false,false,false,false,false,false,false]",
                "\"BuktiKunci\":[false]");
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [Test]
        public void EventBerubahMengirimJenisPerubahan()
        {
            var s = new GameState();
            Perubahan? diterima = null;
            s.Berubah += p => diterima = p;
            s.Umumkan(Perubahan.Kondisi);
            Assert.AreEqual(Perubahan.Kondisi, diterima);
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.SaveDataTests`
Expected: compile error, `GameState`/`Kode`/`SaveData` tidak ditemukan.

- [ ] **Step 3: Buat `Kode.cs`, `GameState.cs`, `SaveData.cs`**

`Assets/_Project/Scripts/Core/Kode.cs`:
```csharp
namespace TheSending.Core
{
    // Indeks array di GameState. Nomor mengikuti Game Story (M1 = indeks 0, dan seterusnya).
    public static class Kode
    {
        public const int M1 = 0, M2 = 1, M3 = 2, M4 = 3, M5 = 4, M6 = 5, M7 = 6;
        public const int B1 = 0, B2 = 1, B3 = 2, B4 = 3, B5 = 4, B6 = 5, B7 = 6, B8 = 7;
        public const int K1 = 0, K2 = 1, K3 = 2, K4 = 3;
        public const int H1 = 0, H2 = 1, H3 = 2, H4 = 3, H5 = 4, H6 = 5, H7 = 6, H8 = 7;

        public const int JumlahMedia = 7, JumlahBukti = 8, JumlahKatanya = 4, JumlahHalaman = 8;
        public const int JumlahAksiSumi = 3, JumlahBahanRitual = 3;
    }
}
```

`Assets/_Project/Scripts/Core/GameState.cs`:
```csharp
using System;
using System.Collections.Generic;

namespace TheSending.Core
{
    // Field publik agar bisa diserialisasi JsonUtility. Ubah hanya lewat kelas aturan di Core.
    [Serializable]
    public class GameState
    {
        public int MalamKe;                 // 0 = Prolog, 1..7 = Malam 1..7
        public float MenitGim = 1050f;      // = JamGim.MulaiProlog (17.30), dicek oleh JamGimTests
        public int KondisiIbu = 60;         // = KondisiIbu.Awal, dicek oleh KondisiIbuTests

        public bool[] MediaDitemukan = new bool[Kode.JumlahMedia];
        public int[] MalamMediaDitemukan = new int[Kode.JumlahMedia];
        public bool[] MediaDinetralkan = new bool[Kode.JumlahMedia];
        public bool[] MediaDiserahkan = new bool[Kode.JumlahMedia];

        public bool[] BuktiKunci = new bool[Kode.JumlahBukti];
        public bool[] KatanyaDiketahui = new bool[Kode.JumlahKatanya];
        public StatusInfo[] StatusKatanya = new StatusInfo[Kode.JumlahKatanya];
        public bool[] HalamanDiary = new bool[Kode.JumlahHalaman];

        public int KepercayaanSumi;
        public bool[] AksiSumiDipakai = new bool[Kode.JumlahAksiSumi];
        public int PintuSumiTerbukaMulaiMalam;
        public bool PunyaMinyakSereh;

        public bool RitualDiketahui;
        public bool[] BahanRitual = new bool[Kode.JumlahBahanRitual];

        public HasilSidang HasilSidang;
        public PilihanAkhir PilihanAkhir;
        public bool RitualTerakhirSelesai;
        public bool LolosKejaranWiryo;

        public bool ObatSudahDiberikan;
        public List<string> TujuanSelesai = new List<string>();
        public Ending Ending;

        [field: NonSerialized] public event Action<Perubahan> Berubah;

        public bool JalurFitnah =>
            HasilSidang == HasilSidang.Gagal || HasilSidang == HasilSidang.Diam || HasilSidang == HasilSidang.Menuduh;

        public void Umumkan(Perubahan perubahan) => Berubah?.Invoke(perubahan);
    }
}
```

`Assets/_Project/Scripts/Core/SaveData.cs`:
```csharp
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
        }

        static void Rentang(int nilai, int min, int maks, string nama)
        {
            if (nilai < min || nilai > maks) throw new FormatException($"{nama} = {nilai}, harus {min}..{maks}.");
        }

        static void Panjang(Array a, int n, string nama)
        {
            if (a == null || a.Length != n) throw new FormatException($"{nama} harus berisi {n} elemen.");
        }
    }
}
```

- [ ] **Step 4: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.SaveDataTests`
Expected: `result="Passed" total="10" passed="10" failed="0"`

- [ ] **Step 5: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add GameState, Kode indices and validated JSON SaveData"
```

---

### Task 3: PuzzleUrutan dan validator puzzle

**Files:**
- Create: `Assets/_Project/Scripts/Core/PuzzleUrutan.cs`, `Assets/_Project/Scripts/Core/Puzzle.cs`
- Test: `Assets/_Project/Tests/EditMode/PuzzleTests.cs`

**Interfaces:**
- Consumes: `KalenderJawa.UrutanPaku`, `KalenderJawa.UrutanLilin` (Task 1).
- Produces:
  - `class PuzzleUrutan<T>(IList<T> kunci, bool ulangDariAwalSaatSalah)`: `bool Pilih(T item)`, `int Kemajuan`, `bool Selesai`.
  - `GembokLemari.KodeBenar = "2003"`, `GembokLemari.Cek(string) : bool`.
  - `AksaraJawa.Ratmi`, `AksaraJawa.SelasaKliwon` (string[]), `AksaraJawa.Buat(string[]) : PuzzleUrutan<string>`.
  - `UrutanPaku.Buat(int malam) : PuzzleUrutan<Hari>` (salah tidak mengulang; penalti diurus Runtime lewat `KondisiIbu.SalahUrutanPaku`).
  - `LilinSesaji.Buat(int malam) : PuzzleUrutan<Pasaran>` (salah tidak mengulang).
  - `KerekanSumur.KecepatanTarikMaks = 0.6f`, `KerekanSumur.TarikanAman(float) : bool`.
  - `class PerbandinganTanah`: `bool Bandingkan(int titik)` (selalu `false`), `bool SemuaSudahDibandingkan`.

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/PuzzleTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PuzzleTests
    {
        [Test]
        public void UrutanBenarMenyelesaikanPuzzle()
        {
            var p = new PuzzleUrutan<int>(new[] { 1, 2, 3 }, ulangDariAwalSaatSalah: false);
            Assert.IsTrue(p.Pilih(1));
            Assert.IsTrue(p.Pilih(2));
            Assert.IsFalse(p.Selesai);
            Assert.IsTrue(p.Pilih(3));
            Assert.IsTrue(p.Selesai);
        }

        [Test]
        public void SalahDenganUlangDariAwalMengembalikanKemajuanKeNol()
        {
            var p = new PuzzleUrutan<int>(new[] { 1, 2, 3 }, ulangDariAwalSaatSalah: true);
            p.Pilih(1);
            Assert.IsFalse(p.Pilih(3));
            Assert.AreEqual(0, p.Kemajuan);
        }

        [Test]
        public void SalahTanpaUlangMempertahankanKemajuan()
        {
            var p = new PuzzleUrutan<int>(new[] { 1, 2, 3 }, ulangDariAwalSaatSalah: false);
            p.Pilih(1);
            Assert.IsFalse(p.Pilih(3));
            Assert.AreEqual(1, p.Kemajuan);
        }

        [Test]
        public void PilihSetelahSelesaiDitolak()
        {
            var p = new PuzzleUrutan<int>(new[] { 1 }, false);
            p.Pilih(1);
            Assert.IsFalse(p.Pilih(1));
        }

        [Test]
        public void KunciKosongDitolak()
        {
            Assert.Throws<ArgumentException>(() => new PuzzleUrutan<int>(new int[0], false));
        }

        [TestCase("2003", true)]
        [TestCase("2030", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void GembokLemariHanyaTerbukaDengan2003(string kode, bool terbuka)
        {
            Assert.AreEqual(terbuka, GembokLemari.Cek(kode));
        }

        [Test]
        public void AksaraRatmiDibacaBerurutan()
        {
            var p = AksaraJawa.Buat(AksaraJawa.Ratmi);
            foreach (var aksara in AksaraJawa.Ratmi) Assert.IsTrue(p.Pilih(aksara));
            Assert.IsTrue(p.Selesai);
        }

        [Test]
        public void PakuMalam3HarusDimulaiDariJumat()
        {
            var p = UrutanPaku.Buat(3);
            Assert.IsFalse(p.Pilih(Hari.Minggu));
            Assert.IsTrue(p.Pilih(Hari.Jumat));
            Assert.IsTrue(p.Pilih(Hari.Sabtu));
        }

        [Test]
        public void LilinMalam7HarusDimulaiDariKliwon()
        {
            var p = LilinSesaji.Buat(7);
            Assert.IsFalse(p.Pilih(Pasaran.Legi));
            foreach (var pasaran in KalenderJawa.UrutanLilin(7)) Assert.IsTrue(p.Pilih(pasaran));
            Assert.IsTrue(p.Selesai);
        }

        [TestCase(0.5f, true)]
        [TestCase(0.6f, true)]
        [TestCase(0.61f, false)]
        public void KerekanJatuhJikaDitarikTerlaluCepat(float kecepatan, bool aman)
        {
            Assert.AreEqual(aman, KerekanSumur.TarikanAman(kecepatan));
        }

        [Test]
        public void TanahMakamTidakPernahCocokDanSelesaiSetelahTigaTitik()
        {
            var t = new PerbandinganTanah();
            Assert.IsFalse(t.Bandingkan(0));
            Assert.IsFalse(t.Bandingkan(1));
            Assert.IsFalse(t.SemuaSudahDibandingkan);
            Assert.IsFalse(t.Bandingkan(2));
            Assert.IsTrue(t.SemuaSudahDibandingkan);
            Assert.Throws<ArgumentOutOfRangeException>(() => t.Bandingkan(3));
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PuzzleTests`
Expected: compile error, `PuzzleUrutan`/`GembokLemari` tidak ditemukan.

- [ ] **Step 3: Buat `PuzzleUrutan.cs` dan `Puzzle.cs`**

`Assets/_Project/Scripts/Core/PuzzleUrutan.cs`:
```csharp
using System;
using System.Collections.Generic;

namespace TheSending.Core
{
    // Puzzle "pilih berurutan": aksara, paku, lilin, dan langkah ritual.
    public class PuzzleUrutan<T>
    {
        readonly T[] kunci;
        readonly bool ulangDariAwalSaatSalah;

        public int Kemajuan { get; private set; }
        public bool Selesai => Kemajuan == kunci.Length;

        public PuzzleUrutan(IList<T> kunci, bool ulangDariAwalSaatSalah)
        {
            if (kunci == null || kunci.Count == 0) throw new ArgumentException("Kunci puzzle kosong.", nameof(kunci));
            this.kunci = new List<T>(kunci).ToArray();
            this.ulangDariAwalSaatSalah = ulangDariAwalSaatSalah;
        }

        // true jika item benar. Salah: kembali ke awal jika ulangDariAwalSaatSalah, selain itu kemajuan tetap.
        public bool Pilih(T item)
        {
            if (Selesai) return false;
            if (EqualityComparer<T>.Default.Equals(item, kunci[Kemajuan]))
            {
                Kemajuan++;
                return true;
            }
            if (ulangDariAwalSaatSalah) Kemajuan = 0;
            return false;
        }
    }
}
```

`Assets/_Project/Scripts/Core/Puzzle.cs`:
```csharp
using System;

namespace TheSending.Core
{
    // Malam 1: kode gembok = tahun rumah berdiri, terukir di blandar.
    public static class GembokLemari
    {
        public const string KodeBenar = "2003";
        public static bool Cek(string kode) => kode == KodeBenar;
    }

    // Malam 2: satu entri = satu aksara (dengan sandhangan) yang dipilih dari tabel Halaman 2.
    public static class AksaraJawa
    {
        public static readonly string[] Ratmi = { "RA", "T", "MI" };
        public static readonly string[] SelasaKliwon = { "SE", "LA", "SA", "KLI", "WO", "N" };

        public static PuzzleUrutan<string> Buat(string[] kata) => new PuzzleUrutan<string>(kata, false);
    }

    // Malam 3: salah urutan tidak mengulang, tetapi Runtime memberi penalti KondisiIbu.SalahUrutanPaku.
    public static class UrutanPaku
    {
        public static PuzzleUrutan<Hari> Buat(int malam) => new PuzzleUrutan<Hari>(KalenderJawa.UrutanPaku(malam), false);
    }

    // Malam 7: salah urutan membuat Mbah Wiryo bangun lebih cepat (diurus Runtime).
    public static class LilinSesaji
    {
        public static PuzzleUrutan<Pasaran> Buat(int malam) => new PuzzleUrutan<Pasaran>(KalenderJawa.UrutanLilin(malam), false);
    }

    // Malam 4: tali yang ditarik lebih cepat dari batas membuat bungkusan jatuh kembali.
    public static class KerekanSumur
    {
        public const float KecepatanTarikMaks = 0.6f; // meter per detik, disetel saat playtest
        public static bool TarikanAman(float kecepatanTarik) => kecepatanTarik <= KecepatanTarikMaks;
    }

    // Malam 5: tiga sampel tanah makam dibandingkan dengan tanah kendi, tidak ada yang cocok.
    public class PerbandinganTanah
    {
        public const int JumlahTitik = 3;
        readonly bool[] sudah = new bool[JumlahTitik];

        public bool SemuaSudahDibandingkan => Array.TrueForAll(sudah, x => x);

        public bool Bandingkan(int titik)
        {
            if (titik < 0 || titik >= JumlahTitik) throw new ArgumentOutOfRangeException(nameof(titik));
            sudah[titik] = true;
            return false; // tanah makam Sumberwangi hitam berpasir, tidak pernah cocok
        }
    }
}
```

- [ ] **Step 4: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PuzzleTests`
Expected: `result="Passed" total="16" passed="16" failed="0"`

- [ ] **Step 5: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add sequence puzzle and puzzle validators"
```

---

### Task 4: PenentuEnding

**Files:**
- Create: `Assets/_Project/Scripts/Core/PenentuEnding.cs`
- Test: `Assets/_Project/Tests/EditMode/PenentuEndingTests.cs`

**Interfaces:**
- Consumes: `GameState` (Task 2).
- Produces: `PenentuEnding.TengahMalam = 1440f`, `PenentuEnding.Hitung(GameState) : Ending` (tanpa efek samping), `PenentuEnding.Cek(GameState) : Ending` (menyimpan ke `s.Ending` dan mengumumkan `Perubahan.Ending` sekali).

Aturan (desain 4.1): ending yang sudah ditetapkan tidak berubah. Lalu urutannya: (1) jalur Fitnah → Fitnah saat Malam 7 pukul 00.00, sebelum itu Belum; (2) Kondisi 0 → Terlambat; (3) Malam 7 pukul 00.00 di jalur utama → Terlambat; (4) Balas + lolos → Balas Dendam; (5) ritual terakhir + Lapor + sidang berhasil → Sejati.

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/PenentuEndingTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PenentuEndingTests
    {
        static GameState Malam7(HasilSidang sidang, float menit = 1300f) =>
            new GameState { MalamKe = 7, HasilSidang = sidang, MenitGim = menit, KondisiIbu = 30 };

        [Test]
        public void StateAwalBelumAdaEnding()
        {
            Assert.AreEqual(Ending.Belum, PenentuEnding.Hitung(new GameState()));
        }

        [Test]
        public void KondisiNolSebelumSidangAdalahTerlambat()
        {
            var s = new GameState { MalamKe = 4, KondisiIbu = 0 };
            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Hitung(s));
        }

        [TestCase(HasilSidang.Gagal)]
        [TestCase(HasilSidang.Diam)]
        [TestCase(HasilSidang.Menuduh)]
        public void JalurFitnahBelumBerakhirSebelumTengahMalam(HasilSidang sidang)
        {
            Assert.AreEqual(Ending.Belum, PenentuEnding.Hitung(Malam7(sidang)));
        }

        [TestCase(HasilSidang.Gagal)]
        [TestCase(HasilSidang.Diam)]
        [TestCase(HasilSidang.Menuduh)]
        public void JalurFitnahBerakhirFitnahTepatTengahMalam(HasilSidang sidang)
        {
            Assert.AreEqual(Ending.Fitnah, PenentuEnding.Hitung(Malam7(sidang, PenentuEnding.TengahMalam)));
        }

        [Test]
        public void JalurFitnahTidakPernahMenjadiTerlambatWalauKondisiNol()
        {
            var s = Malam7(HasilSidang.Gagal, PenentuEnding.TengahMalam);
            s.KondisiIbu = 0;
            Assert.AreEqual(Ending.Fitnah, PenentuEnding.Hitung(s));
        }

        [Test]
        public void SidangGagalDiMalam6BelumBerakhir()
        {
            var s = new GameState { MalamKe = 6, HasilSidang = HasilSidang.Gagal, KondisiIbu = 0 };
            Assert.AreEqual(Ending.Belum, PenentuEnding.Hitung(s));
        }

        [Test]
        public void KondisiNolSetelahSidangBerhasilAdalahTerlambat()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.KondisiIbu = 0;
            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Hitung(s));
        }

        [Test]
        public void WaktuHabisDiJalurUtamaTanpaRitualAdalahTerlambat()
        {
            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Hitung(Malam7(HasilSidang.Berhasil, PenentuEnding.TengahMalam)));
        }

        [Test]
        public void BalasDanLolosSebelumTengahMalamAdalahBalasDendam()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.PilihanAkhir = PilihanAkhir.Balas;
            s.LolosKejaranWiryo = true;
            Assert.AreEqual(Ending.BalasDendam, PenentuEnding.Hitung(s));
        }

        [Test]
        public void BalasTetapiWaktuHabisSebelumLolosAdalahTerlambat()
        {
            var s = Malam7(HasilSidang.Berhasil, PenentuEnding.TengahMalam);
            s.PilihanAkhir = PilihanAkhir.Balas;
            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Hitung(s));
        }

        [Test]
        public void BalasBelumLolosSebelumTengahMalamBelumBerakhir()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.PilihanAkhir = PilihanAkhir.Balas;
            Assert.AreEqual(Ending.Belum, PenentuEnding.Hitung(s));
        }

        [Test]
        public void LaporDanRitualSelesaiSebelumTengahMalamAdalahSejati()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.PilihanAkhir = PilihanAkhir.Lapor;
            s.RitualTerakhirSelesai = true;
            Assert.AreEqual(Ending.Sejati, PenentuEnding.Hitung(s));
        }

        [Test]
        public void CekMenyimpanEndingDanMengumumkanSekali()
        {
            var s = new GameState { MalamKe = 3, KondisiIbu = 0 };
            int jumlah = 0;
            s.Berubah += p => { if (p == Perubahan.Ending) jumlah++; };

            Assert.AreEqual(Ending.Terlambat, PenentuEnding.Cek(s));
            PenentuEnding.Cek(s);

            Assert.AreEqual(Ending.Terlambat, s.Ending);
            Assert.AreEqual(1, jumlah);
        }

        [Test]
        public void EndingYangSudahDitetapkanTidakBerubah()
        {
            var s = Malam7(HasilSidang.Berhasil);
            s.PilihanAkhir = PilihanAkhir.Lapor;
            s.RitualTerakhirSelesai = true;
            PenentuEnding.Cek(s);

            s.MenitGim = PenentuEnding.TengahMalam;
            s.KondisiIbu = 0;

            Assert.AreEqual(Ending.Sejati, PenentuEnding.Cek(s));
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PenentuEndingTests`
Expected: compile error, `PenentuEnding` tidak ditemukan.

- [ ] **Step 3: Buat `PenentuEnding.cs`**

```csharp
namespace TheSending.Core
{
    // Aturan ending final, dokumen desain Bagian 4.1.
    public static class PenentuEnding
    {
        public const float TengahMalam = 24 * 60f;

        public static Ending Hitung(GameState s)
        {
            if (s.Ending != Ending.Belum) return s.Ending;

            bool waktuHabis = s.MalamKe == 7 && s.MenitGim >= TengahMalam;

            if (s.JalurFitnah) return waktuHabis ? Ending.Fitnah : Ending.Belum;
            if (s.KondisiIbu <= 0) return Ending.Terlambat;
            if (waktuHabis) return Ending.Terlambat;
            if (s.PilihanAkhir == PilihanAkhir.Balas && s.LolosKejaranWiryo) return Ending.BalasDendam;
            if (s.RitualTerakhirSelesai && s.PilihanAkhir == PilihanAkhir.Lapor && s.HasilSidang == HasilSidang.Berhasil)
                return Ending.Sejati;
            return Ending.Belum;
        }

        // Dipanggil setiap ada perubahan yang bisa memicu ending. Ending pertama yang tercapai bersifat tetap.
        public static Ending Cek(GameState s)
        {
            var e = Hitung(s);
            if (e != Ending.Belum && s.Ending == Ending.Belum)
            {
                s.Ending = e;
                s.Umumkan(Perubahan.Ending);
            }
            return e;
        }
    }
}
```

- [ ] **Step 4: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PenentuEndingTests`
Expected: `result="Passed" total="18" passed="18" failed="0"`

- [ ] **Step 5: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add PenentuEnding with final ending rules"
```

---

### Task 5: KondisiIbu

**Files:**
- Create: `Assets/_Project/Scripts/Core/KondisiIbu.cs`
- Test: `Assets/_Project/Tests/EditMode/KondisiIbuTests.cs`

**Interfaces:**
- Consumes: `GameState`, `PenentuEnding.Cek` (Task 4), `SaveData` (Task 2).
- Produces: konstanta `KondisiIbu.Awal = 60`, `Min = 0`, `Maks = 100`, `MinJalurFitnah = 1`, `PergantianMalam = -12`, `Obat = 2`, `MediaDinetralkan = 8`, `MediaDiserahkan = -8`, `Tertangkap = -5`, `SalahUrutanPaku = -5`; `KondisiIbu.Ubah(GameState, int)`; `KondisiIbu.SetelahTertangkap(GameState)`. (`BeriObat` ditambahkan di Task 6.)

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/KondisiIbuTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class KondisiIbuTests
    {
        [Test]
        public void NilaiAwalGameStateSamaDenganKonstantaAwal()
        {
            Assert.AreEqual(KondisiIbu.Awal, new GameState().KondisiIbu);
        }

        [Test]
        public void UbahDibatasi0Sampai100()
        {
            var s = new GameState { KondisiIbu = 95 };
            KondisiIbu.Ubah(s, 20);
            Assert.AreEqual(100, s.KondisiIbu);
            KondisiIbu.Ubah(s, -250);
            Assert.AreEqual(0, s.KondisiIbu);
        }

        [Test]
        public void KondisiNolLangsungMenetapkanTerlambat()
        {
            var s = new GameState { MalamKe = 2, KondisiIbu = 5 };
            KondisiIbu.Ubah(s, KondisiIbu.Tertangkap);
            Assert.AreEqual(Ending.Terlambat, s.Ending);
        }

        [Test]
        public void JalurFitnahMenahanKondisiMinimalSatu()
        {
            var s = new GameState { MalamKe = 6, KondisiIbu = 5, HasilSidang = HasilSidang.Gagal };
            KondisiIbu.Ubah(s, KondisiIbu.PergantianMalam);
            Assert.AreEqual(KondisiIbu.MinJalurFitnah, s.KondisiIbu);
            Assert.AreEqual(Ending.Belum, s.Ending);
        }

        [Test]
        public void UbahMengumumkanPerubahanKondisi()
        {
            var s = new GameState();
            bool diumumkan = false;
            s.Berubah += p => diumumkan |= p == Perubahan.Kondisi;
            KondisiIbu.Ubah(s, KondisiIbu.MediaDinetralkan);
            Assert.IsTrue(diumumkan);
            Assert.AreEqual(68, s.KondisiIbu);
        }

        [Test]
        public void PenaltiTertangkapTetapBerlakuSetelahMuatCheckpoint()
        {
            // Desain 4.2: muat checkpoint dulu, baru kurangi 5.
            var checkpoint = SaveData.KeJson(new GameState { MalamKe = 4, KondisiIbu = 40 });
            var s = SaveData.DariJson(checkpoint);
            KondisiIbu.SetelahTertangkap(s);
            Assert.AreEqual(35, s.KondisiIbu);
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.KondisiIbuTests`
Expected: compile error, kelas `KondisiIbu` tidak ditemukan.

- [ ] **Step 3: Buat `KondisiIbu.cs`**

```csharp
using System;

namespace TheSending.Core
{
    // Game Story 6.1 dan desain 4.1, 4.2. Nama kelas sama dengan field GameState.KondisiIbu;
    // di dalam GameState dipakai literal yang dicek oleh KondisiIbuTests.
    public static class KondisiIbu
    {
        public const int Awal = 60, Min = 0, Maks = 100, MinJalurFitnah = 1;
        public const int PergantianMalam = -12, Obat = 2, MediaDinetralkan = 8, MediaDiserahkan = -8;
        public const int Tertangkap = -5, SalahUrutanPaku = -5;

        public static void Ubah(GameState s, int delta)
        {
            int min = s.JalurFitnah ? MinJalurFitnah : Min;
            s.KondisiIbu = Math.Clamp(s.KondisiIbu + delta, min, Maks);
            s.Umumkan(Perubahan.Kondisi);
            PenentuEnding.Cek(s);
        }

        // Panggil SETELAH checkpoint dimuat, supaya penalti tidak hilang.
        public static void SetelahTertangkap(GameState s) => Ubah(s, Tertangkap);
    }
}
```

- [ ] **Step 4: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.KondisiIbuTests`
Expected: `result="Passed" total="6" passed="6" failed="0"`

- [ ] **Step 5: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add KondisiIbu with Fitnah floor and capture penalty"
```

---

### Task 6: JamGim, obat, TujuanMalam, dan PergantianMalam

**Files:**
- Create: `Assets/_Project/Scripts/Core/JamGim.cs`, `Assets/_Project/Scripts/Core/TujuanMalam.cs`, `Assets/_Project/Scripts/Core/PergantianMalam.cs`
- Modify: `Assets/_Project/Scripts/Core/KondisiIbu.cs` (tambah `BeriObat`)
- Test: `Assets/_Project/Tests/EditMode/JamGimTests.cs`, `Assets/_Project/Tests/EditMode/MalamTests.cs`

**Interfaces:**
- Consumes: `KondisiIbu.Ubah`, `PenentuEnding.Cek`, `PenentuEnding.TengahMalam`.
- Produces:
  - `JamGim.DetikNyataPerJamGim = 250f`, `MulaiProlog = 1050f`, `MulaiMalam = 1080f`, `Jam20`, `Jam21`, `Jam22`, `Jam23`, `TengahMalam = 1440f`, `BatasLunak = 1470f`.
  - `JamGim.Batas(int malam) : float`, `JamGim.Maju(GameState, float detikNyata)`, `JamGim.BatasKerasTercapai(GameState) : bool`, `JamGim.DalamJendelaObat(GameState) : bool`, `JamGim.SisaDetikMalam7(GameState) : float`, `JamGim.Format(float menit) : string` (contoh `"21.00"`).
  - `KondisiIbu.BeriObat(GameState) : bool`.
  - Konstanta id tujuan `TujuanMalam.TibaDiRumah`, `BicaraBidan`, `MasukKamarIbu`, `BukaLemari`, `Cutscene23`, `PotretPaku`, `BacaAksara`, `PilihanWiryo`, `SembunyiPertama`, `CabutPaku`, `KunjungiSumi`, `AmbilBungkusanSumur`, `LolosKejaranSumur`, `GaliKendi`, `BandingkanTanah`, `BicaraLaras`, `SidangWarga`; `TujuanMalam.DaftarWajib(int) : IReadOnlyList<string>`, `TujuanMalam.Selesaikan(GameState, string)`, `TujuanMalam.Sudah(GameState, string) : bool`, `TujuanMalam.MalamSelesai(GameState) : bool`.
  - `PergantianMalam.BisaLanjut(GameState) : bool`, `PergantianMalam.Lanjut(GameState)` (melempar `InvalidOperationException` jika belum boleh).

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/JamGimTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class JamGimTests
    {
        [Test]
        public void NilaiAwalGameStateSamaDenganMulaiPrologDanTengahMalamSama()
        {
            Assert.AreEqual(JamGim.MulaiProlog, new GameState().MenitGim);
            Assert.AreEqual(PenentuEnding.TengahMalam, JamGim.TengahMalam);
        }

        [Test]
        public void SatuJamGimSama250DetikNyata()
        {
            var s = new GameState { MalamKe = 3, MenitGim = JamGim.MulaiMalam };
            JamGim.Maju(s, 250f);
            Assert.AreEqual(19 * 60f, s.MenitGim, 0.001f);
        }

        [TestCase(1, 1470f)]
        [TestCase(5, 1470f)]
        [TestCase(6, 1260f)]
        [TestCase(7, 1440f)]
        public void JamBerhentiDiBatasSetiapMalam(int malam, float batas)
        {
            var s = new GameState { MalamKe = malam, MenitGim = JamGim.MulaiMalam };
            JamGim.Maju(s, 100000f);
            Assert.AreEqual(batas, s.MenitGim);
        }

        [Test]
        public void BatasKerasHanyaMalam6Dan7()
        {
            var malam5 = new GameState { MalamKe = 5, MenitGim = JamGim.BatasLunak };
            var malam6 = new GameState { MalamKe = 6, MenitGim = JamGim.Jam21 };
            Assert.IsFalse(JamGim.BatasKerasTercapai(malam5));
            Assert.IsTrue(JamGim.BatasKerasTercapai(malam6));
        }

        [Test]
        public void WaktuHabisMalam7MemicuEnding()
        {
            var s = new GameState { MalamKe = 7, MenitGim = JamGim.Jam23, HasilSidang = HasilSidang.Berhasil };
            JamGim.Maju(s, 100000f);
            Assert.AreEqual(Ending.Terlambat, s.Ending);
        }

        [Test]
        public void SisaWaktuMalam7DalamDetikNyata()
        {
            var s = new GameState { MalamKe = 7, MenitGim = JamGim.Jam23 };
            Assert.AreEqual(250f, JamGim.SisaDetikMalam7(s), 0.001f);
        }

        [TestCase(1260f, "21.00")]
        [TestCase(1050f, "17.30")]
        [TestCase(1470f, "00.30")]
        public void FormatJam(float menit, string teks)
        {
            Assert.AreEqual(teks, JamGim.Format(menit));
        }

        [TestCase(1, 1259f, false)]
        [TestCase(1, 1260f, true)]
        [TestCase(1, 1319f, true)]
        [TestCase(1, 1320f, false)]
        [TestCase(6, 1200f, true)]
        [TestCase(6, 1260f, false)]
        [TestCase(0, 1260f, false)]
        public void JendelaObat(int malam, float menit, bool boleh)
        {
            Assert.AreEqual(boleh, JamGim.DalamJendelaObat(new GameState { MalamKe = malam, MenitGim = menit }));
        }

        [Test]
        public void ObatMenambah2HanyaSekaliSemalam()
        {
            var s = new GameState { MalamKe = 2, MenitGim = JamGim.Jam21, KondisiIbu = 40 };
            Assert.IsTrue(KondisiIbu.BeriObat(s));
            Assert.IsFalse(KondisiIbu.BeriObat(s));
            Assert.AreEqual(42, s.KondisiIbu);
        }

        [Test]
        public void ObatDiLuarJendelaTidakMenambah()
        {
            var s = new GameState { MalamKe = 2, MenitGim = JamGim.Jam22, KondisiIbu = 40 };
            Assert.IsFalse(KondisiIbu.BeriObat(s));
            Assert.AreEqual(40, s.KondisiIbu);
        }
    }
}
```

`Assets/_Project/Tests/EditMode/MalamTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class MalamTests
    {
        static void SelesaikanSemua(GameState s)
        {
            foreach (var id in TujuanMalam.DaftarWajib(s.MalamKe)) TujuanMalam.Selesaikan(s, id);
        }

        [Test]
        public void Malam1TidakSelesaiSebelumCutscene23()
        {
            var s = new GameState { MalamKe = 1 };
            TujuanMalam.Selesaikan(s, TujuanMalam.BukaLemari);
            TujuanMalam.Selesaikan(s, TujuanMalam.PotretPaku);
            Assert.IsFalse(TujuanMalam.MalamSelesai(s));
            TujuanMalam.Selesaikan(s, TujuanMalam.Cutscene23);
            Assert.IsTrue(TujuanMalam.MalamSelesai(s));
        }

        [Test]
        public void SetiapMalam0Sampai6PunyaTujuanWajib()
        {
            for (int malam = 0; malam <= 6; malam++)
                Assert.IsNotEmpty(TujuanMalam.DaftarWajib(malam), $"Malam {malam}");
        }

        [Test]
        public void Malam7TidakPernahSelesaiLewatTujuan()
        {
            Assert.IsFalse(TujuanMalam.MalamSelesai(new GameState { MalamKe = 7 }));
        }

        [Test]
        public void TujuanYangSamaTidakDicatatDuaKali()
        {
            var s = new GameState();
            TujuanMalam.Selesaikan(s, TujuanMalam.TibaDiRumah);
            TujuanMalam.Selesaikan(s, TujuanMalam.TibaDiRumah);
            Assert.AreEqual(1, s.TujuanSelesai.Count);
        }

        [Test]
        public void LanjutSebelumTujuanSelesaiDitolak()
        {
            Assert.Throws<InvalidOperationException>(() => PergantianMalam.Lanjut(new GameState()));
        }

        [Test]
        public void PrologKeMalam1TanpaPenguranganDanJamBerlanjut()
        {
            var s = new GameState { MenitGim = 1110f };
            SelesaikanSemua(s);
            PergantianMalam.Lanjut(s);
            Assert.AreEqual(1, s.MalamKe);
            Assert.AreEqual(60, s.KondisiIbu);
            Assert.AreEqual(1110f, s.MenitGim);
            Assert.IsEmpty(s.TujuanSelesai);
        }

        [Test]
        public void PergantianMalamMengurangi12DanMeresetJamDanObat()
        {
            var s = new GameState { MalamKe = 1, MenitGim = JamGim.BatasLunak, ObatSudahDiberikan = true };
            SelesaikanSemua(s);
            PergantianMalam.Lanjut(s);
            Assert.AreEqual(2, s.MalamKe);
            Assert.AreEqual(48, s.KondisiIbu);
            Assert.AreEqual(JamGim.MulaiMalam, s.MenitGim);
            Assert.IsFalse(s.ObatSudahDiberikan);
        }

        [Test]
        public void PergantianMalamBisaMemicuTerlambat()
        {
            var s = new GameState { MalamKe = 3, KondisiIbu = 10 };
            SelesaikanSemua(s);
            PergantianMalam.Lanjut(s);
            Assert.AreEqual(Ending.Terlambat, s.Ending);
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh "TheSending.Tests.JamGimTests;TheSending.Tests.MalamTests"`
Expected: compile error, `JamGim`/`TujuanMalam`/`PergantianMalam` tidak ditemukan.

- [ ] **Step 3: Buat `JamGim.cs`**

```csharp
using System;

namespace TheSending.Core
{
    // Desain 4.3 dan 4.4. Jam disimpan sebagai menit sejak 00.00 hari itu; lewat tengah malam lanjut di atas 1440.
    // Runtime tidak memanggil Maju saat cutscene, dialog, Jurnal, inventaris, atau jeda terbuka.
    public static class JamGim
    {
        public const float DetikNyataPerJamGim = 250f;
        public const float MulaiProlog = 17 * 60 + 30f;
        public const float MulaiMalam = 18 * 60f;
        public const float Jam20 = 20 * 60f, Jam21 = 21 * 60f, Jam22 = 22 * 60f, Jam23 = 23 * 60f;
        public const float TengahMalam = 24 * 60f;
        public const float BatasLunak = 24 * 60 + 30f;
        const float LamaJendelaObat = 60f;

        public static float Batas(int malam) => malam == 6 ? Jam21 : malam == 7 ? TengahMalam : BatasLunak;

        public static void Maju(GameState s, float detikNyata)
        {
            if (detikNyata <= 0f) return;
            float baru = Math.Min(s.MenitGim + detikNyata * 60f / DetikNyataPerJamGim, Batas(s.MalamKe));
            if (baru <= s.MenitGim) return;
            s.MenitGim = baru;
            s.Umumkan(Perubahan.Jam);
            if (s.MalamKe == 7) PenentuEnding.Cek(s);
        }

        // Malam 6: warga berkumpul (Sidang). Malam 7: tengah malam.
        public static bool BatasKerasTercapai(GameState s) =>
            (s.MalamKe == 6 || s.MalamKe == 7) && s.MenitGim >= Batas(s.MalamKe);

        public static bool DalamJendelaObat(GameState s)
        {
            if (s.MalamKe < 1) return false;
            float mulai = s.MalamKe == 6 ? Jam20 : Jam21;
            return s.MenitGim >= mulai && s.MenitGim < mulai + LamaJendelaObat;
        }

        public static float SisaDetikMalam7(GameState s) =>
            s.MalamKe != 7 ? 0f : Math.Max(0f, (TengahMalam - s.MenitGim) * DetikNyataPerJamGim / 60f);

        public static string Format(float menit)
        {
            int m = (int)menit % (24 * 60);
            return $"{m / 60:00}.{m % 60:00}";
        }
    }
}
```

- [ ] **Step 4: Tambahkan `BeriObat` ke `KondisiIbu.cs`**

Tambahkan metode ini di dalam kelas `KondisiIbu`, setelah `SetelahTertangkap`:
```csharp
        // Desain 4.4. Di Malam 7 jalur utama, Runtime memanggil ini saat Laras memberi obat.
        public static bool BeriObat(GameState s)
        {
            if (s.ObatSudahDiberikan || !JamGim.DalamJendelaObat(s)) return false;
            s.ObatSudahDiberikan = true;
            Ubah(s, Obat);
            return true;
        }
```

- [ ] **Step 5: Buat `TujuanMalam.cs`**

```csharp
using System.Collections.Generic;

namespace TheSending.Core
{
    // Tujuan wajib per malam (Game Story bab 7). Kejadian berwaktu termasuk tujuan wajib.
    // Malam 7 berakhir lewat PenentuEnding, bukan lewat tujuan.
    public static class TujuanMalam
    {
        public const string TibaDiRumah = "TibaDiRumah", BicaraBidan = "BicaraBidan", MasukKamarIbu = "MasukKamarIbu";
        public const string BukaLemari = "BukaLemari", Cutscene23 = "Cutscene23", PotretPaku = "PotretPaku";
        public const string BacaAksara = "BacaAksara", PilihanWiryo = "PilihanWiryo", SembunyiPertama = "SembunyiPertama";
        public const string CabutPaku = "CabutPaku", KunjungiSumi = "KunjungiSumi";
        public const string AmbilBungkusanSumur = "AmbilBungkusanSumur", LolosKejaranSumur = "LolosKejaranSumur";
        public const string GaliKendi = "GaliKendi", BandingkanTanah = "BandingkanTanah", BicaraLaras = "BicaraLaras";
        public const string SidangWarga = "SidangWarga";

        static readonly string[][] Wajib =
        {
            new[] { TibaDiRumah, BicaraBidan, MasukKamarIbu },        // Prolog
            new[] { BukaLemari, Cutscene23, PotretPaku },             // Malam 1
            new[] { BacaAksara, PilihanWiryo, SembunyiPertama },      // Malam 2
            new[] { CabutPaku, KunjungiSumi },                        // Malam 3
            new[] { AmbilBungkusanSumur, LolosKejaranSumur },         // Malam 4
            new[] { GaliKendi, BandingkanTanah, BicaraLaras },        // Malam 5
            new[] { SidangWarga },                                    // Malam 6
            new string[0],                                            // Malam 7
        };

        public static IReadOnlyList<string> DaftarWajib(int malam) => Wajib[malam];

        public static bool Sudah(GameState s, string id) => s.TujuanSelesai.Contains(id);

        public static void Selesaikan(GameState s, string id)
        {
            if (s.TujuanSelesai.Contains(id)) return;
            s.TujuanSelesai.Add(id);
            s.Umumkan(Perubahan.Tujuan);
        }

        public static bool MalamSelesai(GameState s)
        {
            if (s.MalamKe == 7) return false;
            foreach (var id in Wajib[s.MalamKe])
                if (!s.TujuanSelesai.Contains(id)) return false;
            return true;
        }
    }
}
```

- [ ] **Step 6: Buat `PergantianMalam.cs`**

```csharp
using System;

namespace TheSending.Core
{
    public static class PergantianMalam
    {
        public static bool BisaLanjut(GameState s) => s.Ending == Ending.Belum && TujuanMalam.MalamSelesai(s);

        public static void Lanjut(GameState s)
        {
            if (!BisaLanjut(s)) throw new InvalidOperationException($"Malam {s.MalamKe} belum bisa ditutup.");

            bool dariProlog = s.MalamKe == 0;
            s.MalamKe++;
            s.ObatSudahDiberikan = false;
            s.TujuanSelesai.Clear();
            // Prolog bersambung ke Malam 1 pada sore yang sama, jadi jam dan kondisi tidak berubah.
            if (!dariProlog) s.MenitGim = JamGim.MulaiMalam;
            s.Umumkan(Perubahan.Malam);
            if (!dariProlog) KondisiIbu.Ubah(s, KondisiIbu.PergantianMalam);
        }
    }
}
```

- [ ] **Step 7: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh "TheSending.Tests.JamGimTests;TheSending.Tests.MalamTests"`
Expected: `result="Passed" total="29" passed="29" failed="0"`

- [ ] **Step 8: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add game clock, medicine window, night objectives and night transition"
```

---

### Task 7: MediaSantet dan RitualPenangkal

**Files:**
- Create: `Assets/_Project/Scripts/Core/MediaSantet.cs`, `Assets/_Project/Scripts/Core/RitualPenangkal.cs`
- Test: `Assets/_Project/Tests/EditMode/MediaDanRitualTests.cs`

**Interfaces:**
- Consumes: `KondisiIbu.Ubah` dan konstantanya, `PuzzleUrutan<T>`, `Kode.M*`.
- Produces:
  - `MediaSantet.Temukan(GameState, int)`, `MediaSantet.BisaDiserahkan(GameState, int) : bool`, `MediaSantet.Serahkan(GameState, int)`, `MediaSantet.BisaDinetralkan(GameState, int) : bool`, `MediaSantet.Netralkan(GameState, int)`. Indeks di luar rentang melempar `ArgumentOutOfRangeException`. Aksi yang tidak diizinkan melempar `InvalidOperationException`.
  - `RitualPenangkal.Pelajari(GameState, bool dariBuSumi)`, `RitualPenangkal.AmbilBahan(GameState, BahanRitual)`, `RitualPenangkal.BahanLengkap(GameState) : bool`, `RitualPenangkal.BisaMulai(GameState, int media) : bool`, konstruktor `new RitualPenangkal(GameState, int media)`, `bool Lakukan(LangkahRitual)`, `int Kemajuan`, `bool Selesai`.

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/MediaDanRitualTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class MediaDanRitualTests
    {
        static readonly LangkahRitual[] UrutanBenar =
        {
            LangkahRitual.LetakkanMedia, LangkahRitual.TaburGaram, LangkahRitual.TutupKelor,
            LangkahRitual.NyalakanApi, LangkahRitual.AdukBambu, LangkahRitual.KuburAbu,
        };

        static GameState SiapRitual(int media)
        {
            var s = new GameState { MalamKe = 3, KondisiIbu = 40 };
            MediaSantet.Temukan(s, media);
            RitualPenangkal.Pelajari(s, dariBuSumi: true);
            return s;
        }

        [Test]
        public void TemukanMencatatMalamPenemuan()
        {
            var s = new GameState { MalamKe = 2 };
            MediaSantet.Temukan(s, Kode.M1);
            Assert.IsTrue(s.MediaDitemukan[Kode.M1]);
            Assert.AreEqual(2, s.MalamMediaDitemukan[Kode.M1]);
        }

        [Test]
        public void MenyerahkanMediaMengurangi8DanTidakBisaDinetralkan()
        {
            var s = new GameState { MalamKe = 2, KondisiIbu = 48 };
            MediaSantet.Temukan(s, Kode.M1);
            MediaSantet.Serahkan(s, Kode.M1);
            Assert.AreEqual(40, s.KondisiIbu);
            Assert.IsFalse(MediaSantet.BisaDinetralkan(s, Kode.M1));
            Assert.Throws<InvalidOperationException>(() => MediaSantet.Netralkan(s, Kode.M1));
        }

        [Test]
        public void MediaYangBelumDitemukanTidakBisaDiserahkan()
        {
            Assert.Throws<InvalidOperationException>(() => MediaSantet.Serahkan(new GameState(), Kode.M1));
        }

        [Test]
        public void IndeksMediaDiLuarRentangDitolak()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => MediaSantet.Temukan(new GameState(), 7));
        }

        [Test]
        public void RitualDariHalaman4TanpaBahanBelumBisaDimulai()
        {
            var s = new GameState { MalamKe = 3 };
            MediaSantet.Temukan(s, Kode.M2);
            RitualPenangkal.Pelajari(s, dariBuSumi: false);
            Assert.IsFalse(RitualPenangkal.BisaMulai(s, Kode.M2));

            RitualPenangkal.AmbilBahan(s, BahanRitual.Garam);
            RitualPenangkal.AmbilBahan(s, BahanRitual.Kelor);
            RitualPenangkal.AmbilBahan(s, BahanRitual.BambuKuning);
            Assert.IsTrue(RitualPenangkal.BisaMulai(s, Kode.M2));
        }

        [Test]
        public void RitualTanpaPengetahuanDitolak()
        {
            var s = new GameState();
            MediaSantet.Temukan(s, Kode.M1);
            Assert.Throws<InvalidOperationException>(() => new RitualPenangkal(s, Kode.M1));
        }

        [Test]
        public void RitualUrutanBenarMenetralkanMediaDanMenambah8()
        {
            var s = SiapRitual(Kode.M1);
            var r = new RitualPenangkal(s, Kode.M1);
            foreach (var langkah in UrutanBenar) Assert.IsTrue(r.Lakukan(langkah));
            Assert.IsTrue(r.Selesai);
            Assert.IsTrue(s.MediaDinetralkan[Kode.M1]);
            Assert.AreEqual(48, s.KondisiIbu);
        }

        [Test]
        public void RitualSalahUrutanGagalDanHarusDiulang()
        {
            var s = SiapRitual(Kode.M1);
            var r = new RitualPenangkal(s, Kode.M1);
            r.Lakukan(LangkahRitual.LetakkanMedia);
            Assert.IsFalse(r.Lakukan(LangkahRitual.NyalakanApi));
            Assert.AreEqual(0, r.Kemajuan);
            Assert.IsFalse(s.MediaDinetralkan[Kode.M1]);
        }

        [Test]
        public void MediaYangSudahDinetralkanTidakMenambahLagi()
        {
            var s = SiapRitual(Kode.M1);
            MediaSantet.Netralkan(s, Kode.M1);
            Assert.IsFalse(RitualPenangkal.BisaMulai(s, Kode.M1));
            Assert.Throws<InvalidOperationException>(() => MediaSantet.Netralkan(s, Kode.M1));
        }

        [Test]
        public void MenetralkanKendiIndukMenandaiRitualTerakhirSelesai()
        {
            var s = SiapRitual(Kode.M7);
            MediaSantet.Netralkan(s, Kode.M7);
            Assert.IsTrue(s.RitualTerakhirSelesai);
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.MediaDanRitualTests`
Expected: compile error, `MediaSantet`/`RitualPenangkal` tidak ditemukan.

- [ ] **Step 3: Buat `MediaSantet.cs`**

```csharp
using System;

namespace TheSending.Core
{
    // Game Story 6.2. Netralkan dipanggil oleh RitualPenangkal setelah enam langkah benar.
    public static class MediaSantet
    {
        public static void Temukan(GameState s, int m)
        {
            CekIndeks(m);
            if (s.MediaDitemukan[m]) return;
            s.MediaDitemukan[m] = true;
            s.MalamMediaDitemukan[m] = s.MalamKe;
            s.Umumkan(Perubahan.Media);
        }

        public static bool BisaDiserahkan(GameState s, int m) => BisaDinetralkan(s, m);

        public static void Serahkan(GameState s, int m)
        {
            if (!BisaDiserahkan(s, m)) throw new InvalidOperationException($"Media {m + 1} tidak bisa diserahkan.");
            s.MediaDiserahkan[m] = true;
            s.Umumkan(Perubahan.Media);
            KondisiIbu.Ubah(s, KondisiIbu.MediaDiserahkan);
        }

        public static bool BisaDinetralkan(GameState s, int m)
        {
            CekIndeks(m);
            return s.MediaDitemukan[m] && !s.MediaDinetralkan[m] && !s.MediaDiserahkan[m];
        }

        public static void Netralkan(GameState s, int m)
        {
            if (!BisaDinetralkan(s, m)) throw new InvalidOperationException($"Media {m + 1} tidak bisa dinetralkan.");
            s.MediaDinetralkan[m] = true;
            if (m == Kode.M7) s.RitualTerakhirSelesai = true;
            s.Umumkan(Perubahan.Media);
            KondisiIbu.Ubah(s, KondisiIbu.MediaDinetralkan);
        }

        static void CekIndeks(int m)
        {
            if (m < 0 || m >= Kode.JumlahMedia) throw new ArgumentOutOfRangeException(nameof(m), m, "Media harus 0..6.");
        }
    }
}
```

- [ ] **Step 4: Buat `RitualPenangkal.cs`**

```csharp
using System;

namespace TheSending.Core
{
    // Game Story 6.5 dan desain 4.5. Salah langkah = ritual gagal, ulang dari langkah pertama.
    public class RitualPenangkal
    {
        static readonly LangkahRitual[] Urutan =
        {
            LangkahRitual.LetakkanMedia, LangkahRitual.TaburGaram, LangkahRitual.TutupKelor,
            LangkahRitual.NyalakanApi, LangkahRitual.AdukBambu, LangkahRitual.KuburAbu,
        };

        readonly GameState s;
        readonly int media;
        readonly PuzzleUrutan<LangkahRitual> langkah = new PuzzleUrutan<LangkahRitual>(Urutan, true);

        public int Kemajuan => langkah.Kemajuan;
        public bool Selesai => langkah.Selesai;

        public RitualPenangkal(GameState s, int media)
        {
            if (!BisaMulai(s, media)) throw new InvalidOperationException($"Ritual belum bisa dimulai untuk media {media + 1}.");
            this.s = s;
            this.media = media;
        }

        public bool Lakukan(LangkahRitual l)
        {
            if (Selesai) return false;
            bool benar = langkah.Pilih(l);
            if (Selesai) MediaSantet.Netralkan(s, media);
            return benar;
        }

        // Dari Bu Sumi: pengetahuan + satu ikat bahan. Dari Halaman 4: pengetahuan saja.
        public static void Pelajari(GameState s, bool dariBuSumi)
        {
            s.RitualDiketahui = true;
            if (dariBuSumi)
                for (int i = 0; i < s.BahanRitual.Length; i++) s.BahanRitual[i] = true;
            s.Umumkan(Perubahan.Ritual);
        }

        public static void AmbilBahan(GameState s, BahanRitual bahan)
        {
            s.BahanRitual[(int)bahan] = true;
            s.Umumkan(Perubahan.Ritual);
        }

        public static bool BahanLengkap(GameState s) => Array.TrueForAll(s.BahanRitual, x => x);

        public static bool BisaMulai(GameState s, int media) =>
            s.RitualDiketahui && BahanLengkap(s) && MediaSantet.BisaDinetralkan(s, media);
    }
}
```

- [ ] **Step 5: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.MediaDanRitualTests`
Expected: `result="Passed" total="10" passed="10" failed="0"`

- [ ] **Step 6: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add media santet handling and Ritual Penangkal"
```

---

### Task 8: PapanBukti dan BukuHarian

**Files:**
- Create: `Assets/_Project/Scripts/Core/PapanBukti.cs`, `Assets/_Project/Scripts/Core/BukuHarian.cs`
- Test: `Assets/_Project/Tests/EditMode/PapanBuktiTests.cs`

**Interfaces:**
- Consumes: `Kode.B*`, `Kode.K*`, `Kode.H*`, `RitualPenangkal.Pelajari` dan `RitualPenangkal.BahanLengkap` (Task 7).
- Produces: `PapanBukti.TambahBukti(GameState, int)`, `PapanBukti.TambahKatanya(GameState, int)`, `BukuHarian.Temukan(GameState, int)`.

Aturan (desain 4.7): K1 Terbantahkan oleh B2 atau B3, K2 oleh B7, K3 oleh B4 atau B5, K4 menjadi Bukti setelah B6. B1 masuk saat Halaman 1 **dan** 3 sudah ditemukan. Halaman 7 = B7. Halaman 4 mengajarkan ritual tanpa bahan.

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/PapanBuktiTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PapanBuktiTests
    {
        [Test]
        public void KatanyaBaruBerlabelKatanya()
        {
            var s = new GameState();
            PapanBukti.TambahKatanya(s, Kode.K1);
            Assert.IsTrue(s.KatanyaDiketahui[Kode.K1]);
            Assert.AreEqual(StatusInfo.Katanya, s.StatusKatanya[Kode.K1]);
        }

        [TestCase(Kode.K1, Kode.B2)]
        [TestCase(Kode.K1, Kode.B3)]
        [TestCase(Kode.K2, Kode.B7)]
        [TestCase(Kode.K3, Kode.B4)]
        [TestCase(Kode.K3, Kode.B5)]
        public void BuktiPembantahMengubahKatanyaMenjadiTerbantahkan(int k, int b)
        {
            var s = new GameState();
            PapanBukti.TambahKatanya(s, k);
            PapanBukti.TambahBukti(s, b);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[k]);
        }

        [Test]
        public void KatanyaYangDidapatSetelahBuktiLangsungTerbantahkan()
        {
            var s = new GameState();
            PapanBukti.TambahBukti(s, Kode.B4);
            PapanBukti.TambahKatanya(s, Kode.K3);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K3]);
        }

        [Test]
        public void K4MenjadiBuktiSetelahB6()
        {
            var s = new GameState();
            PapanBukti.TambahKatanya(s, Kode.K4);
            PapanBukti.TambahBukti(s, Kode.B6);
            Assert.AreEqual(StatusInfo.Bukti, s.StatusKatanya[Kode.K4]);
        }

        [Test]
        public void BuktiYangTidakTerkaitTidakMengubahKatanya()
        {
            var s = new GameState();
            PapanBukti.TambahKatanya(s, Kode.K2);
            PapanBukti.TambahBukti(s, Kode.B3);
            Assert.AreEqual(StatusInfo.Katanya, s.StatusKatanya[Kode.K2]);
        }

        [Test]
        public void B1ButuhHalaman1DanHalaman3()
        {
            var s = new GameState();
            BukuHarian.Temukan(s, Kode.H1);
            Assert.IsFalse(s.BuktiKunci[Kode.B1]);
            BukuHarian.Temukan(s, Kode.H3);
            Assert.IsTrue(s.BuktiKunci[Kode.B1]);
        }

        [Test]
        public void Halaman7AdalahB7()
        {
            var s = new GameState();
            BukuHarian.Temukan(s, Kode.H7);
            Assert.IsTrue(s.BuktiKunci[Kode.B7]);
        }

        [Test]
        public void Halaman4MengajarkanRitualTanpaBahan()
        {
            var s = new GameState();
            BukuHarian.Temukan(s, Kode.H4);
            Assert.IsTrue(s.RitualDiketahui);
            Assert.IsFalse(RitualPenangkal.BahanLengkap(s));
        }

        [Test]
        public void BuktiBaruDiumumkan()
        {
            var s = new GameState();
            bool diumumkan = false;
            s.Berubah += p => diumumkan |= p == Perubahan.Bukti;
            PapanBukti.TambahBukti(s, Kode.B3);
            Assert.IsTrue(diumumkan);
        }

        [Test]
        public void IndeksDiLuarRentangDitolak()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => PapanBukti.TambahBukti(new GameState(), 8));
            Assert.Throws<ArgumentOutOfRangeException>(() => PapanBukti.TambahKatanya(new GameState(), 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => BukuHarian.Temukan(new GameState(), 8));
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PapanBuktiTests`
Expected: compile error, `PapanBukti`/`BukuHarian` tidak ditemukan.

- [ ] **Step 3: Buat `PapanBukti.cs`**

```csharp
using System;

namespace TheSending.Core
{
    // Game Story 6.3 dan desain 4.7.
    public static class PapanBukti
    {
        public static void TambahBukti(GameState s, int b)
        {
            if (b < 0 || b >= Kode.JumlahBukti) throw new ArgumentOutOfRangeException(nameof(b), b, "Bukti harus 0..7.");
            if (s.BuktiKunci[b]) return;
            s.BuktiKunci[b] = true;
            s.Umumkan(Perubahan.Bukti);
            PerbaruiKatanya(s);
        }

        public static void TambahKatanya(GameState s, int k)
        {
            if (k < 0 || k >= Kode.JumlahKatanya) throw new ArgumentOutOfRangeException(nameof(k), k, "Katanya harus 0..3.");
            if (s.KatanyaDiketahui[k]) return;
            s.KatanyaDiketahui[k] = true;
            s.StatusKatanya[k] = StatusInfo.Katanya;
            s.Umumkan(Perubahan.Katanya);
            PerbaruiKatanya(s);
        }

        static void PerbaruiKatanya(GameState s)
        {
            bool[] b = s.BuktiKunci;
            bool berubah = false;
            berubah |= Tetapkan(s, Kode.K1, b[Kode.B2] || b[Kode.B3], StatusInfo.Terbantahkan);
            berubah |= Tetapkan(s, Kode.K2, b[Kode.B7], StatusInfo.Terbantahkan);
            berubah |= Tetapkan(s, Kode.K3, b[Kode.B4] || b[Kode.B5], StatusInfo.Terbantahkan);
            berubah |= Tetapkan(s, Kode.K4, b[Kode.B6], StatusInfo.Bukti);
            if (berubah) s.Umumkan(Perubahan.Katanya);
        }

        static bool Tetapkan(GameState s, int k, bool syarat, StatusInfo status)
        {
            if (!s.KatanyaDiketahui[k] || !syarat || s.StatusKatanya[k] == status) return false;
            s.StatusKatanya[k] = status;
            return true;
        }
    }
}
```

- [ ] **Step 4: Buat `BukuHarian.cs`**

```csharp
using System;

namespace TheSending.Core
{
    // Game Story 10.3. Halaman tertentu langsung menghasilkan bukti atau pengetahuan ritual.
    public static class BukuHarian
    {
        public static void Temukan(GameState s, int h)
        {
            if (h < 0 || h >= Kode.JumlahHalaman) throw new ArgumentOutOfRangeException(nameof(h), h, "Halaman harus 0..7.");
            if (s.HalamanDiary[h]) return;
            s.HalamanDiary[h] = true;
            s.Umumkan(Perubahan.Diary);

            if (s.HalamanDiary[Kode.H1] && s.HalamanDiary[Kode.H3]) PapanBukti.TambahBukti(s, Kode.B1);
            if (h == Kode.H4) RitualPenangkal.Pelajari(s, dariBuSumi: false);
            if (h == Kode.H7) PapanBukti.TambahBukti(s, Kode.B7);
        }
    }
}
```

- [ ] **Step 5: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PapanBuktiTests`
Expected: `result="Passed" total="14" passed="14" failed="0"`

- [ ] **Step 6: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add Papan Bukti statuses and diary page effects"
```

---

### Task 9: KepercayaanSumi

**Files:**
- Create: `Assets/_Project/Scripts/Core/KepercayaanSumi.cs`
- Test: `Assets/_Project/Tests/EditMode/KepercayaanSumiTests.cs`

**Interfaces:**
- Consumes: `GameState`, `AksiSumi`.
- Produces: `KepercayaanSumi.Maks = 3`, `AmbangRitual = 1`, `AmbangKesaksian = 2`, `AmbangMinyakSereh = 3`; `KepercayaanSumi.Tambah(GameState, AksiSumi) : bool`, `KepercayaanSumi.Tuduh(GameState)`, `KepercayaanSumi.PintuTerbuka(GameState) : bool`, `KepercayaanSumi.BisaAjarkanRitual(GameState) : bool`, `KepercayaanSumi.BisaBeriKesaksian(GameState) : bool`, `KepercayaanSumi.BisaBeriMinyakSereh(GameState) : bool`, `KepercayaanSumi.TerimaMinyakSereh(GameState)`.

Runtime (dialog Bu Sumi) yang memanggil `RitualPenangkal.Pelajari(s, true)`, `PapanBukti.TambahBukti(s, Kode.B5)`, dan `BukuHarian.Temukan(s, Kode.H7)` setelah gerbang di kelas ini terbuka.

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/KepercayaanSumiTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class KepercayaanSumiTests
    {
        [Test]
        public void SetiapAksiHanyaMenambahSekali()
        {
            var s = new GameState { MalamKe = 3 };
            Assert.IsTrue(KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf));
            Assert.IsFalse(KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf));
            Assert.AreEqual(1, s.KepercayaanSumi);
        }

        [Test]
        public void TigaAksiMencapaiMaksimal3()
        {
            var s = new GameState { MalamKe = 4 };
            KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf);
            KepercayaanSumi.Tambah(s, AksiSumi.BantuLampuDanAir);
            KepercayaanSumi.Tambah(s, AksiSumi.BelaDiPosRonda);
            Assert.AreEqual(3, s.KepercayaanSumi);
            Assert.IsTrue(KepercayaanSumi.BisaBeriMinyakSereh(s));
        }

        [Test]
        public void AmbangRitualDanKesaksian()
        {
            var s = new GameState { MalamKe = 3 };
            Assert.IsFalse(KepercayaanSumi.BisaAjarkanRitual(s));
            KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf);
            Assert.IsTrue(KepercayaanSumi.BisaAjarkanRitual(s));
            Assert.IsFalse(KepercayaanSumi.BisaBeriKesaksian(s));
            KepercayaanSumi.Tambah(s, AksiSumi.BantuLampuDanAir);
            Assert.IsTrue(KepercayaanSumi.BisaBeriKesaksian(s));
        }

        [Test]
        public void MenuduhMengurangi1DanMenutupPintuSampaiMalamBerikutnya()
        {
            var s = new GameState { MalamKe = 3 };
            KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf);
            KepercayaanSumi.Tuduh(s);
            Assert.AreEqual(0, s.KepercayaanSumi);
            Assert.IsFalse(KepercayaanSumi.PintuTerbuka(s));
            s.MalamKe = 4;
            Assert.IsTrue(KepercayaanSumi.PintuTerbuka(s));
        }

        [Test]
        public void MenuduhTidakMembuatNilaiNegatif()
        {
            var s = new GameState { MalamKe = 3 };
            KepercayaanSumi.Tuduh(s);
            Assert.AreEqual(0, s.KepercayaanSumi);
        }

        [Test]
        public void MinyakSerehHanyaBisaDiterimaDenganKepercayaan3()
        {
            var s = new GameState { MalamKe = 4, KepercayaanSumi = 2 };
            Assert.Throws<InvalidOperationException>(() => KepercayaanSumi.TerimaMinyakSereh(s));
            s.KepercayaanSumi = 3;
            KepercayaanSumi.TerimaMinyakSereh(s);
            Assert.IsTrue(s.PunyaMinyakSereh);
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.KepercayaanSumiTests`
Expected: compile error, kelas `KepercayaanSumi` tidak ditemukan.

- [ ] **Step 3: Buat `KepercayaanSumi.cs`**

```csharp
using System;

namespace TheSending.Core
{
    // Game Story 6.4. Kelas ini hanya menyimpan nilai dan gerbang; hadiahnya diberikan lewat dialog di Runtime.
    public static class KepercayaanSumi
    {
        public const int Maks = 3, AmbangRitual = 1, AmbangKesaksian = 2, AmbangMinyakSereh = 3;

        public static bool Tambah(GameState s, AksiSumi aksi)
        {
            int i = (int)aksi;
            if (s.AksiSumiDipakai[i]) return false;
            s.AksiSumiDipakai[i] = true;
            s.KepercayaanSumi = Math.Min(Maks, s.KepercayaanSumi + 1);
            s.Umumkan(Perubahan.Kepercayaan);
            return true;
        }

        public static void Tuduh(GameState s)
        {
            s.KepercayaanSumi = Math.Max(0, s.KepercayaanSumi - 1);
            s.PintuSumiTerbukaMulaiMalam = s.MalamKe + 1;
            s.Umumkan(Perubahan.Kepercayaan);
        }

        public static bool PintuTerbuka(GameState s) => s.MalamKe >= s.PintuSumiTerbukaMulaiMalam;
        public static bool BisaAjarkanRitual(GameState s) => s.KepercayaanSumi >= AmbangRitual;
        public static bool BisaBeriKesaksian(GameState s) => s.KepercayaanSumi >= AmbangKesaksian;
        public static bool BisaBeriMinyakSereh(GameState s) => s.KepercayaanSumi >= AmbangMinyakSereh;

        public static void TerimaMinyakSereh(GameState s)
        {
            if (!BisaBeriMinyakSereh(s)) throw new InvalidOperationException("Kepercayaan Bu Sumi belum 3.");
            s.PunyaMinyakSereh = true;
            s.Umumkan(Perubahan.Kepercayaan);
        }
    }
}
```

- [ ] **Step 4: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.KepercayaanSumiTests`
Expected: `result="Passed" total="6" passed="6" failed="0"`

- [ ] **Step 5: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add Kepercayaan Bu Sumi rules"
```

---

### Task 10: SidangWarga

**Files:**
- Create: `Assets/_Project/Scripts/Core/SidangWarga.cs`
- Test: `Assets/_Project/Tests/EditMode/SidangWargaTests.cs`

**Interfaces:**
- Consumes: `GameState`, `Kode.B*`, `Kode.K*`.
- Produces: `readonly struct ItemPapan { bool IsKatanya; int Indeks; static ItemPapan Bukti(int); static ItemPapan Katanya(int); }`; `SidangWarga.JumlahTuduhan = 3`, `MinimalBenar = 2`; `SidangWarga.JawabanBenar(GameState, int tuduhan, ItemPapan) : bool`; `SidangWarga.Selesaikan(GameState, int jumlahBenar) : HasilSidang`; `SidangWarga.Diam(GameState) : HasilSidang`; `SidangWarga.IkutMenuduh(GameState) : HasilSidang`. Menetapkan hasil dua kali melempar `InvalidOperationException`.

Tuduhan (Game Story Malam 6): indeks 0 = rambut di sumur (B4, B5), 1 = dendam (B1, B6, B7), 2 = siapa lagi yang mengerti ilmu (B2, B3).

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/SidangWargaTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class SidangWargaTests
    {
        static GameState DenganBukti(params int[] bukti)
        {
            var s = new GameState { MalamKe = 6 };
            foreach (var b in bukti) s.BuktiKunci[b] = true;
            return s;
        }

        [TestCase(0, Kode.B4)]
        [TestCase(0, Kode.B5)]
        [TestCase(1, Kode.B1)]
        [TestCase(1, Kode.B6)]
        [TestCase(1, Kode.B7)]
        [TestCase(2, Kode.B2)]
        [TestCase(2, Kode.B3)]
        public void BuktiValidYangDimilikiDianggapBenar(int tuduhan, int bukti)
        {
            Assert.IsTrue(SidangWarga.JawabanBenar(DenganBukti(bukti), tuduhan, ItemPapan.Bukti(bukti)));
        }

        [Test]
        public void BuktiYangBelumDimilikiDianggapSalah()
        {
            Assert.IsFalse(SidangWarga.JawabanBenar(DenganBukti(), 0, ItemPapan.Bukti(Kode.B4)));
        }

        [Test]
        public void BuktiUntukTuduhanLainDianggapSalah()
        {
            Assert.IsFalse(SidangWarga.JawabanBenar(DenganBukti(Kode.B3), 0, ItemPapan.Bukti(Kode.B3)));
        }

        [Test]
        public void MenunjukkanKatanyaSelaluSalah()
        {
            var s = DenganBukti(Kode.B6);
            s.KatanyaDiketahui[Kode.K4] = true;
            s.StatusKatanya[Kode.K4] = StatusInfo.Bukti;
            Assert.IsFalse(SidangWarga.JawabanBenar(s, 1, ItemPapan.Katanya(Kode.K4)));
        }

        [TestCase(3, HasilSidang.Berhasil)]
        [TestCase(2, HasilSidang.Berhasil)]
        [TestCase(1, HasilSidang.Gagal)]
        [TestCase(0, HasilSidang.Gagal)]
        public void MinimalDuaBenarUntukBerhasil(int benar, HasilSidang hasil)
        {
            var s = DenganBukti();
            Assert.AreEqual(hasil, SidangWarga.Selesaikan(s, benar));
            Assert.AreEqual(hasil, s.HasilSidang);
        }

        [Test]
        public void DiamDanIkutMenuduhLangsungGagal()
        {
            var a = DenganBukti();
            var b = DenganBukti();
            Assert.AreEqual(HasilSidang.Diam, SidangWarga.Diam(a));
            Assert.AreEqual(HasilSidang.Menuduh, SidangWarga.IkutMenuduh(b));
            Assert.IsTrue(a.JalurFitnah);
            Assert.IsTrue(b.JalurFitnah);
        }

        [Test]
        public void HasilTidakBisaDitetapkanDuaKali()
        {
            var s = DenganBukti();
            SidangWarga.Selesaikan(s, 2);
            Assert.Throws<InvalidOperationException>(() => SidangWarga.Diam(s));
        }

        [Test]
        public void TuduhanDiLuarRentangDitolak()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SidangWarga.JawabanBenar(DenganBukti(), 3, ItemPapan.Bukti(Kode.B1)));
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.SidangWargaTests`
Expected: compile error, `SidangWarga`/`ItemPapan` tidak ditemukan.

- [ ] **Step 3: Buat `SidangWarga.cs`**

```csharp
using System;

namespace TheSending.Core
{
    public readonly struct ItemPapan
    {
        public readonly bool IsKatanya;
        public readonly int Indeks;

        ItemPapan(bool isKatanya, int indeks)
        {
            IsKatanya = isKatanya;
            Indeks = indeks;
        }

        public static ItemPapan Bukti(int b) => new ItemPapan(false, b);
        public static ItemPapan Katanya(int k) => new ItemPapan(true, k);
    }

    // Game Story Malam 6 dan desain 4.8.
    public static class SidangWarga
    {
        public const int JumlahTuduhan = 3, MinimalBenar = 2;

        static readonly int[][] BuktiValid =
        {
            new[] { Kode.B4, Kode.B5 },             // "Rambut di sumur itu rambut Mbok Sumi!"
            new[] { Kode.B1, Kode.B6, Kode.B7 },    // "Dia dendam sama keluargamu..."
            new[] { Kode.B2, Kode.B3 },             // "Siapa lagi yang ngerti ilmu begituan...?"
        };

        public static bool JawabanBenar(GameState s, int tuduhan, ItemPapan item)
        {
            if (tuduhan < 0 || tuduhan >= JumlahTuduhan) throw new ArgumentOutOfRangeException(nameof(tuduhan));
            if (item.IsKatanya) return false;
            if (item.Indeks < 0 || item.Indeks >= Kode.JumlahBukti) throw new ArgumentOutOfRangeException(nameof(item));
            return s.BuktiKunci[item.Indeks] && Array.IndexOf(BuktiValid[tuduhan], item.Indeks) >= 0;
        }

        public static HasilSidang Selesaikan(GameState s, int jumlahBenar) =>
            Tetapkan(s, jumlahBenar >= MinimalBenar ? HasilSidang.Berhasil : HasilSidang.Gagal);

        public static HasilSidang Diam(GameState s) => Tetapkan(s, HasilSidang.Diam);
        public static HasilSidang IkutMenuduh(GameState s) => Tetapkan(s, HasilSidang.Menuduh);

        static HasilSidang Tetapkan(GameState s, HasilSidang hasil)
        {
            if (s.HasilSidang != HasilSidang.Belum) throw new InvalidOperationException("Sidang Warga sudah selesai.");
            s.HasilSidang = hasil;
            s.Umumkan(Perubahan.Sidang);
            return hasil;
        }
    }
}
```

- [ ] **Step 4: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.SidangWargaTests`
Expected: `result="Passed" total="17" passed="17" failed="0"`

- [ ] **Step 5: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add Sidang Warga scoring"
```

---

### Task 11: KewaspadaanSantet

**Files:**
- Create: `Assets/_Project/Scripts/Core/KewaspadaanSantet.cs`
- Test: `Assets/_Project/Tests/EditMode/KewaspadaanSantetTests.cs`

**Interfaces:**
- Consumes: `GameState` (field media), `Gerak`.
- Produces: konstanta `RadiusNapas = 1.5f`, `RadiusJalan = 4f`, `RadiusLari = 8f`, `FaktorMinyakSereh = 0.6f`, `JarakCahaya = 12f`, `KecepatanDasar = 3.5f`, `TambahKecepatanPerTingkat = 0.4f`, `JedaDasar = 180f`, `KurangJedaPerTingkat = 30f`, `JedaMinimum = 60f`; `RadiusSuara(Gerak, bool tahanNapas, bool minyakSereh) : float`; `Terdengar(float jarak, Gerak, bool tahanNapas, bool minyakSereh) : bool`; `CahayaTerlihat(float jarak, bool senterMenyala, bool senterMengarahKeSantet) : bool`; `TingkatKekuatan(GameState) : int`; `KecepatanKejar(int tingkat) : float`; `JedaKemunculanDetik(int tingkat) : float`.

Semua angka adalah nilai awal untuk playtest (desain 4.6).

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/KewaspadaanSantetTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class KewaspadaanSantetTests
    {
        [Test]
        public void DiamSambilMenahanNapasTidakBersuara()
        {
            Assert.AreEqual(0f, KewaspadaanSantet.RadiusSuara(Gerak.Diam, tahanNapas: true, minyakSereh: false));
            Assert.IsFalse(KewaspadaanSantet.Terdengar(0.5f, Gerak.Diam, true, false));
        }

        [Test]
        public void LariTerdengarLebihJauhDariJalan()
        {
            Assert.IsTrue(KewaspadaanSantet.Terdengar(7f, Gerak.Lari, false, false));
            Assert.IsFalse(KewaspadaanSantet.Terdengar(7f, Gerak.Jalan, false, false));
        }

        [Test]
        public void MinyakSerehMemperkecilRadiusLangkah()
        {
            Assert.AreEqual(4.8f, KewaspadaanSantet.RadiusSuara(Gerak.Lari, false, minyakSereh: true), 0.001f);
            Assert.IsFalse(KewaspadaanSantet.Terdengar(6f, Gerak.Lari, false, true));
        }

        [Test]
        public void CahayaHanyaTerlihatJikaSenterMenyalaDanMengarah()
        {
            Assert.IsTrue(KewaspadaanSantet.CahayaTerlihat(10f, true, true));
            Assert.IsFalse(KewaspadaanSantet.CahayaTerlihat(10f, false, true));
            Assert.IsFalse(KewaspadaanSantet.CahayaTerlihat(10f, true, false));
            Assert.IsFalse(KewaspadaanSantet.CahayaTerlihat(13f, true, true));
        }

        [Test]
        public void TingkatKekuatanMenghitungMediaDiserahkanDanMediaLamaYangBelumDinetralkan()
        {
            var s = new GameState { MalamKe = 4 };
            s.MediaDitemukan[Kode.M1] = true; s.MalamMediaDitemukan[Kode.M1] = 2; s.MediaDiserahkan[Kode.M1] = true;
            s.MediaDitemukan[Kode.M2] = true; s.MalamMediaDitemukan[Kode.M2] = 3;                                     // lama, belum dinetralkan
            s.MediaDitemukan[Kode.M3] = true; s.MalamMediaDitemukan[Kode.M3] = 4;                                     // malam ini, belum dihitung
            s.MediaDitemukan[Kode.M4] = true; s.MalamMediaDitemukan[Kode.M4] = 3; s.MediaDinetralkan[Kode.M4] = true; // sudah dinetralkan
            Assert.AreEqual(2, KewaspadaanSantet.TingkatKekuatan(s));
        }

        [Test]
        public void TingkatLebihTinggiLebihCepatDanLebihSering()
        {
            Assert.Greater(KewaspadaanSantet.KecepatanKejar(2), KewaspadaanSantet.KecepatanKejar(0));
            Assert.Less(KewaspadaanSantet.JedaKemunculanDetik(2), KewaspadaanSantet.JedaKemunculanDetik(0));
            Assert.AreEqual(KewaspadaanSantet.JedaMinimum, KewaspadaanSantet.JedaKemunculanDetik(10));
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.KewaspadaanSantetTests`
Expected: compile error, `KewaspadaanSantet` tidak ditemukan.

- [ ] **Step 3: Buat `KewaspadaanSantet.cs`**

```csharp
using System;

namespace TheSending.Core
{
    // Sosok Santet mengandalkan suara langkah dan cahaya senter (Game Story 4.2) dan menguat sesuai desain 4.6.
    // Semua angka adalah nilai awal; disetel saat playtest.
    public static class KewaspadaanSantet
    {
        public const float RadiusNapas = 1.5f, RadiusJalan = 4f, RadiusLari = 8f, FaktorMinyakSereh = 0.6f;
        public const float JarakCahaya = 12f;
        public const float KecepatanDasar = 3.5f, TambahKecepatanPerTingkat = 0.4f;
        public const float JedaDasar = 180f, KurangJedaPerTingkat = 30f, JedaMinimum = 60f;

        public static float RadiusSuara(Gerak gerak, bool tahanNapas, bool minyakSereh)
        {
            if (gerak == Gerak.Diam) return tahanNapas ? 0f : RadiusNapas;
            float r = gerak == Gerak.Lari ? RadiusLari : RadiusJalan;
            return minyakSereh ? r * FaktorMinyakSereh : r;
        }

        public static bool Terdengar(float jarak, Gerak gerak, bool tahanNapas, bool minyakSereh) =>
            jarak <= RadiusSuara(gerak, tahanNapas, minyakSereh);

        public static bool CahayaTerlihat(float jarak, bool senterMenyala, bool senterMengarahKeSantet) =>
            senterMenyala && senterMengarahKeSantet && jarak <= JarakCahaya;

        // Media diserahkan + media dari malam sebelumnya yang belum dinetralkan.
        public static int TingkatKekuatan(GameState s)
        {
            int tingkat = 0;
            for (int m = 0; m < Kode.JumlahMedia; m++)
            {
                if (s.MediaDiserahkan[m]) tingkat++;
                else if (s.MediaDitemukan[m] && !s.MediaDinetralkan[m] && s.MalamMediaDitemukan[m] < s.MalamKe) tingkat++;
            }
            return tingkat;
        }

        public static float KecepatanKejar(int tingkat) => KecepatanDasar + TambahKecepatanPerTingkat * tingkat;

        public static float JedaKemunculanDetik(int tingkat) =>
            Math.Max(JedaMinimum, JedaDasar - KurangJedaPerTingkat * tingkat);
    }
}
```

- [ ] **Step 4: Jalankan test, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.KewaspadaanSantetTests`
Expected: `result="Passed" total="6" passed="6" failed="0"`

- [ ] **Step 5: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add Sosok Santet awareness and strength rules"
```

---

### Task 12: Test integrasi alur ending, catat progres, push

**Files:**
- Test: `Assets/_Project/Tests/EditMode/AlurCeritaTests.cs`
- Modify: `CLAUDE.md` (status Fase 0, log progres, cara singkat menjalankan test)

**Interfaces:**
- Consumes: semua kelas Core dari Task 1–11.
- Produces: tidak ada API baru. Membuktikan aturan bekerja bersama dari Prolog sampai ending.

- [ ] **Step 1: Tulis test integrasi**

`Assets/_Project/Tests/EditMode/AlurCeritaTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class AlurCeritaTests
    {
        static void TutupMalam(GameState s)
        {
            foreach (var id in TujuanMalam.DaftarWajib(s.MalamKe)) TujuanMalam.Selesaikan(s, id);
            PergantianMalam.Lanjut(s);
        }

        static void BeriObatTepatWaktu(GameState s)
        {
            s.MenitGim = s.MalamKe == 6 ? JamGim.Jam20 : JamGim.Jam21;
            Assert.IsTrue(KondisiIbu.BeriObat(s), $"Obat Malam {s.MalamKe}");
        }

        static void Netralkan(GameState s, int media)
        {
            var r = new RitualPenangkal(s, media);
            foreach (LangkahRitual l in System.Enum.GetValues(typeof(LangkahRitual))) r.Lakukan(l);
            Assert.IsTrue(r.Selesai);
        }

        // Jalur wajib sampai sidang berhasil. Mengembalikan state di awal Malam 7.
        static GameState MainSampaiMalam7()
        {
            var s = new GameState();
            TutupMalam(s);                                          // Prolog -> Malam 1 (60)

            BeriObatTepatWaktu(s);                                  // 62
            BukuHarian.Temukan(s, Kode.H1);
            BukuHarian.Temukan(s, Kode.H2);
            TutupMalam(s);                                          // Malam 2 (50)

            BeriObatTepatWaktu(s);                                  // 52
            MediaSantet.Temukan(s, Kode.M1);
            TutupMalam(s);                                          // Malam 3 (40)

            BeriObatTepatWaktu(s);                                  // 42
            MediaSantet.Temukan(s, Kode.M2);
            BukuHarian.Temukan(s, Kode.H3);                         // B1
            BukuHarian.Temukan(s, Kode.H4);
            KepercayaanSumi.Tambah(s, AksiSumi.MintaMaaf);
            KepercayaanSumi.Tambah(s, AksiSumi.BantuLampuDanAir);
            Assert.IsTrue(KepercayaanSumi.BisaAjarkanRitual(s));
            RitualPenangkal.Pelajari(s, dariBuSumi: true);
            Netralkan(s, Kode.M1);                                  // 50
            Netralkan(s, Kode.M2);                                  // 58
            TutupMalam(s);                                          // Malam 4 (46)

            BeriObatTepatWaktu(s);                                  // 48
            PapanBukti.TambahKatanya(s, Kode.K1);
            PapanBukti.TambahKatanya(s, Kode.K2);
            MediaSantet.Temukan(s, Kode.M3);
            PapanBukti.TambahKatanya(s, Kode.K3);
            Netralkan(s, Kode.M3);                                  // 56
            TutupMalam(s);                                          // Malam 5 (44)

            BeriObatTepatWaktu(s);                                  // 46
            MediaSantet.Temukan(s, Kode.M5);
            PapanBukti.TambahBukti(s, Kode.B3);
            PapanBukti.TambahBukti(s, Kode.B4);
            Assert.IsTrue(KepercayaanSumi.BisaBeriKesaksian(s));
            PapanBukti.TambahBukti(s, Kode.B5);
            BukuHarian.Temukan(s, Kode.H7);                         // B7
            Netralkan(s, Kode.M5);                                  // 54
            TutupMalam(s);                                          // Malam 6 (42)

            BeriObatTepatWaktu(s);                                  // 44
            int benar = 0;
            if (SidangWarga.JawabanBenar(s, 0, ItemPapan.Bukti(Kode.B4))) benar++;
            if (SidangWarga.JawabanBenar(s, 1, ItemPapan.Bukti(Kode.B7))) benar++;
            if (SidangWarga.JawabanBenar(s, 2, ItemPapan.Bukti(Kode.B3))) benar++;
            Assert.AreEqual(HasilSidang.Berhasil, SidangWarga.Selesaikan(s, benar));
            TutupMalam(s);                                          // Malam 7 (32)
            return s;
        }

        [Test]
        public void JalurUtamaSampaiEndingSejati()
        {
            var s = MainSampaiMalam7();
            Assert.AreEqual(7, s.MalamKe);
            Assert.AreEqual(32, s.KondisiIbu);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K1]);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K2]);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K3]);

            BeriObatTepatWaktu(s);                                  // Laras memberi obat
            MediaSantet.Temukan(s, Kode.M7);
            PapanBukti.TambahBukti(s, Kode.B8);
            Assert.IsTrue(LilinSesaji.Buat(7).Pilih(Pasaran.Kliwon));
            s.PilihanAkhir = PilihanAkhir.Lapor;
            Netralkan(s, Kode.M7);

            Assert.AreEqual(Ending.Sejati, s.Ending);
        }

        [Test]
        public void JalurUtamaKehabisanWaktuMenjadiTerlambat()
        {
            var s = MainSampaiMalam7();
            JamGim.Maju(s, 100000f);
            Assert.AreEqual(Ending.Terlambat, s.Ending);
        }

        [Test]
        public void JalurBalasDendam()
        {
            var s = MainSampaiMalam7();
            MediaSantet.Temukan(s, Kode.M7);
            s.PilihanAkhir = PilihanAkhir.Balas;
            s.LolosKejaranWiryo = true;
            PenentuEnding.Cek(s);
            Assert.AreEqual(Ending.BalasDendam, s.Ending);
        }

        [Test]
        public void SidangDiamMenujuFitnahDanIbuBertahanSampaiTengahMalam()
        {
            var s = new GameState { MalamKe = 6, KondisiIbu = 6 };
            SidangWarga.Diam(s);
            TutupMalam(s);                                          // -12 ditahan menjadi 1
            Assert.AreEqual(7, s.MalamKe);
            Assert.AreEqual(1, s.KondisiIbu);
            Assert.AreEqual(Ending.Belum, s.Ending);

            KondisiIbu.SetelahTertangkap(s);
            Assert.AreEqual(1, s.KondisiIbu);

            JamGim.Maju(s, 100000f);
            Assert.AreEqual(Ending.Fitnah, s.Ending);
        }

        [Test]
        public void SaveDiTengahJalanBisaDilanjutkan()
        {
            var s = MainSampaiMalam7();
            var muat = SaveData.DariJson(SaveData.KeJson(s));
            Assert.AreEqual(HasilSidang.Berhasil, muat.HasilSidang);
            Assert.AreEqual(s.KondisiIbu, muat.KondisiIbu);
            Assert.IsTrue(muat.MediaDinetralkan[Kode.M5]);
        }
    }
}
```

- [ ] **Step 2: Jalankan test integrasi**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.AlurCeritaTests`
Expected: `result="Passed" total="5" passed="5" failed="0"`. Kode Core sudah lengkap, jadi test ini seharusnya langsung lulus. Jika gagal, itu bug integrasi: perbaiki kode Core-nya, jangan ubah angka di test kecuali hitungan di komentar memang salah terhadap Game Story 6.1.

- [ ] **Step 3: Jalankan seluruh test**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="150" passed="150" failed="0"` (KalenderJawa 13 + SaveData 10 + Puzzle 16 + PenentuEnding 18 + KondisiIbu 6 + JamGim/Malam 29 + MediaDanRitual 10 + PapanBukti 14 + KepercayaanSumi 6 + SidangWarga 17 + KewaspadaanSantet 6 + AlurCerita 5).

- [ ] **Step 4: Perbarui `CLAUDE.md`**

Ubah baris Fase 0 di tabel "Fase dan Progres" menjadi:
```markdown
| 0. Fondasi & Style Lock | H1–H3 | 16–18 Nov | [~] 0A selesai (setup + Core, 150 EditMode test lulus). 0B dan 0C belum |
```

Tambahkan di "Log Progres" (ganti `YYYY-MM-DD` dengan tanggal pengerjaan):
```markdown
- **YYYY-MM-DD:** Fase 0A selesai: Git LFS, paket (test-framework 1.8.0, ai.navigation 2.0.14, timeline 6.6.0, probuilder 6.1.2), assembly Core dan EditMode test, seluruh aturan Core. Berikutnya: rencana 0B (fondasi Runtime).
```

Tambahkan sebagai baris pertama di bagian "Menjalankan Test":
```markdown
- Cara singkat (editor tertutup): `bash tools/run-editmode-tests.sh` atau `bash tools/run-editmode-tests.sh TheSending.Tests.NamaKelasTests`.
```

- [ ] **Step 5: Commit dan push**

```bash
git add Assets/_Project CLAUDE.md
git commit -m "test: add end-to-end story flow tests and record Fase 0A progress"
git push -u origin main
```
Konfirmasi ke tim sebelum push pertama. Expected: push berhasil ke `origin/main`.
