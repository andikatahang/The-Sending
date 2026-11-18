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
| Aksara Jawa (puzzle membaca aksara, tabel Halaman 2) | `Noto Sans Javanese` | GF | `Assets/_Project/Art/Fonts/Resources/NotoSansJavanese-Regular.ttf` (nama file harus persis) | BLOCKING untuk Task 10 (gambar aksara) |

## Karakter (model + animasi)

| Tokoh | Kata kunci | Animasi Mixamo yang dibutuhkan | Status |
|---|---|---|---|
| Mbah Wiryo (68, berwibawa, pakaian Jawa gelap, blangkon) | `old man character`, `elderly asian man`, `javanese man blangkon` | Idle, Talking, Walking, Whisper atau Leaning | |
| Sosok Santet (2,2 m, kurus, hitam arang, kain kafan kotor, rambut panjang menutup wajah) | `ghost woman long hair`, `shrouded figure`, `pocong low poly`, `tall creature horror` | Zombie Walk atau Creeping, Idle, Scream atau Attack | ✓ pocong-scary (Sketchfab), diwarnai arang lewat material Kafan, meluncur tanpa animasi (K9-A). Lisensi: isi di KREDIT |
| Laras (animasi tambahan) | (model Fase 1) | Scared atau Trembling, Handing Object | |

## Audio

| Nama | Kata kunci (EN / ID) | Sumber | Catatan | Status |
|---|---|---|---|---|
| Papan kopong diinjak | `hollow wood knock`, `creaky floorboard` | FS | papan lantai kamar Ibu | |
| Papan dibongkar dengan linggis | `wood prying`, `crowbar wood crack` | FS | | |
| Pintu gudang berderit | `old shed door creak` | FS | beda dari pintu rumah | |
| Kunci dan gembok dibuka | `key unlock`, `padlock open` | FS | | |
| Gemerincing paku (Sosok Santet bergerak) | `metal nails jingle`, `chains rattle soft` | FS | ciri khas Sosok Santet | |
| Langkah Sosok Santet | `dragging footsteps`, `slow heavy steps wood` | FS | patroli ruang tengah | belakangan (gemerincing paku sudah menandai gerak Sosok) |
| Stinger kejutan | `horror sting`, `jump scare hit` | FS, PX | Sosok terlihat lewat celah gudang | |
| Napas tertahan dan detak jantung | `holding breath`, `heartbeat slow` | FS | saat bersembunyi | |
| Ketukan pintu depan | `knock wooden door` | FS | Mbah Wiryo datang | |
| Bisikan laki-laki tua | `old man whisper` | FS | Mbah Wiryo kepada Laras | belakangan (suara tokoh ditunda; Fase 2 memakai teks) |
| Musik tegang bersembunyi | `stealth tension loop`, `horror suspense loop` | PX, FS | loop | |
| Gerimis di atap | `light rain on roof` | FS, PX | ambience malam | |

## Nama file dan prefab yang dipakai kode Fase 2

Kode Fase 2 berjalan dengan aset pengganti. Untuk memakai aset asli, taruh atau tukar dengan nama persis di bawah, lalu jalankan **The Sending > Bangun Ulang Semua**.

**Suara** (`.wav`, `.ogg`, atau `.mp3`; (S) = ada WAV sintetis pengganti, hapus WAV itu beserta `.meta`-nya sebelum menaruh rekaman asli):

| Isi | Jalur tanpa akhiran |
|---|---|
| Papan kopong diinjak (S) | `Assets/_Project/Audio/SFX/PapanKopong` |
| Papan dibongkar linggis | `Assets/_Project/Audio/SFX/PapanDibongkar` |
| Pintu gudang berderit | `Assets/_Project/Audio/SFX/PintuGudangBerderit` |
| Kunci dan gembok dibuka | `Assets/_Project/Audio/SFX/KunciDibuka` |
| Gemerincing paku, loop (S) | `Assets/_Project/Audio/SFX/GemerincingPaku` |
| Stinger kejutan (S) | `Assets/_Project/Audio/SFX/StingerKejutan` |
| Detak jantung, loop (S) | `Assets/_Project/Audio/SFX/DetakJantung` |
| Ketukan pintu depan (S) | `Assets/_Project/Audio/SFX/KetukPintu` |
| Gerimis di atap, loop (S) | `Assets/_Project/Audio/SFX/GerimisDiAtap` |
| Musik tegang bersembunyi, loop | `Assets/_Project/Audio/Music/MusikTegang` (selama belum ada: `DroneHoror`) |

**Prefab** di `Assets/_Project/Prefabs/Pengganti/` (depan = +Z, titik nol di lantai; jangan ganti nama file):

| Prefab | Menggantikan |
|---|---|
| `SosokSantet` | Sosok Santet 2,2 m (juga yang lewat di celah gudang). Prefab Asli dari `pocong-scary` jika K9-A |
| `MbahWiryo` | Mbah Wiryo berdiri, blangkon |
| `Boneka` | boneka kain berlilit benang merah dengan dua jarum (M1), berbaring, kepala ke +Z |
| `Linggis`, `Karung`, `AlatTani` | isi gudang |
| `PapanLantai`, `LubangLantai`, `KotakKayu`, `LemariPakaian` | papan kopong dan lubangnya, kotak kayu kamar Raka, lemari tempat bersembunyi |
| `Ranjang` | dibuat ulang dengan kaki (kolong 0,3 m) untuk bersembunyi di kolong ranjang |

**Gambar aksara** di `Assets/_Project/Art/UI/Aksara/`: `Suku_0.png` … `Suku_8.png` (tulisan di boneka: RA, T, MI, SE, LA, SA, KLI, WO, N) dan `Tabel.png` (Halaman 2). Dibuat `tools/render-aksara.sh` dari Noto Sans Javanese.

**Tidak dipakai di Fase 2:** model kunci gudang dan jarum terpisah (kunci hanya di inventaris; jarum bagian dari prefab `Boneka`), kalender dinding baru (prefab `Kalender` Fase 1 dipakai lagi), animasi Laras (gemetar diganti guncangan skrip), bisikan Mbah Wiryo (suara tokoh ditunda).

## Suara dari paket pihak ketiga (dipilih dari nama dan durasi, belum didengar)

Paket ada di `Assets/_Project/Audio/ThirdParty/`. File slot di `Audio/SFX/` adalah **salinan** file paket dengan nama slot (`.meta` lama dipertahankan, jadi scene tidak berubah). Untuk menukar: salin file paket lain ke nama slot yang sama, lalu **Bangun Ulang Semua**.

| Slot | File paket | Durasi | Alternatif untuk dicoba |
|---|---|---|---|
| `StingerKejutan` | `HorrorSFX/hit/hit3.wav` | 1,9 s | hit2 (1,9 s), hit1 (1,7 s), hit7 (2,4 s) |
| `DetakJantung` (loop saat bersembunyi) | `HorrorSFX/beat/beat1.wav` | 1,95 s | beat2–beat4 (1,8–1,9 s); cek sambungan loop tidak berbunyi klik |
| `DentangJam` (jendela obat) | `HorrorSFX/bell/bell4.wav` | 3,3 s | bell2 (2,1 s), bell3 (3,0 s), bell1 (9,6 s, terlalu panjang?) |

`rise/` (riser 3–9,5 s) belum dipakai: tidak ada slot riser sekali-putar di cutscene 23.00 atau Malam 2 (`DroneHoror`/`MusikTegang` adalah loop). Simpan untuk malam berikutnya. `noise/` juga belum dipakai.

Langkah kaki pemain (`LangkahKaki`): `FootstepsEssentials/Footsteps_DirtyGround_Walk|Run` (Tanah: desa, hutan, tanah luar gudang) dan `Footsteps_Wood_Walk|Run` (Kayu: dalam rumah keluarga, jembatan bambu, emper rumah). Permukaan lain dan set lompat/mendarat dihapus dari paket.
