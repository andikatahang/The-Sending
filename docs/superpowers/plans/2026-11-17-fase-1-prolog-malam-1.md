# Fase 1: Prolog + Malam 1 — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Prolog dan Malam 1 bisa dimainkan dari "Mulai Baru" sampai kartu Malam 2: jalan masuk desa (tutorial jalan, lari, senter), Bu Sumi di jendela, rumah keluarga, Laras, Bidan Wulan, Ibu, obat pukul 21.00, gembok lemari 2003, cutscene pukul 23.00, memotret paku, dan pergantian malam. Semua catatan "Fase 1" di `CLAUDE.md` ikut diselesaikan (inventaris, Esc, scene per bab, jam, penjaga dialog, saringan HUD, test data dialog).

**Architecture:** Aturan baru masuk Core dengan TDD (`Inventaris`, `GembokLemari.Buka`, foto petunjuk di `PapanBukti`, jendela obat di `JamGim`, `PetaScene`, `PemeriksaDialog`). Runtime tetap tipis: komponen kecil yang bisa dipakai lagi di malam berikutnya (`ZonaPemicu`, `InteraktifAlur`, `KameraAdegan`, `PemutarTimeline`, `Suara`, `PemberiObat`, `JadwalObat`, `PanelInventaris`, `PanelGembok`) dan satu "sutradara" per bagian cerita (`AlurProlog`, `AlurPrologRumah`, `AlurMalam1`) yang hanya memanggil kelas aturan Core. Tiga cutscene (ojek pergi, Bu Sumi di jendela, pukul 23.00) memakai Timeline yang asetnya dibuat skrip editor (K1). Scene dibuat skrip editor lewat batchmode. Semua percakapan hanya teks (panel dialog dan kartu); suara tokoh ditunda. Semua model dan suara yang belum didapat tim diganti aset pengganti: prefab dari primitif dan WAV sintetis, masing-masing di jalur tetap supaya tim tinggal menukar isinya.

**Tech Stack:** Unity 6000.6.4f1, URP 17.6, Input System 1.20, uGUI 2.6 (legacy `Text`, `TextMesh`), C# .NET Standard 2.1, Unity Test Framework 1.8.0, Timeline 6.6.0 (cutscene, K1).

## RISIKO LINGKUP DAN JADWAL (dibaca tim sebelum mulai)

Jatah Fase 1 adalah 3 hari (H4–H6, 19–21 Nov). Rencana ini berisi 14 task. Tidak ada konten GDD untuk Prolog dan Malam 1 yang dibuang, tetapi hal-hal berikut dikerjakan dalam bentuk sederhana atau ditunda. Tim yang memutuskan apakah itu diterima (aturan `CLAUDE.md`: konten tidak dipotong diam-diam).

| # | Risiko / penyederhanaan | Dampak | Usul |
|---|---|---|---|
| R1 | **Semua aset Fase 1 belum ada** (kolom Status `docs/aset/fase-1.md` kosong, folder `Art/Models` dan `Audio/SFX` belum ada). Seluruh fase memakai pengganti: rumah dan desa dari kotak, tokoh dari sosok kotak-kotak, 5 suara sintetis, sisanya teks pengganti di HUD | Gim bisa dimainkan, tetapi belum terlihat dan terdengar seperti hasil akhir. Menukar model asli + animasi Mixamo adalah pekerjaan tambahan yang tidak masuk 3 hari ini | Tim menukar aset kapan saja lewat jalur di `docs/aset/fase-1.md` (Task 1). Jika penukaran butuh perubahan kode (animasi), laporkan sebagai pekerjaan Fase 8 atau sisipan |
| R2 | **Tokoh tidak beranimasi.** Idle, Talking, Crying, Praying, Seizure belum ada. Kejang Ibu diganti guncangan lewat skrip, Laras berdoa diganti pose berlutut | Cutscene 23.00 kurang hidup | Diterima untuk Fase 1; animasi dipasang saat model Mixamo datang |
| R3 | **Timeline dibuat skrip** (K1). Aset `.playable`, track, dan binding ditulis kode tanpa pernah dibuka di editor, jadi baru terbukti di Play Mode | Cutscene bisa beku, kamera salah posisi, atau sinyal tidak terpicu | Butir 2, 8, dan 23 daftar cek; jalan keluar di bawah daftar cek. Waktu dan bidikan disetel di skrip pembuat, bukan di jendela Timeline |
| R4 | **Waktu tunggu.** Obat pukul 21.00 dan cutscene pukul 23.00 adalah kejadian berwaktu. Pemain yang cepat bisa menunggu sampai belasan menit nyata (risiko yang sudah tercatat di desain Bagian 9) | Malam 1 terasa lambat | Keputusan tim di K4 |
| R5 | **Denah rumah dan jalan desa dikarang** (dokumen tidak punya denah). Mengubah denah setelah fase berikutnya menaruh benda akan mahal | Fase 2–7 menumpang denah ini | Keputusan tim di K7; cek denah di Play Mode sebelum Fase 2 |
| R6 | **Ditunda, tidak dibuat di Fase 1:** tampilan Kondisi Ibu (wajah makin pucat; nilainya baru berubah banyak mulai Malam 2), lampu teplok yang bisa dibawa pemain (di Fase 1 teplok menyala di kamar Ibu saat listrik padam), model HP di tangan (opsional di daftar aset), isi 11 entri Catatan Bapak/Raka lainnya, **suara tokoh** (semua percakapan teks saja; tangis Laras di telepon dan bisikan doa ditandai "belakangan") | Tidak menghalangi Malam 1 | Masuk Fase 2 (Kondisi Ibu, teplok) dan fase yang memakai catatannya |
| R7 | **14 task dalam 3 hari itu padat**, dan setiap putaran perbaikan dari Play Mode butuh editor ditutup dulu (batchmode). Dua pembuat scene (Task 12, 13) paling berisiko molor karena tata letak baru bisa dinilai setelah dimainkan | Fase 1 bisa lewat H6 | Jika H6 siang Task 13 belum selesai, controller melapor ke tim; jangan memotong sendiri |
| R8 | **Style guide masih DRAF** (cek Play Mode 0C belum selesai). Nilai gaya yang berubah sesudahnya cukup dipasang ulang lewat "Bangun Ulang Semua", tetapi butir 4, 15, 16 daftar cek 0C berubah arti setelah Task 6 ("Mulai Baru" tidak lagi memuat sandbox) | Daftar cek 0C harus selesai dulu, atau diganti butir setara di daftar cek Fase 1 (sudah disiapkan) | Lihat Persiapan Tim bagian A |
| R9 | **Target proyek baru: minimal 2 ending selesai dalam 2 minggu.** Fase 1 tidak diubah susunannya untuk target ini | Setiap hari lewat dari H6 memotong waktu untuk ending | Fase 1 wajib selesai dalam H4–H6; jika tidak, controller melapor ke tim saat itu juga |

## KEPUTUSAN TIM (SUDAH DIPUTUSKAN, 18 Nov)

**K1 = Timeline. K2–K8 = A** (rekomendasi). Suara tokoh ditunda: semua percakapan teks saja. Rencana di bawah sudah mengikuti keputusan ini; baris "Jika tim memilih ..." dibiarkan sebagai catatan saja.

**K1. Cutscene: Timeline (DIPUTUSKAN).**
Desain Bagian 2.4: cutscene memakai Timeline. Supaya tetap bisa dibangun lewat batchmode tanpa membuka editor, aset Timeline **dibuat skrip** (`PembuatTimeline`, Task 12) setiap kali scene-nya dibangun ulang.
- **Tiga cutscene memakai Timeline:** `Prolog_OjekPergi` (ojek menjauh, 3,5 detik), `Prolog_BuSumi` (kamera mendekat ke jendela, tirai menutup, 4 detik), `Malam1_Cutscene23` (kejang, paku, listrik padam, potong ke plafon, 11,5 detik). Aset di `Assets/_Project/Timeline/`.
- **Isi track:** `AnimationTrack` untuk gerak kamera adegan dan ojek (kunci dari titik `Bidik...`/`Titik...` di skrip pembuat), dan `SignalEmitter` di marker track timeline untuk kejadian. Sinyal diteruskan `PemutarTimeline` ke sutradara, yang memutar `Suara`, menyalakan benda, dan memadamkan lampu. **`AudioTrack` tidak dipakai** karena klip yang belum ada harus diganti teks di HUD (`Suara`). **`ActivationTrack` tidak dipakai** karena benda yang muncul harus tetap ada sesudah adegan dan sesudah muat save, jadi sutradara yang menyalakannya (`KeadaanSetelahAdegan`).
- **Sutradara memulai timeline dan menunggu akhirnya:** `yield return pemutar.PutarDanTunggu()` (`PlayableDirector.Play`, selesai saat event `stopped`; wrap mode `None`).
- **Jeda: Update Method `Unscaled Game Time`** untuk ketiga director, sesuai `CLAUDE.md`. Alasan: `KameraAdegan.Mulai()` memanggil `GameManager.Jeda(true)` sehingga `Time.timeScale = 0` selama cutscene; dengan `Game Time` timeline akan beku di frame pertama. Tidak bertabrakan dengan desain jeda: `MenuJeda` tidak bisa dibuka selama `Dijeda`, jadi tidak ada keadaan "cutscene dijeda menu" yang harus menghentikan director.
- Adegan yang hanya berisi dialog atau layar memudar (sambutan Laras, Bidan pamit, masuk kamar Ibu, pintu rumah, pergantian malam) tetap coroutine pendek di sutradara; Timeline tidak menambah apa-apa di sana.

**K2. Foto paku Malam 1 masuk Papan Bukti sebagai apa?**
Masalah: Game Story menyebut memotret paku sebagai "tutorial Papan Bukti", dan desain 4.7 mewajibkan foto manual. Tetapi daftar Papan Bukti hanya punya B1–B8 dan K1–K4; paku Malam 1 tidak termasuk. Tanpa tambahan, Papan Bukti tetap kosong setelah tutorialnya.
- **A. Tambah entri berlabel `[FOTO]`.** `GameState` mendapat daftar foto; foto tampil di Papan Bukti tetapi tidak dihitung di Sidang Warga. Kecil (satu task Core), dan petunjuk yang dipotret di malam lain bisa memakai jalur yang sama.
- B. Tidak ada entri. Memotret hanya menyelesaikan tujuan `PotretPaku` dan memunculkan notifikasi. Paling sedikit kode, tetapi tutorialnya tidak menunjukkan apa-apa.
- **Rekomendasi: A.**
- Jika tim memilih B: lewati Task 3, hapus baris `case Perubahan.Foto` di `Hud` (Task 6), lewati Task 7 Step 4 (baris `[FOTO]` di Jurnal), hapus `; foto:PakuMalam1` dari efek paku di Task 13, dan kurangi 7 dari setiap jumlah test sesudah Task 2 (akhirnya 254, bukan 261).

**K3. Esc menutup panel?**
Masalah: sekarang Esc tidak berbuat apa-apa saat Jurnal atau dialog terbuka. Panel baru (inventaris, gembok) butuh aturan yang sama dengan Jurnal.
- **A. Esc menutup Jurnal, inventaris, dan panel gembok** (tombol aslinya, Tab dan I, tetap menutup juga). Esc **tidak** menutup dialog, karena dialog membawa efek dan pilihan yang tidak boleh terlewat. Esc yang menutup panel tidak ikut membuka menu jeda.
- B. Esc tidak berbuat apa-apa di panel (seperti sekarang). Pemain yang terbiasa Esc akan bingung.
- C. Esc membuka menu jeda di atas panel. Dua lapis jeda, lebih rumit.
- **Rekomendasi: A.**
- Jika tim memilih B: hapus pemeriksaan `JedaMenu.WasPressedThisFrame()` dari `PanelJurnal`, `PanelInventaris` (Task 6 dan 7), dan `PanelGembok` (Task 7; gembok lalu ditutup dengan E).

**K4. Menunggu pukul 21.00 dan 23.00.**
Masalah: 1 jam gim = 250 detik nyata. Dari tiba di rumah (sekitar 18.30–19.00) sampai 21.00 kira-kira 8 menit nyata, lalu 8 menit lagi sampai 23.00. Isi Malam 1 (menjelajah, bicara dengan Laras, foto keluarga, blandar, gembok) kira-kira 6–10 menit. Pemain cepat akan menunggu.
- **A. Ukur dulu.** Tidak ada fitur baru. Daftar cek Play Mode meminta tim mencatat berapa menit menunggu. Untuk uji, `AlatUji` mendapat tombol **F6 = jam gim +30 menit** (hanya editor dan development build).
- B. Tambah "duduk menunggu" di kursi samping ranjang Ibu: jam melompat ke kejadian berikutnya. Kira-kira 30 baris + test, tetapi menambah aturan yang belum ada di desain.
- C. Percepat jam (ubah `JamGim.DetikNyataPerJamGim`). Memengaruhi Malam 7 yang sudah dihitung 25 menit.
- **Rekomendasi: A**, sesuai desain Bagian 9 ("ditinjau saat playtest Fase 1"). Jika hasil ukur terasa lama, B dikerjakan sebagai sisipan kecil.
- Jika tim memilih B sekarang: controller menyisipkan satu task Core (TDD) + satu komponen Runtime setelah Task 8, dan jadwal bertambah sekitar setengah hari. Jika C: controller melapor dulu, karena perhitungan Malam 7 di desain ikut berubah.

**K5. Teks yang tidak ada di dokumen.**
Masalah: Game Story hanya memberi "Dialog Kunci". Beberapa teks yang dibutuhkan adegan tidak ada kalimatnya. Yang **dikutip persis**: Tukang Ojek, ketiga kalimat Bidan Wulan dan Raka, "Hadi... ojo didol...", tulisan di balik foto, dua kalimat penutup Laras dan Raka, tujuan bab, ringkasan siang Malam 2. Yang **ditulis Claude** (ditandai di Task 11): teks telepon Laras, sambutan Laras, kronologi sakit Ibu, adegan Ibu di Prolog, teguran obat, isi Halaman 1 dan 2, keterangan barang inventaris, judul foto paku, semua teks tutorial dan notifikasi, serta isi dua Catatan Raka (tanda dehidrasi; kapan dirujuk ke rumah sakit).
- **A. Pakai draf Claude sekarang, tim menyunting di `PembuatDataFase1.cs`** lalu menjalankan "Bangun Ulang Semua". Dua Catatan Raka berisi informasi kesehatan dasar; tim memeriksa isinya.
- B. Tim menulis semua teks dulu. Menghalangi Task 11.
- **Rekomendasi: A.**

**K6. Assembly test mereferensikan Runtime.**
Masalah: `CLAUDE.md` meminta test yang memeriksa semua aset `DataDialog`, tetapi `DataDialog` ada di assembly Runtime dan assembly test hanya mereferensikan Core (konvensi `CLAUDE.md`).
- **A. Tambah referensi `TheSending.Runtime` ke assembly test**, hanya untuk test data. Aturan gim tetap diuji lewat Core. Konvensi di `CLAUDE.md` diperbarui (Task 14).
- B. Baca aset lewat `SerializedObject` tanpa referensi. Konvensi utuh, kode test lebih panjang dan rapuh terhadap nama field.
- **Rekomendasi: A.**
- Jika tim memilih B: Task 5 Step 7–8 diganti versi `SerializedObject`; laporkan dulu ke controller.

**K7. Denah.**
Masalah: dokumen menyebut ruangan (ruang tengah dengan empat saka guru, kamar Ibu, kamar Raka, kamar Laras, dapur, gudang, kamar mandi) tetapi tidak ada denah. Gudang juga tidak jelas di dalam atau di luar rumah.
- **A. Denah rencana ini:** rumah 14 × 12 m. Ruang tengah di tengah (6 × 8 m, pintu depan di selatan). Barat: kamar Raka (depan) dan kamar Ibu (belakang). Timur: kamar mandi (depan) dan kamar Laras (belakang). Utara: dapur, dan **gudang menempel di sisi timur dapur** dengan pintu terkunci sampai Fase 2. Jalan desa: lurus 100 m dari gapura, kebun bambu, jembatan bambu, rumah Bu Sumi di kanan, lalu rumah keluarga. Angkanya ada di Task 12 dan 13.
- B. Tim menggambar denah sendiri. Menghalangi Task 12 dan 13.
- **Rekomendasi: A**, lalu tim menilai di Play Mode (butir daftar cek). Scene dibuat skrip, jadi menggeser dinding berarti mengubah angka lalu membangun ulang.

**K8. Kabut Prolog.**
Masalah: Game Story: "Azan magrib terdengar dari kejauhan, lalu kabut turun." Style guide mengunci kepadatan kabut 0.06.
- **A. Prolog dimulai dengan kabut tipis 0.02, lalu naik ke 0.06 dalam 20 detik setelah azan.** Sesudah itu semua scene memakai 0.06 seperti style guide. Dicatat sebagai pengecualian di style guide (Task 14).
- B. Kabut 0.06 sejak awal. "Kabut turun" hanya lewat teks.
- **Rekomendasi: A.**
- Jika tim memilih B: set `AlurProlog.KabutSebelumMagrib` sama dengan 0.06.

## Urutan Eksekusi (wajib)

1. **BERHENTI.** K1–K8 sudah diputuskan (lihat di atas). Controller meminta tim: (a) menyelesaikan Daftar Cek Play Mode 0C jika belum (Persiapan Tim A), (b) **menutup editor Unity**. Batchmode (semua test dan semua pembuat scene) tidak bisa jalan selama editor membuka proyek. Controller melanjutkan hanya setelah tim menjawab.
2. Controller membuat branch `fase-1` dari `main`.
3. **Task 1–13** dikerjakan berurutan, editor tetap tertutup. Tim boleh menaruh file suara atau menukar prefab (Persiapan Tim B dan C) kapan saja **lewat Finder dan git**, tanpa membuka editor. Jika tim harus membuka editor (misalnya menukar isi prefab), controller berhenti di batas task sampai editor ditutup lagi.
4. **BERHENTI setelah Task 13.** Controller meminta tim menjalankan Daftar Cek Play Mode Fase 1 (di akhir dokumen ini), lalu menutup editor. Perbaikan dari hasil cek dikerjakan sebagai commit `fix:` sebelum Task 14.
5. **Task 14** (dokumen, build uji Windows, `CLAUDE.md`).

**Model eksekutor:** **sonnet** untuk task yang kodenya sudah lengkap di rencana (Task 1, 2, 3, 4, 5, 7, 10, 11, 14); **opus** untuk task yang banyak menyambung kode lama dan scene (Task 6, 8, 9, 12, 13).

## Global Constraints

- Unity **6000.6.4f1**, URP 17.6. Target build final **hanya Windows** (Mono). Pengembangan dan tes harian di Mac.
- Core: `Assets/_Project/Scripts/Core`, namespace `TheSending.Core`. **Tanpa MonoBehaviour.** TDD: test dulu. Runtime: `Assets/_Project/Scripts/Runtime`, namespace `TheSending.Runtime`. Editor: `Assets/_Project/Scripts/Editor`, namespace `TheSending.EditorTools`. Test: `Assets/_Project/Tests/EditMode`, namespace `TheSending.Tests`.
- **`GameState` hanya diubah lewat kelas aturan Core.** Runtime tidak pernah menulis field `GameState` langsung.
- Hanya satu singleton: `GameManager`. Tidak ada field `static` baru di Runtime. Nama domain bahasa Indonesia, istilah teknis bahasa Inggris.
- **Jeda dan adegan:** setiap adegan memanggil `KameraAdegan.Mulai()` yang menjeda (`Time.timeScale = 0`) dan menyalakan `GameManager.Sinematik`. Cutscene memakai Timeline lewat `PemutarTimeline.PutarDanTunggu()`; setiap `PlayableDirector` memakai Update Method **Unscaled Game Time**, wrap mode **None**, Play On Awake mati (disetel `PembuatTimeline`). Gerak lain di dalam adegan memakai `Time.unscaledDeltaTime`. Jangan memakai `WaitForSeconds` di adegan.
- **KartuTeks:** `Tampilkan`, `KeHitam`, `DariHitam` selalu dijalankan lewat `kartu.StartCoroutine(...)` dan ditunggu satu per satu. Jangan menumpuk dua fade.
- **Jam gim:** 1 jam gim = **250** detik nyata. Prolog mulai **17.30**. Jam hanya maju saat `KunciKursor && !Dijeda`. Jendela obat **21.00–22.00** (Malam 6: 20.00–21.00), obat **+2**. Cutscene Malam 1 pukul **23.00** (`JamGim.Jam23`). Kode gembok **`2003`** (`GembokLemari.KodeBenar`). Kondisi Ibu awal **60**, pergantian malam **-12** (tidak berlaku dari Prolog ke Malam 1).
- **Tujuan wajib:** Prolog `TibaDiRumah`, `BicaraBidan`, `MasukKamarIbu`. Malam 1 `BukaLemari`, `Cutscene23`, `PotretPaku`. Malam berganti hanya lewat `PergantianMalam.Lanjut`, dan `State.Ending` dicek sesudahnya.
- **Checkpoint:** saat "Mulai Baru", saat tiba di rumah (pindah scene), di awal Malam 1, dan di awal Malam 2.
- **Scene:** `Scene_MainMenu` (indeks 0), `Scene_Desa` (1), `Scene_RumahKeluarga` (2), `Scene_Sandbox0B` (3). Scene awal ditentukan `PetaScene.Untuk(state)`; `GameManager.SceneAwal` dihapus.
- **Gaya PSX:** setiap pembuat scene memanggil `PembuatGayaPsx.TerapkanKeScene()` dan `SiapkanKamera()` untuk **kedua** kamera (pemain dan adegan), dan memasang `MenuJeda` + `PanelPengaturan`. Kabut `#1A1F24` kepadatan **0.06** (Prolog mulai **0.02**, K8). Palet: teplok `#FFB46B`, bulan `#6D86A8`, karat/darah `#5A1A12`. Lampu listrik rumah `#FFD9A0`.
- **Pembuat scene:** semua aset dibuat atau dimuat **setelah** `EditorSceneManager.NewScene` (pelajaran 0B). Penyetel field (`PembuatSandbox.Set...`) melempar exception jika nilai null atau nama field salah.
- **Aset pengganti:** material di `Assets/_Project/Art/Materials/Pengganti/`, prefab di `Assets/_Project/Prefabs/Pengganti/`, suara di `Assets/_Project/Audio/SFX/` dan `Assets/_Project/Audio/Music/`. Skrip **tidak menimpa** file yang sudah ada di jalur itu. Azan **tidak pernah** dibuat sintetis.
- **Audio:** setiap `AudioSource` diarahkan ke grup `MixerUtama` (`SFX`, `Ambience`, `Music`, `Dialog`). **Tidak ada suara tokoh di Fase 1** (voice acting ditunda): percakapan hanya teks di panel dialog dan kartu, jadi grup `Dialog` belum dipakai.
- **ProjectSettings:** batchmode Unity menghapus define `SENTIS_ANALYTICS_ENABLED` dari `ProjectSettings/ProjectSettings.asset`. Setelah **setiap** run batchmode jalankan `git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset`. Penghapusan itu **tidak pernah** di-commit. Satu-satunya file ProjectSettings yang boleh berubah di rencana ini adalah `EditorBuildSettings.asset` (Task 13).
- Jangan `git add -A`. Tambahkan hanya jalur yang disebut di langkah commit.
- Commit: `<type>: <deskripsi>`, diakhiri baris `Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ`. **Jangan pernah menambahkan `Co-Authored-By`.**

## Cara Menjalankan Test dan Skrip Editor

- Editor Unity harus tertutup. `bash tools/run-editmode-tests.sh` (semua) atau `bash tools/run-editmode-tests.sh TheSending.Tests.NamaKelasTests`. Timeout Bash 600000 ms.
- Suite sekarang **223** test. Setelah Task 2: **238**. Task 3: **245**. Task 4: **253**. Task 5: **261**. Task 6–14 tidak menambah test.
- RED untuk kelas atau anggota baru berupa compile error (`error CS`). Itu gagal yang diharapkan.
- Kode Runtime dan Editor tidak punya EditMode test. Verifikasinya: seluruh suite lulus (semua assembly terkompilasi), run batchmode pembuat scene, dan daftar cek Play Mode tim.
- Skrip editor: `"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod <Metode> -logFile <log>`.
- Setelah setiap run: perintah `git diff --quiet ... || git checkout ...` di Global Constraints.

## Struktur File

```
Assets/_Project/Scripts/Core/
  Inventaris.cs               barang yang dibawa Raka: id, nama, keterangan, Tambah/Punya (baru)
  PetaScene.cs                scene awal untuk sebuah GameState (baru)
  PemeriksaDialog.cs          pemeriksa data dialog: efek, lompatan, jumlah pilihan (baru)
  GameState.cs  Enums.cs      + Barang, Foto, Perubahan.Barang/Foto (modify)
  Puzzle.cs                   + GembokLemari.Buka (modify)
  PapanBukti.cs               + foto petunjuk (modify)
  JamGim.cs                   + MulaiJendelaObat, AkhirJendelaObat (modify)
  EfekDialog.cs               + efek barang:, foto:, dan Periksa (modify)
  SaveData.cs                 + validasi Barang dan Foto (modify)
Assets/_Project/Scripts/Runtime/
  GameManager.cs              SceneUntukState, Sinematik, jam hanya saat pemain dikendalikan (modify)
  MenuUtama.cs  MenuJeda.cs  AlatUji.cs  Hud.cs  PanelDialog.cs  PanelJurnal.cs   catatan Fase 1 (modify)
  PemainGerak.cs  KartuTeks.cs   + Pindah, + HitamSeketika (modify)
  PanelInventaris.cs          tombol I (baru)
  PanelGembok.cs  LemariAyah.cs   gembok 4 angka dan lemari ayah (baru)
  ZonaPemicu.cs  InteraktifAlur.cs  KameraAdegan.cs  Suara.cs   alat adegan (baru)
  PemutarTimeline.cs          memutar satu cutscene Timeline dan meneruskan sinyalnya (baru)
  TheSending.Runtime.asmdef   + referensi Unity.Timeline (modify)
  TerlihatSaatDisorot.cs      ukiran blandar yang terbaca saat disorot senter (baru)
  PemberiObat.cs  JadwalObat.cs   obat Ibu dan pengingatnya (baru)
  AlurProlog.cs  AlurPrologRumah.cs  AlurMalam1.cs   sutradara cerita (baru)
Assets/_Project/Scripts/Editor/
  PembuatSandbox.cs           penyetel field dibuat internal (modify)
  PembuatPengganti.cs         material, prefab pengganti, pencari klip (baru)
  PembuatSuaraPengganti.cs    lima WAV sintetis (baru)
  PembuatDataFase1.cs         aset dialog Fase 1 dan isi Jurnal (baru)
  PembuatRig.cs               pemain + HP + UI yang sama di setiap scene gameplay (baru)
  PembuatTimeline.cs          aset Timeline cutscene: track gerak, sinyal, director (baru)
  TheSending.Editor.asmdef    + referensi Unity.Timeline (modify)
  PembuatDesa.cs              Scene_Desa (baru)
  PembuatRumahKeluarga.cs     Scene_RumahKeluarga (baru)
  PembuatMenuUtama.cs         daftar scene dan "Bangun Ulang Semua" (modify)
Assets/_Project/Tests/EditMode/
  TheSending.Tests.EditMode.asmdef   + referensi TheSending.Runtime (modify, K6)
  InventarisTests.cs  PetaSceneTests.cs  PemeriksaDialogTests.cs  DataDialogTests.cs   (baru)
  EfekDialogTests.cs  PuzzleTests.cs  SaveDataTests.cs  PapanBuktiTests.cs  JamGimTests.cs   (modify)
Dibuat skrip (ikut di-commit):
  Assets/_Project/Scenes/Scene_Desa.unity, Scene_RumahKeluarga.unity
  Assets/_Project/Data/Fase1/*.asset            10 dialog
  Assets/_Project/Timeline/*.playable           3 cutscene; Timeline/Sinyal/*.asset (6 SignalAsset)
  Assets/_Project/Art/Materials/Pengganti/*.mat
  Assets/_Project/Prefabs/Pengganti/*.prefab
  Assets/_Project/Audio/SFX/*.wav, Audio/Music/DroneHoror.wav   (hanya yang sintetis)
docs/aset/fase-2.md (baru), docs/aset/fase-1.md, docs/style-guide.md, CLAUDE.md (modify)
```

## Persiapan Tim

Tidak ada pekerjaan editor yang wajib seperti Shader Graph di 0C. Yang ada:

### A. Sebelum Task 1 (BLOCKING)

1. ~~Jawab K1–K8~~ Sudah diputuskan 18 Nov (K1 Timeline, K2–K8 A).
2. Selesaikan **Daftar Cek Play Mode Fase 0C** di `CLAUDE.md` jika belum, terutama butir 4, 15, dan 16 (setelah Task 6, "Mulai Baru" memuat Prolog, bukan sandbox). Jika tim memilih tidak menyelesaikannya dulu, katakan ke controller; butir save yang setara sudah ada di daftar cek Fase 1 (butir 27–29).
3. Simpan semua, lalu **tutup editor Unity**.

### B. Suara (tidak menghalangi; kapan saja)

Taruh file di jalur dan nama persis di bawah (`.wav`, `.ogg`, atau `.mp3`), lalu catat lisensinya di `docs/aset/KREDIT.md`. Jika file belum ada saat scene dibuat, adegan menampilkan teks pengganti di HUD, atau memakai WAV sintetis untuk lima suara bertanda (S). Untuk mengganti suara sintetis: hapus file `.wav` sintetis beserta `.meta`-nya, taruh file asli dengan nama dasar yang sama. Setelah menaruh file, minta controller menjalankan "Bangun Ulang Semua" (atau jalankan sendiri menu **The Sending > Bangun Ulang Semua** lalu tutup editor).

| Nama file (tanpa akhiran) | Folder | Isi |
|---|---|---|
| `MotorMenjauh` | `Assets/_Project/Audio/SFX/` | motor ojek pergi |
| `AzanMagrib` | `Assets/_Project/Audio/SFX/` | azan magrib dari kejauhan, rekaman utuh. **Tidak dibuat sintetis** |
| `PintuBerderit` | `Assets/_Project/Audio/SFX/` | pintu kayu berderit |
| `DentangJam` (S) | `Assets/_Project/Audio/SFX/` | jam dinding berdentang |
| `BatukTersedak` | `Assets/_Project/Audio/SFX/` | perempuan batuk dan tersedak |
| `PakuJatuh` (S) | `Assets/_Project/Audio/SFX/` | paku jatuh ke lantai |
| `ListrikPadam` (S) | `Assets/_Project/Audio/SFX/` | listrik padam |
| `LangkahDiAtap` (S) | `Assets/_Project/Audio/SFX/` | langkah berat di atap |
| `DroneHoror` (S) | `Assets/_Project/Audio/Music/` | drone horor (loop) |

Suara tokoh (tangis Laras di telepon, bisikan doa Laras) **belakangan**: voice acting ditunda, tidak dipakai kode Fase 1.

### C. Model (tidak menghalangi; kapan saja, butuh editor)

Setiap benda dan tokoh di scene adalah **objek logika** (collider + skrip, dibuat skrip) dengan **prefab visual** sebagai anak. Untuk menukar model:

1. Minta controller berhenti di batas task, lalu buka editor.
2. Buka prefab di `Assets/_Project/Prefabs/Pengganti/` (daftar di `docs/aset/fase-1.md` setelah Task 1), hapus anak-anak primitifnya, seret model asli sebagai anak, samakan ukuran dan arah (depan = sumbu +Z, titik nol di lantai). **Jangan mengganti nama atau memindahkan file prefab.**
3. Tokoh beranimasi: pasang `Animator` di model dan set **Update Mode = Unscaled Time** (adegan berjalan saat gim dijeda). Jangan menyunting aset di `Assets/_Project/Timeline/`: ditimpa setiap "Bangun Ulang Semua".
4. Simpan, **tutup editor**, beri tahu controller. Scene tidak perlu dibangun ulang; pembangunan ulang juga tidak menimpa prefab yang sudah ada.

Jangan menyunting `Scene_Desa` atau `Scene_RumahKeluarga` dengan tangan: keduanya ditimpa setiap "Bangun Ulang Semua". Perubahan tata letak dilakukan di skrip pembuatnya.

### D. Setelah Task 13 (BLOCKING untuk menutup fase)

Jalankan **Daftar Cek Play Mode Fase 1** di akhir dokumen ini, catat hasilnya, **tutup editor**, beri tahu controller.

---

### Task 1: Daftar aset Fase 2 dan nama file aset Fase 1

Aturan alur aset: di awal Fase N ditulis `docs/aset/fase-(N+1).md`. Task ini juga memberi tahu tim nama file dan prefab yang dipakai kode Fase 1, supaya penukaran aset tidak butuh perubahan kode.

**Files:**
- Create: `docs/aset/fase-2.md`
- Modify: `docs/aset/fase-1.md` (tandai dua suara tokoh "belakangan", tambah satu bagian di akhir)

**Interfaces:**
- Consumes: Game Story bab 7 "Malam 2", format `docs/aset/fase-1.md`.
- Produces: nama file suara dan nama prefab yang sama persis dengan Task 10 (`PembuatPengganti`, `PembuatSuaraPengganti`).

- [ ] **Step 1: Tulis `docs/aset/fase-2.md`**

```markdown
# Daftar Aset Fase 2 (Malam 2: Boneka di Bawah Lantai)

Dikerjakan 22–23 Nov. Cari mulai sekarang. Aturan sama dengan `fase-0.md` dan `fase-1.md`: catat lisensi di `KREDIT.md`, beri ✗ jika tidak ketemu (Claude membuat penggantinya).

Model: utamakan **low poly** (di bawah 5.000 segitiga per benda, karakter di bawah 15.000), format FBX atau GLB. Gaya PSX akan menyamakan tampilannya.

Sumber: **SF** Sketchfab (filter *Downloadable*, lisensi CC0 atau CC-BY) · **AS** Unity Asset Store (gratis) · **MX** Mixamo (karakter + animasi) · **PH** Poly Haven · **AC** ambientCG · **FS** Freesound · **PX** Pixabay · **GF** Google Fonts

## Lingkungan

| Nama | Kata kunci (EN / ID) | Sumber | Tujuan | Status |
|---|---|---|---|---|
| Isi gudang: alat tani (pacul, sabit, garu) | `farm tools low poly`, `hoe`, `sickle`, `rake` / pacul, sabit | SF, AS | `Art/Models/Gudang/` | |
| Tumpukan karung goni (tempat sembunyi) | `burlap sack pile`, `grain sacks` / karung goni | SF, AS | `Art/Models/Gudang/` | |
| Dinding anyaman bambu bercelah | `woven bamboo wall`, `bamboo fence gap` / gedek | SF, AC | `Art/Models/Gudang/` | |
| Papan lantai lepas (bisa dibongkar) | `loose floorboard`, `wooden plank` / papan lantai | SF | `Art/Models/RumahKeluarga/` | |
| Kotak kayu kecil (penyimpan boneka di kamar Raka) | `small wooden box`, `old chest small` / kotak kayu | SF, AS | `Art/Models/RumahKeluarga/` | |
| Tekstur: karung goni, kayu lapuk, tanah kolong rumah | `burlap`, `rotten wood`, `dry soil` | PH, AC | `Art/Textures/` | |

## Benda

| Nama | Kata kunci (EN / ID) | Sumber | Status |
|---|---|---|---|
| Linggis | `crowbar` / linggis | SF, AS | |
| Kunci gudang | `old key`, `rusty key` / kunci tua | SF | |
| Boneka kain putih kumal berlilit benang merah, tertusuk jarum (Media 1) | `voodoo doll`, `cloth doll pins`, `ragdoll creepy` / boneka santet | SF | |
| Jarum | `sewing needle`, `pins` / jarum | SF | |
| Kalender dinding dengan pasaran Jawa | `wall calendar` (tekstur dibuat sendiri) / kalender Jawa | SF | |

## Font

| Nama | Kata kunci | Sumber | Tujuan | Status |
|---|---|---|---|---|
| Aksara Jawa (puzzle membaca aksara, tabel Halaman 2) | `Noto Sans Javanese` | GF | `Assets/_Project/Art/Fonts/Resources/NotoSansJavanese-Regular.ttf` (nama file harus persis) | |

## Karakter (model + animasi)

| Tokoh | Kata kunci | Animasi Mixamo yang dibutuhkan | Status |
|---|---|---|---|
| Mbah Wiryo (68, berwibawa, pakaian Jawa gelap, blangkon) | `old man character`, `elderly asian man`, `javanese man blangkon` | Idle, Talking, Walking, Whisper atau Leaning | |
| Sosok Santet (2,2 m, kurus, hitam arang, kain kafan kotor, rambut panjang menutup wajah) | `ghost woman long hair`, `shrouded figure`, `pocong low poly`, `tall creature horror` | Zombie Walk atau Creeping, Idle, Scream atau Attack | |
| Laras (animasi tambahan) | (model Fase 1) | Scared atau Trembling, Handing Object | |

## Audio

| Nama | Kata kunci (EN / ID) | Sumber | Catatan | Status |
|---|---|---|---|---|
| Papan kopong diinjak | `hollow wood knock`, `creaky floorboard` | FS | papan lantai kamar Ibu | |
| Papan dibongkar dengan linggis | `wood prying`, `crowbar wood crack` | FS | | |
| Pintu gudang berderit | `old shed door creak` | FS | beda dari pintu rumah | |
| Kunci dan gembok dibuka | `key unlock`, `padlock open` | FS | | |
| Gemerincing paku (Sosok Santet bergerak) | `metal nails jingle`, `chains rattle soft` | FS | ciri khas Sosok Santet | |
| Langkah Sosok Santet | `dragging footsteps`, `slow heavy steps wood` | FS | patroli ruang tengah | |
| Stinger kejutan | `horror sting`, `jump scare hit` | FS, PX | Sosok terlihat lewat celah gudang | |
| Napas tertahan dan detak jantung | `holding breath`, `heartbeat slow` | FS | saat bersembunyi | |
| Ketukan pintu depan | `knock wooden door` | FS | Mbah Wiryo datang | |
| Bisikan laki-laki tua | `old man whisper` | FS | Mbah Wiryo kepada Laras | belakangan (suara tokoh ditunda; Fase 2 memakai teks) |
| Musik tegang bersembunyi | `stealth tension loop`, `horror suspense loop` | PX, FS | loop | |
| Gerimis di atap | `light rain on roof` | FS, PX | ambience malam | |
```

- [ ] **Step 2: Tambahkan bagian nama file ke akhir `docs/aset/fase-1.md`**

```markdown

## Nama file dan prefab yang dipakai kode Fase 1

Kode Fase 1 berjalan dengan aset pengganti. Untuk memakai aset asli, taruh atau tukar di jalur berikut; tidak ada kode yang perlu diubah. Skrip pembuat **tidak menimpa** file yang sudah ada.

**Suara** (`.wav`, `.ogg`, atau `.mp3`; nama dasar harus persis). (S) = sudah ada WAV sintetis; hapus file sintetis dan `.meta`-nya sebelum menaruh yang asli.

| Aset di daftar atas | Jalur |
|---|---|
| Motor menjauh | `Assets/_Project/Audio/SFX/MotorMenjauh` |
| Azan magrib | `Assets/_Project/Audio/SFX/AzanMagrib` |
| Pintu kayu berderit | `Assets/_Project/Audio/SFX/PintuBerderit` |
| Jam dinding berdentang (S) | `Assets/_Project/Audio/SFX/DentangJam` |
| Perempuan batuk dan tersedak | `Assets/_Project/Audio/SFX/BatukTersedak` |
| Paku jatuh ke lantai (S) | `Assets/_Project/Audio/SFX/PakuJatuh` |
| Listrik padam (S) | `Assets/_Project/Audio/SFX/ListrikPadam` |
| Langkah berat di atap (S) | `Assets/_Project/Audio/SFX/LangkahDiAtap` |
| Musik drone horor (S) | `Assets/_Project/Audio/Music/DroneHoror` |

**Prefab** di `Assets/_Project/Prefabs/Pengganti/` (buka prefab, ganti anak-anaknya dengan model asli; depan = +Z, titik nol di lantai; jangan ganti nama file):

| Prefab | Menggantikan |
|---|---|
| `Laras`, `Laras_Berdoa` | Laras berdiri; Laras berlutut berdoa |
| `BidanWulan` | Bidan Wulan |
| `BuRatmi_Berbaring` | Bu Ratmi telentang di ranjang (kepala ke arah -Z) |
| `BuSumi_Siluet` | siluet Bu Sumi di jendela |
| `Ojek` | tukang ojek di atas motor |
| `Ranjang`, `LemariAyah`, `Gembok`, `FotoKeluarga`, `JamDinding`, `Kalender`, `Meja`, `Kursi`, `Paku`, `Teplok`, `StopKontak`, `SakaGuru` | perabot dan benda rumah |
| `Gapura`, `JembatanBambu`, `RumpunBambu`, `LampuJalan`, `RumahKecil`, `RumahKeluargaLuar` | jalan masuk desa |

**Tekstur:** pasang pada material di `Assets/_Project/Art/Materials/Pengganti/` (`KayuTua`, `KayuGelap`, `Gedek`, `Tanah`, `Genteng`, `Bambu`, `Kain`). Dinding, lantai, dan plafon rumah memakai material itu.

**Tidak dipakai di Fase 1** (tidak perlu dicari sekarang): model buku harian, map dokumen, obat dan sachet (ketiganya hanya tampil sebagai teks di inventaris), HP di tangan. Suara tokoh (gadis menangis di telepon, bisikan doa perempuan) **belakangan**: voice acting ditunda, semua percakapan teks saja.
```

Lalu di tabel atas file yang sama, isi kolom Status dua baris ini dengan `belakangan (suara tokoh ditunda)`: `| Gadis menangis di telepon | ...` dan `| Bisikan doa perempuan | ...`.

- [ ] **Step 3: Commit**

```bash
git add docs/aset/fase-2.md docs/aset/fase-1.md
git commit -m "docs: add Fase 2 asset list and Fase 1 asset file names" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 2: Core — inventaris, efek `barang`, dan membuka lemari ayah

**Files:**
- Create: `Assets/_Project/Tests/EditMode/InventarisTests.cs`, `Assets/_Project/Scripts/Core/Inventaris.cs`
- Modify: `Assets/_Project/Scripts/Core/GameState.cs`, `Enums.cs`, `Puzzle.cs`, `EfekDialog.cs`, `SaveData.cs`; `Assets/_Project/Tests/EditMode/EfekDialogTests.cs`, `PuzzleTests.cs`, `SaveDataTests.cs`

**Interfaces:**
- Consumes: `GameState.Umumkan`, `BukuHarian.Temukan`, `TujuanMalam.Selesaikan`, `GembokLemari.Cek`, `Kode.H1/H2`.
- Produces:
  - `GameState.Barang` (`List<string>`), `Perubahan.Barang`.
  - `Inventaris` (static): `const string ObatIbu = "ObatIbu", BukuHarianAyah = "BukuHarianAyah", MapSertifikat = "MapSertifikat"`; `bool Dikenal(string id)`, `string Nama(string id)`, `string Keterangan(string id)`, `bool Punya(GameState s, string id)`, `void Tambah(GameState s, string id)`. Id tidak dikenal: `ArgumentException`.
  - `GembokLemari.Buka(GameState s, string kode)` → `bool`.
  - Efek dialog baru: `barang:<Id>`.

Nama dan keterangan barang ditulis pengembang (K5); dokumen hanya menyebut "obat dan oralit", "buku harian ayah", dan "map fotokopi sertifikat tanah".

- [ ] **Step 1: Tulis test yang gagal**

`Assets/_Project/Tests/EditMode/InventarisTests.cs`:
```csharp
using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class InventarisTests
    {
        [Test]
        public void StateBaruTanpaBarang()
        {
            Assert.IsEmpty(new GameState().Barang);
        }

        [Test]
        public void TambahMenyimpanDanMengumumkan()
        {
            var s = new GameState();
            Perubahan? terakhir = null;
            s.Berubah += p => terakhir = p;

            Inventaris.Tambah(s, Inventaris.ObatIbu);

            Assert.IsTrue(Inventaris.Punya(s, Inventaris.ObatIbu));
            Assert.IsFalse(Inventaris.Punya(s, Inventaris.MapSertifikat));
            Assert.AreEqual(Perubahan.Barang, terakhir);
        }

        [Test]
        public void TambahDuaKaliTidakMenggandakan()
        {
            var s = new GameState();
            int jumlahEvent = 0;
            s.Berubah += p => jumlahEvent++;

            Inventaris.Tambah(s, Inventaris.ObatIbu);
            Inventaris.Tambah(s, Inventaris.ObatIbu);

            Assert.AreEqual(1, s.Barang.Count);
            Assert.AreEqual(1, jumlahEvent);
        }

        [Test]
        public void BarangTidakDikenalDitolak()
        {
            var s = new GameState();
            Assert.Throws<ArgumentException>(() => Inventaris.Tambah(s, "Linggis?"));
            Assert.IsEmpty(s.Barang);
        }

        [TestCase(Inventaris.ObatIbu)]
        [TestCase(Inventaris.BukuHarianAyah)]
        [TestCase(Inventaris.MapSertifikat)]
        public void SetiapBarangPunyaNamaDanKeterangan(string id)
        {
            Assert.IsTrue(Inventaris.Dikenal(id));
            Assert.IsNotEmpty(Inventaris.Nama(id));
            Assert.IsNotEmpty(Inventaris.Keterangan(id));
        }

        [Test]
        public void NamaBarangTidakDikenalDitolak()
        {
            Assert.IsFalse(Inventaris.Dikenal("X"));
            Assert.IsFalse(Inventaris.Dikenal(null));
            Assert.Throws<ArgumentException>(() => Inventaris.Nama("X"));
        }
    }
}
```

Tambahkan di dalam kelas `EfekDialogTests` (`EfekDialogTests.cs`):
```csharp
        [Test]
        public void BarangMasukInventaris()
        {
            var s = new GameState();
            EfekDialog.Jalankan(s, "barang:ObatIbu");
            Assert.IsTrue(Inventaris.Punya(s, Inventaris.ObatIbu));
        }

        [Test]
        public void BarangTidakDikenalDitolakTanpaMengubahState()
        {
            var s = new GameState();
            Assert.Throws<FormatException>(() => EfekDialog.Jalankan(s, "katanya:K1; barang:Linggis"));
            Assert.IsFalse(s.KatanyaDiketahui[Kode.K1]);
        }
```

Tambahkan di dalam kelas `PuzzleTests` (`PuzzleTests.cs`):
```csharp
        [Test]
        public void KodeBenarMembukaLemariDanMemberiIsinya()
        {
            var s = new GameState { MalamKe = 1 };

            Assert.IsTrue(GembokLemari.Buka(s, "2003"));

            Assert.IsTrue(Inventaris.Punya(s, Inventaris.BukuHarianAyah));
            Assert.IsTrue(Inventaris.Punya(s, Inventaris.MapSertifikat));
            Assert.IsTrue(s.HalamanDiary[Kode.H1]);
            Assert.IsTrue(s.HalamanDiary[Kode.H2]);
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.BukaLemari));
        }

        [Test]
        public void KodeSalahTidakMengubahApaPun()
        {
            var s = new GameState { MalamKe = 1 };
            Assert.IsFalse(GembokLemari.Buka(s, "1998"));
            Assert.AreEqual(SaveData.KeJson(new GameState { MalamKe = 1 }), SaveData.KeJson(s));
        }

        [Test]
        public void MembukaLemariDuaKaliTidakMenggandakan()
        {
            var s = new GameState { MalamKe = 1 };
            GembokLemari.Buka(s, "2003");
            GembokLemari.Buka(s, "2003");
            Assert.AreEqual(2, s.Barang.Count);
            Assert.AreEqual(1, s.TujuanSelesai.Count);
        }
```

Tambahkan di dalam kelas `SaveDataTests` (`SaveDataTests.cs`):
```csharp
        [Test]
        public void BarangIkutTersimpan()
        {
            var s = new GameState();
            Inventaris.Tambah(s, Inventaris.ObatIbu);
            var hasil = SaveData.DariJson(SaveData.KeJson(s));
            CollectionAssert.AreEqual(new[] { Inventaris.ObatIbu }, hasil.Barang);
        }

        [Test]
        public void BarangTidakDikenalDitolak()
        {
            var s = new GameState();
            s.Barang.Add("Linggis?");
            Assert.Throws<FormatException>(() => SaveData.DariJson(SaveData.KeJson(s)));
        }
```

- [ ] **Step 2: Jalankan, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.InventarisTests`
Expected: gagal dengan `error CS0103` atau `CS0117` (`Inventaris`, `Barang` belum ada).

- [ ] **Step 3: Implementasi**

`Enums.cs`, ganti baris `Perubahan`:
```csharp
    public enum Perubahan { Kondisi, Media, Bukti, Katanya, Kepercayaan, Ritual, Diary, Sidang, Jam, Tujuan, Malam, Ending, Barang }
```

`GameState.cs`, tambahkan tepat setelah baris `public List<string> TujuanSelesai = new List<string>();`:
```csharp
        public List<string> Barang = new List<string>();            // id dari Inventaris
```

`Assets/_Project/Scripts/Core/Inventaris.cs`:
```csharp
using System;

namespace TheSending.Core
{
    // Barang yang dibawa Raka (tombol I). Barang tidak habis dipakai: obat dari Bidan Wulan cukup untuk semua malam.
    public static class Inventaris
    {
        public const string ObatIbu = "ObatIbu", BukuHarianAyah = "BukuHarianAyah", MapSertifikat = "MapSertifikat";

        // id, nama, keterangan
        static readonly string[][] Daftar =
        {
            new[] { ObatIbu, "Obat dan oralit", "Dari Bidan Wulan. Diminumkan kepada Ibu pukul 21.00." },
            new[] { BukuHarianAyah, "Buku harian Bapak", "Banyak halamannya sobek. Halaman yang sudah ditemukan bisa dibaca di Jurnal." },
            new[] { MapSertifikat, "Map fotokopi sertifikat tanah", "Fotokopi sertifikat tanah keluarga di tepi sungai." },
        };

        public static bool Dikenal(string id) => Cari(id) != null;
        public static string Nama(string id) => Wajib(id)[1];
        public static string Keterangan(string id) => Wajib(id)[2];
        public static bool Punya(GameState s, string id) => s.Barang.Contains(id);

        public static void Tambah(GameState s, string id)
        {
            Wajib(id);
            if (s.Barang.Contains(id)) return;
            s.Barang.Add(id);
            s.Umumkan(Perubahan.Barang);
        }

        static string[] Cari(string id)
        {
            foreach (var b in Daftar)
                if (b[0] == id) return b;
            return null;
        }

        static string[] Wajib(string id) =>
            Cari(id) ?? throw new ArgumentException($"Barang '{id}' tidak dikenal.", nameof(id));
    }
}
```

`Puzzle.cs`, ganti kelas `GembokLemari`:
```csharp
    // Malam 1: kode gembok = tahun rumah berdiri, terukir di blandar.
    public static class GembokLemari
    {
        public const string KodeBenar = "2003";
        public static bool Cek(string kode) => kode == KodeBenar;

        // Kode benar: Raka mendapat buku harian ayah (Halaman 1 dan 2, Game Story 10.3) dan map sertifikat.
        // Aman dipanggil ulang: semua langkahnya tidak menggandakan apa pun.
        public static bool Buka(GameState s, string kode)
        {
            if (!Cek(kode)) return false;
            Inventaris.Tambah(s, Inventaris.BukuHarianAyah);
            Inventaris.Tambah(s, Inventaris.MapSertifikat);
            BukuHarian.Temukan(s, Kode.H1);
            BukuHarian.Temukan(s, Kode.H2);
            TujuanMalam.Selesaikan(s, TujuanMalam.BukaLemari);
            return true;
        }
    }
```

`EfekDialog.cs`, di dalam `switch (nama)` tambahkan tepat setelah baris `case "tujuan": ...`:
```csharp
                case "barang": { var id = Wajib(isi, asli); if (!Inventaris.Dikenal(id)) throw new FormatException($"Efek '{asli}': barang '{id}' tidak dikenal."); return s => Inventaris.Tambah(s, id); }
```

`SaveData.cs`, di `Validasi` tambahkan tepat setelah baris `if (s.TujuanSelesai.Contains(null)) ...`:
```csharp
            if (s.Barang == null) throw new FormatException("Barang tidak ada.");
            foreach (var id in s.Barang)
                if (!Inventaris.Dikenal(id)) throw new FormatException($"Barang '{id}' tidak dikenal.");
```

- [ ] **Step 4: Jalankan, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.InventarisTests`
Expected: `result="Passed" total="8" passed="8" failed="0"`

- [ ] **Step 5: Jalankan seluruh suite**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="238" passed="238" failed="0"`

- [ ] **Step 6: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Core Assets/_Project/Tests/EditMode
git commit -m "feat: add inventory rules, barang dialog effect, and opening the father's wardrobe" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 3: Core — foto petunjuk di Papan Bukti (K2)

Lewati task ini jika tim memilih K2-B.

**Files:**
- Modify: `Assets/_Project/Scripts/Core/GameState.cs`, `Enums.cs`, `PapanBukti.cs`, `EfekDialog.cs`, `SaveData.cs`; `Assets/_Project/Tests/EditMode/PapanBuktiTests.cs`, `EfekDialogTests.cs`, `SaveDataTests.cs`

**Interfaces:**
- Consumes: `GameState.Umumkan`, pola `Inventaris` (Task 2).
- Produces:
  - `GameState.Foto` (`List<string>`), `Perubahan.Foto`.
  - `PapanBukti`: `const string FotoPakuMalam1 = "PakuMalam1"`, `bool FotoDikenal(string id)`, `string JudulFoto(string id)`, `void TambahFoto(GameState s, string id)`. Foto tidak mengubah `BuktiKunci` dan tidak dipakai `SidangWarga`.
  - Efek dialog baru: `foto:<Id>`.

- [ ] **Step 1: Tulis test yang gagal**

Tambahkan di dalam kelas `PapanBuktiTests`:
```csharp
        [Test]
        public void FotoMasukPapanDanMengumumkan()
        {
            var s = new GameState();
            Perubahan? terakhir = null;
            s.Berubah += p => terakhir = p;

            PapanBukti.TambahFoto(s, PapanBukti.FotoPakuMalam1);

            CollectionAssert.AreEqual(new[] { PapanBukti.FotoPakuMalam1 }, s.Foto);
            Assert.AreEqual(Perubahan.Foto, terakhir);
            Assert.IsNotEmpty(PapanBukti.JudulFoto(PapanBukti.FotoPakuMalam1));
            CollectionAssert.DoesNotContain(s.BuktiKunci, true);
        }

        [Test]
        public void FotoKembarTidakMenggandakan()
        {
            var s = new GameState();
            PapanBukti.TambahFoto(s, PapanBukti.FotoPakuMalam1);
            PapanBukti.TambahFoto(s, PapanBukti.FotoPakuMalam1);
            Assert.AreEqual(1, s.Foto.Count);
        }

        [Test]
        public void FotoTidakDikenalDitolak()
        {
            var s = new GameState();
            Assert.IsFalse(PapanBukti.FotoDikenal("Hantu"));
            Assert.Throws<ArgumentException>(() => PapanBukti.TambahFoto(s, "Hantu"));
            Assert.Throws<ArgumentException>(() => PapanBukti.JudulFoto("Hantu"));
            Assert.IsEmpty(s.Foto);
        }
```

Tambahkan di dalam kelas `EfekDialogTests`:
```csharp
        [Test]
        public void FotoLewatEfek()
        {
            var s = new GameState { MalamKe = 1 };
            EfekDialog.Jalankan(s, "tujuan:PotretPaku; foto:PakuMalam1");
            Assert.IsTrue(TujuanMalam.Sudah(s, TujuanMalam.PotretPaku));
            CollectionAssert.Contains(s.Foto, PapanBukti.FotoPakuMalam1);
        }

        [Test]
        public void FotoTidakDikenalLewatEfekDitolak()
        {
            Assert.Throws<FormatException>(() => EfekDialog.Jalankan(new GameState(), "foto:Hantu"));
        }
```

Tambahkan di dalam kelas `SaveDataTests`:
```csharp
        [Test]
        public void FotoIkutTersimpan()
        {
            var s = new GameState();
            PapanBukti.TambahFoto(s, PapanBukti.FotoPakuMalam1);
            var hasil = SaveData.DariJson(SaveData.KeJson(s));
            CollectionAssert.AreEqual(new[] { PapanBukti.FotoPakuMalam1 }, hasil.Foto);
        }

        [Test]
        public void FotoTidakDikenalDiSaveDitolak()
        {
            var s = new GameState();
            s.Foto.Add("Hantu");
            Assert.Throws<FormatException>(() => SaveData.DariJson(SaveData.KeJson(s)));
        }
```

- [ ] **Step 2: Jalankan, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PapanBuktiTests`
Expected: gagal dengan `error CS` (`TambahFoto`, `Foto` belum ada).

- [ ] **Step 3: Implementasi**

`Enums.cs`, ganti baris `Perubahan`:
```csharp
    public enum Perubahan { Kondisi, Media, Bukti, Katanya, Kepercayaan, Ritual, Diary, Sidang, Jam, Tujuan, Malam, Ending, Barang, Foto }
```

`GameState.cs`, tambahkan tepat setelah baris `public List<string> Barang ...`:
```csharp
        public List<string> Foto = new List<string>();              // id foto petunjuk dari PapanBukti
```

`PapanBukti.cs`, tambahkan di dalam kelas, tepat setelah `TambahKatanya`:
```csharp
        public const string FotoPakuMalam1 = "PakuMalam1";

        // Foto petunjuk yang bukan Bukti Kunci (desain 4.7). Tampil di Papan Bukti berlabel FOTO, tidak dihitung di Sidang Warga.
        // id, judul
        static readonly string[][] DaftarFoto =
        {
            new[] { FotoPakuMalam1, "Tiga paku berkarat yang dimuntahkan Ibu pada Malam 1" },
        };

        public static bool FotoDikenal(string id) => CariFoto(id) != null;

        public static string JudulFoto(string id) =>
            (CariFoto(id) ?? throw new ArgumentException($"Foto '{id}' tidak dikenal.", nameof(id)))[1];

        public static void TambahFoto(GameState s, string id)
        {
            if (!FotoDikenal(id)) throw new ArgumentException($"Foto '{id}' tidak dikenal.", nameof(id));
            if (s.Foto.Contains(id)) return;
            s.Foto.Add(id);
            s.Umumkan(Perubahan.Foto);
        }

        static string[] CariFoto(string id)
        {
            foreach (var f in DaftarFoto)
                if (f[0] == id) return f;
            return null;
        }
```

`EfekDialog.cs`, tambahkan tepat setelah baris `case "barang": ...`:
```csharp
                case "foto": { var id = Wajib(isi, asli); if (!PapanBukti.FotoDikenal(id)) throw new FormatException($"Efek '{asli}': foto '{id}' tidak dikenal."); return s => PapanBukti.TambahFoto(s, id); }
```

`SaveData.cs`, di `Validasi` tambahkan tepat setelah pemeriksaan `Barang` dari Task 2:
```csharp
            if (s.Foto == null) throw new FormatException("Foto tidak ada.");
            foreach (var id in s.Foto)
                if (!PapanBukti.FotoDikenal(id)) throw new FormatException($"Foto '{id}' tidak dikenal.");
```

- [ ] **Step 4: Jalankan, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PapanBuktiTests`
Expected: `result="Passed" total="17" passed="17" failed="0"`

- [ ] **Step 5: Jalankan seluruh suite**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="245" passed="245" failed="0"`

- [ ] **Step 6: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Core Assets/_Project/Tests/EditMode
git commit -m "feat: add photographed clues to the evidence board" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 4: Core — batas jendela obat dan scene per bab

**Files:**
- Create: `Assets/_Project/Tests/EditMode/PetaSceneTests.cs`, `Assets/_Project/Scripts/Core/PetaScene.cs`
- Modify: `Assets/_Project/Scripts/Core/JamGim.cs`, `Assets/_Project/Tests/EditMode/JamGimTests.cs`

**Interfaces:**
- Consumes: `TujuanMalam.Sudah`, `TujuanMalam.TibaDiRumah`, `JamGim.Jam20/Jam21`.
- Produces:
  - `JamGim.MulaiJendelaObat(int malam)` → `float` menit (Malam 6: 1200, lainnya 1260), `JamGim.AkhirJendelaObat(int malam)` → mulai + 60.
  - `PetaScene` (static): `const string Desa = "Scene_Desa", RumahKeluarga = "Scene_RumahKeluarga"`, `string Untuk(GameState s)`.

- [ ] **Step 1: Tulis test yang gagal**

Tambahkan di dalam kelas `JamGimTests`:
```csharp
        [TestCase(1, 1260f, 1320f)]
        [TestCase(5, 1260f, 1320f)]
        [TestCase(6, 1200f, 1260f)]
        public void MulaiDanAkhirJendelaObat(int malam, float mulai, float akhir)
        {
            Assert.AreEqual(mulai, JamGim.MulaiJendelaObat(malam));
            Assert.AreEqual(akhir, JamGim.AkhirJendelaObat(malam));
            Assert.IsTrue(JamGim.DalamJendelaObat(new GameState { MalamKe = malam, MenitGim = mulai }));
            Assert.IsFalse(JamGim.DalamJendelaObat(new GameState { MalamKe = malam, MenitGim = akhir }));
        }
```

`Assets/_Project/Tests/EditMode/PetaSceneTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PetaSceneTests
    {
        [Test]
        public void PrologDimulaiDiJalanDesa()
        {
            Assert.AreEqual("Scene_Desa", PetaScene.Untuk(new GameState()));
        }

        [Test]
        public void PrologSetelahTibaDiRumahDimuatDiRumah()
        {
            var s = new GameState();
            TujuanMalam.Selesaikan(s, TujuanMalam.TibaDiRumah);
            Assert.AreEqual("Scene_RumahKeluarga", PetaScene.Untuk(s));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(7)]
        public void SetiapMalamDimulaiDiRumahKeluarga(int malam)
        {
            Assert.AreEqual(PetaScene.RumahKeluarga, PetaScene.Untuk(new GameState { MalamKe = malam }));
        }
    }
}
```

- [ ] **Step 2: Jalankan, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PetaSceneTests`
Expected: gagal dengan `error CS0103` (`PetaScene` belum ada).

- [ ] **Step 3: Implementasi**

`JamGim.cs`, ganti method `DalamJendelaObat` dengan:
```csharp
        // Desain 4.4: 21.00-22.00, Malam 6 dimajukan ke 20.00-21.00.
        public static float MulaiJendelaObat(int malam) => malam == 6 ? Jam20 : Jam21;
        public static float AkhirJendelaObat(int malam) => MulaiJendelaObat(malam) + LamaJendelaObat;

        public static bool DalamJendelaObat(GameState s) =>
            s.MalamKe >= 1 && s.MenitGim >= MulaiJendelaObat(s.MalamKe) && s.MenitGim < AkhirJendelaObat(s.MalamKe);
```

`Assets/_Project/Scripts/Core/PetaScene.cs`:
```csharp
namespace TheSending.Core
{
    // Scene yang dimuat "Mulai Baru" dan "Lanjutkan" untuk sebuah GameState (desain 2.7).
    public static class PetaScene
    {
        public const string Desa = "Scene_Desa", RumahKeluarga = "Scene_RumahKeluarga";

        // Prolog dimulai di jalan desa sampai Raka tiba di rumah. Sesudah itu setiap malam dimulai di rumah keluarga.
        public static string Untuk(GameState s) =>
            s.MalamKe == 0 && !TujuanMalam.Sudah(s, TujuanMalam.TibaDiRumah) ? Desa : RumahKeluarga;
    }
}
```

- [ ] **Step 4: Jalankan, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PetaSceneTests`
Expected: `result="Passed" total="5" passed="5" failed="0"`

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.JamGimTests`
Expected: `result="Passed" total="24" passed="24" failed="0"`

- [ ] **Step 5: Jalankan seluruh suite**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="253" passed="253" failed="0"`

- [ ] **Step 6: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Core Assets/_Project/Tests/EditMode
git commit -m "feat: add medicine window bounds and start scene per chapter" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 5: Core — pemeriksa dialog dan test semua aset `DataDialog`

Catatan `CLAUDE.md`: "Tambahkan EditMode test yang menjalankan semua kode efek di setiap aset `DataDialog`". Test ini **mengurai** setiap kode efek (bukan menjalankannya), karena beberapa efek sah melempar `InvalidOperationException` pada state kosong (`serahkan`, `minyak-sereh`, `pilihan-akhir`). Salah ketik tetap tertangkap, karena semua pemeriksaan kode terjadi saat mengurai.

**Files:**
- Create: `Assets/_Project/Tests/EditMode/PemeriksaDialogTests.cs`, `Assets/_Project/Tests/EditMode/DataDialogTests.cs`, `Assets/_Project/Scripts/Core/PemeriksaDialog.cs`
- Modify: `Assets/_Project/Scripts/Core/EfekDialog.cs`, `Assets/_Project/Tests/EditMode/EfekDialogTests.cs`, `Assets/_Project/Tests/EditMode/TheSending.Tests.EditMode.asmdef`

**Interfaces:**
- Consumes: `EfekDialog` (urai), `BarisDialog`, `PilihanDialog`, `DataDialog.Baris` (Runtime).
- Produces:
  - `EfekDialog.Periksa(string kode)`: mengurai saja; `FormatException` jika salah; tidak menyentuh state.
  - `PemeriksaDialog` (static): `const int MaksPilihan = 4`, `List<string> Periksa(IReadOnlyList<BarisDialog> baris)` (daftar masalah; kosong = sehat).
  - Assembly test mereferensikan `TheSending.Runtime` (K6).

- [ ] **Step 1: Tulis test yang gagal**

Tambahkan di dalam kelas `EfekDialogTests`:
```csharp
        [Test]
        public void PeriksaMenerimaKodeBenar()
        {
            Assert.DoesNotThrow(() => EfekDialog.Periksa("tujuan:BicaraBidan; barang:ObatIbu"));
            Assert.DoesNotThrow(() => EfekDialog.Periksa("serahkan:M1"));   // sah diurai walau belum bisa dijalankan
            Assert.DoesNotThrow(() => EfekDialog.Periksa(""));
        }

        [Test]
        public void PeriksaMenolakKodeSalah()
        {
            Assert.Throws<FormatException>(() => EfekDialog.Periksa("bukti:B9"));
            Assert.Throws<FormatException>(() => EfekDialog.Periksa("tujuan:TidakAda"));
            Assert.Throws<FormatException>(() => EfekDialog.Periksa("salah-ketik"));
        }
```

`Assets/_Project/Tests/EditMode/PemeriksaDialogTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PemeriksaDialogTests
    {
        static PilihanDialog P(string efek, int lompat) => new PilihanDialog { Teks = "x", Efek = efek, LompatKe = lompat };

        [Test]
        public void DialogSehatTanpaMasalah()
        {
            var baris = new[]
            {
                new BarisDialog { Teks = "a", Efek = "katanya:K1", Pilihan = new[] { P("bukti:B2", 1), P("", -1) } },
                new BarisDialog { Teks = "b", Akhir = true },
            };
            CollectionAssert.IsEmpty(PemeriksaDialog.Periksa(baris));
        }

        [Test]
        public void DialogKosongDilaporkan()
        {
            Assert.AreEqual(1, PemeriksaDialog.Periksa(new BarisDialog[0]).Count);
            Assert.AreEqual(1, PemeriksaDialog.Periksa(null).Count);
        }

        [Test]
        public void EfekSalahDilaporkanDenganTempatnya()
        {
            var baris = new[]
            {
                new BarisDialog { Teks = "a" },
                new BarisDialog { Teks = "b", Efek = "bukti:B9", Pilihan = new[] { P("salah", -1) } },
            };
            var masalah = PemeriksaDialog.Periksa(baris);
            Assert.AreEqual(2, masalah.Count);
            StringAssert.StartsWith("Baris 1:", masalah[0]);
            StringAssert.StartsWith("Baris 1 pilihan 1:", masalah[1]);
        }

        [Test]
        public void LebihDariEmpatPilihanDilaporkan()
        {
            var baris = new[]
            {
                new BarisDialog { Teks = "a", Pilihan = new[] { P("", -1), P("", -1), P("", -1), P("", -1), P("", -1) } },
            };
            Assert.AreEqual(1, PemeriksaDialog.Periksa(baris).Count);
        }

        [Test]
        public void LompatanDiLuarDaftarDilaporkan()
        {
            var baris = new[] { new BarisDialog { Teks = "a", Pilihan = new[] { P("", 1) } } };
            Assert.AreEqual(1, PemeriksaDialog.Periksa(baris).Count);
        }
    }
}
```

- [ ] **Step 2: Jalankan, pastikan gagal**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PemeriksaDialogTests`
Expected: gagal dengan `error CS0103` (`PemeriksaDialog` belum ada).

- [ ] **Step 3: Implementasi Core**

`EfekDialog.cs`, ganti method `Jalankan` dengan tiga method ini (method `Urai` dan pembantu di bawahnya tidak berubah):
```csharp
        public static void Jalankan(GameState s, string kode)
        {
            // Urai semua bagian dulu, baru jalankan: kode salah di tengah tidak boleh mengubah sebagian state.
            foreach (var a in UraiSemua(kode)) a(s);
        }

        // Hanya mengurai: FormatException jika ada kode salah. Tidak menyentuh state; dipakai test data dialog.
        public static void Periksa(string kode) => UraiSemua(kode);

        static List<Action<GameState>> UraiSemua(string kode)
        {
            var aksi = new List<Action<GameState>>();
            if (string.IsNullOrWhiteSpace(kode)) return aksi;
            foreach (var bagian in kode.Split(';'))
            {
                var k = bagian.Trim();
                if (k.Length == 0) continue;
                int titikDua = k.IndexOf(':');
                string nama = (titikDua < 0 ? k : k.Substring(0, titikDua)).Trim();
                string isi = titikDua < 0 ? "" : k.Substring(titikDua + 1).Trim();
                aksi.Add(Urai(nama, isi, k));
            }
            return aksi;
        }
```

`Assets/_Project/Scripts/Core/PemeriksaDialog.cs`:
```csharp
using System;
using System.Collections.Generic;

namespace TheSending.Core
{
    // Memeriksa data satu percakapan tanpa menjalankannya, supaya salah ketik ketahuan di test, bukan di Play Mode.
    public static class PemeriksaDialog
    {
        public const int MaksPilihan = 4;   // tombol 1-4 di PanelDialog

        // Daftar masalah; kosong berarti dialog sehat.
        public static List<string> Periksa(IReadOnlyList<BarisDialog> baris)
        {
            var masalah = new List<string>();
            if (baris == null || baris.Count == 0)
            {
                masalah.Add("Dialog kosong.");
                return masalah;
            }
            for (int i = 0; i < baris.Count; i++)
            {
                var b = baris[i];
                if (b == null)
                {
                    masalah.Add($"Baris {i}: kosong.");
                    continue;
                }
                CekEfek(masalah, $"Baris {i}", b.Efek);
                var pilihan = b.Pilihan ?? new PilihanDialog[0];
                if (pilihan.Length > MaksPilihan)
                    masalah.Add($"Baris {i}: {pilihan.Length} pilihan, maksimal {MaksPilihan}.");
                for (int p = 0; p < pilihan.Length; p++)
                {
                    CekEfek(masalah, $"Baris {i} pilihan {p + 1}", pilihan[p].Efek);
                    if (pilihan[p].LompatKe >= baris.Count)
                        masalah.Add($"Baris {i} pilihan {p + 1}: LompatKe {pilihan[p].LompatKe} di luar daftar baris.");
                }
            }
            return masalah;
        }

        static void CekEfek(List<string> masalah, string tempat, string efek)
        {
            try { EfekDialog.Periksa(efek); }
            catch (FormatException e) { masalah.Add($"{tempat}: {e.Message}"); }
        }
    }
}
```

- [ ] **Step 4: Jalankan, pastikan lulus**

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.PemeriksaDialogTests`
Expected: `result="Passed" total="5" passed="5" failed="0"`

Run: `bash tools/run-editmode-tests.sh TheSending.Tests.EfekDialogTests`
Expected: `result="Passed" total="25" passed="25" failed="0"` (19 lama + 2 Task 2 + 2 Task 3 + 2 Task 5)

- [ ] **Step 5: Commit bagian Core**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Core Assets/_Project/Tests/EditMode
git commit -m "feat: add dialog data checker and parse-only effect check" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

- [ ] **Step 6: Tambah referensi Runtime ke assembly test (K6)**

`Assets/_Project/Tests/EditMode/TheSending.Tests.EditMode.asmdef`, ganti blok `references`:
```json
    "references": [
        "UnityEngine.TestRunner",
        "UnityEditor.TestRunner",
        "TheSending.Core",
        "TheSending.Runtime"
    ],
```

- [ ] **Step 7: Tulis test aset**

`Assets/_Project/Tests/EditMode/DataDialogTests.cs`:
```csharp
using NUnit.Framework;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;

namespace TheSending.Tests
{
    // Memeriksa SEMUA aset DataDialog di proyek: kode efek, lompatan, dan jumlah pilihan.
    public class DataDialogTests
    {
        [Test]
        public void SemuaAsetDialogLolosPemeriksaan()
        {
            var guid = AssetDatabase.FindAssets("t:DataDialog", new[] { "Assets/_Project/Data" });
            Assert.IsNotEmpty(guid, "Tidak ada aset DataDialog di Assets/_Project/Data.");
            foreach (var g in guid)
            {
                string jalur = AssetDatabase.GUIDToAssetPath(g);
                var data = AssetDatabase.LoadAssetAtPath<DataDialog>(jalur);
                Assert.IsNotNull(data, jalur);
                CollectionAssert.IsEmpty(PemeriksaDialog.Periksa(data.Baris), jalur);
            }
        }
    }
}
```

- [ ] **Step 8: Jalankan seluruh suite**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"` (aset yang ada saat ini hanya `Data/Sandbox/DialogContoh.asset`; aset Fase 1 ikut diperiksa mulai Task 11).

- [ ] **Step 9: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Tests/EditMode
git commit -m "test: check every DataDialog asset for bad effect codes and jumps" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 6: Runtime — catatan Fase 1 (scene per bab, jam, Esc, penjaga dialog, saringan HUD, alat uji)

**Files:**
- Modify: `Assets/_Project/Scripts/Runtime/GameManager.cs`, `MenuUtama.cs`, `MenuJeda.cs`, `Hud.cs`, `PanelDialog.cs`, `PanelJurnal.cs`, `AlatUji.cs`

**Interfaces:**
- Consumes: `PetaScene.Untuk` (Task 4), `PemeriksaDialog.MaksPilihan` (Task 5), `Perubahan.Barang/Foto` (Task 2, 3), `JamGim.Maju`, `JamGim.DetikNyataPerJamGim`.
- Produces:
  - `GameManager`: `string SceneUntukState { get; }` (pengganti `SceneAwal`, yang **dihapus**), `bool Sinematik { get; set; }`. Jam maju hanya saat `KunciKursor && !Dijeda`. Kursor tetap terkunci saat `Sinematik`.
  - `Hud.Tampilkan(string)`: pesan kembar berurutan dan pesan kosong diabaikan. Saat `Sinematik`, titik bidik, prompt, jam, dan baterai disembunyikan; notifikasi tetap tampil.
  - `PanelDialog`: `Mulai` mengembalikan `false` + `LogError` jika `GameManager` tidak ada; `IEnumerator MulaiDanTunggu(DataDialog data)`; `LogWarning` jika baris punya lebih dari 4 pilihan.
  - `PanelJurnal`: Esc ikut menutup (K3). `MenuJeda`: `[DefaultExecutionOrder(-50)]`.
  - `AlatUji`: **F6** jam gim +30 menit; **F9** memuat ulang scene aktif setelah `Tertangkap`.

- [ ] **Step 1: `GameManager.cs`**

a. Ganti dua baris `SceneAwal`:
```csharp
        // Scene yang dimuat "Mulai Baru" dan "Lanjutkan". Fase 1 menggantinya dengan scene Prolog.
        public const string SceneAwal = "Scene_Sandbox0B";
```
dengan:
```csharp
        // Scene yang dimuat "Mulai Baru" dan "Lanjutkan": tergantung bab dan tujuan di State (PetaScene).
        public string SceneUntukState => PetaScene.Untuk(State);
```

b. Tambahkan tepat setelah baris `public bool PeringatanSudahTampil { get; set; }`:
```csharp
        // Diisi KameraAdegan: selama adegan berskrip kursor tetap terkunci walaupun gim dijeda.
        public bool Sinematik { get; set; }
```

c. Ganti isi `Update`:
```csharp
        void Update()
        {
            if (Instance != this) return;
            bool kunci = KunciKursor && (!Dijeda || Sinematik);
            Cursor.lockState = kunci ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !kunci;
            // Jam hanya maju selama pemain dikendalikan: tidak di menu utama, adegan, dialog, Jurnal, inventaris, atau jeda.
            if (KunciKursor && !Dijeda && State.Ending == Ending.Belum) JamGim.Maju(State, Time.deltaTime);
        }
```

d. Di `SaatSceneDimuat`, tambahkan tepat sebelum baris `Time.timeScale = 1f;`:
```csharp
            Sinematik = false;
```

- [ ] **Step 2: `MenuUtama.cs`**

Ganti **kedua** `gm.MuatScene(GameManager.SceneAwal);` (di `Lanjutkan` dan `Mulai`) dengan:
```csharp
gm.MuatScene(gm.SceneUntukState);
```
Di `Lanjutkan` barisnya menjadi `if (gm.Lanjutkan()) gm.MuatScene(gm.SceneUntukState);`.

- [ ] **Step 3: `MenuJeda.cs`**

Ganti baris komentar dan deklarasi kelas:
```csharp
    // Menu jeda di scene gameplay. Esc membuka dan menutup; saat pengaturan terbuka, Esc kembali ke menu jeda.
    // Jalan lebih dulu dari panel lain: Esc yang menutup Jurnal, inventaris, atau gembok dibaca di sini saat gim
    // masih dijeda, jadi Esc yang sama tidak ikut membuka menu jeda.
    [DefaultExecutionOrder(-50)]
    public class MenuJeda : MonoBehaviour
```

- [ ] **Step 4: Ganti seluruh `Hud.cs`**

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

        Text titik, prompt, jam, baterai, notifikasi;
        readonly Queue<string> antrean = new Queue<string>();
        float sisaNotifikasi;
        string pesanTerakhir;   // pesan terakhir yang masuk antrean; null setelah semua selesai tampil

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("HUD", 10).transform;
            akar.SetParent(transform, false);
            titik = UiPabrik.BuatTeks(akar, "TitikBidik", 28, TextAnchor.MiddleCenter, new Vector2(0.49f, 0.48f), new Vector2(0.51f, 0.52f));
            titik.text = "·";
            prompt = UiPabrik.BuatTeks(akar, "Prompt", 30, TextAnchor.UpperCenter, new Vector2(0.3f, 0.36f), new Vector2(0.7f, 0.46f));
            jam = UiPabrik.BuatTeks(akar, "Jam", 34, TextAnchor.UpperRight, new Vector2(0.75f, 0.9f), new Vector2(0.98f, 0.98f));
            baterai = UiPabrik.BuatTeks(akar, "Baterai", 24, TextAnchor.UpperRight, new Vector2(0.75f, 0.85f), new Vector2(0.98f, 0.9f));
            notifikasi = UiPabrik.BuatTeks(akar, "Notifikasi", 28, TextAnchor.LowerLeft, new Vector2(0.02f, 0.04f), new Vector2(0.6f, 0.14f));
        }

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
            pesanTerakhir = null;
        }

        // Pesan kembar berurutan (memotret ulang, perubahan ganda dari satu aksi) cukup tampil sekali.
        public void Tampilkan(string pesan)
        {
            if (string.IsNullOrEmpty(pesan) || pesan == pesanTerakhir) return;
            pesanTerakhir = pesan;
            antrean.Enqueue(pesan);
        }

        void SaatBerubah(Perubahan p)
        {
            switch (p)
            {
                case Perubahan.Bukti: Tampilkan("Bukti baru masuk Papan Bukti."); break;
                case Perubahan.Katanya: Tampilkan("Papan Bukti diperbarui."); break;
                case Perubahan.Diary: Tampilkan("Halaman buku harian ditemukan."); break;
                case Perubahan.Media: Tampilkan("Catatan media santet diperbarui."); break;
                case Perubahan.Barang: Tampilkan("Barang baru masuk inventaris (I)."); break;
                case Perubahan.Foto: Tampilkan("Foto masuk Papan Bukti."); break;
            }
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var s = gm.State;

            // Saat adegan berskrip hanya notifikasi yang tampil (teks pengganti suara harus tetap terbaca).
            bool tampil = !gm.Sinematik;
            titik.enabled = prompt.enabled = jam.enabled = baterai.enabled = tampil;

            prompt.text = interaksi != null && interaksi.Sasaran != null ? $"[E] {interaksi.Sasaran.Prompt}" : "";
            jam.text = JamGim.Format(s.MenitGim);
            baterai.text = $"Baterai {Mathf.CeilToInt(s.BateraiPersen)}%";

            if (sisaNotifikasi > 0f)
            {
                sisaNotifikasi -= Time.unscaledDeltaTime;
                if (sisaNotifikasi > 0f) return;
                notifikasi.text = "";
                if (antrean.Count == 0) pesanTerakhir = null;   // pesan yang sama boleh tampil lagi nanti
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
Jika tim memilih K2-B (Task 3 dilewati), hapus baris `case Perubahan.Foto`.

- [ ] **Step 5: `PanelDialog.cs`**

a. Tambahkan `using System.Collections;` di bawah `using System;`.

b. Ganti `const int MaksPilihan = 4;` dengan:
```csharp
        const int MaksPilihan = PemeriksaDialog.MaksPilihan;
```

c. Ganti method `Mulai`:
```csharp
        public bool Mulai(DataDialog data, Action saatSelesai = null)
        {
            if (Terbuka || data == null) return false;
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("PanelDialog butuh GameManager di scene.", this);
                return false;
            }
            sumber = data;
            this.saatSelesai = saatSelesai;
            try
            {
                dialog = new JalannyaDialog(gm.State, data.Baris);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException || e is InvalidOperationException)
            {
                Gagal(e);
                return false;
            }
            frameMulai = Time.frameCount;   // tombol E yang membuka dialog tidak ikut melanjutkannya
            gm.Jeda(true);
            panel.SetActive(true);
            Gambar();
            return true;
        }

        // Untuk adegan berskrip: yield return panel.MulaiDanTunggu(data);
        public IEnumerator MulaiDanTunggu(DataDialog data)
        {
            if (!Mulai(data)) yield break;
            while (Terbuka) yield return null;
        }
```

d. Di `Gambar`, tambahkan tepat sebelum baris `var sb = new StringBuilder();`:
```csharp
            if (b.Pilihan.Length > MaksPilihan)
                Debug.LogWarning($"Dialog '{sumber.name}' baris {dialog.Indeks}: {b.Pilihan.Length} pilihan, hanya {MaksPilihan} yang tampil.", sumber);
```

- [ ] **Step 6: `PanelJurnal.cs`**

a. Di `Update`, ganti blok `if (k.Jurnal.WasPressedThisFrame()) { ... }` (sampai kurung tutupnya) dengan:
```csharp
            // Tab membuka dan menutup; Esc juga menutup (K3).
            bool tombolJurnal = k.Jurnal.WasPressedThisFrame();
            if (Terbuka && (tombolJurnal || k.JedaMenu.WasPressedThisFrame()))
            {
                panel.SetActive(false);
                gm.Jeda(false);
            }
            else if (tombolJurnal && !gm.Dijeda)
            {
                panel.SetActive(true);
                gm.Jeda(true);
                Gambar(gm.State);
            }
```

b. Di `Gambar`, ganti teks judul:
```csharp
            judul.text = "[1] Papan Bukti     [2] Buku Harian     [3] Catatan     [Tab/Esc] Tutup";
```

- [ ] **Step 7: `AlatUji.cs`**

a. Tambahkan `using TheSending.Core;` dan `using UnityEngine.SceneManagement;` di daftar `using`.

b. Ganti baris `info.text = ...`:
```csharp
            info.text = $"UJI  Malam {gm.State.MalamKe}  Kondisi Ibu {gm.State.KondisiIbu}  [F5] simpan  [F6] jam +30 menit  [F9] tertangkap";
```

c. Ganti blok `if (kb.f9Key.wasPressedThisFrame) { ... }` dengan:
```csharp
            if (kb.f6Key.wasPressedThisFrame)
            {
                JamGim.Maju(gm.State, 30f * JamGim.DetikNyataPerJamGim / 60f);   // 30 menit gim
                Pesan("Jam gim maju 30 menit.");
            }
            if (kb.f9Key.wasPressedThisFrame)
            {
                gm.Tertangkap();
                // Isi scene (lemari, lampu, paku) harus mengikuti state yang baru dimuat.
                gm.MuatScene(SceneManager.GetActiveScene().name);
            }
```

- [ ] **Step 8: Pastikan `SceneAwal` tidak tersisa**

Run: `grep -rn "SceneAwal" Assets/_Project/Scripts`
Expected: tidak ada keluaran.

- [ ] **Step 9: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"`, tanpa `error CS`.

- [ ] **Step 10: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Runtime
git commit -m "feat: start scene per chapter, clock only while controlled, Esc closes journal, dialog guards, HUD duplicate filter" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 7: Runtime — panel inventaris, gembok lemari, dan foto di Jurnal

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/PanelInventaris.cs`, `PanelGembok.cs`, `LemariAyah.cs`
- Modify: `Assets/_Project/Scripts/Runtime/PanelJurnal.cs`

**Interfaces:**
- Consumes: `Inventaris` (Task 2), `GembokLemari.Buka` (Task 2), `PapanBukti.JudulFoto` (Task 3), `KontrolPemain.Inventaris/JedaMenu`, `UiPabrik`, `Hud.Tampilkan`, `Interaktif`.
- Produces:
  - `PanelInventaris` (MonoBehaviour): `bool Terbuka`. I membuka dan menutup, Esc menutup; menjeda lewat `Jeda`.
  - `PanelGembok` (MonoBehaviour): field `[SerializeField] Hud hud`; `bool Terbuka`, `void Buka()`. Ketik 4 angka (baris atas atau numpad), Backspace menghapus, Esc batal.
  - `LemariAyah : Interaktif`: field `[SerializeField] PanelGembok panel`, `[SerializeField] GameObject gembok`. Terbuka = `Inventaris.Punya(state, BukuHarianAyah)` (tahan pergantian malam, karena `TujuanSelesai` dikosongkan setiap malam).
  - `PanelJurnal`: Papan Bukti menampilkan baris `[FOTO] ...`.

- [ ] **Step 1: `PanelInventaris.cs`**

```csharp
using System.Text;
using TheSending.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Inventaris (tombol I): daftar barang beserta keterangannya. Menjeda gim seperti Jurnal.
    public class PanelInventaris : MonoBehaviour
    {
        GameObject panel;
        Text isi;

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Inventaris", 15).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.09f, 0.07f, 0.05f, 0.95f), new Vector2(0.2f, 0.15f), new Vector2(0.8f, 0.85f));
            panel = latar.gameObject;
            UiPabrik.BuatTeks(latar.transform, "Judul", 28, TextAnchor.UpperLeft, new Vector2(0.04f, 0.88f), new Vector2(0.96f, 0.97f)).text =
                "INVENTARIS     [I/Esc] Tutup";
            isi = UiPabrik.BuatTeks(latar.transform, "Isi", 26, TextAnchor.UpperLeft, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.86f));
            panel.SetActive(false);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var k = gm.Kontrol;

            bool tombol = k.Inventaris.WasPressedThisFrame();
            if (Terbuka && (tombol || k.JedaMenu.WasPressedThisFrame())) Tutup();
            else if (tombol && !gm.Dijeda)
            {
                panel.SetActive(true);
                gm.Jeda(true);
                Gambar(gm.State);
            }
        }

        // Panel dimatikan atau dihancurkan saat terbuka (misalnya ganti scene): lepaskan jeda.
        void OnDisable() => Tutup();

        void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }

        void Gambar(GameState s)
        {
            var sb = new StringBuilder();
            foreach (var id in s.Barang)
                sb.AppendLine(Inventaris.Nama(id)).Append("    ").AppendLine(Inventaris.Keterangan(id)).AppendLine();
            isi.text = sb.Length == 0 ? "Belum membawa apa-apa." : sb.ToString();
        }
    }
}
```

- [ ] **Step 2: `PanelGembok.cs`**

```csharp
using TheSending.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Gembok 4 angka lemari ayah (Game Story Malam 1). Angka diketik; kode dicek oleh GembokLemari di Core.
    public class PanelGembok : MonoBehaviour
    {
        const int JumlahAngka = 4;
        const string Petunjuk = "Ketik 4 angka.   [Backspace] hapus   [Esc] batal";

        [SerializeField] Hud hud;

        GameObject panel;
        Text angka, petunjuk;
        string kode = "";

        public bool Terbuka => panel != null && panel.activeSelf;

        void Awake()
        {
            var akar = UiPabrik.BuatCanvas("Gembok", 16).transform;
            akar.SetParent(transform, false);
            var latar = UiPabrik.BuatPanel(akar, "Latar", new Color(0.05f, 0.04f, 0.03f, 0.95f), new Vector2(0.3f, 0.3f), new Vector2(0.7f, 0.7f));
            panel = latar.gameObject;
            UiPabrik.BuatTeks(latar.transform, "Judul", 32, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.75f), new Vector2(0.95f, 0.95f)).text =
                "GEMBOK LEMARI BAPAK";
            angka = UiPabrik.BuatTeks(latar.transform, "Angka", 110, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.3f), new Vector2(0.95f, 0.75f));
            petunjuk = UiPabrik.BuatTeks(latar.transform, "Petunjuk", 24, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.28f));
            panel.SetActive(false);
        }

        public void Buka()
        {
            var gm = GameManager.Instance;
            if (Terbuka || gm == null || gm.Dijeda) return;
            kode = "";
            petunjuk.text = Petunjuk;
            panel.SetActive(true);
            gm.Jeda(true);
            Gambar();
        }

        void Update()
        {
            if (!Terbuka) return;
            var gm = GameManager.Instance;
            var kb = Keyboard.current;
            if (gm == null || kb == null) return;

            if (gm.Kontrol.JedaMenu.WasPressedThisFrame())
            {
                Tutup();
                return;
            }
            if (kb.backspaceKey.wasPressedThisFrame && kode.Length > 0) kode = kode.Substring(0, kode.Length - 1);
            for (int d = 0; d <= 9 && kode.Length < JumlahAngka; d++)
            {
                // Di enum Key, Digit1..Digit9 berurutan lalu Digit0; Numpad0..Numpad9 berurutan.
                Key atas = d == 0 ? Key.Digit0 : Key.Digit1 + (d - 1);
                if (kb[atas].wasPressedThisFrame || kb[Key.Numpad0 + d].wasPressedThisFrame) kode += d;
            }
            if (kode.Length == JumlahAngka) Coba(gm);
            Gambar();
        }

        void Coba(GameManager gm)
        {
            if (GembokLemari.Buka(gm.State, kode))
            {
                Tutup();
                if (hud != null) hud.Tampilkan("Gembok terbuka.");
                return;
            }
            kode = "";
            petunjuk.text = "Gembok tidak terbuka.\n" + Petunjuk;
        }

        // Panel dimatikan atau dihancurkan saat terbuka: lepaskan jeda.
        void OnDisable() => Tutup();

        void Tutup()
        {
            if (!Terbuka) return;
            panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.Jeda(false);
        }

        void Gambar() => angka.text = kode.PadRight(JumlahAngka, '_');
    }
}
```

- [ ] **Step 3: `LemariAyah.cs`**

```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Lemari ayah di kamar Ibu: E membuka panel gembok. Setelah terbuka, gemboknya hilang dan lemari tidak bisa diinteraksi lagi.
    public class LemariAyah : Interaktif
    {
        [SerializeField] PanelGembok panel;
        [SerializeField] GameObject gembok;

        // Buku harian hanya didapat dari lemari ini, dan inventaris tidak dikosongkan saat malam berganti.
        static bool Terbuka =>
            GameManager.Instance != null && Inventaris.Punya(GameManager.Instance.State, Inventaris.BukuHarianAyah);

        public override string Prompt => "Buka gembok lemari Bapak";
        public override bool BisaDiinteraksi => panel != null && !Terbuka;

        public override void Interaksi() => panel.Buka();

        // State bisa berganti kapan saja (muat save, tertangkap), jadi tampilan gembok selalu mengikuti state.
        void Update()
        {
            if (gembok != null && gembok.activeSelf == Terbuka) gembok.SetActive(!Terbuka);
        }
    }
}
```

- [ ] **Step 4: `PanelJurnal.cs`, tampilkan foto (lewati jika K2-B)**

Di `TeksPapan`, tambahkan tepat setelah loop `for (int b = 0; ...)` (baris `[BUKTI]`):
```csharp
            foreach (var id in s.Foto) sb.Append("[FOTO] ").AppendLine(PapanBukti.JudulFoto(id));
```

- [ ] **Step 5: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"`, tanpa `error CS`.

- [ ] **Step 6: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Runtime
git commit -m "feat: add inventory panel, wardrobe padlock panel, and photos in the journal" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 8: Runtime — alat adegan, obat Ibu, dan ukiran blandar

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/ZonaPemicu.cs`, `InteraktifAlur.cs`, `KameraAdegan.cs`, `PemutarTimeline.cs`, `Suara.cs`, `TerlihatSaatDisorot.cs`, `PemberiObat.cs`, `JadwalObat.cs`
- Modify: `Assets/_Project/Scripts/Runtime/PemainGerak.cs`, `KartuTeks.cs`, `TheSending.Runtime.asmdef`

**Interfaces:**
- Consumes: `GameManager.Sinematik/Jeda` (Task 6), `Hud.Tampilkan`, `PanelDialog.Mulai`, `SenterHP.Menyala`, `KondisiIbu.BeriObat`, `JamGim.MulaiJendelaObat/AkhirJendelaObat/Format` (Task 4), `Inventaris.Punya` (Task 2).
- Produces:
  - `ZonaPemicu` (butuh Collider trigger): `event Action Masuk` (dipicu setiap pemain masuk; pelanggan yang menjaga "sekali saja").
  - `InteraktifAlur : Interaktif`: `event Action Ditekan`.
  - `KameraAdegan`: field `[SerializeField] Camera kameraPemain, kameraAdegan`; `bool Aktif`, `void Mulai(Transform bidikan = null)`, `void Bidik(Transform bidikan)`, `void Selesai()`. (Tidak ada `Tunggu`/`Pindahkan`: waktu dan gerak cutscene ada di Timeline.)
  - `PemutarTimeline : MonoBehaviour, INotificationReceiver` (`[RequireComponent(typeof(PlayableDirector))]`): `event Action<string> Sinyal` (nama `SignalAsset`), `IEnumerator PutarDanTunggu()`. Director-nya disetel `PembuatTimeline` (Task 12).
  - `TheSending.Runtime.asmdef` mereferensikan `Unity.Timeline`.
  - `Suara`: field `[SerializeField] AudioSource sumber`, `[SerializeField] Hud hud`, `[SerializeField] string teksPengganti`; `void Putar()`, `void Hentikan()`.
  - `TerlihatSaatDisorot`: field `senter` (`SenterHP`), `kamera` (`Transform`), `tampilan` (`Renderer`), `hud` (`Hud`), `pesan` (`string`).
  - `PemberiObat : Interaktif`: field `hud`, `panel` (`PanelDialog`), `dialogSetelahObat` (`DataDialog`).
  - `JadwalObat`: field `hud`, `panel`, `teguranLaras` (`DataDialog`), `dentang` (`Suara`).
  - `PemainGerak.Pindah(Transform titik)`, `KartuTeks.HitamSeketika()`.

- [ ] **Step 1: `ZonaPemicu.cs`**

```csharp
using System;
using UnityEngine;

namespace TheSending.Runtime
{
    // Zona (collider trigger) yang memberi tahu setiap kali pemain masuk. Pelanggan yang menentukan apa yang terjadi
    // dan menjaga supaya hanya sekali. Tidak terpicu saat gim dijeda (fisika berhenti).
    [RequireComponent(typeof(Collider))]
    public class ZonaPemicu : MonoBehaviour
    {
        public event Action Masuk;

        void OnTriggerEnter(Collider lain)
        {
            if (lain.GetComponent<PemainGerak>() != null) Masuk?.Invoke();
        }
    }
}
```

- [ ] **Step 2: `InteraktifAlur.cs`**

```csharp
using System;

namespace TheSending.Runtime
{
    // Benda yang aksinya ditentukan sutradara scene (pintu pindah scene, pintu terkunci, dan sejenisnya).
    public class InteraktifAlur : Interaktif
    {
        public event Action Ditekan;

        public override void Interaksi() => Ditekan?.Invoke();
    }
}
```

- [ ] **Step 3: `KameraAdegan.cs`**

```csharp
using UnityEngine;

namespace TheSending.Runtime
{
    // Adegan dan cutscene. Selama adegan: gim dijeda (jam berhenti, input pemain mati), kursor tetap terkunci,
    // dan gambar diambil dari kamera adegan jika ada bidikan. Gerak kamera di cutscene diatur Timeline (PemutarTimeline).
    public class KameraAdegan : MonoBehaviour
    {
        [SerializeField] Camera kameraPemain, kameraAdegan;

        public bool Aktif { get; private set; }

        void Awake()
        {
            if (kameraPemain == null || kameraAdegan == null)
            {
                Debug.LogError("KameraAdegan butuh kamera pemain dan kamera adegan.", this);
                enabled = false;
                return;
            }
            kameraAdegan.enabled = false;
        }

        // Tanpa bidikan: gambar tetap dari mata pemain (misalnya saat layar hitam).
        public void Mulai(Transform bidikan = null)
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("KameraAdegan butuh GameManager di scene.", this);
                return;
            }
            if (!Aktif)
            {
                Aktif = true;
                gm.Sinematik = true;
                gm.Jeda(true);
            }
            if (bidikan != null) Bidik(bidikan);
        }

        public void Bidik(Transform bidikan)
        {
            kameraAdegan.transform.SetPositionAndRotation(bidikan.position, bidikan.rotation);
            kameraPemain.enabled = false;
            kameraAdegan.enabled = true;
        }

        public void Selesai()
        {
            if (!Aktif) return;
            Aktif = false;
            if (kameraAdegan != null) kameraAdegan.enabled = false;
            if (kameraPemain != null) kameraPemain.enabled = true;
            var gm = GameManager.Instance;
            if (gm == null) return;
            gm.Sinematik = false;
            gm.Jeda(false);
        }

        // Scene dibongkar di tengah adegan: lepaskan jeda.
        void OnDisable() => Selesai();
    }
}
```

- [ ] **Step 3a: `PemutarTimeline.cs`**

```csharp
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TheSending.Runtime
{
    // Satu cutscene Timeline. PembuatTimeline (Task 12) membuat asetnya dan menyetel director: Play On Awake mati,
    // Update Method Unscaled Game Time (adegan menjeda gim, timeScale 0), wrap mode None (event stopped terpicu di akhir).
    // Sinyal di marker track timeline sampai ke sini (komponen di objek director) dan diteruskan sebagai nama SignalAsset.
    [RequireComponent(typeof(PlayableDirector))]
    public class PemutarTimeline : MonoBehaviour, INotificationReceiver
    {
        public event Action<string> Sinyal;

        PlayableDirector director;
        bool selesai;

        void Awake()
        {
            director = GetComponent<PlayableDirector>();
            director.stopped += SaatBerhenti;
        }

        void OnDestroy() => director.stopped -= SaatBerhenti;

        void SaatBerhenti(PlayableDirector _) => selesai = true;

        // Dipanggil sutradara: yield return pemutar.PutarDanTunggu();
        public IEnumerator PutarDanTunggu()
        {
            if (director.playableAsset == null)
            {
                Debug.LogError($"PemutarTimeline '{name}' tanpa aset Timeline; cutscene dilewati.", this);
                yield break;
            }
            selesai = false;
            director.time = 0;
            director.Play();
            // Cadangan state: jika wrap mode bukan None, stopped tidak pernah terpicu dan adegan akan terkunci.
            while (!selesai && director.state == PlayState.Playing) yield return null;
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SignalEmitter e && e.asset != null) Sinyal?.Invoke(e.asset.name);
        }
    }
}
```

- [ ] **Step 3b: `TheSending.Runtime.asmdef`, tambah referensi Timeline**

Di `Assets/_Project/Scripts/Runtime/TheSending.Runtime.asmdef`, tambahkan `"Unity.Timeline"` sebagai butir terakhir larik `references` (sesudah `"UnityEngine.UI"`, beri koma di baris sebelumnya). Nama assembly sudah dicek di `Library/PackageCache/com.unity.timeline@*/Runtime/Unity.Timeline.asmdef`.

- [ ] **Step 4: `Suara.cs`**

```csharp
using UnityEngine;

namespace TheSending.Runtime
{
    // Satu bunyi adegan. Jika klipnya belum ada (aset belum didapat tim), teks pengganti tampil di HUD
    // supaya adegan tetap terbaca. Begitu klip dipasang, teks pengganti tidak tampil lagi.
    public class Suara : MonoBehaviour
    {
        [SerializeField] AudioSource sumber;
        [SerializeField] Hud hud;
        [SerializeField] string teksPengganti;

        public void Putar()
        {
            if (sumber != null && sumber.clip != null)
            {
                sumber.Play();
                return;
            }
            if (hud != null) hud.Tampilkan(teksPengganti);
        }

        public void Hentikan()
        {
            if (sumber != null) sumber.Stop();
        }
    }
}
```

- [ ] **Step 5: `TerlihatSaatDisorot.cs`**

```csharp
using UnityEngine;

namespace TheSending.Runtime
{
    // Ukiran yang baru terbaca saat disorot senter HP dari dekat (Game Story Malam 1: angka tahun di blandar).
    // Pasang di objek teks ukiran; objek ini tidak boleh punya collider.
    public class TerlihatSaatDisorot : MonoBehaviour
    {
        const float Jangkauan = 7f, SudutMaks = 20f;

        [SerializeField] SenterHP senter;
        [SerializeField] Transform kamera;
        [SerializeField] Renderer tampilan;
        [SerializeField] Hud hud;
        [SerializeField] string pesan;

        bool sudahDiberitahu;

        void Awake()
        {
            if (senter == null || kamera == null || tampilan == null)
            {
                Debug.LogError($"TerlihatSaatDisorot di '{name}' butuh senter, kamera, dan tampilan.", this);
                enabled = false;
                return;
            }
            tampilan.enabled = false;
        }

        void Update()
        {
            Vector3 ke = transform.position - kamera.position;
            // Disorot = senter menyala, cukup dekat, diarahkan ke ukiran, dan tidak terhalang dinding.
            bool disorot = senter.Menyala && ke.magnitude <= Jangkauan && Vector3.Angle(kamera.forward, ke) <= SudutMaks
                && !Physics.Linecast(kamera.position, transform.position, ~0, QueryTriggerInteraction.Ignore);
            tampilan.enabled = disorot;
            if (!disorot || sudahDiberitahu) return;
            sudahDiberitahu = true;
            if (hud != null) hud.Tampilkan(pesan);
        }
    }
}
```

- [ ] **Step 6: `PemberiObat.cs`**

```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Ibu di ranjang. E memberi obat pada jendela obat (desain 4.4); di luar jendela hanya memberi tahu pemain.
    public class PemberiObat : Interaktif
    {
        [SerializeField] Hud hud;
        [SerializeField] PanelDialog panel;
        [SerializeField] DataDialog dialogSetelahObat;

        public override string Prompt => "Beri obat Ibu";

        public override bool BisaDiinteraksi
        {
            get
            {
                var gm = GameManager.Instance;
                return gm != null && gm.State.MalamKe >= 1 && Inventaris.Punya(gm.State, Inventaris.ObatIbu);
            }
        }

        public override void Interaksi()
        {
            var s = GameManager.Instance.State;
            if (KondisiIbu.BeriObat(s))
            {
                Beri("Ibu meminum obat dan oralit.");
                if (panel != null && dialogSetelahObat != null) panel.Mulai(dialogSetelahObat);
                return;
            }
            float mulai = JamGim.MulaiJendelaObat(s.MalamKe);
            Beri(s.ObatSudahDiberikan ? "Obat malam ini sudah diberikan."
                : s.MenitGim < mulai ? $"Belum waktunya. Obat diminum pukul {JamGim.Format(mulai)}."
                : "Jam obat sudah lewat.");
        }

        void Beri(string pesan)
        {
            if (hud != null) hud.Tampilkan(pesan);
        }
    }
}
```

- [ ] **Step 7: `JadwalObat.cs`**

```csharp
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Pengingat obat Malam 1-6 (desain 4.4): notifikasi HP dan dentang jam saat jendela obat dibuka,
    // teguran Laras jika jendela lewat tanpa obat. Malam 7 diurus adegannya sendiri (Laras memberi obat).
    public class JadwalObat : MonoBehaviour
    {
        [SerializeField] Hud hud;
        [SerializeField] PanelDialog panel;
        [SerializeField] DataDialog teguranLaras;
        [SerializeField] Suara dentang;

        int malamDiingatkan = -1, malamDitegur = -1;

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.Dijeda) return;
            var s = gm.State;
            if (s.MalamKe < 1 || s.MalamKe > 6 || s.ObatSudahDiberikan) return;

            float mulai = JamGim.MulaiJendelaObat(s.MalamKe), akhir = JamGim.AkhirJendelaObat(s.MalamKe);
            if (malamDiingatkan != s.MalamKe && s.MenitGim >= mulai && s.MenitGim < akhir)
            {
                malamDiingatkan = s.MalamKe;
                if (dentang != null) dentang.Putar();
                if (hud != null) hud.Tampilkan($"HP: pukul {JamGim.Format(mulai)}. Waktunya obat Ibu.");
            }
            if (malamDitegur != s.MalamKe && s.MenitGim >= akhir)
            {
                malamDitegur = s.MalamKe;
                if (panel != null && teguranLaras != null) panel.Mulai(teguranLaras);
            }
        }
    }
}
```

- [ ] **Step 8: `PemainGerak.cs`, tambah `Pindah`**

Tambahkan di dalam kelas, tepat sebelum `void Update()`:
```csharp
        // Memindahkan pemain ke titik mulai (awal bab, muat checkpoint, adegan).
        // CharacterController dimatikan dulu supaya posisi baru tidak ditimpa.
        public void Pindah(Transform titik)
        {
            cc.enabled = false;
            transform.SetPositionAndRotation(titik.position, Quaternion.Euler(0f, titik.eulerAngles.y, 0f));
            dongak = 0f;
            kamera.localEulerAngles = Vector3.zero;
            cc.enabled = true;
        }
```

- [ ] **Step 9: `KartuTeks.cs`, tambah `HitamSeketika`**

Tambahkan di dalam kelas, tepat sebelum `public IEnumerator KeHitam()`:
```csharp
        // Layar langsung hitam tanpa memudar. Dipanggil sutradara di Start, sebelum frame pertama tergambar.
        public void HitamSeketika()
        {
            judul.text = "";
            isi.text = "";
            grup.alpha = 1f;
        }
```

- [ ] **Step 10: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"`, tanpa `error CS`.

- [ ] **Step 11: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Runtime
git commit -m "feat: add scene tools, Timeline cutscene player, medicine giving and reminder, and flashlight-revealed carving" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 9: Runtime — sutradara Prolog dan Malam 1

Tiga komponen ini hanya mengurutkan kejadian. Semua perubahan `GameState` lewat Core (`TujuanMalam`, `PergantianMalam`). Nama field `[SerializeField]` di sini dipakai persis oleh pembuat scene di Task 12 dan 13.

Teks yang **dikutip dari Game Story**: judul dan tujuan bab ("Prolog: Pulang", "Selasa sore menjelang magrib", "Sampai di rumah dan bertemu Ibu", "Malam 1: Paku Pertama", "Malam Rabu Wage", "Mengenal rumah, memberi obat, dan mencari tahu apa yang terjadi", "Malam 2: Boneka di Bawah Lantai", "Malam Kamis Kliwon", ringkasan siang Malam 2). Teks yang **ditulis pengembang** (K5): `TeksTelepon`, semua notifikasi tutorial, dan kartu "bersambung".

**Files:**
- Create: `Assets/_Project/Scripts/Runtime/AlurProlog.cs`, `AlurPrologRumah.cs`, `AlurMalam1.cs`

**Interfaces:**
- Consumes: `KameraAdegan`, `PemutarTimeline`, `ZonaPemicu`, `InteraktifAlur`, `Suara`, `PemainGerak.Pindah`, `KartuTeks.HitamSeketika` (Task 8); `PanelDialog.MulaiDanTunggu`, `GameManager.SceneUntukState` (Task 6); `TujuanMalam`, `PergantianMalam`, `JamGim.Jam23`.
- Produces:
  - `AlurProlog`: `const float KabutSebelumMagrib = 0.02f, DetikKabutTurun = 20f`, `const string TeksTelepon`, `const string SinyalMotorPergi = "MotorPergi", SinyalTiraiDitutup = "TiraiDitutup"`. Field: `adegan`, `kartu`, `dialog`, `hud`, `dialogOjek`, `bidikGapura`, `bidikJendela`, `ojek`, `timelineOjek`, `timelineBuSumi`, `zonaSenter`, `zonaAzan`, `zonaBuSumi`, `pintuRumah`, `siluetBuSumi`, `tiraiJendela`, `lampuJendela`, `suaraMotor`, `suaraAzan`, `suaraPintu`.
  - `AlurPrologRumah`: field `adegan`, `kartu`, `dialog`, `hud`, `pemain`, `dialogLaras`, `dialogIbu`, `titikMulai`, `bidikIbu`, `bidanWulan`, `zonaKamarIbu`, `malam1`.
  - `AlurMalam1`: `void Mulai()`, `const string SinyalKejang = "Kejang", SinyalPaku = "Paku", SinyalPadam = "Padam", SinyalLangkah = "Langkah"`. Field `adegan`, `kartu`, `dialog`, `hud`, `pemain`, `dialogPenutup`, `titikMulai`, `bidikIbu`, `timeline23`, `ibu`, `larasRuangTengah`, `larasBerdoa`, `paku`, `darah`, `teplok`, `lampuListrik` (`Light[]`), `suaraBatuk`, `suaraPaku`, `suaraListrik`, `suaraLangkah`, `musikDrone`.
  - Nama sinyal dipakai `PembuatTimeline.Sinyal` di Task 12 dan 13 lewat konstanta ini (bukan string ulang).

- [ ] **Step 1: `AlurProlog.cs`**

```csharp
using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Prolog di Scene_Desa (Game Story, Prolog butir 1-5): telepon Laras, turun dari ojek,
    // tutorial jalan/lari/senter, azan dan kabut turun, Bu Sumi di jendela, sampai pintu rumah.
    public class AlurProlog : MonoBehaviour
    {
        public const float KabutSebelumMagrib = 0.02f, DetikKabutTurun = 20f;
        // Ditulis pengembang dari ringkasan GDD (Laras menelepon sambil menangis; Ibu dua minggu sakit, memuntahkan rambut dan paku).
        public const string TeksTelepon =
            "\"Mas... Mas Raka, pulang, Mas.\"\n\n" +
            "\"Ibu sakit. Sudah dua minggu. Tadi Ibu muntah lagi... ada rambutnya, ada pakunya, Mas.\"\n\n" +
            "\"Aku takut.\"";
        // Nama SignalAsset di Timeline Prolog (PembuatDesa memakai konstanta yang sama).
        public const string SinyalMotorPergi = "MotorPergi", SinyalTiraiDitutup = "TiraiDitutup";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] DataDialog dialogOjek;
        [SerializeField] Transform bidikGapura, bidikJendela, ojek;
        [SerializeField] PemutarTimeline timelineOjek, timelineBuSumi;
        [SerializeField] ZonaPemicu zonaSenter, zonaAzan, zonaBuSumi;
        [SerializeField] InteraktifAlur pintuRumah;
        [SerializeField] GameObject siluetBuSumi, tiraiJendela;
        [SerializeField] Light lampuJendela;
        [SerializeField] Suara suaraMotor, suaraAzan, suaraPintu;   // tanpa suara telepon: suara tokoh ditunda, telepon hanya teks

        float kabutAkhir;
        bool sudahSenter, sudahAzan, sudahBuSumi, sedangMasuk;

        void OnEnable()
        {
            zonaSenter.Masuk += SaatZonaSenter;
            zonaAzan.Masuk += SaatZonaAzan;
            zonaBuSumi.Masuk += SaatZonaBuSumi;
            pintuRumah.Ditekan += SaatPintu;
            timelineOjek.Sinyal += SaatSinyal;
            timelineBuSumi.Sinyal += SaatSinyal;
        }

        void OnDisable()
        {
            zonaSenter.Masuk -= SaatZonaSenter;
            zonaAzan.Masuk -= SaatZonaAzan;
            zonaBuSumi.Masuk -= SaatZonaBuSumi;
            pintuRumah.Ditekan -= SaatPintu;
            timelineOjek.Sinyal -= SaatSinyal;
            timelineBuSumi.Sinyal -= SaatSinyal;
        }

        void SaatSinyal(string nama)
        {
            switch (nama)
            {
                case SinyalMotorPergi: suaraMotor.Putar(); break;
                case SinyalTiraiDitutup: TutupTirai(); break;
                default: Debug.LogWarning($"AlurProlog: sinyal tidak dikenal '{nama}'.", this); break;
            }
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurProlog butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            var s = gm.State;
            if (s.MalamKe != 0 || TujuanMalam.Sudah(s, TujuanMalam.TibaDiRumah))
            {
                // Scene desa dibuka di luar Prolog (fase berikutnya): tidak ada adegan pembuka.
                ojek.gameObject.SetActive(false);
                enabled = false;
                return;
            }
            kabutAkhir = RenderSettings.fogDensity;
            RenderSettings.fogDensity = KabutSebelumMagrib;
            kartu.HitamSeketika();
            StartCoroutine(Pembuka());
        }

        IEnumerator Pembuka()
        {
            adegan.Mulai(bidikGapura);
            yield return kartu.StartCoroutine(kartu.Tampilkan("Telepon dari Laras", TeksTelepon, 8f));
            yield return dialog.MulaiDanTunggu(dialogOjek);
            yield return timelineOjek.PutarDanTunggu();   // ojek menjauh; sinyal MotorPergi memutar suara motor
            ojek.gameObject.SetActive(false);
            adegan.Selesai();
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Prolog: Pulang", "Selasa sore menjelang magrib\n\nTujuan: Sampai di rumah dan bertemu Ibu", 4f));
            hud.Tampilkan("W A S D untuk berjalan. Tahan Shift untuk berlari.");
        }

        void SaatZonaSenter()
        {
            if (sudahSenter) return;
            sudahSenter = true;
            hud.Tampilkan("Kebun bambu gelap. Tekan F untuk menyalakan senter HP.");
        }

        void SaatZonaAzan()
        {
            if (sudahAzan) return;
            sudahAzan = true;
            suaraAzan.Putar();
            StartCoroutine(KabutTurun());
        }

        IEnumerator KabutTurun()
        {
            for (float t = 0f; t < DetikKabutTurun; t += Time.deltaTime)
            {
                RenderSettings.fogDensity = Mathf.Lerp(KabutSebelumMagrib, kabutAkhir, t / DetikKabutTurun);
                yield return null;
            }
            RenderSettings.fogDensity = kabutAkhir;
        }

        void SaatZonaBuSumi()
        {
            if (sudahBuSumi) return;
            sudahBuSumi = true;
            StartCoroutine(BuSumiDiJendela());
        }

        // Game Story: sosok perempuan tua di balik jendela menatap Raka, lalu menutup tirai.
        IEnumerator BuSumiDiJendela()
        {
            adegan.Mulai(bidikJendela);
            yield return timelineBuSumi.PutarDanTunggu();   // kamera mendekat; sinyal TiraiDitutup pada 2,5 detik
            TutupTirai();                                   // cadangan jika sinyal tidak terpicu
            adegan.Selesai();
        }

        void TutupTirai()
        {
            if (!siluetBuSumi.activeSelf) return;   // sekali saja: jangan meredupkan lampu dua kali
            siluetBuSumi.SetActive(false);
            tiraiJendela.SetActive(true);
            lampuJendela.intensity *= 0.3f;
        }

        void SaatPintu()
        {
            if (!sedangMasuk) StartCoroutine(MasukRumah());
        }

        IEnumerator MasukRumah()
        {
            sedangMasuk = true;
            var gm = GameManager.Instance;
            adegan.Mulai();
            suaraPintu.Putar();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            TujuanMalam.Selesaikan(gm.State, TujuanMalam.TibaDiRumah);
            gm.SimpanCheckpoint();              // "Lanjutkan" sesudah ini dimuat di rumah (PetaScene)
            gm.MuatScene(gm.SceneUntukState);   // jeda adegan dilepas KameraAdegan.OnDisable saat scene dibongkar
        }
    }
}
```

- [ ] **Step 2: `AlurPrologRumah.cs`**

```csharp
using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara akhir Prolog di rumah (Game Story, Prolog butir 5-7): disambut Laras, Bidan Wulan memberi obat,
    // tutorial inventaris dan Jurnal, lalu masuk kamar Ibu dan bab berganti ke Malam 1.
    public class AlurPrologRumah : MonoBehaviour
    {
        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] DataDialog dialogLaras, dialogIbu;
        [SerializeField] Transform titikMulai, bidikIbu;
        [SerializeField] GameObject bidanWulan;
        [SerializeField] ZonaPemicu zonaKamarIbu;
        [SerializeField] AlurMalam1 malam1;

        bool bidanPamit, masukKamar;

        void OnEnable()
        {
            zonaKamarIbu.Masuk += SaatMasukKamarIbu;
            if (GameManager.Instance != null) GameManager.Instance.Berubah += SaatBerubah;
        }

        void OnDisable()
        {
            zonaKamarIbu.Masuk -= SaatMasukKamarIbu;
            if (GameManager.Instance != null) GameManager.Instance.Berubah -= SaatBerubah;
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurPrologRumah butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            if (gm.State.MalamKe != 0) return;   // Malam 1 dan seterusnya diurus sutradara malam
            // Scene di-Play langsung dari editor dengan state baru: anggap Raka baru tiba.
            TujuanMalam.Selesaikan(gm.State, TujuanMalam.TibaDiRumah);
            pemain.Pindah(titikMulai);
            kartu.HitamSeketika();
            StartCoroutine(Sambutan());
        }

        IEnumerator Sambutan()
        {
            adegan.Mulai();
            yield return kartu.StartCoroutine(kartu.DariHitam());
            yield return dialog.MulaiDanTunggu(dialogLaras);
            adegan.Selesai();
        }

        // Efek "tujuan:BicaraBidan" ada di baris terakhir dialog Bidan Wulan.
        void SaatBerubah(Perubahan p)
        {
            var s = GameManager.Instance.State;
            if (p != Perubahan.Tujuan || bidanPamit || s.MalamKe != 0 || !TujuanMalam.Sudah(s, TujuanMalam.BicaraBidan)) return;
            bidanPamit = true;
            StartCoroutine(BidanPamit());
        }

        IEnumerator BidanPamit()
        {
            while (dialog.Terbuka) yield return null;
            adegan.Mulai();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            bidanWulan.SetActive(false);   // ia berdiri di ambang pintu kamar Ibu; setelah pamit pintunya bebas
            yield return kartu.StartCoroutine(kartu.DariHitam());
            adegan.Selesai();
            hud.Tampilkan("Bidan Wulan pamit pulang.");
            hud.Tampilkan("Tekan I untuk melihat inventaris.");
            hud.Tampilkan("Tekan Tab untuk membuka Jurnal.");
        }

        void SaatMasukKamarIbu()
        {
            var gm = GameManager.Instance;
            if (masukKamar || gm == null || gm.State.MalamKe != 0) return;
            if (!TujuanMalam.Sudah(gm.State, TujuanMalam.BicaraBidan)) return;
            masukKamar = true;
            StartCoroutine(MasukKamarIbu());
        }

        // Game Story: Raka masuk ke kamar Ibu, Ibu menatap kosong dan berbisik pelan, bab berlanjut ke Malam 1.
        IEnumerator MasukKamarIbu()
        {
            var gm = GameManager.Instance;
            var s = gm.State;
            TujuanMalam.Selesaikan(s, TujuanMalam.MasukKamarIbu);
            if (!PergantianMalam.BisaLanjut(s))
            {
                Debug.LogError("Prolog belum bisa ditutup: ada tujuan wajib yang belum selesai.", this);
                yield break;
            }
            adegan.Mulai(bidikIbu);
            yield return dialog.MulaiDanTunggu(dialogIbu);
            yield return kartu.StartCoroutine(kartu.KeHitam());
            adegan.Selesai();
            PergantianMalam.Lanjut(s);   // Prolog -> Malam 1: jam dan Kondisi Ibu tidak berubah
            gm.SimpanCheckpoint();       // checkpoint awal Malam 1
            malam1.Mulai();
        }
    }
}
```

- [ ] **Step 3: `AlurMalam1.cs`**

```csharp
using System.Collections;
using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // Sutradara Malam 1 (Game Story, Malam 1): kartu bab, cutscene pukul 23.00, tutorial memotret, penutup bab.
    // Obat (PemberiObat, JadwalObat) dan gembok (LemariAyah) berjalan sendiri.
    public class AlurMalam1 : MonoBehaviour
    {
        const float LamaKejang = 3f;
        // Game Story, Malam 2, "Ringkasan siang" (dikutip persis).
        const string RingkasanMalam2 =
            "Ibu tetap menolak dibawa ke rumah sakit dan mengamuk setiap kali dipaksa. " +
            "Raka mulai mendengar bisik-bisik tetangga soal Bu Sumi.";
        // Nama SignalAsset di Timeline Malam1_Cutscene23 (PembuatRumahKeluarga memakai konstanta yang sama).
        public const string SinyalKejang = "Kejang", SinyalPaku = "Paku", SinyalPadam = "Padam", SinyalLangkah = "Langkah";

        [SerializeField] KameraAdegan adegan;
        [SerializeField] KartuTeks kartu;
        [SerializeField] PanelDialog dialog;
        [SerializeField] Hud hud;
        [SerializeField] PemainGerak pemain;
        [SerializeField] DataDialog dialogPenutup;
        [SerializeField] Transform titikMulai, bidikIbu, ibu;
        [SerializeField] PemutarTimeline timeline23;
        [SerializeField] GameObject larasRuangTengah, larasBerdoa, paku, darah, teplok;
        [SerializeField] Light[] lampuListrik = new Light[0];
        [SerializeField] Suara suaraBatuk, suaraPaku, suaraListrik, suaraLangkah, musikDrone;   // tanpa bisikan doa: suara tokoh ditunda

        bool berjalan, sibuk, penutupSudah;

        void OnEnable() => timeline23.Sinyal += SaatSinyal;
        void OnDisable() => timeline23.Sinyal -= SaatSinyal;

        void SaatSinyal(string nama)
        {
            switch (nama)
            {
                case SinyalKejang: suaraBatuk.Putar(); StartCoroutine(Kejang()); break;
                case SinyalPaku: darah.SetActive(true); paku.SetActive(true); suaraPaku.Putar(); break;
                case SinyalPadam: suaraListrik.Putar(); Padamkan(); break;
                case SinyalLangkah: suaraLangkah.Putar(); break;
                default: Debug.LogWarning($"AlurMalam1: sinyal tidak dikenal '{nama}'.", this); break;
            }
        }

        void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("AlurMalam1 butuh GameManager di scene.", this);
                enabled = false;
                return;
            }
            if (gm.State.MalamKe == 1)
            {
                kartu.HitamSeketika();   // "Lanjutkan" di Malam 1
                Mulai();
            }
            else if (gm.State.MalamKe >= 2)
            {
                hud.Tampilkan("Malam 2 belum dibuat (Fase 2).");
            }
        }

        // Dipanggil saat layar hitam: dari akhir Prolog, atau saat scene dimuat di Malam 1.
        public void Mulai()
        {
            if (berjalan) return;
            berjalan = true;
            StartCoroutine(Pembuka());
        }

        IEnumerator Pembuka()
        {
            sibuk = true;
            pemain.Pindah(titikMulai);
            // Save yang dibuat setelah cutscene (tombol uji F5): rumah langsung dalam keadaan sesudahnya.
            if (TujuanMalam.Sudah(GameManager.Instance.State, TujuanMalam.Cutscene23)) KeadaanSetelahAdegan();
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Malam 1: Paku Pertama",
                "Malam Rabu Wage\n\nTujuan: Mengenal rumah, memberi obat, dan mencari tahu apa yang terjadi", 5f));
            hud.Tampilkan("Obat Ibu diberikan pukul 21.00. Jam ada di kanan atas.");
            sibuk = false;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (!berjalan || sibuk || gm == null || gm.Dijeda) return;
            var s = gm.State;
            if (s.MalamKe != 1 || s.Ending != Ending.Belum) return;

            if (!TujuanMalam.Sudah(s, TujuanMalam.Cutscene23))
            {
                if (s.MenitGim >= JamGim.Jam23) StartCoroutine(Cutscene23(s));
                return;
            }
            if (!TujuanMalam.Sudah(s, TujuanMalam.PotretPaku)) return;
            if (!penutupSudah) StartCoroutine(Penutup());
            else if (PergantianMalam.BisaLanjut(s)) StartCoroutine(GantiMalam(s));
        }

        // Game Story: pukul 23.00 Ibu kejang lalu memuntahkan darah dan tiga paku berkarat. Listrik padam.
        // Terdengar langkah berat di atap. Urutan dan kamera ada di Timeline Malam1_Cutscene23 (Task 13):
        // 0 kejang + batuk, 3 paku + darah, 5 listrik padam, 6,5 potong ke plafon + langkah, selesai 11,5 detik.
        IEnumerator Cutscene23(GameState s)
        {
            sibuk = true;
            adegan.Mulai(bidikIbu);
            pemain.Pindah(titikMulai);   // Raka sudah di samping ranjang saat adegan selesai
            musikDrone.Putar();
            yield return timeline23.PutarDanTunggu();
            KeadaanSetelahAdegan();      // juga menjadi cadangan jika ada sinyal yang tidak terpicu
            adegan.Selesai();
            TujuanMalam.Selesaikan(s, TujuanMalam.Cutscene23);
            hud.Tampilkan("Arahkan ke paku di lantai, lalu tekan Q untuk memotretnya.");
            hud.Tampilkan("Foto petunjuk masuk ke Papan Bukti di Jurnal (Tab).");
            sibuk = false;
        }

        // Pengganti animasi kejang sampai model Mixamo datang: tubuh Ibu berguncang.
        IEnumerator Kejang()
        {
            var awal = ibu.localRotation;
            for (float t = 0f; t < LamaKejang; t += Time.unscaledDeltaTime)
            {
                ibu.localRotation = awal * Quaternion.Euler(Random.Range(-6f, 6f), 0f, Random.Range(-6f, 6f));
                yield return null;
            }
            ibu.localRotation = awal;
        }

        void KeadaanSetelahAdegan()
        {
            darah.SetActive(true);
            paku.SetActive(true);
            larasRuangTengah.SetActive(false);
            larasBerdoa.SetActive(true);   // Laras menangis sambil berdoa di samping ranjang
            Padamkan();
        }

        void Padamkan()
        {
            foreach (var lampu in lampuListrik) lampu.enabled = false;
            teplok.SetActive(true);
        }

        IEnumerator Penutup()
        {
            sibuk = true;
            penutupSudah = true;
            yield return dialog.MulaiDanTunggu(dialogPenutup);
            if (!PergantianMalam.BisaLanjut(GameManager.Instance.State))
                hud.Tampilkan("Masih ada yang belum diperiksa: lemari Bapak di kamar Ibu.");
            sibuk = false;
        }

        IEnumerator GantiMalam(GameState s)
        {
            sibuk = true;
            var gm = GameManager.Instance;
            adegan.Mulai();
            yield return kartu.StartCoroutine(kartu.KeHitam());
            musikDrone.Hentikan();
            PergantianMalam.Lanjut(s);        // Malam 2, Kondisi Ibu -12
            if (s.Ending != Ending.Belum)     // catatan 0B: -12 bisa langsung memicu ending
            {
                Debug.LogWarning("Ending tercapai saat pergantian malam; layar ending dibuat di Fase 7.", this);
                gm.MuatScene(GameManager.SceneMenuUtama);
                yield break;
            }
            gm.SimpanCheckpoint();            // checkpoint awal Malam 2
            // Fase 2 mengganti bagian di bawah ini dengan sutradara Malam 2.
            yield return kartu.StartCoroutine(kartu.Tampilkan(
                "Malam 2: Boneka di Bawah Lantai",
                "Malam Kamis Kliwon\n\n" + RingkasanMalam2 + "\n\n(Bersambung. Malam 2 dibuat di Fase 2.)", 8f));
            gm.MuatScene(GameManager.SceneMenuUtama);
        }
    }
}
```

- [ ] **Step 4: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"`, tanpa `error CS`.

- [ ] **Step 5: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Runtime
git commit -m "feat: add Prolog and Malam 1 directors" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 10: Editor — aset pengganti (material, prefab, suara sintetis)

Semua aset di task ini adalah pengganti sementara. Skrip **tidak menimpa** file yang sudah ada, jadi prefab, material, atau suara yang sudah ditukar tim tetap aman saat "Bangun Ulang Semua". Nama file sama persis dengan tabel di `docs/aset/fase-1.md` (Task 1).

**Files:**
- Create: `Assets/_Project/Scripts/Editor/PembuatPengganti.cs`, `Assets/_Project/Scripts/Editor/PembuatSuaraPengganti.cs`
- Modify: `Assets/_Project/Scripts/Editor/PembuatSandbox.cs`
- Dibuat skrip: `Assets/_Project/Art/Materials/Pengganti/*.mat` (12), `Assets/_Project/Prefabs/Pengganti/*.prefab` (24), `Assets/_Project/Audio/SFX/*.wav` (4), `Assets/_Project/Audio/Music/DroneHoror.wav`

**Interfaces:**
- Consumes: `PembuatSandbox.Set` (0C), nama file dari Task 1.
- Produces:
  - `PembuatSandbox`: `Kotak`, `SetTeks`, `SetAngka`, `SetSatuObjek` menjadi `internal`; baru `internal static void SetBool(Object, string, bool)` dan `internal static void SetDaftar(Object, string, Object[])`.
  - `PembuatPengganti` (static): `const string FolderSfx, FolderMusik`; `Material Material(string nama)`; `GameObject Pasang(string namaPrefab, Transform induk, Vector3 posisiLokal, float putarY = 0f)`; `AudioClip Klip(string folder, string nama)` (null jika belum ada); `bool AdaSuara(string folder, string nama)`; `void Buat()` (menu **The Sending > Buat Aset Pengganti**).
  - `PembuatSuaraPengganti.Buat()`: `DentangJam`, `PakuJatuh`, `ListrikPadam`, `LangkahDiAtap` (SFX) dan `DroneHoror` (Music). Azan tidak dibuat.

- [ ] **Step 1: `PembuatSandbox.cs`, buka penyetel field untuk pembuat scene lain**

a. Ganti `static void SetTeks(` menjadi `internal static void SetTeks(`, `static void SetAngka(` menjadi `internal static void SetAngka(`, `static void SetSatuObjek(` menjadi `internal static void SetSatuObjek(`, dan `static GameObject Kotak(` menjadi `internal static GameObject Kotak(`.

b. Tambahkan di dalam kelas, tepat setelah metode `SetSatuObjek`:
```csharp
        internal static void SetBool(Object komponen, string nama, bool nilai)
        {
            var so = new SerializedObject(komponen);
            Properti(so, nama).boolValue = nilai;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        internal static void SetDaftar(Object komponen, string nama, Object[] nilai)
        {
            var so = new SerializedObject(komponen);
            var p = Properti(so, nama);
            p.arraySize = nilai.Length;
            for (int i = 0; i < nilai.Length; i++)
                p.GetArrayElementAtIndex(i).objectReferenceValue = nilai[i] ?? throw new ArgumentNullException($"{nama}[{i}]");
            so.ApplyModifiedPropertiesWithoutUndo();
        }
```

- [ ] **Step 2: `PembuatPengganti.cs`**

Ukuran dalam meter. Setiap prefab: depan = +Z, titik nol di lantai (kecuali benda dinding dan `Gembok`: titik nol di tengah benda; `BuRatmi_Berbaring`: titik nol di permukaan kasur, kepala ke -Z).

```csharp
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheSending.EditorTools
{
    // Aset pengganti sampai tim mendapat aset asli: material polos dan prefab dari primitif.
    // TIDAK menimpa file yang sudah ada, supaya prefab dan material yang sudah ditukar tim aman.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatPengganti.Buat
    public static class PembuatPengganti
    {
        const string FolderMaterial = "Assets/_Project/Art/Materials/Pengganti";
        const string FolderPrefab = "Assets/_Project/Prefabs/Pengganti";
        internal const string FolderSfx = "Assets/_Project/Audio/SFX", FolderMusik = "Assets/_Project/Audio/Music";
        static readonly string[] AkhiranSuara = { ".wav", ".ogg", ".mp3" };

        static readonly (string nama, string hex)[] DaftarMaterial =
        {
            ("KayuTua", "5B4030"), ("KayuGelap", "2E2018"), ("Gedek", "8A7A55"), ("Tanah", "3B3026"),
            ("Genteng", "6B3A2A"), ("Bambu", "6F7F3A"), ("Kain", "B9AE96"), ("Kulit", "C69C7B"),
            ("Karat", "5A1A12"), ("Kertas", "E6E0D1"), ("Logam", "6E7074"), ("Gelap", "0B0D10"),
        };

        readonly struct Bagian
        {
            public readonly PrimitiveType Bentuk;
            public readonly string Material;
            public readonly Vector3 Posisi, Ukuran, Putar;

            public Bagian(PrimitiveType bentuk, string material, Vector3 posisi, Vector3 ukuran, Vector3 putar)
            {
                Bentuk = bentuk; Material = material; Posisi = posisi; Ukuran = ukuran; Putar = putar;
            }
        }

        // K = kotak, T = tabung (tinggi = 2 x sy), B = bola. Posisi = titik tengah bagian.
        static Bagian K(string m, float x, float y, float z, float sx, float sy, float sz, float rx = 0f, float ry = 0f, float rz = 0f) =>
            new Bagian(PrimitiveType.Cube, m, new Vector3(x, y, z), new Vector3(sx, sy, sz), new Vector3(rx, ry, rz));
        static Bagian T(string m, float x, float y, float z, float sx, float sy, float sz, float rx = 0f, float ry = 0f, float rz = 0f) =>
            new Bagian(PrimitiveType.Cylinder, m, new Vector3(x, y, z), new Vector3(sx, sy, sz), new Vector3(rx, ry, rz));
        static Bagian B(string m, float x, float y, float z, float s) =>
            new Bagian(PrimitiveType.Sphere, m, new Vector3(x, y, z), Vector3.one * s, Vector3.zero);

        // Sosok berdiri setinggi 1,67 m dari kotak-kotak.
        static Bagian[] Sosok(string baju, string bawah, string kulit = "Kulit") => new[]
        {
            K(bawah, 0f, 0.4f, 0f, 0.3f, 0.8f, 0.2f),
            K(baju, 0f, 1.1f, 0f, 0.4f, 0.6f, 0.22f),
            K(baju, -0.27f, 1.1f, 0f, 0.1f, 0.55f, 0.1f),
            K(baju, 0.27f, 1.1f, 0f, 0.1f, 0.55f, 0.1f),
            K(kulit, 0f, 1.52f, 0f, 0.22f, 0.24f, 0.22f),
            K("Gelap", 0f, 1.62f, -0.02f, 0.24f, 0.1f, 0.24f),
        };

        static readonly (string nama, Bagian[] bagian)[] DaftarPrefab =
        {
            // Tokoh
            ("Laras", Sosok("Kain", "KayuGelap")),
            ("Laras_Berdoa", new[]
            {
                K("KayuGelap", 0f, 0.15f, -0.2f, 0.3f, 0.3f, 0.5f),
                K("Kain", 0f, 0.6f, 0f, 0.4f, 0.6f, 0.22f),
                K("Kain", 0f, 0.75f, 0.18f, 0.3f, 0.1f, 0.25f),
                K("Kulit", 0f, 1.0f, 0.03f, 0.22f, 0.24f, 0.22f, 20f),
                K("Gelap", 0f, 1.1f, 0f, 0.24f, 0.1f, 0.24f, 20f),
            }),
            ("BidanWulan", Sosok("Kertas", "Logam")),
            ("BuRatmi_Berbaring", new[]
            {
                K("Kain", 0f, 0.12f, 0.1f, 0.45f, 0.2f, 1.3f),
                K("Kulit", 0f, 0.12f, -0.72f, 0.22f, 0.2f, 0.24f),
                K("Gelap", 0f, 0.1f, -0.86f, 0.24f, 0.18f, 0.08f),
            }),
            ("BuSumi_Siluet", Sosok("Gelap", "Gelap", "Gelap")),
            ("Ojek", new[]
            {
                K("Logam", 0f, 0.5f, 0f, 0.25f, 0.4f, 1.6f),
                T("Gelap", 0f, 0.3f, 0.7f, 0.6f, 0.05f, 0.6f, 0f, 0f, 90f),
                T("Gelap", 0f, 0.3f, -0.7f, 0.6f, 0.05f, 0.6f, 0f, 0f, 90f),
                K("Kertas", 0f, 0.85f, 0.8f, 0.15f, 0.12f, 0.05f),
                K("KayuGelap", 0f, 1.15f, -0.1f, 0.4f, 0.6f, 0.25f),
                B("Gelap", 0f, 1.6f, -0.1f, 0.3f),
            }),
            // Perabot dan benda rumah
            ("Ranjang", new[]
            {
                K("KayuTua", 0f, 0.25f, 0f, 1.2f, 0.3f, 2.0f),
                K("Kain", 0f, 0.47f, 0f, 1.1f, 0.14f, 1.9f),
                K("KayuGelap", 0f, 0.6f, -1.0f, 1.2f, 0.8f, 0.08f),
            }),
            ("LemariAyah", new[]
            {
                K("KayuGelap", 0f, 1.0f, 0f, 1.2f, 2.0f, 0.6f),
                K("KayuTua", 0f, 1.0f, 0.305f, 0.03f, 1.8f, 0.01f),
            }),
            ("Gembok", new[]
            {
                K("Logam", 0f, 0f, 0f, 0.1f, 0.12f, 0.04f),
                K("Logam", 0f, 0.09f, 0f, 0.07f, 0.06f, 0.02f),
            }),
            ("FotoKeluarga", new[]
            {
                K("KayuGelap", 0f, 0f, 0f, 0.6f, 0.45f, 0.04f),
                K("Kertas", 0f, 0f, 0.022f, 0.5f, 0.35f, 0.01f),
            }),
            ("JamDinding", new[]
            {
                K("KayuGelap", 0f, 0f, 0f, 0.3f, 0.6f, 0.1f),
                T("Kertas", 0f, 0.12f, 0.055f, 0.22f, 0.005f, 0.22f, 90f),
                K("Logam", 0f, -0.15f, 0.055f, 0.04f, 0.2f, 0.01f),
            }),
            ("Kalender", new[]
            {
                K("Kertas", 0f, 0f, 0f, 0.35f, 0.5f, 0.02f),
                K("Karat", 0f, 0.2f, 0.012f, 0.35f, 0.1f, 0.01f),
            }),
            ("Meja", new[]
            {
                K("KayuTua", 0f, 0.73f, 0f, 1.0f, 0.05f, 0.6f),
                K("KayuTua", -0.45f, 0.35f, -0.25f, 0.06f, 0.7f, 0.06f),
                K("KayuTua", 0.45f, 0.35f, -0.25f, 0.06f, 0.7f, 0.06f),
                K("KayuTua", -0.45f, 0.35f, 0.25f, 0.06f, 0.7f, 0.06f),
                K("KayuTua", 0.45f, 0.35f, 0.25f, 0.06f, 0.7f, 0.06f),
            }),
            ("Kursi", new[]
            {
                K("KayuGelap", 0f, 0.22f, 0f, 0.34f, 0.44f, 0.34f),
                K("KayuTua", 0f, 0.45f, 0f, 0.4f, 0.05f, 0.4f),
                K("KayuTua", 0f, 0.75f, -0.18f, 0.4f, 0.55f, 0.04f),
            }),
            ("Paku", new[]
            {
                T("Karat", -0.08f, 0.02f, 0.03f, 0.02f, 0.07f, 0.02f, 90f, 25f),
                T("Karat", 0.05f, 0.02f, -0.04f, 0.02f, 0.07f, 0.02f, 90f, -40f),
                T("Karat", 0.1f, 0.02f, 0.09f, 0.02f, 0.07f, 0.02f, 90f, 80f),
            }),
            ("Teplok", new[]
            {
                T("Logam", 0f, 0.06f, 0f, 0.1f, 0.06f, 0.1f),
                T("Kertas", 0f, 0.2f, 0f, 0.06f, 0.08f, 0.06f),
            }),
            ("StopKontak", new[] { K("Kertas", 0f, 0f, 0f, 0.1f, 0.1f, 0.03f) }),
            ("SakaGuru", new[]
            {
                K("Tanah", 0f, 0.1f, 0f, 0.45f, 0.2f, 0.45f),
                K("KayuGelap", 0f, 1.5f, 0f, 0.3f, 3.0f, 0.3f),
            }),
            // Jalan masuk desa
            ("Gapura", new[]
            {
                K("Genteng", -2.5f, 1.75f, 0f, 0.8f, 3.5f, 0.8f),
                K("Genteng", 2.5f, 1.75f, 0f, 0.8f, 3.5f, 0.8f),
                K("KayuGelap", -2.5f, 3.6f, 0f, 1.1f, 0.2f, 1.1f),
                K("KayuGelap", 2.5f, 3.6f, 0f, 1.1f, 0.2f, 1.1f),
                K("KayuTua", 0f, 3.3f, 0f, 5.0f, 0.25f, 0.3f),
            }),
            ("JembatanBambu", new[]
            {
                K("Bambu", 0f, 0.05f, 0f, 2.0f, 0.1f, 8.0f),
                K("Bambu", -1.0f, 0.3f, 0f, 0.08f, 0.08f, 8.0f),
                K("Bambu", -1.0f, 0.7f, 0f, 0.08f, 0.08f, 8.0f),
                K("Bambu", 1.0f, 0.3f, 0f, 0.08f, 0.08f, 8.0f),
                K("Bambu", 1.0f, 0.7f, 0f, 0.08f, 0.08f, 8.0f),
            }),
            ("RumpunBambu", new[]
            {
                T("Bambu", 0f, 2.5f, 0f, 0.1f, 2.5f, 0.1f),
                T("Bambu", 0.3f, 2.5f, 0.2f, 0.1f, 2.5f, 0.1f, 0f, 0f, 4f),
                T("Bambu", -0.3f, 2.5f, 0.1f, 0.1f, 2.5f, 0.1f, 0f, 0f, -4f),
                T("Bambu", 0.1f, 2.5f, -0.3f, 0.1f, 2.5f, 0.1f, 4f),
                T("Bambu", -0.2f, 2.5f, -0.2f, 0.1f, 2.5f, 0.1f, -3f),
            }),
            ("LampuJalan", new[]
            {
                T("KayuGelap", 0f, 1.5f, 0f, 0.08f, 1.5f, 0.08f),
                K("KayuGelap", 0.25f, 2.9f, 0f, 0.5f, 0.06f, 0.06f),
                K("Logam", 0.5f, 2.8f, 0f, 0.25f, 0.12f, 0.25f),
            }),
            // Rumah Bu Sumi dari luar: 6 x 3 x 5 m, jendela di (1.2, 1.5, 2.5).
            ("RumahKecil", new[]
            {
                K("Gedek", 0f, 1.5f, 0f, 6f, 3f, 5f),
                K("Genteng", 0f, 3.3f, 0f, 6.6f, 0.6f, 5.6f),
                K("KayuGelap", -1.5f, 1.0f, 2.51f, 0.9f, 2.0f, 0.04f),
                K("KayuGelap", 1.2f, 1.5f, 2.51f, 1.3f, 1.3f, 0.04f),
                K("Gelap", 1.2f, 1.5f, 2.535f, 1.1f, 1.1f, 0.01f),
            }),
            // Rumah keluarga dari luar: titik nol di ambang pintu depan, badan rumah ke arah -Z.
            ("RumahKeluargaLuar", new[]
            {
                K("Gedek", 0f, 1.5f, -6f, 14f, 3f, 12f),
                K("Genteng", 0f, 3.6f, -6f, 15f, 1.2f, 13f),
                K("KayuGelap", -2f, 1.5f, 1.2f, 0.25f, 3f, 0.25f),
                K("KayuGelap", 2f, 1.5f, 1.2f, 0.25f, 3f, 0.25f),
                K("Genteng", 0f, 3.1f, 0.8f, 6f, 0.15f, 2f),
                K("KayuGelap", 0f, 1.1f, 0.02f, 1.2f, 2.2f, 0.06f),
            }),
        };

        [MenuItem("The Sending/Buat Aset Pengganti")]
        public static void Buat()
        {
            foreach (var folder in new[] { FolderMaterial, FolderPrefab, FolderSfx, FolderMusik })
                Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
            foreach (var m in DaftarMaterial) Material(m.nama);
            foreach (var p in DaftarPrefab) BuatPrefab(p.nama, p.bagian);
            PembuatSuaraPengganti.Buat();
            AssetDatabase.SaveAssets();
            Debug.Log($"Aset pengganti siap: {DaftarMaterial.Length} material, {DaftarPrefab.Length} prefab.");
        }

        internal static Material Material(string nama)
        {
            string jalur = $"{FolderMaterial}/{nama}.mat";
            var ada = AssetDatabase.LoadAssetAtPath<Material>(jalur);
            if (ada != null) return ada;

            string hex = Array.Find(DaftarMaterial, d => d.nama == nama).hex
                ?? throw new ArgumentException($"Material pengganti '{nama}' tidak ada di daftar.");
            var shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? throw new InvalidOperationException("Shader 'Universal Render Pipeline/Lit' tidak ditemukan.");
            var baru = new Material(shader);
            if (!ColorUtility.TryParseHtmlString("#" + hex, out var warna))
                throw new FormatException($"Warna material '{nama}' salah: {hex}");
            baru.SetColor("_BaseColor", warna);
            baru.SetFloat("_Smoothness", 0f);
            AssetDatabase.CreateAsset(baru, jalur);
            return baru;
        }

        // Menaruh prefab visual sebagai anak objek logika (induk null = di akar scene). Posisi dan putaran lokal.
        internal static GameObject Pasang(string namaPrefab, Transform induk, Vector3 posisiLokal, float putarY = 0f)
        {
            string jalur = $"{FolderPrefab}/{namaPrefab}.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(jalur)
                ?? throw new FileNotFoundException($"Prefab pengganti belum ada: {jalur}. Jalankan PembuatPengganti.Buat.");
            var g = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            g.transform.SetParent(induk, false);
            g.transform.localPosition = posisiLokal;
            g.transform.localRotation = Quaternion.Euler(0f, putarY, 0f);
            return g;
        }

        internal static bool AdaSuara(string folder, string nama) =>
            Array.Exists(AkhiranSuara, akhiran => File.Exists($"{folder}/{nama}{akhiran}"));

        // Klip suara dengan nama dasar itu (.wav, .ogg, atau .mp3). Null = belum ada; adegan memakai teks pengganti.
        internal static AudioClip Klip(string folder, string nama)
        {
            foreach (var akhiran in AkhiranSuara)
            {
                var klip = AssetDatabase.LoadAssetAtPath<AudioClip>($"{folder}/{nama}{akhiran}");
                if (klip != null) return klip;
            }
            return null;
        }

        static void BuatPrefab(string nama, Bagian[] bagian)
        {
            string jalur = $"{FolderPrefab}/{nama}.prefab";
            if (File.Exists(jalur)) return;   // sudah ada (mungkin sudah ditukar tim): jangan ditimpa

            var akar = new GameObject(nama);
            foreach (var b in bagian)
            {
                var g = GameObject.CreatePrimitive(b.Bentuk);
                g.name = b.Material;
                g.transform.SetParent(akar.transform, false);
                g.transform.localPosition = b.Posisi;
                g.transform.localRotation = Quaternion.Euler(b.Putar);
                g.transform.localScale = b.Ukuran;
                g.GetComponent<Renderer>().sharedMaterial = Material(b.Material);
            }
            PrefabUtility.SaveAsPrefabAsset(akar, jalur);
            Object.DestroyImmediate(akar);
        }
    }
}
```

- [ ] **Step 3: `PembuatSuaraPengganti.cs`**

```csharp
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Lima WAV sintetis (mono, 16 bit, 22050 Hz) sebagai pengganti sampai tim mendapat rekaman asli.
    // Tidak menimpa file dengan nama dasar yang sama. Azan tidak pernah dibuat sintetis.
    public static class PembuatSuaraPengganti
    {
        const int Hz = 22050;

        public static void Buat()
        {
            Tulis(PembuatPengganti.FolderSfx, "DentangJam", Dentang());
            Tulis(PembuatPengganti.FolderSfx, "PakuJatuh", LogamJatuh());
            Tulis(PembuatPengganti.FolderSfx, "ListrikPadam", ListrikPadam());
            Tulis(PembuatPengganti.FolderSfx, "LangkahDiAtap", Langkah());
            Tulis(PembuatPengganti.FolderMusik, "DroneHoror", Drone());
        }

        static void Tulis(string folder, string nama, float[] sampel)
        {
            if (PembuatPengganti.AdaSuara(folder, nama)) return;

            string jalur = $"{folder}/{nama}.wav";
            int byteData = sampel.Length * 2;
            using (var w = new BinaryWriter(File.Create(jalur)))
            {
                w.Write(Encoding.ASCII.GetBytes("RIFF"));
                w.Write(36 + byteData);
                w.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
                w.Write(16);            // ukuran blok fmt
                w.Write((short)1);      // PCM
                w.Write((short)1);      // mono
                w.Write(Hz);
                w.Write(Hz * 2);        // byte per detik
                w.Write((short)2);      // byte per sampel
                w.Write((short)16);     // bit per sampel
                w.Write(Encoding.ASCII.GetBytes("data"));
                w.Write(byteData);
                foreach (float s in sampel) w.Write((short)(Mathf.Clamp(s, -1f, 1f) * 32767f));
            }
            AssetDatabase.ImportAsset(jalur);
            var klip = AssetDatabase.LoadAssetAtPath<AudioClip>(jalur);
            if (klip == null || klip.samples != sampel.Length)
                throw new InvalidOperationException($"WAV sintetis gagal diimpor atau panjangnya salah: {jalur}");
        }

        static float[] Kosong(float detik) => new float[(int)(detik * Hz)];

        // Menambahkan nada sinus yang meluruh, mulai detik ke-`mulai`. luruh 0 = keras tetap.
        static void Nada(float[] s, float mulai, float frek, float lama, float keras, float luruh)
        {
            int awal = (int)(mulai * Hz), n = (int)(lama * Hz);
            for (int i = 0; i < n && awal + i < s.Length; i++)
            {
                float t = i / (float)Hz;
                s[awal + i] += keras * Mathf.Exp(-luruh * t) * Mathf.Sin(2f * Mathf.PI * frek * t);
            }
        }

        // Menambahkan derau yang meluruh. Benih tetap supaya file yang dibuat ulang sama persis.
        static void Derau(float[] s, float mulai, float lama, float keras, float luruh, int benih)
        {
            var acak = new System.Random(benih);
            int awal = (int)(mulai * Hz), n = (int)(lama * Hz);
            for (int i = 0; i < n && awal + i < s.Length; i++)
                s[awal + i] += keras * Mathf.Exp(-luruh * i / Hz) * (float)(acak.NextDouble() * 2.0 - 1.0);
        }

        // Tiga dentang, jeda 1,2 detik.
        static float[] Dentang()
        {
            var s = Kosong(4.5f);
            for (int k = 0; k < 3; k++)
            {
                Nada(s, k * 1.2f, 660f, 2f, 0.45f, 2.5f);
                Nada(s, k * 1.2f, 1320f, 2f, 0.2f, 4f);
            }
            return s;
        }

        // Tiga denting logam pendek.
        static float[] LogamJatuh()
        {
            var s = Kosong(1.5f);
            float[] mulai = { 0f, 0.25f, 0.6f }, frek = { 2600f, 3100f, 2850f };
            for (int k = 0; k < 3; k++)
            {
                Nada(s, mulai[k], frek[k], 0.4f, 0.4f, 14f);
                Derau(s, mulai[k], 0.03f, 0.3f, 80f, 10 + k);
            }
            return s;
        }

        // Dengung 50 Hz, klik, lalu nada rendah yang meluruh.
        static float[] ListrikPadam()
        {
            var s = Kosong(1.6f);
            Nada(s, 0f, 50f, 0.8f, 0.3f, 0f);
            Nada(s, 0f, 100f, 0.8f, 0.15f, 0f);
            Derau(s, 0.8f, 0.05f, 0.8f, 60f, 3);
            Nada(s, 0.8f, 40f, 0.8f, 0.4f, 5f);
            return s;
        }

        // Lima hentakan berat, jeda 0,9 detik.
        static float[] Langkah()
        {
            var s = Kosong(5f);
            for (int k = 0; k < 5; k++)
            {
                Nada(s, 0.3f + k * 0.9f, 55f, 0.5f, 0.8f, 9f);
                Derau(s, 0.3f + k * 0.9f, 0.12f, 0.25f, 30f, 20 + k);
            }
            return s;
        }

        // Loop 20 detik. Semua frekuensi kelipatan 0,05 Hz, jadi setiap nada genap putarannya dan sambungan loop mulus.
        static float[] Drone()
        {
            var s = Kosong(20f);
            Nada(s, 0f, 55f, 20f, 0.3f, 0f);
            Nada(s, 0f, 58.05f, 20f, 0.25f, 0f);
            Nada(s, 0f, 82.4f, 20f, 0.15f, 0f);
            Nada(s, 0f, 110.1f, 20f, 0.1f, 0f);
            return s;
        }
    }
}
```

- [ ] **Step 4: Jalankan pembuat aset pengganti lewat batchmode**

```bash
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod TheSending.EditorTools.PembuatPengganti.Buat -logFile "${TMPDIR:-/tmp}/the-sending-pengganti.log"; echo "exit=$?"
grep -E "Aset pengganti siap|error CS|Exception" "${TMPDIR:-/tmp}/the-sending-pengganti.log" | sort -u | head
ls Assets/_Project/Art/Materials/Pengganti/*.mat | wc -l
ls Assets/_Project/Prefabs/Pengganti/*.prefab | wc -l
ls Assets/_Project/Audio/SFX/*.wav Assets/_Project/Audio/Music/*.wav
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
```
Expected: `exit=0`; baris `Aset pengganti siap: 12 material, 24 prefab.`; tanpa `error CS` dan tanpa `Exception`; `12`; `24`; lima file: `DentangJam.wav`, `LangkahDiAtap.wav`, `ListrikPadam.wav`, `PakuJatuh.wav` di `SFX/` dan `DroneHoror.wav` di `Music/`. Jika tim sudah menaruh rekaman asli dengan nama dasar yang sama, file `.wav` sintetisnya memang tidak dibuat; jumlah file berkurang dan itu benar.

- [ ] **Step 5: Jalankan lagi, pastikan tidak ada yang ditimpa**

Jalankan perintah Step 4 sekali lagi, lalu:
```bash
git status --short Assets/_Project/Art/Materials Assets/_Project/Prefabs Assets/_Project/Audio
```
Expected: daftar yang sama dengan sebelum run kedua (semua masih `??`, tidak ada file yang berubah isi; cek cepat dengan `shasum Assets/_Project/Audio/SFX/DentangJam.wav` sebelum dan sesudah, hasilnya sama).

- [ ] **Step 6: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"`, tanpa `error CS`.

- [ ] **Step 7: Commit**

File `.wav` masuk lewat Git LFS (`.gitattributes`). `git lfs install` harus sudah pernah dijalankan.
```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Editor Assets/_Project/Art/Materials Assets/_Project/Art/Materials.meta Assets/_Project/Prefabs Assets/_Project/Prefabs.meta Assets/_Project/Audio/SFX Assets/_Project/Audio/SFX.meta Assets/_Project/Audio/Music Assets/_Project/Audio/Music.meta
git commit -m "feat: add placeholder materials, prefabs and synthetic sounds" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 11: Editor — data dialog Fase 1 dan isi Jurnal

Sepuluh aset `DataDialog` dan empat isi Jurnal. **Rencana ini sengaja tidak memuat kalimat cerita.** Kode di Step 1 berisi penanda, dan Step 2 mengganti setiap penanda dengan teks dari dokumen sumber:

- `[[SALIN GS:n]]` = salin **persis** kalimat pada baris `n` di `docs/TheSending_FullGameStory.md` (hanya isi kalimatnya: tanpa nama pembicara, tanpa tanda `*`, tanpa `\`).
- `[[TULIS ...]]` = teks yang tidak ada kalimatnya di dokumen (K5). Implementer menulisnya dalam bahasa Indonesia sehari-hari, mengikuti keterangan di tabel Step 2. Tim menyuntingnya nanti di file ini.

Pembuat menolak berjalan selama masih ada penanda (`TanpaPenanda`), jadi penanda tidak mungkin ikut masuk gim.

**Files:**
- Create: `Assets/_Project/Scripts/Editor/PembuatDataFase1.cs`
- Dibuat skrip: `Assets/_Project/Data/Fase1/*.asset` (10); `Assets/_Project/Data/Jurnal.asset` berubah (4 isi)

**Interfaces:**
- Consumes: `DataDialog`, `DataJurnal`, `BarisDialog`, `PemeriksaDialog.Periksa` (Task 5), efek `barang:` (Task 2), `tujuan:`.
- Produces: `PembuatDataFase1` (static): konstanta nama aset `Ojek`, `LarasSambutan`, `BidanWulan`, `IbuProlog`, `LarasKronologi`, `IbuSetelahObat`, `TeguranObat`, `FotoKeluarga`, `PenutupMalam1`, `Kalender`; `DataDialog Muat(string nama)`; `void Buat()` (menu **The Sending > Buat Data Fase 1**). Aset yang sudah ada **diisi ulang di tempat** (GUID tetap), jadi referensi dari scene tidak putus.

| Aset | Baris | Pembicara berurutan | Efek | Dipakai oleh |
|---|---|---|---|---|
| `Ojek` | 1 | Tukang Ojek | — | `AlurProlog.dialogOjek` |
| `LarasSambutan` | 3 | Laras, Raka, Laras | — | `AlurPrologRumah.dialogLaras` |
| `BidanWulan` | 3 | Bidan Wulan, Raka, Bidan Wulan | baris 1 `barang:ObatIbu`; baris 3 `tujuan:BicaraBidan` | `PemicuDialog` Bidan Wulan |
| `IbuProlog` | 3 | Raka, Ibu, Raka | — | `AlurPrologRumah.dialogIbu` |
| `LarasKronologi` | 5 | Laras, Raka, Laras, Raka, Laras | — | `PemicuDialog` Laras |
| `IbuSetelahObat` | 1 | Ibu (dengan terjemahan) | — | `PemberiObat.dialogSetelahObat` |
| `TeguranObat` | 2 | Laras, Raka | — | `JadwalObat.teguranLaras` |
| `FotoKeluarga` | 2 | Raka, Tulisan Bapak | — | `PemicuDialog` foto keluarga |
| `PenutupMalam1` | 2 | Laras, Raka | — | `AlurMalam1.dialogPenutup` |
| `Kalender` | 1 | Raka | — | `PemicuDialog` kalender dapur |

- [ ] **Step 1: Tulis `PembuatDataFase1.cs` (masih berisi penanda)**

```csharp
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Aset dialog Fase 1 (Prolog + Malam 1) dan isi Jurnal yang terbuka di Fase 1.
    // Tim menyunting teks DI FILE INI lalu menjalankan The Sending > Bangun Ulang Semua (K5).
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatDataFase1.Buat
    public static class PembuatDataFase1
    {
        const string Folder = "Assets/_Project/Data/Fase1";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        internal const string Ojek = "Ojek", LarasSambutan = "LarasSambutan", BidanWulan = "BidanWulan",
            IbuProlog = "IbuProlog", LarasKronologi = "LarasKronologi", IbuSetelahObat = "IbuSetelahObat",
            TeguranObat = "TeguranObat", FotoKeluarga = "FotoKeluarga", PenutupMalam1 = "PenutupMalam1",
            Kalender = "Kalender";

        // Game Story 10.3: Halaman 2 adalah tabel aksara Jawa. Bentuk aksaranya (gambar atau font) datang di Fase 2
        // bersama puzzle aksara; di Fase 1 halaman ini berisi urutan bacanya saja.
        const string IsiHalaman2 =
            "ha  na  ca  ra  ka\n" +
            "da  ta  sa  wa  la\n" +
            "pa  dha  ja  ya  nya\n" +
            "ma  ga  ba  tha  nga";

        [MenuItem("The Sending/Buat Data Fase 1")]
        public static void Buat()
        {
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();

            // Prolog
            Simpan(Ojek,
                B("Tukang Ojek", "[[SALIN GS:384]]"));
            Simpan(LarasSambutan,
                B("Laras", "[[TULIS sambutan-1]]"),
                B("Raka", "[[TULIS sambutan-2]]"),
                B("Laras", "[[TULIS sambutan-3]]"));
            Simpan(BidanWulan,
                B("Bidan Wulan", "[[SALIN GS:386]]", efek: "barang:ObatIbu"),
                B("Raka", "[[SALIN GS:388]]"),
                B("Bidan Wulan", "[[SALIN GS:390]]", efek: "tujuan:BicaraBidan"));
            Simpan(IbuProlog,
                B("Raka", "[[TULIS ibu-prolog-1]]"),
                B("Ibu", "..."),
                B("Raka", "[[TULIS ibu-prolog-3]]"));

            // Malam 1
            Simpan(LarasKronologi,
                B("Laras", "[[TULIS kronologi-1]]"),
                B("Raka", "[[TULIS kronologi-2]]"),
                B("Laras", "[[TULIS kronologi-3]]"),
                B("Raka", "[[TULIS kronologi-4]]"),
                B("Laras", "[[TULIS kronologi-5]]"));
            Simpan(IbuSetelahObat,
                B("Ibu", "[[SALIN GS:422 jawa]]", "[[SALIN GS:422 terjemahan]]"));
            Simpan(TeguranObat,
                B("Laras", "[[TULIS teguran-1]]"),
                B("Raka", "[[TULIS teguran-2]]"));
            Simpan(FotoKeluarga,
                B("Raka", "[[TULIS foto-1]]"),
                B("Tulisan Bapak", "[[SALIN GS:418 kutipan]]"));
            Simpan(PenutupMalam1,
                B("Laras", "[[SALIN GS:424]]"),
                B("Raka", "[[SALIN GS:426]]"));
            Simpan(Kalender,
                B("Raka", "[[TULIS kalender-1]]"));

            IsiJurnal();
            AssetDatabase.SaveAssets();
            Debug.Log($"Data Fase 1 dibuat: 10 dialog di {Folder}, 4 isi Jurnal.");
        }

        internal static DataDialog Muat(string nama) =>
            AssetDatabase.LoadAssetAtPath<DataDialog>($"{Folder}/{nama}.asset")
            ?? throw new FileNotFoundException($"Dialog Fase 1 belum ada: {Folder}/{nama}.asset. Jalankan PembuatDataFase1.Buat.");

        static BarisDialog B(string pembicara, string teks, string terjemahan = "", string efek = "") =>
            new BarisDialog { Pembicara = pembicara, Teks = teks, Terjemahan = terjemahan, Efek = efek };

        // Baris terakhir selalu menutup percakapan. Aset yang sudah ada diisi ulang di tempat supaya GUID tetap.
        static void Simpan(string nama, params BarisDialog[] baris)
        {
            foreach (var b in baris)
            {
                if (string.IsNullOrWhiteSpace(b.Teks)) throw new InvalidDataException($"Dialog '{nama}': ada baris tanpa teks.");
                TanpaPenanda(nama, b.Teks, b.Terjemahan);
            }
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

        // Jurnal.asset dibuat PembuatSandbox (0B). Di sini hanya empat isi yang terbuka di Fase 1 yang diisi.
        static void IsiJurnal()
        {
            var j = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");
            string h1 = "[[TULIS halaman-1]]", c1 = "[[TULIS catatan-raka-1]]", c2 = "[[TULIS catatan-raka-2]]";
            TanpaPenanda("Jurnal", h1, c1, c2);
            j.HalamanDiary[Kode.H1].Isi = h1;
            j.HalamanDiary[Kode.H2].Isi = IsiHalaman2;
            j.CatatanRaka[0].Isi = c1;
            j.CatatanRaka[1].Isi = c2;
            EditorUtility.SetDirty(j);
        }

        static void TanpaPenanda(string nama, params string[] teks)
        {
            foreach (var t in teks)
                if (t != null && t.Contains("[["))
                    throw new InvalidDataException($"'{nama}' masih berisi penanda: \"{t}\". Lihat Task 11 Step 2.");
        }
    }
}
```

Catatan: `Kode.H1` dan `Kode.H2` adalah indeks halaman (0 dan 1), sama dengan yang dipakai `GembokLemari.Buka` di Task 2. `Terjemahan` boleh kosong; `Teks` tidak.

- [ ] **Step 2: Ganti setiap penanda dengan teks sumber**

Buka `docs/TheSending_FullGameStory.md` pada baris yang disebut, lalu ganti penandanya di `PembuatDataFase1.cs`. Tanda kutip ganda di dalam teks ditulis `\"`.

| Penanda | Sumber | Aturan |
|---|---|---|
| `[[SALIN GS:384]]` | Game Story baris 384 ("Prolog: Pulang" > "Dialog Kunci", kalimat Tukang Ojek) | salin persis |
| `[[SALIN GS:386]]` | baris 386 (kalimat pertama Bidan Wulan) | salin persis |
| `[[SALIN GS:388]]` | baris 388 (kalimat Raka) | salin persis |
| `[[SALIN GS:390]]` | baris 390 (kalimat kedua Bidan Wulan) | salin persis |
| `[[SALIN GS:422 jawa]]` | baris 422 ("Malam 1" > "Dialog Kunci", kalimat Ibu) | bagian bahasa Jawa saja, sebelum tanda kurung |
| `[[SALIN GS:422 terjemahan]]` | baris 422 | isi di dalam tanda kurung, tanpa kurungnya |
| `[[SALIN GS:418 kutipan]]` | baris 418 ("Malam 1" > "Puzzle") | hanya kalimat di dalam tanda kutip (tulisan di balik foto) |
| `[[SALIN GS:424]]` | baris 424 (kalimat Laras) | salin persis |
| `[[SALIN GS:426]]` | baris 426 (kalimat Raka) | salin persis |
| `[[TULIS sambutan-1..3]]` | Game Story baris 376 (Prolog, Alur Kejadian butir 5) | 3 kalimat pendek: Laras menyambut, Raka menanyakan Ibu, Laras mengarahkan Raka ke Bidan Wulan di pintu kamar Ibu |
| `[[TULIS ibu-prolog-1]]`, `[[TULIS ibu-prolog-3]]` | Game Story baris 380 (butir 7) | 1 kalimat pendek masing-masing: Raka menyapa Ibu; Raka menanggapi karena Ibu tidak menjawab. Baris Ibu tetap `"..."` (dokumen tidak memberi kalimatnya) |
| `[[TULIS kronologi-1..5]]` | Game Story baris 402 (Malam 1, Alur Kejadian butir 1) dan tabel "5. Latar Belakang Cerita" **hanya baris 237–238** | 5 kalimat bergantian: Laras menceritakan urutan sakit Ibu, Raka belum percaya. Jangan memakai isi baris 230–236: itu baru terungkap di malam lain |
| `[[TULIS teguran-1]]`, `[[TULIS teguran-2]]` | Game Story baris 352 ("6.7 Obat Ibu") | Laras mengingatkan bahwa jam obat sudah lewat; Raka mengakui lupa |
| `[[TULIS foto-1]]` | Game Story baris 418 | 1 kalimat: Raka membalik bingkai foto dan melihat ada tulisan |
| `[[TULIS kalender-1]]` | Game Story baris 65 (tabel "3.1 Latar Waktu", baris Prolog dan Malam 1) | 1 kalimat: Raka membaca hari Masehi dan penanggalan Jawa malam ini |
| `[[TULIS halaman-1]]` | Game Story baris 410 (Malam 1 butir 5) dan baris 827 (tabel "10.3", Halaman 1) | 4–6 kalimat, sudut pandang orang pertama (Bapak), isi sesuai dua baris itu saja |
| `[[TULIS catatan-raka-1]]` | Game Story baris 784 (9.1, judul pertama Catatan Raka) dan baris 386, 390 | 3–4 kalimat pengetahuan umum sesuai judul entri pertama. Bahasa netral, tanpa dosis obat |
| `[[TULIS catatan-raka-2]]` | Game Story baris 784 (judul kedua Catatan Raka) | 3–4 kalimat pengetahuan umum sesuai judul entri kedua; tutup dengan anjuran ke puskesmas atau rumah sakit |

Jangan menambah baris atau efek. Jumlah baris per aset harus tetap seperti tabel di atas.

- [ ] **Step 3: Pastikan tidak ada penanda tersisa**

```bash
grep -cE '\[\[(SALIN|TULIS)' Assets/_Project/Scripts/Editor/PembuatDataFase1.cs
```
Expected: `0`. (Sebelum Step 2 angkanya `26`.)

- [ ] **Step 4: Jalankan pembuat data lewat batchmode**

```bash
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod TheSending.EditorTools.PembuatDataFase1.Buat -logFile "${TMPDIR:-/tmp}/the-sending-data1.log"; echo "exit=$?"
grep -E "Data Fase 1 dibuat|error CS|Exception" "${TMPDIR:-/tmp}/the-sending-data1.log" | sort -u | head
ls Assets/_Project/Data/Fase1/*.asset | wc -l
grep -c 'Isi: *$' Assets/_Project/Data/Jurnal.asset
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
```
Expected: `exit=0`; baris `Data Fase 1 dibuat: 10 dialog di Assets/_Project/Data/Fase1, 4 isi Jurnal.`; tanpa `error CS` dan `Exception`; `10`. Baris terakhir menghitung isi Jurnal yang masih kosong; angkanya harus **4 lebih kecil** daripada `git show HEAD:Assets/_Project/Data/Jurnal.asset | grep -c 'Isi: *$'`.

- [ ] **Step 5: Jalankan seluruh suite**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"`. `DataDialogTests.SemuaAsetDialogLolosPemeriksaan` sekarang ikut memeriksa 10 aset Fase 1.

- [ ] **Step 6: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git add Assets/_Project/Scripts/Editor Assets/_Project/Data
git commit -m "feat: add Fase 1 dialog assets and journal text" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

### Task 12: Editor — rig pemain bersama dan `Scene_Desa`

`PembuatRig` membuat bagian yang sama di setiap scene gameplay, supaya `Scene_Desa`, `Scene_RumahKeluarga`, dan scene fase berikutnya tidak menyalin 60 baris yang sama. `PembuatDesa` membuat jalan masuk desa untuk Prolog (denah K7).

**Denah `Scene_Desa`** (meter; +Z = utara, pemain berjalan dari z = 2 ke z = 100):

| z | Isi | x |
|---|---|---|
| 0 | Gapura. Ojek di (1.5, 1), pergi ke z = -25 (Timeline `Prolog_OjekPergi`) | 0 |
| 2 | Titik mulai pemain, menghadap utara | 0 |
| 10, 44, 90 | Lampu jalan redup | -3, 3, -3 |
| 18 | `zonaSenter` (tutorial F) | selebar jalan |
| 20–40 | Kebun bambu, 6 rumpun di tiap sisi | ±3.5 dan ±4.5 |
| 48–56 | Jembatan bambu 2 × 8 m di atas air; sisi kiri dan kanan ditutup pembatas | 0 |
| 58 | `zonaAzan` (azan + kabut turun, K8) | selebar jalan |
| 72 | `zonaBuSumi` (adegan jendela) | selebar jalan |
| 75 | Rumah Bu Sumi di kanan jalan, menghadap jalan. Jendela di (7.5, 1.5, 76.2) | 10 |
| 100 | Rumah keluarga (tampak luar), pintu `pintuRumah` | 0 |

Koridor jalan dibatasi dinding tak terlihat di x = ±6.5 dan z = -1.5.

**Files:**
- Create: `Assets/_Project/Scripts/Editor/PembuatRig.cs`, `Assets/_Project/Scripts/Editor/PembuatTimeline.cs`, `Assets/_Project/Scripts/Editor/PembuatDesa.cs`
- Modify: `Assets/_Project/Scripts/Editor/TheSending.Editor.asmdef`
- Dibuat skrip: `Assets/_Project/Scenes/Scene_Desa.unity`; `Assets/_Project/Timeline/Prolog_OjekPergi.playable`, `Prolog_BuSumi.playable`; `Assets/_Project/Timeline/Sinyal/MotorPergi.asset`, `TiraiDitutup.asset`

**Interfaces:**
- Consumes: `PembuatSandbox.Set/SetTeks` (Task 10), `PembuatPengganti` (Task 10), `PembuatDataFase1.Muat` (Task 11), `PembuatGayaPsx.TerapkanKeScene/SiapkanKamera`, `PetaScene.Desa` (Task 4), semua field dan konstanta sinyal `AlurProlog` (Task 9), `KameraAdegan`, `PemutarTimeline`, `ZonaPemicu`, `InteraktifAlur`, `Suara` (Task 8), `PanelInventaris`, `PanelGembok` (Task 7); Timeline API (`TimelineAsset.CreateTrack<T>`, `CreateMarkerTrack`, `markerTrack.CreateMarker<SignalEmitter>`, `AnimationTrack.CreateClip(AnimationClip)`, `AnimationPlayableAsset.removeStartOffset`, `PlayableDirector.SetGenericBinding`; dicek di `Library/PackageCache/com.unity.timeline@*/Runtime/Unity.Timeline.api`).
- Produces:
  - `Rig` (kelas data): `PemainGerak Pemain`, `Transform Kamera`, `SenterHP Senter`, `Hud Hud`, `PanelDialog Dialog`, `KartuTeks Kartu`, `PanelGembok Gembok`, `KameraAdegan Adegan`, `Camera KameraCutscene` (kamera adegan, di akar scene; digerakkan Timeline).
  - `PembuatTimeline` (static, internal): `(TimelineAsset Tl, PlayableDirector Director, PemutarTimeline Pemutar) Buat(string nama, float durasi)`; `void Gerak(TimelineAsset tl, PlayableDirector director, GameObject benda, params (float detik, Transform titik)[] kunci)`; `void Sinyal(TimelineAsset tl, float detik, string nama)`; `void Simpan(TimelineAsset tl)`.
  - `PembuatRig` (static, internal): `Rig Buat(Vector3 posisiPemain)`; `Suara BuatSuara(Rig rig, string nama, string folder, string grup, string teksPengganti, bool ulang = false)`; `Transform Titik(string nama, Vector3 posisi, Vector3 menghadap)`; `ZonaPemicu Zona(string nama, Vector3 pusat, Vector3 ukuran)`; `Light Lampu(string nama, Vector3 posisi, string hex, float jangkauan, float intensitas)`; `GameObject Balok(string nama, Vector3 posisi, Vector3 ukuran, string material)`; `GameObject Logika(string nama, Vector3 posisi, Vector3 ukuranCollider)`; `void Batas(string nama, Vector3 pusat, Vector3 ukuran)`; `Color Hex(string hex)`.
  - `PembuatDesa`: `internal const string JalurScene`; `void Buat()` (menu **The Sending > Buat Scene Desa**).

- [ ] **Step 1: `PembuatRig.cs`**

```csharp
using System;
using System.IO;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Referensi ke bagian rig yang dibutuhkan pembuat scene untuk menyambungkan sutradara dan benda.
    internal sealed class Rig
    {
        public PemainGerak Pemain;
        public Transform Kamera;
        public SenterHP Senter;
        public Hud Hud;
        public PanelDialog Dialog;
        public KartuTeks Kartu;
        public PanelGembok Gembok;
        public KameraAdegan Adegan;
        public Camera KameraCutscene;
    }

    // Bagian yang sama di setiap scene gameplay: GameManager, pemain + HP, kamera adegan, UI, dan alat bantu tata letak.
    // Dipanggil SETELAH EditorSceneManager.NewScene.
    internal static class PembuatRig
    {
        const string JalurMixer = "Assets/_Project/Audio/Resources/MixerUtama.mixer";
        const string JalurJurnal = "Assets/_Project/Data/Jurnal.asset";

        // Pemain selalu mulai menghadap +Z; sutradara memindahkannya lewat PemainGerak.Pindah bila perlu.
        internal static Rig Buat(Vector3 posisiPemain)
        {
            var jurnal = AssetDatabase.LoadAssetAtPath<DataJurnal>(JalurJurnal)
                ?? throw new FileNotFoundException($"{JalurJurnal} belum ada. Jalankan PembuatSandbox.Buat sekali.");

            new GameObject("GameManager").AddComponent<GameManager>();

            var pemain = new GameObject("Pemain");
            pemain.transform.position = posisiPemain;
            var cc = pemain.AddComponent<CharacterController>();
            cc.height = 1.7f;
            cc.radius = 0.3f;
            cc.center = new Vector3(0f, 0.85f, 0f);

            var kamera = new GameObject("Kamera");
            kamera.tag = "MainCamera";
            kamera.transform.SetParent(pemain.transform, false);
            kamera.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            var cam = kamera.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            PembuatGayaPsx.SiapkanKamera(cam);
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

            var gerak = pemain.AddComponent<PemainGerak>();
            Set(gerak, "kamera", kamera.transform);
            var interaksi = pemain.AddComponent<PemainInteraksi>();
            Set(interaksi, "kamera", kamera.transform);
            var senterHp = pemain.AddComponent<SenterHP>();
            Set(senterHp, "cahaya", senter);
            Set(pemain.AddComponent<KompasHP>(), "pemain", pemain.transform);

            // Kamera adegan: mati sampai KameraAdegan.Mulai. Tanpa AudioListener (pendengar tetap di kamera pemain).
            // Tetap di akar scene: Timeline menggerakkannya dengan posisi dunia (PembuatTimeline.Gerak).
            var camAdegan = new GameObject("KameraAdegan").AddComponent<Camera>();
            camAdegan.nearClipPlane = 0.05f;
            PembuatGayaPsx.SiapkanKamera(camAdegan);
            camAdegan.enabled = false;

            var ui = new GameObject("UI");
            var hud = ui.AddComponent<Hud>();
            Set(hud, "interaksi", interaksi);
            var kameraHp = pemain.AddComponent<KameraHP>();
            Set(kameraHp, "kamera", kamera.transform);
            Set(kameraHp, "hud", hud);
            var dialog = ui.AddComponent<PanelDialog>();
            Set(ui.AddComponent<PanelJurnal>(), "data", jurnal);
            var kartu = ui.AddComponent<KartuTeks>();   // tanpa kartu pembuka: sutradara scene yang menampilkan kartu
            var pengaturan = ui.AddComponent<PanelPengaturan>();
            Set(ui.AddComponent<MenuJeda>(), "pengaturan", pengaturan);
            Set(ui.AddComponent<AlatUji>(), "hud", hud);
            ui.AddComponent<PanelInventaris>();
            var gembok = ui.AddComponent<PanelGembok>();
            Set(gembok, "hud", hud);
            var adegan = ui.AddComponent<KameraAdegan>();
            Set(adegan, "kameraPemain", cam);
            Set(adegan, "kameraAdegan", camAdegan);

            return new Rig
            {
                Pemain = gerak, Kamera = kamera.transform, Senter = senterHp, Hud = hud,
                Dialog = dialog, Kartu = kartu, Gembok = gembok, Adegan = adegan, KameraCutscene = camAdegan,
            };
        }

        // Satu bunyi adegan (2D). Klip null = aset belum ada: komponen Suara menampilkan teks pengganti di HUD.
        internal static Suara BuatSuara(Rig rig, string nama, string folder, string grup, string teksPengganti, bool ulang = false)
        {
            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(JalurMixer)
                ?? throw new FileNotFoundException($"{JalurMixer} belum ada (dibuat tim di 0C).");
            var daftar = mixer.FindMatchingGroups(grup);
            if (daftar.Length == 0) throw new ArgumentException($"Grup mixer '{grup}' tidak ada di MixerUtama.");

            var g = new GameObject("Suara " + nama);
            var sumber = g.AddComponent<AudioSource>();
            sumber.playOnAwake = false;
            sumber.loop = ulang;
            sumber.spatialBlend = 0f;
            sumber.outputAudioMixerGroup = daftar[0];
            sumber.clip = PembuatPengganti.Klip(folder, nama);
            var suara = g.AddComponent<Suara>();
            Set(suara, "sumber", sumber);
            Set(suara, "hud", rig.Hud);
            SetTeks(suara, "teksPengganti", teksPengganti);
            return suara;
        }

        // Titik berdiri pemain atau bidikan kamera adegan: berada di `posisi`, menghadap ke `menghadap`.
        internal static Transform Titik(string nama, Vector3 posisi, Vector3 menghadap)
        {
            var t = new GameObject(nama).transform;
            t.position = posisi;
            t.LookAt(menghadap);
            return t;
        }

        internal static ZonaPemicu Zona(string nama, Vector3 pusat, Vector3 ukuran)
        {
            var g = new GameObject(nama);
            g.transform.position = pusat;
            var kotak = g.AddComponent<BoxCollider>();
            kotak.isTrigger = true;
            kotak.size = ukuran;
            return g.AddComponent<ZonaPemicu>();
        }

        internal static Light Lampu(string nama, Vector3 posisi, string hex, float jangkauan, float intensitas)
        {
            var lampu = new GameObject(nama).AddComponent<Light>();
            lampu.type = LightType.Point;
            lampu.color = Hex(hex);
            lampu.range = jangkauan;
            lampu.intensity = intensitas;
            lampu.transform.position = posisi;
            return lampu;
        }

        // Kotak bermaterial pengganti, dengan collider (dinding, lantai, pintu).
        internal static GameObject Balok(string nama, Vector3 posisi, Vector3 ukuran, string material)
        {
            var g = Kotak(nama, posisi, ukuran);
            g.GetComponent<Renderer>().sharedMaterial = PembuatPengganti.Material(material);
            return g;
        }

        // Objek logika: collider kotak (alas di titik nol) tempat skrip Interaktif dipasang. Visualnya prefab anak.
        internal static GameObject Logika(string nama, Vector3 posisi, Vector3 ukuranCollider)
        {
            var g = new GameObject(nama);
            g.transform.position = posisi;
            var kotak = g.AddComponent<BoxCollider>();
            kotak.size = ukuranCollider;
            kotak.center = new Vector3(0f, ukuranCollider.y / 2f, 0f);
            return g;
        }

        // Dinding tak terlihat.
        internal static void Batas(string nama, Vector3 pusat, Vector3 ukuran)
        {
            var g = new GameObject(nama);
            g.transform.position = pusat;
            g.AddComponent<BoxCollider>().size = ukuran;
        }

        internal static Color Hex(string hex) =>
            ColorUtility.TryParseHtmlString("#" + hex, out var warna) ? warna : throw new FormatException($"Warna salah: {hex}");
    }
}
```

- [ ] **Step 1a: `TheSending.Editor.asmdef`, tambah referensi Timeline**

Di `Assets/_Project/Scripts/Editor/TheSending.Editor.asmdef`, tambahkan `"Unity.Timeline"` sebagai butir terakhir larik `references` (sesudah `"Unity.RenderPipelines.Universal.Runtime"`, beri koma di baris sebelumnya).

- [ ] **Step 1b: `PembuatTimeline.cs`**

```csharp
using System;
using System.IO;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TheSending.EditorTools
{
    // Aset Timeline cutscene dibuat dari kode supaya bisa dibangun ulang lewat batchmode tanpa membuka editor (K1).
    // Aset ditimpa setiap scene pemakainya dibangun ulang: setel waktu dan bidikan di skrip pembuat scene, bukan di jendela Timeline.
    internal static class PembuatTimeline
    {
        const string Folder = "Assets/_Project/Timeline";
        const string FolderSinyal = Folder + "/Sinyal";

        // Aset .playable baru + objek "Timeline <nama>" berisi PlayableDirector dan PemutarTimeline.
        internal static (TimelineAsset Tl, PlayableDirector Director, PemutarTimeline Pemutar) Buat(string nama, float durasi)
        {
            Directory.CreateDirectory(FolderSinyal);
            string jalur = $"{Folder}/{nama}.playable";
            AssetDatabase.DeleteAsset(jalur);   // scene pemakainya dibangun ulang di run yang sama, jadi GUID baru tidak memutus apa pun
            var tl = ScriptableObject.CreateInstance<TimelineAsset>();
            AssetDatabase.CreateAsset(tl, jalur);   // harus tersimpan dulu: track, klip, dan marker lalu masuk sebagai sub-aset
            tl.durationMode = TimelineAsset.DurationMode.FixedLength;
            tl.fixedDuration = durasi;

            var g = new GameObject("Timeline " + nama);
            var director = g.AddComponent<PlayableDirector>();
            director.playableAsset = tl;
            director.playOnAwake = false;
            // Adegan menjeda gim (Time.timeScale 0) selama cutscene; dengan Game Time timeline beku di frame pertama.
            director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
            director.extrapolationMode = DirectorWrapMode.None;   // berhenti di akhir: event stopped terpicu
            return (tl, director, g.AddComponent<PemutarTimeline>());
        }

        // Gerak lurus antar titik kunci (posisi dan arah hadap titik). Dua kunci berselisih 0,01 detik = potongan kamera.
        // Benda harus di akar scene, supaya posisi lokal = posisi dunia.
        internal static void Gerak(TimelineAsset tl, PlayableDirector director, GameObject benda, params (float detik, Transform titik)[] kunci)
        {
            if (benda.transform.parent != null) throw new ArgumentException($"'{benda.name}' harus di akar scene.");
            if (kunci.Length < 2) throw new ArgumentException("Gerak butuh minimal dua kunci.");
            var animator = benda.GetComponent<Animator>();
            if (animator == null) animator = benda.AddComponent<Animator>();   // tanpa controller: hanya Timeline yang menggerakkan

            var klip = new AnimationClip { name = "Gerak " + benda.name };
            for (int sumbu = 0; sumbu < 3; sumbu++)
            {
                var posisi = new AnimationCurve();
                var sudut = new AnimationCurve();
                float sudutSebelum = 0f;
                for (int i = 0; i < kunci.Length; i++)
                {
                    float s = kunci[i].titik.eulerAngles[sumbu];
                    sudutSebelum = i == 0 ? s : sudutSebelum + Mathf.DeltaAngle(sudutSebelum, s);   // jangan berputar lewat 360
                    posisi.AddKey(kunci[i].detik, kunci[i].titik.position[sumbu]);
                    sudut.AddKey(kunci[i].detik, sudutSebelum);
                }
                char huruf = "xyz"[sumbu];
                klip.SetCurve("", typeof(Transform), "m_LocalPosition." + huruf, Lurus(posisi));
                klip.SetCurve("", typeof(Transform), "localEulerAnglesRaw." + huruf, Lurus(sudut));
            }
            AssetDatabase.AddObjectToAsset(klip, tl);

            var track = tl.CreateTrack<AnimationTrack>("Gerak " + benda.name);
            var aset = (AnimationPlayableAsset)track.CreateClip(klip).asset;
            aset.removeStartOffset = false;   // kunci adalah posisi dunia, bukan relatif terhadap posisi awal
            director.SetGenericBinding(track, animator);
        }

        // Sinyal di marker track timeline. Diterima PemutarTimeline di objek director, diteruskan ke sutradara dengan nama ini.
        // Jangan di detik 0 tepat; pakai 0,05 supaya pasti terlewati saat diputar.
        internal static void Sinyal(TimelineAsset tl, float detik, string nama)
        {
            string jalur = $"{FolderSinyal}/{nama}.asset";
            var sinyal = AssetDatabase.LoadAssetAtPath<SignalAsset>(jalur);
            if (sinyal == null)
            {
                sinyal = ScriptableObject.CreateInstance<SignalAsset>();
                AssetDatabase.CreateAsset(sinyal, jalur);   // nama aset = nama file = nama yang diterima sutradara
            }
            tl.CreateMarkerTrack();   // tidak berbuat apa-apa jika sudah ada
            var emitter = tl.markerTrack.CreateMarker<SignalEmitter>(detik);
            emitter.asset = sinyal;
            emitter.retroactive = false;
            emitter.emitOnce = true;
        }

        // Dipanggil sekali setelah semua track dan sinyal dibuat, sebelum scene disimpan.
        internal static void Simpan(TimelineAsset tl)
        {
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tl))) EditorUtility.SetDirty(o);
            AssetDatabase.SaveAssets();
        }

        static AnimationCurve Lurus(AnimationCurve kurva)
        {
            for (int i = 0; i < kurva.length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(kurva, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(kurva, i, AnimationUtility.TangentMode.Linear);
            }
            return kurva;
        }
    }
}
```

- [ ] **Step 2: `PembuatDesa.cs`**

Teks pengganti suara hanya menyebut jenis bunyinya; tampil di HUD selama file suara belum ada.

```csharp
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Membuat Scene_Desa: jalan masuk desa untuk Prolog (denah K7). Jangan sunting scene dengan tangan; ubah angka di sini.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatDesa.Buat
    public static class PembuatDesa
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.Desa + ".unity";
        const float SetengahLebar = 6.5f, UjungJalan = 100f;
        const string Sfx = PembuatPengganti.FolderSfx;

        [MenuItem("The Sending/Buat Scene Desa")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();
            var rig = PembuatRig.Buat(new Vector3(0f, 0.05f, 2f));

            Lingkungan();
            var sutradara = new GameObject("Sutradara Prolog").AddComponent<AlurProlog>();
            Set(sutradara, "adegan", rig.Adegan);
            Set(sutradara, "kartu", rig.Kartu);
            Set(sutradara, "dialog", rig.Dialog);
            Set(sutradara, "hud", rig.Hud);
            Set(sutradara, "dialogOjek", PembuatDataFase1.Muat(PembuatDataFase1.Ojek));

            // Gapura dan ojek
            PembuatPengganti.Pasang("Gapura", null, Vector3.zero);
            var ojek = PembuatPengganti.Pasang("Ojek", null, new Vector3(1.5f, 0f, 1f), 180f);
            Set(sutradara, "ojek", ojek.transform);
            Set(sutradara, "bidikGapura", Titik("BidikGapura", new Vector3(-2.5f, 1.7f, 7f), new Vector3(0.5f, 1.2f, 0f)));
            // Cutscene ojek pergi (3,5 detik): ojek lurus ke z = -25 dalam 3 detik, suara motor sejak awal.
            var ojekPergi = PembuatTimeline.Buat("Prolog_OjekPergi", 3.5f);
            PembuatTimeline.Gerak(ojekPergi.Tl, ojekPergi.Director, ojek,
                (0f, ojek.transform), (3f, Titik("TitikOjekPergi", new Vector3(1.5f, 0f, -25f), new Vector3(1.5f, 0f, -30f))));
            PembuatTimeline.Sinyal(ojekPergi.Tl, 0.05f, AlurProlog.SinyalMotorPergi);
            PembuatTimeline.Simpan(ojekPergi.Tl);
            Set(sutradara, "timelineOjek", ojekPergi.Pemutar);

            // Zona tutorial dan azan
            var selebarJalan = new Vector3(SetengahLebar * 2f, 3f, 1.5f);
            Set(sutradara, "zonaSenter", Zona("ZonaSenter", new Vector3(0f, 1.5f, 18f), selebarJalan));
            Set(sutradara, "zonaAzan", Zona("ZonaAzan", new Vector3(0f, 1.5f, 58f), selebarJalan));
            Set(sutradara, "zonaBuSumi", Zona("ZonaBuSumi", new Vector3(0f, 1.5f, 72f), selebarJalan));

            // Rumah Bu Sumi di kanan jalan, menghadap jalan. Jendela prefab (1.2, 1.5, 2.5) jatuh di (7.5, 1.5, 76.2).
            PembuatPengganti.Pasang("RumahKecil", null, new Vector3(10f, 0f, 75f), -90f);
            Balok("AmbangJendela", new Vector3(7.25f, 0.45f, 76.2f), new Vector3(0.1f, 0.9f, 1.4f), "Gedek");
            Balok("CahayaJendela", new Vector3(7.47f, 1.5f, 76.2f), new Vector3(0.02f, 1.1f, 1.1f), "Kain");
            var siluet = PembuatPengganti.Pasang("BuSumi_Siluet", null, new Vector3(7.38f, 0f, 76.2f), -90f);
            var tirai = Balok("Tirai", new Vector3(7.2f, 1.5f, 76.2f), new Vector3(0.03f, 1.2f, 1.3f), "Kain");
            tirai.SetActive(false);
            Set(sutradara, "siluetBuSumi", siluet);
            Set(sutradara, "tiraiJendela", tirai);
            Set(sutradara, "lampuJendela", Lampu("LampuJendela", new Vector3(7.1f, 1.7f, 76.2f), "FFB46B", 4f, 2f));
            var bidikJendela = Titik("BidikJendela", new Vector3(2.5f, 1.6f, 74.5f), new Vector3(7.4f, 1.5f, 76.2f));
            Set(sutradara, "bidikJendela", bidikJendela);
            // Cutscene Bu Sumi (4 detik): kamera adegan mendekat pelan ke jendela, tirai menutup pada 2,5 detik.
            var buSumi = PembuatTimeline.Buat("Prolog_BuSumi", 4f);
            PembuatTimeline.Gerak(buSumi.Tl, buSumi.Director, rig.KameraCutscene.gameObject,
                (0f, bidikJendela), (4f, Titik("BidikJendelaDekat", new Vector3(3.5f, 1.6f, 75f), new Vector3(7.4f, 1.5f, 76.2f))));
            PembuatTimeline.Sinyal(buSumi.Tl, 2.5f, AlurProlog.SinyalTiraiDitutup);
            PembuatTimeline.Simpan(buSumi.Tl);
            Set(sutradara, "timelineBuSumi", buSumi.Pemutar);

            // Rumah keluarga: prefab menghadap selatan (ke arah pemain), pintunya memindahkan ke Scene_RumahKeluarga.
            PembuatPengganti.Pasang("RumahKeluargaLuar", null, new Vector3(0f, 0f, UjungJalan), 180f);
            var pintu = Balok("PintuRumah", new Vector3(0f, 1.05f, UjungJalan - 0.15f), new Vector3(1.1f, 2.1f, 0.2f), "KayuGelap");
            var alurPintu = pintu.AddComponent<InteraktifAlur>();
            SetTeks(alurPintu, "prompt", "Masuk rumah");
            Set(sutradara, "pintuRumah", alurPintu);

            // Suara. Azan tidak punya pengganti sintetis; selama filenya belum ada hanya teks ini yang tampil.
            // Tidak ada suara telepon: suara tokoh ditunda, telepon Laras hanya kartu teks.
            Set(sutradara, "suaraMotor", BuatSuara(rig, "MotorMenjauh", Sfx, "SFX", "(Suara: motor menjauh.)"));
            Set(sutradara, "suaraAzan", BuatSuara(rig, "AzanMagrib", Sfx, "Ambience", "(Suara: azan magrib dari kejauhan.)"));
            Set(sutradara, "suaraPintu", BuatSuara(rig, "PintuBerderit", Sfx, "SFX", "(Suara: pintu kayu berderit.)"));

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene desa dibuat: {JalurScene}");
        }

        static void Lingkungan()
        {
            var bulan = new GameObject("CahayaBulan").AddComponent<Light>();
            bulan.type = LightType.Directional;
            bulan.intensity = 0.15f;
            bulan.color = Hex("6D86A8");
            bulan.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            Balok("Tanah", new Vector3(0f, -0.1f, 50f), new Vector3(40f, 0.2f, 140f), "Tanah");
            Batas("BatasBarat", new Vector3(-SetengahLebar, 2f, 50f), new Vector3(0.2f, 4f, 140f));
            Batas("BatasTimur", new Vector3(SetengahLebar, 2f, 50f), new Vector3(0.2f, 4f, 140f));
            Batas("BatasSelatan", new Vector3(0f, 2f, -1.5f), new Vector3(SetengahLebar * 2f, 4f, 0.2f));

            float[] zLampu = { 10f, 44f, 90f };
            for (int i = 0; i < zLampu.Length; i++)
            {
                float x = i % 2 == 0 ? -3f : 3f;
                PembuatPengganti.Pasang("LampuJalan", null, new Vector3(x, 0f, zLampu[i]), x < 0f ? 0f : 180f);
                Lampu("CahayaLampuJalan", new Vector3(x < 0f ? x + 0.5f : x - 0.5f, 2.6f, zLampu[i]), "FFB46B", 8f, 1.5f);
            }

            // Kebun bambu z 20-40: enam rumpun di tiap sisi, selang-seling jaraknya dari jalan.
            for (int i = 0; i < 6; i++)
            {
                float z = 20f + i * 4f, x = 3.5f + i % 2;
                PembuatPengganti.Pasang("RumpunBambu", null, new Vector3(-x, 0f, z), i * 37f);
                PembuatPengganti.Pasang("RumpunBambu", null, new Vector3(x, 0f, z + 2f), i * 53f);
            }

            // Jembatan bambu z 48-56. Air hanya tampilan; kedua sisi jembatan ditutup pembatas.
            var air = Balok("Air", new Vector3(0f, 0.01f, 52f), new Vector3(SetengahLebar * 2f, 0.02f, 6f), "Gelap");
            Object.DestroyImmediate(air.GetComponent<Collider>());
            PembuatPengganti.Pasang("JembatanBambu", null, new Vector3(0f, 0f, 52f));
            Batas("BatasAirBarat", new Vector3(-3.75f, 1f, 52f), new Vector3(5.5f, 2f, 6.2f));
            Batas("BatasAirTimur", new Vector3(3.75f, 1f, 52f), new Vector3(5.5f, 2f, 6.2f));
        }
    }
}
```

- [ ] **Step 3: Buat scene lewat batchmode**

```bash
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod TheSending.EditorTools.PembuatDesa.Buat -logFile "${TMPDIR:-/tmp}/the-sending-desa.log"; echo "exit=$?"
grep -E "Scene desa dibuat|error CS|Exception" "${TMPDIR:-/tmp}/the-sending-desa.log" | sort -u | head
test -f Assets/_Project/Scenes/Scene_Desa.unity && echo ada
grep -c "AlurProlog\|m_Name: Sutradara Prolog" Assets/_Project/Scenes/Scene_Desa.unity
ls Assets/_Project/Timeline/Prolog_OjekPergi.playable Assets/_Project/Timeline/Prolog_BuSumi.playable Assets/_Project/Timeline/Sinyal/MotorPergi.asset Assets/_Project/Timeline/Sinyal/TiraiDitutup.asset
grep "m_TimeUpdateMode" Assets/_Project/Scenes/Scene_Desa.unity
grep -c "AnimationTrack\|SignalEmitter" Assets/_Project/Timeline/Prolog_BuSumi.playable
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
```
Expected: `exit=0`; baris `Scene desa dibuat: Assets/_Project/Scenes/Scene_Desa.unity`; tanpa `error CS` dan `Exception` (`MissingFieldException` atau `ArgumentNullException` berarti nama field di Task 9 dan di sini tidak sama: betulkan pembuat scene, bukan sutradaranya); `ada`; angka `grep -c` pertama minimal `1`; keempat file Timeline dan sinyal ada; dua baris `m_TimeUpdateMode: 2` (urutan enum `DirectorUpdateMode`: DSPClock 0, GameTime 1, UnscaledGameTime 2, Manual 3); `AnimationTrack\|SignalEmitter` minimal `2`. Jika nilainya bukan 2, cek urutan enum di dokumentasi Unity 6000.6 sebelum mengubah apa pun, lalu laporkan. Scene belum bisa dimuat dari menu utama sampai didaftarkan di Build Settings (Task 13).

- [ ] **Step 4: Jalankan seluruh suite (cek kompilasi)**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"`, tanpa `error CS`.

- [ ] **Step 5: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git status --short
git add Assets/_Project/Scripts/Editor Assets/_Project/Scenes/Scene_Desa.unity Assets/_Project/Scenes/Scene_Desa.unity.meta Assets/_Project/Timeline Assets/_Project/Timeline.meta
git commit -m "feat: add shared player rig, Timeline cutscene builder, and Scene_Desa for the Prolog" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```
Jika `git status` menunjukkan file lain yang berubah (misalnya aset pengganti yang dibuat ulang), jangan di-add; laporkan.

---

### Task 13: Editor — `Scene_RumahKeluarga`, daftar scene, dan "Bangun Ulang Semua"

**Denah `Scene_RumahKeluarga`** (K7; meter; rumah 14 × 12, x dari -7 sampai 7, z dari 0 (selatan, pintu depan) sampai 12; tinggi dinding 3):

| Ruang | x | z | Pintu (lebar 1) | Isi Fase 1 |
|---|---|---|---|---|
| Ruang tengah | -3..3 | 0..8 | pintu depan x 0, z 0 (tertutup) | 4 saka guru, blandar + ukiran, foto keluarga, jam dinding, meja, stop kontak, Laras |
| Kamar Raka | -7..-3 | 0..4 | x -3, z 2 | ranjang, meja |
| Kamar Ibu | -7..-3 | 4..8 | x -3, z 6 | ranjang + Ibu, lemari Bapak + gembok, meja + teplok, kursi; Bidan Wulan berdiri di pintu |
| Kamar mandi | 3..7 | 0..4 | x 3, z 2 | bak |
| Kamar Laras | 3..7 | 4..8 | x 3, z 6 | ranjang |
| Dapur | -7..3 | 8..12 | x 0, z 8 | meja, kalender |
| Gudang | 3..7 | 8..12 | x 3, z 10 (terkunci sampai Fase 2) | kosong |

Objek yang baru muncul pada cutscene 23.00 dibuat **nonaktif**: `Paku` (dengan `PetunjukFoto`), `Bercak`, `Teplok`, `Laras_Berdoa`. `AlurMalam1` yang menyalakannya (lewat sinyal Timeline `Malam1_Cutscene23` dan `KeadaanSetelahAdegan`).

**Files:**
- Create: `Assets/_Project/Scripts/Editor/PembuatRumahKeluarga.cs`
- Modify: `Assets/_Project/Scripts/Editor/PembuatMenuUtama.cs`
- Dibuat skrip: `Assets/_Project/Scenes/Scene_RumahKeluarga.unity`; `Assets/_Project/Timeline/Malam1_Cutscene23.playable`; `Assets/_Project/Timeline/Sinyal/Kejang.asset`, `Paku.asset`, `Padam.asset`, `Langkah.asset`; `ProjectSettings/EditorBuildSettings.asset` berubah (4 scene)

**Interfaces:**
- Consumes: `PembuatRig`, `Rig.KameraCutscene`, dan `PembuatTimeline` (Task 12), `PembuatPengganti` (Task 10), `PembuatDataFase1` (Task 11), `PembuatSandbox.Set/SetTeks/SetBool/SetAngka/SetSatuObjek/SetDaftar` (Task 10), `PetaScene.RumahKeluarga` (Task 4), semua field `AlurPrologRumah` dan `AlurMalam1` serta konstanta `AlurMalam1.Sinyal...` (Task 9), `PemberiObat`, `JadwalObat`, `TerlihatSaatDisorot` (Task 8), `LemariAyah` (Task 7), `PemicuDialog`, `PetunjukFoto`, `PengisiBaterai`, `AktifDiMalam` (0B), `GembokLemari.KodeBenar`, efek `tujuan:PotretPaku` dan `foto:PakuMalam1` (Task 3).
- Produces: `PembuatRumahKeluarga`: `internal const string JalurScene`, `void Buat()` (menu **The Sending > Buat Scene Rumah Keluarga**). `PembuatMenuUtama.DaftarkanScene` mendaftarkan 4 scene sesuai Global Constraints; `BangunSemua` membangun semuanya.

- [ ] **Step 1: `PembuatRumahKeluarga.cs`**

Jika tim memilih K2-B: ganti `"tujuan:PotretPaku; foto:PakuMalam1"` menjadi `"tujuan:PotretPaku"`.

```csharp
using System.IO;
using TheSending.Core;
using TheSending.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static TheSending.EditorTools.PembuatRig;
using static TheSending.EditorTools.PembuatSandbox;

namespace TheSending.EditorTools
{
    // Membuat Scene_RumahKeluarga (denah K7) untuk akhir Prolog dan Malam 1.
    // Jangan sunting scene dengan tangan; ubah angka di sini lalu bangun ulang.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatRumahKeluarga.Buat
    public static class PembuatRumahKeluarga
    {
        internal const string JalurScene = "Assets/_Project/Scenes/" + PetaScene.RumahKeluarga + ".unity";
        const float Tinggi = 3f, Tebal = 0.15f;
        const string Sfx = PembuatPengganti.FolderSfx, Musik = PembuatPengganti.FolderMusik;
        const string WarnaLampuRumah = "FFD9A0", WarnaTeplok = "FFB46B";
        const string JalurFont = "Assets/_Project/Art/Fonts/Resources/VT323-Regular.ttf";

        [MenuItem("The Sending/Buat Scene Rumah Keluarga")]
        public static void Buat()
        {
            PembuatPengganti.Buat();
            Directory.CreateDirectory(Path.GetDirectoryName(JalurScene));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PembuatGayaPsx.TerapkanKeScene();
            var rig = PembuatRig.Buat(new Vector3(0f, 0.05f, 1.2f));

            var sutradara = new GameObject("Sutradara");
            var prolog = sutradara.AddComponent<AlurPrologRumah>();
            var malam1 = sutradara.AddComponent<AlurMalam1>();
            foreach (var alur in new Object[] { prolog, malam1 })
            {
                Set(alur, "adegan", rig.Adegan);
                Set(alur, "kartu", rig.Kartu);
                Set(alur, "dialog", rig.Dialog);
                Set(alur, "hud", rig.Hud);
                Set(alur, "pemain", rig.Pemain);
            }
            Set(prolog, "dialogLaras", PembuatDataFase1.Muat(PembuatDataFase1.LarasSambutan));
            Set(prolog, "dialogIbu", PembuatDataFase1.Muat(PembuatDataFase1.IbuProlog));
            Set(prolog, "malam1", malam1);
            Set(prolog, "titikMulai", Titik("TitikMulaiProlog", new Vector3(0f, 0.05f, 1.2f), new Vector3(0f, 0.05f, 6f)));
            Set(malam1, "dialogPenutup", PembuatDataFase1.Muat(PembuatDataFase1.PenutupMalam1));

            Bangunan();
            Perabot(rig);
            Ukiran(rig);
            SetDaftar(malam1, "lampuListrik", LampuListrik());
            KamarIbu(rig, prolog, malam1);
            BendaAdegan(malam1);
            Tokoh(rig, sutradara, prolog, malam1);
            SuaraDanJadwal(rig, sutradara, malam1);

            EditorSceneManager.SaveScene(scene, JalurScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scene rumah keluarga dibuat: {JalurScene}");
        }

        // Dinding lurus sejajar sumbu, dari (x1, z1) ke (x2, z2).
        static void Dinding(float x1, float z1, float x2, float z2)
        {
            var ukuran = new Vector3(Mathf.Abs(x2 - x1) + Tebal, Tinggi, Mathf.Abs(z2 - z1) + Tebal);
            Balok("Dinding", new Vector3((x1 + x2) / 2f, Tinggi / 2f, (z1 + z2) / 2f), ukuran, "Gedek");
        }

        static void Bangunan()
        {
            Balok("Lantai", new Vector3(0f, -0.1f, 6f), new Vector3(14f, 0.2f, 12f), "KayuTua");
            Balok("Plafon", new Vector3(0f, Tinggi + 0.1f, 6f), new Vector3(14f, 0.2f, 12f), "KayuGelap");

            // Dinding luar. Pintu depan (selatan) tertutup: di Fase 1 pemain tidak keluar rumah lagi.
            Dinding(-7f, 0f, -0.6f, 0f);
            Dinding(0.6f, 0f, 7f, 0f);
            Dinding(-7f, 12f, 7f, 12f);
            Dinding(-7f, 0f, -7f, 12f);
            Dinding(7f, 0f, 7f, 12f);
            Balok("PintuDepan", new Vector3(0f, 1.05f, 0f), new Vector3(1.2f, 2.1f, 0.1f), "KayuGelap");
            Balok("AmbangPintuDepan", new Vector3(0f, 2.55f, 0f), new Vector3(1.2f, 0.9f, Tebal), "Gedek");

            // Sekat barat (x = -3): pintu kamar Raka z 1.5-2.5, pintu kamar Ibu z 5.5-6.5.
            Dinding(-3f, 0f, -3f, 1.5f);
            Dinding(-3f, 2.5f, -3f, 5.5f);
            Dinding(-3f, 6.5f, -3f, 8f);
            // Sekat timur (x = 3): pintu kamar mandi z 1.5-2.5, kamar Laras z 5.5-6.5, gudang z 9.5-10.5.
            Dinding(3f, 0f, 3f, 1.5f);
            Dinding(3f, 2.5f, 3f, 5.5f);
            Dinding(3f, 6.5f, 3f, 9.5f);
            Dinding(3f, 10.5f, 3f, 12f);
            // Sekat antarkamar (z = 4) dan sekat dapur (z = 8) dengan pintu dapur x -0.6..0.6.
            Dinding(-7f, 4f, -3f, 4f);
            Dinding(3f, 4f, 7f, 4f);
            Dinding(-7f, 8f, -0.6f, 8f);
            Dinding(0.6f, 8f, 7f, 8f);

            // Gudang terkunci sampai Fase 2. Prompt-nya sudah memberi tahu pemain; E tidak berbuat apa-apa.
            var pintuGudang = Balok("PintuGudang", new Vector3(3f, 1.05f, 10f), new Vector3(0.1f, 2.1f, 1f), "KayuGelap");
            SetTeks(pintuGudang.AddComponent<InteraktifAlur>(), "prompt", "Pintu gudang (terkunci)");

            // Empat saka guru dan blandar ruang tengah.
            foreach (float x in new[] { -1.5f, 1.5f })
                foreach (float z in new[] { 2.5f, 5.5f })
                    PembuatPengganti.Pasang("SakaGuru", null, new Vector3(x, 0f, z));
            Balok("Blandar", new Vector3(0f, 2.75f, 4f), new Vector3(6f, 0.3f, 0.3f), "KayuGelap");
        }

        // Benda atau tokoh yang membuka dialog: objek logika + PemicuDialog + prefab visual.
        // tengah = true untuk benda dinding (titik nol prefab di tengah benda).
        static PemicuDialog BendaDialog(Rig rig, string prefab, Vector3 posisi, Vector3 ukuran, float putarY, bool tengah,
            string dialog, string prompt)
        {
            var g = Logika(prefab, posisi, ukuran);
            PembuatPengganti.Pasang(prefab, g.transform, tengah ? new Vector3(0f, ukuran.y / 2f, 0f) : Vector3.zero, putarY);
            var pemicu = g.AddComponent<PemicuDialog>();
            Set(pemicu, "dialog", PembuatDataFase1.Muat(dialog));
            Set(pemicu, "panel", rig.Dialog);
            SetTeks(pemicu, "prompt", prompt);
            return pemicu;
        }

        static void Perabot(Rig rig)
        {
            // Ruang tengah
            PembuatPengganti.Pasang("Meja", null, new Vector3(-2.2f, 0f, 3.6f), 90f);
            PembuatPengganti.Pasang("Kursi", null, new Vector3(-1.6f, 0f, 3.6f), -90f);
            PembuatPengganti.Pasang("JamDinding", null, new Vector3(1.8f, 1.9f, 7.86f), 180f);
            BendaDialog(rig, "FotoKeluarga", new Vector3(-1.8f, 1.275f, 7.87f), new Vector3(0.6f, 0.45f, 0.1f), 180f, true,
                PembuatDataFase1.FotoKeluarga, "Periksa foto keluarga");
            var stopKontak = Logika("StopKontak", new Vector3(2.88f, 0.35f, 3.5f), new Vector3(0.15f, 0.15f, 0.15f));
            PembuatPengganti.Pasang("StopKontak", stopKontak.transform, new Vector3(0f, 0.075f, 0f), -90f);
            var pengisi = stopKontak.AddComponent<PengisiBaterai>();
            Set(pengisi, "hud", rig.Hud);
            SetTeks(pengisi, "prompt", "Isi baterai HP");

            // Kamar Raka, kamar Laras, kamar mandi, dapur
            PembuatPengganti.Pasang("Ranjang", null, new Vector3(-6.2f, 0f, 2f));
            PembuatPengganti.Pasang("Meja", null, new Vector3(-4.2f, 0f, 0.5f));
            PembuatPengganti.Pasang("Ranjang", null, new Vector3(6.2f, 0f, 6f));
            Balok("BakMandi", new Vector3(6.3f, 0.4f, 0.8f), new Vector3(1f, 0.8f, 1f), "Tanah");
            PembuatPengganti.Pasang("Meja", null, new Vector3(-5f, 0f, 10.5f));
            BendaDialog(rig, "Kalender", new Vector3(-1.5f, 1.25f, 8.13f), new Vector3(0.35f, 0.5f, 0.1f), 0f, true,
                PembuatDataFase1.Kalender, "Lihat kalender");
        }

        // Lampu listrik yang padam pada cutscene 23.00.
        static Light[] LampuListrik() => new[]
        {
            Lampu("Lampu ruang tengah", new Vector3(0f, 2.5f, 2.5f), WarnaLampuRumah, 7f, 1.6f),
            Lampu("Lampu kamar Ibu", new Vector3(-5f, 2.6f, 6f), WarnaLampuRumah, 7f, 1.6f),
            Lampu("Lampu kamar Raka", new Vector3(-5f, 2.6f, 2f), WarnaLampuRumah, 7f, 1.6f),
            Lampu("Lampu kamar Laras", new Vector3(5f, 2.6f, 6f), WarnaLampuRumah, 7f, 1.6f),
            Lampu("Lampu dapur", new Vector3(-2f, 2.6f, 10f), WarnaLampuRumah, 7f, 1.6f),
        };

        // Angka tahun di sisi selatan blandar. Hanya terbaca saat disorot senter (TerlihatSaatDisorot).
        static void Ukiran(Rig rig)
        {
            var ukiran = new GameObject("Ukiran blandar");
            ukiran.transform.position = new Vector3(0f, 2.75f, 3.84f);
            var teks = ukiran.AddComponent<TextMesh>();
            var font = AssetDatabase.LoadAssetAtPath<Font>(JalurFont) ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var tampilan = ukiran.GetComponent<MeshRenderer>();
            teks.font = font;
            tampilan.sharedMaterial = font.material;
            teks.text = GembokLemari.KodeBenar;
            teks.anchor = TextAnchor.MiddleCenter;
            teks.alignment = TextAlignment.Center;
            teks.fontSize = 64;
            teks.characterSize = 0.04f;
            teks.color = Hex("E6E0D1");

            var sorot = ukiran.AddComponent<TerlihatSaatDisorot>();
            Set(sorot, "senter", rig.Senter);
            Set(sorot, "kamera", rig.Kamera);
            Set(sorot, "tampilan", tampilan);
            Set(sorot, "hud", rig.Hud);
            SetTeks(sorot, "pesan", "Ada ukiran angka di blandar.");
        }

        static void KamarIbu(Rig rig, AlurPrologRumah prolog, AlurMalam1 malam1)
        {
            PembuatPengganti.Pasang("Ranjang", null, new Vector3(-6.2f, 0f, 6f));
            PembuatPengganti.Pasang("Meja", null, new Vector3(-6.3f, 0f, 4.45f));
            PembuatPengganti.Pasang("Kursi", null, new Vector3(-3.6f, 0f, 4.6f), -45f);

            // Ibu: objek logika di atas kasur (permukaan kasur y = 0.54). Adegan hanya mengguncang visualnya.
            var ibu = Logika("Ibu", new Vector3(-6.2f, 0.54f, 6f), new Vector3(1.1f, 0.5f, 1.9f));
            var visualIbu = PembuatPengganti.Pasang("BuRatmi_Berbaring", ibu.transform, Vector3.zero);
            var obat = ibu.AddComponent<PemberiObat>();
            Set(obat, "hud", rig.Hud);
            Set(obat, "panel", rig.Dialog);
            Set(obat, "dialogSetelahObat", PembuatDataFase1.Muat(PembuatDataFase1.IbuSetelahObat));
            Set(malam1, "ibu", visualIbu.transform);

            // Lemari Bapak di dinding utara kamar, menghadap selatan; gembok di depan pintunya.
            var lemari = Logika("LemariAyah", new Vector3(-4.2f, 0f, 7.6f), new Vector3(1.2f, 2f, 0.6f));
            PembuatPengganti.Pasang("LemariAyah", lemari.transform, Vector3.zero, 180f);
            var gembok = PembuatPengganti.Pasang("Gembok", lemari.transform, new Vector3(0f, 1f, -0.33f), 180f);
            var skripLemari = lemari.AddComponent<LemariAyah>();
            Set(skripLemari, "panel", rig.Gembok);
            Set(skripLemari, "gembok", gembok);
            SetTeks(skripLemari, "prompt", "Lemari Bapak");

            Set(prolog, "zonaKamarIbu", Zona("ZonaKamarIbu", new Vector3(-4.2f, 1.2f, 6f), new Vector3(1.4f, 2.4f, 2.4f)));
            Set(malam1, "titikMulai", Titik("TitikSampingRanjang", new Vector3(-4.6f, 0.05f, 6f), new Vector3(-6.2f, 0.05f, 6f)));
            var bidikIbu = Titik("BidikIbu", new Vector3(-4.3f, 1.7f, 5.2f), new Vector3(-6.2f, 0.7f, 6f));
            Set(prolog, "bidikIbu", bidikIbu);
            Set(malam1, "bidikIbu", bidikIbu);
            Cutscene23(rig, malam1, bidikIbu);
        }

        // Timeline Malam1_Cutscene23 (11,5 detik), urutan sama dengan Game Story Malam 1:
        // 0 kejang + batuk (3 detik), 3 paku + darah, 5 listrik padam, 6,5 potong ke plafon + langkah di atap.
        static void Cutscene23(Rig rig, AlurMalam1 malam1, Transform bidikIbu)
        {
            var dekat = Titik("BidikIbuDekat", new Vector3(-4.8f, 1.6f, 5.5f), new Vector3(-6.2f, 0.7f, 6f));
            var atap = Titik("BidikAtap", new Vector3(-5f, 1f, 5.5f), new Vector3(-5.5f, 3f, 6.5f));
            var c = PembuatTimeline.Buat("Malam1_Cutscene23", 11.5f);
            // Kamera mendekat pelan ke ranjang, lalu potong ke plafon dan diam di sana.
            PembuatTimeline.Gerak(c.Tl, c.Director, rig.KameraCutscene.gameObject,
                (0f, bidikIbu), (6.49f, dekat), (6.5f, atap), (11.5f, atap));
            PembuatTimeline.Sinyal(c.Tl, 0.05f, AlurMalam1.SinyalKejang);
            PembuatTimeline.Sinyal(c.Tl, 3f, AlurMalam1.SinyalPaku);
            PembuatTimeline.Sinyal(c.Tl, 5f, AlurMalam1.SinyalPadam);
            PembuatTimeline.Sinyal(c.Tl, 6.5f, AlurMalam1.SinyalLangkah);
            PembuatTimeline.Simpan(c.Tl);
            Set(malam1, "timeline23", c.Pemutar);
        }

        // Objek yang baru dinyalakan AlurMalam1 pada cutscene 23.00. Semuanya dibuat nonaktif.
        static void BendaAdegan(AlurMalam1 malam1)
        {
            var paku = Logika("Paku", new Vector3(-5f, 0f, 6.6f), new Vector3(0.5f, 0.1f, 0.5f));
            PembuatPengganti.Pasang("Paku", paku.transform, Vector3.zero);
            var foto = paku.AddComponent<PetunjukFoto>();
            SetTeks(foto, "efek", "tujuan:PotretPaku; foto:PakuMalam1");
            SetTeks(foto, "pesan", "Paku dipotret.");

            var bercak = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bercak.name = "Bercak";
            bercak.transform.position = new Vector3(-5.1f, 0.005f, 6.3f);
            bercak.transform.localScale = new Vector3(0.6f, 0.005f, 0.6f);
            bercak.GetComponent<Renderer>().sharedMaterial = PembuatPengganti.Material("Karat");
            Object.DestroyImmediate(bercak.GetComponent<Collider>());

            var teplok = new GameObject("Teplok");
            teplok.transform.position = new Vector3(-6.3f, 0.76f, 4.45f);
            PembuatPengganti.Pasang("Teplok", teplok.transform, Vector3.zero);
            Lampu("CahayaTeplok", new Vector3(-6.3f, 1f, 4.45f), WarnaTeplok, 6f, 2f).transform.SetParent(teplok.transform, true);

            var larasBerdoa = PembuatPengganti.Pasang("Laras_Berdoa", null, new Vector3(-5.2f, 0f, 4.9f), -60f);

            Set(malam1, "paku", paku);
            Set(malam1, "darah", bercak);
            Set(malam1, "teplok", teplok);
            Set(malam1, "larasBerdoa", larasBerdoa);
            foreach (var g in new[] { paku, bercak, teplok, larasBerdoa }) g.SetActive(false);
        }

        static void Tokoh(Rig rig, GameObject sutradara, AlurPrologRumah prolog, AlurMalam1 malam1)
        {
            var laras = BendaDialog(rig, "Laras", new Vector3(0.8f, 0f, 6.6f), new Vector3(0.5f, 1.7f, 0.5f), 180f, false,
                PembuatDataFase1.LarasKronologi, "Bicara dengan Laras");
            Set(malam1, "larasRuangTengah", laras.gameObject);

            // Bidan Wulan berdiri di ambang pintu kamar Ibu dan menutup jalan sampai ia pamit (AlurPrologRumah).
            var bidan = BendaDialog(rig, "BidanWulan", new Vector3(-3f, 0f, 6f), new Vector3(0.5f, 1.8f, 1f), 90f, false,
                PembuatDataFase1.BidanWulan, "Bicara dengan Bidan Wulan");
            SetBool(bidan, "sekaliSaja", true);
            Set(prolog, "bidanWulan", bidan.gameObject);

            // Bidan hanya ada di Prolog: "Lanjutkan" di Malam 1 memuat scene ini tanpa dia.
            var hanyaProlog = sutradara.AddComponent<AktifDiMalam>();
            SetAngka(hanyaProlog, "dari", 0);
            SetAngka(hanyaProlog, "sampai", 0);
            SetSatuObjek(hanyaProlog, "sasaran", bidan.gameObject);
        }

        static void SuaraDanJadwal(Rig rig, GameObject sutradara, AlurMalam1 malam1)
        {
            var jadwal = sutradara.AddComponent<JadwalObat>();
            Set(jadwal, "hud", rig.Hud);
            Set(jadwal, "panel", rig.Dialog);
            Set(jadwal, "teguranLaras", PembuatDataFase1.Muat(PembuatDataFase1.TeguranObat));
            Set(jadwal, "dentang", BuatSuara(rig, "DentangJam", Sfx, "SFX", "(Suara: jam dinding berdentang.)"));

            Set(malam1, "suaraBatuk", BuatSuara(rig, "BatukTersedak", Sfx, "SFX", "(Suara: Ibu terbatuk.)"));
            Set(malam1, "suaraPaku", BuatSuara(rig, "PakuJatuh", Sfx, "SFX", "(Suara: logam jatuh ke lantai.)"));
            Set(malam1, "suaraListrik", BuatSuara(rig, "ListrikPadam", Sfx, "SFX", "(Suara: listrik padam.)"));
            Set(malam1, "suaraLangkah", BuatSuara(rig, "LangkahDiAtap", Sfx, "SFX", "(Suara: langkah di atap.)"));
            Set(malam1, "musikDrone", BuatSuara(rig, "DroneHoror", Musik, "Music", "(Musik latar.)", ulang: true));
        }
    }
}
```

- [ ] **Step 2: `PembuatMenuUtama.cs`, daftar scene dan "Bangun Ulang Semua"**

a. Ganti seluruh isi metode `BangunSemua` (atribut `[MenuItem(...)]` tidak diubah):
```csharp
        public static void BangunSemua()
        {
            PembuatGayaPsx.Pasang();
            PembuatPengganti.Buat();
            PembuatDataFase1.Buat();
            PembuatSandbox.Buat();
            PembuatDesa.Buat();
            PembuatRumahKeluarga.Buat();
            Buat();   // terakhir: mendaftarkan scene dan meninggalkan menu utama terbuka
        }
```

b. Ganti seluruh metode `DaftarkanScene` beserta komentarnya:
```csharp
        // Urutan = indeks build. Menu utama harus indeks 0 supaya build dibuka dari sana.
        public static void DaftarkanScene()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(JalurScene, true),
                new EditorBuildSettingsScene(PembuatDesa.JalurScene, true),
                new EditorBuildSettingsScene(PembuatRumahKeluarga.JalurScene, true),
                new EditorBuildSettingsScene(PembuatSandbox.JalurScene, true),
            };
        }
```

- [ ] **Step 3: Bangun ulang semua lewat batchmode**

```bash
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod TheSending.EditorTools.PembuatMenuUtama.BangunSemua -logFile "${TMPDIR:-/tmp}/the-sending-bangun.log"; echo "exit=$?"
grep -E "Gaya PSX terpasang|Aset pengganti siap|Data Fase 1 dibuat|Scene sandbox dibuat|Scene desa dibuat|Scene rumah keluarga dibuat|Scene menu utama dibuat|error CS|Exception" "${TMPDIR:-/tmp}/the-sending-bangun.log" | sort -u
grep "path:" ProjectSettings/EditorBuildSettings.asset
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
```
Lalu cek Timeline cutscene 23.00:
```bash
ls Assets/_Project/Timeline/*.playable Assets/_Project/Timeline/Sinyal/*.asset
grep "m_TimeUpdateMode" Assets/_Project/Scenes/Scene_RumahKeluarga.unity
grep -c "SignalEmitter" Assets/_Project/Timeline/Malam1_Cutscene23.playable
```
Expected: `exit=0`; ketujuh baris "terpasang/siap/dibuat" ada; tanpa `error CS` dan `Exception`; empat baris `path:` berurutan: `Scene_MainMenu.unity`, `Scene_Desa.unity`, `Scene_RumahKeluarga.unity`, `Scene_Sandbox0B.unity`; tiga `.playable` dan enam sinyal; satu baris `m_TimeUpdateMode: 2` di rumah keluarga; `SignalEmitter` minimal `4`.

- [ ] **Step 4: Jalankan seluruh suite**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"`, tanpa `error CS`.

- [ ] **Step 5: Commit**

```bash
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git status --short
git add Assets/_Project/Scripts/Editor Assets/_Project/Scenes Assets/_Project/Data Assets/_Project/Timeline ProjectSettings/EditorBuildSettings.asset
git commit -m "feat: add Scene_RumahKeluarga with the 23.00 Timeline cutscene, register Fase 1 scenes and rebuild-all" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```
`Assets/_Project/Data/Sandbox/DialogContoh.asset.meta` ikut berubah (data contoh sandbox selalu dibuat ulang); itu wajar. Timeline Prolog ikut dibuat ulang (GUID baru) oleh "Bangun Ulang Semua"; itu wajar. Jika `git status` menunjukkan perubahan di luar lima jalur di atas (misalnya `Assets/Settings/`), jangan di-add; laporkan ke controller.

- [ ] **Step 6: BERHENTI (Urutan Eksekusi butir 4)**

Controller meminta tim menjalankan **Daftar Cek Play Mode Fase 1** (di bawah), mencatat hasilnya, lalu menutup editor. Setiap perbaikan dikerjakan di skrip pembuat atau skrip Runtime, dibangun ulang dengan perintah Step 3, dan di-commit sebagai `fix: <deskripsi>` dengan baris `Claude-Session` yang sama. Task 14 dimulai setelah tim menyatakan daftar cek selesai.

---

### Task 14: Dokumen, build uji Windows, dan pembaruan `CLAUDE.md`

**Files:**
- Modify: `docs/style-guide.md`, `docs/aset/KREDIT.md`, `CLAUDE.md`

**Interfaces:**
- Consumes: hasil Daftar Cek Play Mode Fase 1 dari tim; `PembuatBuild.Windows` (0C); `EditorBuildSettings.scenes` (Task 13).
- Produces: style guide dengan pengecualian kabut Prolog (K8), warna lampu listrik, dan aturan aset pengganti; kredit aset buatan sendiri; `CLAUDE.md` dengan konvensi baru (K6, adegan, scene buatan skrip), status Fase 1, catatan Fase 2, daftar cek Fase 1, dan log.

- [ ] **Step 1: `docs/style-guide.md`**

a. Di tabel "## Kabut dan cahaya", tambahkan satu baris tepat setelah baris `| Kabut | ...` (lewati jika tim memilih K8-B):
```markdown
| Kabut Prolog (pengecualian) | Di `Scene_Desa` selama Prolog kabut mulai 0.02, lalu naik ke 0.06 dalam 20 detik setelah azan (`AlurProlog.KabutSebelumMagrib`, `DetikKabutTurun`). Semua scene dan bab lain 0.06 sejak awal |
```

b. Di tabel "## Palet semantik", tambahkan satu baris tepat setelah baris `| Hangat = aman (lampu teplok) | ...`:
```markdown
| Lampu listrik rumah (padam sejak cutscene Malam 1 pukul 23.00) | `#FFD9A0` |
```

c. Tambahkan bagian baru di akhir file:
```markdown
## Aset pengganti (sejak Fase 1)

Selama aset asli belum ada, gim memakai pengganti buatan skrip (`PembuatPengganti`, `PembuatSuaraPengganti`): material polos di `Assets/_Project/Art/Materials/Pengganti/`, prefab dari primitif di `Assets/_Project/Prefabs/Pengganti/`, dan lima WAV sintetis di `Assets/_Project/Audio/SFX/` dan `Audio/Music/`. Skrip tidak menimpa file yang sudah ada. Cara menukar dengan aset asli: `docs/aset/fase-1.md`, bagian "Nama file dan prefab yang dipakai kode Fase 1".

Aturan gaya untuk pengganti dan aset asli sama: warna mengikuti palet di atas, tanpa kilap (Smoothness 0), dan tekstur mengikuti prasetel PSX.

Scene gameplay (`Scene_Desa`, `Scene_RumahKeluarga`) dibuat skrip dan ditimpa setiap **The Sending > Bangun Ulang Semua**. Tata letak diubah di skrip pembuatnya, bukan di scene.
```

- [ ] **Step 2: `docs/aset/KREDIT.md`**

Tambahkan dua baris di akhir tabel:
```markdown
| Suara sintetis: DentangJam, PakuJatuh, ListrikPadam, LangkahDiAtap, DroneHoror | `Assets/_Project/Audio/SFX/*.wav`, `Assets/_Project/Audio/Music/DroneHoror.wav` | Tim The Sending (dibuat skrip `PembuatSuaraPengganti`) | — | Buatan sendiri | Tidak |
| Material dan prefab pengganti | `Assets/_Project/Art/Materials/Pengganti/`, `Assets/_Project/Prefabs/Pengganti/` | Tim The Sending (dibuat skrip `PembuatPengganti`) | — | Buatan sendiri | Tidak |
```
Jika tim sudah menaruh rekaman atau model asli selama fase ini dan belum mencatatnya, minta tim menambahkan barisnya (pembuat, URL, lisensi). Jangan mengarang isi kredit.

- [ ] **Step 3: Jalankan seluruh suite**

Run: `bash tools/run-editmode-tests.sh`
Expected: `result="Passed" total="261" passed="261" failed="0"`.

- [ ] **Step 4: Uji build Windows lewat batchmode**

Jalankan dengan timeout Bash maksimum (600000 ms) atau `run_in_background`; jika terpotong timeout, jalankan lagi.
```bash
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD" -executeMethod TheSending.EditorTools.PembuatBuild.Windows -logFile "${TMPDIR:-/tmp}/the-sending-build-win.log"; echo "exit=$?"
grep -E "Build Windows selesai|Build Windows gagal|Shader error|error CS|BuildFailedException" "${TMPDIR:-/tmp}/the-sending-build-win.log" | sort -u | head
ls -la Builds/Windows/TheSending.exe
git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset
git status --short
```
Expected: `exit=0`; baris `Build Windows selesai: Builds/Windows/TheSending.exe (... MB)`; file `.exe` ada; tanpa `Shader error`; `Builds/` tidak muncul di `git status`. Build ini memuat empat scene. Menjalankannya di laptop Windows tetap pekerjaan Fase 8. Jika build gagal karena hal di luar kode proyek (modul, lisensi, platform), jangan habiskan lebih dari 30 menit: catat pesan errornya di log progres dan laporkan ke tim.

- [ ] **Step 5: Perbarui `CLAUDE.md`**

a. Di "## Konvensi Kode", ganti butir yang diawali `Assets/_Project/Tests/EditMode` (satu baris) menjadi (lewati jika tim memilih K6-B):
```markdown
- `Assets/_Project/Tests/EditMode`: assembly `TheSending.Tests.EditMode` (mereferensikan Core). Referensi ke Runtime hanya untuk test data (`DataDialogTests` memeriksa semua aset `DataDialog`); aturan gim tetap diuji lewat Core.
```

b. Di "## Konvensi Kode", tambahkan butir berikut tepat sebelum butir `**Commit tanpa baris ...`:
```markdown
- **Adegan dan cutscene:** `KameraAdegan.Mulai()` menjeda dan menyalakan `GameManager.Sinematik`. Cutscene memakai Timeline yang dibuat skrip (`PembuatTimeline`: track gerak + `SignalEmitter`), diputar sutradara lewat `PemutarTimeline.PutarDanTunggu()`; director Unscaled Game Time, wrap None. Sinyal diteruskan sebagai nama `SignalAsset` ke sutradara (konstanta `Sinyal...`). Jangan menyunting aset di `Assets/_Project/Timeline/` dengan tangan. Jangan memakai `WaitForSeconds` di adegan. Jam gim hanya maju saat pemain dikendalikan.
- **Suara tokoh ditunda:** semua percakapan teks saja (panel dialog, kartu). Grup mixer `Dialog` disiapkan untuk voice acting nanti.
- **Satu sutradara per bagian cerita** (`AlurProlog`, `AlurPrologRumah`, `AlurMalam1`): hanya mengurutkan kejadian dan memanggil kelas aturan Core. Scene awal ditentukan `PetaScene.Untuk(state)`.
- **Scene gameplay dibuat skrip** (`PembuatDesa`, `PembuatRumahKeluarga`, bagian bersama di `PembuatRig`) dan ditimpa setiap The Sending > Bangun Ulang Semua. Jangan menyunting scene itu dengan tangan. Batchmode: `-executeMethod TheSending.EditorTools.PembuatMenuUtama.BangunSemua`.
- **Teks dialog** ada di skrip pembuat data (`PembuatDataFase1`), bukan disunting di Inspector. Pembuat menolak teks kosong, penanda `[[...]]`, dan kode efek yang salah.
- **Aset pengganti** di `Art/Materials/Pengganti/`, `Prefabs/Pengganti/`, `Audio/SFX/`, `Audio/Music/`. Skrip tidak menimpa file yang ada. Benda di scene = objek logika (collider + skrip) dengan prefab visual sebagai anak.
- Setelah setiap run batchmode: `git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset` (batchmode menghapus define `SENTIS_ANALYTICS_ENABLED`; penghapusan itu tidak di-commit).
```

c. Di tabel "## Fase dan Progres", ganti baris Fase 1. Jika semua butir daftar cek sudah dinyatakan selesai oleh tim:
```markdown
| 1. Prolog + Malam 1 | H4–H6 | 19–21 Nov | [x] Selesai (261 EditMode test lulus, cek Play Mode tim selesai, build uji Windows berhasil). Semua model dan sebagian suara masih aset pengganti; suara tokoh ditunda |
```
Jika masih ada butir yang belum lolos, pakai `[~] Selesai di kode ...` dan sebut nomor butir yang tersisa.

d. Ganti seluruh bagian "## Catatan untuk Fase 1 dan sesudahnya (dari review 0B dan keputusan 0C)" (dari judul itu sampai tepat sebelum "## Daftar Cek Play Mode Fase 0B") dengan:
```markdown
## Catatan untuk fase berikutnya (dari review 0B, keputusan 0C, dan Fase 1)

Semua catatan "Fase 1" dari review 0B sudah dikerjakan di Fase 1: scene per bab (`PetaScene`), inventaris (I), Esc menutup Jurnal/inventaris/gembok (tidak menutup dialog), jam hanya maju saat pemain dikendalikan, penjaga `PanelDialog`, saringan pesan kembar HUD, peringatan lebih dari 4 pilihan, dan test semua aset `DataDialog`.

Fase 2:
- `AlurMalam1.GantiMalam` berakhir di kartu "bersambung" lalu kembali ke menu; ganti bagian itu dengan sutradara Malam 2. `AlurMalam1.Start` menampilkan notifikasi sementara jika scene dimuat pada Malam 2 ke atas; hapus saat Malam 2 ada.
- `PetaScene.Untuk` baru mengenal Prolog dan rumah keluarga; tambah scene bab berikutnya di sana (dengan test).
- Pintu gudang di `Scene_RumahKeluarga` adalah `InteraktifAlur` tanpa pelanggan (hanya prompt "terkunci"). Sutradara Malam 2 yang membukanya.
- Ditunda dari Fase 1: tampilan Kondisi Ibu, lampu teplok yang bisa dibawa, model HP di tangan, isi 11 entri Catatan Bapak/Raka lainnya, bentuk aksara di Halaman 2 (sekarang hanya urutan bacanya).
- Tokoh belum beranimasi. Cutscene sudah Timeline (K1); saat model Mixamo datang, tambahkan `AnimationTrack` tokoh di `PembuatTimeline` (bukan di jendela Timeline). Animator di luar Timeline: Update Mode Unscaled Time.
- Suara tokoh ditunda (voice acting "belakangan"); Malam 2 juga teks saja kecuali tim memutuskan lain.
- Pakai ulang untuk malam berikutnya: `PembuatRig`, `PembuatTimeline`, `PemutarTimeline`, `ZonaPemicu`, `InteraktifAlur`, `KameraAdegan`, `Suara`, `TerlihatSaatDisorot`, `JadwalObat` (sudah berlaku Malam 1–6), `PemberiObat`, jalur `foto:` di Papan Bukti.
- Hasil ukur waktu tunggu (K4) dan catatan denah (K7) dari daftar cek Fase 1: lihat log progres. Jika tim meminta "duduk menunggu", itu sisipan kecil di awal Fase 2.

Fase 7: hentikan gerak dan input pemain setelah `State.Ending` terisi. Layar ending tidak punya pemain, jadi kursor otomatis terlihat. `AlurMalam1.GantiMalam` sekarang kembali ke menu utama jika ending tercapai saat pergantian malam; ganti dengan layar ending.

Fase 8: `UnityConnectSettings` m_Enabled 1 dan define `SENTIS_ANALYTICS_ENABLED` (ditambah editor sendiri) menyalakan layanan/analytics Unity; pastikan dimatikan untuk build rilis jika tidak dipakai. Pengaturan grafis baru "Layar penuh" saja; tambahkan pemilih resolusi jika tes Windows membutuhkannya. `PembuatBuild.Windows` membuat development build (tombol uji F5/F6/F9 hidup); build rilis harus non-development. Aset pengganti yang masih tersisa ditukar atau dicatat di layar kredit.
```

e. Tambahkan bagian baru tepat sebelum "## Log Progres": judul `## Daftar Cek Play Mode Fase 1 (dikerjakan tim)`, lalu **salin seluruh isi** bagian "Daftar Cek Play Mode Fase 1" di akhir rencana ini (kalimat prasyarat dan butir 1–35, tanpa mengubah nomor). Di bawahnya tambahkan satu baris hasil: tanggal cek dan nomor butir yang belum lolos (atau "semua lolos").

f. Tambahkan di akhir "## Log Progres" (isi `YYYY-MM-DD` dengan tanggal hari itu; isi bagian dalam kurung siku dari hasil daftar cek butir 32–35):
```markdown
- **YYYY-MM-DD:** Fase 1 selesai: Prolog dan Malam 1 bisa dimainkan dari "Mulai Baru" sampai kartu Malam 2. Core: inventaris, `GembokLemari.Buka`, foto petunjuk di Papan Bukti, jendela obat, `PetaScene`, `PemeriksaDialog` (261 EditMode test). Runtime: panel inventaris dan gembok, alat adegan, obat Ibu, ukiran blandar, tiga sutradara, tiga cutscene Timeline yang dibuat skrip. Percakapan teks saja (suara tokoh ditunda). Scene `Scene_Desa` dan `Scene_RumahKeluarga` dibuat skrip; semua model dan lima suara masih aset pengganti. Build uji Windows berhasil. Waktu tunggu (K4): [hasil ukur tim]. Denah (K7): [catatan tim]. Berikutnya: rencana Fase 2 (Malam 2).
```

- [ ] **Step 6: Commit**

```bash
git status --short
git add docs/style-guide.md docs/aset/KREDIT.md CLAUDE.md
git commit -m "docs: record Fase 1 conventions, Play Mode checklist and progress" -m "Claude-Session: https://claude.ai/code/session_011hHEoA3ue8iJkTKYbVgboQ"
```

---

## Daftar Cek Play Mode Fase 1 (dikerjakan tim)

Prasyarat: Task 13 selesai dan editor dibuka sesudahnya. Buka `Assets/_Project/Scenes/Scene_MainMenu.unity`, tekan Play. Tombol uji (hanya editor dan development build): **F5** simpan checkpoint, **F6** jam gim +30 menit, **F9** tertangkap. Selama file suara asli belum ada, bunyi diganti teks "(Suara: ...)" di HUD; itu dihitung lolos. Tidak ada suara tokoh di Fase 1: semua percakapan hanya teks. Tiga cutscene (butir 2, 8, 23) adalah Timeline; selama cutscene Esc tidak membuka menu jeda. Catat nomor butir yang gagal beserta apa yang terlihat.

**Menu dan Prolog di jalan desa**

1. "Mulai Baru" memuat jalan desa, bukan sandbox. Layar hitam, kartu "Telepon dari Laras" tampil sekitar 8 detik (teks saja, tanpa suara).
2. Kamera adegan menampilkan gapura dan ojek. Dialog Tukang Ojek 1 baris; E menutupnya. Timeline: ojek bergerak mulus menjauh sekitar 3 detik (tidak melompat ke titik nol dunia), suara motor di awal, lalu ojek hilang dan kontrol kembali.
3. Kartu "Prolog: Pulang" tampil sekitar 4 detik, lalu pemain bisa bergerak dan muncul notifikasi cara berjalan dan berlari. Selama adegan dan kartu: pemain tidak bisa bergerak, titik bidik, jam, dan baterai tidak tampil.
4. Jam mulai 17.30 dan baru berjalan setelah pemain bisa bergerak. Esc (menu jeda) menghentikan jam.
5. Kabut awal tipis (gapura masih terlihat dari sekitar 30 m). Di kebun bambu muncul notifikasi senter (F) satu kali saja, walaupun zona dilewati bolak-balik.
6. Jembatan bambu bisa dilewati tanpa tersangkut, dan pemain tidak bisa turun ke air di kiri atau kanannya.
7. Setelah jembatan: azan terdengar satu kali (atau teks penggantinya), lalu kabut menebal selama sekitar 20 detik sampai jarak pandang sekitar 15 m.
8. Di depan rumah Bu Sumi: Timeline: kamera adegan membidik jendela dan mendekat pelan, sekitar 2,5 detik kemudian siluet hilang, tirai menutup, lampu jendela meredup (sekali, tidak meredup dua kali), lalu kontrol kembali sekitar 4 detik sejak mulai. Terjadi satu kali saja.
9. Pemain tidak bisa keluar dari jalan: ke kiri, ke kanan, atau ke belakang gapura.
10. Pintu rumah keluarga menampilkan prompt "Masuk rumah". E: suara pintu, layar memudar ke hitam, lalu rumah keluarga dimuat.

**Prolog di rumah**

11. Layar memudar dari hitam, pemain berdiri di dalam pintu depan menghadap ruang tengah, dan dialog sambutan Laras (3 baris) berjalan sendiri.
12. Bidan Wulan berdiri di ambang pintu kamar Ibu dan menghalangi jalan masuk. E: dialog 3 baris, lalu notifikasi barang baru.
13. Setelah dialog itu: layar hitam sebentar, Bidan Wulan tidak ada lagi, lalu tiga notifikasi berurutan (pamit, tombol I, tombol Tab) tanpa pesan kembar. Dialog Bidan Wulan tidak bisa diulang.
14. I membuka inventaris: 1 barang dengan nama dan keterangan, jam berhenti. I atau Esc menutupnya. Esc yang menutup panel tidak ikut membuka menu jeda.
15. Tab membuka Jurnal; Esc menutupnya. Tab Catatan: dua entri pertama Catatan Raka berisi teks.
16. Masuk kamar Ibu: kamera adegan membidik ranjang, dialog 3 baris, layar hitam, kartu "Malam 1: Paku Pertama" sekitar 5 detik. Sesudahnya pemain berdiri di samping ranjang dan muncul notifikasi jam obat.

**Malam 1**

17. E ke Ibu sebelum 21.00 (prompt "Beri obat Ibu"): pesan bahwa belum waktunya, dengan jam yang benar.
18. Laras di ruang tengah: dialog 5 baris. Foto keluarga: dialog 2 baris. Kalender di dapur: 1 baris. Pintu gudang menampilkan prompt "Pintu gudang (terkunci)" dan tidak terbuka. Stop kontak mengisi baterai HP.
19. Blandar ruang tengah: tanpa senter, angka tidak terlihat. Senter (F) diarahkan ke blandar dari jarak kurang dari 7 m: angka terlihat dan notifikasi muncul satu kali. Angka tidak terlihat menembus dinding dari ruangan lain.
20. Lemari Bapak di kamar Ibu: E membuka panel gembok. Kode salah: pesan salah, panel tetap terbuka. Backspace menghapus angka, Esc membatalkan. Kode benar: gembok hilang, inventaris bertambah 2 barang, dan Jurnal > Buku Harian menampilkan Halaman 1 dan 2 (font Kalam).
21. F6 memajukan jam 30 menit. Pukul 21.00: jam berdentang dan muncul notifikasi HP. E ke Ibu: Kondisi Ibu naik 2 (60 menjadi 62), lalu dialog Ibu 1 baris dengan terjemahan miring dalam kurung. E sekali lagi: pesan bahwa obat sudah diberikan.
22. Uji terpisah ("Mulai Baru" lagi): biarkan lewat pukul 22.00 tanpa memberi obat. Dialog teguran Laras (2 baris) muncul satu kali; E ke Ibu: pesan bahwa jam obat sudah lewat.
23. Pukul 23.00, di ruangan mana pun pemain berada, cutscene Timeline berjalan (sekitar 11,5 detik): kamera ke ranjang lalu mendekat pelan, guncangan sekitar 3 detik, dua objek muncul di lantai, lampu listrik padam, teplok menyala, kamera memotong ke plafon sekitar 5 detik, lalu kontrol kembali dengan pemain di samping ranjang. Urutan bunyi: batuk, logam jatuh, listrik padam, langkah di atap. Musik latar terus berbunyi tanpa jeda yang terdengar di sambungan loop. Esc selama cutscene tidak membuka menu jeda. Buka `Assets/_Project/Timeline/Malam1_Cutscene23.playable` di jendela Timeline (pilih objek `Timeline Malam1_Cutscene23`): satu track gerak kamera dan empat sinyal di baris marker; director Update Method **Unscaled Game Time**.
24. Setelah cutscene: Laras tidak ada lagi di ruang tengah dan berlutut di dekat ranjang. Dua notifikasi tutorial memotret muncul berurutan.
25. Q ke arah paku di lantai (jarak sampai 6 m): kilatan, notifikasi foto, dan Jurnal > Papan Bukti menampilkan satu baris `[FOTO]`. Memotret lagi tidak menambah baris dan tidak memunculkan notifikasi kembar.
26. Setelah foto: dialog penutup 2 baris. Jika lemari belum dibuka, muncul pengingat lemari dan malam baru berganti setelah lemari dibuka. Jika lemari sudah dibuka: layar hitam, kartu "Malam 2: Boneka di Bawah Lantai" sekitar 8 detik, lalu kembali ke menu utama.

**Save**

27. "Mulai Baru" saat sudah ada save memunculkan konfirmasi. "Batal" kembali ke menu. "Ya, mulai baru": Prolog mulai lagi dari gapura dengan Kondisi Ibu 60 dan inventaris kosong.
28. "Lanjutkan" memuat scene yang benar: keluar ke menu saat masih di jalan desa lalu "Lanjutkan" = jalan desa dari awal Prolog; setelah masuk rumah = rumah keluarga dengan sambutan Laras; setelah kartu Malam 1 = di samping ranjang pada Malam 1 dengan inventaris yang sama dan tanpa Bidan Wulan; setelah kartu Malam 2 = rumah keluarga dengan notifikasi "Malam 2 belum dibuat (Fase 2)." dan Kondisi Ibu 12 lebih rendah daripada di akhir Malam 1.
29. Di Malam 1 tekan F9: Kondisi Ibu turun 5 dan scene dimuat ulang dari checkpoint. Stop Play, Play lagi, "Lanjutkan": penalti tetap. Lalu setelah cutscene 23.00 tekan F5, keluar ke menu, "Lanjutkan": rumah langsung dalam keadaan sesudah cutscene (lampu listrik padam, teplok menyala, paku di lantai, Laras di dekat ranjang) dan cutscene tidak terulang.

**Gaya, suara, dan catatan**

30. Gaya PSX sama di kedua scene dan di kedua kamera (kamera pemain dan kamera adegan): piksel besar yang tajam, dither, kabut. Tidak ada bidikan adegan yang tampak lebih halus daripada tampilan bermain.
31. Esc membuka menu jeda di kedua scene. Mengubah volume di Pengaturan memengaruhi bunyi adegan dan musik latar. Tidak ada error merah di Console selama seluruh alur dari butir 1 sampai 26.
32. Waktu tunggu (K4): main tanpa F6 dan catat berapa menit nyata dari tiba di rumah sampai pukul 21.00, dari memberi obat sampai pukul 23.00, dan berapa menit di antaranya tidak ada yang bisa dikerjakan.
33. Denah (K7): nilai ukuran ruangan, lebar pintu, letak perabot, dan panjang jalan desa. Catat yang perlu digeser; perubahannya dikerjakan di `PembuatDesa.cs` dan `PembuatRumahKeluarga.cs`.
34. Teks (K5): baca semua teks yang ditulis pengembang (sambutan Laras, kronologi, adegan Ibu di Prolog, teguran obat, kalender, Halaman 1, dua Catatan Raka, notifikasi tutorial) dan catat suntingannya. Dua Catatan Raka berisi informasi kesehatan dasar: periksa isinya.
35. Catat aset pengganti yang paling mengganggu (model atau suara) supaya dicari lebih dulu.

**Jika butir 19 gagal karena angka terlihat menembus dinding atau tidak terlihat sama sekali:** shader font bawaan `TextMesh` tidak menguji kedalaman dan tidak terkena kabut. Laporkan ke controller; jalan keluarnya mengganti ukiran dengan kotak-kotak kecil bermaterial `Kertas` yang membentuk angka, tanpa mengubah `TerlihatSaatDisorot`. **Jika butir 5, 7, atau 8 tidak terpicu:** `CharacterController` tidak memicu `OnTriggerEnter` pada zona; laporkan, jalan keluarnya menambah `Rigidbody` kinematik pada objek zona di `PembuatRig.Zona`. **Jika bunyi adegan tidak terdengar saat cutscene** padahal filenya ada: periksa apakah jeda ikut menghentikan audio; laporkan. **Jika cutscene Timeline beku di frame pertama dan gim terkunci:** Update Method director bukan Unscaled Game Time, atau wrap mode bukan None; perbaiki di `PembuatTimeline.Buat`. **Jika kamera atau ojek melompat ke titik nol dunia:** `removeStartOffset` atau track offset; laporkan, perbaiki di `PembuatTimeline.Gerak`. **Jika sinyal tidak terpicu** (tidak ada bunyi atau benda tidak muncul selama cutscene, tetapi keadaan akhir benar): sutradara memakai cadangan sesudah timeline; laporkan, cek apakah `PemutarTimeline` ada di objek director yang sama.

## Self-Review

**Cakupan spesifikasi**

| Butir lingkup Fase 1 | Task |
|---|---|
| Daftar aset Fase 2 dan nama file aset Fase 1 (alur aset `CLAUDE.md`) | 1 |
| Inventaris (I), efek `barang:`, lemari ayah dan gembok 2003 | 2, 7, 13 |
| Foto paku masuk Papan Bukti sebagai `[FOTO]` (K2, desain 4.7) | 3, 7, 13 |
| Jendela obat 21.00–22.00, obat +2, pengingat dan teguran (desain 4.4, Game Story 6.7) | 4, 8, 11, 13 |
| Scene per bab, "Lanjutkan" memuat scene sesuai state, `SceneAwal` dihapus | 4, 6, 13 |
| Test semua aset `DataDialog` (K6) | 5, 11 |
| Jam hanya maju saat pemain dikendalikan; Esc menutup panel (K3); penjaga dialog; saringan HUD; peringatan lebih dari 4 pilihan; alat uji F6 | 6 |
| Alat adegan: kamera adegan, zona, interaktif alur, suara dengan teks pengganti, pemutar Timeline (K1) | 8 |
| Tiga cutscene Timeline dibuat skrip, director Unscaled Game Time (K1, `CLAUDE.md`) | 8, 9, 12, 13 |
| Suara tokoh ditunda: telepon dan doa Laras teks/pose saja, aset ditandai "belakangan" | 1, 9, 12, 13 |
| Ukiran blandar yang terbaca saat disorot senter | 8, 13 |
| Prolog: telepon, ojek di gapura, tutorial jalan/lari/senter, azan dan kabut (K8), Bu Sumi di jendela, tiba di rumah | 9, 11, 12 |
| Prolog di rumah: Laras, Bidan Wulan memberi obat, tutorial inventaris dan Jurnal, kamar Ibu, masuk Malam 1 | 9, 11, 13 |
| Malam 1: menjelajah, kronologi Laras, obat, gembok, Halaman 1 dan 2, cutscene 23.00, memotret paku, penutup, pergantian malam dan cek `State.Ending` | 2, 8, 9, 11, 13 |
| Enam cutscene Fase 1 di desain Bagian 5 | 9 (alur), 12 dan 13 (Timeline, bidikan, objek, suara) |
| Checkpoint: Mulai Baru, tiba di rumah, awal Malam 1, awal Malam 2 | 6, 9 |
| Aset pengganti: material, 24 prefab, 5 suara sintetis; azan tidak sintetis | 10 |
| Denah desa dan rumah (K7); gaya PSX di kedua kamera; `MenuJeda` + `PanelPengaturan` di scene baru | 12, 13 |
| Build Settings 4 scene dan "Bangun Ulang Semua" | 13 |
| Style guide (K8, lampu listrik, aset pengganti), kredit, build uji Windows, `CLAUDE.md` | 14 |
| Daftar cek Play Mode Fase 1, termasuk pengganti butir 4, 15, 16 daftar cek 0C (butir 1, 27–29) | bagian di atas, 14 |
| Ditunda (R6): tampilan Kondisi Ibu, teplok yang dibawa, HP di tangan, 11 catatan lain, bentuk aksara Halaman 2 | Tidak dikerjakan; dicatat untuk Fase 2 di Task 14 Step 5d |
| "Duduk menunggu" (K4-B) | Tidak dikerjakan; diputuskan setelah butir 32 |

**Pemindaian placeholder:** tidak ada "TBD" atau langkah tanpa kode. Isian yang disengaja: (1) 26 penanda `[[SALIN ...]]` dan `[[TULIS ...]]` di Task 11 Step 1, masing-masing punya sumber dan aturan di tabel Step 2, dijaga `grep` di Step 3 dan `TanpaPenanda` saat pembuat dijalankan; rencana ini sengaja tidak memuat kalimat cerita. (2) `YYYY-MM-DD` dan dua kurung siku hasil cek di log progres (Task 14 Step 5f). (3) Varian baris status Fase 1 (Task 14 Step 5c) bergantung pada hasil cek tim. Angka tata letak (denah, posisi kamera adegan, kekuatan lampu) dan parameter suara sintetis adalah nilai awal rencana ini, dinilai tim di butir 30–35.

**Konsistensi tipe dan nama**
- Field `AlurProlog` (Task 9) = yang disetel `PembuatDesa`: `adegan`, `kartu`, `dialog`, `hud`, `dialogOjek`, `bidikGapura`, `bidikJendela`, `ojek`, `timelineOjek`, `timelineBuSumi`, `zonaSenter`, `zonaAzan`, `zonaBuSumi`, `pintuRumah`, `siluetBuSumi`, `tiraiJendela`, `lampuJendela`, `suaraMotor`, `suaraAzan`, `suaraPintu` (20 field, 20 `Set`). `TitikOjekPergi` dan `BidikJendelaDekat` hanya dipakai pembuat Timeline.
- Field `AlurPrologRumah` = yang disetel `PembuatRumahKeluarga`: `adegan`, `kartu`, `dialog`, `hud`, `pemain`, `dialogLaras`, `dialogIbu`, `titikMulai`, `bidikIbu`, `bidanWulan`, `zonaKamarIbu`, `malam1` (12).
- Field `AlurMalam1`: `adegan`, `kartu`, `dialog`, `hud`, `pemain`, `dialogPenutup`, `titikMulai`, `bidikIbu`, `timeline23`, `ibu`, `larasRuangTengah`, `larasBerdoa`, `paku`, `darah`, `teplok`, `lampuListrik` (lewat `SetDaftar`), `suaraBatuk`, `suaraPaku`, `suaraListrik`, `suaraLangkah`, `musikDrone` (21). `BidikAtap` dan `BidikIbuDekat` hanya dipakai pembuat Timeline.
- Nama sinyal: `AlurProlog.SinyalMotorPergi/SinyalTiraiDitutup` dan `AlurMalam1.SinyalKejang/SinyalPaku/SinyalPadam/SinyalLangkah` dipakai sebagai konstanta oleh `PembuatTimeline.Sinyal` di Task 12 dan 13, dan dicocokkan `PemutarTimeline` lewat `SignalAsset.name` (nama file `Timeline/Sinyal/<nama>.asset`). `Rig.KameraCutscene` (Task 12) dipakai Task 12 dan 13.
- Field komponen lain: `KameraAdegan.kameraPemain/kameraAdegan` (`PemutarTimeline` tanpa field; director disetel `PembuatTimeline.Buat`), `Suara.sumber/hud/teksPengganti`, `TerlihatSaatDisorot.senter/kamera/tampilan/hud/pesan`, `PemberiObat.hud/panel/dialogSetelahObat`, `JadwalObat.hud/panel/teguranLaras/dentang`, `PanelGembok.hud`, `LemariAyah.panel/gembok`, `PemicuDialog.dialog/panel/sekaliSaja`, `PetunjukFoto.efek/pesan`, `AktifDiMalam.dari/sampai/sasaran`, `Interaktif.prompt`, `PengisiBaterai.hud` cocok dengan deklarasi di Task 7, 8 dan kode 0B.
- Tipe nilai: `pemain` menerima `PemainGerak` (`Rig.Pemain`); `senter` menerima `SenterHP`; `kamera` menerima `Transform`; `ibu`, semua `bidik...`, `titik...`, `ojek` menerima `Transform`; `bidanWulan`, `larasRuangTengah`, `larasBerdoa`, `paku`, `darah`, `teplok`, `siluetBuSumi`, `tiraiJendela`, `gembok` menerima `GameObject`.
- Sepuluh nama dialog di `PembuatDataFase1` dipakai dengan konstanta yang sama di Task 12 (`Ojek`) dan Task 13 (sembilan lainnya). Efek yang dipakai (`barang:ObatIbu`, `tujuan:BicaraBidan`, `tujuan:PotretPaku`, `foto:PakuMalam1`) sama dengan `Inventaris.ObatIbu`, `TujuanMalam.BicaraBidan/PotretPaku`, `PapanBukti.FotoPakuMalam1`.
- Nama file suara dan 24 nama prefab di Task 10 sama dengan tabel Task 1 dan dengan argumen `BuatSuara`/`Pasang` di Task 12 dan 13. Sembilan suara: tiga di `PembuatDesa`, enam di `PembuatRumahKeluarga` (`TeleponLaras` dan `BisikanDoa` dihapus: suara tokoh ditunda).
- Grup mixer yang dipakai (`SFX`, `Ambience`, `Music`) ada di style guide; `Dialog` belum dipakai di Fase 1.
- `PembuatSandbox.Kotak/SetTeks/SetAngka/SetSatuObjek/SetBool/SetDaftar` dibuat `internal` di Task 10 sebelum dipakai Task 12 dan 13. `PembuatDesa.JalurScene` dan `PembuatRumahKeluarga.JalurScene` memakai `PetaScene.Desa/RumahKeluarga`, jadi nama scene di Build Settings sama dengan yang dimuat `GameManager.SceneUntukState`.
- Jumlah test: 261 sejak Task 5 sampai Task 14; Task 10–14 tidak menambah test. Pembuat (Task 10, 11) memeriksa dirinya sendiri dan menggagalkan batchmode jika salah.

**Yang belum bisa diverifikasi tanpa menjalankan Unity** (implementer melapor jika berbeda): `TextMesh` dengan material font bawaan di URP (terlihat, tidak menembus dinding; jalan keluar ada di bawah daftar cek); `CharacterController` memicu `OnTriggerEnter` pada `ZonaPemicu`; audio tetap berbunyi saat `Time.timeScale` 0; `mixer.FindMatchingGroups("SFX")[0]` mengembalikan grup yang benar; `Shader.Find("Universal Render Pipeline/Lit")` berhasil di batchmode; `AudioClip.samples` terisi tepat setelah `ImportAsset`; rel jembatan dan lebar pintu 0,85 m nyaman dilewati kapsul pemain; arah hadap prefab setelah diputar (rumah Bu Sumi, lemari, foto); `PembuatGayaPsx.Pasang` tidak mengubah `Assets/Settings/` saat dijalankan ulang; kode Task 1–9 yang belum ada di repo saat rencana ditulis (nama field diambil dari teks rencana, bukan dari kompilasi). **Timeline (K1):** API sudah dicek di `Unity.Timeline.api` paket 6.6.0 (termasuk: marker track timeline mengirim notifikasi ke `INotificationReceiver` di objek director, `TimelinePlayable.cs`), tetapi belum terbukti di Play Mode: `AnimationClip.SetCurve` dengan `m_LocalPosition`/`localEulerAnglesRaw` pada klip non-legacy, `removeStartOffset = false` + track offset nol memberi posisi dunia, sinyal di 0,05 detik terpicu, event `stopped` terpicu dengan wrap None, `AssetDatabase.CreateAsset` untuk `.playable`, dan nilai serialisasi `m_TimeUpdateMode: 2` untuk Unscaled Game Time.

