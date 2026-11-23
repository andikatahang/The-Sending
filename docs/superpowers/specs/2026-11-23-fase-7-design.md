# Desain Fase 7: Malam 7 + Ending

**Tanggal:** 23 Nov 2026 · **Branch:** `fase-7` · **Jatah:** H17–H19 (2–4 Des), dikerjakan lebih awal.
Sumber: GS 664–788, DS 4.1/4.3/4.4/4.9 dan 5, roadmap Fase 7, catatan Fase 7 di `CLAUDE.md`, `docs/aset/fase-7.md`.

## Maksud

Menutup gim: Malam 7 bisa dimainkan di kedua jalur, keempat ending bisa dicapai, dan setiap ending ditutup pesan penutup (GS 9.2). Ini memenuhi target minimal tim (ending Fitnah dan Sejati) sekaligus konten penuh GDD (Balas Dendam dan Terlambat).

- **Tujuan:** jalur utama (Watuireng, lilin sesaji, M7, pilihan, kejaran Wiryo, ritual terakhir), jalur Fitnah (bertahan sampai 00.00), empat epilog, pesan penutup, dan layar ending yang menggantikan "kembali ke menu" di semua malam.
- **Bukan tujuan:** suara tokoh (tetap teks), model asli untuk semua tokoh, balancing akhir (Fase 8), dan build rilis.
- **Dianggap gagal walaupun jalan:** ending bisa terlewat atau macet (pemain tidak bisa keluar dari Malam 7), ending yang salah menurut urutan DS 4.1, dan input pemain masih hidup setelah `State.Ending` terisi.

## Batasan

Aturan tim di `CLAUDE.md`: Core TDD, scene dibuat skrip, satu sutradara per bagian cerita, Timeline dari `PembuatTimeline` (Unscaled, wrap None, bisa dilewati dengan Spasi), tidak ada `WaitForSeconds` di adegan, jeda lewat `GameManager.Jeda`, Input System saja, commit tanpa `Co-Authored-By`.

## Pendekatan

| Pendekatan | Untung / rugi | Dipilih |
|---|---|---|
| Pola Fase 2–6: Core + sutradara + skrip pembuat + Timeline skrip | Konsisten, bisa dibangun ulang, sudah teruji | **Ya** |
| Scene Watuireng/Epilog disusun tangan di editor | Cepat untuk diatur, tetapi hilang saat Bangun Ulang Semua dan melanggar aturan tim | Tidak |
| Ending sebagai sistem/scene terpisah tanpa sutradara | Tidak cocok dengan pola `PetaScene` dan checkpoint yang ada | Tidak |

## Keputusan tim

- **K49-A (23 Nov):** jalan sawah ke Watuireng bisa dijalani (sekitar 60–80 m) di awal `Scene_Watuireng`, dengan Sosok membuntuti dari jauh. Pulang ke rumah setelah kejaran memakai kartu teks.
- **K50-A (23 Nov):** epilog di `Scene_Epilog` berupa 3–4 bidikan kamera Timeline per ending di set kecil (memakai ulang prefab), dialog dan narasi lewat kartu teks, lalu pesan penutup, lalu menu.
- **Usulan Claude yang tidak dikoreksi tim (23 Nov):**
  - Mbah Wiryo mengejar dengan otak `SosokSantet`/`PerilakuSantet` dan model `tokoh-dasar` Mbah Wiryo. Lolos = sampai di gerbang kebun. Tertangkap = muat checkpoint sebelum kejaran, lalu −5 (DS 4.2).
  - Lilin salah urutan: jam gim +15 menit ("Mbah Wiryo makin gelisah") dan urutan diulang. (Aturan "salah ketiga = Wiryo bangun" dibuang saat menulis rencana, K51: belum ada checkpoint sebelum pilihan.)
  - Jalur Fitnah: Sosok berpatroli di rumah seperti Malam 2, Raka bersembunyi dan menjaga kamar Ibu sampai 00.00. Tertangkap = checkpoint (Kondisi Ibu tetap minimal 1).
  - Obat Malam 7 jalur utama: Laras memberi otomatis pukul 21.00 (+2) dengan notifikasi HUD (DS 4.4).

## Desain

### Core (TDD)

- **Laju jam Malam 7:** 250 detik nyata per jam gim (DS 4.3), sama dengan Malam 6. `JamGim.DetikPerJamGim(7)` dan `SisaDetikMalam7` memakai laju itu.
- **Obat otomatis:** `ObatMalam7.Cek(s)` memberi +2 sekali pada 21.00 jika `HasilSidang == Berhasil`.
- **Lilin:** `LilinSesaji` ditambah penghitung salah: setiap salah memajukan `MenitGim` 15 menit, salah ketiga mengembalikan `WiryoBangun`. Penghitungnya bukan bagian `GameState` (sesi puzzle saja), tetapi waktu yang maju tersimpan.
- **Kokok cemani:** aturan jarak sederhana (dekat kandang tanpa senter mati = berkokok, Wiryo makin gelisah, misalnya +5 menit, sekali per 20 detik nyata).
- **`PetaScene`:** `Watuireng` dan `Epilog`. `Untuk(state)` memilih Watuireng hanya untuk checkpoint sebelum kejaran (`PilihanAkhir != Belum && !LolosKejaranWiryo`), dan memilih Epilog saat `Ending != Belum`.
- **Bahan ritual:** sebelum ritual terakhir, Bu Sumi memanggil `RitualPenangkal.Pelajari(s, dariBuSumi: true)` jika bahan belum lengkap (catatan 0B).
- Semua ending lewat `PenentuEnding.Cek`/`Pilih`/`LolosDariWiryo` (sudah ada).

### Rumah keluarga (`AlurMalam7`)

- Awal malam (setelah kartu "Malam 7: Malam Selasa Kliwon"), jalur utama: adegan berskrip lingkaran garam dan kelor di sekeliling ranjang. Bu Sumi mengucapkan kutipan induk santet persis. Pintu depan memuat `Scene_Watuireng`.
- Kembali dari Watuireng (kartu teks pulang), pilihan Lapor: ritual terakhir di anglo bersama Bu Sumi (cutscene penuh), `MediaSantet.Netralkan(M7)`, lalu `PenentuEnding.Cek` (Sejati). Pilihan Balas tidak kembali ke rumah: setelah lolos, ending Balas Dendam.
- Jalur Fitnah: kartu ringkas, Sosok berpatroli (pakai ulang Malam 2), titik sembunyi yang sama. Pukul 00.00: ending Fitnah.
- Checkpoint di awal Malam 7 (otomatis, seperti malam lain) dan sebelum kejaran Wiryo.

### `Scene_Watuireng` (`AlurWatuireng`, `PembuatWatuireng`, indeks build berikutnya)

- Jalan sawah berkabut sekitar 70 m: Sosok terlihat membuntuti dari jauh (berjalan sejajar, tidak menyerang).
- Rumah Mbah Wiryo: Wiryo bersemadi di depan sesaji (diam). Kandang cemani di samping jalan masuk.
- Sanggar: buku catatan (B8 dan Halaman 8, kutipan GS persis), lima lilin bertanda pasaran (urutan dari `KalenderJawa.UrutanLilin(7)`: Kliwon, Legi, Pahing, Pon, Wage), kendi induk M7, lontar dengan dialog pilihan (`pilihan-akhir:Lapor`/`Balas`).
- Setelah pilihan (atau salah lilin ketiga): checkpoint, cutscene pendek Wiryo memergoki (kutipan "Bapakmu dulu juga keras kepala..." persis), kejaran keluar rumah dan kebun. Lolos di gerbang: `PenentuEnding.LolosDariWiryo`, lalu kartu pulang dan rumah (Lapor) atau epilog (Balas).

### Layar ending dan `Scene_Epilog`

- **Satu jalan bersama:** `GameManager` memuat `Scene_Epilog` saat `Ending` terisi (lewat `PetaScene.Untuk`). Semua `KeMenuJikaEnding` dan cabang "Ending → menu" di `AlurMalam1`–`AlurMalam6` dan `AlurDesaMalam6` diganti ke jalan itu. Gerak dan input pemain berhenti setelah `Ending` terisi.
- **Terlambat di tengah malam:** kartu "Kondisi Ibu..." singkat, lalu epilog.
- **`AlurEpilog`:** memutar Timeline ending yang tersimpan, dengan kartu teks untuk dialog dan narasi:
  - Sejati: kamar Ibu pagi, Bripka Anton dengan kutipan KUHP, rumah Bu Sumi, narasi Raka.
  - Fitnah: pemakaman gerimis (`indonesia-grave`), Pakde, Wiryo di belakang, puing (`PuingRumahKecil`).
  - Balas Dendam: kamar mandi bercermin (`the-bathroom`), Sosok di cermin, suara Wiryo.
  - Terlambat: kamar Laras, kartu "Santet belum selesai."
- Setelah ending apa pun: kartu pesan penutup GS 9.2, lalu menu utama. Save dihapus atau ditandai selesai supaya "Lanjutkan" tidak memuat epilog berulang (diputuskan di rencana).
- Volume profile pagi untuk epilog Sejati (DS 3: kabut dan pass PSX sama).

### Catatan wajib dari `CLAUDE.md`

`AlurDesaMalam6.GantiMalam` memuat Malam 7 (hapus kartu "Bersambung" dan notifikasi "Malam 7 belum dibuat"). Bu Sumi bergabung bila `HasilSidang.Berhasil`. `PuingRumahSumi` di desa dipakai lagi di epilog Fitnah. `the-bathroom` (ganti tekstur bermerek), `keris` (K16). Risiko Timeline Malam 6 yang tercatat diperiksa ulang saat cek Play Mode.

## Keputusan Claude dari pemeriksaan pembuat (bisa dikoreksi)

- Jalur Fitnah Malam 7 tidak punya obat (Ibu tetap meninggal pukul 00.00; Kondisi minimal 1).
- Waktu habis saat masih di Watuireng = Terlambat (sudah begitu di `PenentuEnding.Hitung`); `GameManager` langsung ke epilog dari scene mana pun.
- Pilihan Balas juga melewati kejaran Wiryo; ending Balas Dendam baru tercapai setelah lolos (DS 4.1 butir 3).

## Diserahkan ke pembuat rencana

Nomor K berikutnya mulai K51. Koordinat dan denah, isi teks draf (ditandai untuk dicek tim), nama file suara pengganti, angka penalti kokok dan lilin (bisa disetel di Fase 8), dan perlakuan save setelah ending.
