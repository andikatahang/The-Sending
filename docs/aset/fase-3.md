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
| Sofa tua di ruang tengah (Pakde duduk) | sudah diimpor tim: `ThirdParty/old-sofas` | — | dipasang Claude di Fase 3 | ✓ dikonversi di Fase 3 (sofa tiga dudukan, prefab Asli Sofa, K22-A) |
| Tekstur: dempul hitam, kayu saka bertekstur | `black putty`, `old wood pillar` | PH, AC | `Art/Textures/` | |

## Benda

| Nama | Kata kunci (EN / ID) | Sumber | Status |
|---|---|---|---|
| Tujuh paku berkarat berukir (di saka) | `rusty nail`, `old iron nail` / paku berkarat | SF | |
| Tang | `pliers` / tang | SF, AS | |
| Bungkusan buah dari Pakde | `fruit basket`, `plastic bag fruit` / buah tangan | SF | |
| Garam (rak bumbu dapur), daun kelor, bilah bambu kuning | `salt jar`, `moringa leaves`, `yellow bamboo` / garam, daun kelor, bambu kuning | SF, PH | |
| Lampu teplok yang bisa dibawa (rusak, diperbaiki untuk Bu Sumi) | `oil lamp`, `kerosene lamp` / lampu teplok | SF | belakangan (teplok Bu Sumi diperbaiki di tempat, tidak dibawa) |
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
| `ThirdParty/old-sofas` | sofa tua (`.rar` berisi OBJ + tekstur) | Fase 3, ruang tengah | dipakai sejak Fase 3 (K22-A) |
| `ThirdParty/indonesia-grave` | makam (`.rar` berisi OBJ + foto tekstur) | Fase 5, `Scene_Makam` | |
| `ThirdParty/the-bathroom` | kamar mandi (FBX) | Fase 7, epilog Balas Dendam | tekstur `rinso`, `pepsodent`, `vixal`, `vinilex` memuat merek nyata: diganti label rekaan saat dipakai |
| `ThirdParty/keris` | keris (`.zip` berisi OBJ) | Fase 7, sanggar Mbah Wiryo | |
| `ThirdParty/kuntilanak-merah-scary` | kuntilanak ber-rig | tidak dipakai jika K9-A | hapus jika tidak dipakai |

## Nama file dan prefab yang dipakai kode Fase 3

Kode Fase 3 berjalan dengan aset pengganti. Untuk memakai aset asli, taruh atau tukar dengan nama persis di bawah, lalu jalankan **The Sending > Bangun Ulang Semua**.

**Suara** (`.wav`, `.ogg`, atau `.mp3`; (S) = ada WAV sintetis pengganti, hapus WAV itu beserta `.meta`-nya sebelum menaruh rekaman asli):

| Isi | Jalur tanpa akhiran |
|---|---|
| Ketukan tiang kayu padat (S) | `Assets/_Project/Audio/SFX/KetukPadat` |
| Ketukan tiang kayu berongga (S) | `Assets/_Project/Audio/SFX/KetukBerongga` |
| Paku dicabut dengan tang (S) | `Assets/_Project/Audio/SFX/PakuDicabut` |
| Api arang di anglo, loop (S) | `Assets/_Project/Audio/SFX/ApiAnglo` |
| Pintu dibanting (S) | `Assets/_Project/Audio/SFX/PintuDibanting` |
| Ibu menjerit seperti ditusuk | `Assets/_Project/Audio/SFX/IbuMenjerit` |
| Motor Pakde datang / pergi | `Assets/_Project/Audio/SFX/MotorDatang`, `Assets/_Project/Audio/SFX/MotorPergi` |
| Air ditimba dari sumur | `Assets/_Project/Audio/SFX/AirDitimba` |
| Pintu kayu berderit (pintu rumah keluarga, pintu rumah Bu Sumi) | `Assets/_Project/Audio/SFX/PintuBerderit` |

**Prefab** di `Assets/_Project/Prefabs/Pengganti/` (depan = +Z, titik nol di lantai; jangan ganti nama file):

| Prefab | Menggantikan |
|---|---|
| `PakdeDarmo` | Pakde Darmo berdiri, kemeja batik |
| `BuSumi` | Bu Sumi berdiri, rambut putih disanggul, kebaya lusuh |
| `BuahTangan` | bungkusan buah dari Pakde (di meja ruang tengah) |
| `KotakSepatu` | kotak sepatu lama di kamar Raka (Halaman 3) |
| `Tang`, `HalamanSobek`, `BambuKuning` | isi gudang: tang, Halaman 4, bilah bambu kuning bersandar |
| `Dempul`, `PakuSaka` | dempul hitam di saka (0,02 × 0,95 × 0,2 m, sumbu tipis = X) dan satu paku berkarat mendatar sepanjang X |
| `Anglo`, `ApiAnglo`, `RakBumbu` | anglo dapur, nyala apinya, rak bumbu dengan toples garam (tempat mengambil garam) |
| `GaramToples`, `IkatKelor`, `KorekApi` | garam, seikat daun kelor, dan korek api di meja samping anglo: langkah Ritual Penangkal (K25-B); bilah bambu kuning di sana memakai `BambuKuning` |
| `PohonKelor` | pohon kelor di pekarangan belakang |
| `Sofa` | sofa tiga dudukan di ruang tengah (prefab Asli dari `old-sofas` jika K22-A) |
| `Sumur`, `RakBotol`, `Lumpang`, `BaleBale` | rumah Bu Sumi (`Scene_RumahBuSumi`): sumur timba di halaman belakang, rak botol jamu, lumpang batu, bale-bale bambu. Bagian luar rumahnya tetap `RumahKecil` di `Scene_Desa` |

**Tidak dipakai di Fase 3:** model tekstur dempul dan saka bertekstur (saka memakai prefab Asli Fase 1, dempul kotak gelap), timba terpisah (bagian dari prefab `Sumur`), animasi Pakde, Bu Sumi, dan Ibu menjerit (guncangan skrip), lampu teplok yang bisa dibawa.
