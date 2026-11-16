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
- **Gaya Retro PSX / low-fi**, wajib konsisten di semua malam. Nilai gaya ada di `docs/style-guide.md` (dipasang oleh menu The Sending > Pasang Gaya PSX) dan dikunci setelah cek Play Mode 0C.
- **Git + GitHub.** Serialisasi Force Text sudah aktif. Git LFS aktif (`.gitattributes`). Setiap anggota tim wajib menjalankan `git lfs install` sekali sebelum clone/pull.
- **Aturan ending final, jam gim, obat, ritual, sidang:** lihat Bagian 4 dokumen desain.

## Platform dan Build

- **Target build final hanya Windows** (Standalone, scripting backend Mono).
- Pekerjaan dilakukan di **Mac**. Pengetesan harian lewat **Play Mode di Mac**.
- **Build Mac hanya untuk tes internal**, tidak didistribusikan.
- **Build Windows dibuat dari Mac** dengan modul **Windows Build Support (Mono)**. Per 15 Nov modul ini **belum terpasang** (hanya Mac dan WebGL), jadi dipasang lewat Unity Hub di awal Fase 0.
- Build Windows **wajib dites di laptop Windows anggota tim pada Fase 8**, terutama tampilan PSX, kabut, cahaya, resolusi, dan save. Daftar ceknya ada di Bagian 7.1 dokumen desain.
- Mac memakai Metal, Windows memakai DirectX, jadi tampilan bisa berbeda. Shader buatan sendiri dibuat dengan Shader Graph supaya lintas platform.
- Build uji Windows dari Mac: menu The Sending > Build Windows (uji, Mono), hasil di `Builds/Windows/` (development build).

## Konvensi Kode

- `Assets/_Project/Scripts/Core`: C# murni, aturan gim, tanpa MonoBehaviour. Assembly `TheSending.Core`.
- `Assets/_Project/Scripts/Runtime`: MonoBehaviour. Assembly `TheSending.Runtime` (mereferensikan Core).
- `Assets/_Project/Tests/EditMode`: assembly `TheSending.Tests.EditMode` (mereferensikan Core).
- Nama domain memakai bahasa Indonesia sesuai GDD (`KondisiIbu`, `PapanBukti`, `MalamKe`). Istilah teknis bahasa Inggris.
- Logika baru di Core ditulis dengan TDD: test dulu, lalu kode.
- Hanya satu singleton: `GameManager`.
- Raycast interaksi (E) dan kamera HP (Q) memakai QueryTriggerInteraction.Ignore: objek Interaktif dan PetunjukFoto wajib memakai collider biasa (bukan trigger).
- **Jeda = `Time.timeScale` 0.** `GameManager.Jeda(true/false)` adalah penghitung dan mengatur `Time.timeScale`. Setiap panel yang menjeda wajib melepasnya tepat sekali, termasuk di `OnDisable`. UI yang bergerak saat jeda memakai `Time.unscaledDeltaTime`. Cutscene Timeline yang berjalan saat jeda memakai `PlayableDirector` dengan Update Method **Unscaled Game Time**.
- `GameManager.StateDiganti` berarti GameState diganti (Mulai Baru, Lanjutkan, Tertangkap). `Perubahan.Malam` hanya saat malam benar-benar berganti; konten pembuka malam berlangganan ke sini.
- Domain reload saat Play dimatikan. Field `static` baru di Runtime wajib dibersihkan sendiri (`OnDestroy`, atau `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`).
- Kursor diatur `GameManager` (`KunciKursor`, diisi `PemainGerak`). Skrip lain tidak menyentuh `Cursor`.
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
| 0. Fondasi & Style Lock | H1–H3 | 16–18 Nov | [~] 0A, 0B, 0C selesai di kode (223 EditMode test lulus, build uji Windows berhasil). Menunggu cek Play Mode 0C oleh tim, lalu style guide dikunci |
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

## Catatan untuk Fase 0B (dari review akhir 0A)

- Setelah `KondisiIbu.SetelahTertangkap`, Runtime wajib menyimpan ulang checkpoint, supaya penalti tidak hilang saat keluar gim (desain 4.2).
- `SaveData.DariJson` mengembalikan `GameState` baru, jadi `GameManager` harus berlangganan ulang `Berubah` setiap kali memuat save.
- `PergantianMalam.Lanjut` bisa memicu `Perubahan.Malam` lalu `Perubahan.Ending` (karena -12). Cek `s.Ending` sebelum memuat malam berikutnya.
- Malam 6: Runtime menandai tujuan `SidangWarga` hanya setelah `SidangWarga.Selesaikan`/`Diam`/`IkutMenuduh` dipanggil.
- Menahan napas hanya meredam suara saat diam (sedang bersembunyi), sesuai Game Story 6.6.
- Puzzle paku: cek `Selesai` sebelum memberi penalti -5, karena `Pilih` juga mengembalikan false setelah selesai.
- Malam 7: jika bahan ritual belum lengkap, adegan Bu Sumi memanggil `RitualPenangkal.Pelajari(s, dariBuSumi: true)` sebelum ritual terakhir.
- Balancing: tanpa menetralkan media sama sekali, Kondisi Ibu habis sekitar pergantian Malam 6 ke 7. Tutorial ritual Malam 3 praktis wajib.
- Pilihan terakhir dan lolos dari Mbah Wiryo wajib lewat `PenentuEnding.Pilih` dan `PenentuEnding.LolosDariWiryo`.

## Catatan untuk Fase 1 dan sesudahnya (dari review 0B dan keputusan 0C)

Sudah diputuskan di 0C (aturannya ada di "Konvensi Kode"): jeda = `Time.timeScale` 0; event `StateDiganti`; kursor milik `GameManager`; field static membersihkan diri. Setiap scene punya objek `GameManager` (duplikat menghancurkan diri) supaya bisa di-Play langsung. `AktifDiMalam` dipasang di objek yang selalu aktif. Jumlah entri Catatan Bapak (7) dan Catatan Raka (6) sudah cocok dengan Game Story 9.1.

Fase 1, saat konten pertama memakainya:
- Ganti `GameManager.SceneAwal` dari `Scene_Sandbox0B` ke scene Prolog. "Lanjutkan" harus memuat scene sesuai `State.MalamKe`, bukan selalu `SceneAwal`. Daftarkan scene baru di `PembuatMenuUtama.DaftarkanScene`.
- Inventaris (tombol I) belum dibuat; aksinya sudah ada di `KontrolPemain.Inventaris`. Panelnya menjeda lewat `Jeda` seperti Jurnal.
- Scene baru: skrip pembuatnya memanggil `PembuatGayaPsx.TerapkanKeScene()` dan `SiapkanKamera()`, dan memasang `MenuJeda` + `PanelPengaturan`.
- `PanelDialog.Mulai` belum menjaga `GameManager.Instance` null; cutscene tanpa GameManager akan error.
- Notifikasi HUD belum menyaring pesan kembar berurutan (memotret ulang, `Perubahan.Katanya` ganda).
- Lebih dari 4 pilihan dialog disembunyikan diam-diam; beri `LogWarning`.
- Jalankan `KartuTeks.Tampilkan`/`KeHitam`/`DariHitam` lewat `kartu.StartCoroutine(...)`, bukan dari MonoBehaviour lain. Fade hitam belum dijaga token, jadi jangan tumpuk dua fade.
- Tambahkan EditMode test yang menjalankan semua kode efek di setiap aset `DataDialog` terhadap `GameState` sementara, supaya kode efek yang salah ketahuan sebelum Play Mode.

Fase 7: hentikan gerak dan input pemain setelah `State.Ending` terisi. Layar ending tidak punya pemain, jadi kursor otomatis terlihat.

Fase 8: pengaturan grafis baru "Layar penuh" saja; tambahkan pemilih resolusi jika tes Windows membutuhkannya. `PembuatBuild.Windows` membuat development build (tombol uji F5/F9 hidup); build rilis harus non-development.

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

## Daftar Cek Play Mode Fase 0C (dikerjakan tim)

Prasyarat: Shader Graph `PSX_Layar`, material, dan `MixerUtama` sudah dibuat, font sudah terpasang (langkah di `docs/superpowers/plans/2026-11-16-fase-0c-gaya-dan-menu.md`, bagian "Persiapan Tim"). Buka `Assets/_Project/Scenes/Scene_MainMenu.unity`, tekan Play.

1. Peringatan konten tampil lebih dulu dan kursor terlihat. Enter atau klik "Lanjut" membuka menu. Tombol bisa dipilih dengan mouse dan dengan panah/WASD + Enter; tombol terpilih berwarna karat.
2. Jika belum ada save, tombol "Lanjutkan" tidak ada.
3. Pengaturan: ubah volume, sensitivitas, Balik sumbu Y. Stop lalu Play lagi: nilainya masih sama. Esc menutup pengaturan.
4. "Mulai Baru" memuat sandbox. Kursor terkunci, kartu teks tampil, lalu pemain bisa bergerak.
5. Tidak ada peringatan mixer atau parameter di Console. Tampilan PSX: piksel besar kotak-kotak (bukan buram), pola dither terlihat di gradasi kabut dan tepi cahaya senter, warna agak pudar, sudut layar gelap, ada bintik film. Dinding jauh tertelan kabut. Pola dither harus seukuran piksel besar, bukan piksel layar. Jika dither halus seukuran piksel layar, fitur `PSX_Layar` (injection point 600) berjalan setelah upscale URP: ganti ke `BeforeRenderingPostProcessing` di `PembuatGayaPsx.AturRenderer`, jalankan The Sending > Pasang Gaya PSX, dan catat di `docs/style-guide.md`.
6. Esc membuka menu jeda: jam berhenti, jarum kompas (C) berhenti berputar, kursor terlihat. "Lanjut" atau Esc menutupnya dan kursor terkunci lagi. Esc saat dialog atau Jurnal terbuka tidak membuka menu jeda.
7. Dari menu jeda buka Pengaturan, naikkan sensitivitas, kembali: putaran mouse terasa lebih cepat. Balik sumbu Y membalik dongak.
8. Shift kanan juga berlari.
9. Regresi 0B saat dunia berhenti: buka dialog di Meja segera setelah memotret Foto keluarga; notifikasi tetap hilang sendiri sekitar 3 detik dan kilatan kamera tetap memudar.
10. Save: tekan F5 (notifikasi "Checkpoint disimpan.", Kondisi Ibu 60 di kiri atas). Esc > "Keluar ke Menu Utama" (peringatan konten tidak tampil lagi). "Lanjutkan" kini ada; klik: Kondisi Ibu 60. Tekan F9: Kondisi Ibu 55. Stop Play. Play lagi dari `Scene_MainMenu`, "Lanjutkan": Kondisi Ibu tetap 55 (penalti tersimpan).
11. "Mulai Baru" saat ada save memunculkan konfirmasi. "Batal" kembali ke menu. "Ya, mulai baru": Kondisi Ibu 60; keluar ke menu lalu "Lanjutkan": tetap 60 (save lama tertimpa).
12. Tekstur: taruh satu gambar besar (misalnya 2048 px) di `Assets/_Project/Art/Textures/`. Di Inspector: Max Size 256, Filter Mode Point, Generate Mipmap mati.
13. Font (VT323 dan Kalam sudah terpasang): teks UI memakai VT323 dan tab Buku Harian (Tab > 2) memakai Kalam. Tidak ada peringatan font di Console.
14. Catat nilai gaya yang ingin diubah (kuat dither, grain, vignette, kabut, ukuran teks). Setelah disetel, status `docs/style-guide.md` diganti dari DRAF menjadi TERKUNCI dan Fase 0 ditandai selesai.

## Log Progres

- **2026-11-15:** Brainstorming selesai. Dokumen desain ditulis, repo git dibuat. Berikutnya: rencana implementasi Fase 0.
- **2026-11-15:** Ditambahkan keputusan platform: target final hanya Windows, kerja dan tes harian di Mac, build Windows dari Mac lalu dites di laptop Windows pada Fase 8.
- **2026-11-15:** Fase 0A selesai: Git LFS, paket (test-framework 1.8.0, ai.navigation 2.0.14, timeline 6.6.0, probuilder 6.1.2), assembly Core dan EditMode test, seluruh aturan Core. Berikutnya: rencana 0B (fondasi Runtime).
- **2026-11-16:** Fase 0B selesai di kode: GameManager, save/checkpoint ke file, kontrol pemain, interaksi, HUD, senter/kompas/kamera HP, dialog, Jurnal, kartu teks, AktifDiMalam. Scene uji: `Assets/_Project/Scenes/Scene_Sandbox0B.unity` (dibuat ulang lewat menu The Sending > Buat Scene Sandbox 0B). Menunggu cek Play Mode tim. Review akhir 0B: kegagalan tulis/baca save tidak lagi melempar exception ke gameplay.
- **2026-11-16:** Tim mengecek `Scene_Sandbox0B` di Play Mode, tidak ada error. Fase 0B selesai. Berikutnya: rencana 0C.
- **2026-11-16:** Fase 0C selesai di kode: jeda memakai `Time.timeScale`, event `StateDiganti`, pengaturan (volume, sensitivitas, balik Y, layar penuh) tersimpan di PlayerPrefs, `Scene_MainMenu` dengan peringatan konten, menu jeda, gaya PSX (render scale 0.4, point, dither 5 bit, Volume bersama, kabut, prasetel tekstur), `docs/style-guide.md` (draf), build uji Windows dari Mac berhasil. Shader full-screen memakai Shader Graph + Custom Function (`PSX_Layar.shadergraph`, dibuat tim). Inventaris pindah ke Fase 1. Menunggu cek Play Mode 0C oleh tim.
