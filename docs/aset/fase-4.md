# Daftar Aset Fase 4 (Malam 4: Rambut di Dasar Sumur)

Dikerjakan 26–27 Nov. Cari mulai sekarang. Aturan sama dengan `fase-3.md`: catat lisensi di `KREDIT.md`, beri ✗ jika tidak ketemu (Claude membuat penggantinya).

Model: utamakan **low poly** (di bawah 5.000 segitiga per benda, karakter di bawah 15.000), format FBX atau GLB. Gaya PSX akan menyamakan tampilannya. Paket dari Sketchfab yang datang sebagai `.zip`/`.rar` boleh ditaruh apa adanya di `Assets/_Project/Art/ThirdParty/<nama>/`; Claude mengekstrak, merapikan, dan mengonversinya saat dipakai.

Sumber: **SF** Sketchfab (filter *Downloadable*, lisensi CC0 atau CC-BY) · **AS** Unity Asset Store (gratis) · **MX** Mixamo (karakter + animasi) · **PH** Poly Haven · **AC** ambientCG · **FS** Freesound · **PX** Pixabay

## Lingkungan

| Nama | Kata kunci (EN / ID) | Sumber | Tujuan | Status |
|---|---|---|---|---|
| Pos ronda (gardu kayu beratap, bangku, kentongan) | `wooden guard post`, `small wooden hut`, `gazebo low poly` / pos ronda, gardu | SF, AS | `Art/Models/Desa/` | |
| Sumur timba belakang dengan kerekan dan tiang | `old well pulley`, `rope pulley wheel`, `stone well` / sumur timba, kerekan | SF, AS | `Art/Models/RumahKeluarga/` (pekarangan belakang, dibuat Fase 3) | |
| Plafon dapur anyaman bambu (bisa dinaiki) | `woven bamboo ceiling`, `attic wooden beams` / plafon gedek | SF, AC | `Art/Models/RumahKeluarga/` | |
| Tangga bambu (disimpan di gudang) | `bamboo ladder`, `wooden ladder` / tangga bambu | SF, AS | `Art/Models/Gudang/` | |
| Tekstur: batu sumur berlumut, air keruh | `mossy stone`, `murky water` | PH, AC | `Art/Textures/` | |

## Benda

| Nama | Kata kunci (EN / ID) | Sumber | Status |
|---|---|---|---|
| Tali tambang gulungan | `rope coil` / tali tambang | SF, AS | |
| Pengait besi | `iron hook`, `grappling hook small` / pengait | SF | |
| Bungkusan kain hitam berisi rambut uban dan kembang busuk (Media 3) | `black cloth bundle`, `wilted flowers` / buntelan kain hitam | SF | |
| Telur ayam hitam berisi jarum (Media 4) | `black egg`, `dark egg` / telur cemani | SF | |
| Kentongan bambu | `bamboo slit drum`, `kentongan` / kentongan | SF | |
| Gelas kopi, termos, dan papan catur di pos ronda | `coffee glass`, `thermos`, `chess board low poly` / termos, gelas kopi | SF, AS | |

## Karakter (model + animasi)

| Tokoh | Kata kunci | Animasi Mixamo yang dibutuhkan | Status |
|---|---|---|---|
| Pak Joko (50, kepala dusun, tegas; sarung dan peci) | `middle aged asian man`, `man sarong`, `village chief` | Idle, Talking, Pointing, Arms Crossed | |
| Warga 1 dan Warga 2 (peronda; boleh model yang sama dengan warna baju berbeda) | `asian villager man`, `farmer man` | Idle, Talking, Sitting Talking, Pointing | |

## Audio

| Nama | Kata kunci (EN / ID) | Sumber | Catatan | Status |
|---|---|---|---|---|
| Kerekan sumur berderit | `well pulley creak`, `rusty pulley` | FS | puzzle kerekan, loop pendek | |
| Tali ditarik dan diulur | `rope pull`, `rope friction` | FS | | |
| Benda jatuh ke air sumur | `splash well`, `object falls into water` | FS | bungkusan jatuh kembali | |
| Suasana pos ronda (jangkrik, obrolan pelan, radio jauh) | `night crickets village`, `men talking distant` | FS, PX | grup Ambience | |
| Kentongan dipukul | `kentongan`, `bamboo knock alarm` | FS | | |
| Ayam berkokok di malam hari | `rooster crow night` | FS, PX | dipakai lagi Malam 7 (cemani) | |
| Napas berat dan langkah cepat Sosok saat mengejar | `monster breathing`, `heavy running footsteps` | FS | sekuens kejar pertama | |
| Musik kejar | `horror chase music loop` | PX | grup Music, loop | |

## Nama file dan prefab yang dipakai kode Fase 4

Kode Fase 4 memuat aset pengganti dengan nama di bawah. Untuk menukar dengan aset asli: buka prefab di `Assets/_Project/Prefabs/Pengganti/`, ganti anak visualnya (depan = +Z, titik nol di lantai), atau timpa file suara dengan nama yang sama. Jangan mengganti nama file. Prefab di `Prefabs/Asli/` dipakai lebih dulu jika ada. Nama prefab sesuai rencana Task 6; tim memeriksa daftar akhirnya di folder setelah Task 6 selesai.

**Prefab (`Prefabs/Pengganti/`)**

| Prefab | Dipakai untuk |
|---|---|
| `SumurTimba` | sumur belakang dengan kerekan dan tiang |
| `PlafonAnyaman` | plafon dapur yang bisa dinaiki |
| `TanggaBambu` | tangga di gudang |
| `TaliTambang` | tali puzzle kerekan |
| `PengaitBesi` | pengait puzzle kerekan |
| `BungkusanHitam` | Media 3 (bungkusan kain hitam) |
| `TelurCemani` | Media 4 (telur hitam berisi jarum) |
| `Kentongan` | pos ronda |
| `PosRonda` | gardu, bangku, gelas kopi, termos, papan catur di `Scene_Desa` |
| `PakJoko`, `Warga1`, `Warga2` | tokoh di pos ronda (kapsul pengganti) |

**Suara (`Audio/SFX/`, WAV sintetis, ditandai (S))**

| File | Dipakai untuk |
|---|---|
| `KerekanDerit.wav` | kerekan sumur berderit |
| `TaliDitarik.wav` | tali ditarik dan diulur |
| `BendaMasukAir.wav` | bungkusan jatuh ke air |
| `Kentongan.wav` | kentongan dipukul |
| `AyamBerkokok.wav` | ayam berkokok |

## Musik dari Horror Starter Pack

Lagu asli tetap di `Assets/Horror Starter Pack/`; salinan .ogg (loop) ada di `Assets/_Project/Audio/Music/` dengan nama slot, ditemukan pembuat lewat nama dasar.

| Slot | Dipakai di | Lagu |
|---|---|---|
| `MusikTegang` | Sosok muncul Malam 2 dan 3 | `sp-underthestairs` (115 dtk) |
| `MusikKejar` | kejaran Malam 4 | `sp-horroraction` (38 dtk) |
| `MusikMalam` | musik latar Malam 1 (menggantikan `DroneHoror` sintetis) | `sp-lost` (79 dtk) |

Musik menu utama: belum ada slotnya di kode, jadi tidak dipasang. Sisa lagu belum terpakai.
