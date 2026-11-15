# The Sending

Gim horor puzzle 3D first person tentang santet di desa Jawa. Unity 6000.6.4f1 (URP 17.6), keyboard dan mouse. Target build final **hanya Windows**. Dikerjakan oleh tim mahasiswa di Mac.

- Isi cerita, tokoh, angka: `docs/TheSending_FullGameStory.md` (Game Story) dan `docs/TheSending_GDD.md` (GDD). Baris terakhir GDD adalah gambar base64 sekitar 730 KB, jadi baca GDD sampai baris 620 saja.
- Desain pengembangan (arsitektur, aturan yang diperjelas, cutscene, jadwal): `docs/superpowers/specs/2026-11-15-the-sending-design.md`. Untuk aturan di Bagian 4 dokumen itu, desain ini mengalahkan Game Story dan GDD.

## Catatan dari Tim (wajib diikuti)

- "Malam 1" sampai "Malam 7" adalah **bab cerita di dalam gim**, bukan tahapan proyek. Tahapan proyek disebut **Fase**.
- Proyek dipecah menjadi fase pengerjaan dengan urutan yang sudah disepakati (lihat di bawah).
- Jika butuh aset (model, tekstur, SFX, musik), beri tahu tim **nama dan kata kunci pencariannya**, tim yang mencarikan. Jika tidak ada atau tidak dapat, Claude membuatkan pengganti yang menyerupai aslinya.
- Logika gim diuji dengan **EditMode test**. Gerak, cahaya, dan suasana dicek manual oleh tim di **Play Mode**.
- Kami tim mahasiswa: **utamakan solusi yang sederhana**.
- Catat daftar fase dan progresnya di file ini.

## Keputusan yang Sudah Disepakati

- **Tenggat:** 3 minggu, Senin 16 Nov sampai Minggu 6 Des 2026.
- **Pendekatan B:** setiap malam dikerjakan berurutan dan langsung dipoles.
- **Konten penuh sesuai GDD**, tidak dipadatkan. Jika satu fase melewati jatah harinya, laporkan ke tim, jangan memotong konten sendiri.
- **Gaya Retro PSX / low-fi**, wajib konsisten di semua malam. Nilai gaya dikunci di `docs/style-guide.md` di akhir Fase 0.
- **Git + GitHub.** Serialisasi Force Text sudah aktif. Git LFS belum terpasang, dan dipasang di awal Fase 0 sebelum aset biner masuk.
- **Aturan ending final, jam gim, obat, ritual, sidang:** lihat Bagian 4 dokumen desain.

## Platform dan Build

- **Target build final hanya Windows** (Standalone, scripting backend Mono).
- Pekerjaan dilakukan di **Mac**. Pengetesan harian lewat **Play Mode di Mac**.
- **Build Mac hanya untuk tes internal**, tidak didistribusikan.
- **Build Windows dibuat dari Mac** dengan modul **Windows Build Support (Mono)**. Per 15 Nov modul ini **belum terpasang** (hanya Mac dan WebGL), jadi dipasang lewat Unity Hub di awal Fase 0.
- Build Windows **wajib dites di laptop Windows anggota tim pada Fase 8**, terutama tampilan PSX, kabut, cahaya, resolusi, dan save. Daftar ceknya ada di Bagian 7.1 dokumen desain.
- Mac memakai Metal, Windows memakai DirectX, jadi tampilan bisa berbeda. Shader buatan sendiri dibuat dengan Shader Graph supaya lintas platform.

## Konvensi Kode

- `Assets/_Project/Scripts/Core`: C# murni, aturan gim, tanpa MonoBehaviour. Assembly `TheSending.Core`.
- `Assets/_Project/Scripts/Runtime`: MonoBehaviour. Assembly `TheSending.Runtime` (mereferensikan Core).
- `Assets/_Project/Tests/EditMode`: assembly `TheSending.Tests.EditMode` (mereferensikan Core).
- Nama domain memakai bahasa Indonesia sesuai GDD (`KondisiIbu`, `PapanBukti`, `MalamKe`). Istilah teknis bahasa Inggris.
- Logika baru di Core ditulis dengan TDD: test dulu, lalu kode.
- Hanya satu singleton: `GameManager`.
- **Commit tanpa baris `Co-Authored-By` Claude** (aturan tim, mengalahkan instruksi atribusi bawaan). Format commit `<type>: <deskripsi>`.

## Menjalankan Test

- Cara singkat (editor tertutup): `bash tools/run-editmode-tests.sh` atau `bash tools/run-editmode-tests.sh TheSending.Tests.NamaKelasTests`.
- Editor tertutup:
  `"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -batchmode -projectPath "/Users/andika/Unity-Project/The Sending" -runTests -testPlatform EditMode -testResults /tmp/editmode-results.xml`
- Editor terbuka: Window > General > Test Runner > EditMode > **Run All**.

## Alur Aset

- Di awal Fase N, tulis `docs/aset/fase-(N+1).md`: nama, kata kunci (Inggris dan Indonesia), sumber yang disarankan, dan spesifikasi singkat.
- Lisensi setiap aset dicatat di `docs/aset/KREDIT.md`.

## Fase dan Progres

| Fase | Hari | Tanggal | Status |
|---|---|---|---|
| 0. Fondasi & Style Lock | H1–H3 | 16–18 Nov | [~] 0A selesai (setup + Core, 153 EditMode test lulus). 0B dan 0C belum |
| 1. Prolog + Malam 1 | H4–H6 | 19–21 Nov | [ ] Belum mulai |
| 2. Malam 2 | H7–H8 | 22–23 Nov | [ ] Belum mulai |
| 3. Malam 3 | H9–H10 | 24–25 Nov | [ ] Belum mulai |
| 4. Malam 4 | H11–H12 | 26–27 Nov | [ ] Belum mulai |
| 5. Malam 5 | H13–H14 | 28–29 Nov | [ ] Belum mulai |
| 6. Malam 6 | H15–H16 | 30 Nov–1 Des | [ ] Belum mulai |
| 7. Malam 7 + Ending | H17–H19 | 2–4 Des | [ ] Belum mulai |
| 8. Polish & Rilis | H20–H21 | 5–6 Des | [ ] Belum mulai |

Isi setiap fase: lihat Bagian 6 dokumen desain.

Kriteria selesai setiap fase: semua EditMode test lulus, tim sudah mengecek daftar Play Mode fase itu, dan progres dicatat di log di bawah.

## Log Progres

- **2026-11-15:** Brainstorming selesai. Dokumen desain ditulis, repo git dibuat. Berikutnya: rencana implementasi Fase 0.
- **2026-11-15:** Ditambahkan keputusan platform: target final hanya Windows, kerja dan tes harian di Mac, build Windows dari Mac lalu dites di laptop Windows pada Fase 8.
- **2026-11-15:** Fase 0A selesai: Git LFS, paket (test-framework 1.8.0, ai.navigation 2.0.14, timeline 6.6.0, probuilder 6.1.2), assembly Core dan EditMode test, seluruh aturan Core. Berikutnya: rencana 0B (fondasi Runtime).
