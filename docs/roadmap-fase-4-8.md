# Roadmap Fase 4–8: The Sending

**Ditulis:** 19 November 2026, di cabang `fase-3` (Fase 3 sudah di kode, menunggu cek Play Mode tim).
**Untuk siapa:** agen AI (tanpa skill khusus) yang nanti menulis rencana rinci setiap fase, dan tim yang membacanya.

Dokumen ini hanya peta. Isi cerita, dialog, dan angka tetap diambil dari sumbernya. Setiap butir menyebut sumber dengan nomor baris supaya bisa dibuka langsung.

## Cara Memakai Roadmap Ini

Sebelum menulis rencana Fase N, baca `docs/panduan-rencana-fase.md` (cara menulis rencana fase: format task, daftar cek Play Mode, penomoran K, langkah commit). Lalu baca bagian Fase N di sini, buka semua sumber yang disebut, dan periksa kode yang ada (nama kelas di sini benar per 19 Nov; cek lagi dengan `ls Assets/_Project/Scripts/Core Assets/_Project/Scripts/Runtime Assets/_Project/Scripts/Editor`). Rencana Fase 3 (`docs/superpowers/plans/2026-11-18-fase-3-malam-3.md`) adalah contoh terdekat untuk bentuk rencana, pola sutradara, dan pola pembuat scene.

"Calon K" di bawah adalah keputusan yang kelihatannya perlu ditanyakan ke tim. Penomoran final dibuat di rencana fase, meneruskan nomor terakhir yang sudah dipakai (K25 di Fase 3). Jika rencana Fase 4 memakai K26–K30, rencana Fase 5 mulai dari K31, dan seterusnya. Jelaskan tradeoff tiap pilihan di chat dulu, baru tanyakan ke tim.

Daftar "Lapis A / Lapis B" di setiap fase membagi isi menjadi:
- **Lapis A:** jalan menuju target minimal (ending **Fitnah** dan **Sejati**). Dikerjakan lebih dulu.
- **Lapis B:** konten penuh GDD yang tidak memutus jalur kedua ending itu. Tetap dikerjakan (konten tidak dipotong), hanya urutannya belakangan. Jika fase molor, laporkan ke tim; jangan memotong sendiri.

### Sumber dan urutan kekuatannya

| Singkatan | Berkas | Catatan |
|---|---|---|
| **CL** | `CLAUDE.md` | Aturan tim, konvensi kode, catatan per fase, log |
| **DS** | `docs/superpowers/specs/2026-11-15-the-sending-design.md` | Bagian 4 (baris 149–215) **mengalahkan** GS dan GDD. Bagian 5 cutscene (217–250), 6 fase (252–271), 7.1 tes Windows (282–297), 9 risiko (309–322) |
| **GS** | `docs/TheSending_FullGameStory.md` | Isi cerita. Malam 4 (529–570), Malam 5 (571–617), Malam 6 (618–663), Malam 7 (664–712), Ending (713–767), pesan penutup 9.2 (786–788), scene 10.1 (792–803), variabel 10.2 (805–821), halaman diary 10.3 (823–834), kepekaan konten 10.4 (836–847) |
| **GDD** | `docs/TheSending_GDD.md` | **Baca hanya sampai baris 620** (baris terakhir gambar base64 ~730 KB). Aturan (61–101), outcome (173–185), tingkat tantangan per malam (195–205), ringkasan per malam (567–575) |
| **AS4** | `docs/aset/fase-4.md` | Daftar aset Malam 4 (sudah ada) |

### Pertentangan sumber yang sudah terlihat

Semua di bawah diselesaikan dengan DS Bagian 4, dan Core sudah mengikutinya.

1. **Urutan cek ending.** GS 8.1 (715–724) dan GDD 183 mengecek Terlambat lebih dulu, lalu Fitnah. DS 4.1 (153–163) mengecek **Fitnah lebih dulu**: di jalur Fitnah Kondisi Ibu ditahan minimal 1 dan Ibu tetap hidup sampai 00.00 Malam 7. `PenentuEnding.Hitung` dan `KondisiIbu.MinJalurFitnah` sudah begitu.
2. **Kondisi Ibu 0.** GS 6.1 (246) dan GDD 93: Kondisi 0 kapan pun = Terlambat. DS 4.1: tidak berlaku setelah sidang gagal (lihat butir 1).
3. **Obat.** GS 6.7 (350–352): setiap malam 21.00. DS 4.4 (179–183): Malam 6 jendela 20.00–21.00, Malam 7 jalur utama Laras memberi obat otomatis (+2). `JamGim.MulaiJendelaObat` sudah menangani Malam 6; obat otomatis Malam 7 belum ada di kode.
4. **Balas Dendam dan waktu.** GS 8.4 tidak menyebut waktu. DS 4.1 butir 3: hitung mundur tetap berjalan sampai lolos dari Mbah Wiryo; jika waktu habis lebih dulu, Terlambat.
5. **Akhir Ending Terlambat.** GS 8.5 (766): pemain dikembalikan ke menu. GS 9.2 (786–788) dan GDD 597: setiap ending ditutup pesan penutup. Usul: kartu "Santet belum selesai.", lalu pesan penutup, lalu menu.
6. **Scene.** GS 10.1 menaruh pos ronda, sumur, pohon asem di `Scene_Desa`. Di kode, sumur belakang dan kejaran Malam 4 sudah diputuskan di pekarangan belakang `Scene_RumahKeluarga` (rencana Fase 3, K18-A dan R3, baris 19 dan 41). DS 2.7 (107–120) menambah kamar mandi bercermin di `Scene_RumahKeluarga` untuk epilog Balas Dendam.

## Ringkasan Semua Fase

| Fase | Hari | Tanggal | Isi singkat | Status (19 Nov) |
|---|---|---|---|---|
| 0. Fondasi & Style Lock | H1–H3 | 16–18 Nov | Core, kontrol, HP, Jurnal, save, gaya PSX, menu | Selesai di kode; style guide menunggu dikunci setelah cek 0C |
| 1. Prolog + Malam 1 | H4–H6 | 19–21 Nov | Jalan desa, rumah keluarga, obat, gembok, cutscene 23.00 | Selesai |
| 2. Malam 2 | H7–H8 | 22–23 Nov | Kompas, gudang, M1, aksara, Mbah Wiryo, Sosok Santet, bersembunyi | Selesai (termasuk Task 10 gambar aksara) |
| 3. Malam 3 | H9–H10 | 24–25 Nov | Pakde, tujuh paku, rumah Bu Sumi, Kepercayaan, Ritual Penangkal | Selesai di kode, **menunggu cek Play Mode tim** |
| 4. Malam 4 | H11–H12 | 26–27 Nov | Pos ronda, kerekan sumur, M3, M4, B2, kejaran pertama | Belum |
| 5. Malam 5 | H13–H14 | 28–29 Nov | Pohon asem (M5, M6), makam, B3–B7, rumah Pakde (B6) | Belum |
| 6. Malam 6 | H15–H16 | 30 Nov–1 Des | Kerasukan, persiapan sampai 21.00, Sidang Warga, rumah Bu Sumi terbakar | Belum |
| 7. Malam 7 + Ending | H17–H19 | 2–4 Des | Watuireng, lilin sesaji, M7, pilihan akhir, kejaran Wiryo, ritual terakhir, jalur Fitnah, 4 epilog | Belum |
| 8. Polish & Rilis | H20–H21 | 5–6 Des | Main penuh, balancing, bug, build Windows rilis dan tes di laptop Windows | Belum |

Sumber: CL tabel "Fase dan Progres", DS 6 (252–271).

### Target minimal dan jadwal

Keputusan 18 Nov (CL "Keputusan yang Sudah Disepakati"): minimal ending **Fitnah** dan **Sejati** bisa diselesaikan dalam **2 minggu** (sampai sekitar Minggu 29 Nov), dan urutan pengerjaan mendahulukan jalur ke dua ending itu. Jadwal asli menaruh Malam 6 dan 7 di 30 Nov–4 Des, jadi target ini hanya tercapai jika cadangan dari fase awal dipakai. Per 19 Nov, Fase 3 sudah di kode 5 hari lebih cepat dari jatahnya (24–25 Nov), jadi cadangan itu ada.

Usul urutan kerja (tim yang memutuskan): kerjakan **Lapis A Fase 4, 5, 6, 7 berurutan** sampai kedua ending bisa dimainkan dari awal, lalu kembali mengisi **Lapis B** tiap fase, lalu Fase 8. Setiap fase tetap ditutup dengan kriteria selesai DS 6 (EditMode test lulus, cek Play Mode tim, log di CL).

**Jalur minimal Fitnah:** Malam 4 wajib (bungkusan sumur, lolos kejaran) → Malam 5 wajib (gali kendi, bandingkan tanah, bicara Laras) → Malam 6 sidang gagal/diam/menuduh → Malam 7 jalur Fitnah (bertahan sampai 00.00) → epilog Fitnah → pesan penutup.

**Jalur minimal Sejati:** sama sampai Malam 5 → Malam 6 sidang berhasil (minimal 2 tuduhan dijawab benar) → Malam 7 jalur utama (Watuireng, lilin sesaji, M7, pilih Lapor, kembali, ritual terakhir sebelum 00.00) → epilog Sejati → pesan penutup.

**Bukti yang cukup untuk sidang berhasil tanpa konten opsional:** tuduhan 1 dengan B4 (kesaksian Laras, Malam 5 wajib `BicaraLaras`), tuduhan 3 dengan B3 (tanah merah dan Pak Lasno, Malam 5 wajib `BandingkanTanah`). Tuduhan 2 bisa dengan B1 jika Halaman 1 dan 3 ditemukan (`BukuHarian.Temukan`). Jadi B2, B5, B6, B7, M4, M6, dan rumah Pakde semuanya Lapis B. Pastikan dialog Laras yang memberi B4 dan perbandingan tanah yang memberi B3 tidak bisa terlewat di Malam 5.

**Kondisi Ibu di kedua jalur:** CL "Catatan untuk Fase 0B": tanpa menetralkan media, Kondisi Ibu habis sekitar pergantian Malam 6 ke 7; tutorial ritual Malam 3 praktis wajib. Maka menetralkan M3 dan M5 di anglo (`RitualDapur`, sudah ada) termasuk Lapis A, supaya Ibu masih hidup saat sidang. Balancing angka di Fase 8.

---

## Fase 4: Malam 4, Rambut di Dasar Sumur

**Jatah:** H11–H12, 26–27 Nov (2 hari). Bisa dimulai segera setelah Fase 3 lolos cek Play Mode.

### Cerita dan tujuan

Sumber: GS 529–570; GDD 197, 569; DS 5 (baris Malam 4).
- Malam Sabtu Pahing. Ringkasan siang: air sumur berbau busuk, kabar santet menyebar (GS 533).
- Raka meminjam tali di pos ronda; warga dan Pak Joko membicarakan Bu Sumi; K1 dan K2 masuk sebagai Katanya. Pilihan diam atau membela Bu Sumi (GS 541–543). Membela = `AksiSumi.BelaDiPosRonda` +1 (GS 6.4, baris 310).
- Puzzle kerekan: pasang tali, atur pengait, turunkan dan tarik pelan; terlalu cepat = bungkusan jatuh lagi (GS 559–561).
- M3 (bungkusan rambut uban dan kembang busuk) dari dasar sumur. Dua warga menuduh itu rambut Bu Sumi; K3 masuk (GS 545–547).
- Halaman 5 di balik kalender dapur (GS 549; GS 10.3 baris 829).
- Opsional: kompas di dapur, tangga dari gudang, M4 (telur cemani berisi jarum dan rambut Ibu) di plafon. Lalu kembali ke pos ronda untuk B2 (GS 551–553).
- Sekuens kejar di pekarangan: Sosok keluar dari arah sumur, memburu sampai ke dalam rumah (GS 555).
- Dialog kunci (GS 563–569) dikutip persis.

**Tujuan wajib di Core** (`TujuanMalam`, sudah ada): `AmbilBungkusanSumur`, `LolosKejaranSumur`. Ditambah obat 21.00–22.00 seperti malam lain.

### Scene

- `Scene_RumahKeluarga`: pekarangan belakang sudah ada (`PembuatMalam3.Pekarangan`, x −7..3, z 12..19). Sumur belakang dan kejaran ditaruh di sini (rencana Fase 3 R3 dan K18-A). Dapur: kalender Malam 4 dengan Halaman 5 (kalender per malam diatur `AktifDiMalam`), plafon dapur yang bisa dinaiki, tangga di gudang.
- `Scene_Desa`: pos ronda baru. Desa sudah punya sutradara malam (`AlurDesaMalam3`) dan titik muncul dari `PetaScene`; Malam 4 memakai pola yang sama. Batas jalan malam belum ada (rencana Fase 3 R12).
- `Scene_RumahBuSumi`: tidak ada isi Malam 4 di GS. Pintu tetap tertutup jika Raka menuduh di Malam 3 (`KepercayaanSumi.PintuTerbuka`).

### Core

Sudah ada: `TujuanMalam` (tujuan Malam 4), `KerekanSumur.TarikanAman` (batas 0,6 m/detik), `MediaSantet`, `PapanBukti` (Katanya dan pembantahnya, DS 4.7), `KepercayaanSumi.Tambah(BelaDiPosRonda)`, `PerilakuSantet` (keadaan Kejar), `KewaspadaanSantet`, `RitualPenangkal.MediaBerikutnya`, `BukuHarian`, `EfekDialog` (kode `katanya:`, `bukti:`, `media:`, `halaman:`, `kepercayaan:`, `tujuan:`).

Perlu dibuat (TDD, test dulu):
- Barang baru di `Inventaris`: tali tambang, pengait (dan tangga jika dibawa).
- Foto/penanda baru di `PapanBukti` jika ada petunjuk yang dipotret manual.
- Puzzle kerekan lengkap jika lebih dari batas kecepatan (misalnya urutan pasang tali → pengait → turun → tarik). Simpan sesederhana mungkin.
- Syarat B2: M4 sudah ditemukan sebelum bertanya di pos ronda (bisa cukup lewat dialog bersyarat; cek apakah `PemeriksaDialog`/`DataDialog` sudah mendukung syarat).
- `PetaScene`: titik muncul desa Malam 4 (pos ronda) dengan test.

### Runtime dan Editor

Ikuti pola Fase 2–3 (CL "Konvensi Kode"):
- Sutradara `AlurMalam4` (mengurutkan kejadian saja, memanggil Core). Bagian desa: perluas `AlurDesaMalam3` atau buat `AlurDesaMalam4` (calon K).
- Pembuat scene `PembuatMalam4` (pola `PembuatMalam2`/`PembuatMalam3`), dipanggil `BangunSemua`. Data dialog di `PembuatDataFase4` (pola `PembuatDataFase3`). Jangan menyunting scene dengan tangan.
- Komponen baru: sumur dan kerekan (panel atau kontrol tahan tombol, memakai `Time.unscaledDeltaTime` jika berjalan saat jeda), plafon dan tangga, pos ronda (warga + Pak Joko sebagai `PemicuDialog`).
- Pakai ulang: `SosokSantet` + `LantaiNavMesh` + `TempatSembunyi` untuk kejaran, `RitualDapur` dan `KuburAbu` untuk menetralkan M3/M4, `KameraAdegan`, `PemutarTimeline`, `KartuTeks`, `JadwalObat`, `KursiTunggu`.
- Checkpoint wajib **sebelum** sekuens kejar (DS 2.6, baris 103–105; GDD 95). Tertangkap: muat checkpoint lalu −5 (DS 4.2).

### Cutscene (DS 5)

- Dua warga menuduh rambut milik Bu Sumi: adegan berskrip.
- Sosok keluar dari sumur, kejaran dimulai: cutscene penuh pendek (Timeline dibuat `PembuatTimeline`, Unscaled Game Time, wrap None, tanpa `WaitForSeconds`).

### Aset

`docs/aset/fase-4.md` (AS4) sudah berisi pos ronda, sumur dan kerekan, plafon gedek, tangga bambu, tali, pengait, M3, M4, kentongan, Pak Joko, dua warga, dan suara (kerekan, tali, percikan, suasana pos ronda, kentongan, ayam berkokok, napas dan langkah Sosok, musik kejar). Status masih kosong per 19 Nov; Claude membuat pengganti untuk yang bertanda ✗. Di awal Fase 4 tulis `docs/aset/fase-5.md` (CL "Alur Aset").

### Calon K

1. Cara ke pos ronda: lewat pintu depan → `Scene_Desa` (pola K17-B Fase 3) atau langsung. Berapa kali boleh bolak-balik (pinjam tali, lalu kembali untuk B2).
2. Kontrol puzzle kerekan: tahan tombol, gulir mouse, atau tombol naik/turun; umpan balik saat terlalu cepat.
3. Kejaran pertama: apa yang dihitung "lolos" (masuk rumah lalu pintu ditutup, atau bersembunyi), dan rute Sosok dari sumur.
4. Naik plafon: animasi panjat atau pindah posisi langsung dengan layar hitam singkat.
5. Pandangan dari dalam lemari masih tembus (CL Fase 4, R4 Fase 2): diperbaiki sekarang atau Fase 8.

### Ketergantungan dari Fase 3 (CL "Catatan untuk fase berikutnya", rencana Fase 3 baris 3320–3328)

- `AlurMalam3.GantiMalam` berakhir di kartu "bersambung" lalu kembali ke menu; ganti dengan `gm.MuatScene(gm.SceneUntukState)` dan sutradara Malam 4. Hapus notifikasi sementara di `AlurMalam3.Start` untuk Malam 4 ke atas. `PintuDepanMalam3` hanya aktif di Malam 3.
- `AlurRumahBuSumi` hanya mengisi Malam 3.
- `AlurMalam3.LamaSosok` (40 detik) dan `KewaspadaanSantet.JedaKemunculanDetik` masih nilai awal/belum dipakai.
- Ditunda dari fase sebelumnya: lampu teplok yang bisa dibawa, model HP di tangan, Catatan Bapak/Raka yang belum terisi (GS 9.1 baris 780–784 menyebut "sumur timba dan kerekan" dan "ayam cemani" yang cocok untuk Malam 4), animasi tokoh, suara tokoh.

### Lapis A / Lapis B

- **A:** pos ronda (pinjam tali, K1/K2, pilihan bela/diam), kerekan, M3, K3, kejaran pertama dengan checkpoint, obat, menetralkan M3, pergantian ke Malam 5. Pilihan membela tetap A karena +1 Kepercayaan membantu B5/B7 (rencana Fase 3 R4).
- **B:** Halaman 5, plafon dan M4, B2 di pos ronda, Catatan Bapak baru, model asli warga.

### Risiko

- Kejaran adalah sekuens bergerak pertama; NavMesh pekarangan dan pintu belakang bisa membuat Sosok macet (lihat jalan keluar di CL "Daftar Cek Play Mode Fase 2", paragraf setelah butir 32).
- Puzzle kerekan rawan terasa kaku; nilai `KecepatanTarikMaks` disetel saat cek tim.

---

## Fase 5: Malam 5, Tanah Merah

**Jatah:** H13–H14, 28–29 Nov (2 hari). Akhir fase ini = akhir minggu ke-2 (batas target 2 ending).

### Cerita dan tujuan

Sumber: GS 571–617; GDD 199, 571; DS 5 (tidak ada cutscene khusus Malam 5).
- Malam Minggu Pon. Ringkasan siang: Ibu makin lemah, Laras tidak berani keluar kamar (GS 575).
- Kompas ke pohon asem di batas tanah; gali kendi tanah merah dengan pacul (M5). Opsional: kain kafan berajah di dahan, diambil dengan galah (M6) (GS 583–585).
- Catatan Bapak: tanah makam Sumberwangi hitam berpasir; Raka ke makam (GS 587).
- Makam malam hari: makam ayah; ambil sampel di tiga titik, bandingkan dengan kendi lewat inspeksi (warna, tekstur, bau); tidak ada yang cocok, lalu Pak Lasno mengenali tanah Watuireng = B3 (GS 589, 597–599).
- Dialog Laras: lembut → B4 dan Halaman 6; keras → Laras mengunci diri, coba lagi setelah membawakan teh hangat dari dapur (GS 591, 603–608).
- Kepercayaan ≥ 2: Bu Sumi bercerita sisir hilang (B5), memberi Halaman 7 (B7) (GS 593; GS 6.4 baris 312–314). Kepercayaan 3: minyak sereh (`KepercayaanSumi.TerimaMinyakSereh`, sudah ada).
- Opsional: rumah Pakde saat Pakde di pos ronda; anjing penjaga dialihkan dengan sisa makanan; kunci laci di pot bunga teras; kuitansi dan catatan "W" = B6, K4 menjadi Bukti. Bisa juga awal Malam 6 (GS 595, 600–601).

**Tujuan wajib di Core** (sudah ada): `GaliKendi`, `BandingkanTanah`, `BicaraLaras`.

### Scene

- `Scene_Desa` atau pekarangan: pohon asem "di batas tanah" (calon K).
- **`Scene_Makam` baru** (DS 2.7; GS 10.1). Paket `indonesia-grave` sudah ada di `Art/ThirdParty` (untracked, CL Fase 5).
- **`Scene_RumahPakde` baru** (DS 2.7; DS 6 baris Fase 5: anjing, kunci di pot, B6). Lapis B.
- `Scene_RumahBuSumi`: isi Malam 5 (B5, Halaman 7) di `AlurRumahBuSumi`.
- `Scene_RumahKeluarga`: kamar Laras (dialog, pintu terkunci), dapur (teh).
- Setiap scene baru wajib masuk `PetaScene` (dengan test) dan Build Settings (`ProjectSettings/EditorBuildSettings.asset`, hanya file itu yang di-commit; `ProjectSettings.asset` dikembalikan setelah batchmode).

### Core

Sudah ada: `PerbandinganTanah` (3 titik), `TujuanMalam`, `KepercayaanSumi.BisaBeriKesaksian`, `BukuHarian`, `PapanBukti` (K4 menjadi Bukti setelah B6, DS 4.7), `MediaSantet`.

Perlu dibuat:
- Barang: pacul, galah, sampel tanah, teh hangat, sisa makanan, kunci laci (sesuai yang dipakai).
- Aturan dialog Laras: keras → terkunci sampai teh dibawa (bisa cukup state kecil + syarat dialog).
- Perilaku anjing (jika dibuat di Core, sederhana; atau pakai ulang `PerilakuSantet` dengan angka lain, calon K).
- `PetaScene` untuk `Scene_Makam` dan `Scene_RumahPakde` (dan titik muncul desa Malam 5).

### Runtime dan Editor

- Sutradara `AlurMalam5` (rumah), sutradara desa Malam 5, `AlurMakam`, `AlurRumahPakde`; `AlurRumahBuSumi` diperluas.
- Pembuat `PembuatMalam5`, `PembuatMakam`, `PembuatRumahPakde`, data `PembuatDataFase5`.
- Panel inspeksi tanah (pola `PanelAksara`/`PanelGembok`: menjeda lewat `GameManager.Jeda`, melepas tepat sekali termasuk di `OnDisable`).
- Gali dan galah: pola `PapanLantai`/`AmbilBarang`.

### Cutscene

DS 5 tidak mencatat cutscene Malam 5. Pak Lasno muncul di makam cukup adegan berskrip (`KameraAdegan` + dialog).

### Aset

Tulis `docs/aset/fase-5.md` di awal Fase 4. Kandidat dari GS: pohon asem, pacul, kendi kecil, tanah merah, kain kafan berajah, galah bambu, makam desa (pakai `indonesia-grave`), Pak Lasno, teh, rumah Pakde (halaman, teras, pot, meja kerja, laci), anjing kampung, kuitansi. Suara: menggali, anjing menggonggong, suasana makam.

### Calon K

1. Letak pohon asem: pekarangan rumah keluarga atau `Scene_Desa`.
2. Cara ke makam dan rumah Pakde: lewat `Scene_Desa` (pola K17-B) atau pintu langsung; ada checkpoint di scene baru atau tidak (Fase 3 R4: desa dan rumah Bu Sumi tanpa checkpoint).
3. Anjing: AI sendiri, `PerilakuSantet` dipakai ulang, atau patroli tetap tanpa AI.
4. Bentuk panel inspeksi tanah (tiga baris teks per sampel atau gambar).
5. Rumah Pakde di Malam 5, Malam 6, atau keduanya (GS 595).

### Ketergantungan

- `AlurMalam4.GantiMalam` memuat Malam 5 (pola sama dengan Fase 4).
- B5 dan B7 butuh Kepercayaan 2 dari Malam 3–4 (rencana Fase 3 R4).
- Catatan Bapak "tanah makam hitam berpasir" harus ada di Jurnal sebelum ke makam (GS 587).

### Lapis A / Lapis B

- **A:** M5 dan gali kendi, makam dengan perbandingan tanah dan Pak Lasno (B3), dialog Laras dengan dua cabang dan teh (B4, Halaman 6), obat, menetralkan M5, pergantian ke Malam 6. `Scene_Makam` boleh memakai pengganti dulu.
- **B:** M6 di dahan, Bu Sumi Malam 5 (B5, B7), minyak sereh, `Scene_RumahPakde` dengan anjing dan B6.

### Risiko

- Dua scene baru dalam 2 hari. Rumah Pakde paling mudah ditunda tanpa memutus jalur ending.
- Jika B4 bisa terlewat (misalnya pemain memilih kasar lalu tidak membawa teh), sidang Sejati jadi bergantung pada B1/B5–B7. `BicaraLaras` wajib harus berarti B4 benar-benar didapat; tegaskan di rencana.

---

## Fase 6: Malam 6, Obor

**Jatah:** H15–H16, 30 Nov–1 Des (2 hari). Untuk target minimal, Lapis A fase ini sebaiknya mulai sebelum 29 Nov.

### Cerita dan tujuan

Sumber: GS 618–663; GDD 83, 87, 201, 573; DS 4.3, 4.4, 4.8.
- Malam Senin Wage. Ringkasan siang: warga berkumpul di balai dusun, Raka tidak diajak bicara (GS 622).
- Ibu kerasukan, berbicara dengan suara berat (GS 630). Suara tokoh ditunda: tampil sebagai teks.
- Persiapan sampai 21.00: menyiapkan bukti, rumah Pakde jika belum, M6 jika belum (GS 632). Obat 20.00–21.00 (DS 4.4). Batas keras 21.00 (DS 4.3; `JamGim.Batas(6)`).
- 21.00: Laras berlari masuk, warga membawa obor ke rumah Bu Sumi (GS 634). Raka tiba; Pak Joko memimpin (GS 636).
- Sidang: tiga tuduhan, masing-masing dijawab satu item Papan Bukti; benar jika item ada di daftar valid **dan** berlabel Bukti; minimal 2 benar; "Diam saja" atau "Ikut menuduh" kapan pun = gagal langsung (DS 4.8, baris 204–209; GS 644–652).
- Berhasil: Pak Joko menghentikan warga, akan melapor ke Bhabinkamtibmas, Bu Sumi bergabung (GS 640). Gagal: rumah Bu Sumi dibakar, Bu Sumi selamat dengan luka bakar dan dibawa kerabat, jalur Fitnah terkunci (GS 642).
- Dialog kunci GS 654–662.

**Tujuan wajib di Core** (sudah ada): `SidangWarga`. Catatan CL "Catatan untuk Fase 0B": tandai tujuan ini hanya setelah `SidangWarga.Selesaikan`/`Diam`/`IkutMenuduh` dipanggil.

### Scene

- `Scene_RumahKeluarga`: kerasukan, persiapan, Laras masuk.
- `Scene_Desa`: depan rumah Bu Sumi dengan kerumunan obor, tempat sidang. Versi rumah terbakar dan puing (dipakai lagi di epilog Fitnah, DS 5 baris terakhir).
- `Scene_RumahPakde`: opsional, jika di-Lapis-B-kan di Fase 5.

### Core

Sudah ada: `SidangWarga` (`JawabanBenar`, `Selesaikan`, `Diam`, `IkutMenuduh`), `GameState.JalurFitnah`, `KondisiIbu` minimal 1 di jalur Fitnah, `JamGim.Batas`, `BatasKerasTercapai`, jendela obat Malam 6, `PenentuEnding`.

Perlu dibuat:
- Urutan sidang (tuduhan 1–3, menghitung jawaban benar, berhenti di Diam/Menuduh) jika tidak cukup di Runtime. Logika penilaian sudah di Core; tambahkan test jika ada aturan baru.
- Apa yang terjadi jika pemain belum di sidang saat 21.00 (otomatis dibawa ke sidang).
- Pergantian Malam 6 → 7 setelah sidang (cek `s.Ending` dulu, CL Fase 0B).

### Runtime dan Editor

- `AlurMalam6` (rumah) dan sutradara sidang di desa. `PembuatMalam6`, `PembuatDataFase6`.
- **Panel Sidang:** tampilkan tuduhan, daftar item Papan Bukti (Bukti dan Katanya; Katanya selalu salah), tombol "Diam saja" dan "Ikut menuduh". Pola panel yang menjeda.
- HUD hitung mundur sampai 21.00.
- Api/rumah terbakar sederhana (partikel atau material bercahaya, gaya PSX).

### Cutscene (DS 5)

Ibu kerasukan (penuh), Laras berlari masuk 21.00 (berskrip), warga membawa obor (penuh), berhasil: Pak Joko menghentikan warga (penuh), gagal: rumah Bu Sumi dibakar (penuh). Semua Timeline dibuat skrip.

### Aset

Tulis `docs/aset/fase-6.md` di awal Fase 5: obor, kerumunan warga (boleh model warga Fase 4 dengan warna berbeda), api dan asap, puing hangus, suara kerumunan, teriakan "Bakar!", api berkobar, musik tegang.

### Calon K

1. Bentuk sidang: panel 2D dengan daftar item, atau dialog dengan pilihan.
2. Jumlah warga dan cara memperlihatkannya (model, siluet, atau beberapa model diulang).
3. Kerasukan: cutscene penuh atau adegan berskrip, karena suara tokoh ditunda.
4. Setelah gagal: pemain melihat rumah terbakar lalu langsung Malam 7, atau ada kartu teks.
5. Menampilkan Pasal 252 KUHP / main hakim sendiri di Catatan Raka sekarang (GS 9, baris 776; DS 9: dicek ulang di Fase 8).

### Ketergantungan

- B1–B7 dan status Katanya dari Fase 3–5. K3 terbantah oleh B4/B5 (DS 4.7).
- Fase 4: model warga dan Pak Joko. Fase 5: rumah Pakde (jika B6 di Malam 6).

### Lapis A / Lapis B

- **A:** semuanya kecuali yang di B. Sidang dengan kedua hasil, batas 21.00, obat 20.00–21.00, cabang berhasil dan gagal, pergantian ke Malam 7 dengan `HasilSidang` tersimpan.
- **B:** cutscene kerasukan versi penuh (sementara kartu atau adegan singkat), rumah Pakde di awal Malam 6, M6 di Malam 6, kerumunan yang lebih hidup.

### Risiko

- Sidang adalah inti tema (GS 10.4 baris 846: digarap serius). Teks warga dan Raka perlu dicek tim.
- Lima cutscene; jika waktu sempit, cabang berhasil/gagal didahulukan.

---

## Fase 7: Malam 7 + Ending

**Jatah:** H17–H19, 2–4 Des (3 hari). Fase paling berat (DS 9, baris 318).

### Cerita dan tujuan

Sumber: GS 664–767, 786–788; GDD 85, 175–183, 203, 575, 593–597; DS 4.1, 4.3, 4.4, 4.9.
- Malam Selasa Kliwon, batas keras 00.00 (~25 menit nyata; DS 4.3).
- **Jalur utama** (sidang berhasil, GS 672–690): Bu Sumi dan Laras membuat lingkaran garam dan kelor; Bu Sumi menjelaskan induk santet; Raka ke Watuireng lewat jalan sawah berkabut, Sosok membuntuti; ayam cemani berkokok jika terlalu dekat; sanggar: B8 dan Halaman 8; puzzle lilin sesaji (padamkan sesuai pasaran mulai malam ini: Kliwon, Legi, Pahing, Pon, Wage; salah = Mbah Wiryo bangun lebih cepat) (GS 692–694; DS 4.9); M7 kendi induk; lontar mantra pambalik dan pilihan terakhir (GS 696–701); Mbah Wiryo memergoki, kejaran keluar rumah dan kebun; kembali dan ritual terakhir bersama Bu Sumi sebelum 00.00. Obat: Laras otomatis (DS 4.4).
- **Jalur Fitnah** (GS 703–705): tanpa Bu Sumi, Raka bertahan di rumah menjaga Ibu dari Sosok sampai 00.00; Ibu meninggal tepat tengah malam apa pun yang dilakukan.
- **Ending** (DS 4.1 mengalahkan GS 8.1): Fitnah, Terlambat, Balas Dendam, Sejati. Isi epilog GS 726–766. Pesan penutup setelah ending apa pun (GS 786–788).

**Tujuan wajib di Core:** daftar Malam 7 kosong; malam berakhir lewat ending (`TujuanMalam.MalamSelesai` selalu false di Malam 7).

### Scene

- **`Scene_Watuireng` baru:** jalan sawah, rumah dan sanggar Mbah Wiryo, kandang cemani, kebun (DS 2.7; GS 10.1).
- `Scene_RumahKeluarga`: lingkaran garam, ritual terakhir, jalur Fitnah (bertahan), kamar mandi bercermin untuk epilog Balas Dendam (DS 2.7).
- **`Scene_Epilog` baru:** set kecil untuk empat ending, memakai ulang prefab (DS 2.7; DS 5 baris terakhir: kamar mandi bercermin, model Raka, Volume profile pagi/siang, puing rumah Bu Sumi, latar rumah Pakde). Pagi/siang: Volume profile terpisah, kabut dan pass PSX sama (DS 3, baris 143).

### Core

Sudah ada: `LilinSesaji.Buat(malam)` (urutan dari `KalenderJawa.UrutanLilin`), `PenentuEnding` (`Hitung`, `Cek`, `Pilih`, `LolosDariWiryo`), `MediaSantet.Netralkan(M7)` menyalakan `RitualTerakhirSelesai`, `JamGim` (Malam 7 memanggil `PenentuEnding.Cek` tiap maju, `SisaDetikMalam7`), `KondisiIbu` minimal 1 di jalur Fitnah, efek dialog `pilihan-akhir:`, `RitualPenangkal.Pelajari(s, dariBuSumi: true)`.

Perlu dibuat:
- Obat otomatis Malam 7 jalur utama (DS 4.4; belum ada di kode).
- Jika bahan ritual belum lengkap, adegan Bu Sumi memanggil `RitualPenangkal.Pelajari(s, dariBuSumi: true)` sebelum ritual terakhir (CL Fase 0B).
- Akibat salah urutan lilin ("Mbah Wiryo terbangun lebih cepat"): angka/aturan kecil dengan test.
- Kokok ayam cemani sebagai sumber suara (bisa `KewaspadaanSantet` atau aturan jarak sederhana).
- `PetaScene` untuk `Scene_Watuireng` dan `Scene_Epilog` (dengan test).

### Runtime dan Editor

- `AlurMalam7` (rumah, dua jalur), `AlurWatuireng`, `AlurEpilog`. `PembuatMalam7`, `PembuatWatuireng`, `PembuatEpilog`, `PembuatDataFase7`.
- Mbah Wiryo yang mengejar: pakai ulang `SosokSantet`/`PerilakuSantet` dengan model manusia, atau AI kejar sederhana (calon K).
- HUD hitung mundur ke 00.00.
- Layar ending: hentikan gerak dan input pemain setelah `State.Ending` terisi (CL Fase 7). Ganti semua "kembali ke menu jika ending" (`AlurMalam1.GantiMalam`, `AlurMalam2.KeMenuJikaEnding`, `AlurMalam2.Tertangkap`, dan yang serupa di Malam 3–6) dengan layar ending. Layar ending tidak punya pemain, jadi kursor otomatis terlihat (CL Fase 7).
- Ending Terlambat bisa terjadi di malam mana pun (Kondisi 0 sebelum sidang): satu jalan bersama dari setiap sutradara ke `Scene_Epilog`.

### Cutscene (DS 5)

Lingkaran garam (berskrip), Mbah Wiryo memergoki Raka (penuh pendek), ritual terakhir (penuh), epilog Sejati (panjang), Fitnah, Balas Dendam, Terlambat (penuh), pesan penutup (kartu teks).

### Aset

Tulis `docs/aset/fase-7.md` di awal Fase 6. Sudah ada di `Art/ThirdParty` (untracked): `keris` dan `the-bathroom` (ganti tekstur bermerek nyata; CL Fase 7). `kuntilanak-merah-scary` tidak punya tujuan (docs/aset/fase-3.md baris 60: hapus jika tidak dipakai). Kandidat lain: Mbah Wiryo (blangkon, baju hitam, akik; GDD 237), ayam cemani dan kandang, sesaji dan lilin, kendi induk, lontar, buku catatan, sawah berkabut, Bripka Anton, Bidan Wulan, pemakaman (payung, gerimis), puing, cermin, suara ayam (sudah di AS4), kemenyan, api besar.

### Calon K

1. Jalan ke dan dari Watuireng: berjalan penuh atau sebagian kartu teks (jatah waktu 25 menit nyata).
2. Mbah Wiryo mengejar: model manusia dengan AI Sosok, atau adegan kejar yang lebih sederhana; titik "lolos".
3. Akibat salah urutan lilin.
4. Jalur Fitnah Malam 7: Sosok berpatroli dan pemain bersembunyi, atau hanya menunggu; apa yang dilakukan pemain selama ~25 menit.
5. Bentuk epilog: Timeline penuh di `Scene_Epilog` atau kartu teks + beberapa bidikan kamera (DS 9: pakai ulang prefab).
6. Ending Terlambat di tengah malam: langsung pindah ke epilog, atau kartu dulu.

### Ketergantungan

- `HasilSidang` dan Bu Sumi bergabung dari Fase 6.
- Ritual dari Fase 3 (`RitualDapur`, `KuburAbu`), Sosok dan bersembunyi dari Fase 2, model warga dan Pak Joko dari Fase 4, puing dari Fase 6, latar rumah Pakde dari Fase 5.

### Lapis A / Lapis B

- **A (Sejati):** lingkaran garam (bisa singkat), jalan ke Watuireng, sanggar dengan B8, lilin sesaji, M7, pilihan Lapor, kejaran Wiryo (atau versi sederhana), ritual terakhir sebelum 00.00, obat otomatis, epilog Sejati, pesan penutup.
- **A (Fitnah):** Malam 7 jalur Fitnah sampai 00.00, epilog Fitnah, pesan penutup.
- **A (umum):** layar ending yang menghentikan input; Terlambat minimal (kartu "Santet belum selesai." lalu pesan penutup), karena Terlambat bisa terpicu di jalur mana pun.
- **B:** Balas Dendam (pilihan Balas, kamar mandi bercermin, epilog), cutscene Terlambat penuh, ayam cemani berkokok, Halaman 8, Sosok membuntuti di sawah, `keris`.

### Risiko

- Tiga scene/set baru dan delapan cutscene dalam 3 hari. DS 9: set epilog memakai ulang prefab.
- Ending Fitnah dan Sejati masuk target 2 minggu; jika Fase 7 mulai 2 Des sesuai jadwal asli, target itu terlewat. Inilah alasan usul urutan Lapis A di atas.

---

## Fase 8: Polish & Rilis

**Jatah:** H20–H21, 5–6 Des (2 hari).

### Isi

Sumber: DS 6 (baris Fase 8), DS 7.1 (282–297), DS 9 (309–322), CL Fase 8.
- Main dari awal sampai akhir; balancing angka (Kondisi Ibu per malam, jam, `PerilakuSantet`, `KewaspadaanSantet`, tingkat kekuatan DS 4.6, `KerekanSumur`); perbaikan bug.
- Isi Lapis B yang masih tertinggal (dengan keputusan tim).
- Build Windows (Mono) dari Mac, lalu tes di laptop Windows anggota tim dengan daftar DS 7.1: tampilan PSX, kabut, cahaya, resolusi (1366×768 dan 1920×1080), save di `persistentDataPath` Windows, input, audio, **4 ending bisa dicapai**. Temuan dicatat di CL lalu diperbaiki.

### Catatan dari CL "Fase 8"

- `UnityConnectSettings` m_Enabled 1 dan define `SENTIS_ANALYTICS_ENABLED` menyalakan layanan/analytics Unity; matikan untuk build rilis jika tidak dipakai.
- Pengaturan grafis baru "Layar penuh"; tambah pemilih resolusi jika tes Windows membutuhkannya.
- `PembuatBuild.Windows` membuat development build (F5/F6/F9 hidup); build rilis harus non-development.
- Aset pengganti yang tersisa ditukar atau dicatat di layar kredit. Lisensi `pocong-scary`, mobil LMPV, Footsteps, Horror SFX diisi di `docs/aset/KREDIT.md`. Aset CC-BY wajib dikredit di gim (DS 8 butir 4).
- Model Sosok sekitar 30.000 segitiga: cek kinerja di laptop Windows.
- DS 9: cek ulang informasi Pasal 252 KUHP sebelum build.
- Style guide: pastikan `docs/style-guide.md` berstatus TERKUNCI (CL Fase 0, butir 19 daftar 0C).

### Perlu dibuat

- Layar kredit (jika belum ada).
- Mode build rilis di `PembuatBuild` (non-development).
- Pemilih resolusi (calon K).

### Calon K

1. Pemilih resolusi: dibuat atau tidak, tergantung hasil tes Windows.
2. Konten Lapis B yang belum selesai: diselesaikan, atau tim memutuskan lain (konten tidak dipotong tanpa keputusan tim).
3. Suara tokoh: tetap teks saja di rilis atau tidak (CL: suara belakangan, grup mixer `Dialog` sudah ada).

### Risiko

- Hanya 2 hari dan bergantung pada akses laptop Windows anggota tim.
- Perbedaan Metal dan DirectX (DS 9) baru terlihat di sini; build uji Windows sebelumnya sebaiknya sudah dicoba di laptop Windows lebih awal jika memungkinkan.
