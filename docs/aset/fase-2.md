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
