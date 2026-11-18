# Daftar Aset Fase 3 (Malam 3: Tujuh Paku di Saka)

Dikerjakan 24–25 Nov. Cari mulai sekarang. Aturan sama dengan `fase-2.md`: catat lisensi di `KREDIT.md`, beri ✗ jika tidak ketemu (Claude membuat penggantinya).

Model: utamakan **low poly** (di bawah 5.000 segitiga per benda, karakter di bawah 15.000), format FBX atau GLB. Gaya PSX akan menyamakan tampilannya. Paket dari Sketchfab yang datang sebagai `.zip`/`.rar` boleh ditaruh apa adanya di `Assets/_Project/Art/ThirdParty/<nama>/`; Claude mengekstrak, merapikan, dan mengonversinya saat dipakai.

Sumber: **SF** Sketchfab (filter *Downloadable*, lisensi CC0 atau CC-BY) · **AS** Unity Asset Store (gratis) · **MX** Mixamo (karakter + animasi) · **PH** Poly Haven · **AC** ambientCG · **FS** Freesound · **PX** Pixabay

## Lingkungan

| Nama | Kata kunci (EN / ID) | Sumber | Tujuan | Status |
|---|---|---|---|---|
| Interior rumah Bu Sumi (dapur jamu, rak botol, lumpang) | `village kitchen interior`, `herbal medicine shelf`, `mortar pestle stone` / dapur jamu, lumpang | SF, AS | `Art/Models/RumahBuSumi/` | |
| Sumur timba kecil di halaman Bu Sumi | `old well bucket`, `village well` / sumur timba | SF, AS | `Art/Models/RumahBuSumi/` | |
| Kotak sepatu lama (Halaman 3) | `old shoe box`, `cardboard box` / kotak sepatu | SF | `Art/Models/RumahKeluarga/` | |
| Anglo (tungku arang, tempat Ritual Penangkal) | `charcoal stove`, `clay brazier` / anglo | SF | `Art/Models/RumahKeluarga/` | |
| Sofa tua di ruang tengah (Pakde duduk) | sudah diimpor tim: `ThirdParty/old-sofas` | — | dipasang Claude di Fase 3 | ✓ (belum dikonversi) |
| Tekstur: dempul hitam, kayu saka bertekstur | `black putty`, `old wood pillar` | PH, AC | `Art/Textures/` | |

## Benda

| Nama | Kata kunci (EN / ID) | Sumber | Status |
|---|---|---|---|
| Tujuh paku berkarat berukir (di saka) | `rusty nail`, `old iron nail` / paku berkarat | SF | |
| Tang | `pliers` / tang | SF, AS | |
| Bungkusan buah dari Pakde | `fruit basket`, `plastic bag fruit` / buah tangan | SF | |
| Garam (rak bumbu dapur), daun kelor, bilah bambu kuning | `salt jar`, `moringa leaves`, `yellow bamboo` / garam, daun kelor, bambu kuning | SF, PH | |
| Lampu teplok yang bisa dibawa (rusak, diperbaiki untuk Bu Sumi) | `oil lamp`, `kerosene lamp` / lampu teplok | SF | |
| Timba air | `wooden bucket` / timba | SF, AS | |

## Karakter (model + animasi)

| Tokoh | Kata kunci | Animasi Mixamo yang dibutuhkan | Status |
|---|---|---|---|
| Pakde Darmo (55, kemeja batik, jam tangan emas) | `middle aged asian man`, `man batik shirt` | Idle, Talking, Angry Gesture, Walking | |
| Bu Sumi (62, rambut putih disanggul, kebaya lusuh) | `old asian woman`, `grandmother kebaya` | Idle, Talking, Crying, Closing Door | |
| Ibu menjerit saat jam berdentang | (model Fase 1) | Scream (berbaring) | |

## Audio

| Nama | Kata kunci (EN / ID) | Sumber | Catatan | Status |
|---|---|---|---|---|
| Ibu menjerit seperti ditusuk | `woman scream pain` | FS | efek, bukan dialog | |
| Ketukan pada tiang kayu (padat dan berongga) | `knock wooden pillar`, `hollow wood knock` | FS | puzzle mencari saka | |
| Paku dicabut dengan tang | `nail pull`, `pliers metal squeak` | FS | tujuh kali | |
| Api anglo, arang menyala | `charcoal fire crackle`, `small fire` | FS, PX | Ritual Penangkal | |
| Motor datang dan pergi | `motorcycle arrive`, `scooter idle leave` | FS | Pakde Darmo | |
| Pintu dibanting | `door slam wooden` | FS | Bu Sumi menutup pintu | |
| Air ditimba | `well bucket water`, `rope pulley` | FS | membantu Bu Sumi | |

## Paket yang sudah diimpor tim, dipakai di fase lain (K16)

| Paket | Isi | Dipakai | Catatan |
|---|---|---|---|
| `ThirdParty/pocong-scary` | model pocong (sekitar 30.000 segitiga) | Fase 2, Sosok Santet (K9-A) | lisensi dan pembuat diisi di KREDIT |
| `ThirdParty/old-sofas` | sofa tua (`.rar` berisi OBJ + tekstur) | Fase 3, ruang tengah | |
| `ThirdParty/indonesia-grave` | makam (`.rar` berisi OBJ + foto tekstur) | Fase 5, `Scene_Makam` | |
| `ThirdParty/the-bathroom` | kamar mandi (FBX) | Fase 7, epilog Balas Dendam | tekstur `rinso`, `pepsodent`, `vixal`, `vinilex` memuat merek nyata: diganti label rekaan saat dipakai |
| `ThirdParty/keris` | keris (`.zip` berisi OBJ) | Fase 7, sanggar Mbah Wiryo | |
| `ThirdParty/kuntilanak-merah-scary` | kuntilanak ber-rig | tidak dipakai jika K9-A | hapus jika tidak dipakai |
