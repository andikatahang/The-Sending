# The Sending

Gim horor puzzle 3D first person tentang santet di desa Jawa. Unity 6000.6.4f1 (URP 17.6), keyboard dan mouse. Target build final **hanya Windows**. Dikerjakan oleh tim mahasiswa di Mac.

- Isi cerita, tokoh, angka: `docs/TheSending_FullGameStory.md` (Game Story) dan `docs/TheSending_GDD.md` (GDD). Baris terakhir GDD adalah gambar base64 sekitar 730 KB, jadi baca GDD sampai baris 620 saja.
- Desain pengembangan (arsitektur, aturan yang diperjelas, cutscene, jadwal): `docs/superpowers/specs/2026-11-15-the-sending-design.md`. Untuk aturan di Bagian 4 dokumen itu, desain ini mengalahkan Game Story dan GDD.
- **Memulai fase baru (siapa pun yang melanjutkan):** baca `docs/panduan-rencana-fase.md` (alur fase, bentuk rencana, aturan kerja dengan agent AI; tidak butuh skill khusus) dan `docs/roadmap-fase-4-8.md` (isi dan sumber setiap fase berikutnya). Rencana fase yang sudah ada di `docs/superpowers/plans/`.

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
- **Target minimal (diputuskan 18 Nov):** minimal **2 ending** (**Fitnah** dan **Sejati**) bisa diselesaikan dalam **2 minggu**. Konten penuh tetap tujuan akhir, tetapi urutan pengerjaan mendahulukan jalur ke 2 ending itu.
- **Suara (voice acting) belakangan.** Semua percakapan cukup teks di panel dialog sampai diputuskan lain.
- **Konten penuh sesuai GDD**, tidak dipadatkan. Jika satu fase melewati jatah harinya, laporkan ke tim, jangan memotong konten sendiri.
- **Gaya Retro PSX / low-fi**, wajib konsisten di semua malam. Nilai gaya ada di `docs/style-guide.md` (dipasang oleh menu The Sending > Pasang Gaya PSX) dan dikunci setelah cek Play Mode 0C.
- **Git + GitHub.** Serialisasi Force Text sudah aktif. Git LFS aktif (`.gitattributes`). Setiap anggota tim wajib menjalankan `git lfs install` sekali sebelum clone/pull.
- **Aturan ending final, jam gim, obat, ritual, sidang:** lihat Bagian 4 dokumen desain.

## Platform dan Build

- **Target build final hanya Windows** (Standalone, scripting backend Mono).
- Pekerjaan dilakukan di **Mac**. Pengetesan harian lewat **Play Mode di Mac**.
- **Build Mac hanya untuk tes internal**, tidak didistribusikan.
- **Build Windows dibuat dari Mac** dengan modul **Windows Build Support (Mono)**. Modul sudah terpasang; build Windows pertama dari Mac berhasil pada 16 Nov.
- Build Windows **wajib dites di laptop Windows anggota tim pada Fase 8**, terutama tampilan PSX, kabut, cahaya, resolusi, dan save. Daftar ceknya ada di Bagian 7.1 dokumen desain.
- Mac memakai Metal, Windows memakai DirectX, jadi tampilan bisa berbeda. Shader buatan sendiri dibuat dengan Shader Graph supaya lintas platform.
- Build uji Windows dari Mac: menu The Sending > Build Windows (uji, Mono), hasil di `Builds/Windows/` (development build).

## Konvensi Kode

- `Assets/_Project/Scripts/Core`: C# murni, aturan gim, tanpa MonoBehaviour. Assembly `TheSending.Core`.
- `Assets/_Project/Scripts/Runtime`: MonoBehaviour. Assembly `TheSending.Runtime` (mereferensikan Core).
- `Assets/_Project/Tests/EditMode`: assembly `TheSending.Tests.EditMode` (mereferensikan Core). Referensi ke Runtime hanya untuk test data (`DataDialogTests` memeriksa semua aset `DataDialog`); aturan gim tetap diuji lewat Core.
- Nama domain memakai bahasa Indonesia sesuai GDD (`KondisiIbu`, `PapanBukti`, `MalamKe`). Istilah teknis bahasa Inggris.
- Logika baru di Core ditulis dengan TDD: test dulu, lalu kode.
- Hanya satu singleton: `GameManager`.
- Raycast interaksi (E) dan kamera HP (Q) memakai QueryTriggerInteraction.Ignore: objek Interaktif dan PetunjukFoto wajib memakai collider biasa (bukan trigger).
- **Jeda = `Time.timeScale` 0.** `GameManager.Jeda(true/false)` adalah penghitung dan mengatur `Time.timeScale`. Setiap panel yang menjeda wajib melepasnya tepat sekali, termasuk di `OnDisable`. UI yang bergerak saat jeda memakai `Time.unscaledDeltaTime`. Cutscene Timeline yang berjalan saat jeda memakai `PlayableDirector` dengan Update Method **Unscaled Game Time**.
- `GameManager.StateDiganti` berarti GameState diganti (Mulai Baru, Lanjutkan, Tertangkap). `Perubahan.Malam` hanya saat malam benar-benar berganti; konten pembuka malam berlangganan ke sini.
- Domain reload saat Play dimatikan. Field `static` baru di Runtime wajib dibersihkan sendiri (`OnDestroy`, atau `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`).
- Kursor diatur `GameManager` (`KunciKursor`, diisi `PemainGerak`). Skrip lain tidak menyentuh `Cursor`.
- **Adegan dan cutscene:** `KameraAdegan.Mulai()` menjeda dan menyalakan `GameManager.Sinematik`. Cutscene memakai Timeline yang dibuat skrip (`PembuatTimeline`: track gerak + `SignalEmitter`), diputar sutradara lewat `PemutarTimeline.PutarDanTunggu()`; director Unscaled Game Time, wrap None. Sinyal diteruskan sebagai nama `SignalAsset` ke sutradara (konstanta `Sinyal...`). Jangan menyunting aset di `Assets/_Project/Timeline/` dengan tangan. Jangan memakai `WaitForSeconds` di adegan. Jam gim hanya maju saat pemain dikendalikan.
- **Suara tokoh ditunda:** semua percakapan teks saja (panel dialog, kartu). Grup mixer `Dialog` disiapkan untuk voice acting nanti.
- **Satu sutradara per bagian cerita** (`AlurProlog`, `AlurPrologRumah`, `AlurMalam1`, `AlurMalam2`, `AlurMalam3`, `AlurDesaMalam3`, `AlurRumahBuSumi`): hanya mengurutkan kejadian dan memanggil kelas aturan Core. Scene awal ditentukan `PetaScene.Untuk(state)`.
- **Malam 3:** pintu depan rumah keluarga memuat `Scene_Desa` (sutradara `AlurDesaMalam3`); pintu rumah Bu Sumi di desa memuat `Scene_RumahBuSumi` (indeks build 4), yang kembali ke desa. Titik muncul di desa dari `PetaScene.DiDepanRumahBuSumi`; tidak ada checkpoint di desa atau di rumah Bu Sumi, jadi `PetaScene.Untuk` tidak pernah memilih keduanya di Malam 3. Puzzle paku di `TujuhPaku` (Core, kemajuan di `GameState.PakuDicabut`); salah cabut menyimpan checkpoint lalu memunculkan Sosok Malam 2 (K21). Ritual Penangkal dengan benda (`RitualDapur` + `BendaRitual` di anglo, garam, kelor, korek, bambu; urutan dicek `RitualPenangkal`), langkah keenam `KuburAbu` di pekarangan belakang (pintu belakang dapur terbuka mulai Malam 3). Guncangan pengganti animasi tokoh: `Guncang.Posisi`.
- **Sosok Santet:** otaknya `PerilakuSantet` (Core, diuji); di scene `SosokSantet` + `NavMeshAgent` (mati sampai objeknya dinyalakan). NavMesh dibangun saat runtime oleh `LantaiNavMesh` (setelan agen radius 0,25 m, trigger dan pemain diabaikan); jangan bake NavMesh ke scene dan jangan ubah setelan agen di ProjectSettings. Bersembunyi lewat `TempatSembunyi` (`PemainGerak.Sembunyi`, `Pindah` mengakhirinya). Sosok mengabaikan langkah jalan (hanya lari, 8 m; jalan hanya dalam 1,5 m) dan melihat senter dalam 6 m.
- **Aksara Jawa** tidak ditulis dengan `Text`/`TextMesh` (tidak bisa menyusun sandhangan). Pakai gambar dari `tools/render-aksara.sh`.
- **Raycast dan pemain:** pemain di layer Ignore Raycast dan raycast memakai `Physics.DefaultRaycastLayers`, supaya kapsul pemain tidak menghalangi foto (Q) dan interaksi (E).
- **Suara langkah** per permukaan lewat `LangkahKaki` dan `Permukaan`. Paket Footsteps dan Horror SFX ada di `Audio/ThirdParty`.
- **Scene dibuat skrip, jadi suntingan tangan di scene hilang saat `BangunSemua`.** Perubahan permanen harus masuk skrip pembuat (`PembuatMalam2`, `PembuatMalam3`, `PembuatRumahKeluarga`, `PembuatDesa`, `PembuatRumahBuSumi`, dan lain-lain).
- **Scene gameplay dibuat skrip** (`PembuatDesa`, `PembuatRumahKeluarga`, bagian bersama di `PembuatRig`) dan ditimpa setiap The Sending > Bangun Ulang Semua. Jangan menyunting scene itu dengan tangan. Batchmode: `-executeMethod TheSending.EditorTools.PembuatMenuUtama.BangunSemua`.
- **Teks dialog** ada di skrip pembuat data (`PembuatDataFase1`), bukan disunting di Inspector. Pembuat menolak teks kosong, penanda `[[...]]`, dan kode efek yang salah.
- **Aset pengganti** di `Art/Materials/Pengganti/`, `Prefabs/Pengganti/`, `Audio/SFX/`, `Audio/Music/`. Skrip tidak menimpa file yang ada. Benda di scene = objek logika (collider + skrip) dengan prefab visual sebagai anak.
- Setelah setiap run batchmode: `git diff --quiet ProjectSettings/ProjectSettings.asset || git checkout -- ProjectSettings/ProjectSettings.asset` (batchmode menghapus define `SENTIS_ANALYTICS_ENABLED`; penghapusan itu tidak di-commit).
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
| 1. Prolog + Malam 1 | H4–H6 | 19–21 Nov | [x] Selesai (269 EditMode test lulus, build uji Windows berhasil, cek Play Mode tim lulus 18 Nov). Hutan, rumah limasan, tekstur, dan perabot dari paket aset di `Art/ThirdParty` (dibuat `PembuatAsetAsli`); mobil Prolog dari Sketchfab via glTFast. Suara tokoh ditunda |
| 2. Malam 2 | H7–H8 | 22–23 Nov | [x] Selesai (306 EditMode test lulus, cek Play Mode tim 18 Nov "overall oke", build uji Windows berhasil). Gambar aksara dirender dari Noto Sans Javanese (`tools/render-aksara.sh`). Sosok Santet dari `pocong-scary`; tokoh lain dan sebagian suara masih pengganti; suara tokoh ditunda |
| 3. Malam 3 | H9–H10 | 24–25 Nov | [~] Selesai di kode (338 EditMode test lulus, build uji Windows berhasil). Menunggu cek Play Mode Fase 3 oleh tim. Pakde, Bu Sumi, dan rumah Bu Sumi masih pengganti; sofa dari `old-sofas`; suara tokoh ditunda |
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

## Catatan untuk fase berikutnya (dari review 0B, keputusan 0C, dan Fase 1–3)

Semua catatan "Fase 3" sudah dikerjakan di Fase 3: `AlurMalam2.GantiMalam` memuat Malam 3, `PetaScene.RumahBuSumi` dan `DiDepanRumahBuSumi` (dengan test), boneka (M1) dinetralkan di anglo (diambil otomatis dari kotak kayu), tang dan Halaman 4 di gudang, Sosok Malam 2 dipakai ulang untuk paku salah urutan, sofa ruang tengah.

Fase 4:
- `AlurMalam3.GantiMalam` berakhir di kartu "bersambung" lalu kembali ke menu; ganti dengan `gm.MuatScene(gm.SceneUntukState)` dan sutradara Malam 4. `AlurMalam3.Start` menampilkan notifikasi sementara jika scene dimuat pada Malam 4 ke atas; hapus saat Malam 4 ada. Pintu depan Malam 3 (`PintuDepanMalam3`) hanya aktif di Malam 3.
- Pekarangan belakang sudah ada (`PembuatMalam3.Pekarangan`, x −7..3, z 12..19, pagar tak terlihat); sumur belakang dan kejaran Malam 4 ditaruh di sini. Halaman 5 di balik kalender dapur: kalender per malam diatur `AktifDiMalam` (Malam 1, 2, 3 masing-masing satu objek).
- Pos ronda di `Scene_Desa`: desa sudah punya sutradara malam untuk Malam 3 (`AlurDesaMalam3`, titik muncul dari `PetaScene`); Malam 4 memakai pola yang sama (sutradara baru atau perluasan). Batas jalan malam belum ada (R12). Aksi Kepercayaan `BelaDiPosRonda` (+1) lewat efek `kepercayaan:BelaDiPosRonda`.
- `AlurRumahBuSumi` hanya mengisi Malam 3; Malam 4 ke atas baru notifikasi. Malam 4: pintu tetap tertutup jika dituduh di Malam 3 (`KepercayaanSumi.PintuTerbuka`). Malam 5: kesaksian B5 dan Halaman 7 (B7) untuk Kepercayaan ≥ 2.
- `RitualDapur` dipakai lagi untuk M3–M6 (anglo memuat media berikutnya menurut nomor, `RitualPenangkal.MediaBerikutnya`; media di atas anglo belum tampak).
- `AlurMalam3.LamaSosok` (40 detik) dan `KewaspadaanSantet.JedaKemunculanDetik` masih nilai awal / belum dipakai.
- Ditunda: lampu teplok yang bisa dibawa, model HP di tangan, Catatan Bapak/Raka selain lima entri yang terisi, animasi tokoh, suara tokoh.
- Sekuens kejar pertama memakai `PerilakuSantet` dalam keadaan Kejar; checkpoint wajib sebelum sekuens kejar (desain 2.6). Pandangan dari dalam lemari masih tembus (R4 Fase 2).

Fase 5: `indonesia-grave` (K16) untuk `Scene_Makam`.

Fase 7: hentikan gerak dan input pemain setelah `State.Ending` terisi. Layar ending tidak punya pemain, jadi kursor otomatis terlihat. `AlurMalam1.GantiMalam`, `AlurMalam2.KeMenuJikaEnding`, dan `AlurMalam2.Tertangkap` kembali ke menu utama jika ending tercapai; ganti dengan layar ending. `the-bathroom` (ganti tekstur bermerek nyata) dan `keris` (K16).

Fase 8: `UnityConnectSettings` m_Enabled 1 dan define `SENTIS_ANALYTICS_ENABLED` (ditambah editor sendiri) menyalakan layanan/analytics Unity; pastikan dimatikan untuk build rilis jika tidak dipakai. Pengaturan grafis baru "Layar penuh" saja; tambahkan pemilih resolusi jika tes Windows membutuhkannya. `PembuatBuild.Windows` membuat development build (tombol uji F5/F6/F9 hidup); build rilis harus non-development. Aset pengganti yang masih tersisa ditukar atau dicatat di layar kredit. Lisensi `pocong-scary`, mobil LMPV, Footsteps, dan Horror SFX diisi di KREDIT. Model Sosok sekitar 30.000 segitiga: cek kinerja di laptop Windows.

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
5. Tidak ada peringatan mixer atau parameter di Console.
6. Tepi objek terlihat seperti piksel besar yang tajam, bukan buram.
7. Color banding/dither terlihat di gradasi kabut dan tepi cahaya senter.
8. Warna dan kepadatan kabut sesuai gaya; dinding jauh tertelan kabut.
9. Grain dan vignette terlihat halus (tidak mengganggu).
10. Highlight tidak kusam atau abu-abu. Jika kusam, catat untuk tim: coba Tonemapping None atau matikan HDR.
11. Esc membuka menu jeda: jam berhenti, jarum kompas (C) berhenti berputar, kursor terlihat. "Lanjut" atau Esc menutupnya dan kursor terkunci lagi. Esc saat dialog atau Jurnal terbuka tidak membuka menu jeda.
12. Dari menu jeda buka Pengaturan, naikkan sensitivitas, kembali: putaran mouse terasa lebih cepat. Balik sumbu Y membalik dongak.
13. Shift kanan juga berlari.
14. Regresi 0B saat dunia berhenti: buka dialog di Meja segera setelah memotret Foto keluarga; notifikasi tetap hilang sendiri sekitar 3 detik dan kilatan kamera tetap memudar.
15. Save: tekan F5 (notifikasi "Checkpoint disimpan.", Kondisi Ibu 60 di kiri atas). Esc > "Keluar ke Menu Utama" (peringatan konten tidak tampil lagi). "Lanjutkan" kini ada; klik: Kondisi Ibu 60. Tekan F9: Kondisi Ibu 55. Stop Play. Play lagi dari `Scene_MainMenu`, "Lanjutkan": Kondisi Ibu tetap 55 (penalti tersimpan).
16. "Mulai Baru" saat ada save memunculkan konfirmasi. "Batal" kembali ke menu. "Ya, mulai baru": Kondisi Ibu 60; keluar ke menu lalu "Lanjutkan": tetap 60 (save lama tertimpa).
17. Tekstur: taruh satu gambar besar (misalnya 2048 px) di `Assets/_Project/Art/Textures/`. Di Inspector: Max Size 256, Filter Mode Point, Generate Mipmap mati.
18. Font (VT323 dan Kalam sudah terpasang): teks UI memakai VT323 dan tab Buku Harian (Tab > 2) memakai Kalam. Tidak ada peringatan font di Console.
19. Catat nilai gaya yang ingin diubah (kuat dither, grain, vignette, kabut, ukuran teks). Setelah disetel, status `docs/style-guide.md` diganti dari DRAF menjadi TERKUNCI dan Fase 0 ditandai selesai.

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
26. Setelah foto: dialog penutup 2 baris. Jika lemari belum dibuka, muncul pengingat lemari dan malam baru berganti setelah lemari dibuka. Jika lemari sudah dibuka: layar hitam, kartu "Malam 2: Boneka di Bawah Lantai" sekitar 8 detik, lalu kembali ke menu utama. (Sejak Fase 2: kartu Malam 2 ditampilkan di rumah, lalu Malam 2 dimulai.)

**Save**

27. "Mulai Baru" saat sudah ada save memunculkan konfirmasi. "Batal" kembali ke menu. "Ya, mulai baru": Prolog mulai lagi dari gapura dengan Kondisi Ibu 60 dan inventaris kosong.
28. "Lanjutkan" memuat scene yang benar: keluar ke menu saat masih di jalan desa lalu "Lanjutkan" = jalan desa dari awal Prolog; setelah masuk rumah = rumah keluarga dengan sambutan Laras; setelah kartu Malam 1 = di samping ranjang pada Malam 1 dengan inventaris yang sama dan tanpa Bidan Wulan; setelah kartu Malam 2 = rumah keluarga dengan notifikasi "Malam 2 belum dibuat (Fase 2)." dan Kondisi Ibu 12 lebih rendah daripada di akhir Malam 1. (Sejak Fase 2: kartu Malam 2 ditampilkan di rumah, lalu Malam 2 dimulai.)
29. Di Malam 1 tekan F9: Kondisi Ibu turun 5 dan scene dimuat ulang dari checkpoint. Stop Play, Play lagi, "Lanjutkan": penalti tetap. Lalu setelah cutscene 23.00 tekan F5, keluar ke menu, "Lanjutkan": rumah langsung dalam keadaan sesudah cutscene (lampu listrik padam, teplok menyala, paku di lantai, Laras di dekat ranjang) dan cutscene tidak terulang.

**Gaya, suara, dan catatan**

30. Gaya PSX sama di kedua scene dan di kedua kamera (kamera pemain dan kamera adegan): piksel besar yang tajam, dither, kabut. Tidak ada bidikan adegan yang tampak lebih halus daripada tampilan bermain.
31. Esc membuka menu jeda di kedua scene. Mengubah volume di Pengaturan memengaruhi bunyi adegan dan musik latar. Tidak ada error merah di Console selama seluruh alur dari butir 1 sampai 26.
32. Waktu tunggu (K4): main tanpa F6 dan catat berapa menit nyata dari tiba di rumah sampai pukul 21.00, dari memberi obat sampai pukul 23.00, dan berapa menit di antaranya tidak ada yang bisa dikerjakan.
33. Denah (K7): nilai ukuran ruangan, lebar pintu, letak perabot, dan panjang jalan desa. Catat yang perlu digeser; perubahannya dikerjakan di `PembuatDesa.cs` dan `PembuatRumahKeluarga.cs`.
34. Teks (K5): baca semua teks yang ditulis pengembang (sambutan Laras, kronologi, adegan Ibu di Prolog, teguran obat, kalender, Halaman 1, dua Catatan Raka, notifikasi tutorial) dan catat suntingannya. Dua Catatan Raka berisi informasi kesehatan dasar: periksa isinya.
35. Catat aset pengganti yang paling mengganggu (model atau suara) supaya dicari lebih dulu.

Hasil cek 2026-11-18: secara keseluruhan benar ("overall correct"). Dua temuan sudah diperbaiki (`459b5dc` paku dipindah ke genangan darah dan target foto dilebarkan; `12f4c6c` kursi tunggu untuk K4 opsi B) dan perlu dicek ulang di Play Mode: butir 19/25 (paku di atas genangan, Q memberi [FOTO]) dan kursi di kamar Ibu melompat ke 21.00/23.00 (tidak melewati pemberian obat). Juga `ba5c411`: bawaan gaya PSX diselaraskan ke 8 bit / dither 1.5 sesuai style guide.

## Daftar Cek Play Mode Fase 2 (dikerjakan tim)

Prasyarat: Task 12 selesai dan editor dibuka sesudahnya. Tombol uji (editor dan development build): **F5** simpan checkpoint, **F6** jam gim +30 menit, **F9** tertangkap. Bunyi yang filenya belum ada tampil sebagai teks "(Suara: ...)" di HUD; itu dihitung lolos. Percakapan teks saja. Cara cepat ke Malam 2: mainkan Malam 1 sampai selesai, atau "Lanjutkan" dari save yang dibuat setelah kartu Malam 2. Catat nomor butir yang gagal beserta apa yang terlihat.

**Masuk Malam 2**

1. Akhir Malam 1 (setelah foto paku, dialog penutup, lemari sudah dibuka): layar hitam, rumah dimuat ulang (lampu listrik menyala, paku, darah, dan teplok tidak ada), kartu "Malam 2: Boneka di Bawah Lantai" sekitar 8 detik dengan "Malam Kamis Kliwon", ringkasan siang, dan tujuan. Gerimis terdengar.
2. Raka berdiri di kamar Ibu menghadap ranjang. Dua notifikasi: kamar berbau anyir, dan petunjuk kompas (C). Kondisi Ibu (teks uji kiri atas) 12 lebih rendah daripada akhir Malam 1.
3. Kompas (C): jarum berputar makin kencang saat mendekat ke sisi timur ranjang Ibu, diam jika jauh (lebih dari 12 m).
4. Laras di ruang tengah: dialog 3 baris, lalu "Barang baru masuk inventaris". Inventaris (I) berisi "Kunci gudang". Kalender di dapur: "Rabu, Malam Kamis Kliwon." Wajah Ibu sedikit lebih pucat daripada di Malam 1 (K14).

**Gudang, celah, dan linggis**

5. Pintu gudang tanpa kunci: pesan terkunci. Dengan kunci: prompt "Buka pintu gudang", E membuka (bunyi kunci dan pintu), pintu hilang, ada petunjuk senter.
6. Masuk gudang pertama kali: adegan sekitar 4 detik, kamera mendekat ke celah di dinding kanan (timur), sosok tinggi gelap lewat di luar celah dari kiri ke kanan, stinger terdengar saat sosok di depan celah, lalu kontrol kembali. Tidak terulang saat keluar-masuk gudang lagi.
7. Linggis (bersandar di dinding gudang): "Ambil linggis", linggis hilang dari scene, inventaris bertambah. Isi gudang: rak alat tani dan tumpukan karung.

**Papan, boneka, dan aksara**

8. Berjalan di atas papan dekat ranjang Ibu: bunyi kopong dan notifikasi satu kali. E sebelum punya linggis: pesan butuh alat.
9. E dengan linggis ("Bongkar papan lantai dengan linggis"): papan hilang, lubang gelap dan boneka terlihat, notifikasi foto otomatis dan media, panel aksara terbuka. Jurnal > Papan Bukti menampilkan `[MEDIA] M1` tanpa menyebut bacaannya.
10. Panel aksara: sembilan kotak dalam dua baris (3 dan 6), kotak pertama ditandai warna karat, tabel Halaman 2 di kanan. Aksara tampil sebagai aksara Jawa (bukan kotak kosong); pada SE dan WO tanda taling di kiri aksara, pada KLI pasangan la di bawah ka. (Jika gambar belum dibuat: kotak berisi `[RA]` dan seterusnya, tabel berupa teks; dihitung lolos dengan catatan.)
11. Tombol 1–4 memilih bacaan. Salah: pesan "Bukan itu", tanda tidak pindah. Benar: bacaan tampil di bawah kotak, tanda pindah ke suku berikutnya. Esc menutup panel tanpa membuka menu jeda; E di boneka membuka lagi dengan kemajuan yang sama. Jam berhenti selama panel terbuka.
12. Setelah RA-T-MI dan SE-LA-SA-KLI-WO-N: panel tertutup, boneka hilang dari lubang, dialog 4 baris (Laras: "Selasa Kliwon... itu weton Ibu, Mas."), notifikasi hitung mundur, dan HUD kanan atas menampilkan "Selasa Kliwon: 5 malam lagi". Jika belum pukul 21.00, ada petunjuk obat dan kursi.

**Obat dan Mbah Wiryo**

13. Kursi di kamar Ibu setelah aksara terbaca dan sebelum 21.00: prompt "Duduk menunggu sampai 21.00", kartu jam, lalu pengingat obat. Beri obat: Kondisi Ibu +2.
14. Segera setelah obat (atau pukul 22.00 tanpa obat, setelah teguran Laras): ketukan pintu, layar hitam, kamera membidik Mbah Wiryo di dekat pintu depan, dialog dengan dua pilihan (tombol 1 dan 2). Kutipan "Barang begini jangan disimpan di rumah, Le. Biar Mbah yang larung ke kali, supaya ibumu tenang." tampil persis.
15. Pilihan "Serahkan": Kondisi Ibu turun 8, notifikasi Mbah Wiryo membawa boneka; kotak kayu di kamar Raka kosong. Jurnal: M1 bertanda "(diserahkan)". Uji terpisah, pilihan "Simpan": Kondisi Ibu tidak berubah, boneka tampil di atas kotak kayu kamar Raka.
16. Setelah pilihan: kamera ke Laras, "Ingat ya, Nduk." dengan terjemahan, Laras bergetar sekitar 2 detik, layar hitam, Mbah Wiryo dan Laras hilang.

**Sekuens bersembunyi**

17. Kartu "Malam Kamis Kliwon" (Laras masuk kamar, gemerincing paku), Raka di kamar Laras menghadap pintu. Musik tegang dan gemerincing paku terdengar; gemerincing makin keras saat Sosok dekat (suara 3D). Tiga notifikasi tutorial.
18. Sosok Santet (sekitar 2,2 m) keluar dari dapur dan berpatroli berkeliling ruang tengah tanpa tersangkut di saka, meja, atau pintu.
19. Lemari pakaian di dinding barat ruang tengah: E "Bersembunyi di lemari" memindahkan pandangan ke dalam lemari; W/A/S/D tidak menggerakkan, mouse masih memutar pandangan; prompt "[E] Keluar [Spasi] Tahan napas"; detak jantung terdengar. E lagi keluar ke depan lemari (tidak langsung masuk lagi).
20. Di dalam lemari tanpa menahan napas, saat Sosok lewat paling dekat: Sosok berbelok ke lemari dan Raka tertangkap. Dengan Spasi ditahan saat Sosok lewat: aman. Bar "Napas" mengecil saat menahan dan pulih setelah dilepas; setelah sekitar 8 detik napas lepas sendiri.
21. Model Sosok menghadap arah jalannya (K9-A; jika terbalik, lihat Task 9 Step 2) dan tidak tampak gepeng atau melayang.
22. Berjalan dalam sekitar 4 m dari Sosok: ia berbalik mendatangi. Berlari: ketahuan dari sekitar 8 m. Diam sambil menahan napas di tempat terbuka: tidak ketahuan walau Sosok lewat dekat.
23. Senter (F) diarahkan ke Sosok dari kejauhan (sampai 12 m) dengan jalur pandang bebas: Sosok mendatangi. Senter ke Sosok di balik dinding: tidak bereaksi.
24. Tertangkap: stinger, kartu "Sosok itu menemukan Raka.", layar hitam, rumah dimuat ulang dengan Raka di kamar Laras (bukan di awal Malam 2), Kondisi Ibu 5 lebih rendah (K12-A). Pilihan Mbah Wiryo tetap.
25. Kolong ranjang Ibu dan balik karung gudang (K13-A): masing-masing bisa dimasuki dan dikeluari dengan E; pandangan rendah (kolong sekitar 25 cm, karung sekitar 90 cm); Sosok tidak menemukan Raka yang menahan napas di sana.
26. Sampai kamar Raka tanpa tertangkap: Sosok hilang, musik berhenti, notifikasi gemerincing menjauh, sekitar 3 detik kemudian layar hitam, kartu "Malam 3: Tujuh Paku di Saka" dengan "Malam Jumat Legi" dan "(Bersambung...)", lalu menu utama. (Sejak Fase 3: kartu Malam 3 ditampilkan di rumah, lalu Malam 3 dimulai.)
27. Catat apakah sekuens terasa terlalu mudah atau terlalu sulit (berapa kali tertangkap, berapa lama), dan apakah Sosok pernah macet atau menembus dinding.

**Save, gaya, dan catatan**

28. "Lanjutkan" dari menu setelah butir 26: rumah keluarga dengan notifikasi "Malam 3 belum dibuat (Fase 3).", HUD "Selasa Kliwon: 4 malam lagi", Kondisi Ibu 12 lebih rendah. Tekan F5 di tengah Malam 2 setelah mengambil linggis, keluar, "Lanjutkan": pintu gudang sudah terbuka, linggis tidak ada di gudang, kartu Malam 2 tampil lagi. (Sejak Fase 3: kartu Malam 3 ditampilkan di rumah, lalu Malam 3 dimulai.)
29. Gaya PSX sama di kamera pemain dan kamera adegan (celah gudang, Mbah Wiryo). Kabut dan gelap gudang membuat senter perlu. Tidak ada error merah di Console selama butir 1–26.
30. Esc membuka menu jeda saat bersembunyi; Sosok berhenti selama jeda dan melanjutkan sesudahnya.
31. Teks (K15): baca semua teks yang ditulis pengembang (dialog Laras, baris Raka saat membaca, Mbah Wiryo selain kutipan, kartu, tiga Catatan Bapak di Jurnal > Catatan, isi Media M1) dan catat suntingannya. Periksa isi budaya Catatan Bapak.
32. Catat aset pengganti yang paling mengganggu (model atau suara) supaya dicari lebih dulu.

**Jika butir 18 gagal (Sosok tidak bergerak, error "tidak berada di NavMesh"):** periksa Console untuk "NavMesh gagal dibangun". Jalan keluar: perbesar `LantaiNavMesh.ukuran` atau turunkan `agentClimb`; jika pintu tidak bisa dilewati, perkecil `RadiusAgen` ke 0,2. **Jika Sosok berjalan menembus lemari atau saka:** collider benda itu tidak terkumpul (prefab Asli tanpa collider); laporkan. **Jika Sosok macet di titik patroli:** naikkan `SosokSantet.JarakTiba`. **Jika butir 20 terlalu sulit atau mudah:** setel `PerilakuSantet.JarakTangkap`, `KewaspadaanSantet.RadiusNapas`, atau geser titik "Patroli 5" di `PembuatMalam2.Sembunyi`, lalu bangun ulang (test Core mungkin perlu ikut disesuaikan; laporkan dulu). **Jika butir 6 beku atau sosok di celah tidak bergerak:** sama dengan jalan keluar Timeline Fase 1 (Update Method, wrap mode, `removeStartOffset`). **Jika butir 10 menampilkan kotak kosong:** font tidak termuat saat render; jalankan ulang `bash tools/render-aksara.sh` dan periksa nama file font.

Hasil cek 2026-11-18: secara keseluruhan oke. Butir 10 dihitung lolos dengan catatan (teks pengganti `[RA]`, gambar aksara menunggu font). Perbaikan sesudah cek ada di `git log 6106050..HEAD`.

## Daftar Cek Play Mode Fase 3 (dikerjakan tim)

Prasyarat: Task 12 selesai dan editor dibuka sesudahnya. Tombol uji (editor dan development build): **F5** simpan checkpoint, **F6** jam gim +30 menit, **F9** tertangkap. Bunyi yang filenya belum ada tampil sebagai teks "(Suara: ...)" di HUD; itu dihitung lolos. Percakapan teks saja. Cara cepat ke Malam 3: mainkan Malam 2 sampai selesai, atau "Lanjutkan" dari save yang dibuat setelah kartu Malam 3. Untuk menguji cabang berbeda (minta maaf, menuduh, salah cabut), tekan F5 di rumah keluarga tepat sebelum percabangan lalu "Lanjutkan" (F5 di desa atau rumah Bu Sumi juga dimuat di rumah keluarga). Catat nomor butir yang gagal beserta apa yang terlihat.

**Masuk Malam 3 dan Pakde**

1. Akhir Malam 2 (sampai kamar Raka tanpa tertangkap): layar hitam, rumah dimuat ulang, kartu "Malam 3: Tujuh Paku di Saka" sekitar 8 detik dengan "Malam Jumat Legi", ringkasan siang, dan tujuan. Tidak ada lagi kartu "(Bersambung...)" Malam 3. HUD "Selasa Kliwon: 4 malam lagi"; Kondisi Ibu (teks uji kiri atas) 12 lebih rendah daripada akhir Malam 2.
2. Motor berhenti (bunyi atau teks), ketukan, layar hitam, lalu kamera membidik Pakde Darmo di samping sofa ruang tengah; buah tangan ada di meja. Dialog 7 baris; kutipan "Kamu itu masih anak kecil, Le. Nggak ngerti keadaan. Tanah itu dijual, ibumu bisa berobat, kalian bisa hidup enak." tampil persis. Muncul notifikasi Papan Bukti diperbarui.
3. Setelah dialog: layar hitam, Pakde hilang, motor menjauh, Raka berdiri di kamarnya, dua notifikasi. Jurnal > Papan Bukti menampilkan `[KATANYA] K4` (pembeli tanah). Buah tangan tetap di meja.
4. Sofa di dinding selatan ruang tengah: dudukan menghadap ke tengah ruang, tidak melayang atau tenggelam, tidak menghalangi pintu kamar Raka atau pintu depan. (Jika menghadap dinding: Task 9 Step 2.)

**Halaman 3, jeritan, dan kompas**

5. Kotak sepatu di kamar Raka: "Buka kotak sepatu lama", dialog 2 baris, notifikasi halaman ditemukan dan bukti baru (B1). Jurnal > Buku Harian: Halaman 3 berisi teks. Jurnal > Papan Bukti: B1.
6. Segera setelah dialog kotak sepatu: kamera membidik ranjang Ibu, Ibu berguncang sekitar 2,5 detik, dentang jam dan jeritan (atau teksnya), lalu dua notifikasi (jeritan dan kompas). Terjadi sekali saja.
7. Kompas (C) berputar paling kencang di dekat saka guru timur laut ruang tengah (dekat pintu kamar Laras), lebih pelan di saka lain.
8. Setiap jam penuh (F6 mempercepat) sampai paku ketujuh tercabut: dentang, jeritan, dan notifikasi "Jam berdentang...". Tidak terjadi sebelum kotak sepatu dibuka, dan tidak lagi setelah paku tercabut semua.

**Saka, tang, Halaman 4, dan paku**

9. Ketuk saka (E, "Ketuk saka guru"): tiga saka berbunyi padat dengan notifikasi; saka timur laut berbunyi berongga, dempul hitam hilang, tujuh paku muncul, notifikasi media dan petunjuk ke gudang. Saka tidak bisa diketuk lagi setelah itu.
10. Kalender di dapur: "Kamis. Malam ini malam Jumat Legi."
11. Gudang terbuka sejak awal Malam 3 (tanpa kunci, pintunya tidak ada). Tang di rak alat: "Ambil tang", tang hilang, inventaris (I) bertambah "Tang". Halaman di rak: dialog 2 baris; Jurnal > Buku Harian Halaman 4 berisi aturan mencabut paku dan enam langkah ritual dengan tempat bahannya.
12. Paku tanpa tang: prompt "Paku berukir ...", E memunculkan "Butuh tang." Dengan tang: "Cabut paku berukir ...". Ukiran dari atas ke bawah: Selasa, Jumat, Minggu, Kamis, Sabtu, Rabu, Senin. Paku bisa dibidik satu per satu dari depan saka.
13. Urutan benar Jumat, Sabtu, Minggu, Senin, Selasa, Rabu, Kamis: setiap paku hilang dengan bunyi cabut; setelah paku ketujuh muncul "Jeritan Ibu berhenti" dan petunjuk Bu Sumi. Pintu depan kini berprompt "Pergi ke rumah Bu Sumi".
14. Uji terpisah, salah urutan: paku tidak lepas, Kondisi Ibu turun 5, stinger, Sosok Santet keluar dari dapur, musik tegang, notifikasi bersembunyi. Paku yang sudah dicabut tetap hilang.
15. Selama Sosok ada: lemari pakaian ruang tengah bisa dipakai bersembunyi, detak jantung terdengar, menahan napas bekerja seperti Malam 2. Sekitar 40 detik kemudian Sosok hilang dan muncul "Gemerincing paku itu menjauh". Salah cabut lagi saat Sosok masih ada: Kondisi Ibu −5 lagi, Sosok tidak muncul dua kali.
16. Tertangkap (atau F9 saat Sosok ada): kartu "Sosok itu menemukan Raka.", rumah dimuat ulang dengan Raka di ruang tengah (tanpa kartu Malam 3 panjang dan tanpa Pakde), paku yang sudah dicabut tetap tercabut, Kondisi Ibu 5 lebih rendah daripada saat salah cabut. Stop Play, Play, "Lanjutkan": keadaan sama.
17. Pintu depan sebelum paku ketujuh: "Raka tidak bisa pergi selama Ibu terus menjerit." Saat Sosok masih ada: "Gemerincing paku masih terdengar...".

**Jalan desa malam (K17-B)**

18. E di pintu depan: bunyi pintu, layar hitam, `Scene_Desa` pada malam hari: Raka berdiri di depan emper rumah keluarga menghadap selatan, kabut tebal, lampu jalan menyala, notifikasi arah ke rumah Bu Sumi. Tidak ada mobil, kartu Prolog, azan, atau tutorial senter. Jam terus berjalan.
19. Pintu rumah keluarga di desa sebelum berkunjung: "Raka belum menemui Bu Sumi." Berjalan ke selatan sekitar 25 m: rumah Bu Sumi di kanan jalan, jendelanya menyala. Pintunya berprompt "Ketuk pintu Bu Sumi".
20. Ketuk: bunyi ketukan, layar hitam, `Scene_RumahBuSumi`: Raka di dalam ambang pintu dapur, Bu Sumi berdiri di depannya, dapur gelap (teplok mati), cahaya redup dari pintu belakang. Gaya PSX dan kabut sama.

**Rumah Bu Sumi**

21. Pintu keluar sebelum bicara: "Raka belum bicara dengan Bu Sumi." Teplok dan sumur sebelum bicara: "Raka belum bicara dengan Bu Sumi."
22. Bicara: kalimat Jawa dengan terjemahan miring dalam kurung, lalu tiga pilihan (tombol 1–3).
23. Pilihan 1 (minta maaf): Bu Sumi melunak dan menyebut teplok dan gentong, lalu layar hitam, Raka dan Bu Sumi di bale-bale, dialog cerita 8 baris dengan kutipan "Aku ora tau nyantet sapa-sapa. Aku mung tau nangis." dan "Wong sing teka tanpa diundang, ojo dikei apa-apa." persis, lalu notifikasi bahan.
24. Teplok di meja: "Perbaiki lampu teplok", kartu sekitar 2,5 detik, dapur menjadi terang hangat. Lewat pintu belakang ke halaman: sumur "Timba air untuk Bu Sumi", kartu, bunyi timba. Setelah keduanya: "Beri tahu Bu Sumi." Bicara: terima kasih. Bicara lagi: pamit.
25. Uji terpisah, pilihan 3 (menawarkan bantuan) tanpa minta maaf: teplok dan sumur, lapor ke Bu Sumi, terima kasih, lalu langsung adegan cerita di bale-bale (Kepercayaan 1).
26. Pintu keluar setelah bicara tetapi sebelum membantu: peringatan sekali, E lagi untuk keluar. Keluar: layar hitam, desa, Raka di depan pintu rumah Bu Sumi menghadap jalan, notifikasi arah pulang. Ketuk lagi: "Sudah larut. Bu Sumi tidak membuka pintunya lagi malam ini."
27. Uji terpisah, pilihan 2 (menuduh): Bu Sumi mengusir Raka; bunyi pintu dibanting, layar hitam, desa di depan pintu Bu Sumi, notifikasi pintu dibanting dan Halaman 4.
28. Pintu rumah keluarga di desa setelah berkunjung: layar hitam, rumah keluarga, Raka berdiri di dalam pintu depan, notifikasi keadaan bahan. Pintu depan lagi: "Sudah larut. Raka tidak keluar lagi malam ini." "Lanjutkan" dari menu setelah keluar di tengah jalan desa: rumah keluarga dari checkpoint terakhir (kunjungan hilang jika checkpoint dibuat sebelum berangkat).

**Ritual Penangkal (K25-B)**

29. Jalur tanpa bahan dari Bu Sumi (pilihan 2): pintu belakang dapur kini terbuka (di Malam 1–2 tertutup). Rak bumbu di meja dapur "Ambil garam", pohon kelor di pekarangan belakang "Petik daun kelor", bambu kuning di gudang "Ambil bilah bambu kuning". Sebelum Halaman 4 dibaca, ketiganya tidak berprompt. Pekarangan berpagar, tidak bisa keluar.
30. Anglo di dapur sebelum bahan lengkap: E memberi notifikasi bahan belum lengkap; meja kecil di sampingnya kosong. Setelah bahan lengkap: toples garam, ikat daun kelor, korek api di meja, bilah bambu kuning bersandar. Prompt anglo "Letakkan boneka kain di anglo" jika boneka disimpan di Malam 2, selain itu "Letakkan tujuh paku di anglo".
31. Urutan benar: anglo, garam, kelor, korek (api anglo menyala, cahaya jingga dan bunyi api), bambu: setiap langkah memberi notifikasi; setelah bambu muncul petunjuk kubur abu, dan anglo serta benda-benda itu tidak berprompt lagi. Salah langkah (misalnya korek sebelum garam): "Ritual gagal...", api padam, mulai lagi dari anglo. E pada garam sebelum anglo: "Belum ada media di atas anglo." Jam tetap berjalan selama ritual.
32. Pekarangan: tanah "Kubur abu di sini": notifikasi media dinetralkan, Kondisi Ibu +8, Jurnal media itu "(dinetralkan)", api anglo padam. Jika boneka yang dinetralkan, boneka di kotak kayu kamar Raka hilang, dan anglo kini menawarkan paku. Ritual kedua memakai bahan yang sama.

**Obat dan penutup**

33. Obat 21.00 seperti malam sebelumnya. Jika semua tujuan selesai sebelum 21.00: notifikasi jam obat dan kursi; kursi di kamar Ibu "Duduk menunggu sampai 21.00"; malam belum ditutup sebelum obat diberikan atau pukul 22.00 lewat. Jika jendela obat lewat saat Raka di desa atau rumah Bu Sumi, teguran Laras muncul setelah pulang.
34. Setelah obat (atau 22.00): layar hitam, kartu "Malam 4: Rambut di Dasar Sumur" dengan "Malam Sabtu Pahing" dan "(Bersambung...)", lalu menu utama. "Lanjutkan": rumah keluarga dengan notifikasi "Malam 4 belum dibuat (Fase 4).", HUD "3 malam lagi", Kondisi Ibu 12 lebih rendah.

**Regresi, gaya, dan catatan**

35. Prolog dan Malam 1–2 tetap berjalan: Prolog di desa sama seperti sebelumnya (mobil, azan, siluet Bu Sumi, pintu rumah "Masuk rumah"), pintu Bu Sumi di desa tidak bereaksi di Prolog; pintu belakang tertutup; Pakde, paku, benda ritual, dan pekarangan tidak muncul atau tidak bereaksi; Sosok Malam 2 berpatroli tanpa macet di sofa.
36. Tidak ada error merah di Console selama butir 1–34, di ketiga scene.
37. Teks (K24): baca semua teks yang ditulis pengembang (dialog Pakde selain kutipan, semua kalimat Jawa Bu Sumi dan terjemahannya, cerita tiga tahun lalu, kotak sepatu, Halaman 3, Halaman 4, Catatan Bapak "Rumah limasan dan saka guru" dan "Daun kelor dan bambu kuning", isi Media M2, K4, B1, notifikasi ritual dan jalan desa) dan catat suntingannya. Minta anggota tim yang fasih memeriksa bahasa Jawa.
38. Catat: apakah urutan ukiran paku terlalu mudah atau sulit, apakah 40 detik Sosok terasa pas, berapa menit nyata dari awal Malam 3 sampai kartu Malam 4 (termasuk jalan desa), apakah benda ritual mudah dibidik, dan aset pengganti yang paling mengganggu (Pakde, Bu Sumi, rumah Bu Sumi, anglo).

**Jika butir 12 gagal (prompt paku tidak muncul, yang muncul tidak ada atau "Ketuk saka guru"):** collider `KetukSaka` menutupi paku; geser paku menjauh dari saka dengan memperkecil `x - 0.02f` di `PembuatMalam3.SakaDanPaku` (misalnya `x - 0.05f`) lalu bangun ulang. **Jika butir 14 gagal (Sosok tidak muncul, error "tidak berada di NavMesh"):** titik muncul (−0,5; 10,5) di dapur terhalang anglo atau meja; geser `TitikMunculSosokMalam3` di `PembuatMalam3.Pasang`. **Jika butir 4 gagal:** arah sofa (Task 9 Step 2) atau ukuran di `Prefab("Sofa", ...)`. **Jika butir 18 atau 20 gagal (scene tidak dimuat, error "Scene ... couldn't be loaded"):** scene belum terdaftar; jalankan ulang "Bangun Ulang Semua" dan periksa `EditorBuildSettings.asset`. **Jika butir 18 menaruh Raka di dalam emper atau melayang:** geser `TitikDepanRumahMalam3` di `PembuatDesa.Malam3`. **Jika butir 19 gagal (pintu Bu Sumi tidak berprompt):** collider `PintuBuSumi` tertutup dinding depan `RumahKecil`; geser x dari 7,3 ke 7,1. **Jika butir 29 gagal (tidak bisa lewat pintu belakang):** periksa bahwa `PintuBelakang` mati di Malam 3 (`AktifDiMalam` 0–2 di objek Sutradara). **Jika butir 30–31 sulit dibidik:** perbesar ukuran collider di daftar `benda` di `PembuatMalam3.Ritual`.

Hasil cek: belum dicek (menunggu tim sebelum merge PR Fase 3).

## Log Progres

- **2026-11-15:** Brainstorming selesai. Dokumen desain ditulis, repo git dibuat. Berikutnya: rencana implementasi Fase 0.
- **2026-11-15:** Ditambahkan keputusan platform: target final hanya Windows, kerja dan tes harian di Mac, build Windows dari Mac lalu dites di laptop Windows pada Fase 8.
- **2026-11-15:** Fase 0A selesai: Git LFS, paket (test-framework 1.8.0, ai.navigation 2.0.14, timeline 6.6.0, probuilder 6.1.2), assembly Core dan EditMode test, seluruh aturan Core. Berikutnya: rencana 0B (fondasi Runtime).
- **2026-11-16:** Fase 0B selesai di kode: GameManager, save/checkpoint ke file, kontrol pemain, interaksi, HUD, senter/kompas/kamera HP, dialog, Jurnal, kartu teks, AktifDiMalam. Scene uji: `Assets/_Project/Scenes/Scene_Sandbox0B.unity` (dibuat ulang lewat menu The Sending > Buat Scene Sandbox 0B). Menunggu cek Play Mode tim. Review akhir 0B: kegagalan tulis/baca save tidak lagi melempar exception ke gameplay.
- **2026-11-16:** Tim mengecek `Scene_Sandbox0B` di Play Mode, tidak ada error. Fase 0B selesai. Berikutnya: rencana 0C.
- **2026-11-16:** Fase 0C selesai di kode: jeda memakai `Time.timeScale`, event `StateDiganti`, pengaturan (volume, sensitivitas, balik Y, layar penuh) tersimpan di PlayerPrefs, `Scene_MainMenu` dengan peringatan konten, menu jeda, gaya PSX (render scale 0.4, point, kuantisasi warna + dither, Volume bersama, kabut, prasetel tekstur), `docs/style-guide.md` (draf), build uji Windows dari Mac berhasil. Shader full-screen memakai Shader Graph + Custom Function (`PSX_Layar.shadergraph`, dibuat tim). Inventaris pindah ke Fase 1. Menunggu cek Play Mode 0C oleh tim.
- **2026-11-17:** Graph `PSX_Layar` dibetulkan (Hasil tersambung ke Base Color; sebelumnya layar abu-abu rata) dan dither dibuat tahan terhadap posisi ternormalisasi. Tim memilih BitWarna 8 dan KuatDither 1.5 setelah cek di Play Mode.
- **2026-11-18:** Fase 1 selesai di kode: Prolog dan Malam 1 bisa dimainkan dari "Mulai Baru" sampai kartu Malam 2. Core: inventaris, `GembokLemari.Buka`, foto petunjuk di Papan Bukti, jendela obat, `PetaScene`, `PemeriksaDialog`, `JamGim.KejadianBerikutnya/Tunggu` (269 EditMode test). Runtime: panel inventaris dan gembok, alat adegan, obat Ibu, ukiran blandar, kursi tunggu, tiga sutradara, tiga cutscene Timeline yang dibuat skrip. Percakapan teks saja (suara tokoh ditunda). `Scene_Desa` dan `Scene_RumahKeluarga` dibuat skrip; semua model dan lima suara masih aset pengganti. Build uji Windows berhasil. Waktu tunggu (K4) dan denah (K7): belum dicatat tim. Berikutnya: cek ulang dua perbaikan di Play Mode, lalu rencana Fase 2 (Malam 2).
- **2026-11-18:** Ojek Prolog diganti mobil LMPV (Sketchfab, glTFast). Paket aset (Essential Nature Pack, TraditionalHouseModular, Yughues Concrete, nappin House Interior) dipindah ke `Art/ThirdParty`, dikonversi ke URP + prasetel PSX; hutan desa, rumah gaya limasan, tekstur per meter, dan perabot dipasang lewat skrip pembuat (`PembuatAsetAsli`, prefab di `Prefabs/Asli`). Tim mengecek Play Mode: lulus. Fase 1 selesai. Berikutnya: rencana Fase 2 (Malam 2).
- **2026-11-18:** Fase 2 selesai kecuali Task 10: Malam 2 bisa dimainkan dari akhir Malam 1 sampai kartu Malam 3. Core: barang Malam 2, `BacaanAksara`, hitung mundur Selasa Kliwon, `PerilakuSantet`, kedatangan Mbah Wiryo (306 EditMode test). Runtime: `TempatSembunyi` dan napas, `SosokSantet` dengan NavMesh runtime, `PanelAksara`, `PapanLantai`, `AmbilBarang`, `BonekaDiKotak`, `WajahIbu`, `AlurMalam2`. Timeline celah gudang. Tim mengecek Play Mode 18 Nov: overall oke. Perbaikan sesudah cek: mobil tetap di jalan (pivot glb), raycast mengabaikan kapsul pemain (foto paku), azan hanya `DetikAzan` dengan teks "sudah magrib", suara baru (mobil, klik senter, kilat kamera, batuk, ambience malam, langkah per permukaan, Horror SFX di slot stinger/detak/jam), Sosok mengabaikan jalan (lari 8 m, jalan 1,5 m, senter 6 m), perbaikan tertangkap saat lari, paket Footsteps dan Horror SFX dipindah ke `Audio/ThirdParty`. Build uji Windows berhasil.
- **2026-11-18:** Task 10 selesai: font Noto Sans Javanese dipasang tim, `tools/render-aksara.sh` merender 9 suku dan tabel Halaman 2 ke `Art/UI/Aksara`, `PanelAksara` memakai gambar (bukan `[RA]`). 306 EditMode test lulus. Fase 2 selesai; cek ulang butir 10 daftar Fase 2 di Play Mode. Berikutnya: rencana Fase 3 (Malam 3).
- **2026-11-19:** Fase 3 selesai di kode: Malam 3 bisa dimainkan dari akhir Malam 2 sampai kartu Malam 4. Core: `TujuhPaku` (saka dan tujuh paku), tang, tujuan `PakdeDatang`/`RitualPertama`, dentang tiap jam, penantian obat Malam 3, `PetaScene.RumahBuSumi`/`DiDepanRumahBuSumi`, `RitualPenangkal.MediaBerikutnya` (338 EditMode test). Runtime: `KetukSaka`, `PakuDiSaka`, `AmbilBahan`, `RitualDapur`, `BendaRitual`, `KuburAbu`, `Guncang`, `AlurMalam3`, `AlurDesaMalam3`, `AlurRumahBuSumi`. Jalan desa malam di `Scene_Desa`, `Scene_RumahBuSumi` baru; pintu belakang dan pekarangan rumah keluarga. Keputusan K17–K25 (18 Nov): K17-B, K25-B, sisanya A. Build uji Windows berhasil. Menunggu cek Play Mode Fase 3 (setelan Sosok dan paku, teks K24) sebelum merge. Berikutnya: rencana Fase 4 (Malam 4).
