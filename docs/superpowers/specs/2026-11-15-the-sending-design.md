# Desain Pengembangan: The Sending

**Tanggal:** 15 November 2026
**Status:** Disetujui per bagian dalam sesi brainstorming, menunggu tinjauan akhir
**Sumber isi gim:** `docs/TheSending_FullGameStory.md` (Game Story) dan `docs/TheSending_GDD.md` (GDD)

Dokumen ini tidak mengulang isi cerita. Isi cerita, tokoh, angka, dan dialog tetap mengacu ke Game Story dan GDD. Dokumen ini mencatat **cara membangunnya**: batasan, arsitektur, aturan yang diperjelas, fase, testing, dan alur aset. Jika ada yang bertentangan, **dokumen ini yang berlaku** untuk hal-hal di Bagian 4.

---

## 1. Batasan dan Keputusan Utama

| Hal | Keputusan |
|---|---|
| Engine | Unity 6000.6.4f1, URP 17.6, Input System baru (sudah aktif), keyboard dan mouse |
| Target build final | **Hanya Windows** (Standalone, scripting backend Mono) |
| Lingkungan kerja | Mac. Pengetesan harian lewat Play Mode di Mac. Build Mac boleh dibuat **hanya untuk tes internal**, tidak didistribusikan |
| Build Windows | Dibuat dari Mac dengan modul **Windows Build Support (Mono)**, lalu **wajib dites di laptop Windows anggota tim pada Fase 8** |
| Tenggat | 3 minggu, H1 = Senin 16 Nov 2026 sampai H21 = Minggu 6 Des 2026 |
| Tim | Tim mahasiswa (5 orang). Claude menulis kode, tim mencarikan aset dan mengecek Play Mode |
| Pendekatan | **B: per malam secara berurutan, langsung dipoles.** Setiap fase menghasilkan bab yang sudah jadi |
| Isi | **Konten penuh sesuai GDD, tidak dipadatkan.** 8 scene, cutscene dengan Timeline, NPC beranimasi, jam real-time, 3 jenis tempat sembunyi, semua konten opsional |
| Pemotongan | Tidak ada pemotongan otomatis. Jika satu fase melewati jatah harinya, Claude melapor dan tim yang memutuskan |
| Gaya visual | **Retro PSX / low-fi**, dikunci di Fase 0 dan wajib konsisten di semua malam |
| Version control | Git + GitHub, `.gitignore` Unity, serialisasi Force Text (sudah aktif), Git LFS untuk file biner |
| Pengujian | Logika gim diuji dengan EditMode test. Gerak, cahaya, dan suasana dicek manual di Play Mode |
| Prinsip | Utamakan solusi sederhana. Tidak ada abstraksi atau fitur yang tidak diminta |

Catatan istilah: "Malam 1" sampai "Malam 7" adalah **bab cerita di dalam gim**, bukan tahapan proyek. Tahapan proyek disebut **Fase**.

---

## 2. Arsitektur

### 2.1 Struktur Folder

```
Assets/_Project/
├── Scripts/
│   ├── Core/        C# murni: aturan gim, tanpa MonoBehaviour
│   ├── Runtime/     MonoBehaviour: pemain, interaksi, AI, UI, puzzle, cutscene
│   └── Editor/      alat bantu editor (hanya jika perlu)
├── Tests/EditMode/  EditMode test untuk Core
├── Data/            ScriptableObject: bukti, Katanya, halaman buku harian, dialog
├── Scenes/
├── Prefabs/
├── Art/             Models, Textures, Materials, Animations, Fonts
├── Audio/           Music, SFX, Ambience, Dialog
└── Settings/        URP asset, Volume profile, preset impor tekstur
```

### 2.2 Assembly

| Assembly | Isi | Referensi |
|---|---|---|
| `TheSending.Core` | Aturan gim | (tidak ada) |
| `TheSending.Runtime` | Komponen scene | Core |
| `TheSending.Editor` | Alat editor | Core, Runtime (Editor only) |
| `TheSending.Tests.EditMode` | Test | Core, NUnit (Editor only) |

Logika yang diuji hidup di Core, sehingga bisa diuji tanpa scene dan tanpa menekan Play.

### 2.3 Isi Core

Nama domain memakai bahasa Indonesia sesuai GDD agar tim mudah mencocokkan dokumen dengan kode. Istilah teknis tetap bahasa Inggris (`Save`, `Load`, `Event`).

| Kelas | Tanggung jawab |
|---|---|
| `GameState` | Semua variabel cerita dari Game Story bagian 10.2 (`MalamKe`, `KondisiIbu`, `MediaDitemukan/Dinetralkan/Diserahkan[7]`, `BuktiKunci[8]`, `StatusKatanya[4]`, `KepercayaanSumi`, `RitualDiketahui`, `HalamanDiary[8]`, `HasilSidang`, `PilihanAkhir`, `SisaWaktuMalam7`), ditambah status tujuan malam, bahan ritual, dan jam gim |
| `KondisiIbu` | Perubahan nilai sesuai Game Story 6.1, batas 0–100, dan batas minimal 1 di jalur Fitnah |
| `PapanBukti` | Menambah Bukti dan Katanya, mengubah label Katanya menjadi Bukti atau Terbantahkan |
| `KepercayaanSumi` | Nilai 0–3, aksi sekali pakai, pintu tertutup setelah menuduh, ambang 1/2/3 |
| `RitualPenangkal` | Validasi urutan enam langkah, gagal berarti mengulang dari awal |
| `SidangWarga` | Penilaian tiga tuduhan, Diam atau Menuduh langsung gagal |
| `PenentuEnding` | Aturan ending final di Bagian 4.1 |
| `KalenderJawa` | Malam ke-N menjadi hari dan pasaran, urutan paku, urutan lilin |
| `JamGim` | Waktu gim, jeda, kejadian berwaktu, batas waktu |
| `TujuanMalam` | Daftar tujuan wajib dan opsional setiap malam, termasuk kejadian berwaktu |
| `KewaspadaanSantet` | Nilai kewaspadaan dari suara dan cahaya, tingkat kekuatan Sosok Santet |
| Validator puzzle | `GembokLemari` (2003), `AksaraJawa`, `UrutanPaku`, `KerekanSumur` (tempo), `PerbandinganTanah`, `LilinSesaji` |
| `SaveData` | Serialisasi `GameState` ke JSON dan kembali |

Core memberi tahu perubahan lewat event C# (`KondisiBerubah`, `BuktiBaru`, `KatanyaBerubah`, `KepercayaanBerubah`, `JamBerubah`, `EndingDitentukan`).

### 2.4 Runtime

- **`GameManager`**: satu-satunya singleton, `DontDestroyOnLoad`. Menyimpan `GameState` dan memuat scene.
- **`AktifDiMalam`**: komponen yang menyalakan objek hanya pada rentang malam tertentu. Dipakai untuk perubahan isi scene antarmalam.
- **Pemain**: `CharacterController` dan aset `InputSystem_Actions` yang sudah ada, ditambah aksi sesuai kontrol GDD: WASD, Shift lari, mouse, E interaksi/sembunyi, F senter, Q kamera, C kompas, tahan Spasi menahan napas, Tab Jurnal, I inventaris, Esc jeda.
- **Interaksi**: raycast dari kamera ke komponen yang bisa diinteraksi (ambil, buka, ketuk, cabut, inspeksi, bersembunyi).
- **HP**: senter (baterai, diisi di kamar Raka), kamera (memotret ke Papan Bukti), kompas (jarum berputar makin kencang di dekat media).
- **Sosok Santet**: NavMesh dan state machine sederhana (Patroli, Selidiki, Kejar). Inputnya suara langkah dan cahaya senter, dihitung oleh `KewaspadaanSantet` di Core.
- **UI**: uGUI. Jurnal (Papan Bukti, halaman buku harian, Catatan Bapak, Catatan Raka), inventaris, dialog dengan pilihan, notifikasi, kartu teks, menu, jeda.
- **Cutscene**: Timeline. Kamera sinematik dianimasikan langsung di Timeline, tanpa Cinemachine.

### 2.5 Data

ScriptableObject untuk data yang diisi di Inspector tanpa menyentuh kode:
- `DataBukti` (B1–B8) dan `DataKatanya` (K1–K4), termasuk daftar bukti pembantah.
- `DataHalamanDiary` (Halaman 1–8) dan entri Catatan Bapak serta Catatan Raka.
- `DataDialog`: daftar baris (pembicara, teks, terjemahan bahasa Jawa), pilihan, dan efek pilihan (kode efek yang dijalankan Runtime ke Core).

### 2.6 Save

Satu slot. `GameState` disimpan sebagai JSON (`JsonUtility`) ke `Application.persistentDataPath`. Checkpoint otomatis di awal setiap malam dan sebelum setiap sekuens kejar. Menu "Lanjutkan" memuat checkpoint terakhir.

### 2.7 Scene

Sesuai Game Story 10.1, ditambah satu ruangan:

| Scene | Isi |
|---|---|
| `Scene_MainMenu` | Menu utama, pengaturan (audio, grafis, kontrol), peringatan konten |
| `Scene_Desa` | Jalan desa, pekarangan, sumur, pohon asem, pos ronda, bagian luar rumah Bu Sumi dan Pakde |
| `Scene_RumahKeluarga` | Interior rumah, **ditambah kamar mandi dengan cermin** untuk epilog Balas Dendam |
| `Scene_RumahBuSumi` | Interior rumah Bu Sumi |
| `Scene_RumahPakde` | Interior dan halaman rumah Pakde |
| `Scene_Makam` | Makam Desa Sumberwangi |
| `Scene_Watuireng` | Jalan sawah, rumah dan sanggar Mbah Wiryo |
| `Scene_Epilog` | Set-set kecil untuk empat ending, memakai ulang prefab dari scene utama |

### 2.8 Paket yang Ditambahkan

`com.unity.test-framework` (dicantumkan eksplisit di `manifest.json`), `com.unity.ai.navigation`, `com.unity.timeline`, `com.unity.probuilder`.

---

## 3. Gaya Visual (Style Guide Awal)

Nilai di bawah adalah titik awal. Di akhir Fase 0 nilainya dikunci dan disalin ke `docs/style-guide.md`. Setelah dikunci, perubahan hanya boleh atas persetujuan tim.

| Aspek | Nilai awal |
|---|---|
| Resolusi render | URP Render Scale 0.4, Upscaling Filter: Point, anti-aliasing mati |
| Full-screen pass | Shader Graph Fullscreen: kuantisasi warna 5 bit per kanal dan dither Bayer 4×4 |
| Post-processing | Film Grain, Vignette, saturasi -30, tonemapping Neutral |
| Kabut | Exponential Squared, warna `#1A1F24`, kepadatan awal 0.06 (jarak pandang sekitar 15 m) |
| Tekstur | Preset impor otomatis: ukuran maksimal 256, filter Point, tanpa mipmap |
| Cahaya | Realtime, ambient sangat gelap (`#05070A`). Senter adalah spotlight dengan bayangan. Bake lightmap opsional di Fase 8 |
| Palet semantik | Hangat (teplok `#FFB46B`) berarti aman, dingin (bulan `#6D86A8`) berarti ancaman, karat dan darah `#5A1A12`, kayu tua cokelat gelap |
| Font | Dua keluarga: UI dan subtitle memakai **VT323**, buku harian memakai **Kalam** (keduanya Google Fonts, lisensi OFL) |
| Audio | Mixer group: Master, Music, SFX, Ambience, Dialog |
| Siang/pagi | Hanya untuk epilog: Volume profile terpisah, tetap memakai kabut dan pass PSX yang sama |

Cara menjaga konsistensi: preset impor tekstur dan satu Volume profile bersama dipakai semua scene, dan setiap fase ditutup dengan pengecekan gaya terhadap `docs/style-guide.md`.

---

## 4. Aturan yang Diperjelas

Aturan berikut mengisi celah atau menyelesaikan pertentangan di Game Story dan GDD. Semuanya diuji dengan EditMode test.

### 4.1 Penentuan Ending (final)

Dicek sesuai urutan berikut:

1. **Fitnah**: `HasilSidang` adalah Gagal, Diam, atau Menuduh. Hasilnya selalu Fitnah, apa pun yang terjadi sesudahnya. Di jalur ini Kondisi Ibu ditahan minimal 1 sehingga Ibu tetap hidup sampai pukul 00.00 Malam 7, lalu cutscene Fitnah ("Ibu meninggal tepat tengah malam") diputar.
2. **Terlambat**:
   - Kondisi Ibu mencapai 0 sebelum sidang (Malam 1 sampai sidang di Malam 6), langsung saat itu juga.
   - Kondisi Ibu mencapai 0 setelah sidang berhasil (sisa Malam 6 dan Malam 7 jalur utama), langsung saat itu juga.
   - Waktu Malam 7 habis di jalur utama sebelum ritual terakhir selesai.
3. **Balas Dendam**: pemain memilih membacakan mantra pambalik, lalu lolos dari kejaran Mbah Wiryo. Hitung mundur tetap berjalan sampai saat lolos. Jika waktu habis lebih dulu, hasilnya Terlambat.
4. **Sejati**: sidang berhasil, ritual terakhir dengan kendi induk selesai sebelum pukul 00.00, dan pemain memilih jalur laporan.

### 4.2 Tertangkap Sosok Santet

Muat checkpoint terakhir dulu, **lalu** kurangi Kondisi Ibu 5. Penalti tidak hilang karena muat ulang. Jika pengurangan ini membuat Kondisi Ibu 0, berlaku aturan 4.1.

### 4.3 Jam Gim

- Setiap malam dimulai pukul 18.00.
- 1 jam gim = 250 detik nyata (sekitar 4 menit 10 detik), jadi 18.00–00.00 sekitar 25 menit, sesuai Malam 7 di Game Story.
- Jam berhenti saat cutscene, dialog, Jurnal, inventaris, dan menu jeda terbuka.
- **Malam 1–5:** malam berakhir setelah **semua tujuan wajib** selesai. Jam berhenti di 00.30 tanpa hukuman.
- **Malam 6:** batas keras pukul 21.00, warga berkumpul dan Sidang Warga dimulai.
- **Malam 7:** batas keras pukul 00.00.
- Kejadian berwaktu (misalnya cutscene pukul 23.00 di Malam 1) adalah tujuan wajib. Malam tidak bisa berakhir sebelum kejadian itu terjadi.

### 4.4 Obat Ibu

- Jendela pemberian obat: 21.00–22.00. Pengingat lewat notifikasi HP dan dentang jam dinding. Lewat dari 22.00 berarti Laras menegur dan tidak ada +2.
- **Malam 6:** jendelanya dimajukan ke 20.00–21.00.
- **Malam 7 jalur utama:** Laras memberi obat secara otomatis (tetap +2) karena Raka berada di Watuireng.

### 4.5 Bahan Ritual Penangkal

- Jika dipelajari dari Bu Sumi (Kepercayaan minimal 1), Bu Sumi memberi satu ikat bahan.
- Jika dipelajari dari Halaman 4: garam di rak bumbu dapur, daun kelor di pohon pekarangan, bilah bambu kuning di gudang.
- Setelah didapat, bahan bisa dipakai berulang tanpa dihitung.
- Abu dikubur dengan tangan di pekarangan.
- Media yang sudah dinetralkan memberi +8 satu kali per media. Media yang diserahkan kepada Mbah Wiryo tidak bisa dinetralkan.

### 4.6 Kekuatan Sosok Santet

Tingkat kekuatan = jumlah media yang diserahkan + jumlah media dari malam sebelumnya yang belum dinetralkan. Tingkat ini menaikkan kecepatan dan frekuensi kemunculan. Angka per tingkat disetel saat playtest.

### 4.7 Masuknya Petunjuk ke Papan Bukti

- Media santet otomatis terfoto saat ditemukan.
- Petunjuk lain (misalnya paku di Malam 1) harus dipotret manual dengan tombol Q.
- Kesaksian dan dialog masuk otomatis saat dialognya selesai.
- Label Katanya otomatis berubah saat bukti pembantahnya didapat (K1 oleh B2 atau B3, K2 oleh B7, K3 oleh B4 atau B5, K4 menjadi Bukti setelah B6).

### 4.8 Sidang Warga

- Setiap tuduhan dijawab dengan tepat satu item dari Papan Bukti.
- Jawaban benar jika item itu ada di daftar bukti valid untuk tuduhan tersebut (Game Story Malam 6) **dan** berlabel Bukti.
- Minimal 2 dari 3 tuduhan dijawab benar berarti berhasil.
- Memilih "Diam saja" atau "Ikut menuduh" kapan pun membuat sidang langsung gagal (`HasilSidang` Diam atau Menuduh).

### 4.9 Kalender Jawa

Urutan tujuh paku (mulai Jumat) dan urutan lilin sesaji (mulai Kliwon) sudah konsisten dengan penanggalan Jawa. Keduanya dihitung oleh `KalenderJawa` dari `MalamKe`, tidak ditulis manual di kode puzzle.

---

## 5. Daftar Cutscene

| Bab | Momen | Jenis | Fase |
|---|---|---|---|
| Setiap malam | Teks ringkasan siang | Kartu teks | 0 |
| Prolog | Layar hitam, telepon Laras | Kartu teks dan audio | 1 |
| Prolog | Turun dari ojek di gapura | Cutscene penuh (pendek) | 1 |
| Prolog | Bu Sumi di jendela menutup tirai | Adegan berskrip | 1 |
| Prolog | Disambut Laras dan Bidan Wulan | Adegan berskrip (dialog) | 1 |
| Prolog | Ibu menatap kosong, masuk Malam 1 | Cutscene penuh (pendek) | 1 |
| Malam 1 | Pukul 23.00: Ibu kejang, muntah darah dan paku, listrik padam, langkah di atap | Cutscene penuh (tujuan wajib) | 1 |
| Malam 2 | Sosok Santet terlihat lewat celah dinding gudang | Adegan berskrip | 2 |
| Malam 2 | Mbah Wiryo datang, berbisik kepada Laras | Adegan berskrip (dialog) | 2 |
| Malam 3 | Pakde Darmo datang membujuk | Adegan berskrip (dialog) | 3 |
| Malam 3 | Ibu menjerit setiap jam berdentang | Adegan berskrip | 3 |
| Malam 4 | Dua warga menuduh rambut milik Bu Sumi | Adegan berskrip | 4 |
| Malam 4 | Sosok Santet keluar dari sumur, kejaran dimulai | Cutscene penuh (pendek) | 4 |
| Malam 6 | Ibu kerasukan | Cutscene penuh | 6 |
| Malam 6 | Laras berlari masuk pukul 21.00 | Adegan berskrip | 6 |
| Malam 6 | Warga membawa obor di depan rumah Bu Sumi | Cutscene penuh | 6 |
| Malam 6 | Berhasil: Pak Joko menghentikan warga | Cutscene penuh | 6 |
| Malam 6 | Gagal: rumah Bu Sumi dibakar | Cutscene penuh | 6 |
| Malam 7 | Lingkaran garam oleh Bu Sumi dan Laras | Adegan berskrip | 7 |
| Malam 7 | Mbah Wiryo memergoki Raka, kejaran dimulai | Cutscene penuh (pendek) | 7 |
| Malam 7 | Ritual terakhir bersama Bu Sumi | Cutscene penuh | 7 |
| Epilog | Ending Sejati | Cutscene penuh (panjang) | 7 |
| Epilog | Ending Fitnah | Cutscene penuh | 7 |
| Epilog | Ending Balas Dendam | Cutscene penuh | 7 |
| Epilog | Ending Terlambat | Cutscene penuh | 7 |
| Epilog | Pesan penutup gim | Kartu teks | 7 |

Kebutuhan tambahan dari epilog: kamar mandi dengan cermin, model Raka, Volume profile pagi/siang, set puing rumah Bu Sumi, dan latar rumah Pakde.

---

## 6. Fase dan Jadwal

| Fase | Hari | Tanggal | Isi |
|---|---|---|---|
| 0. Fondasi & Style Lock | H1–H3 | 16–18 Nov | Git LFS, modul Windows Build Support (Mono) lewat Unity Hub, paket, folder dan assembly. Seluruh logika Core dengan EditMode test. Kontrol pemain, interaksi, HP (senter, kamera, kompas), Jurnal dan Papan Bukti, sistem dialog, jam gim, save dan checkpoint, kartu teks. Tampilan PSX dan `docs/style-guide.md`. `Scene_MainMenu` dengan peringatan konten |
| 1. Prolog + Malam 1 | H4–H6 | 19–21 Nov | Jalan masuk desa (tutorial), rumah keluarga, NPC Laras, Bidan Wulan, Ibu, obat, gembok 2003, cutscene pukul 23.00 |
| 2. Malam 2 | H7–H8 | 22–23 Nov | Deteksi kompas, gudang dan linggis, M1, puzzle aksara, kunjungan Mbah Wiryo dan pilihannya, AI Sosok Santet, bersembunyi (lemari, kolong ranjang, karung) dan menahan napas |
| 3. Malam 3 | H9–H10 | 24–25 Nov | Pakde datang, Halaman 3 dan 4, mengetuk saka, tujuh paku, `Scene_RumahBuSumi` dan Kepercayaan, Ritual Penangkal |
| 4. Malam 4 | H11–H12 | 26–27 Nov | Pos ronda dan Katanya, kerekan sumur, M3, M4 di plafon, B2, sekuens kejar pertama |
| 5. Malam 5 | H13–H14 | 28–29 Nov | Pohon asem (M5, M6), `Scene_Makam` dan perbandingan tanah, Pak Lasno, kesaksian Laras dan Bu Sumi, `Scene_RumahPakde` (anjing, kunci di pot, B6) |
| 6. Malam 6 | H15–H16 | 30 Nov–1 Des | Kerasukan, timer persiapan, kerumunan obor, Sidang Warga, rumah Bu Sumi terbakar |
| 7. Malam 7 + Ending | H17–H19 | 2–4 Des | `Scene_Watuireng`, ayam cemani, sanggar, B8 dan Halaman 8, lilin sesaji, pilihan mantra, kejaran Mbah Wiryo, ritual terakhir, varian jalur Fitnah, `Scene_Epilog` dengan 4 ending dan pesan penutup |
| 8. Polish & Rilis | H20–H21 | 5–6 Des | Main dari awal sampai akhir, balancing angka, perbaikan bug. Build Windows (Mono) dari Mac, lalu dites di laptop Windows anggota tim (lihat Bagian 7.1) |

**Kriteria selesai setiap fase:**
1. Semua EditMode test lulus.
2. Tim mengecek daftar cek Play Mode yang ditulis Claude untuk fase tersebut (gerak, cahaya, suasana, gaya).
3. Progres dicatat di `CLAUDE.md`.

---

## 7. Testing

- **EditMode test (TDD):** test ditulis sebelum kode untuk semua kelas Core di Bagian 2.3, termasuk tabel lengkap `PenentuEnding`, batas minimal 1 di jalur Fitnah, penalti tertangkap setelah muat checkpoint, jendela obat setiap malam, `KalenderJawa`, dan simpan-muat `SaveData`.
- **Menjalankan test:**
  - Editor tertutup: Claude menjalankan `Unity -batchmode -projectPath "<proyek>" -runTests -testPlatform EditMode -testResults <file>.xml`.
  - Editor terbuka: tim membuka Window > General > Test Runner lalu klik **Run All**.
- **Play Mode (manual oleh tim, di Mac):** setiap fase ditutup dengan daftar cek berisi hal-hal yang bisa diamati, misalnya "kabut membatasi pandangan sekitar 15 m" atau "Sosok Santet bereaksi pada langkah lari dari sekitar 8 m".
- **Build Mac:** boleh dibuat kapan saja untuk tes internal. Tidak didistribusikan dan bukan target rilis.

### 7.1 Tes Build Windows (Fase 8)

Build Windows dibuat dari Mac (File > Build Profiles > Windows, scripting backend Mono), lalu dijalankan di laptop Windows anggota tim. Play Mode di Mac memakai Metal, sedangkan Windows memakai DirectX, jadi hasil tampilan bisa berbeda. Yang wajib dicek:

| Aspek | Yang dicek |
|---|---|
| Tampilan PSX | Render scale rendah tetap kotak-kotak (point), dither dan kuantisasi warna muncul, tidak ada layar hitam atau magenta dari shader full-screen |
| Kabut | Jarak pandang dan warna kabut sama dengan di Mac |
| Cahaya | Senter, lampu teplok, dan ambient tidak lebih terang atau lebih gelap dari di Mac, bayangan senter muncul |
| Resolusi | Layar penuh di resolusi laptop (misalnya 1366×768 dan 1920×1080), UI tidak terpotong, rasio layar berbeda tetap aman |
| Save | File save tercipta di `persistentDataPath` Windows (`%USERPROFILE%\AppData\LocalLow\<Company>\The Sending`), Lanjutkan memuat checkpoint dengan benar setelah gim ditutup dan dibuka lagi |
| Lainnya | Input keyboard dan mouse, audio, dan semua 4 ending bisa dicapai |

Masalah yang ditemukan dicatat di `CLAUDE.md` lalu diperbaiki di Fase 8.

---

## 8. Alur Aset

1. Di awal Fase N, Claude menulis `docs/aset/fase-(N+1).md`. Daftar untuk Fase 0 dan Fase 1 ditulis di awal Fase 0.
2. Setiap aset berisi: nama, kata kunci pencarian (Inggris dan Indonesia), sumber yang disarankan (Unity Asset Store gratis, Sketchfab, Poly Haven, ambientCG, Mixamo, Freesound, Pixabay, Google Fonts), dan spesifikasi singkat (format FBX/WAV/OGG, batas jumlah poligon).
3. Tim mencari aset dan memasukkannya ke `Assets/_Project/Art` atau `Assets/_Project/Audio`.
4. Lisensi setiap aset dicatat di `docs/aset/KREDIT.md`. Aset CC-BY wajib dicantumkan kreditnya di gim.
5. Jika aset tidak ditemukan, Claude membuat pengganti yang menyerupai aslinya: model dengan ProBuilder, tekstur buatan sendiri, dan SFX sintetis sederhana. Pengganti tetap melewati preset PSX supaya gayanya seragam.

---

## 9. Risiko

| Risiko | Penanganan |
|---|---|
| Jadwal 3 minggu sangat padat untuk konten penuh | Batas hari per fase dipantau. Keterlambatan dilaporkan, dan tim yang memutuskan |
| Fase 0 paling berat, padahal semua fase bergantung padanya | Logika Core dikerjakan pertama karena aturannya sudah lengkap dan paling mudah diuji |
| Fase 7 sangat berat (Watuireng, 2 cutscene, 4 epilog) | Set epilog memakai ulang prefab dari scene utama |
| Aset dari banyak sumber membuat gaya tidak seragam | Preset impor PSX, Volume profile bersama, pengecekan gaya di setiap fase |
| Pemain yang selesai lebih cepat harus menunggu kejadian berwaktu (misalnya pukul 23.00 di Malam 1) | Ditinjau saat playtest Fase 1, kecepatan jam bisa disetel |
| Git LFS belum terpasang | Dipasang di awal Fase 0 (`brew install git-lfs` lalu `git lfs install`), baru setelah itu `.gitattributes` dibuat dan aset biner masuk |
| Informasi Pasal 252 KUHP bisa berubah | Dicek ulang di Fase 8 sebelum build |
| Modul Windows Build Support belum terpasang (per 15 Nov hanya ada Mac dan WebGL) | Dipasang lewat Unity Hub di awal Fase 0 (Installs > 6000.6.4f1 > Add modules > Windows Build Support (Mono)) |
| Tampilan berbeda antara Metal (Mac) dan DirectX (Windows) | Shader full-screen memakai Shader Graph (lintas platform). Tes wajib di laptop Windows pada Fase 8 (Bagian 7.1) |
| Laptop Windows tim lebih lemah dari Mac | Gaya PSX sudah ringan karena render scale rendah. Performa ikut dicek saat tes Windows |
