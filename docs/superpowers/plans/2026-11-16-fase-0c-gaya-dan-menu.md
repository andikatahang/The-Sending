# Fase 0C: Gaya PSX, Pengaturan, dan Menu — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Menutup Fase 0: tampilan Retro PSX terpasang dan tercatat di `docs/style-guide.md`, jeda benar-benar menghentikan dunia gim, pengaturan (audio, layar penuh, mouse) tersimpan, `Scene_MainMenu` dengan peringatan konten, menu jeda, serta sisa catatan review 0B.

**Architecture:** Aturan yang bisa diuji masuk Core (`Pengaturan`: batas nilai, JSON, desibel) dengan EditMode test. Runtime tetap tipis: `GameManager` (satu-satunya singleton, `DontDestroyOnLoad`, ada di setiap scene dan duplikatnya menghancurkan diri) kini juga memegang `Time.timeScale`, kursor, pengaturan, dan pindah scene. Menu dibangun lewat kode seperti UI 0B, tetapi memakai `Button` + `EventSystem` supaya bisa mouse dan keyboard. Gaya PSX dipasang oleh skrip editor yang bisa dijalankan ulang (`PembuatGayaPsx`), bukan lewat langkah Inspector. Scene dibuat oleh skrip editor lewat batchmode.

**Tech Stack:** Unity 6000.6.4f1, URP 17.6 (Render Graph, `FullScreenPassRendererFeature`), Input System 1.20 (`InputSystemUIInputModule`), uGUI 2.6 (legacy `Text`, `Button`), C# .NET Standard 2.1, Unity Test Framework 1.8.0.

Fase 0 terdiri dari 0A (selesai, Core), 0B (selesai, Runtime), dan **0C (ini)**. Inventaris (tombol I) **tidak** dikerjakan di 0C; pindah ke Fase 1 (keputusan tim).

## KEPUTUSAN TIM (sudah diputuskan, 16 Nov)

**K1. Shader full-screen PSX memakai Shader Graph** (sesuai `CLAUDE.md` dan desain Bagian 3). Pembagian kerja:
- Claude menulis `Assets/_Project/Art/Shaders/PSX_Layar.hlsl` (Task 3): file Custom Function berisi kuantisasi 5 bit dan dither Bayer 4×4.
- Anggota tim membuat `PSX_Layar.shadergraph` (URP Fullscreen Shader Graph) dan material `PSX_Layar.mat` di editor, mengikuti langkah di "Persiapan Tim". Ini **prasyarat yang menghalangi Task 6**.
- `PembuatGayaPsx` hanya memuat material itu dari jalurnya dan gagal dengan pesan jelas jika belum ada.

**K2. Audio Mixer dibuat manual oleh tim di 0C**, pada sesi editor yang sama dengan Shader Graph (Unity tidak punya API publik untuk membuatnya dari skrip). Kode tetap jalan tanpa mixer (volume utama lewat `AudioListener`, peringatan di Console).

**K3. Pengaturan grafis hanya "Layar penuh" (Ya/Tidak).** Layar penuh memakai resolusi asli layar; jendela 1280×720. Pemilih resolusi tidak dibuat; tambahkan di Fase 8 jika tes Windows membutuhkannya.

**K4. SSAO bawaan template URP dimatikan** di renderer (tidak cocok dengan gaya PSX, memberatkan laptop lemah).

## Urutan Eksekusi (wajib)

1. Controller membuat commit `chore:` untuk perubahan ProjectSettings buatan Unity (define SENTIS, `Settings.json` ProBuilder) di awal branch 0C.
2. **Task 1–5** dikerjakan berurutan (editor tertutup).
3. **BERHENTI.** Controller meminta tim mengerjakan "Persiapan Tim" (Shader Graph + material, Audio Mixer, font bila sudah ada), menyimpan, lalu **MENUTUP editor Unity**. Batchmode tidak bisa jalan selama editor membuka proyek. Controller memeriksa: `ls Assets/_Project/Art/Shaders/PSX_Layar.shadergraph Assets/_Project/Art/Shaders/PSX_Layar.mat` dan melanjutkan hanya jika keduanya ada. File buatan tim (graph, material, mixer, font, beserta `.meta`) ikut di-commit di Task 6.
4. **Task 6–8** dikerjakan berurutan.

## Global Constraints

- Unity **6000.6.4f1**, URP 17.6. Target build final **hanya Windows** (Mono). Pengembangan dan tes harian di Mac.
- Core: `Assets/_Project/Scripts/Core`, namespace `TheSending.Core`, assembly `TheSending.Core`. **Tanpa MonoBehaviour.** TDD.
- Runtime: `Assets/_Project/Scripts/Runtime`, namespace `TheSending.Runtime`. Editor: `Assets/_Project/Scripts/Editor`, namespace `TheSending.EditorTools`. Test: `Assets/_Project/Tests/EditMode`, namespace `TheSending.Tests`.
- Nama domain bahasa Indonesia, istilah teknis bahasa Inggris. Hanya satu singleton: `GameManager`. Tidak ada field `static` baru di Runtime kecuali yang membersihkan diri (domain reload mati).
- **Jeda:** `GameManager.Jeda(bool)` tetap penghitung, dan mengatur `Time.timeScale = Dijeda ? 0f : 1f`. UI yang bergerak saat jeda memakai `Time.unscaledDeltaTime`.
- **Event:** `GameManager.StateDiganti` dipicu setiap `GameState` diganti. `Perubahan.Malam` hanya saat `MalamKe` benar-benar berubah.
- Resolusi render: Render Scale **0.4**, Upscaling Filter **Point (Nearest-Neighbor)**, MSAA **mati**, AA kamera **None**.
- Full-screen pass: Shader Graph `Assets/_Project/Art/Shaders/PSX_Layar.shadergraph` + Custom Function `PSX_Layar.hlsl`, material `Assets/_Project/Art/Shaders/PSX_Layar.mat`, properti `_BitWarna` (**5**) dan `_KuatDither` (**1**); kuantisasi **5 bit per kanal** + dither **Bayer 4×4**, `InjectionPoint.AfterRenderingPostProcessing`.
- Post-processing (satu profil bersama `Assets/_Project/Settings/PSX_Volume.asset`): Film Grain intensitas **0.4**, Vignette intensitas **0.35** kehalusan **0.4**, saturasi **-30**, Tonemapping **Neutral**.
- Kabut: Exponential Squared, warna **`#1A1F24`**, kepadatan **0.06**. Ambient flat **`#05070A`**. Latar kamera = warna kabut.
- Tekstur di `Assets/_Project/Art/`: impor pertama otomatis maks **256**, filter **Point**, **tanpa mipmap**.
- Palet: teplok `#FFB46B`, bulan `#6D86A8`, karat/darah `#5A1A12`.
- Font: `VT323-Regular.ttf` (UI) dan `Kalam-Regular.ttf` (buku harian) di `Assets/_Project/Art/Fonts/Resources/`. Jika belum ada, kode memakai `LegacyRuntime.ttf`.
- Audio Mixer: `Assets/_Project/Audio/Resources/MixerUtama.mixer`, grup `Master`, `Music`, `SFX`, `Ambience`, `Dialog`, parameter terekspos `VolMaster`, `VolMusic`, `VolSFX`, `VolAmbience`, `VolDialog`.
- Scene: `Scene_MainMenu` (indeks 0 di Build Settings), `Scene_Sandbox0B` (indeks 1). `GameManager.SceneAwal = "Scene_Sandbox0B"` sampai Fase 1 menggantinya.
- ProjectSettings: controller sudah meng-commit perubahan buatan Unity (define `SENTIS_ANALYTICS_ENABLED`, `Settings.json` ProBuilder) sebagai commit `chore:` di awal branch 0C. Sesudah itu define SENTIS **tidak lagi di-revert**. Setelah setiap run batchmode jalankan `git status --short`; jika Unity mengubah ProjectSettings dengan cara lain (selain `EditorBuildSettings.asset` di Task 7), jangan di-add dan laporkan.
- Jangan `git add -A`. Tambahkan hanya jalur yang disebut di langkah commit.
- Commit: `<type>: <deskripsi>`, diakhiri baris `Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ`. **Jangan pernah menambahkan `Co-Authored-By`.**

## Cara Menjalankan Test dan Skrip Editor

- Editor Unity harus tertutup. `bash tools/run-editmode-tests.sh` (semua) atau `bash tools/run-editmode-tests.sh TheSending.Tests.NamaKelasTests`. Timeout Bash 600000 ms.
- Suite sekarang **208** test. Setelah Task 1 menjadi **223**.
- RED untuk kelas baru berupa compile error. Itu gagal yang diharapkan.
- Kode Runtime dan Editor tidak punya EditMode test. Verifikasinya: seluruh suite lulus (semua assembly terkompilasi), run batchmode skrip editor, dan daftar cek Play Mode tim.
- Skrip editor: `"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod <Metode> -logFile <log>`.

## Struktur File

```
Assets/_Project/Scripts/Core/
  Pengaturan.cs               volume, sensitivitas, balik Y, layar penuh: batas nilai, JSON, desibel (baru)
Assets/_Project/Scripts/Runtime/
  GameManager.cs              + timeScale, StateDiganti, kursor, pengaturan, MuatScene (modify)
  PenyimpanPengaturan.cs      PlayerPrefs + penerapan ke mixer dan layar (baru)
  PemainGerak.cs              kursor pindah ke GameManager, sensitivitas dari pengaturan (modify)
  KontrolPemain.cs            + Shift kanan (modify)
  AktifDiMalam.cs  Hud.cs  PanelJurnal.cs   peringatan + StateDiganti + font Kalam (modify)
  UiPabrik.cs                 font VT323/Kalam, tombol, canvas menu, EventSystem (modify)
  PanelPengaturan.cs          panel pengaturan, dipakai menu utama dan menu jeda (baru)
  MenuJeda.cs                 Esc: Lanjut, Pengaturan, Keluar ke Menu Utama (baru)
  AlatUji.cs                  F5 simpan checkpoint, F9 tertangkap; hanya editor/development build (baru)
  MenuUtama.cs                peringatan konten + menu utama (baru)
Assets/_Project/Scripts/Editor/
  TheSending.Editor.asmdef    + referensi URP (modify)
  PembuatGayaPsx.cs           aset URP, fitur full-screen, Volume, kabut, kamera; memuat material buatan tim (baru)
  PrasetelTeksturPsx.cs       AssetPostprocessor tekstur (baru)
  PembuatSandbox.cs           memakai gaya PSX, menu jeda, alat uji (modify)
  PembuatMenuUtama.cs         Scene_MainMenu + Build Settings + "Bangun Ulang Semua" (baru)
  PembuatBuild.cs             build Windows (Mono) dari Mac (baru)
Assets/_Project/Art/Shaders/PSX_Layar.hlsl             Custom Function: kuantisasi + dither (baru, Task 3)
Assets/_Project/Art/Shaders/PSX_Layar.shadergraph, PSX_Layar.mat   (dibuat TIM di editor sebelum Task 6)
Assets/_Project/Art/Fonts/Resources/.gitkeep           tempat tim menaruh TTF (baru)
Assets/_Project/Audio/Resources/.gitkeep               tempat tim membuat MixerUtama (baru)
Assets/_Project/Settings/PSX_Volume.asset              (dibuat skrip)
Assets/_Project/Scenes/Scene_MainMenu.unity            (dibuat skrip)
Assets/_Project/Tests/EditMode/PengaturanTests.cs      (baru)
docs/style-guide.md                                    (baru)
docs/aset/fase-0.md, CLAUDE.md                         (modify)
```

## Persiapan Tim (satu sesi editor, setelah Task 5 dan SEBELUM Task 6)

Bagian A **menghalangi Task 6**. Bagian B dan C tidak menghalangi (kode toleran), tetapi B dikerjakan di sesi yang sama. Setelah selesai: simpan semua (Cmd+S), lalu **tutup editor Unity** dan beri tahu controller.

### A. Shader Graph dan material PSX (WAJIB, sekitar 15 menit)

File `Assets/_Project/Art/Shaders/PSX_Layar.hlsl` sudah ada (dibuat Task 3). Nama node, port, dan menu di bawah sudah dicocokkan dengan paket Shader Graph dan URP 17.6 di proyek ini.

1. Project window: buka `Assets/_Project/Art/Shaders/`. Klik kanan > **Create > Shader Graph > URP > Fullscreen Shader Graph**. Beri nama **`PSX_Layar`**. Klik ganda untuk membukanya.
2. Jika graph baru sudah berisi node contoh (Saturation, Contrast, properti "Scene ..."), hapus semua node dan semua properti di Blackboard. Sisakan hanya blok **Fragment** dengan **Base Color** dan **Alpha**.
3. Blackboard (panel kiri), tombol **+** > **Float**, dua kali. Pilih tiap properti, lalu di **Graph Inspector > Node Settings** isi:

   | Name | Reference | Mode | Min | Max | Default |
   |---|---|---|---|---|---|
   | `Bit Warna` | `_BitWarna` | Slider | 2 | 8 | 5 |
   | `Kuat Dither` | `_KuatDither` | Slider | 0 | 2 | 1 |

   Kolom **Reference harus persis** (skrip memeriksa `_BitWarna` dan `_KuatDither`).
4. Tekan Spasi di area kosong, buat node **URP Sample Buffer** (Input > Universal). Di node itu set **Source Buffer = BlitSource**. Port masukan **UV** dibiarkan kosong (bawaannya posisi layar).
5. Buat node **Screen Position** (Input > Geometry). Set **Mode = Pixel**.
6. Buat node **Custom Function** (Utility). Pilih node itu, lalu di **Graph Inspector > Node Settings**:
   - **Inputs** (tombol +, urutan dan tipe harus persis): `Warna` = Vector 3, `PosisiPiksel` = Vector 2, `BitWarna` = Float, `KuatDither` = Float.
   - **Outputs**: `Hasil` = Vector 3.
   - **Type** = File. **Name** = `PsxLayar` (tanpa akhiran `_float`). **Source** = seret file `PSX_Layar.hlsl`.
7. Sambungkan:
   - URP Sample Buffer **Output** → Custom Function **Warna**
   - Screen Position **Out** → Custom Function **PosisiPiksel**
   - properti **Bit Warna** (seret dari Blackboard) → **BitWarna**
   - properti **Kuat Dither** → **KuatDither**
   - Custom Function **Hasil** → Fragment **Base Color**
   - **Alpha** dibiarkan 1.
8. **Graph Inspector > Graph Settings**: **Precision** tetap **Single** (file .hlsl hanya menyediakan varian `_float`). Klik **Save Asset**. Tidak boleh ada error merah di node Custom Function.
9. Project window: klik kanan `PSX_Layar.shadergraph` > **Create > Material**. Beri nama **`PSX_Layar`**, sehingga ada `Assets/_Project/Art/Shaders/PSX_Layar.mat`. Di Inspector material harus tampak dua slider: Bit Warna (5) dan Kuat Dither (1).

**Mengapa Screen Position (Mode Pixel), bukan UV × ukuran layar:** mode Pixel memberi koordinat piksel target render yang sedang digambar (`positionCS.xy`), jadi pola dither mengikuti piksel besar hasil Render Scale 0.4. UV × `_ScreenParams` akan memakai ukuran layar penuh dan pola dither menjadi terlalu halus.

### B. Audio Mixer (sesi yang sama, sekitar 5 menit)

1. Project window: buka `Assets/_Project/Audio/Resources/`, klik kanan > Create > Audio > Audio Mixer, beri nama **`MixerUtama`**.
2. Klik ganda `MixerUtama`. Di panel Groups, klik kanan **Master** > Add child group, empat kali. Beri nama `Music`, `SFX`, `Ambience`, `Dialog`.
3. Untuk kelima grup (termasuk Master): klik grupnya, lalu di Inspector klik kanan tulisan **Volume** > **Expose 'Volume (of ...)' to script**.
4. Di jendela Audio Mixer kanan atas, buka **Exposed Parameters**, klik kanan tiap parameter > Rename menjadi: `VolMaster`, `VolMusic`, `VolSFX`, `VolAmbience`, `VolDialog`.
5. Simpan (Cmd+S).

### C. Font (kapan saja)

Unduh dari Google Fonts, lalu taruh dengan nama persis ini:
- `Assets/_Project/Art/Fonts/Resources/VT323-Regular.ttf`
- `Assets/_Project/Art/Fonts/Resources/Kalam-Regular.ttf`

Tidak ada kode yang perlu diubah; font terpakai di Play berikutnya. Catat lisensi OFL di `docs/aset/KREDIT.md`.

---

### Task 1: Aturan Core untuk pengaturan

**Files:**
- Create: `Assets/_Project/Tests/EditMode/PengaturanTests.cs`, `Assets/_Project/Scripts/Core/Pengaturan.cs`

**Interfaces:**
- Consumes: `UnityEngine.JsonUtility` (sudah dipakai `SaveData`).
- Produces: `Pengaturan` (`[Serializable]`, C# biasa):
  - `static readonly string[] NamaGrup` = `{ "Master", "Music", "SFX", "Ambience", "Dialog" }`
  - `const float SensitivitasMin = 0.02f, SensitivitasMaks = 0.3f, SensitivitasAwal = 0.08f, DesibelSenyap = -80f`
  - field `float[] Volume` (5 nilai, 0..1, awal 1), `float Sensitivitas`, `bool BalikY`, `bool LayarPenuh = true`
  - `Pengaturan Rapikan()`, `static float KeDesibel(float linear)`, `string KeJson()`, `static Pengaturan DariJson(string json)`

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/PengaturanTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PengaturanTests
    {
        [Test]
        public void BawaanSesuaiDesain()
        {
            var p = new Pengaturan();
            Assert.AreEqual(new[] { 1f, 1f, 1f, 1f, 1f }, p.Volume);
            Assert.AreEqual(Pengaturan.SensitivitasAwal, p.Sensitivitas);
            Assert.IsFalse(p.BalikY);
            Assert.IsTrue(p.LayarPenuh);
        }

        [Test]
        public void NamaGrupSesuaiMixerStyleGuide()
        {
            Assert.AreEqual(new[] { "Master", "Music", "SFX", "Ambience", "Dialog" }, Pengaturan.NamaGrup);
        }

        [Test]
        public void RapikanMembatasiVolumeKeNolSampaiSatu()
        {
            var p = new Pengaturan { Volume = new[] { 1.5f, -0.2f, 0.4f, 1f, 0f } };
            p.Rapikan();
            Assert.AreEqual(new[] { 1f, 0f, 0.4f, 1f, 0f }, p.Volume);
        }

        [Test]
        public void RapikanMembatasiSensitivitas()
        {
            Assert.AreEqual(Pengaturan.SensitivitasMaks, new Pengaturan { Sensitivitas = 9f }.Rapikan().Sensitivitas);
            Assert.AreEqual(Pengaturan.SensitivitasMin, new Pengaturan { Sensitivitas = 0f }.Rapikan().Sensitivitas);
        }

        [Test]
        public void RapikanMenggantiNaNDenganNilaiBawaan()
        {
            var p = new Pengaturan { Sensitivitas = float.NaN, Volume = new[] { float.NaN, 1f, 1f, 1f, 1f } };
            p.Rapikan();
            Assert.AreEqual(Pengaturan.SensitivitasAwal, p.Sensitivitas);
            Assert.AreEqual(1f, p.Volume[0]);
        }

        [Test]
        public void RapikanMemperbaikiArrayVolumeYangHilangAtauPendek()
        {
            Assert.AreEqual(new[] { 1f, 1f, 1f, 1f, 1f }, new Pengaturan { Volume = null }.Rapikan().Volume);
            Assert.AreEqual(new[] { 0.3f, 0.5f, 1f, 1f, 1f }, new Pengaturan { Volume = new[] { 0.3f, 0.5f } }.Rapikan().Volume);
        }

        [Test]
        public void JsonBolakBalikMenjagaNilai()
        {
            var p = new Pengaturan { Sensitivitas = 0.12f, BalikY = true, LayarPenuh = false };
            p.Volume[1] = 0.3f;
            var q = Pengaturan.DariJson(p.KeJson());
            Assert.AreEqual(0.3f, q.Volume[1], 0.0001f);
            Assert.AreEqual(0.12f, q.Sensitivitas, 0.0001f);
            Assert.IsTrue(q.BalikY);
            Assert.IsFalse(q.LayarPenuh);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("{bukan json")]
        public void JsonKosongAtauRusakMenghasilkanBawaan(string json)
        {
            var p = Pengaturan.DariJson(json);
            Assert.AreEqual(new[] { 1f, 1f, 1f, 1f, 1f }, p.Volume);
            Assert.AreEqual(Pengaturan.SensitivitasAwal, p.Sensitivitas);
            Assert.IsTrue(p.LayarPenuh);
        }

        [Test]
        public void JsonDenganNilaiDiLuarBatasDirapikan()
        {
            var p = Pengaturan.DariJson("{\"Volume\":[2,-1],\"Sensitivitas\":9}");
            Assert.AreEqual(new[] { 1f, 0f, 1f, 1f, 1f }, p.Volume);
            Assert.AreEqual(Pengaturan.SensitivitasMaks, p.Sensitivitas);
        }

        [TestCase(1f, 0f)]
        [TestCase(0.5f, -6.02f)]
        [TestCase(0f, -80f)]
        [TestCase(-1f, -80f)]
        public void KeDesibelMengubahLinearKeDesibel(float linear, float desibel)
        {
            Assert.AreEqual(desibel, Pengaturan.KeDesibel(linear), 0.01f);
        }
    }
}
```

- [ ] **Step 2: Jalankan, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PengaturanTests`
Expected: tidak ada hasil test, log berisi `error CS0246` (tipe `Pengaturan` tidak ditemukan).

- [ ] **Step 3: Tulis implementasi**

`Assets/_Project/Scripts/Core/Pengaturan.cs`:
```csharp
using System;
using UnityEngine;

namespace TheSending.Core
{
    // Pengaturan pemain (bukan bagian save cerita). Runtime menyimpannya sebagai JSON di PlayerPrefs.
    [Serializable]
    public class Pengaturan
    {
        // Urutan = grup Audio Mixer di style guide. Parameter mixer yang diekspos bernama "Vol" + nama grup.
        public static readonly string[] NamaGrup = { "Master", "Music", "SFX", "Ambience", "Dialog" };
        public const float SensitivitasMin = 0.02f, SensitivitasMaks = 0.3f, SensitivitasAwal = 0.08f;
        public const float DesibelSenyap = -80f;

        public float[] Volume = { 1f, 1f, 1f, 1f, 1f };   // 0..1 per grup
        public float Sensitivitas = SensitivitasAwal;
        public bool BalikY;
        public bool LayarPenuh = true;

        // Memaksa semua nilai ke rentang yang sah. Dipanggil setelah memuat dan setelah setiap perubahan.
        public Pengaturan Rapikan()
        {
            var v = new float[NamaGrup.Length];
            for (int i = 0; i < v.Length; i++)
                v[i] = Volume != null && i < Volume.Length ? Batasi(Volume[i], 0f, 1f, 1f) : 1f;
            Volume = v;
            Sensitivitas = Batasi(Sensitivitas, SensitivitasMin, SensitivitasMaks, SensitivitasAwal);
            return this;
        }

        public static float KeDesibel(float linear) =>
            linear <= 0.0001f ? DesibelSenyap : 20f * (float)Math.Log10(linear);

        public string KeJson() => JsonUtility.ToJson(this);

        // PlayerPrefs adalah data luar: kosong atau rusak berarti pengaturan bawaan, bukan error.
        public static Pengaturan DariJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return new Pengaturan();
            try
            {
                return (JsonUtility.FromJson<Pengaturan>(json) ?? new Pengaturan()).Rapikan();
            }
            catch (ArgumentException)
            {
                return new Pengaturan();
            }
        }

        static float Batasi(float nilai, float min, float maks, float bawaan) =>
            float.IsNaN(nilai) ? bawaan : Math.Min(maks, Math.Max(min, nilai));
    }
}
```

- [ ] **Step 4: Jalankan, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PengaturanTests`
Expected: `result="Passed" total="15" passed="15" failed="0"`

- [ ] **Step 5: Jalankan seluruh suite**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="223" passed="223" failed="0"`

- [ ] **Step 6: Commit**

```bash
git add Assets/_Project/Scripts/Core/Pengaturan.cs Assets/_Project/Scripts/Core/Pengaturan.cs.meta Assets/_Project/Tests/EditMode/PengaturanTests.cs Assets/_Project/Tests/EditMode/PengaturanTests.cs.meta
git commit -m "feat: add Pengaturan rules with clamping, JSON and decibel conversion" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 2: Jeda dengan timeScale, StateDiganti, kursor, dan sisa catatan review 0B

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/PenyimpanPengaturan.cs`
- Modify: `Assets/_Project/Scripts/Runtime/GameManager.cs`, `PemainGerak.cs`, `KontrolPemain.cs`, `AktifDiMalam.cs`, `Hud.cs`, `PanelJurnal.cs`, `CLAUDE.md`

**Interfaces:**
- Consumes: `Pengaturan` (Task 1), `GameState`, `JamGim.Maju`, `BerkasSave`, `Checkpoint`, `KondisiIbu.SetelahTertangkap`, `RentangMalam.Berisi`.
- Produces:
  - `GameManager`: `const string SceneMenuUtama = "Scene_MainMenu"`, `const string SceneAwal = "Scene_Sandbox0B"`, `Pengaturan Pengaturan { get; }`, `bool KunciKursor { get; set; }`, `bool PeringatanSudahTampil { get; set; }`, `event Action StateDiganti`, `void SimpanPengaturan()`, `void MuatScene(string nama)`. `Jeda(bool)` kini mengatur `Time.timeScale` dan memberi `LogWarning` saat underflow. Anggota lama (`Instance`, `State`, `Kontrol`, `Dijeda`, `AdaSave`, `Berubah`, `MulaiBaru()`, `Lanjutkan()`, `SimpanCheckpoint()`, `Tertangkap()`) tidak berubah.
  - `PenyimpanPengaturan` (static, tanpa state): `const string JalurMixer = "MixerUtama"`, `bool AdaMixer`, `Pengaturan Muat()`, `void Simpan(Pengaturan p)`, `void Terapkan(Pengaturan p)`.
  - `PemainGerak`: konstanta `SensitivitasMouse` **dihapus** (diganti `GameManager.Pengaturan.Sensitivitas`); tidak lagi menyentuh `Cursor`.

**Skrip yang berjalan saat jeda, sudah diperiksa terhadap `Time.timeScale = 0`:**

| Skrip | Berbasis waktu | Hasil |
|---|---|---|
| `Hud` notifikasi | `Time.unscaledDeltaTime` | Tetap hilang setelah 3 detik saat dialog/Jurnal terbuka. Tidak diubah |
| `KartuTeks` pudar dan tahan | `Time.unscaledDeltaTime` | Tetap jalan (kartu sendiri yang menjeda). Tidak diubah |
| `KameraHP` kilatan | `Time.unscaledDeltaTime` | Tetap memudar. Tidak diubah |
| `PanelDialog`, `PanelJurnal` | Tidak ada; `Update` dan input tetap jalan saat timeScale 0 | Tidak diubah |
| `KompasHP` putaran jarum | `Time.deltaTime` | Jarum berhenti saat jeda. Itu yang diinginkan (dunia berhenti). Tidak diubah |
| `SenterHP`, `PemainGerak`, `PemainInteraksi`, jam di `GameManager` | Sudah dijaga `Dijeda` | Tidak diubah |
| Input System | Update mode bawaan (Dynamic Update; proyek tidak punya aset InputSettings) | Input tetap terbaca saat timeScale 0 |
| `Button` uGUI (menu baru) | Transisi warna memakai waktu unscaled | Aman |

- [ ] **Step 1: Buat `PenyimpanPengaturan.cs`**

```csharp
using TheSending.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace TheSending.Runtime
{
    // Menyimpan Pengaturan ke PlayerPrefs dan menerapkannya ke Audio Mixer dan layar. Tanpa state.
    public static class PenyimpanPengaturan
    {
        const string Kunci = "pengaturan";
        const int LebarJendela = 1280, TinggiJendela = 720;

        // Assets/_Project/Audio/Resources/MixerUtama.mixer, dibuat manual oleh tim (lihat docs/style-guide.md).
        public const string JalurMixer = "MixerUtama";

        public static bool AdaMixer => Resources.Load<AudioMixer>(JalurMixer) != null;

        public static Pengaturan Muat() => Pengaturan.DariJson(PlayerPrefs.GetString(Kunci, ""));

        public static void Simpan(Pengaturan p)
        {
            PlayerPrefs.SetString(Kunci, p.KeJson());
            PlayerPrefs.Save();
        }

        public static void Terapkan(Pengaturan p)
        {
            var mixer = Resources.Load<AudioMixer>(JalurMixer);
            if (mixer == null)
            {
                // shortcut: tanpa mixer hanya volume Master yang berlaku, buat MixerUtama sebelum audio pertama masuk (Fase 1).
                AudioListener.volume = p.Volume[0];
            }
            else
            {
                AudioListener.volume = 1f;
                for (int i = 0; i < Pengaturan.NamaGrup.Length; i++)
                {
                    string nama = "Vol" + Pengaturan.NamaGrup[i];
                    if (!mixer.SetFloat(nama, Pengaturan.KeDesibel(p.Volume[i])))
                        Debug.LogWarning($"Parameter '{nama}' belum diekspos di MixerUtama.", mixer);
                }
            }

            if (Application.isEditor || Screen.fullScreen == p.LayarPenuh) return;
            if (p.LayarPenuh)
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
            else
                Screen.SetResolution(LebarJendela, TinggiJendela, FullScreenMode.Windowed);
        }
    }
}
```

- [ ] **Step 2: Ganti seluruh isi `GameManager.cs`**

```csharp
using System;
using TheSending.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheSending.Runtime
{
    // Satu-satunya singleton. Menyimpan GameState dan Pengaturan, menjalankan jam gim,
    // mengurus save, checkpoint, jeda (Time.timeScale), kursor, dan pindah scene.
    // Setiap scene punya objek GameManager supaya bisa di-Play langsung; duplikat menghancurkan diri.
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public const string SceneMenuUtama = "Scene_MainMenu";
        // Scene yang dimuat "Mulai Baru" dan "Lanjutkan". Fase 1 menggantinya dengan scene Prolog.
        public const string SceneAwal = "Scene_Sandbox0B";

        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; }
        public KontrolPemain Kontrol { get; private set; }
        public Pengaturan Pengaturan { get; private set; }
        public bool Dijeda => jumlahJeda > 0;
        public bool AdaSave => BerkasSave.Ada(Folder);

        // Diisi PemainGerak: true selama ada pemain yang dikendalikan. Scene tanpa pemain (menu, ending) membiarkannya false.
        public bool KunciKursor { get; set; }
        // Peringatan konten cukup tampil sekali per sesi gim.
        public bool PeringatanSudahTampil { get; set; }

        // Diteruskan dari GameState aktif. UI berlangganan di sini, bukan ke GameState,
        // karena GameState diganti setiap kali save dimuat.
        public event Action<Perubahan> Berubah;
        // GameState diganti (Mulai Baru, Lanjutkan, Tertangkap). Pelanggan menggambar ulang dari State baru.
        public event Action StateDiganti;

        int jumlahJeda;
        static string Folder => Application.persistentDataPath;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                enabled = false;
                Destroy(gameObject);   // duplikat dari scene lain; yang pertama tetap hidup
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Kontrol = new KontrolPemain();
            Kontrol.Aktifkan();
            Pengaturan = PenyimpanPengaturan.Muat();
            Pasang(new GameState());
        }

        void Start()
        {
            if (Instance != this) return;
            // AudioMixer.SetFloat belum berlaku di Awake, jadi pengaturan diterapkan di Start.
            PenyimpanPengaturan.Terapkan(Pengaturan);
            if (!PenyimpanPengaturan.AdaMixer)
                Debug.LogWarning("MixerUtama belum ada di Assets/_Project/Audio/Resources/. Hanya volume utama yang berlaku.", this);
        }

        void OnDestroy()
        {
            if (Instance != this) return;
            Kontrol?.Buang();
            Time.timeScale = 1f;                      // domain reload mati: jangan tinggalkan editor dalam keadaan beku
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Instance = null;
        }

        void Update()
        {
            if (Instance != this) return;
            bool kunci = KunciKursor && !Dijeda;
            Cursor.lockState = kunci ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !kunci;
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

        public bool SimpanCheckpoint() => Checkpoint.Simpan(Folder, State);

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
        // Selama ada yang menjeda, Time.timeScale = 0: fisika, NavMesh, Animator, audio, dan WaitForSeconds berhenti.
        public void Jeda(bool aktif)
        {
            if (!aktif && jumlahJeda == 0)
            {
                Debug.LogWarning("Jeda(false) dipanggil tanpa pasangan Jeda(true).", this);
                return;
            }
            jumlahJeda += aktif ? 1 : -1;
            Time.timeScale = Dijeda ? 0f : 1f;
        }

        // Dipanggil panel pengaturan setelah mengubah nilai: rapikan, simpan ke PlayerPrefs, terapkan.
        public void SimpanPengaturan()
        {
            Pengaturan.Rapikan();
            PenyimpanPengaturan.Simpan(Pengaturan);
            PenyimpanPengaturan.Terapkan(Pengaturan);
        }

        // Panel yang sedang menjeda melepas jedanya sendiri di OnDisable saat scene lama dibongkar.
        public void MuatScene(string nama) => SceneManager.LoadScene(nama);

        void Pasang(GameState s)
        {
            int malamLama = State != null ? State.MalamKe : s.MalamKe;
            if (State != null) State.Berubah -= Teruskan;
            State = s;
            State.Berubah += Teruskan;
            StateDiganti?.Invoke();
            if (State.MalamKe != malamLama) Berubah?.Invoke(Perubahan.Malam);
            if (State.Ending != Ending.Belum) Berubah?.Invoke(Perubahan.Ending);
        }

        void Teruskan(Perubahan p) => Berubah?.Invoke(p);
    }
}
```

- [ ] **Step 3: Ganti seluruh isi `PemainGerak.cs`**

```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class PemainGerak : MonoBehaviour
    {
        public const float KecepatanJalan = 2.2f, KecepatanLari = 4.0f;
        public const float BatasDongak = 80f;
        const float Gravitasi = -9.81f;

        [SerializeField] Transform kamera;

        CharacterController cc;
        float dongak, kecepatanJatuh;

        public Gerak GerakSekarang { get; private set; }
        public Napas Napas { get; } = new Napas();
        public Transform Kamera => kamera;

        void Awake()
        {
            cc = GetComponent<CharacterController>();
            if (kamera == null && Camera.main != null) kamera = Camera.main.transform;
            if (kamera == null)
            {
                Debug.LogError($"{GetType().Name} di '{name}' butuh referensi kamera.", this);
                enabled = false;
            }
        }

        // Kursor milik GameManager; pemain hanya memberi tahu bahwa ada yang dikendalikan.
        void OnEnable()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning($"PemainGerak di '{name}' tidak menemukan GameManager; pemain tidak bisa bergerak.", this);
                return;
            }
            GameManager.Instance.KunciKursor = true;
        }

        void OnDisable()
        {
            if (GameManager.Instance != null) GameManager.Instance.KunciKursor = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.Dijeda)
            {
                GerakSekarang = Gerak.Diam;
                return;
            }

            var k = gm.Kontrol;
            var p = gm.Pengaturan;
            Vector2 lihat = k.Lihat.ReadValue<Vector2>() * p.Sensitivitas;
            transform.Rotate(0f, lihat.x, 0f);
            dongak = Mathf.Clamp(dongak + (p.BalikY ? lihat.y : -lihat.y), -BatasDongak, BatasDongak);
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

- [ ] **Step 4: Ikat Shift kanan di `KontrolPemain.cs`**

Di dalam konstruktor `KontrolPemain()`, setelah blok `Gerak.AddCompositeBinding(...)`, tambahkan:
```csharp
            Lari.AddBinding("<Keyboard>/rightShift");
```

- [ ] **Step 5: Ganti seluruh isi `AktifDiMalam.cs`**

```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Menyalakan objek sasaran hanya pada malam dari..sampai (0 = Prolog). Dipakai untuk perubahan isi scene antarmalam.
    // Pasang di objek yang selalu aktif, jangan di sasaran atau anak sasaran, supaya tidak mematikan dirinya sendiri.
    public class AktifDiMalam : MonoBehaviour
    {
        [SerializeField, Range(0, 7)] int dari;
        [SerializeField, Range(0, 7)] int sampai = 7;
        [SerializeField] GameObject[] sasaran = new GameObject[0];

        void OnEnable()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogWarning($"AktifDiMalam di '{name}' tidak menemukan GameManager; sasaran tidak diatur.", this);
                return;
            }
            foreach (var g in sasaran)
                if (g != null && transform.IsChildOf(g.transform))
                    Debug.LogWarning($"AktifDiMalam di '{name}' berada di dalam sasarannya ('{g.name}') dan akan mematikan dirinya sendiri.", this);
            gm.Berubah += SaatBerubah;
            gm.StateDiganti += Terapkan;
            Terapkan();
        }

        void OnDisable()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            gm.Berubah -= SaatBerubah;
            gm.StateDiganti -= Terapkan;
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

- [ ] **Step 6: `Hud.cs` — peringatan dan `StateDiganti`**

Ganti metode `OnEnable` dan `OnDisable` dengan:
```csharp
        void OnEnable()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogWarning("Hud tidak menemukan GameManager; notifikasi perubahan tidak akan muncul.", this);
                return;
            }
            gm.Berubah += SaatBerubah;
            gm.StateDiganti += SaatStateDiganti;
        }

        void OnDisable()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            gm.Berubah -= SaatBerubah;
            gm.StateDiganti -= SaatStateDiganti;
        }

        // Notifikasi milik state lama tidak berlaku lagi setelah save dimuat.
        void SaatStateDiganti()
        {
            antrean.Clear();
            notifikasi.text = "";
            sisaNotifikasi = 0f;
        }
```

- [ ] **Step 7: `PanelJurnal.cs` — peringatan data kosong**

Di akhir `Awake()`, sebelum `panel.SetActive(false);`, tambahkan:
```csharp
            if (data == null)
                Debug.LogWarning("PanelJurnal tanpa DataJurnal: semua entri tampil sebagai '(belum ditulis)'.", this);
```

- [ ] **Step 8: Catat aturan jeda dan static di `CLAUDE.md`**

Di bagian "## Konvensi Kode", tepat sebelum baris `- **Commit tanpa baris ...`, tambahkan:
```markdown
- **Jeda = `Time.timeScale` 0.** `GameManager.Jeda(true/false)` adalah penghitung dan mengatur `Time.timeScale`. Setiap panel yang menjeda wajib melepasnya tepat sekali, termasuk di `OnDisable`. UI yang bergerak saat jeda memakai `Time.unscaledDeltaTime`. Cutscene Timeline yang berjalan saat jeda memakai `PlayableDirector` dengan Update Method **Unscaled Game Time**.
- `GameManager.StateDiganti` berarti GameState diganti (Mulai Baru, Lanjutkan, Tertangkap). `Perubahan.Malam` hanya saat malam benar-benar berganti; konten pembuka malam berlangganan ke sini.
- Domain reload saat Play dimatikan. Field `static` baru di Runtime wajib dibersihkan sendiri (`OnDestroy`, atau `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`).
- Kursor diatur `GameManager` (`KunciKursor`, diisi `PemainGerak`). Skrip lain tidak menyentuh `Cursor`.
```

- [ ] **Step 9: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="223" passed="223" failed="0"`, tanpa `error CS`. Lalu `grep -rn "SensitivitasMouse\|Cursor\." Assets/_Project/Scripts --include="*.cs"` hanya menampilkan baris `Cursor.` di `GameManager.cs`.

- [ ] **Step 10: Commit**

```bash
git add Assets/_Project CLAUDE.md
git commit -m "feat: pause via timeScale, StateDiganti event, cursor owned by GameManager" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 3: Font, tombol menu, dan panel pengaturan

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/PanelPengaturan.cs`, `Assets/_Project/Art/Fonts/Resources/.gitkeep`, `Assets/_Project/Audio/Resources/.gitkeep`, `Assets/_Project/Art/Shaders/PSX_Layar.hlsl`
- Modify: `Assets/_Project/Scripts/Runtime/UiPabrik.cs`, `PanelJurnal.cs`, `docs/aset/fase-0.md`

**Interfaces:**
- Consumes: `GameManager.Pengaturan`, `GameManager.SimpanPengaturan()`, `Pengaturan.NamaGrup` (Task 1–2).
- Produces:
  - `UiPabrik.Font` (VT323 atau bawaan), `UiPabrik.FontTulisan` (Kalam atau bawaan), `Canvas BuatCanvasMenu(string nama, int urutan)`, `Button BuatTombol(Transform induk, string label, Vector2 anchorMin, Vector2 anchorMax, UnityAction saatKlik)`, `void Pilih(Button tombol)` (null = lepas pilihan). Metode lama tidak berubah.
  - `PanelPengaturan` (MonoBehaviour): `bool Terbuka`, `void Buka(Action saatTutup = null)`, `void Tutup()`. Tidak memanggil `Jeda`; pemiliknya (menu jeda) yang menjeda.

**Keputusan navigasi:** panel dalam gim dari 0B (dialog, Jurnal) tetap keyboard saja. Menu (utama, jeda, pengaturan) memakai `Button` uGUI dengan `EventSystem` + `InputSystemUIInputModule`: kursor memang terlihat di menu, dan `Button` memberi klik mouse sekaligus navigasi panah/WASD + Enter tanpa kode tambahan. Modul input memasang aksi UI bawaannya sendiri saat `OnEnable`, jadi tidak ada aset yang disambung manual.

**Keputusan font:** font dicari lewat `Resources.Load<Font>` dengan nama file tanpa ekstensi. Tim cukup menaruh TTF di folder `Resources` (lihat "Persiapan Tim").

- [ ] **Step 1: Siapkan folder dan file untuk sesi editor tim (font, mixer, Custom Function shader)**

```bash
mkdir -p "Assets/_Project/Art/Fonts/Resources" "Assets/_Project/Audio/Resources" "Assets/_Project/Art/Shaders"
touch "Assets/_Project/Art/Fonts/Resources/.gitkeep" "Assets/_Project/Audio/Resources/.gitkeep"
```

`Assets/_Project/Art/Shaders/PSX_Layar.hlsl` (dipakai node Custom Function di `PSX_Layar.shadergraph`, yang dibuat tim; lihat "Persiapan Tim" bagian A):
```hlsl
// Custom Function untuk Shader Graph PSX_Layar (URP Fullscreen): kuantisasi warna per kanal + dither Bayer 4x4.
// Node Custom Function: Type = File, Name = PsxLayar, Inputs: Warna (Vector3), PosisiPiksel (Vector2),
// BitWarna (Float), KuatDither (Float); Output: Hasil (Vector3). Precision graph = Single.
// Tidak bergantung pada pustaka lain, jadi sama di Metal (Mac) dan DirectX (Windows).
#ifndef THESENDING_PSX_LAYAR_INCLUDED
#define THESENDING_PSX_LAYAR_INCLUDED

float3 PsxKeSrgb(float3 c)
{
    float3 rendah = c * 12.92;
    float3 tinggi = 1.055 * pow(max(c, 0.0031308), 1.0 / 2.4) - 0.055;
    return lerp(rendah, tinggi, step(0.0031308, c));
}

float3 PsxKeLinear(float3 c)
{
    float3 rendah = c / 12.92;
    float3 tinggi = pow((max(c, 0.04045) + 0.055) / 1.055, 2.4);
    return lerp(rendah, tinggi, step(0.04045, c));
}

void PsxLayar_float(float3 Warna, float2 PosisiPiksel, float BitWarna, float KuatDither, out float3 Hasil)
{
    static const float Bayer4[16] =
    {
         0.0,  8.0,  2.0, 10.0,
        12.0,  4.0, 14.0,  6.0,
         3.0, 11.0,  1.0,  9.0,
        15.0,  7.0, 13.0,  5.0
    };

    float3 warna = saturate(Warna);
    // Kuantisasi di ruang sRGB supaya warna gelap tidak habis jadi hitam (proyek memakai ruang warna Linear).
    #ifndef UNITY_COLORSPACE_GAMMA
    warna = PsxKeSrgb(warna);
    #endif

    // PosisiPiksel = Screen Position mode Pixel: piksel target render (resolusi rendah), jadi pola ikut piksel besar.
    uint2 piksel = uint2(floor(PosisiPiksel)) & 3;
    float ambang = (Bayer4[piksel.x + piksel.y * 4] + 0.5) / 16.0 - 0.5;
    float tingkat = exp2(BitWarna) - 1.0;
    warna = saturate(floor(warna * tingkat + 0.5 + ambang * KuatDither) / tingkat);

    #ifndef UNITY_COLORSPACE_GAMMA
    warna = PsxKeLinear(warna);
    #endif
    Hasil = warna;
}

#endif
```

- [ ] **Step 2: Ganti seluruh isi `UiPabrik.cs`**

```csharp
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Membangun uGUI lewat kode, supaya tidak ada prefab UI yang harus disambung manual.
    public static class UiPabrik
    {
        static readonly Color WarnaTeks = new Color(0.9f, 0.88f, 0.82f);
        static readonly Color WarnaTombol = new Color(0.10f, 0.09f, 0.08f, 0.92f);
        static readonly Color WarnaTombolSorot = new Color(0.353f, 0.102f, 0.071f, 1f);   // karat #5A1A12
        static readonly Color WarnaTombolTekan = new Color(0.55f, 0.20f, 0.12f, 1f);

        static Font font, fontTulisan;

        // Satu pintu untuk font. VT323 = UI dan subtitle, Kalam = tulisan tangan buku harian.
        // File .ttf ditaruh tim di Assets/_Project/Art/Fonts/Resources/ dengan nama di bawah.
        public static Font Font => font != null ? font : (font = MuatFont("VT323-Regular"));
        public static Font FontTulisan => fontTulisan != null ? fontTulisan : (fontTulisan = MuatFont("Kalam-Regular"));

        static Font MuatFont(string nama)
        {
            var f = Resources.Load<Font>(nama);
            if (f != null) return f;
            Debug.LogWarning($"Font '{nama}.ttf' belum ada di Assets/_Project/Art/Fonts/Resources/; memakai font bawaan.");
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        // Domain reload mati: kosongkan cache supaya font yang baru ditaruh tim terbaca di Play berikutnya.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Bersihkan()
        {
            font = null;
            fontTulisan = null;
        }

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

        // Canvas yang bisa diklik: menu utama, menu jeda, pengaturan.
        public static Canvas BuatCanvasMenu(string nama, int urutan)
        {
            var canvas = BuatCanvas(nama, urutan);
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            if (EventSystem.current == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
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
            t.color = WarnaTeks;
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

        // Tombol menu: bisa diklik mouse dan dipilih dengan panah/WASD + Enter. Warna sorot = karat (palet style guide).
        public static Button BuatTombol(Transform induk, string label, Vector2 anchorMin, Vector2 anchorMax, UnityAction saatKlik)
        {
            var img = BuatPanel(induk, "Tombol " + label, Color.white, anchorMin, anchorMax);
            img.raycastTarget = true;
            var tombol = img.gameObject.AddComponent<Button>();
            tombol.targetGraphic = img;   // AddComponent saat runtime tidak mengisinya sendiri
            var warna = tombol.colors;
            warna.normalColor = WarnaTombol;
            warna.highlightedColor = WarnaTombolSorot;
            warna.selectedColor = WarnaTombolSorot;
            warna.pressedColor = WarnaTombolTekan;
            tombol.colors = warna;
            tombol.onClick.AddListener(saatKlik);
            BuatTeks(img.transform, "Label", 30, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one).text = label;
            return tombol;
        }

        // Memindahkan pilihan keyboard ke tombol; null melepas pilihan.
        public static void Pilih(Button tombol)
        {
            if (EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(tombol != null ? tombol.gameObject : null);
        }
    }
}
```

- [ ] **Step 3: Buat `PanelPengaturan.cs`**

```csharp
using System;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Panel pengaturan untuk menu utama dan menu jeda. Setiap perubahan langsung disimpan dan diterapkan.
    public class PanelPengaturan : MonoBehaviour
    {
        const float LangkahVolume = 0.1f, LangkahSensitivitas = 0.01f;
        const float AtasBaris = 0.86f, TinggiBaris = 0.09f, JarakBaris = 0.015f;
        static readonly string[] LabelGrup = { "Volume utama", "Musik", "Efek suara", "Suasana", "Dialog" };

        GameObject panel;
        Button tombolPertama;
        Action saatTutup, segarkan;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvasMenu("Pengaturan", 60).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.03f, 0.03f, 0.04f, 0.97f), new Vector2(0.2f, 0.08f), new Vector2(0.8f, 0.92f));
            latar.raycastTarget = true;   // menutup klik ke menu di belakangnya
            panel = latar.gameObject;
            var t = latar.transform;
            UiPabrik.BuatTeks(t, "Judul", 44, TextAnchor.MiddleCenter, new Vector2(0f, 0.88f), new Vector2(1f, 0.98f)).text = "PENGATURAN";

            int baris = 0;
            for (int i = 0; i < Pengaturan.NamaGrup.Length; i++)
            {
                int grup = i;
                BarisNilai(t, baris++, LabelGrup[i],
                    p => Mathf.RoundToInt(p.Volume[grup] * 100f) + "%",
                    (p, arah) => p.Volume[grup] += arah * LangkahVolume);
            }
            BarisNilai(t, baris++, "Sensitivitas mouse",
                p => Mathf.RoundToInt(p.Sensitivitas * 100f).ToString(),
                (p, arah) => p.Sensitivitas += arah * LangkahSensitivitas);
            BarisNilai(t, baris++, "Balik sumbu Y", p => p.BalikY ? "Ya" : "Tidak", (p, arah) => p.BalikY = !p.BalikY);
            BarisNilai(t, baris++, "Layar penuh", p => p.LayarPenuh ? "Ya" : "Tidak", (p, arah) => p.LayarPenuh = !p.LayarPenuh);

            UiPabrik.BuatTombol(t, "Kembali", new Vector2(0.35f, 0.03f), new Vector2(0.65f, 0.11f), Tutup);
            panel.SetActive(false);
        }

        // Satu baris: label, tombol kurang, nilai, tombol tambah.
        void BarisNilai(Transform induk, int baris, string label, Func<Pengaturan, string> baca, Action<Pengaturan, int> ubah)
        {
            float atas = AtasBaris - baris * TinggiBaris, bawah = atas - TinggiBaris + JarakBaris;
            UiPabrik.BuatTeks(induk, label, 30, TextAnchor.MiddleLeft, new Vector2(0.06f, bawah), new Vector2(0.54f, atas)).text = label;
            var kurang = UiPabrik.BuatTombol(induk, "<", new Vector2(0.56f, bawah), new Vector2(0.63f, atas), () => Ubah(ubah, -1));
            var nilai = UiPabrik.BuatTeks(induk, label + " (nilai)", 30, TextAnchor.MiddleCenter, new Vector2(0.64f, bawah), new Vector2(0.84f, atas));
            UiPabrik.BuatTombol(induk, ">", new Vector2(0.85f, bawah), new Vector2(0.92f, atas), () => Ubah(ubah, 1));
            if (tombolPertama == null) tombolPertama = kurang;
            segarkan += () => nilai.text = baca(GameManager.Instance.Pengaturan);
        }

        void Ubah(Action<Pengaturan, int> ubah, int arah)
        {
            var gm = GameManager.Instance;
            ubah(gm.Pengaturan, arah);
            gm.SimpanPengaturan();   // merapikan nilai (batas), menyimpan, dan menerapkan
            segarkan();
        }

        public void Buka(Action saatTutup = null)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("PanelPengaturan butuh GameManager di scene.", this);
                return;
            }
            this.saatTutup = saatTutup;
            panel.SetActive(true);
            segarkan();
            UiPabrik.Pilih(tombolPertama);
        }

        public void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            var selesai = saatTutup;
            saatTutup = null;
            selesai?.Invoke();
        }
    }
}
```

- [ ] **Step 4: `PanelJurnal.cs` — tab Buku Harian memakai font tulisan tangan**

Di metode `Gambar(GameState s)`, setelah baris `judul.text = ...;`, tambahkan:
```csharp
            isi.font = tab == Tab.BukuHarian ? UiPabrik.FontTulisan : UiPabrik.Font;
```

- [ ] **Step 5: Perbarui `docs/aset/fase-0.md`**

Ganti dua baris tabel font dengan:
```markdown
| VT323 (UI dan subtitle) | `VT323` | GF | `Assets/_Project/Art/Fonts/Resources/VT323-Regular.ttf` (nama file harus persis) | |
| Kalam (tulisan tangan buku harian) | `Kalam` | GF | `Assets/_Project/Art/Fonts/Resources/Kalam-Regular.ttf` (nama file harus persis) | |
```
Tambahkan satu kalimat di bawah tabel font:
```markdown
Font dimuat lewat `Resources.Load`, jadi wajib berada di folder `Resources` itu. Tidak ada kode yang perlu diubah setelah file ditaruh.
```

- [ ] **Step 6: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="223" passed="223" failed="0"`, tanpa `error CS`.

- [ ] **Step 7: Commit**

```bash
git add Assets/_Project docs/aset/fase-0.md
git commit -m "feat: add font resolver, menu buttons, settings panel and PSX custom function" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 4: Menu jeda dan alat uji save

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/MenuJeda.cs`, `Assets/_Project/Scripts/Runtime/AlatUji.cs`

**Interfaces:**
- Consumes: `GameManager.Jeda`, `Dijeda`, `Kontrol.JedaMenu`, `MuatScene`, `SceneMenuUtama`, `SimpanCheckpoint`, `Tertangkap`; `UiPabrik.BuatCanvasMenu/BuatTombol/Pilih`; `PanelPengaturan.Buka/Tutup/Terbuka`; `Hud.Tampilkan(string)`.
- Produces:
  - `MenuJeda` (MonoBehaviour): field `[SerializeField] PanelPengaturan pengaturan`; `bool Terbuka`. Esc membuka hanya jika gim tidak sedang dijeda oleh panel lain.
  - `AlatUji` (MonoBehaviour): field `[SerializeField] Hud hud`. Aktif hanya jika `Debug.isDebugBuild` (editor dan development build). F5 = simpan checkpoint, F9 = tertangkap.

- [ ] **Step 1: Buat `MenuJeda.cs`**

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Menu jeda di scene gameplay. Esc membuka dan menutup; saat pengaturan terbuka, Esc kembali ke menu jeda.
    public class MenuJeda : MonoBehaviour
    {
        [SerializeField] PanelPengaturan pengaturan;

        GameObject panel;
        CanvasGroup grup;
        Button tombolLanjut;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvasMenu("MenuJeda", 50).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0f, 0f, 0f, 0.8f), Vector2.zero, Vector2.one);
            latar.raycastTarget = true;
            panel = latar.gameObject;
            grup = panel.AddComponent<CanvasGroup>();
            var t = latar.transform;
            UiPabrik.BuatTeks(t, "Judul", 72, TextAnchor.MiddleCenter, new Vector2(0.2f, 0.66f), new Vector2(0.8f, 0.82f)).text = "JEDA";
            tombolLanjut = UiPabrik.BuatTombol(t, "Lanjut", new Vector2(0.38f, 0.52f), new Vector2(0.62f, 0.60f), Tutup);
            UiPabrik.BuatTombol(t, "Pengaturan", new Vector2(0.38f, 0.41f), new Vector2(0.62f, 0.49f), BukaPengaturan);
            UiPabrik.BuatTombol(t, "Keluar ke Menu Utama", new Vector2(0.38f, 0.30f), new Vector2(0.62f, 0.38f), KeMenuUtama);
            panel.SetActive(false);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || !gm.Kontrol.JedaMenu.WasPressedThisFrame()) return;
            if (pengaturan != null && pengaturan.Terbuka) pengaturan.Tutup();
            else if (Terbuka) Tutup();
            else if (!gm.Dijeda) Buka();   // dialog, Jurnal, dan kartu teks tidak ditimpa menu jeda
        }

        // Panel dimatikan atau dihancurkan saat terbuka (misalnya pindah scene): lepaskan jeda.
        void OnDisable() => Tutup();

        void Buka()
        {
            panel.SetActive(true);
            GameManager.Instance.Jeda(true);
            Fokus();
        }

        void Fokus()
        {
            grup.interactable = true;
            UiPabrik.Pilih(tombolLanjut);
        }

        void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            UiPabrik.Pilih(null);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }

        void BukaPengaturan()
        {
            if (pengaturan == null)
            {
                Debug.LogWarning("MenuJeda tanpa PanelPengaturan.", this);
                return;
            }
            grup.interactable = false;   // navigasi keyboard tidak boleh pindah ke tombol di balik pengaturan
            pengaturan.Buka(Fokus);
        }

        // Jeda dilepas oleh OnDisable saat scene ini dibongkar.
        void KeMenuUtama() => GameManager.Instance.MuatScene(GameManager.SceneMenuUtama);
    }
}
```

- [ ] **Step 2: Buat `AlatUji.cs`**

```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Tombol uji untuk daftar cek save (CLAUDE.md). Hanya hidup di editor dan development build.
    public class AlatUji : MonoBehaviour
    {
        [SerializeField] Hud hud;

        Text info;

        void Awake()
        {
            if (!Debug.isDebugBuild)
            {
                enabled = false;
                return;
            }
            var akar = UiPabrik.BuatCanvas("AlatUji", 12).transform;
            akar.SetParent(transform, false);
            info = UiPabrik.BuatTeks(akar, "Info", 22, TextAnchor.UpperLeft, new Vector2(0.02f, 0.92f), new Vector2(0.7f, 0.98f));
        }

        void Update()
        {
            var gm = GameManager.Instance;
            var kb = Keyboard.current;
            if (gm == null || kb == null) return;
            info.text = $"UJI  Malam {gm.State.MalamKe}  Kondisi Ibu {gm.State.KondisiIbu}  [F5] simpan checkpoint  [F9] tertangkap";
            if (gm.Dijeda) return;

            if (kb.f5Key.wasPressedThisFrame)
                Pesan(gm.SimpanCheckpoint() ? "Checkpoint disimpan." : "Checkpoint GAGAL disimpan.");
            if (kb.f9Key.wasPressedThisFrame)
            {
                gm.Tertangkap();
                Pesan("Tertangkap: checkpoint dimuat, Kondisi Ibu -5.");
            }
        }

        void Pesan(string teks)
        {
            if (hud != null) hud.Tampilkan(teks);
            else Debug.Log(teks, this);
        }
    }
}
```

- [ ] **Step 3: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="223" passed="223" failed="0"`, tanpa `error CS`.

- [ ] **Step 4: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add pause menu and save test keys for development builds" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 5: Menu utama dengan peringatan konten

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/MenuUtama.cs`

**Interfaces:**
- Consumes: `GameManager.AdaSave`, `Lanjutkan()`, `MulaiBaru()`, `SimpanCheckpoint()`, `MuatScene()`, `SceneAwal`, `PeringatanSudahTampil`, `Kontrol.JedaMenu`; `UiPabrik`; `PanelPengaturan`.
- Produces: `MenuUtama` (MonoBehaviour): field `[SerializeField] PanelPengaturan pengaturan`; `const string TeksPeringatan`.

**Teks peringatan.** GDD tidak memberi kalimat jadi, hanya topiknya: "peringatan konten tentang darah, paku, kekerasan massa, dan kematian" (GDD bagian alur membuka gim dan bagian etika) dan "Seluruh nama desa dan tokoh adalah fiksi, dan ritual dalam gim tidak disajikan sebagai panduan nyata". Kalimat di `TeksPeringatan` disusun dari dua sumber itu; tim boleh mengganti susunannya.

**Alur.** Peringatan konten (sekali per sesi) → menu. "Lanjutkan" hanya tampil jika ada save. "Mulai Baru" saat ada save meminta konfirmasi, lalu langsung menulis checkpoint baru supaya save lama benar-benar tertimpa. Keduanya memuat `GameManager.SceneAwal`.

- [ ] **Step 1: Buat `MenuUtama.cs`**

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Scene_MainMenu: peringatan konten dulu, lalu Lanjutkan / Mulai Baru / Pengaturan / Keluar.
    public class MenuUtama : MonoBehaviour
    {
        // Topik dari GDD (darah, paku, kekerasan massa, kematian; nama fiksi; ritual bukan panduan). Susunan kalimat oleh pengembang.
        public const string TeksPeringatan =
            "Gim ini memuat darah, paku, kekerasan massa, dan kematian.\n\n" +
            "Seluruh nama desa dan tokoh adalah fiksi.\nRitual dalam gim tidak disajikan sebagai panduan nyata.";

        [SerializeField] PanelPengaturan pengaturan;

        GameObject layarPeringatan, layarKonfirmasi;
        CanvasGroup grupMenu;
        Button tombolMengerti, tombolLanjutkan, tombolMulaiBaru, tombolBatal;
        Text pesan;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvasMenu("MenuUtama", 50).transform;
            akar.SetParent(transform, false);

            var menu = UiPabrik.BuatPanel(akar, "Menu", Color.clear, Vector2.zero, Vector2.one);
            grupMenu = menu.gameObject.AddComponent<CanvasGroup>();
            var m = menu.transform;
            UiPabrik.BuatTeks(m, "Judul", 120, TextAnchor.MiddleCenter, new Vector2(0.1f, 0.62f), new Vector2(0.9f, 0.88f)).text = "THE SENDING";
            tombolLanjutkan = UiPabrik.BuatTombol(m, "Lanjutkan", new Vector2(0.38f, 0.46f), new Vector2(0.62f, 0.54f), Lanjutkan);
            tombolMulaiBaru = UiPabrik.BuatTombol(m, "Mulai Baru", new Vector2(0.38f, 0.36f), new Vector2(0.62f, 0.44f), MulaiBaru);
            UiPabrik.BuatTombol(m, "Pengaturan", new Vector2(0.38f, 0.26f), new Vector2(0.62f, 0.34f), BukaPengaturan);
            UiPabrik.BuatTombol(m, "Keluar", new Vector2(0.38f, 0.16f), new Vector2(0.62f, 0.24f), Keluar);
            pesan = UiPabrik.BuatTeks(m, "Pesan", 26, TextAnchor.MiddleCenter, new Vector2(0.1f, 0.05f), new Vector2(0.9f, 0.12f));

            layarKonfirmasi = BuatLayar(akar, "Konfirmasi", "Save lama akan ditimpa.\nMulai permainan baru?");
            UiPabrik.BuatTombol(layarKonfirmasi.transform, "Ya, mulai baru", new Vector2(0.28f, 0.30f), new Vector2(0.48f, 0.38f), Mulai);
            tombolBatal = UiPabrik.BuatTombol(layarKonfirmasi.transform, "Batal", new Vector2(0.52f, 0.30f), new Vector2(0.72f, 0.38f), TutupKonfirmasi);

            layarPeringatan = BuatLayar(akar, "Peringatan", "PERINGATAN KONTEN\n\n" + TeksPeringatan);
            tombolMengerti = UiPabrik.BuatTombol(layarPeringatan.transform, "Lanjut", new Vector2(0.40f, 0.20f), new Vector2(0.60f, 0.28f), TutupPeringatan);
        }

        // Layar penuh hitam di atas menu; menutup klik ke tombol di belakangnya.
        static GameObject BuatLayar(Transform akar, string nama, string teks)
        {
            var latar = UiPabrik.BuatPanel(akar, nama, Color.black, Vector2.zero, Vector2.one);
            latar.raycastTarget = true;
            UiPabrik.BuatTeks(latar.transform, "Teks", 36, TextAnchor.MiddleCenter, new Vector2(0.15f, 0.42f), new Vector2(0.85f, 0.85f)).text = teks;
            latar.gameObject.SetActive(false);
            return latar.gameObject;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("MenuUtama butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            tombolLanjutkan.gameObject.SetActive(gm.AdaSave);
            if (gm.PeringatanSudahTampil)
            {
                FokusMenu();
                return;
            }
            grupMenu.interactable = false;
            layarPeringatan.SetActive(true);
            UiPabrik.Pilih(tombolMengerti);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || !gm.Kontrol.JedaMenu.WasPressedThisFrame()) return;
            if (pengaturan != null && pengaturan.Terbuka) pengaturan.Tutup();
            else if (layarKonfirmasi.activeSelf) TutupKonfirmasi();
        }

        void FokusMenu()
        {
            grupMenu.interactable = true;
            UiPabrik.Pilih(tombolLanjutkan.gameObject.activeSelf ? tombolLanjutkan : tombolMulaiBaru);
        }

        void TutupPeringatan()
        {
            GameManager.Instance.PeringatanSudahTampil = true;
            layarPeringatan.SetActive(false);
            FokusMenu();
        }

        void Lanjutkan()
        {
            var gm = GameManager.Instance;
            if (gm.Lanjutkan()) gm.MuatScene(GameManager.SceneAwal);
            else pesan.text = "Save tidak bisa dibaca. Pilih Mulai Baru.";
        }

        void MulaiBaru()
        {
            if (!GameManager.Instance.AdaSave)
            {
                Mulai();
                return;
            }
            grupMenu.interactable = false;
            layarKonfirmasi.SetActive(true);
            UiPabrik.Pilih(tombolBatal);
        }

        void TutupKonfirmasi()
        {
            layarKonfirmasi.SetActive(false);
            FokusMenu();
        }

        void Mulai()
        {
            var gm = GameManager.Instance;
            gm.MulaiBaru();
            gm.SimpanCheckpoint();   // langsung menimpa save lama, supaya Lanjutkan tidak memuat permainan sebelumnya
            gm.MuatScene(GameManager.SceneAwal);
        }

        void BukaPengaturan()
        {
            if (pengaturan == null)
            {
                Debug.LogWarning("MenuUtama tanpa PanelPengaturan.", this);
                return;
            }
            grupMenu.interactable = false;
            pengaturan.Buka(FokusMenu);
        }

        void Keluar()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
```

- [ ] **Step 2: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="223" passed="223" failed="0"`, tanpa `error CS`.

- [ ] **Step 3: Commit**

```bash
git add Assets/_Project
git commit -m "feat: add main menu with content warning, continue and new game" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 6: Gaya PSX — aset URP, fitur full-screen, Volume, dan prasetel tekstur

**Files:**
- **Prasyarat (dibuat tim, lihat "Persiapan Tim" A dan "Urutan Eksekusi"):** `Assets/_Project/Art/Shaders/PSX_Layar.shadergraph`, `Assets/_Project/Art/Shaders/PSX_Layar.mat`. Editor Unity harus sudah ditutup.
- Create: `Assets/_Project/Scripts/Editor/PembuatGayaPsx.cs`, `Assets/_Project/Scripts/Editor/PrasetelTeksturPsx.cs`
- Modify: `Assets/_Project/Scripts/Editor/TheSending.Editor.asmdef`
- Generated (oleh skrip, ikut di-commit): `Assets/_Project/Settings/PSX_Volume.asset`; perubahan di `Assets/Settings/PC_RPAsset.asset`, `Mobile_RPAsset.asset`, `PC_Renderer.asset`, `Mobile_Renderer.asset`

**Interfaces:**
- Consumes: material `Assets/_Project/Art/Shaders/PSX_Layar.mat` buatan tim (properti `_BitWarna`, `_KuatDither`); URP 17.6 `FullScreenPassRendererFeature` (`injectionPoint`, `fetchColorBuffer`, `requirements`, `passMaterial`, `passIndex`), `ScriptableRendererData.rendererFeatures`, `VolumeProfile.Add<T>/TryGet<T>`, `VolumeParameter<T>.Override`, `CameraExtensions.GetUniversalAdditionalCameraData`.
- Produces:
  - `PembuatGayaPsx` (static, Editor): menu **The Sending > Pasang Gaya PSX**, metode batchmode `TheSending.EditorTools.PembuatGayaPsx.Pasang`; `static void TerapkanKeScene()` (kabut + ambient ke scene yang sedang dibuka); `static void SiapkanKamera(Camera kamera)` (latar warna kabut, post-processing aktif, AA None); konstanta `SkalaRender`, `KepadatanKabut`, `WarnaKabut`, `WarnaAmbient`.
  - `PrasetelTeksturPsx` (`AssetPostprocessor`): hanya untuk `Assets/_Project/Art/`, hanya pada impor pertama.

**Cara kerja "satu Volume profile bersama".** `PSX_Volume.asset` dipasang sebagai **Volume Profile bawaan di aset URP** (`m_VolumeProfile`), menggantikan `SampleSceneProfile` milik template. Semua scene otomatis memakainya tanpa objek Volume. Scene epilog siang/pagi nanti menambah Volume lokal untuk menimpa.

Aset URP diubah lewat `SerializedObject` (nama field), bukan properti C#, karena `upscalingFilter` ditandai usang di URP 17 dan penggantinya (`m_SelectedUpscalerName`) hanya ada jika kerangka upscaler baru aktif. Skrip mengisi keduanya bila ada.

- [ ] **Step 1: Tambah referensi URP di `TheSending.Editor.asmdef`**

Ganti blok `"references"` menjadi:
```json
    "references": [
        "TheSending.Core",
        "TheSending.Runtime",
        "Unity.RenderPipelines.Core.Runtime",
        "Unity.RenderPipelines.Universal.Runtime"
    ],
```

- [ ] **Step 2: Pastikan prasyarat dari tim ada**

```bash
ls Assets/_Project/Art/Shaders/PSX_Layar.hlsl Assets/_Project/Art/Shaders/PSX_Layar.shadergraph Assets/_Project/Art/Shaders/PSX_Layar.mat
grep -c "PsxLayar\|_BitWarna\|_KuatDither" Assets/_Project/Art/Shaders/PSX_Layar.shadergraph
```
Expected: ketiga file ada, dan hitungan grep lebih dari 0. Jika graph atau material belum ada: **berhenti** dan laporkan ke controller (jangan membuat penggantinya sendiri).

- [ ] **Step 3: Buat `PembuatGayaPsx.cs`**

```csharp
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TheSending.EditorTools
{
    // Memasang gaya PSX (docs/style-guide.md) ke aset URP proyek. Aman dijalankan berulang.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatGayaPsx.Pasang
    public static class PembuatGayaPsx
    {
        // Material dari PSX_Layar.shadergraph. Dibuat tim di editor (rencana 0C, "Persiapan Tim" bagian A).
        const string JalurMaterial = "Assets/_Project/Art/Shaders/PSX_Layar.mat";
        const string JalurVolume = "Assets/_Project/Settings/PSX_Volume.asset";
        const string NamaFitur = "PSX_Layar";
        const string NamaFiturSsao = "ScreenSpaceAmbientOcclusion";
        static readonly string[] JalurUrp = { "Assets/Settings/PC_RPAsset.asset", "Assets/Settings/Mobile_RPAsset.asset" };
        static readonly string[] JalurRenderer = { "Assets/Settings/PC_Renderer.asset", "Assets/Settings/Mobile_Renderer.asset" };

        // Nilai style guide. Ubah di sini, jalankan ulang menu, lalu perbarui docs/style-guide.md.
        public const float SkalaRender = 0.4f;
        public const float BitWarna = 5f, KuatDither = 1f;
        public const float IntensitasGrain = 0.4f, IntensitasVignette = 0.35f, KehalusanVignette = 0.4f, Saturasi = -30f;
        public const float KepadatanKabut = 0.06f;
        public static readonly Color WarnaKabut = new Color32(0x1A, 0x1F, 0x24, 0xFF);
        public static readonly Color WarnaAmbient = new Color32(0x05, 0x07, 0x0A, 0xFF);

        const int MsaaMati = 1;               // MsaaQuality.Disabled
        const int UpscalingPoint = 2;         // UpscalingFilterSelection.Point
        const string UpscalerPoint = "Nearest-Neighbor";

        [MenuItem("The Sending/Pasang Gaya PSX")]
        public static void Pasang()
        {
            var material = MuatMaterial();   // gagal lebih dulu, sebelum ada aset yang diubah
            Directory.CreateDirectory(Path.GetDirectoryName(JalurVolume));
            AssetDatabase.Refresh();
            var volume = AmbilAtauBuatVolume();
            foreach (var jalur in JalurUrp) AturUrp(jalur, volume);
            foreach (var jalur in JalurRenderer) AturRenderer(jalur, material);
            AssetDatabase.SaveAssets();
            Debug.Log("Gaya PSX terpasang.");
        }

        // Kabut dan ambient untuk scene yang sedang dibuka. Dipanggil pembuat scene.
        public static void TerapkanKeScene()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = WarnaAmbient;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = KepadatanKabut;
            RenderSettings.fogColor = WarnaKabut;
        }

        // Latar = warna kabut (langit tidak ikut berkabut), post-processing aktif, tanpa anti-aliasing.
        public static void SiapkanKamera(Camera kamera)
        {
            kamera.clearFlags = CameraClearFlags.SolidColor;
            kamera.backgroundColor = WarnaKabut;
            var data = kamera.GetUniversalAdditionalCameraData();
            data.renderPostProcessing = true;
            data.antialiasing = AntialiasingMode.None;
        }

        static Material MuatMaterial()
        {
            var m = AssetDatabase.LoadAssetAtPath<Material>(JalurMaterial);
            if (m == null)
                throw new FileNotFoundException(
                    "Material PSX belum ada. Tim harus membuat PSX_Layar.shadergraph dan PSX_Layar.mat di editor " +
                    "(docs/superpowers/plans/2026-11-16-fase-0c-gaya-dan-menu.md, Persiapan Tim bagian A).", JalurMaterial);
            if (ShaderUtil.ShaderHasError(m.shader))
                throw new InvalidOperationException($"Shader '{m.shader.name}' pada {JalurMaterial} punya error; buka graph-nya di editor.");
            if (!m.HasProperty("_BitWarna") || !m.HasProperty("_KuatDither"))
                throw new InvalidOperationException(
                    $"{JalurMaterial} tidak punya properti _BitWarna/_KuatDither. Periksa kolom Reference kedua properti di Blackboard graph.");
            m.SetFloat("_BitWarna", BitWarna);
            m.SetFloat("_KuatDither", KuatDither);
            EditorUtility.SetDirty(m);
            return m;
        }

        static VolumeProfile AmbilAtauBuatVolume()
        {
            var profil = AssetDatabase.LoadAssetAtPath<VolumeProfile>(JalurVolume);
            if (profil == null)
            {
                profil = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profil, JalurVolume);
            }
            Komponen<FilmGrain>(profil).intensity.Override(IntensitasGrain);
            var vignette = Komponen<Vignette>(profil);
            vignette.intensity.Override(IntensitasVignette);
            vignette.smoothness.Override(KehalusanVignette);
            Komponen<ColorAdjustments>(profil).saturation.Override(Saturasi);
            Komponen<Tonemapping>(profil).mode.Override(TonemappingMode.Neutral);
            EditorUtility.SetDirty(profil);
            return profil;
        }

        static T Komponen<T>(VolumeProfile profil) where T : VolumeComponent
        {
            if (!profil.TryGet<T>(out var komponen))
            {
                komponen = profil.Add<T>();
                AssetDatabase.AddObjectToAsset(komponen, profil);
            }
            EditorUtility.SetDirty(komponen);
            return komponen;
        }

        static void AturUrp(string jalur, VolumeProfile volume)
        {
            var urp = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(jalur);
            if (urp == null) throw new FileNotFoundException("Aset URP tidak ditemukan.", jalur);
            var so = new SerializedObject(urp);
            Properti(so, "m_RenderScale").floatValue = SkalaRender;
            Properti(so, "m_MSAA").intValue = MsaaMati;
            Properti(so, "m_UpscalingFilter").intValue = UpscalingPoint;
            var upscaler = so.FindProperty("m_SelectedUpscalerName");   // hanya ada jika kerangka upscaler baru aktif
            if (upscaler != null) upscaler.stringValue = UpscalerPoint;
            Properti(so, "m_VolumeProfile").objectReferenceValue = volume;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void AturRenderer(string jalur, Material material)
        {
            var data = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(jalur);
            if (data == null) throw new FileNotFoundException("Renderer URP tidak ditemukan.", jalur);

            var fitur = data.rendererFeatures.Find(f => f != null && f.name == NamaFitur) as FullScreenPassRendererFeature;
            if (fitur == null)
            {
                // Sama dengan yang dilakukan Inspector URP saat "Add Renderer Feature": sub-aset + daftar + peta id lokal.
                fitur = ScriptableObject.CreateInstance<FullScreenPassRendererFeature>();
                fitur.name = NamaFitur;
                AssetDatabase.AddObjectToAsset(fitur, data);
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(fitur, out _, out long idLokal);
                var so = new SerializedObject(data);
                var daftar = Properti(so, "m_RendererFeatures");
                daftar.arraySize++;
                daftar.GetArrayElementAtIndex(daftar.arraySize - 1).objectReferenceValue = fitur;
                var peta = Properti(so, "m_RendererFeatureMap");
                peta.arraySize++;
                peta.GetArrayElementAtIndex(peta.arraySize - 1).longValue = idLokal;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            fitur.injectionPoint = FullScreenPassRendererFeature.InjectionPoint.AfterRenderingPostProcessing;
            fitur.fetchColorBuffer = true;
            fitur.requirements = ScriptableRenderPassInput.None;
            fitur.passMaterial = material;
            fitur.passIndex = 0;
            EditorUtility.SetDirty(fitur);

            // SSAO bawaan template tidak cocok dengan gaya PSX dan memberatkan laptop lemah.
            foreach (var f in data.rendererFeatures)
            {
                if (f == null || f.name != NamaFiturSsao) continue;
                f.SetActive(false);
                EditorUtility.SetDirty(f);
            }
            EditorUtility.SetDirty(data);
        }

        static SerializedProperty Properti(SerializedObject so, string nama) =>
            so.FindProperty(nama) ?? throw new MissingFieldException(so.targetObject.GetType().Name, nama);
    }
}
```

- [ ] **Step 4: Buat `PrasetelTeksturPsx.cs`**

```csharp
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Prasetel impor tekstur gaya PSX untuk semua tekstur di Assets/_Project/Art.
    // Hanya berlaku pada impor pertama (belum ada .meta), supaya tim masih bisa menyetel satu tekstur lewat Inspector.
    public class PrasetelTeksturPsx : AssetPostprocessor
    {
        const string Folder = "Assets/_Project/Art/";
        const int UkuranMaks = 256;

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(Folder)) return;
            var importer = (TextureImporter)assetImporter;
            if (!importer.importSettingsMissing) return;
            importer.maxTextureSize = UkuranMaks;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
        }
    }
}
```

- [ ] **Step 5: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="223" passed="223" failed="0"`, tanpa `error CS`.

- [ ] **Step 6: Pasang gaya PSX lewat batchmode, dua kali**

Run (editor tertutup, timeout Bash 600000 ms), jalankan perintah ini **dua kali** berturut-turut:
```bash
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod TheSending.EditorTools.PembuatGayaPsx.Pasang -logFile "${TMPDIR:-/tmp}/the-sending-psx.log"; echo "exit=$?"; grep -E "Gaya PSX terpasang|Shader error|error CS|Exception" "${TMPDIR:-/tmp}/the-sending-psx.log" | head -20
```
Expected setiap kali: `exit=0` dan satu baris `Gaya PSX terpasang.`, tanpa `Shader error` atau `Exception`. `FileNotFoundException: Material PSX belum ada` atau `InvalidOperationException` tentang properti berarti prasyarat tim belum benar: berhenti dan laporkan pesan itu ke controller.

Lalu periksa hasilnya:
```bash
grep -n "m_RenderScale\|m_UpscalingFilter\|m_SelectedUpscalerName\|m_MSAA\|m_VolumeProfile" Assets/Settings/PC_RPAsset.asset
grep -c "m_Name: PSX_Layar" Assets/Settings/PC_Renderer.asset Assets/Settings/Mobile_Renderer.asset
grep -n "m_Name: FilmGrain\|m_Name: Vignette\|m_Name: ColorAdjustments\|m_Name: Tonemapping" Assets/_Project/Settings/PSX_Volume.asset
```
Expected: `m_RenderScale: 0.4`, `m_UpscalingFilter: 2`, `m_MSAA: 1`, `m_VolumeProfile` berisi GUID `PSX_Volume.asset` (bukan `10fc4df2da32a41aaa32d77bc913491c`); jika baris `m_SelectedUpscalerName` ada, isinya `Nearest-Neighbor`. Jumlah `PSX_Layar` tepat **1** di tiap renderer (bukti aman dijalankan ulang). Empat komponen Volume ada.

Jika muncul `MissingFieldException`, nama field aset URP berbeda di versi ini: buka aset `.asset` terkait, cari nama field yang benar, perbaiki skrip, dan sebutkan di laporan. Jika muncul `Shader error` yang menyebut `PSX_Layar.hlsl`, perbaiki file .hlsl itu sesuai pesan; jika menyebut graph-nya, laporkan ke controller (graph diperbaiki tim di editor).

- [ ] **Step 7: Commit (termasuk graph, material, mixer, dan font buatan tim)**

```bash
git status --short   # ProjectSettings tidak boleh berubah; jika berubah, jangan di-add dan laporkan
git add Assets/_Project Assets/Settings
git commit -m "feat: add PSX look installer with Shader Graph full-screen pass, shared volume and texture preset" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 7: Pembuat scene — sandbox bergaya PSX, Scene_MainMenu, dan Build Settings

**Files:**
- Create: `Assets/_Project/Scripts/Editor/PembuatMenuUtama.cs`
- Modify: `Assets/_Project/Scripts/Editor/PembuatSandbox.cs`
- Generated (oleh skrip, ikut di-commit): `Assets/_Project/Scenes/Scene_MainMenu.unity`, `Assets/_Project/Scenes/Scene_Sandbox0B.unity`, `Assets/_Project/Data/Sandbox/DialogContoh.asset`, `ProjectSettings/EditorBuildSettings.asset`

**Interfaces:**
- Consumes: `PembuatGayaPsx.Pasang/TerapkanKeScene/SiapkanKamera` (Task 6); field `[SerializeField]`: `MenuJeda.pengaturan`, `AlatUji.hud`, `MenuUtama.pengaturan`; `GameManager.SceneMenuUtama`.
- Produces:
  - `PembuatSandbox`: `internal const string JalurScene`, `internal static void Set(Object komponen, string nama, Object nilai)` (sebelumnya private). Scene sandbox kini berisi `PanelPengaturan`, `MenuJeda`, `AlatUji`.
  - `PembuatMenuUtama`: menu **The Sending > Buat Scene Menu Utama** (`Buat`), **The Sending > Bangun Ulang Semua (gaya PSX + scene)** (`BangunSemua`, juga metode batchmode), `static void DaftarkanScene()` (Build Settings: MainMenu indeks 0, Sandbox indeks 1).

**Jurnal:** jumlah entri sudah dicocokkan dengan Game Story 9.1: Catatan Bapak **7** (weton dan pasaran; aksara Jawa; limasan dan saka guru; sumur timba; ayam cemani; kelor dan bambu kuning; larung) dan Catatan Raka **6**. `Jurnal.asset` dan `PembuatSandbox.AmbilAtauBuatJurnal` sudah benar; tidak ada yang diubah.

- [ ] **Step 1: Ubah `PembuatSandbox.cs`**

a. Ganti komentar kepala dan konstanta scene:
```csharp
    // Membuat scene uji Fase 0 (0B + 0C).
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatSandbox.Buat
    public static class PembuatSandbox
    {
        internal const string JalurScene = "Assets/_Project/Scenes/Scene_Sandbox0B.unity";
```

b. Ganti enam baris `RenderSettings.... = ...;` (dari `RenderSettings.ambientMode` sampai `RenderSettings.fogColor`) dengan:
```csharp
            PembuatGayaPsx.TerapkanKeScene();
```

c. Ganti baris `kamera.AddComponent<Camera>().nearClipPlane = 0.05f;` dengan:
```csharp
            var cam = kamera.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            PembuatGayaPsx.SiapkanKamera(cam);
```

d. Ganti dua baris `SetTeks(kartu, ...)` dengan:
```csharp
            SetTeks(kartu, "judulSaatMulai", "Sandbox Fase 0");
            SetTeks(kartu, "isiSaatMulai", "WASD jalan, Shift lari, E interaksi, F senter, Q kamera, C kompas, tahan Spasi menahan napas, Tab Jurnal, Esc jeda. Uji: F5 simpan checkpoint, F9 tertangkap.");
            var panelPengaturan = ui.AddComponent<PanelPengaturan>();
            Set(ui.AddComponent<MenuJeda>(), "pengaturan", panelPengaturan);
            Set(ui.AddComponent<AlatUji>(), "hud", hud);
```

e. Ubah tanda tangan `static void Set(Object komponen, string nama, Object nilai)` menjadi:
```csharp
        internal static void Set(Object komponen, string nama, Object nilai)
```

- [ ] **Step 2: Buat `PembuatMenuUtama.cs`**

```csharp
using System.IO;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Membuat Scene_MainMenu dan mendaftarkan scene ke Build Settings.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatMenuUtama.BangunSemua
    public static class PembuatMenuUtama
    {
        const string JalurScene = "Assets/_Project/Scenes/" + GameManager.SceneMenuUtama + ".unity";

        [MenuItem("The Sending/Bangun Ulang Semua (gaya PSX + scene)")]
        public static void BangunSemua()
        {
            PembuatGayaPsx.Pasang();
            PembuatSandbox.Buat();
            Buat();
        }

        [MenuItem("The Sending/Buat Scene Menu Utama")]
        public static void Buat()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();

            new GameObject("GameManager").AddComponent<GameManager>();

            var kamera = new GameObject("Kamera");
            kamera.tag = "MainCamera";
            kamera.transform.position = new Vector3(0f, 1.4f, -3.5f);
            kamera.transform.rotation = Quaternion.Euler(8f, 0f, 0f);
            PembuatGayaPsx.SiapkanKamera(kamera.AddComponent<Camera>());
            kamera.AddComponent<AudioListener>();

            // Latar sederhana supaya gaya PSX terlihat di belakang menu: lantai, meja, dan lampu teplok hangat.
            var lantai = GameObject.CreatePrimitive(PrimitiveType.Plane);
            lantai.name = "Lantai";
            var meja = GameObject.CreatePrimitive(PrimitiveType.Cube);
            meja.name = "Meja";
            meja.transform.position = new Vector3(0.8f, 0.4f, 1f);
            meja.transform.localScale = new Vector3(1.2f, 0.8f, 0.6f);
            var teplok = new GameObject("Teplok").AddComponent<Light>();
            teplok.type = LightType.Point;
            teplok.color = new Color32(0xFF, 0xB4, 0x6B, 0xFF);
            teplok.range = 6f;
            teplok.intensity = 2f;
            teplok.transform.position = new Vector3(0.8f, 1.1f, 1f);

            var ui = new GameObject("UI");
            var pengaturan = ui.AddComponent<PanelPengaturan>();
            PembuatSandbox.Set(ui.AddComponent<MenuUtama>(), "pengaturan", pengaturan);

            EditorSceneManager.SaveScene(scene, JalurScene);
            DaftarkanScene();
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene menu utama dibuat: {JalurScene}");
        }

        // Menu utama harus indeks 0 supaya build dibuka dari sana.
        public static void DaftarkanScene()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(JalurScene, true),
                new EditorBuildSettingsScene(PembuatSandbox.JalurScene, true),
            };
        }
    }
}
```

- [ ] **Step 3: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="223" passed="223" failed="0"`, tanpa `error CS`.

- [ ] **Step 4: Bangun ulang semua lewat batchmode**

Run (editor tertutup, timeout Bash 600000 ms):
```bash
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod TheSending.EditorTools.PembuatMenuUtama.BangunSemua -logFile "${TMPDIR:-/tmp}/the-sending-scene.log"; echo "exit=$?"; grep -E "Gaya PSX terpasang|Scene sandbox dibuat|Scene menu utama dibuat|Shader error|error CS|Exception" "${TMPDIR:-/tmp}/the-sending-scene.log" | head -20
grep -n "path:" ProjectSettings/EditorBuildSettings.asset
grep -c "m_Fog: 1" Assets/_Project/Scenes/Scene_MainMenu.unity Assets/_Project/Scenes/Scene_Sandbox0B.unity
```
Expected: `exit=0`; tiga baris `Gaya PSX terpasang.`, `Scene sandbox dibuat: ...`, `Scene menu utama dibuat: ...`; tanpa `Exception`. Build Settings berisi tepat dua `path:`, urutannya `Scene_MainMenu.unity` lalu `Scene_Sandbox0B.unity`. `m_Fog: 1` ada di kedua scene. Jika muncul `MissingFieldException`, nama field di skrip pembuat tidak cocok dengan komponen: perbaiki skrip pembuat (bukan komponennya) dan jalankan ulang.

- [ ] **Step 5: Commit**

```bash
git status --short   # dari ProjectSettings hanya EditorBuildSettings.asset yang boleh berubah; selain itu laporkan
git add Assets/_Project Assets/Settings ProjectSettings/EditorBuildSettings.asset
git commit -m "feat: build main menu scene, PSX sandbox with pause menu, and register build scenes" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 8: Style guide, uji build Windows, dan pembaruan CLAUDE.md

**Files:**
- Create: `docs/style-guide.md`, `Assets/_Project/Scripts/Editor/PembuatBuild.cs`
- Modify: `CLAUDE.md`

**Interfaces:**
- Consumes: `EditorBuildSettings.scenes` (Task 7), semua nilai `PembuatGayaPsx` (Task 6).
- Produces: `PembuatBuild` (static, Editor): menu **The Sending > Build Windows (uji, Mono)** dan metode batchmode `TheSending.EditorTools.PembuatBuild.Windows`; keluaran `Builds/Windows/TheSending.exe` (folder `Builds/` sudah ada di `.gitignore`).

- [ ] **Step 1: Tulis `docs/style-guide.md`**

```markdown
# Style Guide The Sending (Retro PSX / low-fi)

**Status: DRAF.** Dikunci setelah tim menyelesaikan Daftar Cek Play Mode Fase 0C. Setelah dikunci, perubahan hanya boleh atas persetujuan tim, dan setiap fase ditutup dengan pengecekan gaya terhadap dokumen ini.

Semua nilai render dipasang oleh skrip, bukan lewat Inspector: menu **The Sending > Pasang Gaya PSX** (`Assets/_Project/Scripts/Editor/PembuatGayaPsx.cs`). Untuk mengubah nilai: ubah konstanta di skrip itu, jalankan menunya, lalu perbarui tabel di bawah.

## Render

| Aspek | Nilai | Tempat |
|---|---|---|
| Render Scale | 0.4 | `Assets/Settings/PC_RPAsset.asset` (dan Mobile) |
| Upscaling Filter | Point (Nearest-Neighbor) | aset URP |
| Anti-aliasing | MSAA mati, AA kamera None | aset URP, kamera |
| Full-screen pass | Kuantisasi 5 bit per kanal + dither Bayer 4×4 (kuat 1), dihitung di ruang sRGB, setelah post-processing | Shader Graph `Assets/_Project/Art/Shaders/PSX_Layar.shadergraph` (URP Fullscreen) + Custom Function `PSX_Layar.hlsl`, material `PSX_Layar.mat` di folder yang sama (slider Bit Warna, Kuat Dither), fitur renderer `PSX_Layar` |
| SSAO | Mati | fitur renderer `ScreenSpaceAmbientOcclusion` |

## Post-processing

Satu profil bersama: `Assets/_Project/Settings/PSX_Volume.asset`, dipasang sebagai Volume Profile bawaan aset URP sehingga berlaku di semua scene tanpa objek Volume. Kamera wajib menyalakan Post Processing (`PembuatGayaPsx.SiapkanKamera`).

| Efek | Nilai |
|---|---|
| Film Grain | tipe Thin 1, intensitas 0.4, response 0.8 |
| Vignette | hitam, intensitas 0.35, kehalusan 0.4 |
| Color Adjustments | saturasi -30 |
| Tonemapping | Neutral |

Epilog siang/pagi: Volume lokal terpisah di scene epilog, tetap memakai kabut dan pass PSX yang sama.

## Kabut dan cahaya

| Aspek | Nilai |
|---|---|
| Kabut | Exponential Squared, `#1A1F24`, kepadatan 0.06 (jarak pandang sekitar 15 m) |
| Latar kamera | Solid Color `#1A1F24` (sama dengan kabut) |
| Ambient | Flat `#05070A` |
| Cahaya | Realtime. Senter = spotlight dengan bayangan. Bake lightmap opsional di Fase 8 |

Setiap scene baru memanggil `PembuatGayaPsx.TerapkanKeScene()` dan `SiapkanKamera()` dari skrip pembuatnya.

## Palet semantik

| Arti | Warna |
|---|---|
| Hangat = aman (lampu teplok) | `#FFB46B` |
| Dingin = ancaman (cahaya bulan) | `#6D86A8` |
| Karat dan darah; sorotan tombol menu | `#5A1A12` |
| Teks UI | `#E6E0D1` |
| Kayu tua | cokelat gelap |

## Tekstur

Semua tekstur di `Assets/_Project/Art/` otomatis diimpor dengan ukuran maksimal 256, filter Point, tanpa mipmap (`PrasetelTeksturPsx`, hanya pada impor pertama). Tekstur di luar folder itu tidak ikut aturan ini, jadi jangan menaruh tekstur gim di luar `Art/`.

## Font

| Pemakaian | Font | File |
|---|---|---|
| UI dan subtitle | VT323 | `Assets/_Project/Art/Fonts/Resources/VT323-Regular.ttf` |
| Buku harian (tulisan tangan) | Kalam | `Assets/_Project/Art/Fonts/Resources/Kalam-Regular.ttf` |

Kode mengambil font hanya lewat `UiPabrik.Font` dan `UiPabrik.FontTulisan`. Jika file belum ada, dipakai font bawaan Unity dan muncul peringatan di Console.

## Audio

Mixer: `Assets/_Project/Audio/Resources/MixerUtama.mixer`. Grup: `Master` > `Music`, `SFX`, `Ambience`, `Dialog`. Parameter terekspos: `VolMaster`, `VolMusic`, `VolSFX`, `VolAmbience`, `VolDialog`. Setiap `AudioSource` wajib diarahkan ke salah satu grup, supaya pengaturan volume berlaku.

## UI

Dibangun lewat kode (`UiPabrik`), kanvas acuan 1920×1080. Panel dalam gim (dialog, Jurnal) memakai keyboard. Menu (utama, jeda, pengaturan) memakai tombol yang bisa mouse dan keyboard.
```

- [ ] **Step 2: Buat `PembuatBuild.cs`**

```csharp
using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Build Windows (Mono) dari Mac, untuk uji. Build rilis non-development dibuat di Fase 8.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatBuild.Windows
    public static class PembuatBuild
    {
        const string Keluaran = "Builds/Windows/TheSending.exe";

        [MenuItem("The Sending/Build Windows (uji, Mono)")]
        public static void Windows()
        {
            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone) != ScriptingImplementation.Mono2x)
                throw new BuildFailedException("Scripting backend Standalone harus Mono (CLAUDE.md).");

            var opsi = new BuildPlayerOptions
            {
                scenes = Array.ConvertAll(EditorBuildSettings.scenes, s => s.path),
                locationPathName = Keluaran,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development,   // supaya tombol uji F5/F9 hidup di laptop Windows
            };
            var laporan = BuildPipeline.BuildPlayer(opsi);
            if (laporan.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"Build Windows gagal: {laporan.summary.result}, {laporan.summary.totalErrors} error.");
            Debug.Log($"Build Windows selesai: {Keluaran} ({laporan.summary.totalSize / (1024 * 1024)} MB)");
        }
    }
}
```

- [ ] **Step 3: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="223" passed="223" failed="0"`, tanpa `error CS`.

- [ ] **Step 4: Uji build Windows lewat batchmode**

Build pertama mengompilasi varian shader URP untuk DirectX dan bisa memakan 5–15 menit. Jalankan dengan timeout Bash maksimum (600000 ms) atau `run_in_background`; jika terpotong timeout, jalankan lagi (cache membuat run kedua jauh lebih cepat).
```bash
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod TheSending.EditorTools.PembuatBuild.Windows -logFile "${TMPDIR:-/tmp}/the-sending-build-win.log"; echo "exit=$?"; grep -E "Build Windows selesai|Build Windows gagal|Shader error|error CS|BuildFailedException" "${TMPDIR:-/tmp}/the-sending-build-win.log" | head -20; ls -la Builds/Windows/TheSending.exe
```
Expected: `exit=0`, baris `Build Windows selesai: Builds/Windows/TheSending.exe (... MB)`, dan file `.exe` ada. Tidak ada `Shader error` (ini bukti Shader Graph `PSX_Layar` terkompilasi untuk DirectX). Menjalankan `.exe` di laptop Windows adalah pekerjaan Fase 8.

Setelah build: `git status --short`. `Builds/` tidak boleh muncul (sudah di `.gitignore`). Jika ProjectSettings berubah, jangan di-add dan laporkan. Jika build gagal karena hal di luar kode proyek (modul, lisensi, platform), jangan habiskan lebih dari 30 menit: catat pesan errornya di log progres `CLAUDE.md` dan laporkan ke tim.

- [ ] **Step 5: Perbarui `CLAUDE.md`**

a. Di "## Keputusan yang Sudah Disepakati", ganti kalimat `Nilai gaya dikunci di docs/style-guide.md di akhir Fase 0.` pada butir Gaya Retro PSX menjadi:
```markdown
Nilai gaya ada di `docs/style-guide.md` (dipasang oleh menu The Sending > Pasang Gaya PSX) dan dikunci setelah cek Play Mode 0C.
```

b. Di "## Platform dan Build", tambahkan satu butir di akhir daftar (aturan Shader Graph yang sudah ada tidak diubah):
```markdown
- Build uji Windows dari Mac: menu The Sending > Build Windows (uji, Mono), hasil di `Builds/Windows/` (development build).
```

c. Ganti baris Fase 0 di tabel "Fase dan Progres" menjadi:
```markdown
| 0. Fondasi & Style Lock | H1–H3 | 16–18 Nov | [~] 0A, 0B, 0C selesai di kode (223 EditMode test lulus, build uji Windows berhasil). Menunggu cek Play Mode 0C oleh tim, lalu style guide dikunci |
```

d. Ganti seluruh bagian "## Catatan untuk Fase 0C dan sesudahnya (dari review akhir 0B)" (dari judul itu sampai tepat sebelum "## Daftar Cek Play Mode Fase 0B") dengan:
```markdown
## Catatan untuk Fase 1 dan sesudahnya (dari review 0B dan keputusan 0C)

Sudah diputuskan di 0C (aturannya ada di "Konvensi Kode"): jeda = `Time.timeScale` 0; event `StateDiganti`; kursor milik `GameManager`; field static membersihkan diri. Setiap scene punya objek `GameManager` (duplikat menghancurkan diri) supaya bisa di-Play langsung. `AktifDiMalam` dipasang di objek yang selalu aktif. Jumlah entri Catatan Bapak (7) dan Catatan Raka (6) sudah cocok dengan Game Story 9.1.

Fase 1, saat konten pertama memakainya:
- Ganti `GameManager.SceneAwal` dari `Scene_Sandbox0B` ke scene Prolog. "Lanjutkan" harus memuat scene sesuai `State.MalamKe`, bukan selalu `SceneAwal`. Daftarkan scene baru di `PembuatMenuUtama.DaftarkanScene`.
- Inventaris (tombol I) belum dibuat; aksinya sudah ada di `KontrolPemain.Inventaris`. Panelnya menjeda lewat `Jeda` seperti Jurnal.
- Scene baru: skrip pembuatnya memanggil `PembuatGayaPsx.TerapkanKeScene()` dan `SiapkanKamera()`, dan memasang `MenuJeda` + `PanelPengaturan`.
- Jika tim belum membuat `MixerUtama` (lihat `docs/style-guide.md`), buat sebelum audio pertama masuk. Setiap `AudioSource` diarahkan ke grup mixer.
- `PanelDialog.Mulai` belum menjaga `GameManager.Instance` null; cutscene tanpa GameManager akan error.
- Notifikasi HUD belum menyaring pesan kembar berurutan (memotret ulang, `Perubahan.Katanya` ganda).
- Lebih dari 4 pilihan dialog disembunyikan diam-diam; beri `LogWarning`.
- Jalankan `KartuTeks.Tampilkan`/`KeHitam`/`DariHitam` lewat `kartu.StartCoroutine(...)`, bukan dari MonoBehaviour lain. Fade hitam belum dijaga token, jadi jangan tumpuk dua fade.
- Tambahkan EditMode test yang menjalankan semua kode efek di setiap aset `DataDialog` terhadap `GameState` sementara, supaya kode efek yang salah ketahuan sebelum Play Mode.

Fase 7: hentikan gerak dan input pemain setelah `State.Ending` terisi. Layar ending tidak punya pemain, jadi kursor otomatis terlihat.

Fase 8: pengaturan grafis baru "Layar penuh" saja; tambahkan pemilih resolusi jika tes Windows membutuhkannya. `PembuatBuild.Windows` membuat development build (tombol uji F5/F9 hidup); build rilis harus non-development.
```

e. Tambahkan bagian baru tepat sebelum "## Log Progres":
```markdown
## Daftar Cek Play Mode Fase 0C (dikerjakan tim)

Prasyarat: Shader Graph `PSX_Layar`, material, dan `MixerUtama` sudah dibuat, font sudah ditaruh (langkah di `docs/superpowers/plans/2026-11-16-fase-0c-gaya-dan-menu.md`, bagian "Persiapan Tim"). Buka `Assets/_Project/Scenes/Scene_MainMenu.unity`, tekan Play.

1. Peringatan konten tampil lebih dulu dan kursor terlihat. Enter atau klik "Lanjut" membuka menu. Tombol bisa dipilih dengan mouse dan dengan panah/WASD + Enter; tombol terpilih berwarna karat.
2. Jika belum ada save, tombol "Lanjutkan" tidak ada.
3. Pengaturan: ubah volume, sensitivitas, Balik sumbu Y. Stop lalu Play lagi: nilainya masih sama. Esc menutup pengaturan.
4. "Mulai Baru" memuat sandbox. Kursor terkunci, kartu teks tampil, lalu pemain bisa bergerak.
5. Tidak ada peringatan mixer atau parameter di Console. Tampilan PSX: piksel besar kotak-kotak (bukan buram), pola dither terlihat di gradasi kabut dan tepi cahaya senter, warna agak pudar, sudut layar gelap, ada bintik film. Dinding jauh tertelan kabut. Pola dither seukuran piksel besar, bukan piksel layar.
6. Esc membuka menu jeda: jam berhenti, jarum kompas (C) berhenti berputar, kursor terlihat. "Lanjut" atau Esc menutupnya dan kursor terkunci lagi. Esc saat dialog atau Jurnal terbuka tidak membuka menu jeda.
7. Dari menu jeda buka Pengaturan, naikkan sensitivitas, kembali: putaran mouse terasa lebih cepat. Balik sumbu Y membalik dongak.
8. Shift kanan juga berlari.
9. Regresi 0B saat dunia berhenti: buka dialog di Meja segera setelah memotret Foto keluarga; notifikasi tetap hilang sendiri sekitar 3 detik dan kilatan kamera tetap memudar.
10. Save: tekan F5 (notifikasi "Checkpoint disimpan.", Kondisi Ibu 60 di kiri atas). Esc > "Keluar ke Menu Utama" (peringatan konten tidak tampil lagi). "Lanjutkan" kini ada; klik: Kondisi Ibu 60. Tekan F9: Kondisi Ibu 55. Stop Play. Play lagi dari `Scene_MainMenu`, "Lanjutkan": Kondisi Ibu tetap 55 (penalti tersimpan).
11. "Mulai Baru" saat ada save memunculkan konfirmasi. "Batal" kembali ke menu. "Ya, mulai baru": Kondisi Ibu 60; keluar ke menu lalu "Lanjutkan": tetap 60 (save lama tertimpa).
12. Tekstur: taruh satu gambar besar (misalnya 2048 px) di `Assets/_Project/Art/Textures/`. Di Inspector: Max Size 256, Filter Mode Point, Generate Mipmap mati.
13. Font: setelah TTF ditaruh, teks UI memakai VT323 dan tab Buku Harian (Tab > 2) memakai Kalam. Peringatan font di Console hilang.
14. Catat nilai gaya yang ingin diubah (kuat dither, grain, vignette, kabut, ukuran teks). Setelah disetel, status `docs/style-guide.md` diganti dari DRAF menjadi TERKUNCI dan Fase 0 ditandai selesai.
```

f. Tambahkan di "Log Progres" (ganti `YYYY-MM-DD` dengan tanggal pengerjaan):
```markdown
- **YYYY-MM-DD:** Fase 0C selesai di kode: jeda memakai `Time.timeScale`, event `StateDiganti`, pengaturan (volume, sensitivitas, balik Y, layar penuh) tersimpan di PlayerPrefs, `Scene_MainMenu` dengan peringatan konten, menu jeda, gaya PSX (render scale 0.4, point, dither 5 bit, Volume bersama, kabut, prasetel tekstur), `docs/style-guide.md` (draf), build uji Windows dari Mac berhasil. Shader full-screen memakai Shader Graph + Custom Function (`PSX_Layar.shadergraph`, dibuat tim). Inventaris pindah ke Fase 1. Menunggu cek Play Mode 0C dan font dari tim.
```

- [ ] **Step 6: Commit**

```bash
git status --short
git add docs/style-guide.md Assets/_Project CLAUDE.md
git commit -m "docs: add style guide, Windows smoke build script and 0C Play Mode checklist" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

## Daftar Cek Play Mode Fase 0C (dikerjakan tim)

Sama dengan yang ditulis ke `CLAUDE.md` di Task 8 Step 5e (14 butir). Ringkasnya: (1–4) menu utama, peringatan konten, pengaturan tersimpan, Mulai Baru; (5) tampilan PSX; (6–9) menu jeda dan dunia berhenti; (10–11) save, Lanjutkan, tertangkap, penalti -5 tersimpan, timpa save; (12–13) prasetel tekstur dan font; (14) catatan penyetelan lalu style guide dikunci.

**Jika butir 5 gagal karena dither seukuran piksel layar (bukan piksel besar):** fitur `PSX_Layar` ternyata berjalan setelah upscale. Ganti `InjectionPoint.AfterRenderingPostProcessing` menjadi `BeforeRenderingPostProcessing` di `PembuatGayaPsx.AturRenderer`, jalankan menu **Pasang Gaya PSX**, dan catat perubahan itu di `docs/style-guide.md`. **Jika layar hitam atau magenta:** buka `PSX_Layar.shadergraph`, periksa error di node Custom Function dan sambungan di "Persiapan Tim" bagian A.

## Self-Review

**Cakupan spesifikasi**

| Butir lingkup 0C | Task |
|---|---|
| Jeda = `Time.timeScale`, pemeriksaan skrip saat jeda, peringatan underflow, aturan di CLAUDE.md | 2 |
| Event `StateDiganti`; `Perubahan.Malam` hanya saat malam berubah; `AktifDiMalam` dan `Hud` menyesuaikan | 2 |
| Kursor pindah dari `PemainGerak` ke `GameManager`; peringatan `AktifDiMalam`/`Hud`/`PanelJurnal`; Shift kanan; static membersihkan diri | 2, 3 |
| Pengaturan: Core + test, PlayerPrefs, volume, sensitivitas, balik Y, layar penuh | 1, 2, 3 |
| Font satu pintu (VT323/Kalam, fallback) dan lokasi file untuk tim | 3, Persiapan Tim C |
| Audio Mixer: langkah manual, kode toleran jika belum ada | 2, Persiapan Tim B |
| Menu jeda (Lanjut, Pengaturan, Keluar ke Menu Utama) | 4 |
| Cara memicu checkpoint dan tertangkap untuk cek save | 4 (`AlatUji`) |
| `Scene_MainMenu`: peringatan konten, Lanjutkan, Mulai Baru + konfirmasi, Pengaturan, Keluar; konstanta `SceneAwal` | 2, 5, 7 |
| PSX: render scale, point, AA mati, full-screen pass (Custom Function Task 3, graph + material oleh tim, dipasang Task 6), Volume bersama, kabut, ambient, prasetel tekstur | 3, Persiapan Tim A, 6 |
| Sandbox memakai gaya PSX + menu jeda; pembuat `Scene_MainMenu`; Build Settings | 7 |
| Jumlah entri Jurnal vs Game Story 9.1 | 7 (sudah cocok, tidak ada perubahan) |
| `docs/style-guide.md`, uji build Windows, CLAUDE.md (baris fase, log, daftar cek, catatan 0C) | 8 |
| Inventaris (I) | Tidak dikerjakan, ditunda ke Fase 1 (keputusan tim) |

**Pemindaian placeholder:** tidak ada "TBD", "serupa Task N", atau langkah tanpa kode. Satu-satunya isian adalah `YYYY-MM-DD` di log progres (aturan yang sama dengan rencana 0B). Nilai grain (0.4) dan vignette (0.35/0.4) tidak ada di desain; itu nilai awal yang ditetapkan rencana ini dan disetel tim di butir 14.

**Konsistensi tipe dan nama**
- `Pengaturan.NamaGrup` (5 nama) dipakai `PenyimpanPengaturan.Terapkan` (`"Vol" + nama`) dan `PanelPengaturan` (`LabelGrup` 5 label, urutan sama); nama parameter mixer di Persiapan Tim, Global Constraints, dan style guide sama: `VolMaster`, `VolMusic`, `VolSFX`, `VolAmbience`, `VolDialog`.
- `GameManager`: `Pengaturan`, `SimpanPengaturan()`, `KunciKursor`, `PeringatanSudahTampil`, `StateDiganti`, `MuatScene(string)`, `SceneMenuUtama`, `SceneAwal` didefinisikan di Task 2 dan dipakai dengan nama yang sama di Task 3 (`PanelPengaturan`), 4 (`MenuJeda`, `AlatUji`), 5 (`MenuUtama`), 7 (`PembuatMenuUtama`).
- `UiPabrik.BuatCanvasMenu(string, int)`, `BuatTombol(Transform, string, Vector2, Vector2, UnityAction)`, `Pilih(Button)` didefinisikan di Task 3, dipakai di Task 3–5 dengan urutan argumen yang sama.
- `PanelPengaturan.Buka(Action)`, `Tutup()`, `Terbuka` dipakai `MenuJeda` dan `MenuUtama`.
- Field `[SerializeField]` yang diisi pembuat scene: `MenuJeda.pengaturan`, `AlatUji.hud`, `MenuUtama.pengaturan` cocok dengan `PembuatSandbox.Set(..., "pengaturan"/"hud", ...)`.
- Jalur material satu-satunya: `Assets/_Project/Art/Shaders/PSX_Layar.mat` (Persiapan Tim A langkah 9, Global Constraints, `PembuatGayaPsx.JalurMaterial`, style guide). Pembuat scene tidak menyentuh material; mereka hanya memanggil `PembuatGayaPsx`. Nama fungsi `PsxLayar` (node) = `PsxLayar_float` (.hlsl); urutan input node = urutan parameter; Reference `_BitWarna`/`_KuatDither` = yang diperiksa `MuatMaterial`.
- `PembuatGayaPsx.Pasang/TerapkanKeScene/SiapkanKamera` didefinisikan di Task 6, dipakai di Task 7. `PembuatSandbox.JalurScene` dan `Set` dibuat `internal` di Task 7 sebelum dipakai `PembuatMenuUtama`.
- `PemainGerak.SensitivitasMouse` dihapus di Task 2; tidak ada pemakai lain (dicek dengan grep di Task 2 Step 9).
- Jumlah test: 208 + 15 = 223, dipakai konsisten di semua task. `PengaturanTests` berisi 8 `[Test]` tunggal, 3 `[TestCase]` JSON kosong/rusak, dan 4 `[TestCase]` desibel.

**Yang belum bisa diverifikasi tanpa menjalankan Unity** (implementer melapor jika berbeda): apakah graph Fullscreen baru dibuat kosong atau berisi node contoh (langkah A2 menangani keduanya); apakah Screen Position mode Pixel di target Fullscreen benar-benar memberi piksel target render berskala (jika pola dither salah ukuran, lihat jalan keluar di atas); label persis kolom properti Float di Graph Inspector (Mode/Min/Max/Default); fitur full-screen `AfterRenderingPostProcessing` berjalan di resolusi rendah sebelum upscale (ada jalan keluar di atas); nama field `m_SelectedUpscalerName` aktif atau tidak di proyek ini (skrip menangani keduanya); `BuildPipeline.BuildPlayer` ke Windows dari batchmode tanpa `-buildTarget` (jika Unity menolak, tambahkan `-buildTarget Win64` pada perintah Task 8 Step 4); urutan bongkar scene lama sebelum `OnEnable` scene baru untuk `KunciKursor` (dicek butir 4 dan 6 daftar cek).
