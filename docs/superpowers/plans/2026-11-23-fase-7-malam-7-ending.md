# Fase 7: Malam 7 + Ending — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Malam 7 bisa dimainkan dari akhir sidang Malam 6 (dua jalur), keempat ending bisa dicapai dan diputar di `Scene_Epilog`, setiap ending ditutup pesan penutup lalu menu utama.

**Architecture:** Aturan baru di Core (TDD): laju jam Malam 7, obat otomatis, `JamGim.Majukan` untuk penalti lilin/kokok, `PerilakuSantet.MulaiKejar`, `PetaScene` untuk Watuireng/Epilog, `BerkasSave.Hapus`. Runtime: `GameManager` memuat `Scene_Epilog` begitu `Ending` terisi (satu jalan bersama untuk semua sutradara), `AlurMalam7` (rumah, dua jalur), `AlurWatuireng`, `AlurEpilog`, `PembuntutSosok`, `LilinPasaran`. Editor: `PembuatDataFase7`, `PembuatMalam7`, `PembuatWatuireng`, `PembuatEpilog`; semua scene dibangun ulang dengan "Bangun Ulang Semua".

**Tech Stack:** Unity 6000.6.4f1, URP 17.6, C#, NUnit EditMode, Timeline 6.6.0, AI Navigation 2.0.14, Input System.

**Spec:** `docs/superpowers/specs/2026-11-23-fase-7-design.md` (baca dulu). Sumber cerita: Game Story `docs/TheSending_FullGameStory.md` baris 664–788 (Malam 7, Ending, pesan penutup), 823–834 (Halaman 8). Aturan: `docs/superpowers/specs/2026-11-15-the-sending-design.md` Bagian 4.1, 4.3, 4.4, 4.9, 5.

## RISIKO LINGKUP DAN JADWAL

| # | Risiko / penyederhanaan | Dampak | Usul |
|---|---|---|---|
| R1 | Epilog = bidikan kamera bergerak + kartu teks (K50-A), tokoh diam (model `tokoh-dasar` pengganti) | Kurang sinematik | Fase 8 bila ada waktu |
| R2 | "Salah lilin" hanya memajukan jam 15 menit (tanpa Wiryo bangun di tempat). Desain awal menyebut salah ketiga = bangun; dibuang karena checkpoint sebelum pilihan tidak ada (K51, keputusan Claude) | Akibat salah urutan lebih ringan | Setel angka di Fase 8 |
| R3 | Jalur Fitnah: tertangkap memuat checkpoint awal Malam 7 (jam kembali ke 18.00) | Pemain yang sering tertangkap bermain lebih lama | Diterima; hasilnya tetap Fitnah |
| R4 | Keluar gim di Watuireng sebelum pilihan: "Lanjutkan" mulai dari awal Malam 7 (lingkaran garam diulang) | Kemajuan hilang | Sama dengan desa/makam (tanpa checkpoint) |
| R5 | Teks yang tidak ada di Game Story (narasi epilog, dialog sela, Halaman 8, Catatan Bapak pasaran) adalah DRAF | Perlu cek tim | Daftar cek butir teks |
| R6 | Semua model baru pengganti (sanggar, kandang, cemani, lilin, kendi induk, lontar, buku, Bripka Anton, Raka, cermin, hujan) | Tampilan | `docs/aset/fase-7.md` sudah ada |
| R7 | Ending dimuat langsung dari `GameManager` saat `Ending` terisi, juga di tengah dialog/cutscene | Adegan terpotong | Satu-satunya pemicu yang memotong adegan adalah habis waktu/Kondisi 0; sutradara memanggil `Netralkan(M7)`/`LolosDariWiryo` setelah adegannya selesai |
| R8 | Pekerjaan di Windows (bukan Mac); skrip batchmode sudah disesuaikan (`b5cd1d1`) | — | — |

## KEPUTUSAN TIM

- **K49-A, K50-A: SUDAH DIPUTUSKAN (23 Nov).** Lihat spec.
- **K51 (keputusan Claude, bisa dikoreksi tim):** salah lilin = jam +15 menit, urutan diulang; kokok cemani = jam +5 menit (paling sering sekali per 20 detik nyata). Tidak ada "salah ketiga".
- **K52 (keputusan Claude):** setelah pesan penutup, save dihapus (`BerkasSave.Hapus`), jadi "Lanjutkan" hilang dan gim selesai. "Lanjutkan" sebelum epilog selesai (save dengan `Ending` terisi) memutar epilog lagi.
- **K53 (keputusan Claude):** Mbah Wiryo memakai `SosokSantet` (+ `PerilakuSantet.MulaiKejar`) tanpa gemerincing; lolos = zona gerbang kebun.

## Urutan Eksekusi

Editor Unity **tertutup** selama semua task (batchmode). Task 1–9 berurutan (tidak paralel: semuanya menjalankan Unity batchmode). Setelah Task 9: **berhenti** untuk cek Play Mode tim (daftar di bawah). Task 10 setelah cek. Model: Task 1–3 menengah (sonnet); Task 4–9 terkuat (opus), karena menyambung banyak kode lama dan scene.

## Global Constraints

- Unity 6000.6.4f1, URP 17.6. Batchmode: `bash tools/run-editmode-tests.sh [Filter]`, `bash tools/bangun-semua.sh [Metode]` (keduanya memulihkan `ProjectSettings.asset`). Setelah perintah Unity lain: `git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset`.
- Core (`Assets/_Project/Scripts/Core`, assembly `TheSending.Core`): C# murni, TDD, hanya kelas Core yang mengubah `GameState`. Runtime (`Scripts/Runtime`): MonoBehaviour. Editor (`Scripts/Editor`, namespace `TheSending.EditorTools`).
- Nama domain bahasa Indonesia, istilah teknis Inggris. Hanya satu singleton `GameManager`. Kursor hanya diatur `GameManager`. Input System saja.
- Jeda = `GameManager.Jeda(true/false)` tepat sekali, termasuk `OnDisable`. UI saat jeda memakai `Time.unscaledDeltaTime`. Adegan: `KameraAdegan.Mulai/Selesai`, Timeline dari `PembuatTimeline` (Unscaled, wrap None), `PemutarTimeline.PutarDanTunggu`, `KartuTeks.Tampilkan`, `Guncang.Posisi`. **Tidak ada `WaitForSeconds`.** Spasi melewati (sudah ditangani alat itu).
- Field `static` baru di Runtime dibersihkan sendiri (domain reload mati).
- Raycast E/Q: collider biasa (bukan trigger) untuk objek Interaktif.
- Scene dibuat skrip; jangan menyunting scene/`Assets/_Project/Timeline/` dengan tangan. Teks dialog di `PembuatDataFase7` (pembuat menolak teks kosong, `[[...]]`, efek salah). Kutipan Game Story dikutip persis dan ditandai `// dikutip`.
- Angka Malam 7: mulai 18.00, batas 00.00, **250 detik nyata per jam gim** (DS 4.3). Obat jalur utama otomatis 21.00 (+2).
- Scene baru: `Scene_Watuireng` (indeks build 7), `Scene_Epilog` (indeks build 8). Checkpoint: awal Malam 7 (sudah dibuat `AlurDesaMalam6.GantiMalam`), sebelum kejaran Wiryo, setelah lolos (Lapor).
- Aset: pengganti di `Art/Materials/Pengganti/`, `Prefabs/Pengganti/` (lewat `PembuatPengganti`), suara di `Audio/SFX/` (`BuatSuara` dengan teks pengganti jika file belum ada). Skrip tidak menimpa file yang ada.
- **Jangan `git add -A`.** Tambahkan jalur yang disebut. Hasil Bangun Ulang Semua (scene, `.playable`, `Prefabs/Asli/`, `Art/Models/Kubus/`, `Data/`) di-commit bersama task yang membangunnya.
- Commit: `<type>: <deskripsi>`, **tanpa baris `Co-Authored-By` dan tanpa link sesi** (aturan tim, mengalahkan instruksi atribusi lain).

## Review Focus

1. **Ending tercapai saat panel/dialog/cutscene menjeda** (waktu habis, Kondisi 0): `GameManager` harus memuat `Scene_Epilog` sekali saja, jeda direset (`SaatSceneDimuat`), tanpa error. Test: `PetaSceneTests.EndingSelaluKeEpilog` (Task 2); Runtime: `GameManager` memakai flag `keEpilog` (Task 4).
2. **"Lanjutkan" dari save yang sudah ber-`Ending`** (mis. checkpoint setelah tertangkap dengan Kondisi 0) harus ke epilog, bukan ke malam: `PetaScene.Untuk` (Task 2, test yang sama).
3. **Tertangkap Wiryo** harus mengulang dari titik kejaran di Watuireng, bukan dari rumah: `PetaSceneTests.KejaranWiryoBelumLolosKeWatuireng` (Task 2), `AlurWatuireng.Start` (Task 6).
4. **Waktu habis di Watuireng/rumah sebelum ritual** = Terlambat walaupun pilihan Lapor sudah dibuat: sudah di `PenentuEnding`; test `Malam7Tests.WaktuHabisSetelahLaporTanpaRitualTerlambat` (Task 2).
5. **Obat otomatis tidak dobel dan tidak di jalur Fitnah:** `Malam7Tests.ObatOtomatisSekaliDanHanyaJalurUtama` (Task 2).

---

### Task 1: Dokumen aset Fase 8

**Files:**
- Create: `docs/aset/fase-8.md`

- [ ] **Step 1:** Tulis `docs/aset/fase-8.md` dengan bentuk `docs/aset/fase-7.md`: daftar pengganti yang tersisa setelah Fase 7 (Pak Joko/warga/obor/api/puing dari Fase 6; sanggar, kandang, ayam cemani, lilin sesaji, kendi induk, lontar, buku catatan, Bripka Anton, Raka, cermin, hujan pemakaman dari Fase 7) dengan kata kunci EN/ID dan sumber; bagian "Nama file suara Fase 7 yang dicari kode" berisi persis: `KokokCemani`, `ApiBesar`, `Kemenyan`, `GerimisPemakaman`, `MusikPenutup`, `MusikKejarWiryo`, `BurungPagi` (folder `Audio/SFX/` atau `Audio/Music/` untuk dua musik). Status Fase 8 (balancing, build rilis) sebagai catatan.
- [ ] **Step 2: Commit** `git add docs/aset/fase-8.md && git commit -m "docs: daftar aset Fase 8"`

### Task 2: Core — aturan Malam 7 (TDD)

**Files:**
- Modify: `Assets/_Project/Scripts/Core/JamGim.cs`, `Core/PetaScene.cs`, `Core/PerilakuSantet.cs`, `Core/BerkasSave.cs`, `Core/Puzzle.cs` (konstanta lilin/kokok)
- Create: `Core/ObatMalam7.cs`, `Tests/EditMode/Malam7Tests.cs`
- Modify tests: `Tests/EditMode/PetaSceneTests.cs`, `PerilakuSantetTests.cs`, `BerkasSaveTests.cs`, `JamGimTests.cs`

**Interfaces (Produces):**
- `JamGim.DetikNyataPerJamGimMalam7 = 250f`; `JamGim.DetikPerJamGim(int malam)` mengembalikan 250 untuk malam 6 dan 7. `SisaDetikMalam7` memakai `DetikPerJamGim(7)`.
- `JamGim.Majukan(GameState s, float menit)`: tambah menit, dijepit ke `Batas(s.MalamKe)`, `Umumkan(Perubahan.Jam)`, lalu `ObatMalam7.Cek(s)` dan `PenentuEnding.Cek(s)` bila Malam 7. Menit ≤ 0 tidak berbuat apa-apa.
- `JamGim.Maju`: di Malam 7 memanggil `ObatMalam7.Cek(s)` sebelum `PenentuEnding.Cek(s)`.
- `public static class ObatMalam7 { public static bool Cek(GameState s); }`: true dan +2 (`KondisiIbu.Ubah(s, KondisiIbu.Obat)`, `ObatSudahDiberikan = true`) hanya jika `MalamKe == 7`, `HasilSidang == Berhasil`, `!ObatSudahDiberikan`, `MenitGim >= JamGim.Jam21`, `Ending == Belum`.
- `LilinSesaji.MenitSalah = 15f`; `public static class KokokCemani { public const float Jarak = 3f, Menit = 5f, JedaDetik = 20f; public static bool Berkokok(float jarak) => jarak <= Jarak; }`.
- `PerilakuSantet.MulaiKejar()`: `Keadaan = Kejar`.
- `PetaScene.Watuireng = "Scene_Watuireng"`, `PetaScene.Epilog = "Scene_Epilog"`. `Untuk(s)`: `Ending != Belum` → Epilog; `MalamKe == 7 && PilihanAkhir != Belum && !LolosKejaranWiryo` → Watuireng; selain itu aturan lama.
- `BerkasSave.Hapus(string folder)`: hapus file save (dan `.tmp`) jika ada; aman jika tidak ada.

- [ ] **Step 1: Tulis test yang gagal**
  - `Malam7Tests.LajuJamMalam7SetengahLajuBiasa`: `DetikPerJamGim(7) == 250`; Maju 250 detik di Malam 7 dari 18.00 → 19.00.
  - `Malam7Tests.SisaDetikMalam7Dari18Sampai24 == 1500`.
  - `Malam7Tests.ObatOtomatisSekaliDanHanyaJalurUtama`: Malam 7 Berhasil 20.59 → `Cek` false; 21.00 → true, Kondisi +2; panggil lagi → false; jalur Gagal 21.30 → false; Maju melewati 21.00 di jalur Berhasil → `ObatSudahDiberikan`.
  - `Malam7Tests.MajukanDijepitDiTengahMalamDanMemicuEnding`: Malam 7 Berhasil 23.50, `Majukan(s, 15)` → `MenitGim == 1440`, `Ending == Terlambat`. Malam 7 jalur Gagal 23.55 `Majukan(5)` → `Fitnah`.
  - `Malam7Tests.WaktuHabisSetelahLaporTanpaRitualTerlambat`: Berhasil, `Pilih(Lapor)`, Maju sampai 00.00 → Terlambat.
  - `Malam7Tests.KokokHanyaDekatKandang`: `Berkokok(2.9f)` true, `Berkokok(3.1f)` false.
  - `PerilakuSantetTests.MulaiKejarLangsungMengejar`: `MulaiKejar()` → `Keadaan == Kejar`; `Tick` dengan jarak 0,5 dan tidak bersembunyi → true.
  - `PetaSceneTests.EndingSelaluKeEpilog` (Ending Fitnah di Malam 7 dan Terlambat di Malam 3 → `Epilog`); `KejaranWiryoBelumLolosKeWatuireng` (Malam 7, Lapor, belum lolos → Watuireng; sudah lolos → RumahKeluarga; Belum → RumahKeluarga).
  - `BerkasSaveTests.HapusMenghapusSaveDanAmanJikaTidakAda` (folder sementara seperti test BerkasSave yang ada).
- [ ] **Step 2:** `bash tools/run-editmode-tests.sh` → gagal (compile error dihitung gagal).
- [ ] **Step 3:** Tulis kode sesuai Interfaces.
- [ ] **Step 4:** `bash tools/run-editmode-tests.sh` → `result="Passed"`, total sekitar 390.
- [ ] **Step 5: Commit** `git add Assets/_Project/Scripts/Core Assets/_Project/Tests/EditMode && git commit -m "feat: aturan Core Malam 7 (laju jam, obat otomatis, Majukan, PetaScene epilog/Watuireng, MulaiKejar, hapus save)"` (sertakan `.meta` file baru).

### Task 3: Runtime — jalan bersama ke epilog

**Files:**
- Modify: `Runtime/GameManager.cs`, `Runtime/PemainGerak.cs`, `Runtime/AlurMalam1.cs`, `AlurMalam2.cs`, `AlurMalam3.cs`, `AlurMalam4.cs`, `AlurMalam5.cs`, `AlurDesaMalam6.cs`, `AlurMalam6.cs`, `Runtime/Hud.cs`, dan setiap `gm.MuatScene(... SceneMenuUtama ...)` karena ending (grep `SceneMenuUtama` dan `KeMenuJikaEnding` di `Runtime/`).

**Interfaces:**
- Consumes: `PetaScene.Untuk/Epilog` (Task 2), `BerkasSave.Hapus`.
- Produces: `GameManager.HapusSave()` (memanggil `BerkasSave.Hapus(Folder)`); `GameManager` memuat `PetaScene.Epilog` sekali saat `State.Ending != Belum` dan scene aktif bukan Epilog/menu (cek di `Update`, flag `keEpilog` direset di `SaatSceneDimuat` bila scene = Epilog atau menu).

- [ ] **Step 1:** `GameManager`: flag ke epilog seperti di atas; `HapusSave()`.
- [ ] **Step 2:** `PemainGerak`: tidak memproses gerak/pandang/interaksi saat `State.Ending != Belum` (lihat cara `PemainGerak` membaca `GameManager.Instance`).
- [ ] **Step 3:** Ganti semua cabang "ending → menu utama" di sutradara dengan `gm.MuatScene(gm.SceneUntukState)` (yang kini memilih Epilog). `KeMenuJikaEnding` diganti nama `KeEpilogJikaEnding`, isinya memuat `gm.SceneUntukState`. Hapus `Debug.LogWarning` "layar ending dibuat di Fase 7".
- [ ] **Step 4:** `AlurDesaMalam6.GantiMalam`: setelah `PergantianMalam.Lanjut` dan cek ending, `gm.SimpanCheckpoint()` lalu `gm.MuatScene(gm.SceneUntukState)` (hapus kartu "(Bersambung...)"). `AlurMalam6.Start`: hapus notifikasi "Malam 7 belum dibuat (Fase 7)." (cukup `if (s.MalamKe != 6) return;`).
- [ ] **Step 5:** `Hud`: di Malam 7 baris hitung mundur menampilkan `"Malam ini Selasa Kliwon\nTengah malam: {menit} menit lagi"` dengan menit nyata = `ceil(JamGim.SisaDetikMalam7(s) / 60)` (pola baris Malam 6 di `Hud.cs:145-149`).
- [ ] **Step 6:** `bash tools/run-editmode-tests.sh` → semua lulus (tidak ada `error CS`).
- [ ] **Step 7: Commit** `feat: ending selalu ke Scene_Epilog, input berhenti setelah ending, Malam 6 berlanjut ke Malam 7`.

### Task 4: Editor — data Fase 7 dan tokoh baru

**Files:**
- Create: `Editor/PembuatDataFase7.cs`
- Modify: `Editor/PembuatPengganti.Tokoh.cs` (tokoh `Raka`, `BripkaAnton`), `Editor/PembuatPengganti.cs` (bentuk pengganti baru), `Editor/PembuatMenuUtama.cs` (panggil `PembuatDataFase7.Buat()` setelah Fase 6)

**Interfaces (Produces):** konstanta nama dialog di `PembuatDataFase7` dan `Muat(string)` (pola `PembuatDataFase6`):
- Rumah: `LingkaranGaram` (Bu Sumi, Laras, Raka; berisi kutipan Bu Sumi GS "Santet iki ono induke, Le..." `// dikutip`, terjemahan persis; efek terakhir `ritual-dari-sumi` jika belum ada — efek boleh dipakai berulang? cek `RitualPenangkal.Pelajari`; jika melempar saat sudah dipelajari, panggil dari kode di `AlurMalam7` dengan cek, bukan efek), `RitualTerakhir` (Bu Sumi memimpin, Raka), `FitnahPembuka` tidak perlu (kartu).
- Watuireng: `BukuCatatan` (efek `bukti:B8`, `halaman:H8`; kutipan "Darmo, Sumberwangi: Hadi (rampung), Ratmi, Raka, Laras." `// dikutip`), `KendiInduk` (efek `media:M7`; foto keluarga tertusuk jarum dan sisir Bu Sumi), `Lontar` (baris dengan dua pilihan, teks pilihan dari tabel GS "Pilihan Terakhir", efek `pilihan-akhir:Lapor` / `pilihan-akhir:Balas`), `WiryoMemergoki` (kutipan "Bapakmu dulu juga keras kepala, Le. Lihat sekarang dia di mana." `// dikutip`), `SesajiDilihat` (petunjuk lilin: urutan pasaran mulai malam ini).
- Jurnal: Halaman 8 (isi: ayah melihat Wiryo di pekarangan pada malam sebelum sakit, DRAF) lewat cara halaman diary diisi di pembuat data sebelumnya (cari `Halaman` di `PembuatDataFase5`/`PembuatSandbox`); Catatan Bapak "Sesaji dan hari pasaran" (urutan Kliwon, Legi, Pahing, Pon, Wage; DRAF).
- Tokoh: `Raka` (kemeja/kaus gelap, celana jeans), `BripkaAnton` (seragam coklat polisi, topi) dalam daftar `Tokoh` yang sama.
- Bentuk pengganti baru (`DaftarPrefab`): `LilinSesaji` (lilin + piring kecil), `NampanSesaji`, `KendiInduk` (gelap, lebih besar dari `KendiTanahMerah`), `Lontar`, `BukuCatatan`, `KandangAyam`, `AyamCemani`, `Cermin`, `Payung`.
- [ ] **Step 1:** Tulis `PembuatDataFase7` (pola `PembuatDataFase6`: `B`, `P`, `Simpan`, `Muat`, `IsiJurnal`), tokoh, bentuk.
- [ ] **Step 2:** `bash tools/bangun-semua.sh TheSending.EditorTools.PembuatDataFase7.Buat` → `OK`. Lalu `bash tools/run-editmode-tests.sh` → lulus (`DataDialogTests` memeriksa semua aset dialog baru).
- [ ] **Step 3: Commit** kode + `Assets/_Project/Data/Fase7/` + `Data/Jurnal.asset` + prefab pengganti baru (+`.meta`): `feat: data dialog Fase 7, Halaman 8, tokoh Raka dan Bripka Anton, bentuk pengganti Malam 7`.

### Task 5: Runtime + Editor — AlurMalam7 di rumah keluarga

**Files:**
- Create: `Runtime/AlurMalam7.cs`, `Editor/PembuatMalam7.cs`
- Modify: `Editor/PembuatRumahKeluarga.cs` (panggil `PembuatMalam7.Pasang(rig, sutradara, sosok, ibu)` setelah `PembuatMalam6.Pasang`), `Editor/PembuatMalam2.cs` jika perlu `AktifPada` sampai 7.

**Interfaces:**
- Consumes: `PembuatDataFase7.Muat(...)` (Task 4), `PetaScene.Watuireng`, `SosokSantet` dari `PembuatMalam2.Pasang`, pola `AlurMalam6`/`AlurMalam2` (sembunyi, tertangkap).
- Produces: `public class AlurMalam7 : MonoBehaviour` dengan konstanta sinyal `SinyalApi = "ApiInduk"`, `SinyalJerit = "JeritSosok"`, `SinyalLenyap = "SosokLenyap"`.

Perilaku (`Start`, hanya `MalamKe == 7`):
- **Jalur utama, belum ke Watuireng** (`PilihanAkhir == Belum`): kartu "Malam 7: Malam Selasa Kliwon" (`KalenderJawa.NamaMalam(7)` + ringkasan DRAF + "Tujuan: Membakar media induk sebelum tengah malam"), lalu adegan lingkaran garam: kamera ke ranjang Ibu, Bu Sumi dan Laras tampil, cincin garam dan kelor tampil, dialog `LingkaranGaram`. Lalu HUD "Berangkat ke Watuireng lewat pintu depan." Pintu depan (`InteraktifAlur`, prompt "Berangkat ke Watuireng") → `KameraAdegan`, layar hitam, `MuatScene(PetaScene.Watuireng)`.
- **Jalur utama, sudah lolos dengan Lapor** (`LolosKejaranWiryo`): Raka di ruang tengah, Bu Sumi menunggu di dapur; pastikan ritual diketahui dan bahan lengkap (`RitualPenangkal.Pelajari(s, dariBuSumi: true)` bila perlu, lihat API), lalu cutscene ritual terakhir (Timeline `Malam7_RitualTerakhir`, sekitar 8 detik, kamera ke anglo; sinyal `ApiInduk` menyalakan api anglo dan suara api; `JeritSosok` memutar stinger dan menampilkan Sosok di dekat jendela dapur; `SosokLenyap` mematikan Sosok), dialog `RitualTerakhir`, lalu `MediaSantet.Netralkan(s, Kode.M7)` (Ending Sejati, `GameManager` memuat epilog). Simpan checkpoint dulu sebelum cutscene.
- **Jalur Fitnah** (`s.JalurFitnah`): kartu Malam 7 dengan teks Fitnah (GS: tanpa Bu Sumi, Raka hanya bisa bertahan dan menjaga Ibu sampai 00.00), Sosok berpatroli (objek dari Malam 2, titik patroli yang sama), HUD petunjuk sembunyi. Tertangkap: pola `AlurMalam2.Tertangkap` (stinger, kartu, `gm.Tertangkap()`, `MuatScene(gm.SceneUntukState)`). Pintu depan: "Aku tidak boleh meninggalkan Ibu malam ini." Pukul 00.00 Ending Fitnah dimuat `GameManager`.
- Obat otomatis: saat `ObatSudahDiberikan` berubah jadi true di jalur utama, HUD "Laras memberi Ibu obat pukul 21.00." sekali.
- Bu Sumi dan Laras di rumah hanya di jalur utama (Bu Sumi bergabung karena `HasilSidang.Berhasil`).

Editor `PembuatMalam7.Pasang`: sutradara `AlurMalam7`, titik mulai/pulang, tokoh Bu Sumi + Laras (Malam 7 saja, `AktifPada`), cincin garam (lingkaran 12–16 kubus putih kecil dan beberapa ikat kelor di lantai sekitar ranjang Ibu, mati sampai adegan), pintu depan Malam 7, Timeline ritual, suara (`ApiBesar`, `StingerKejutan`, `DetakJantung`, `MusikTegang`/`DroneHoror`). Pastikan objek Malam 6 yang aktif "6..7" tidak bentrok (periksa `AktifPada` di `PembuatMalam6`).

- [ ] **Step 1:** Tulis `AlurMalam7` dan `PembuatMalam7`.
- [ ] **Step 2:** `bash tools/bangun-semua.sh TheSending.EditorTools.PembuatRumahKeluarga.Buat` → `OK`.
- [ ] **Step 3:** `bash tools/run-editmode-tests.sh` → lulus.
- [ ] **Step 4: Commit** kode + `Scene_RumahKeluarga.unity` + `Timeline/*.playable` baru/berubah + `Prefabs/Asli/` dan `Art/Models/Kubus/` yang berubah: `feat: Malam 7 di rumah (lingkaran garam, ritual terakhir, jalur Fitnah)`.

### Task 6: Runtime — AlurWatuireng, LilinPasaran, PembuntutSosok

**Files:**
- Create: `Runtime/AlurWatuireng.cs`, `Runtime/LilinPasaran.cs`, `Runtime/PembuntutSosok.cs`
- Modify: `Runtime/SosokSantet.cs` (`public void KejarLangsung()` memanggil `otak.MulaiKejar()`; aman dipanggil setelah `OnEnable`)

**Interfaces (Produces):**
- `public class LilinPasaran : Interaktif { public Pasaran Pasaran; public event Action<LilinPasaran> Dipadamkan; public void Nyalakan(bool nyala); }` prompt "Padamkan lilin {Pasaran}" (cahaya `Light` anak + nyala visual).
- `public class PembuntutSosok : MonoBehaviour`: menjaga objek Sosok (tanpa NavMesh) sejajar pemain pada `jarakBelakang` (bawaan 16 m) di sisi sawah (`offsetX`), menghadap pemain; mati saat pemain melewati `zBerhenti`. Diam saat dijeda.
- `public class AlurWatuireng : MonoBehaviour` konstanta `SinyalMemergoki = "Memergoki"`.

Perilaku:
- `Start`: kartu hitam lalu `DariHitam`. Jika `PilihanAkhir != Belum && !LolosKejaranWiryo` (dari checkpoint/tertangkap): pindah ke `titikKejar`, langsung `Kejaran(dariCheckpoint: true)`. Selain itu Raka di awal jalan sawah, HUD "Jalan sawah ke Watuireng. Jangan sampai Mbah Wiryo tahu aku datang." dan pembuntut aktif.
- Kandang cemani: zona; saat pemain di dalamnya dan `KokokCemani.Berkokok(jarak)`, paling sering sekali per `JedaDetik` (waktu nyata, saat tidak dijeda): suara kokok, `JamGim.Majukan(s, KokokCemani.Menit)`, HUD "Ayam cemani berkokok ribut! Mbah Wiryo bergerak dalam semadinya."
- Buku catatan (`PemicuDialog` dengan `BukuCatatan`, sekali saja).
- Lilin: `PuzzleUrutan<Pasaran>` dari `LilinSesaji.Buat(7)`; benar = lilin padam; salah = `JamGim.Majukan(s, LilinSesaji.MenitSalah)`, semua lilin menyala lagi, puzzle baru, HUD "Salah urutan. Mbah Wiryo makin gelisah. Ulangi dari Kliwon." Selesai = kendi induk bisa di-E (`KendiInduk`: M7 terfoto otomatis lewat efek `media:M7`).
- Lontar: bisa di-E setelah M7 ditemukan → dialog `Lontar` (pilihan). Setelah dialog: `gm.SimpanCheckpoint()`, lalu `Kejaran(false)`.
- `Kejaran`: cutscene pendek (Timeline `Watuireng_Memergoki`, sekitar 3 detik, kamera ke Wiryo yang berdiri) + dialog `WiryoMemergoki`, lalu Wiryo (`SosokSantet`) aktif di `titikMunculWiryo` dan `KejarLangsung()`, musik kejar, HUD "Lari! Keluar lewat kebun ke gerbang!". Tertangkap: pola `AlurMalam4.Tertangkap` (−5 lewat `gm.Tertangkap()`, muat `gm.SceneUntukState`). Zona gerbang kebun: `PenentuEnding.LolosDariWiryo(s)`; jika `Ending` terisi (Balas Dendam) berhenti (GameManager memuat epilog); jika Lapor: Wiryo mati, `gm.SimpanCheckpoint()`, kartu "Raka berlari pulang menembus sawah. Kendi induk dan buku catatan itu ia dekap erat." (≈5 detik), `MuatScene(gm.SceneUntukState)` (rumah).
- Obat otomatis HUD seperti `AlurMalam7`.

- [ ] **Step 1:** Tulis ketiga kelas dan `KejarLangsung`.
- [ ] **Step 2:** `bash tools/run-editmode-tests.sh` → lulus (kompilasi).
- [ ] **Step 3: Commit** `feat: sutradara Watuireng, lilin sesaji, Sosok membuntuti, Wiryo mengejar`.

### Task 7: Editor — Scene_Watuireng

**Files:**
- Create: `Editor/PembuatWatuireng.cs`
- Modify: `Editor/PembuatMenuUtama.cs` (`BangunSemua` memanggil `PembuatWatuireng.Buat()`; `DaftarkanScene` menambah indeks 7)

Tata letak (koordinat bebas, angka di skrip): jalan sawah tanah lurus sekitar 70 m (z 0..70, lebar 2 m) diapit petak sawah (balok hijau gelap rendah, bahan baru `Sawah` jika perlu) dan batas tak terlihat; kabut lebih tebal (sekitar 0,1); cahaya bulan dingin `6D86A8`. Rumah Wiryo di ujung (sekitar z 75–85): ruang 8 x 6 m dari `Balok` (dinding `KayuGelap`/`Gedek`, atap), pintu depan terbuka, pintu belakang ke kebun. Di dalam: Mbah Wiryo bersemadi (prefab `MbahWiryo` diam, menghadap sesaji), nampan sesaji, lima `LilinPasaran` berderet di depan kendi induk (urutan fisik acak, bukan urutan jawaban), lontar, buku catatan di meja, satu teplok redup. Kandang cemani di samping jalan masuk (dengan `AyamCemani` dan zona). Kebun di belakang (beberapa pohon/rumpun dari `Prefabs/Asli`), pagar batas, gerbang kebun dengan zona lolos menjauh dari rumah. `LantaiNavMesh` mencakup rumah dan kebun. Wiryo pengejar: objek `SosokSantet` + `NavMeshAgent` (pola `PembuatMalam2.Sembunyi`) dengan prefab `MbahWiryo`, titik patroli di kebun, tanpa gemerincing. Sosok pembuntut: prefab `SosokSantet` + `PembuntutSosok`. Rig dari `PembuatRig.Buat`, langkah bawaan `Tanah`. Timeline `Watuireng_Memergoki`. Suara: `KokokCemani`, `Kemenyan` (suasana sanggar, ulang), `MusikKejarWiryo` (cadangan `MusikKejar`/`DroneHoror`), `StingerKejutan`.

- [ ] **Step 1:** Tulis `PembuatWatuireng` (pola `PembuatMakam`).
- [ ] **Step 2:** `bash tools/bangun-semua.sh TheSending.EditorTools.PembuatWatuireng.Buat` → `OK`; lalu `bash tools/bangun-semua.sh TheSending.EditorTools.PembuatMenuUtama.Buat` → `OK` (Build Settings).
- [ ] **Step 3:** `bash tools/run-editmode-tests.sh` → lulus.
- [ ] **Step 4: Commit** kode + `Scenes/Scene_Watuireng.unity(.meta)` + `ProjectSettings/EditorBuildSettings.asset` + `.playable` + aset baru: `feat: Scene_Watuireng (sawah, sanggar Mbah Wiryo, kandang cemani, kebun)`.

### Task 8: Runtime + Editor — Scene_Epilog

**Files:**
- Create: `Runtime/AlurEpilog.cs`, `Editor/PembuatEpilog.cs`
- Modify: `Editor/PembuatMenuUtama.cs` (`BangunSemua`, `DaftarkanScene` indeks 8)

**Interfaces (Produces):**
- `[Serializable] public class BidikanEpilog { public GameObject set; public PemutarTimeline timeline; public bool pagi; public string judul; [TextArea] public string teks; public float detik = 6f; }`
- `AlurEpilog` dengan daftar `BidikanEpilog[] sejati, fitnah, balas, terlambat`; `Suara musik`; `Camera kamera`; `Light cahayaPagi`.

Perilaku `AlurEpilog.Start`: `GameManager` wajib ada; pilih daftar dari `State.Ending` (`Belum` → muat menu). Untuk setiap bidikan: semua set mati kecuali `set`, `pagi` mengatur `RenderSettings.fogColor`/`ambientLight` dan `cahayaPagi`, putar timeline (`PutarDanTunggu`) bersamaan dengan kartu (teks bawah) atau kartu setelahnya (pilih yang paling sederhana dengan `KartuTeks`; kartu boleh berlatar layar terlihat bila `KartuTeks` mendukung, selain itu kartu hitam setelah bidikan). Lalu kartu pesan penutup GS 9.2 persis: "Kepercayaan boleh dihormati, tetapi tuduhan harus dibuktikan. Jangan main hakim sendiri, dan jangan tinggalkan pengobatan medis." Lalu `gm.HapusSave()`, `gm.MulaiBaru()`, `MuatScene(GameManager.SceneMenuUtama)`.

Isi (GS 8.2–8.5; teks Game Story dikutip persis, narasi lain DRAF singkat):
- **Sejati:** (1) kamar Ibu pagi, Ibu meminta air; (2) teras rumah: Bidan Wulan, Pak Joko, Bripka Anton, kutipan Bripka Anton GS; (3) rumah Bu Sumi pagi: Raka meminta maaf; (4) narasi penutup Raka GS; kartu "Pesan:" GS 8.2.
- **Fitnah:** (1) pemakaman di bawah gerimis (makam, payung, pelayat, partikel hujan sederhana), kutipan Pakde GS; (2) Mbah Wiryo di barisan belakang mengusap cincin akik; (3) puing rumah Bu Sumi (`PuingRumahKecil`); kartu "Pesan:" GS 8.3.
- **Balas Dendam:** (1) kartu: Ibu pulih, tiga hari kemudian Pakde ditemukan meninggal (tanpa gambar kekerasan, hanya teks); (2) kamar mandi bercermin: Raka di depan cermin (`Cermin` + `KacaGelap`), Sosok berdiri di belakangnya, suara Wiryo GS (dengan terjemahan); kartu "Pesan:" GS 8.4.
- **Terlambat:** (1) kamar Ibu malam: Raka terduduk; (2) pagi, kamar Laras: kartu Laras memuntahkan segumpal rambut; (3) kartu "Santet belum selesai." GS; kartu "Pesan:" GS 8.5.

Editor `PembuatEpilog`: scene tanpa pemain (GameManager + UI `KartuTeks` + satu kamera PSX + `AudioListener`), set-set kecil berjauhan (x berselang 50 m) memakai prefab yang ada (`Ranjang`, `BuRatmi_Berbaring`, `RumahKeluargaLuar`, `RumahKecil`, `PuingRumahKecil`, `GundukanMakam`, `Kamboja`, `NisanHadi`, tokoh-tokoh, `Payung`, `Cermin`), timeline per bidikan (`Epilog_<Ending>_<n>`, kamera bergerak pelan 4–6 detik), `cahayaPagi` (directional hangat, mati bawaan). Musik `MusikPenutup` (cadangan diam).

- [ ] **Step 1:** Tulis `AlurEpilog` dan `PembuatEpilog`.
- [ ] **Step 2:** `bash tools/bangun-semua.sh TheSending.EditorTools.PembuatEpilog.Buat` → `OK`; `bash tools/bangun-semua.sh TheSending.EditorTools.PembuatMenuUtama.Buat` → `OK`.
- [ ] **Step 3:** `bash tools/run-editmode-tests.sh` → lulus.
- [ ] **Step 4: Commit** `feat: Scene_Epilog dengan empat ending dan pesan penutup`.

### Task 9: Bangun Ulang Semua dan review akhir kode

- [ ] **Step 1:** `bash tools/bangun-semua.sh` → `OK` (jika gagal hanya karena kata "Exception" di nama file pada laporan ukuran build, catat sebagai GAGAL palsu seperti Fase 6; selain itu perbaiki).
- [ ] **Step 2:** `bash tools/run-editmode-tests.sh` → semua lulus.
- [ ] **Step 3:** Review akhir: `git diff 483fbb7..HEAD -- '*.cs'` dengan fokus Review Focus di atas dan daftar `docs/panduan-rencana-fase.md` Bagian 4 (jeda tepat sekali, event unsubscribe, alur macet, state hilang saat ganti scene/"Lanjutkan", `WaitForSeconds`, `static`).
- [ ] **Step 4: Commit** semua hasil regenerasi (scene, `.playable`, `Prefabs/Asli/`, `Art/Models/Kubus/`, `Data/`): `chore: bangun ulang semua scene Fase 7`.
- [ ] **Step 5: BERHENTI** untuk cek Play Mode tim (Daftar Cek di bawah).

### Task 10: Penutup Fase 7

- [ ] **Step 1:** `bash tools/bangun-semua.sh TheSending.EditorTools.PembuatBuild.Windows` (nama metode cek di `PembuatBuild.cs`) → build uji Windows di `Builds/Windows/`.
- [ ] **Step 2:** Perbarui `CLAUDE.md`: baris Fase 7 di tabel, konvensi Malam 7 (scene, sutradara, checkpoint, epilog), catatan untuk Fase 8, Daftar Cek Play Mode Fase 7, log progres bertanggal. Catatan Fase 7 lama dihapus/ditandai selesai.
- [ ] **Step 3: Commit** `docs: Fase 7 selesai di kode`.

## Daftar Cek Play Mode Fase 7 (dikerjakan tim)

Prasyarat: editor dibuka setelah Task 9. Tombol uji F5/F6/F9, Spasi melewati kartu/adegan. Cara cepat: checkpoint 21.00 Malam 6 → selesaikan sidang (berhasil atau gagal).

1. Akhir sidang Malam 6 (berhasil): kartu "Malam 7: Malam Selasa Kliwon", tidak ada "(Bersambung...)". HUD "Malam ini Selasa Kliwon" + "Tengah malam: N menit lagi" (sekitar 25 dari 18.00).
2. Lingkaran garam: kamera ke ranjang, Bu Sumi dan Laras, cincin garam dan kelor di lantai, kutipan Bu Sumi persis.
3. Pintu depan "Berangkat ke Watuireng" → `Scene_Watuireng`: jalan sawah berkabut, Sosok terlihat membuntuti dari jauh dan hilang dekat rumah Wiryo.
4. Mendekati kandang: kokok, jam +5 menit, HUD; tidak berulang lebih cepat dari sekitar 20 detik.
5. Sanggar: Wiryo bersemadi diam. Buku catatan: B8 dan Halaman 8 (Jurnal). Lilin salah: jam +15 menit, lilin menyala lagi. Urutan benar Kliwon, Legi, Pahing, Pon, Wage: lilin padam satu per satu.
6. Kendi induk: M7 di Papan Bukti. Lontar: dua pilihan sesuai GS.
7. Pilih Lapor: cutscene Wiryo memergoki (kutipan persis), Wiryo mengejar, tidak macet di pintu/kebun. Tertangkap: kartu, Watuireng dimuat ulang di titik kejaran, Kondisi −5.
8. Lolos di gerbang kebun: kartu pulang, rumah keluarga, ritual terakhir (api, jeritan, Sosok lenyap), lalu epilog Sejati: 4 bidikan, kutipan Bripka Anton dan narasi Raka persis, pesan penutup, menu utama tanpa "Lanjutkan".
9. Uji terpisah, pilih Balas: kejaran, lolos → epilog Balas Dendam (cermin, Sosok di belakang Raka, suara Wiryo).
10. Uji terpisah: biarkan waktu habis di Watuireng → epilog Terlambat ("Santet belum selesai.").
11. Obat: pukul 21.00 jalur utama HUD "Laras memberi Ibu obat", Kondisi +2 sekali.
12. Jalur Fitnah (sidang gagal): kartu Malam 7 Fitnah, Sosok berpatroli, bisa bersembunyi; pintu depan menolak. F6 sampai 00.00 → epilog Fitnah (pemakaman gerimis, kutipan Pakde, Wiryo di belakang, puing).
13. Terlambat di malam lain (F9 berulang sampai Kondisi 0): langsung epilog Terlambat, tidak kembali ke menu tanpa epilog.
14. Setelah ending apa pun, gerak dan mouse pemain berhenti; kursor terlihat di epilog; Esc tidak membuka menu jeda di epilog.
15. Tidak ada error merah di Console di `Scene_Watuireng`, `Scene_Epilog`, dan rumah Malam 7. Gaya PSX sama; epilog pagi lebih terang tetapi tetap berkabut dan berpiksel.
16. Teks (R5): baca semua teks DRAF Fase 7 dan catat suntingan; minta yang fasih memeriksa bahasa Jawa.
17. Catat: menit nyata Malam 7 jalur utama dari awal sampai ritual (cukup atau tidak), rasa kejaran Wiryo, keterbacaan lilin.

**Jika butir 7 gagal (Wiryo diam, error NavMesh):** perbesar `LantaiNavMesh.ukuran` di `PembuatWatuireng`, geser titik muncul dari dinding. **Jika butir 8/9 tidak ke epilog:** periksa flag `keEpilog` di `GameManager` dan `PetaScene.Untuk`. **Jika timeline beku:** Update Method Unscaled Game Time, wrap None, `removeStartOffset`.

## Self-Review

| Lingkup (spec/DS/GS/CLAUDE.md) | Task |
|---|---|
| Laju jam Malam 7, obat otomatis, Majukan | 2 |
| `PetaScene` Watuireng/Epilog, hapus save, MulaiKejar | 2 |
| Layar ending bersama, input berhenti, ganti "ke menu" | 3 |
| `AlurDesaMalam6.GantiMalam` ke Malam 7, hapus notifikasi `AlurMalam6` | 3 |
| HUD hitung mundur 00.00 | 3 |
| Dialog, Halaman 8, B8, M7, pilihan akhir, Catatan Bapak pasaran | 4 |
| Lingkaran garam, Bu Sumi bergabung, ritual terakhir, `Pelajari(dariBuSumi)` | 5 |
| Jalur Fitnah Malam 7 | 5 |
| Watuireng: sawah + pembuntut, kandang cemani, sanggar, lilin, kendi, lontar, kejaran Wiryo | 6, 7 |
| Empat epilog + pesan penutup, Volume/cahaya pagi | 8 |
| Scene baru di Build Settings | 7, 8 |
| `the-bathroom`, `keris` (K16): dipakai bila sesuai di set cermin/sanggar, selain itu tetap untracked dan dicatat di CLAUDE.md untuk Fase 8 | 7, 8, 10 |
| Build uji Windows, CLAUDE.md, aset Fase 8 | 1, 10 |

Nama yang dipakai lintas task: `JamGim.Majukan`, `ObatMalam7.Cek`, `KokokCemani`, `LilinSesaji.MenitSalah`, `PerilakuSantet.MulaiKejar`, `SosokSantet.KejarLangsung`, `PetaScene.Watuireng/Epilog`, `BerkasSave.Hapus`, `GameManager.HapusSave`, `PembuatDataFase7.Muat`, `AlurMalam7`, `AlurWatuireng`, `AlurEpilog`, `BidikanEpilog`, `LilinPasaran`, `PembuntutSosok`.
