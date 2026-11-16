# Fase 0B: Fondasi Runtime — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Membangun lapisan Runtime yang dipakai semua malam: GameManager, save ke file dan checkpoint, kontrol pemain (jalan, lari, lihat, tahan napas), interaksi, HP (senter, kompas, kamera), dialog, Jurnal dan Papan Bukti, kartu teks, serta scene uji `Scene_Sandbox0B` untuk dicek tim di Play Mode.

**Architecture:** Logika yang bisa diuji tetap masuk Core (C# murni, EditMode test): napas, baterai senter, kompas, efek dialog, alur dialog, berkas save, checkpoint. Runtime berisi MonoBehaviour tipis yang membaca input, memanggil Core, dan menggambar UI. UI dibangun lewat kode (uGUI, font bawaan) dan input didefinisikan lewat kode (Input System), sehingga tidak ada prefab atau aset input yang harus disambung manual. Scene uji dibuat oleh skrip editor yang dijalankan lewat batchmode.

**Tech Stack:** Unity 6000.6.4f1, URP 17.6, Input System 1.20, uGUI 2.6 (legacy `Text`), C# .NET Standard 2.1, Unity Test Framework 1.8.0.

Fase 0 terdiri dari 0A (selesai, Core), **0B (ini)**, dan 0C (gaya PSX, `docs/style-guide.md`, `Scene_MainMenu`, menu jeda).

## Global Constraints

- Unity **6000.6.4f1**, URP 17.6. Target build final **hanya Windows** (Mono). Pengembangan dan tes harian di Mac.
- Core: `Assets/_Project/Scripts/Core`, namespace `TheSending.Core`, assembly `TheSending.Core`. **Tanpa MonoBehaviour.**
- Runtime: `Assets/_Project/Scripts/Runtime`, namespace `TheSending.Runtime`, assembly `TheSending.Runtime` (mereferensikan Core, Unity.InputSystem).
- Editor: `Assets/_Project/Scripts/Editor`, namespace `TheSending.EditorTools`, assembly `TheSending.Editor` (Editor only).
- Test: `Assets/_Project/Tests/EditMode`, namespace `TheSending.Tests`, assembly `TheSending.Tests.EditMode`.
- Nama domain bahasa Indonesia sesuai GDD, istilah teknis bahasa Inggris.
- Hanya satu singleton: `GameManager`.
- Ubah `GameState` hanya lewat kelas aturan di Core.
- Kontrol sesuai GDD: W/A/S/D jalan, Shift lari, mouse lihat, E interaksi, F senter, Q kamera, C kompas, tahan Spasi menahan napas, Tab Jurnal, I inventaris, Esc jeda. (Inventaris dan menu jeda dibuat di 0C dan fase malam; aksinya sudah didefinisikan di sini.)
- Catatan untuk 0B di `CLAUDE.md` wajib dipatuhi: checkpoint disimpan ulang setelah penalti tertangkap; `GameManager` berlangganan ulang `Berubah` setiap kali `GameState` diganti; pilihan terakhir lewat `PenentuEnding.Pilih`.
- TDD untuk semua kode Core baru. Kode Runtime diverifikasi dengan kompilasi (menjalankan test suite mengompilasi semua assembly) dan daftar cek Play Mode oleh tim di Task 10.
- Commit memakai format `<type>: <deskripsi>` dan diakhiri baris `Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ`. **Jangan pernah menambahkan baris `Co-Authored-By` Claude.**

## Cara Menjalankan Test

- Editor Unity harus tertutup. `bash tools/run-editmode-tests.sh` (semua) atau `bash tools/run-editmode-tests.sh TheSending.Tests.NamaKelasTests`. Timeout Bash 600000 ms.
- RED untuk kelas baru biasanya berupa compile error. Itu gagal yang diharapkan.
- Setiap task ditutup dengan seluruh suite lulus (jumlah = sebelumnya + test baru).

## Struktur File

```
Assets/_Project/Scripts/Core/
  Napas.cs              batas tahan napas
  BateraiSenter.cs      baterai senter HP (disimpan di GameState)
  Kompas.cs             kecepatan putar jarum kompas
  RentangMalam.cs       aturan "aktif di malam X..Y"
  Dialog.cs             BarisDialog, PilihanDialog (data serializable)
  EfekDialog.cs         kode efek teks -> panggilan Core
  JalannyaDialog.cs     alur satu percakapan
  BerkasSave.cs         tulis/baca checkpoint.json (atomik)
  Checkpoint.cs         simpan, muat setelah tertangkap
  GameState.cs          + BateraiPersen (modify)
  SaveData.cs           + validasi BateraiPersen (modify)
Assets/_Project/Scripts/Runtime/
  TheSending.Runtime.asmdef
  GameManager.cs        singleton, state, jeda, jam, save
  KontrolPemain.cs      semua InputAction (kode)
  PemainGerak.cs        CharacterController, lihat, lari, napas
  Interaktif.cs         basis benda yang bisa diinteraksi
  PemainInteraksi.cs    raycast + tombol E
  UiPabrik.cs           pembuat elemen uGUI lewat kode
  Hud.cs                titik bidik, prompt, jam, baterai, notifikasi
  SenterHP.cs  KompasHP.cs  KameraHP.cs
  PenandaMedia.cs  PetunjukFoto.cs  PengisiBaterai.cs
  DataDialog.cs  PanelDialog.cs  PemicuDialog.cs
  DataJurnal.cs  PanelJurnal.cs
  KartuTeks.cs  AktifDiMalam.cs
Assets/_Project/Scripts/Editor/
  TheSending.Editor.asmdef
  PembuatSandbox.cs     membuat Scene_Sandbox0B + data contoh
Assets/_Project/Tests/EditMode/
  NapasTests.cs  BateraiKompasTests.cs  EfekDialogTests.cs  JalannyaDialogTests.cs  BerkasSaveTests.cs
```

---

### Task 1: Aturan Core untuk napas, baterai, kompas, dan rentang malam

**Files:**
- Create: `Assets/_Project/Scripts/Core/Napas.cs`, `BateraiSenter.cs`, `Kompas.cs`, `RentangMalam.cs`
- Modify: `Assets/_Project/Scripts/Core/GameState.cs` (tambah field `BateraiPersen`), `Assets/_Project/Scripts/Core/SaveData.cs` (validasi)
- Test: `Assets/_Project/Tests/EditMode/NapasTests.cs`, `Assets/_Project/Tests/EditMode/BateraiKompasTests.cs`

**Interfaces:**
- Consumes: `GameState`, `SaveData` (Fase 0A).
- Produces:
  - `class Napas`: `const float BatasDetik = 8f`, `const float PulihPerDetik = 0.5f`; `float Sisa`, `bool SedangMenahan`; `void Tick(float detik, bool inginMenahan)`.
  - `GameState.BateraiPersen` (float, awal 100).
  - `static class BateraiSenter`: `const float Penuh = 100f`, `const float DetikNyalaSampaiHabis = 600f`; `bool BisaNyala(GameState)`, `void Pakai(GameState, float detik)`, `void Isi(GameState)`.
  - `static class Kompas`: `const float JarakDeteksi = 12f`, `PutaranMin = 90f`, `PutaranMaks = 1080f`; `float KecepatanPutar(float jarakMediaTerdekat)` (derajat per detik).
  - `static class RentangMalam`: `bool Berisi(int malam, int dari, int sampai)`.

Aturan: menahan napas maksimal 8 detik; setelah habis, tombol harus dilepas dulu sebelum bisa menahan lagi (mencegah kedip). Baterai habis dalam 10 menit nyala nyata dan diisi penuh di kamar Raka. Jarum kompas diam (0) jika media terdekat ≥ 12 m, makin dekat makin cepat.

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/NapasTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class NapasTests
    {
        [Test]
        public void MenahanMengurangiSisa()
        {
            var n = new Napas();
            n.Tick(3f, true);
            Assert.IsTrue(n.SedangMenahan);
            Assert.AreEqual(Napas.BatasDetik - 3f, n.Sisa, 0.001f);
        }

        [Test]
        public void SetelahBatasHabisNapasTerlepasWalauTombolMasihDitahan()
        {
            var n = new Napas();
            n.Tick(Napas.BatasDetik, true);
            Assert.IsFalse(n.SedangMenahan);
            n.Tick(1f, true);
            Assert.IsFalse(n.SedangMenahan);
        }

        [Test]
        public void HarusMelepasTombolSebelumBisaMenahanLagi()
        {
            var n = new Napas();
            n.Tick(Napas.BatasDetik, true);
            n.Tick(2f, false);                 // lepas, pulih 1 detik
            n.Tick(0.1f, true);
            Assert.IsTrue(n.SedangMenahan);
        }

        [Test]
        public void TidakMenahanMemulihkanSampaiBatas()
        {
            var n = new Napas();
            n.Tick(4f, true);
            n.Tick(100f, false);
            Assert.IsFalse(n.SedangMenahan);
            Assert.AreEqual(Napas.BatasDetik, n.Sisa, 0.001f);
        }
    }
}
```

`Assets/_Project/Tests/EditMode/BateraiKompasTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class BateraiKompasTests
    {
        [Test]
        public void BateraiAwalPenuhDanHabisSetelah600Detik()
        {
            var s = new GameState();
            Assert.AreEqual(BateraiSenter.Penuh, s.BateraiPersen);
            BateraiSenter.Pakai(s, 300f);
            Assert.AreEqual(50f, s.BateraiPersen, 0.001f);
            BateraiSenter.Pakai(s, 1000f);
            Assert.AreEqual(0f, s.BateraiPersen);
            Assert.IsFalse(BateraiSenter.BisaNyala(s));
        }

        [Test]
        public void IsiMengembalikanBateraiPenuh()
        {
            var s = new GameState { BateraiPersen = 3f };
            BateraiSenter.Isi(s);
            Assert.AreEqual(BateraiSenter.Penuh, s.BateraiPersen);
        }

        [Test]
        public void BateraiDiLuarRentangDitolakSaatMuatSave()
        {
            var asli = SaveData.KeJson(new GameState());
            var json = asli.Replace("\"BateraiPersen\":100.0", "\"BateraiPersen\":150.0");
            Assert.AreNotEqual(asli, json);
            Assert.Throws<FormatException>(() => SaveData.DariJson(json));
        }

        [TestCase(20f, 0f)]
        [TestCase(12f, 0f)]
        [TestCase(6f, 585f)]
        [TestCase(0f, 1080f)]
        public void JarumKompasMakinCepatSaatMediaMakinDekat(float jarak, float derajatPerDetik)
        {
            Assert.AreEqual(derajatPerDetik, Kompas.KecepatanPutar(jarak), 0.001f);
        }

        [Test]
        public void JarakTidakValidMembuatJarumDiam()
        {
            Assert.AreEqual(0f, Kompas.KecepatanPutar(float.NaN));
            Assert.AreEqual(0f, Kompas.KecepatanPutar(-1f));
        }

        [TestCase(3, 2, 5, true)]
        [TestCase(1, 2, 5, false)]
        [TestCase(6, 2, 5, false)]
        public void RentangMalamInklusif(int malam, int dari, int sampai, bool aktif)
        {
            Assert.AreEqual(aktif, RentangMalam.Berisi(malam, dari, sampai));
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh "TheSending.Tests.NapasTests;TheSending.Tests.BateraiKompasTests"`
Expected: compile error (`Napas`, `BateraiSenter`, `Kompas`, `RentangMalam`, `BateraiPersen` tidak ditemukan).

- [ ] **Step 3: Tambahkan field dan validasi**

Di `GameState.cs`, tambahkan setelah baris `public int KondisiIbu = 60; ...`:
```csharp
        public float BateraiPersen = 100f;  // = BateraiSenter.Penuh, dicek oleh BateraiKompasTests
```

Di `SaveData.cs`, di dalam `Validasi`, setelah pengecekan `MenitGim`, tambahkan:
```csharp
            if (!(s.BateraiPersen >= 0f && s.BateraiPersen <= 100f))
                throw new FormatException($"BateraiPersen = {s.BateraiPersen}, harus 0..100.");
```

- [ ] **Step 4: Buat empat kelas Core**

`Assets/_Project/Scripts/Core/Napas.cs`:
```csharp
using System;

namespace TheSending.Core
{
    // GDD: menahan napas mengurangi suara, tetapi tidak bisa terlalu lama.
    public class Napas
    {
        public const float BatasDetik = 8f;
        public const float PulihPerDetik = 0.5f;

        bool harusLepasDulu;

        public float Sisa { get; private set; } = BatasDetik;
        public bool SedangMenahan { get; private set; }

        public void Tick(float detik, bool inginMenahan)
        {
            if (!inginMenahan) harusLepasDulu = false;

            if (inginMenahan && !harusLepasDulu && Sisa > 0f)
            {
                SedangMenahan = true;
                Sisa = Math.Max(0f, Sisa - detik);
                if (Sisa <= 0f)
                {
                    SedangMenahan = false;
                    harusLepasDulu = true;
                }
                return;
            }

            SedangMenahan = false;
            Sisa = Math.Min(BatasDetik, Sisa + PulihPerDetik * detik);
        }
    }
}
```

`Assets/_Project/Scripts/Core/BateraiSenter.cs`:
```csharp
using System;

namespace TheSending.Core
{
    // GDD: baterai senter HP berkurang perlahan dan hanya bisa diisi di kamar Raka.
    public static class BateraiSenter
    {
        public const float Penuh = 100f;
        public const float DetikNyalaSampaiHabis = 600f; // nilai awal, disetel saat playtest

        public static bool BisaNyala(GameState s) => s.BateraiPersen > 0f;

        public static void Pakai(GameState s, float detik)
        {
            if (detik <= 0f || s.BateraiPersen <= 0f) return;
            s.BateraiPersen = Math.Max(0f, s.BateraiPersen - detik * Penuh / DetikNyalaSampaiHabis);
        }

        public static void Isi(GameState s) => s.BateraiPersen = Penuh;
    }
}
```

`Assets/_Project/Scripts/Core/Kompas.cs`:
```csharp
namespace TheSending.Core
{
    // GDD: jarum kompas berputar tidak karuan di dekat media santet, makin dekat makin kencang.
    public static class Kompas
    {
        public const float JarakDeteksi = 12f;
        public const float PutaranMin = 90f, PutaranMaks = 1080f; // derajat per detik

        public static float KecepatanPutar(float jarakMediaTerdekat)
        {
            if (float.IsNaN(jarakMediaTerdekat) || jarakMediaTerdekat < 0f || jarakMediaTerdekat >= JarakDeteksi)
                return 0f;
            float dekat = 1f - jarakMediaTerdekat / JarakDeteksi;
            return PutaranMin + (PutaranMaks - PutaranMin) * dekat;
        }
    }
}
```

`Assets/_Project/Scripts/Core/RentangMalam.cs`:
```csharp
namespace TheSending.Core
{
    public static class RentangMalam
    {
        public static bool Berisi(int malam, int dari, int sampai) => malam >= dari && malam <= sampai;
    }
}
```

- [ ] **Step 5: Jalankan test, pastikan lulus, lalu seluruh suite**

Run: `bash tools/run-editmode-tests.sh "TheSending.Tests.NapasTests;TheSending.Tests.BateraiKompasTests"`
Expected: `result="Passed" total="14" passed="14" failed="0"`
Run: `bash tools/run-editmode-tests.sh` → semua lulus (170).

- [ ] **Step 6: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add breath, flashlight battery, compass and night range rules"
```

---
### Task 2: Data dialog, efek dialog, dan alur percakapan (Core)

**Files:**
- Create: `Assets/_Project/Scripts/Core/Dialog.cs`, `Assets/_Project/Scripts/Core/EfekDialog.cs`, `Assets/_Project/Scripts/Core/JalannyaDialog.cs`
- Test: `Assets/_Project/Tests/EditMode/EfekDialogTests.cs`, `Assets/_Project/Tests/EditMode/JalannyaDialogTests.cs`

**Interfaces:**
- Consumes: `TujuanMalam.Selesaikan`, `PapanBukti.TambahBukti/TambahKatanya`, `BukuHarian.Temukan`, `MediaSantet.Temukan/Serahkan`, `KepercayaanSumi.Tambah/Tuduh/TerimaMinyakSereh`, `RitualPenangkal.Pelajari`, `PenentuEnding.Pilih`, `Kode.*`.
- Produces:
  - `[Serializable] class PilihanDialog { string Teks; string Efek; int LompatKe = -1; }` (`-1` = percakapan selesai).
  - `[Serializable] class BarisDialog { string Pembicara; string Teks; string Terjemahan; string Efek; bool Akhir; PilihanDialog[] Pilihan; }`.
  - `static class EfekDialog`: `void Jalankan(GameState, string kode)`. Kode (dipisah `;`): `tujuan:<id>`, `bukti:B1..B8`, `katanya:K1..K4`, `halaman:H1..H8`, `media:M1..M7`, `serahkan:M1..M7`, `kepercayaan:<AksiSumi>`, `tuduh-sumi`, `ritual-dari-sumi`, `minyak-sereh`, `pilihan-akhir:Lapor|Balas`. Kode tidak dikenal atau isi salah → `FormatException`.
  - `class JalannyaDialog(GameState, IReadOnlyList<BarisDialog>)`: `int Indeks`, `bool Selesai`, `BarisDialog Sekarang`, `bool MenungguPilihan`, `void Lanjut()`, `void Pilih(int nomor)`. Efek baris dijalankan saat baris itu tampil; efek pilihan dijalankan saat dipilih.

Tujuan: penulis dialog mengisi efek sebagai teks di Inspector (misalnya `kepercayaan:MintaMaaf` atau `bukti:B5;halaman:H7`), dan semua efek tetap lewat aturan Core.

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/EfekDialogTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class EfekDialogTests
    {
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void KodeKosongTidakMengubahApaPun(string kode)
        {
            var s = new GameState();
            EfekDialog.Jalankan(s, kode);
            Assert.AreEqual(SaveData.KeJson(new GameState()), SaveData.KeJson(s));
        }

        [Test]
        public void BeberapaEfekDipisahTitikKoma()
        {
            var s = new GameState();
            EfekDialog.Jalankan(s, "katanya:K1; bukti:B2");
            Assert.IsTrue(s.BuktiKunci[Kode.B2]);
            Assert.AreEqual(StatusInfo.Terbantahkan, s.StatusKatanya[Kode.K1]);
        }

        [Test]
        public void HalamanTujuhMemberiB7()
        {
            var s = new GameState();
            EfekDialog.Jalankan(s, "halaman:H7");
            Assert.IsTrue(s.HalamanDiary[Kode.H7]);
            Assert.IsTrue(s.BuktiKunci[Kode.B7]);
        }

        [Test]
        public void KepercayaanBertambah()
        {
            var s = new GameState { MalamKe = 3 };
            EfekDialog.Jalankan(s, "kepercayaan:MintaMaaf");
            Assert.AreEqual(1, s.KepercayaanSumi);
        }

        [Test]
        public void TujuanDitandaiSelesai()
        {
            var s = new GameState { MalamKe = 5 };
            EfekDialog.Jalankan(s, "tujuan:BicaraLaras");
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.BicaraLaras));
        }

        [Test]
        public void MediaDitemukanLaluDiserahkan()
        {
            var s = new GameState { MalamKe = 2 };
            EfekDialog.Jalankan(s, "media:M1;serahkan:M1");
            Assert.IsTrue(s.MediaDiserahkan[Kode.M1]);
            Assert.AreEqual(52, s.KondisiIbu);
        }

        [Test]
        public void PilihanAkhirLewatPenentuEnding()
        {
            var s = new GameState { MalamKe = 7, HasilSidang = HasilSidang.Berhasil, MenitGim = 1300f };
            EfekDialog.Jalankan(s, "pilihan-akhir:Lapor");
            Assert.AreEqual(PilihanAkhir.Lapor, s.PilihanAkhir);
        }

        [TestCase("bukit:B1")]
        [TestCase("bukti:B9")]
        [TestCase("bukti:B0")]
        [TestCase("bukti:K1")]
        [TestCase("kepercayaan:Peluk")]
        [TestCase("tujuan:")]
        public void KodeSalahDitolak(string kode)
        {
            Assert.Throws<FormatException>(() => EfekDialog.Jalankan(new GameState { MalamKe = 3 }, kode));
        }
    }
}
```

`Assets/_Project/Tests/EditMode/JalannyaDialogTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class JalannyaDialogTests
    {
        static BarisDialog Baris(string teks, string efek = null, bool akhir = false, params PilihanDialog[] pilihan) =>
            new BarisDialog { Pembicara = "Laras", Teks = teks, Efek = efek, Akhir = akhir, Pilihan = pilihan };

        static PilihanDialog Pilihan(string teks, string efek, int lompatKe) =>
            new PilihanDialog { Teks = teks, Efek = efek, LompatKe = lompatKe };

        [Test]
        public void BarisBerjalanBerurutanLaluSelesai()
        {
            var d = new JalannyaDialog(new GameState(), new[] { Baris("satu"), Baris("dua") });
            Assert.AreEqual("satu", d.Sekarang.Teks);
            d.Lanjut();
            Assert.AreEqual("dua", d.Sekarang.Teks);
            d.Lanjut();
            Assert.IsTrue(d.Selesai);
            Assert.IsNull(d.Sekarang);
        }

        [Test]
        public void EfekBarisDijalankanSaatBarisTampil()
        {
            var s = new GameState();
            var d = new JalannyaDialog(s, new[] { Baris("satu"), Baris("dua", "bukti:B4") });
            Assert.IsFalse(s.BuktiKunci[Kode.B4]);
            d.Lanjut();
            Assert.IsTrue(s.BuktiKunci[Kode.B4]);
        }

        [Test]
        public void BarisAkhirMenutupPercakapan()
        {
            var d = new JalannyaDialog(new GameState(), new[] { Baris("satu", akhir: true), Baris("tidak tampil") });
            d.Lanjut();
            Assert.IsTrue(d.Selesai);
        }

        [Test]
        public void PilihanMenjalankanEfekDanMelompat()
        {
            var s = new GameState { MalamKe = 3 };
            var d = new JalannyaDialog(s, new[]
            {
                Baris("Bu Sumi menatap dingin.", null, false,
                    Pilihan("Bu, saya minta maaf soal yang dulu.", "kepercayaan:MintaMaaf", 2),
                    Pilihan("Ibu yang bikin ibu saya sakit, kan?", "tuduh-sumi", -1)),
                Baris("tidak tampil"),
                Baris("Aku ora tau nyantet sapa-sapa."),
            });
            Assert.IsTrue(d.MenungguPilihan);
            d.Pilih(0);
            Assert.AreEqual(2, d.Indeks);
            Assert.AreEqual(1, s.KepercayaanSumi);
        }

        [Test]
        public void PilihanDenganLompatMinusSatuMenutupPercakapan()
        {
            var s = new GameState { MalamKe = 3 };
            var d = new JalannyaDialog(s, new[] { Baris("?", null, false, Pilihan("Tuduh", "tuduh-sumi", -1)) });
            d.Pilih(0);
            Assert.IsTrue(d.Selesai);
            Assert.IsFalse(KepercayaanSumi.PintuTerbuka(s));
        }

        [Test]
        public void LanjutSaatMenungguPilihanDitolak()
        {
            var d = new JalannyaDialog(new GameState(), new[] { Baris("?", null, false, Pilihan("ya", null, -1)) });
            Assert.Throws<InvalidOperationException>(() => d.Lanjut());
        }

        [Test]
        public void DialogKosongDitolak()
        {
            Assert.Throws<ArgumentException>(() => new JalannyaDialog(new GameState(), new BarisDialog[0]));
        }

        [Test]
        public void LompatanDiLuarDaftarDitolak()
        {
            var d = new JalannyaDialog(new GameState(), new[] { Baris("?", null, false, Pilihan("ya", null, 5)) });
            Assert.Throws<ArgumentOutOfRangeException>(() => d.Pilih(0));
        }
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh "TheSending.Tests.EfekDialogTests;TheSending.Tests.JalannyaDialogTests"`
Expected: compile error (`EfekDialog`, `JalannyaDialog`, `BarisDialog`, `PilihanDialog` tidak ditemukan).

- [ ] **Step 3: Buat `Dialog.cs`**

```csharp
using System;

namespace TheSending.Core
{
    [Serializable]
    public class PilihanDialog
    {
        public string Teks;
        public string Efek;          // kode EfekDialog, boleh kosong
        public int LompatKe = -1;    // indeks baris berikutnya; -1 = percakapan selesai
    }

    [Serializable]
    public class BarisDialog
    {
        public string Pembicara;
        public string Teks;
        public string Terjemahan;    // terjemahan kalimat bahasa Jawa, boleh kosong
        public string Efek;          // dijalankan saat baris ini tampil
        public bool Akhir;           // percakapan selesai setelah baris ini
        public PilihanDialog[] Pilihan = new PilihanDialog[0];
    }
}
```

- [ ] **Step 4: Buat `EfekDialog.cs`**

```csharp
using System;

namespace TheSending.Core
{
    // Kode efek yang ditulis penulis dialog di Inspector, dipisah ';'.
    // Kode tidak dikenal -> FormatException, supaya salah ketik ketahuan saat tes, bukan diam-diam diabaikan.
    public static class EfekDialog
    {
        public static void Jalankan(GameState s, string kode)
        {
            if (string.IsNullOrWhiteSpace(kode)) return;
            foreach (var bagian in kode.Split(';'))
            {
                var k = bagian.Trim();
                if (k.Length == 0) continue;
                int titikDua = k.IndexOf(':');
                string nama = (titikDua < 0 ? k : k.Substring(0, titikDua)).Trim();
                string isi = titikDua < 0 ? "" : k.Substring(titikDua + 1).Trim();
                JalankanSatu(s, nama, isi, k);
            }
        }

        static void JalankanSatu(GameState s, string nama, string isi, string asli)
        {
            switch (nama)
            {
                case "tujuan": TujuanMalam.Selesaikan(s, Wajib(isi, asli)); break;
                case "bukti": PapanBukti.TambahBukti(s, Nomor(isi, 'B', Kode.JumlahBukti, asli)); break;
                case "katanya": PapanBukti.TambahKatanya(s, Nomor(isi, 'K', Kode.JumlahKatanya, asli)); break;
                case "halaman": BukuHarian.Temukan(s, Nomor(isi, 'H', Kode.JumlahHalaman, asli)); break;
                case "media": MediaSantet.Temukan(s, Nomor(isi, 'M', Kode.JumlahMedia, asli)); break;
                case "serahkan": MediaSantet.Serahkan(s, Nomor(isi, 'M', Kode.JumlahMedia, asli)); break;
                case "kepercayaan": KepercayaanSumi.Tambah(s, NilaiEnum<AksiSumi>(isi, asli)); break;
                case "tuduh-sumi": KepercayaanSumi.Tuduh(s); break;
                case "ritual-dari-sumi": RitualPenangkal.Pelajari(s, dariBuSumi: true); break;
                case "minyak-sereh": KepercayaanSumi.TerimaMinyakSereh(s); break;
                case "pilihan-akhir": PenentuEnding.Pilih(s, NilaiEnum<PilihanAkhir>(isi, asli)); break;
                default: throw new FormatException($"Efek dialog tidak dikenal: '{asli}'.");
            }
        }

        static string Wajib(string isi, string asli)
        {
            if (isi.Length == 0) throw new FormatException($"Efek '{asli}' butuh isi setelah ':'.");
            return isi;
        }

        // "B5" -> indeks 4
        static int Nomor(string isi, char huruf, int jumlah, string asli)
        {
            if (isi.Length >= 2 && char.ToUpperInvariant(isi[0]) == huruf
                && int.TryParse(isi.Substring(1), out int n) && n >= 1 && n <= jumlah)
                return n - 1;
            throw new FormatException($"Efek '{asli}': harus {huruf}1..{huruf}{jumlah}.");
        }

        static T NilaiEnum<T>(string isi, string asli) where T : struct
        {
            if (Enum.TryParse(isi, false, out T hasil) && Enum.IsDefined(typeof(T), hasil)) return hasil;
            throw new FormatException($"Efek '{asli}': nilai '{isi}' tidak dikenal.");
        }
    }
}
```

- [ ] **Step 5: Buat `JalannyaDialog.cs`**

```csharp
using System;
using System.Collections.Generic;

namespace TheSending.Core
{
    // Alur satu percakapan. Baris berjalan berurutan; baris yang punya pilihan menunggu Pilih().
    public class JalannyaDialog
    {
        readonly IReadOnlyList<BarisDialog> baris;
        readonly GameState s;

        public int Indeks { get; private set; }
        public bool Selesai { get; private set; }
        public BarisDialog Sekarang => Selesai ? null : baris[Indeks];
        public bool MenungguPilihan => !Selesai && baris[Indeks].Pilihan != null && baris[Indeks].Pilihan.Length > 0;

        public JalannyaDialog(GameState s, IReadOnlyList<BarisDialog> baris)
        {
            if (baris == null || baris.Count == 0) throw new ArgumentException("Dialog kosong.", nameof(baris));
            this.s = s ?? throw new ArgumentNullException(nameof(s));
            this.baris = baris;
            Masuk(0);
        }

        public void Lanjut()
        {
            if (Selesai) return;
            if (MenungguPilihan) throw new InvalidOperationException("Baris ini menunggu pilihan.");
            if (baris[Indeks].Akhir || Indeks + 1 >= baris.Count) Selesai = true;
            else Masuk(Indeks + 1);
        }

        public void Pilih(int nomor)
        {
            if (!MenungguPilihan) throw new InvalidOperationException("Tidak ada pilihan di baris ini.");
            var pilihan = baris[Indeks].Pilihan;
            if (nomor < 0 || nomor >= pilihan.Length) throw new ArgumentOutOfRangeException(nameof(nomor));
            var p = pilihan[nomor];
            EfekDialog.Jalankan(s, p.Efek);
            if (p.LompatKe < 0) Selesai = true;
            else Masuk(p.LompatKe);
        }

        void Masuk(int i)
        {
            if (i < 0 || i >= baris.Count) throw new ArgumentOutOfRangeException(nameof(i), i, "Lompatan dialog di luar daftar baris.");
            Indeks = i;
            EfekDialog.Jalankan(s, baris[i].Efek);
        }
    }
}
```

- [ ] **Step 6: Jalankan test, pastikan lulus, lalu seluruh suite**

Run: `bash tools/run-editmode-tests.sh "TheSending.Tests.EfekDialogTests;TheSending.Tests.JalannyaDialogTests"`
Expected: `result="Passed" total="23" passed="23" failed="0"`
Run: `bash tools/run-editmode-tests.sh` → semua lulus (193).

- [ ] **Step 7: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add dialog data, text-coded dialog effects and conversation flow"
```

---

### Task 3: Berkas save dan checkpoint (Core)

**Files:**
- Create: `Assets/_Project/Scripts/Core/BerkasSave.cs`, `Assets/_Project/Scripts/Core/Checkpoint.cs`
- Test: `Assets/_Project/Tests/EditMode/BerkasSaveTests.cs`

**Interfaces:**
- Consumes: `SaveData.KeJson/DariJson`, `KondisiIbu.SetelahTertangkap`.
- Produces:
  - `static class BerkasSave`: `const string NamaFile = "checkpoint.json"`; `string Jalur(string folder)`, `bool Ada(string folder)`, `void Tulis(string folder, GameState)`, `GameState Baca(string folder)` (null jika belum ada atau rusak, dengan `Debug.LogWarning`), `void Hapus(string folder)`.
  - `static class Checkpoint`: `void Simpan(string folder, GameState)`, `GameState MuatSetelahTertangkap(string folder)` (null jika belum ada checkpoint; jika ada: kurangi 5 lalu **simpan ulang**).

Folder dikirim sebagai parameter (Runtime memakai `Application.persistentDataPath`), sehingga test bisa memakai folder sementara. Penulisan lewat file `.tmp` lalu `File.Replace`, supaya save lama tidak rusak jika gim mati saat menulis.

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/BerkasSaveTests.cs`:
```csharp
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
    }
}
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.BerkasSaveTests`
Expected: compile error (`BerkasSave`, `Checkpoint` tidak ditemukan).

- [ ] **Step 3: Buat `BerkasSave.cs` dan `Checkpoint.cs`**

`Assets/_Project/Scripts/Core/BerkasSave.cs`:
```csharp
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
        }

        public static void Hapus(string folder)
        {
            if (File.Exists(Jalur(folder))) File.Delete(Jalur(folder));
        }
    }
}
```

`Assets/_Project/Scripts/Core/Checkpoint.cs`:
```csharp
namespace TheSending.Core
{
    public static class Checkpoint
    {
        public static void Simpan(string folder, GameState s) => BerkasSave.Tulis(folder, s);

        // Desain 4.2: muat checkpoint, kurangi 5, lalu simpan ulang supaya penalti tidak hilang
        // saat pemain keluar gim. null jika belum ada checkpoint.
        public static GameState MuatSetelahTertangkap(string folder)
        {
            var s = BerkasSave.Baca(folder);
            if (s == null) return null;
            KondisiIbu.SetelahTertangkap(s);
            BerkasSave.Tulis(folder, s);
            return s;
        }
    }
}
```

- [ ] **Step 4: Jalankan test, pastikan lulus, lalu seluruh suite**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.BerkasSaveTests`
Expected: `result="Passed" total="6" passed="6" failed="0"`
Run: `bash tools/run-editmode-tests.sh` → semua lulus (199).

- [ ] **Step 5: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add atomic save file and checkpoint with persisted capture penalty"
```

---
### Task 4: Assembly Runtime, GameManager, kontrol, dan gerak pemain

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/TheSending.Runtime.asmdef`, `GameManager.cs`, `KontrolPemain.cs`, `PemainGerak.cs`

**Interfaces:**
- Consumes: `GameState`, `JamGim.Maju`, `BerkasSave`, `Checkpoint`, `KondisiIbu.SetelahTertangkap`, `Napas`, `Gerak`, `Ending`, `Perubahan`.
- Produces:
  - `GameManager` (MonoBehaviour, `[DefaultExecutionOrder(-100)]`): `static GameManager Instance`, `GameState State`, `KontrolPemain Kontrol`, `bool Dijeda`, `bool AdaSave`, `event Action<Perubahan> Berubah` (diteruskan dari state aktif), `void MulaiBaru()`, `bool Lanjutkan()`, `void SimpanCheckpoint()`, `void Tertangkap()`, `void Jeda(bool aktif)` (penghitung; jam dan gerak berhenti selama > 0).
  - `KontrolPemain` (C# biasa): `InputAction Gerak, Lihat, Lari, Interaksi, Senter, Kamera, Kompas, TahanNapas, Jurnal, Inventaris, JedaMenu, Klik`; `InputAction[] PilihanAngka` (tombol 1–4); `void Aktifkan()`, `void Buang()`.
  - `PemainGerak` (MonoBehaviour, butuh `CharacterController`): field `[SerializeField] Transform kamera`; `Gerak GerakSekarang`, `Napas Napas`, `Transform Kamera`; konstanta `KecepatanJalan = 2.2f`, `KecepatanLari = 4.0f`, `SensitivitasMouse = 0.08f`, `BatasDongak = 80f`.

Kode Runtime tidak punya EditMode test. Verifikasinya: seluruh suite tetap lulus (yang berarti semua assembly terkompilasi) dan cek Play Mode di Task 10.

- [ ] **Step 1: Buat `TheSending.Runtime.asmdef`**

```json
{
    "name": "TheSending.Runtime",
    "rootNamespace": "TheSending.Runtime",
    "references": [
        "TheSending.Core",
        "Unity.InputSystem",
        "UnityEngine.UI"
    ],
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

- [ ] **Step 2: Buat `KontrolPemain.cs`**

```csharp
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace TheSending.Runtime
{
    // Semua tombol gim sesuai GDD, didefinisikan lewat kode supaya tidak ada aset input yang harus diedit.
    public class KontrolPemain
    {
        public readonly InputAction Gerak = new InputAction("Gerak", InputActionType.Value);
        public readonly InputAction Lihat = new InputAction("Lihat", InputActionType.Value, "<Mouse>/delta");
        public readonly InputAction Lari = new InputAction("Lari", InputActionType.Button, "<Keyboard>/leftShift");
        public readonly InputAction Interaksi = new InputAction("Interaksi", InputActionType.Button, "<Keyboard>/e");
        public readonly InputAction Senter = new InputAction("Senter", InputActionType.Button, "<Keyboard>/f");
        public readonly InputAction Kamera = new InputAction("Kamera", InputActionType.Button, "<Keyboard>/q");
        public readonly InputAction Kompas = new InputAction("Kompas", InputActionType.Button, "<Keyboard>/c");
        public readonly InputAction TahanNapas = new InputAction("TahanNapas", InputActionType.Button, "<Keyboard>/space");
        public readonly InputAction Jurnal = new InputAction("Jurnal", InputActionType.Button, "<Keyboard>/tab");
        public readonly InputAction Inventaris = new InputAction("Inventaris", InputActionType.Button, "<Keyboard>/i");
        public readonly InputAction JedaMenu = new InputAction("JedaMenu", InputActionType.Button, "<Keyboard>/escape");
        public readonly InputAction Klik = new InputAction("Klik", InputActionType.Button, "<Mouse>/leftButton");
        public readonly InputAction[] PilihanAngka =
        {
            new InputAction("Pilihan1", InputActionType.Button, "<Keyboard>/1"),
            new InputAction("Pilihan2", InputActionType.Button, "<Keyboard>/2"),
            new InputAction("Pilihan3", InputActionType.Button, "<Keyboard>/3"),
            new InputAction("Pilihan4", InputActionType.Button, "<Keyboard>/4"),
        };

        public KontrolPemain()
        {
            Gerak.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
        }

        IEnumerable<InputAction> Semua()
        {
            var daftar = new List<InputAction>
            {
                Gerak, Lihat, Lari, Interaksi, Senter, Kamera, Kompas, TahanNapas, Jurnal, Inventaris, JedaMenu, Klik,
            };
            daftar.AddRange(PilihanAngka);
            return daftar;
        }

        public void Aktifkan()
        {
            foreach (var a in Semua()) a.Enable();
        }

        public void Buang()
        {
            foreach (var a in Semua()) a.Dispose();
        }
    }
}
```

- [ ] **Step 3: Buat `GameManager.cs`**

```csharp
using System;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Satu-satunya singleton. Menyimpan GameState, menjalankan jam gim, mengurus save, checkpoint, dan jeda.
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; }
        public KontrolPemain Kontrol { get; private set; }
        public bool Dijeda => jumlahJeda > 0;
        public bool AdaSave => BerkasSave.Ada(Folder);

        // Diteruskan dari GameState aktif. UI berlangganan di sini, bukan ke GameState,
        // karena GameState diganti setiap kali save dimuat (catatan 0B di CLAUDE.md).
        public event Action<Perubahan> Berubah;

        int jumlahJeda;
        static string Folder => Application.persistentDataPath;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);   // duplikat dari scene lain; yang pertama tetap hidup
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Kontrol = new KontrolPemain();
            Kontrol.Aktifkan();
            Pasang(new GameState());
        }

        void OnDestroy()
        {
            if (Instance != this) return;
            Kontrol?.Buang();
            Instance = null;
        }

        void Update()
        {
            if (!Dijeda && State.Ending == Ending.Belum) JamGim.Maju(State, Time.deltaTime);
        }

        public void MulaiBaru() => Pasang(new GameState());

        public bool Lanjutkan()
        {
            var s = BerkasSave.Baca(Folder);
            if (s == null) return false;
            Pasang(s);
            return true;
        }

        public void SimpanCheckpoint() => Checkpoint.Simpan(Folder, State);

        // Desain 4.2. Posisi pemain dikembalikan oleh scene malam (titik mulai checkpoint), bukan di sini.
        public void Tertangkap()
        {
            var s = Checkpoint.MuatSetelahTertangkap(Folder);
            if (s != null)
            {
                Pasang(s);
                return;
            }
            KondisiIbu.SetelahTertangkap(State);
            SimpanCheckpoint();
        }

        // Dialog, Jurnal, kartu teks, dan menu memanggil Jeda(true) saat dibuka dan Jeda(false) saat ditutup.
        public void Jeda(bool aktif) => jumlahJeda = Math.Max(0, jumlahJeda + (aktif ? 1 : -1));

        void Pasang(GameState s)
        {
            if (State != null) State.Berubah -= Teruskan;
            State = s;
            State.Berubah += Teruskan;
            Berubah?.Invoke(Perubahan.Malam);                       // minta UI menggambar ulang dari state baru
            if (State.Ending != Ending.Belum) Berubah?.Invoke(Perubahan.Ending);
        }

        void Teruskan(Perubahan p) => Berubah?.Invoke(p);
    }
}
```

- [ ] **Step 4: Buat `PemainGerak.cs`**

```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PemainGerak : MonoBehaviour
    {
        public const float KecepatanJalan = 2.2f, KecepatanLari = 4.0f;
        public const float SensitivitasMouse = 0.08f, BatasDongak = 80f;
        const float Gravitasi = -9.81f;

        [SerializeField] Transform kamera;

        CharacterController cc;
        float dongak, kecepatanJatuh;

        public Gerak GerakSekarang { get; private set; }
        public Napas Napas { get; } = new Napas();
        public Transform Kamera => kamera;

        void Awake() => cc = GetComponent<CharacterController>();

        void Update()
        {
            var gm = GameManager.Instance;
            bool bisaGerak = gm != null && !gm.Dijeda;
            Cursor.lockState = bisaGerak ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !bisaGerak;
            if (!bisaGerak)
            {
                GerakSekarang = Gerak.Diam;
                return;
            }

            var k = gm.Kontrol;
            Vector2 lihat = k.Lihat.ReadValue<Vector2>() * SensitivitasMouse;
            transform.Rotate(0f, lihat.x, 0f);
            dongak = Mathf.Clamp(dongak - lihat.y, -BatasDongak, BatasDongak);
            kamera.localEulerAngles = new Vector3(dongak, 0f, 0f);

            Vector2 masuk = Vector2.ClampMagnitude(k.Gerak.ReadValue<Vector2>(), 1f);
            bool bergerak = masuk.sqrMagnitude > 0.01f;
            bool lari = bergerak && k.Lari.IsPressed();
            Vector3 arah = (transform.right * masuk.x + transform.forward * masuk.y) * (lari ? KecepatanLari : KecepatanJalan);
            kecepatanJatuh = cc.isGrounded ? -1f : kecepatanJatuh + Gravitasi * Time.deltaTime;
            arah.y = kecepatanJatuh;
            cc.Move(arah * Time.deltaTime);

            GerakSekarang = !bergerak ? Gerak.Diam : lari ? Gerak.Lari : Gerak.Jalan;
            Napas.Tick(Time.deltaTime, k.TahanNapas.IsPressed());
        }
    }
}
```

- [ ] **Step 5: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: semua lulus (199) dan tidak ada `error CS` di log. Jika ada error kompilasi di Runtime, perbaiki sesuai pesan error tanpa mengubah API di bagian Interfaces, dan sebutkan di laporan.

- [ ] **Step 6: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add runtime assembly, GameManager, input actions and player movement"
```

---

### Task 5: Interaksi, pembuat UI, dan HUD

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/Interaktif.cs`, `PemainInteraksi.cs`, `UiPabrik.cs`, `Hud.cs`

**Interfaces:**
- Consumes: `GameManager` (Task 4), `JamGim.Format`, `Perubahan`.
- Produces:
  - `abstract class Interaktif : MonoBehaviour`: `[SerializeField] string prompt = "Periksa"`; `virtual string Prompt`, `virtual bool BisaDiinteraksi`, `abstract void Interaksi()`.
  - `PemainInteraksi`: `[SerializeField] Transform kamera`; `const float Jangkauan = 2.2f`; `Interaktif Sasaran`.
  - `static class UiPabrik`: `Font Font`; `Canvas BuatCanvas(string nama, int urutan)`; `RectTransform Isi(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax)`; `Text BuatTeks(Transform induk, string nama, int ukuran, TextAnchor rata, Vector2 anchorMin, Vector2 anchorMax)`; `Image BuatPanel(Transform induk, string nama, Color warna, Vector2 anchorMin, Vector2 anchorMax)`.
  - `Hud`: `[SerializeField] PemainInteraksi interaksi`; `void Tampilkan(string pesan)` (antrean notifikasi, 3 detik per pesan).

Semua UI memakai font bawaan `LegacyRuntime.ttf` dan tidak butuh EventSystem: pilihan dan tab dipilih dengan keyboard. Font dan warna diganti di 0C.

- [ ] **Step 1: Buat `Interaktif.cs` dan `PemainInteraksi.cs`**

`Interaktif.cs`:
```csharp
using UnityEngine;

namespace TheSending.Runtime
{
    // Basis semua benda yang bisa diinteraksi dengan E. Butuh Collider di objek ini atau anaknya.
    public abstract class Interaktif : MonoBehaviour
    {
        [SerializeField] string prompt = "Periksa";

        public virtual string Prompt => prompt;
        public virtual bool BisaDiinteraksi => true;
        public abstract void Interaksi();
    }
}
```

`PemainInteraksi.cs`:
```csharp
using UnityEngine;

namespace TheSending.Runtime
{
    public class PemainInteraksi : MonoBehaviour
    {
        public const float Jangkauan = 2.2f;

        [SerializeField] Transform kamera;

        public Interaktif Sasaran { get; private set; }

        void Update()
        {
            Sasaran = null;
            var gm = GameManager.Instance;
            if (gm == null || gm.Dijeda) return;

            if (Physics.Raycast(kamera.position, kamera.forward, out var hit, Jangkauan, ~0, QueryTriggerInteraction.Collide))
            {
                var benda = hit.collider.GetComponentInParent<Interaktif>();
                if (benda != null && benda.isActiveAndEnabled && benda.BisaDiinteraksi) Sasaran = benda;
            }
            if (Sasaran != null && gm.Kontrol.Interaksi.WasPressedThisFrame()) Sasaran.Interaksi();
        }
    }
}
```

- [ ] **Step 2: Buat `UiPabrik.cs`**

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Membangun uGUI lewat kode, supaya tidak ada prefab UI yang harus disambung manual.
    public static class UiPabrik
    {
        static Font font;
        public static Font Font => font != null ? font : (font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        public static Canvas BuatCanvas(string nama, int urutan)
        {
            var go = new GameObject(nama, typeof(Canvas), typeof(CanvasScaler));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = urutan;
            var skala = go.GetComponent<CanvasScaler>();
            skala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            skala.referenceResolution = new Vector2(1920, 1080);
            skala.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        public static RectTransform Isi(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static Text BuatTeks(Transform induk, string nama, int ukuran, TextAnchor rata, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(nama, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(induk, false);
            Isi((RectTransform)go.transform, anchorMin, anchorMax);
            var t = go.GetComponent<Text>();
            t.font = Font;
            t.fontSize = ukuran;
            t.alignment = rata;
            t.color = new Color(0.9f, 0.88f, 0.82f);
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Truncate;
            t.raycastTarget = false;
            return t;
        }

        public static Image BuatPanel(Transform induk, string nama, Color warna, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(nama, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(induk, false);
            Isi((RectTransform)go.transform, anchorMin, anchorMax);
            var img = go.GetComponent<Image>();
            img.color = warna;
            img.raycastTarget = false;
            return img;
        }
    }
}
```

- [ ] **Step 3: Buat `Hud.cs`**

```csharp
using System.Collections.Generic;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Titik bidik, prompt interaksi, jam HP, baterai, dan notifikasi singkat.
    public class Hud : MonoBehaviour
    {
        const float LamaNotifikasi = 3f;

        [SerializeField] PemainInteraksi interaksi;

        Text prompt, jam, baterai, notifikasi;
        readonly Queue<string> antrean = new Queue<string>();
        float sisaNotifikasi;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("HUD", 10).transform;
            akar.SetParent(transform, false);
            UiPabrik.BuatTeks(akar, "TitikBidik", 28, TextAnchor.MiddleCenter, new Vector2(0.49f, 0.48f), new Vector2(0.51f, 0.52f)).text = "·";
            prompt = UiPabrik.BuatTeks(akar, "Prompt", 30, TextAnchor.UpperCenter, new Vector2(0.3f, 0.36f), new Vector2(0.7f, 0.46f));
            jam = UiPabrik.BuatTeks(akar, "Jam", 34, TextAnchor.UpperRight, new Vector2(0.75f, 0.9f), new Vector2(0.98f, 0.98f));
            baterai = UiPabrik.BuatTeks(akar, "Baterai", 24, TextAnchor.UpperRight, new Vector2(0.75f, 0.85f), new Vector2(0.98f, 0.9f));
            notifikasi = UiPabrik.BuatTeks(akar, "Notifikasi", 28, TextAnchor.LowerLeft, new Vector2(0.02f, 0.04f), new Vector2(0.6f, 0.14f));
        }

        void OnEnable()
        {
            if (GameManager.Instance != null) GameManager.Instance.Berubah += SaatBerubah;
        }

        void OnDisable()
        {
            if (GameManager.Instance != null) GameManager.Instance.Berubah -= SaatBerubah;
        }

        public void Tampilkan(string pesan) => antrean.Enqueue(pesan);

        void SaatBerubah(Perubahan p)
        {
            switch (p)
            {
                case Perubahan.Bukti: Tampilkan("Bukti baru masuk Papan Bukti."); break;
                case Perubahan.Katanya: Tampilkan("Papan Bukti diperbarui."); break;
                case Perubahan.Diary: Tampilkan("Halaman buku harian ditemukan."); break;
                case Perubahan.Media: Tampilkan("Catatan media santet diperbarui."); break;
            }
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var s = gm.State;

            prompt.text = interaksi != null && interaksi.Sasaran != null ? $"[E] {interaksi.Sasaran.Prompt}" : "";
            jam.text = JamGim.Format(s.MenitGim);
            baterai.text = $"Baterai {Mathf.CeilToInt(s.BateraiPersen)}%";

            if (sisaNotifikasi > 0f)
            {
                sisaNotifikasi -= Time.unscaledDeltaTime;
                if (sisaNotifikasi <= 0f) notifikasi.text = "";
            }
            else if (antrean.Count > 0)
            {
                notifikasi.text = antrean.Dequeue();
                sisaNotifikasi = LamaNotifikasi;
            }
        }
    }
}
```

- [ ] **Step 4: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: semua lulus (199), tanpa `error CS`.

- [ ] **Step 5: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add interaction raycast, code-built UI helpers and HUD"
```

---

### Task 6: Alat HP — senter, kompas, kamera, dan pengisi baterai

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/SenterHP.cs`, `KompasHP.cs`, `KameraHP.cs`, `PenandaMedia.cs`, `PetunjukFoto.cs`, `PengisiBaterai.cs`

**Interfaces:**
- Consumes: `GameManager`, `Hud.Tampilkan`, `Interaktif`, `UiPabrik`, `BateraiSenter`, `Kompas.KecepatanPutar`, `EfekDialog.Jalankan`, `Kode.JumlahMedia`.
- Produces:
  - `SenterHP`: `[SerializeField] Light cahaya`; `bool Menyala`. F menyalakan/mematikan; mati sendiri saat baterai habis.
  - `PenandaMedia`: `[SerializeField, Range(0, 6)] int media`; `static float JarakTerdekat(Vector3 posisi, GameState s)` (tak hingga jika tidak ada; hanya media yang belum ditemukan).
  - `KompasHP`: `[SerializeField] Transform pemain`. C membuka/menutup kompas di pojok kanan bawah.
  - `PetunjukFoto`: `[SerializeField] string efek` (kode `EfekDialog`), `[SerializeField] string pesan = "Dipotret."`; `string Pesan`; `void Dipotret(GameState s)`.
  - `KameraHP`: `[SerializeField] Transform kamera`, `[SerializeField] Hud hud`; `const float JangkauanFoto = 6f`. Q memotret: kilatan putih singkat; jika mengenai `PetunjukFoto`, efeknya dijalankan.
  - `PengisiBaterai : Interaktif` (stop kontak di kamar Raka): `[SerializeField] Hud hud`; mengisi baterai penuh.

- [ ] **Step 1: Buat `SenterHP.cs`**

```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    public class SenterHP : MonoBehaviour
    {
        [SerializeField] Light cahaya;

        public bool Menyala => cahaya != null && cahaya.enabled;

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || cahaya == null) return;
            var s = gm.State;

            if (!gm.Dijeda && gm.Kontrol.Senter.WasPressedThisFrame())
                cahaya.enabled = !cahaya.enabled && BateraiSenter.BisaNyala(s);

            if (!cahaya.enabled) return;
            if (!gm.Dijeda) BateraiSenter.Pakai(s, Time.deltaTime);
            if (!BateraiSenter.BisaNyala(s)) cahaya.enabled = false;
        }
    }
}
```

- [ ] **Step 2: Buat `PenandaMedia.cs` dan `KompasHP.cs`**

`PenandaMedia.cs`:
```csharp
using System.Collections.Generic;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Pasang di objek media santet (M1..M7 = 0..6) supaya kompas HP bereaksi sampai media ditemukan.
    public class PenandaMedia : MonoBehaviour
    {
        static readonly List<PenandaMedia> Semua = new List<PenandaMedia>();

        [SerializeField, Range(0, Kode.JumlahMedia - 1)] int media;

        void OnEnable() => Semua.Add(this);
        void OnDisable() => Semua.Remove(this);

        public static float JarakTerdekat(Vector3 posisi, GameState s)
        {
            float terdekat = float.PositiveInfinity;
            foreach (var p in Semua)
            {
                if (s.MediaDitemukan[p.media]) continue;
                terdekat = Mathf.Min(terdekat, Vector3.Distance(posisi, p.transform.position));
            }
            return terdekat;
        }
    }
}
```

`KompasHP.cs`:
```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Jarum menunjuk utara (+Z dunia); di dekat media santet jarum berputar makin kencang.
    public class KompasHP : MonoBehaviour
    {
        [SerializeField] Transform pemain;

        GameObject panel;
        RectTransform jarum;
        float putaran;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Kompas", 11).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.05f, 0.05f, 0.06f, 0.85f), new Vector2(0.84f, 0.06f), new Vector2(0.96f, 0.27f));
            panel = latar.gameObject;
            var j = UiPabrik.BuatPanel(latar.transform, "Jarum", new Color(0.6f, 0.1f, 0.08f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            jarum = j.rectTransform;
            jarum.pivot = new Vector2(0.5f, 0f);
            jarum.sizeDelta = new Vector2(8f, 90f);
            UiPabrik.BuatTeks(latar.transform, "U", 22, TextAnchor.UpperCenter, new Vector2(0f, 0.82f), new Vector2(1f, 1f)).text = "U";
            panel.SetActive(false);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            if (!gm.Dijeda && gm.Kontrol.Kompas.WasPressedThisFrame()) panel.SetActive(!panel.activeSelf);
            if (!panel.activeSelf) return;

            float jarak = PenandaMedia.JarakTerdekat(pemain.position, gm.State);
            putaran = (putaran + Kompas.KecepatanPutar(jarak) * Time.deltaTime) % 360f;
            jarum.localEulerAngles = new Vector3(0f, 0f, pemain.eulerAngles.y + putaran);
        }
    }
}
```

- [ ] **Step 3: Buat `PetunjukFoto.cs` dan `KameraHP.cs`**

`PetunjukFoto.cs`:
```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Petunjuk yang masuk Papan Bukti saat dipotret dengan Q (desain 4.7). Butuh Collider.
    public class PetunjukFoto : MonoBehaviour
    {
        [SerializeField] string efek;
        [SerializeField] string pesan = "Dipotret.";

        public string Pesan => pesan;

        public void Dipotret(GameState s) => EfekDialog.Jalankan(s, efek);
    }
}
```

`KameraHP.cs`:
```csharp
using System;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    public class KameraHP : MonoBehaviour
    {
        public const float JangkauanFoto = 6f;
        const float LamaKilatan = 0.15f;

        [SerializeField] Transform kamera;
        [SerializeField] Hud hud;

        Image kilatan;
        float sisaKilatan;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("KilatanKamera", 30).transform;
            akar.SetParent(transform, false);
            kilatan = UiPabrik.BuatPanel(akar, "Kilatan", new Color(1f, 1f, 1f, 0f), Vector2.zero, Vector2.one);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (sisaKilatan > 0f)
            {
                sisaKilatan -= Time.unscaledDeltaTime;
                kilatan.color = new Color(1f, 1f, 1f, Mathf.Clamp01(sisaKilatan / LamaKilatan) * 0.8f);
            }

            if (gm.Dijeda || !gm.Kontrol.Kamera.WasPressedThisFrame()) return;
            sisaKilatan = LamaKilatan;

            if (!Physics.Raycast(kamera.position, kamera.forward, out var hit, JangkauanFoto, ~0, QueryTriggerInteraction.Collide)) return;
            var petunjuk = hit.collider.GetComponentInParent<PetunjukFoto>();
            if (petunjuk == null) return;
            try
            {
                petunjuk.Dipotret(gm.State);
                if (hud != null) hud.Tampilkan(petunjuk.Pesan);
            }
            catch (FormatException e)
            {
                Debug.LogError($"Efek foto salah di '{petunjuk.name}': {e.Message}", petunjuk);
            }
        }
    }
}
```

- [ ] **Step 4: Buat `PengisiBaterai.cs`**

```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Stop kontak di kamar Raka: satu-satunya tempat mengisi baterai HP (GDD).
    public class PengisiBaterai : Interaktif
    {
        [SerializeField] Hud hud;

        public override void Interaksi()
        {
            BateraiSenter.Isi(GameManager.Instance.State);
            if (hud != null) hud.Tampilkan("Baterai HP penuh.");
        }
    }
}
```

- [ ] **Step 5: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: semua lulus (199), tanpa `error CS`.

- [ ] **Step 6: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add phone flashlight, compass, camera and battery charger"
```

---

### Task 7: Dialog di Runtime — data, panel, dan pemicu

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/DataDialog.cs`, `PanelDialog.cs`, `PemicuDialog.cs`

**Interfaces:**
- Consumes: `BarisDialog`, `PilihanDialog`, `JalannyaDialog` (Task 2), `GameManager.Jeda/Kontrol/State`, `UiPabrik`, `Interaktif`.
- Produces:
  - `DataDialog : ScriptableObject` (`[CreateAssetMenu(menuName = "The Sending/Dialog")]`): `BarisDialog[] Baris`.
  - `PanelDialog`: `bool Terbuka`; `void Mulai(DataDialog data, Action saatSelesai = null)`. E atau klik kiri melanjutkan, tombol 1–4 memilih. Jam dan gerak berhenti selama dialog terbuka. Data dialog yang salah (kode efek tidak dikenal, lompatan di luar daftar) ditulis sebagai `Debug.LogError` dan dialog ditutup, bukan membuat gim crash.
  - `PemicuDialog : Interaktif`: `[SerializeField] DataDialog dialog`, `[SerializeField] PanelDialog panel`, `[SerializeField] bool sekaliSaja`.

- [ ] **Step 1: Buat `DataDialog.cs` dan `PemicuDialog.cs`**

`DataDialog.cs`:
```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Satu percakapan. Penulis mengisi baris, pilihan, dan kode efek (lihat EfekDialog) di Inspector.
    [CreateAssetMenu(menuName = "The Sending/Dialog", fileName = "Dialog")]
    public class DataDialog : ScriptableObject
    {
        public BarisDialog[] Baris = new BarisDialog[0];
    }
}
```

`PemicuDialog.cs`:
```csharp
using UnityEngine;

namespace TheSending.Runtime
{
    public class PemicuDialog : Interaktif
    {
        [SerializeField] DataDialog dialog;
        [SerializeField] PanelDialog panel;
        [SerializeField] bool sekaliSaja;

        bool sudah;

        public override bool BisaDiinteraksi => dialog != null && panel != null && !(sekaliSaja && sudah);

        public override void Interaksi()
        {
            sudah = true;
            panel.Mulai(dialog);
        }
    }
}
```

- [ ] **Step 2: Buat `PanelDialog.cs`**

```csharp
using System;
using System.Text;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Kotak dialog di bawah layar. E atau klik kiri melanjutkan; tombol 1-4 memilih jawaban.
    public class PanelDialog : MonoBehaviour
    {
        const int MaksPilihan = 4;

        GameObject panel;
        Text pembicara, teks, terjemahan, pilihan;
        JalannyaDialog dialog;
        DataDialog sumber;
        Action saatSelesai;
        int frameMulai;

        public bool Terbuka => dialog != null;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Dialog", 20).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.03f, 0.03f, 0.04f, 0.88f), new Vector2(0.1f, 0.03f), new Vector2(0.9f, 0.33f));
            panel = latar.gameObject;
            pembicara = UiPabrik.BuatTeks(latar.transform, "Pembicara", 30, TextAnchor.UpperLeft, new Vector2(0.03f, 0.78f), new Vector2(0.97f, 0.95f));
            teks = UiPabrik.BuatTeks(latar.transform, "Teks", 30, TextAnchor.UpperLeft, new Vector2(0.03f, 0.45f), new Vector2(0.97f, 0.78f));
            terjemahan = UiPabrik.BuatTeks(latar.transform, "Terjemahan", 24, TextAnchor.UpperLeft, new Vector2(0.03f, 0.3f), new Vector2(0.97f, 0.45f));
            terjemahan.fontStyle = FontStyle.Italic;
            pilihan = UiPabrik.BuatTeks(latar.transform, "Pilihan", 26, TextAnchor.UpperLeft, new Vector2(0.05f, 0.03f), new Vector2(0.97f, 0.3f));
            panel.SetActive(false);
        }

        public void Mulai(DataDialog data, Action saatSelesai = null)
        {
            if (Terbuka || data == null) return;
            sumber = data;
            this.saatSelesai = saatSelesai;
            try
            {
                dialog = new JalannyaDialog(GameManager.Instance.State, data.Baris);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException || e is InvalidOperationException)
            {
                Gagal(e);
                return;
            }
            frameMulai = Time.frameCount;   // tombol E yang membuka dialog tidak ikut melanjutkannya
            GameManager.Instance.Jeda(true);
            panel.SetActive(true);
            Gambar();
        }

        void Update()
        {
            if (!Terbuka || Time.frameCount == frameMulai) return;
            var k = GameManager.Instance.Kontrol;
            try
            {
                if (dialog.MenungguPilihan)
                {
                    int jumlah = Math.Min(dialog.Sekarang.Pilihan.Length, MaksPilihan);
                    for (int i = 0; i < jumlah; i++)
                    {
                        if (!k.PilihanAngka[i].WasPressedThisFrame()) continue;
                        dialog.Pilih(i);
                        break;
                    }
                }
                else if (k.Interaksi.WasPressedThisFrame() || k.Klik.WasPressedThisFrame())
                {
                    dialog.Lanjut();
                }
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException || e is InvalidOperationException)
            {
                Gagal(e);
                return;
            }

            if (dialog.Selesai) Tutup();
            else Gambar();
        }

        void Gambar()
        {
            var b = dialog.Sekarang;
            pembicara.text = b.Pembicara;
            teks.text = b.Teks;
            terjemahan.text = string.IsNullOrEmpty(b.Terjemahan) ? "" : $"({b.Terjemahan})";
            if (!dialog.MenungguPilihan)
            {
                pilihan.text = "[E] Lanjut";
                return;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < b.Pilihan.Length && i < MaksPilihan; i++) sb.AppendLine($"{i + 1}. {b.Pilihan[i].Teks}");
            pilihan.text = sb.ToString();
        }

        void Tutup()
        {
            dialog = null;
            panel.SetActive(false);
            GameManager.Instance.Jeda(false);
            var selesai = saatSelesai;
            saatSelesai = null;
            selesai?.Invoke();
        }

        // Data dialog salah adalah kesalahan penulisan: catat jelas, tutup dialog, gim tetap jalan.
        void Gagal(Exception e)
        {
            Debug.LogError($"Dialog '{(sumber != null ? sumber.name : "?")}' berisi data salah: {e.Message}", sumber);
            if (dialog != null) Tutup();
        }
    }
}
```

- [ ] **Step 3: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: semua lulus (199), tanpa `error CS`.

- [ ] **Step 4: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add dialog asset, dialog panel and dialog trigger"
```

---

### Task 8: Jurnal dan Papan Bukti

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/DataJurnal.cs`, `PanelJurnal.cs`

**Interfaces:**
- Consumes: `GameState` (field bukti, Katanya, media, halaman), `StatusInfo`, `Kode.*`, `GameManager`, `UiPabrik`.
- Produces:
  - `[Serializable] class EntriJurnal { string Judul; string Isi; }`.
  - `DataJurnal : ScriptableObject` (`[CreateAssetMenu(menuName = "The Sending/Jurnal")]`): `EntriJurnal[] Bukti` (8), `Katanya` (4), `Media` (7), `HalamanDiary` (8), `CatatanBapak`, `CatatanRaka`.
  - `PanelJurnal`: `[SerializeField] DataJurnal data`; `bool Terbuka`. Tab membuka/menutup (hanya jika tidak sedang jeda karena dialog atau kartu teks); tombol 1/2/3 berganti tab Papan Bukti, Buku Harian, Catatan.

Papan Bukti menampilkan media santet yang ditemukan, bukti yang dimiliki, dan Katanya dengan labelnya (KATANYA, BUKTI, TERBANTAHKAN), sesuai Game Story 6.3.

- [ ] **Step 1: Buat `DataJurnal.cs`**

```csharp
using System;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    [Serializable]
    public class EntriJurnal
    {
        public string Judul;
        [TextArea(2, 8)] public string Isi;
    }

    // Teks Jurnal. Indeks mengikuti Kode (B1 = 0, K1 = 0, M1 = 0, Halaman 1 = 0).
    [CreateAssetMenu(menuName = "The Sending/Jurnal", fileName = "Jurnal")]
    public class DataJurnal : ScriptableObject
    {
        public EntriJurnal[] Bukti = new EntriJurnal[Kode.JumlahBukti];
        public EntriJurnal[] Katanya = new EntriJurnal[Kode.JumlahKatanya];
        public EntriJurnal[] Media = new EntriJurnal[Kode.JumlahMedia];
        public EntriJurnal[] HalamanDiary = new EntriJurnal[Kode.JumlahHalaman];
        public EntriJurnal[] CatatanBapak = new EntriJurnal[0];
        public EntriJurnal[] CatatanRaka = new EntriJurnal[0];
    }
}
```

- [ ] **Step 2: Buat `PanelJurnal.cs`**

```csharp
using System.Text;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    public class PanelJurnal : MonoBehaviour
    {
        enum Tab { PapanBukti, BukuHarian, Catatan }

        static readonly EntriJurnal Kosong = new EntriJurnal { Judul = "(belum ditulis)", Isi = "" };

        [SerializeField] DataJurnal data;

        GameObject panel;
        Text judul, isi;
        Tab tab;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Jurnal", 15).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.09f, 0.07f, 0.05f, 0.95f), new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.92f));
            panel = latar.gameObject;
            judul = UiPabrik.BuatTeks(latar.transform, "Judul", 28, TextAnchor.UpperLeft, new Vector2(0.03f, 0.9f), new Vector2(0.97f, 0.98f));
            isi = UiPabrik.BuatTeks(latar.transform, "Isi", 26, TextAnchor.UpperLeft, new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.88f));
            panel.SetActive(false);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var k = gm.Kontrol;

            if (k.Jurnal.WasPressedThisFrame())
            {
                if (Terbuka)
                {
                    panel.SetActive(false);
                    gm.Jeda(false);
                }
                else if (!gm.Dijeda)
                {
                    panel.SetActive(true);
                    gm.Jeda(true);
                    Gambar(gm.State);
                }
            }
            if (!Terbuka) return;

            for (int i = 0; i < 3; i++)
            {
                if (!k.PilihanAngka[i].WasPressedThisFrame()) continue;
                tab = (Tab)i;
                Gambar(gm.State);
            }
        }

        void Gambar(GameState s)
        {
            judul.text = "[1] Papan Bukti     [2] Buku Harian     [3] Catatan     [Tab] Tutup";
            isi.text = tab == Tab.PapanBukti ? TeksPapan(s)
                     : tab == Tab.BukuHarian ? TeksDiary(s)
                     : TeksCatatan();
        }

        string TeksPapan(GameState s)
        {
            var sb = new StringBuilder();
            for (int m = 0; m < Kode.JumlahMedia; m++)
            {
                if (!s.MediaDitemukan[m]) continue;
                string status = s.MediaDinetralkan[m] ? " (dinetralkan)" : s.MediaDiserahkan[m] ? " (diserahkan)" : "";
                Tulis(sb, $"[MEDIA] M{m + 1} ", Entri(data != null ? data.Media : null, m), status);
            }
            for (int b = 0; b < Kode.JumlahBukti; b++)
                if (s.BuktiKunci[b]) Tulis(sb, $"[BUKTI] B{b + 1} ", Entri(data != null ? data.Bukti : null, b), "");
            for (int k = 0; k < Kode.JumlahKatanya; k++)
                if (s.KatanyaDiketahui[k]) Tulis(sb, $"[{Label(s.StatusKatanya[k])}] K{k + 1} ", Entri(data != null ? data.Katanya : null, k), "");
            return sb.Length == 0 ? "Belum ada informasi." : sb.ToString();
        }

        string TeksDiary(GameState s)
        {
            var sb = new StringBuilder();
            for (int h = 0; h < Kode.JumlahHalaman; h++)
                if (s.HalamanDiary[h]) Tulis(sb, $"Halaman {h + 1}: ", Entri(data != null ? data.HalamanDiary : null, h), "");
            return sb.Length == 0 ? "Belum ada halaman yang ditemukan." : sb.ToString();
        }

        string TeksCatatan()
        {
            var sb = new StringBuilder();
            sb.AppendLine("CATATAN BAPAK");
            if (data != null) foreach (var e in data.CatatanBapak) Tulis(sb, "- ", e ?? Kosong, "");
            sb.AppendLine().AppendLine("CATATAN RAKA");
            if (data != null) foreach (var e in data.CatatanRaka) Tulis(sb, "- ", e ?? Kosong, "");
            return sb.ToString();
        }

        static void Tulis(StringBuilder sb, string awalan, EntriJurnal e, string akhiran)
        {
            sb.Append(awalan).Append(e.Judul).AppendLine(akhiran);
            if (!string.IsNullOrEmpty(e.Isi)) sb.Append("    ").AppendLine(e.Isi);
        }

        static EntriJurnal Entri(EntriJurnal[] daftar, int i) =>
            daftar != null && i < daftar.Length && daftar[i] != null ? daftar[i] : Kosong;

        static string Label(StatusInfo status) =>
            status == StatusInfo.Bukti ? "BUKTI" : status == StatusInfo.Terbantahkan ? "TERBANTAHKAN" : "KATANYA";
    }
}
```

- [ ] **Step 3: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: semua lulus (199), tanpa `error CS`.

- [ ] **Step 4: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add journal data asset and journal panel with evidence board"
```

---

### Task 9: Kartu teks dan objek yang aktif per malam

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/KartuTeks.cs`, `Assets/_Project/Scripts/Runtime/AktifDiMalam.cs`

**Interfaces:**
- Consumes: `GameManager` (`Jeda`, `Berubah`, `State.MalamKe`), `RentangMalam.Berisi` (Task 1), `UiPabrik`, `Perubahan.Malam`.
- Produces:
  - `KartuTeks`: `[SerializeField] string judulSaatMulai`, `[SerializeField] string isiSaatMulai`, `[SerializeField] float detikSaatMulai = 4f`; `bool Tampil`; `IEnumerator Tampilkan(string judul, string isi, float detik)` (memudar masuk, tahan, memudar keluar; jeda selama tampil); `IEnumerator KeHitam()`, `IEnumerator DariHitam()` (layar hitam untuk cutscene). Jika `judulSaatMulai` diisi, kartu tampil otomatis di `Start` (dipakai untuk teks ringkasan siang di awal setiap malam).
  - `AktifDiMalam`: `[SerializeField, Range(0, 7)] int dari`, `[SerializeField, Range(0, 7)] int sampai = 7`, `[SerializeField] GameObject[] sasaran`. Menyalakan objek sasaran hanya jika `MalamKe` di dalam rentang, dan menerapkan ulang setiap ada `Perubahan.Malam` (termasuk saat save dimuat).

Komponen `AktifDiMalam` mengatur objek lain (`sasaran`), bukan dirinya sendiri, supaya komponen ini tetap hidup untuk menerima perubahan malam.

- [ ] **Step 1: Buat `KartuTeks.cs`**

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Layar hitam dengan judul dan teks: ringkasan siang di awal malam, transisi, dan dasar cutscene.
    public class KartuTeks : MonoBehaviour
    {
        const float LamaPudar = 0.8f;

        [SerializeField, TextArea] string judulSaatMulai;
        [SerializeField, TextArea(3, 8)] string isiSaatMulai;
        [SerializeField] float detikSaatMulai = 4f;

        CanvasGroup grup;
        Text judul, isi;

        public bool Tampil { get; private set; }

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("KartuTeks", 40).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", Color.black, Vector2.zero, Vector2.one);
            grup = latar.gameObject.AddComponent<CanvasGroup>();
            grup.alpha = 0f;
            judul = UiPabrik.BuatTeks(latar.transform, "Judul", 54, TextAnchor.MiddleCenter, new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.75f));
            isi = UiPabrik.BuatTeks(latar.transform, "Isi", 30, TextAnchor.UpperCenter, new Vector2(0.15f, 0.2f), new Vector2(0.85f, 0.52f));
        }

        void Start()
        {
            if (!string.IsNullOrEmpty(judulSaatMulai))
                StartCoroutine(Tampilkan(judulSaatMulai, isiSaatMulai, detikSaatMulai));
        }

        public IEnumerator Tampilkan(string teksJudul, string teksIsi, float detik)
        {
            if (Tampil) yield break;
            Tampil = true;
            GameManager.Instance.Jeda(true);
            judul.text = teksJudul;
            isi.text = teksIsi;
            yield return Pudar(0f, 1f);
            for (float t = 0f; t < detik; t += Time.unscaledDeltaTime) yield return null;
            yield return Pudar(1f, 0f);
            GameManager.Instance.Jeda(false);
            Tampil = false;
        }

        public IEnumerator KeHitam()
        {
            judul.text = "";
            isi.text = "";
            yield return Pudar(grup.alpha, 1f);
        }

        public IEnumerator DariHitam()
        {
            yield return Pudar(grup.alpha, 0f);
        }

        IEnumerator Pudar(float dari, float ke)
        {
            for (float t = 0f; t < LamaPudar; t += Time.unscaledDeltaTime)
            {
                grup.alpha = Mathf.Lerp(dari, ke, t / LamaPudar);
                yield return null;
            }
            grup.alpha = ke;
        }
    }
}
```

- [ ] **Step 2: Buat `AktifDiMalam.cs`**

```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Menyalakan objek sasaran hanya pada malam dari..sampai (0 = Prolog). Dipakai untuk perubahan isi scene antarmalam.
    public class AktifDiMalam : MonoBehaviour
    {
        [SerializeField, Range(0, 7)] int dari;
        [SerializeField, Range(0, 7)] int sampai = 7;
        [SerializeField] GameObject[] sasaran = new GameObject[0];

        void OnEnable()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.Berubah += SaatBerubah;
            Terapkan();
        }

        void OnDisable()
        {
            if (GameManager.Instance != null) GameManager.Instance.Berubah -= SaatBerubah;
        }

        void SaatBerubah(Perubahan p)
        {
            if (p == Perubahan.Malam) Terapkan();
        }

        void Terapkan()
        {
            bool aktif = RentangMalam.Berisi(GameManager.Instance.State.MalamKe, dari, sampai);
            foreach (var g in sasaran)
                if (g != null) g.SetActive(aktif);
        }
    }
}
```

- [ ] **Step 3: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: semua lulus (199), tanpa `error CS`.

- [ ] **Step 4: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add text card with fades and per-night object toggling"
```

---

### Task 10: Scene uji Sandbox 0B, data Jurnal awal, daftar cek Play Mode

**Files:**
- Create: `Assets/_Project/Scripts/Editor/TheSending.Editor.asmdef`, `Assets/_Project/Scripts/Editor/PembuatSandbox.cs`
- Generated (oleh skrip, ikut di-commit): `Assets/_Project/Scenes/Scene_Sandbox0B.unity`, `Assets/_Project/Data/Sandbox/DialogContoh.asset`, `Assets/_Project/Data/Jurnal.asset`
- Modify: `CLAUDE.md`

**Interfaces:**
- Consumes: semua komponen Runtime dari Task 4–9 dan nama field `[SerializeField]`-nya: `PemainGerak.kamera`, `PemainInteraksi.kamera`, `SenterHP.cahaya`, `KompasHP.pemain`, `KameraHP.kamera`, `KameraHP.hud`, `Hud.interaksi`, `PanelJurnal.data`, `KartuTeks.judulSaatMulai`, `KartuTeks.isiSaatMulai`, `PemicuDialog.dialog`, `PemicuDialog.panel`, `Interaktif.prompt`, `PetunjukFoto.efek`, `PetunjukFoto.pesan`, `PengisiBaterai.hud`, `AktifDiMalam.dari/sampai/sasaran`.
- Produces: menu **The Sending > Buat Scene Sandbox 0B** dan metode batchmode `TheSending.EditorTools.PembuatSandbox.Buat`. `Jurnal.asset` hanya dibuat jika belum ada, supaya isi yang ditulis tim tidak tertimpa.

Judul entri Jurnal diambil dari Game Story 6.2 (media), 6.3 (bukti dan Katanya), 9.1 (catatan), dan 10.3 (halaman buku harian). Isi lengkapnya ditulis di fase malam terkait.

- [ ] **Step 1: Buat `TheSending.Editor.asmdef`**

```json
{
    "name": "TheSending.Editor",
    "rootNamespace": "TheSending.EditorTools",
    "references": [
        "TheSending.Core",
        "TheSending.Runtime"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": false,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

- [ ] **Step 2: Buat `PembuatSandbox.cs`**

```csharp
using System;
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheSending.EditorTools
{
    // Membuat scene uji Fase 0B.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatSandbox.Buat
    public static class PembuatSandbox
    {
        const string JalurScene = "Assets/_Project/Scenes/Scene_Sandbox0B.unity";
        const string JalurDialog = "Assets/_Project/Data/Sandbox/DialogContoh.asset";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        [MenuItem("The Sending/Buat Scene Sandbox 0B")]
        public static void Buat()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            Directory.CreateDirectory(Path.GetDirectoryName(JalurDialog));
            var dialog = BuatDialogContoh();
            var jurnal = AmbilAtauBuatJurnal();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.08f, 0.09f, 0.11f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.06f;
            RenderSettings.fogColor = new Color(0.10f, 0.12f, 0.14f);

            var bulan = new GameObject("CahayaBulan").AddComponent<Light>();
            bulan.type = LightType.Directional;
            bulan.intensity = 0.15f;
            bulan.color = new Color(0.43f, 0.53f, 0.66f);
            bulan.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var lantai = GameObject.CreatePrimitive(PrimitiveType.Plane);
            lantai.name = "Lantai";
            lantai.transform.localScale = new Vector3(3f, 1f, 3f);
            Kotak("Dinding", new Vector3(0f, 1.5f, 8f), new Vector3(16f, 3f, 0.2f));

            new GameObject("GameManager").AddComponent<GameManager>();

            var pemain = new GameObject("Pemain");
            pemain.transform.position = new Vector3(0f, 0.05f, -4f);
            var cc = pemain.AddComponent<CharacterController>();
            cc.height = 1.7f;
            cc.radius = 0.3f;
            cc.center = new Vector3(0f, 0.85f, 0f);
            var kamera = new GameObject("Kamera");
            kamera.tag = "MainCamera";
            kamera.transform.SetParent(pemain.transform, false);
            kamera.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            kamera.AddComponent<Camera>().nearClipPlane = 0.05f;
            kamera.AddComponent<AudioListener>();
            var senterGo = new GameObject("Senter");
            senterGo.transform.SetParent(kamera.transform, false);
            var senter = senterGo.AddComponent<Light>();
            senter.type = LightType.Spot;
            senter.range = 15f;
            senter.spotAngle = 50f;
            senter.intensity = 3f;
            senter.shadows = LightShadows.Soft;
            senter.enabled = false;

            Set(pemain.AddComponent<PemainGerak>(), "kamera", kamera.transform);
            var interaksi = pemain.AddComponent<PemainInteraksi>();
            Set(interaksi, "kamera", kamera.transform);
            Set(pemain.AddComponent<SenterHP>(), "cahaya", senter);
            Set(pemain.AddComponent<KompasHP>(), "pemain", pemain.transform);

            var ui = new GameObject("UI");
            var hud = ui.AddComponent<Hud>();
            Set(hud, "interaksi", interaksi);
            var kameraHp = pemain.AddComponent<KameraHP>();
            Set(kameraHp, "kamera", kamera.transform);
            Set(kameraHp, "hud", hud);
            var panelDialog = ui.AddComponent<PanelDialog>();
            Set(ui.AddComponent<PanelJurnal>(), "data", jurnal);
            var kartu = ui.AddComponent<KartuTeks>();
            SetTeks(kartu, "judulSaatMulai", "Sandbox Fase 0B");
            SetTeks(kartu, "isiSaatMulai", "WASD jalan, Shift lari, E interaksi, F senter, Q kamera, C kompas, tahan Spasi menahan napas, Tab Jurnal.");

            var meja = Kotak("Meja (dialog)", new Vector3(-2f, 0.4f, 0f), new Vector3(1.2f, 0.8f, 0.6f));
            var pemicu = meja.AddComponent<PemicuDialog>();
            Set(pemicu, "dialog", dialog);
            Set(pemicu, "panel", panelDialog);
            SetTeks(pemicu, "prompt", "Bicara dengan Bu Sumi");

            var foto = Kotak("Foto keluarga (Q)", new Vector3(2f, 1.4f, 7.85f), new Vector3(0.6f, 0.4f, 0.05f));
            var petunjuk = foto.AddComponent<PetunjukFoto>();
            SetTeks(petunjuk, "efek", "katanya:K1");
            SetTeks(petunjuk, "pesan", "Foto keluarga dipotret.");

            var stopKontak = Kotak("Stop kontak (isi baterai)", new Vector3(4f, 0.5f, 7.85f), new Vector3(0.15f, 0.15f, 0.05f));
            var pengisi = stopKontak.AddComponent<PengisiBaterai>();
            Set(pengisi, "hud", hud);
            SetTeks(pengisi, "prompt", "Isi baterai HP");

            var boneka = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boneka.name = "Media M1 (kompas)";
            boneka.transform.position = new Vector3(6f, 0.2f, 4f);
            boneka.transform.localScale = Vector3.one * 0.3f;
            boneka.AddComponent<PenandaMedia>();   // media = 0 = M1

            var khusus = Kotak("Hanya Malam 2-3", new Vector3(-5f, 0.5f, 4f), Vector3.one);
            var pengatur = new GameObject("AktifDiMalam (Malam 2-3)").AddComponent<AktifDiMalam>();
            SetAngka(pengatur, "dari", 2);
            SetAngka(pengatur, "sampai", 3);
            SetSatuObjek(pengatur, "sasaran", khusus);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene sandbox dibuat: {JalurScene}");
        }

        static DataDialog BuatDialogContoh()
        {
            var d = ScriptableObject.CreateInstance<DataDialog>();
            d.Baris = new[]
            {
                new BarisDialog
                {
                    Pembicara = "Bu Sumi", Teks = "Opo maneh? Wis bengi.", Terjemahan = "Apa lagi? Sudah malam.",
                    Pilihan = new[]
                    {
                        new PilihanDialog { Teks = "Bu, saya minta maaf soal yang dulu.", Efek = "kepercayaan:MintaMaaf", LompatKe = 1 },
                        new PilihanDialog { Teks = "Ibu yang bikin ibu saya sakit, kan?", Efek = "tuduh-sumi", LompatKe = 2 },
                    },
                },
                new BarisDialog
                {
                    Pembicara = "Bu Sumi", Teks = "Aku ora tau nyantet sapa-sapa. Aku mung tau nangis.",
                    Terjemahan = "Aku tidak pernah menyantet siapa pun. Aku cuma pernah menangis.",
                    Efek = "bukti:B5", Akhir = true,
                },
                new BarisDialog { Pembicara = "Bu Sumi", Teks = "Lungo!", Terjemahan = "Pergi!", Akhir = true },
            };
            AssetDatabase.DeleteAsset(JalurDialog);   // data contoh selalu dibuat ulang
            AssetDatabase.CreateAsset(d, JalurDialog);
            return d;
        }

        static DataJurnal AmbilAtauBuatJurnal()
        {
            var ada = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal);
            if (ada != null) return ada;   // jangan timpa isi yang sudah ditulis tim

            var j = ScriptableObject.CreateInstance<DataJurnal>();
            j.Media = Entri(
                "Boneka kain beraksara Jawa", "Tujuh paku berkarat", "Bungkusan rambut uban dan kembang busuk",
                "Telur ayam cemani berisi jarum", "Tanah kuburan merah dalam kendi", "Kain kafan bertulis rajah",
                "Kendi induk berisi foto keluarga dan sisir Bu Sumi");
            j.Bukti = Entri(
                "Halaman buku harian ayah tentang tekanan Pakde Darmo",
                "Telur ayam cemani; cemani hanya dipelihara di Watuireng",
                "Sampel tanah merah dan kesaksian Pak Lasno",
                "Kesaksian Laras soal Mbah Wiryo di sumur",
                "Kesaksian Bu Sumi soal sisir yang hilang",
                "Kuitansi uang muka dan catatan pembayaran Pakde untuk \"W\"",
                "Surat permintaan maaf ayah kepada Bu Sumi",
                "Buku catatan pesanan Mbah Wiryo");
            j.Katanya = Entri(
                "Bu Sumi itu dukun", "Bu Sumi pernah mengutuk keluarga Raka",
                "Rambut di sumur milik Bu Sumi", "Pembeli tanah sudah siap");
            j.HalamanDiary = Entri(
                "Tawaran pembeli tanah lewat Darmo, ayah menolak", "Tabel aksara Jawa",
                "Darmo marah dan berkata ayah akan menyesal", "Cara mencabut paku santet dan Ritual Penangkal",
                "Ayah menemukan paku di bantal sebelum sakit",
                "Rencana wasiat di notaris; selesaikan masalah tanah dengan musyawarah",
                "Surat permintaan maaf ayah kepada Bu Sumi",
                "Ayah melihat Wiryo di pekarangan pada malam sebelum ia sakit");
            j.CatatanBapak = Entri(
                "Weton dan hari pasaran", "Tabel aksara Jawa", "Rumah limasan dan saka guru", "Sumur timba dan kerekan",
                "Ayam cemani", "Daun kelor dan bambu kuning dalam kepercayaan Jawa", "Tradisi larung");
            j.CatatanRaka = Entri(
                "Tanda dehidrasi dan pentingnya cairan", "Kapan harus dirujuk ke rumah sakit", "Pasal 252 KUHP baru",
                "Main hakim sendiri adalah tindak pidana", "Cara yang benar menyelesaikan sengketa waris",
                "Cara membedakan fakta dan gosip");
            AssetDatabase.CreateAsset(j, JalurJurnal);
            return j;
        }

        static EntriJurnal[] Entri(params string[] judul) =>
            Array.ConvertAll(judul, t => new EntriJurnal { Judul = t, Isi = "" });

        static GameObject Kotak(string nama, Vector3 posisi, Vector3 ukuran)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = nama;
            g.transform.position = posisi;
            g.transform.localScale = ukuran;
            return g;
        }

        static SerializedProperty Properti(SerializedObject so, string nama) =>
            so.FindProperty(nama) ?? throw new MissingFieldException(so.targetObject.GetType().Name, nama);

        static void Set(Object komponen, string nama, Object nilai)
        {
            var so = new SerializedObject(komponen);
            Properti(so, nama).objectReferenceValue = nilai;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetTeks(Object komponen, string nama, string nilai)
        {
            var so = new SerializedObject(komponen);
            Properti(so, nama).stringValue = nilai;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetAngka(Object komponen, string nama, int nilai)
        {
            var so = new SerializedObject(komponen);
            Properti(so, nama).intValue = nilai;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetSatuObjek(Object komponen, string nama, Object nilai)
        {
            var so = new SerializedObject(komponen);
            var p = Properti(so, nama);
            p.arraySize = 1;
            p.GetArrayElementAtIndex(0).objectReferenceValue = nilai;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
```

- [ ] **Step 3: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: semua lulus (199), tanpa `error CS`.

- [ ] **Step 4: Jalankan pembuat scene lewat batchmode**

Run (editor tertutup, timeout Bash 600000 ms):
```bash
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod TheSending.EditorTools.PembuatSandbox.Buat -logFile "${TMPDIR:-/tmp}/the-sending-sandbox.log"; echo "exit=$?"; grep -E "Scene sandbox dibuat|error CS|Exception" "${TMPDIR:-/tmp}/the-sending-sandbox.log" | head -20
```
Expected: `exit=0` dan baris `Scene sandbox dibuat: Assets/_Project/Scenes/Scene_Sandbox0B.unity`. File `Scene_Sandbox0B.unity`, `DialogContoh.asset`, dan `Jurnal.asset` ada. Jika muncul `MissingFieldException`, nama field di skrip tidak cocok dengan komponen: perbaiki skrip pembuat (bukan komponennya) dan jalankan ulang.

- [ ] **Step 5: Perbarui `CLAUDE.md`**

Ubah baris Fase 0 di tabel "Fase dan Progres" menjadi:
```markdown
| 0. Fondasi & Style Lock | H1–H3 | 16–18 Nov | [~] 0A dan 0B selesai (Core + Runtime, 199 EditMode test lulus). 0C belum |
```

Tambahkan di "Log Progres" (ganti `YYYY-MM-DD` dengan tanggal pengerjaan):
```markdown
- **YYYY-MM-DD:** Fase 0B selesai di kode: GameManager, save/checkpoint ke file, kontrol pemain, interaksi, HUD, senter/kompas/kamera HP, dialog, Jurnal, kartu teks, AktifDiMalam. Scene uji: `Assets/_Project/Scenes/Scene_Sandbox0B.unity` (dibuat ulang lewat menu The Sending > Buat Scene Sandbox 0B). Menunggu cek Play Mode tim.
```

Tambahkan bagian baru tepat sebelum "## Log Progres":
```markdown
## Daftar Cek Play Mode Fase 0B (dikerjakan tim)

Buka `Assets/_Project/Scenes/Scene_Sandbox0B.unity`, tekan Play, lalu cek:

1. Kartu teks "Sandbox Fase 0B" memudar masuk, tampil sekitar 4 detik, lalu memudar keluar. Selama tampil, pemain tidak bisa bergerak dan jam tidak berjalan.
2. W/A/S/D berjalan, Shift berlari (terasa lebih cepat), mouse memutar pandangan, dongak berhenti di sekitar 80°. Kursor terkunci saat bermain.
3. Jam di kanan atas mulai 17.30 dan naik 1 menit gim setiap sekitar 4 detik nyata.
4. F menyalakan/mematikan senter. Baterai turun sekitar 1% setiap 6 detik saat menyala. E di "Stop kontak" mengisi baterai ke 100% dan muncul notifikasi.
5. C membuka kompas di kanan bawah. Mendekati bola "Media M1" membuat jarum berputar makin cepat; menjauh lebih dari 12 m membuat jarum diam.
6. Q ke arah "Foto keluarga" memunculkan kilatan putih dan notifikasi "Foto keluarga dipotret." lalu "Papan Bukti diperbarui.". Q ke arah lain hanya memunculkan kilatan.
7. E di "Meja" membuka dialog Bu Sumi; jam berhenti, terjemahan tampil miring dalam kurung. Tombol 1 lanjut ke kalimat berikutnya dan muncul notifikasi bukti; E menutup dialog.
8. Tab membuka Jurnal (jam berhenti). Tombol 1/2/3 berganti tab. Papan Bukti menampilkan "[KATANYA] K1 ..." atau "[BUKTI] B5 ...". Tab menutup Jurnal.
9. Kotak "Hanya Malam 2-3" tidak terlihat (karena sekarang Prolog, malam 0).
10. Catat hal yang terasa salah (kecepatan, sensitivitas mouse, ukuran teks) untuk disetel di 0C.
```

- [ ] **Step 6: Commit**

```bash
git add Assets/_Project CLAUDE.md
git commit -m "feat: add sandbox scene builder, initial journal data and Play Mode checklist"
```

