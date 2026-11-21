# Daftar Aset Fase 5 (Malam 5: Tanah Merah)

Status (21 Nov): Fase 5 selesai di kode. Baris tanpa status masih memakai pengganti dari Claude (`Prefabs/Pengganti/`, suara sintetis); tim boleh menukar dengan aset asli.

Dikerjakan 28–29 Nov. Cari mulai sekarang. Aturan sama dengan `fase-4.md`: catat lisensi di `KREDIT.md`, beri ✗ jika tidak ketemu (Claude membuat penggantinya).

Model: utamakan **low poly** (di bawah 5.000 segitiga per benda, karakter di bawah 15.000), format FBX atau GLB. Gaya PSX akan menyamakan tampilannya. Paket dari Sketchfab yang datang sebagai `.zip`/`.rar` boleh ditaruh apa adanya di `Assets/_Project/Art/ThirdParty/<nama>/`; Claude mengekstrak, merapikan, dan mengonversinya saat dipakai.

Sumber: **SF** Sketchfab (filter *Downloadable*, lisensi CC0 atau CC-BY) · **AS** Unity Asset Store (gratis) · **MX** Mixamo (karakter + animasi) · **PH** Poly Haven · **AC** ambientCG · **FS** Freesound · **PX** Pixabay

## Lingkungan

| Nama | Kata kunci (EN / ID) | Sumber | Tujuan | Status |
|---|---|---|---|---|
| Makam desa (nisan kayu dan batu, gundukan, kamboja) | `javanese cemetery`, `graveyard low poly`, `frangipani tree` / makam desa, kuburan, kamboja | SF, AS | `Scene_Makam` | dipakai di `Scene_Makam` (Task 7) |
| Pohon asem besar dengan dahan atas | `tamarind tree`, `big old tree low poly` / pohon asem, pohon asam jawa | SF, AS | `Scene_Desa` atau pekarangan, batas tanah | |
| Rumah Pakde Darmo (rumah batu lebih mewah, halaman, teras, pagar) | `village house modern indonesia`, `house with yard low poly` / rumah kampung, rumah tembok | SF, AS | `Scene_RumahPakde` (Lapis B) | |
| Pot bunga besar di teras | `flower pot big`, `clay pot plant` / pot bunga, pot tanah liat | SF, AS | `Scene_RumahPakde` | |
| Meja kerja dengan laci | `wooden desk drawers low poly` / meja kerja, meja laci | SF, AS | `Scene_RumahPakde` | |
| Tekstur: tanah makam hitam berpasir, tanah merah lengket | `black sand soil`, `red clay mud`, `wet soil` | PH, AC | `Art/Textures/` | |
| Tekstur: rumput liar dan batu nisan berlumut | `wild grass`, `mossy gravestone` | PH, AC | `Art/Textures/` | |

## Benda

| Nama | Kata kunci (EN / ID) | Sumber | Status |
|---|---|---|---|
| Pacul | `hoe farm tool`, `mattock` / cangkul, pacul | SF, AS | |
| Kendi kecil berisi tanah merah (Media 5) | `small clay jug`, `terracotta jar` / kendi, kendil | SF | |
| Kain kafan berajah tergantung di dahan (Media 6) | `white cloth hanging`, `shroud cloth` / kain kafan, mori | SF | |
| Galah bambu panjang | `bamboo pole long` / galah bambu | SF, AS | |
| Wadah sampel tanah (botol kecil atau plastik) | `small glass jar`, `sample bag` / botol kecil | SF | |
| Cangkir teh hangat dan nampan | `tea cup tray`, `ceramic mug` / gelas teh, cangkir | SF, AS | |
| Sisa makanan untuk anjing (tulang atau nasi bungkus) | `dog bone`, `food scraps` / tulang, sisa makanan | SF | |
| Kunci laci kecil | `small key`, `brass key low poly` / kunci kecil | SF, AS | |
| Kuitansi dan buku catatan pembayaran | `receipt paper`, `old ledger book` / kuitansi, buku catatan | SF | |
| Nisan makam ayah Raka (nama terbaca) | `wooden grave marker`, `headstone` / nisan | SF | |

## Karakter (model + animasi)

| Tokoh | Kata kunci | Animasi Mixamo yang dibutuhkan | Status |
|---|---|---|---|
| Pak Lasno (penjaga makam, tua; sarung dan peci lusuh, bisa membawa lampu) | `old asian man`, `elderly man sarong`, `graveyard keeper` | Idle, Talking, Pointing, Walking | |
| Anjing kampung penjaga rumah Pakde | `village dog`, `stray dog low poly` | Idle, Walking, Barking, Sniffing | |

Pak Joko, Warga 1, dan Warga 2 (Fase 4) dipakai lagi tanpa aset baru.

## Audio

| Nama | Kata kunci (EN / ID) | Sumber | Catatan | Status |
|---|---|---|---|---|
| Cangkul menggali tanah | `digging soil shovel`, `hoe dig dirt` | FS | gali kendi dan sampel tanah | |
| Kendi diangkat atau pecah | `clay pot lift`, `ceramic jar scrape` | FS | | |
| Kain digapai dengan galah dan jatuh | `cloth rustle`, `pole branch rustle` | FS | | |
| Suasana makam malam (angin, jangkrik jauh, burung hantu) | `graveyard night ambience`, `owl night` | FS, PX | grup Ambience | |
| Langkah di tanah dan rumput basah | `footsteps wet grass`, `footsteps dirt` | FS | `Permukaan` baru untuk `LangkahKaki` | |
| Anjing menggonggong dan menggeram | `dog barking night`, `dog growl` | FS, PX | patroli anjing Pakde | |
| Anjing mengunyah makanan | `dog eating` | FS | pengalih perhatian | |
| Kunci diputar dan laci ditarik | `key turn lock`, `drawer open wooden` | FS | laci meja kerja Pakde | |
| Gelas teh diletakkan | `ceramic cup set down` | FS | teh untuk Laras | |
| Laras menangis pelan (bukan suara tokoh, hanya isak) | `soft crying`, `sobbing` | FS | opsional; suara tokoh ditunda | |

## Paket yang sudah ada di `Art/ThirdParty` (belum terpakai)

| Paket | Isi | Dipakai di | Catatan |
|---|---|---|---|
| `indonesia-grave` | makam (`.rar` berisi OBJ + foto tekstur) | Fase 5, `Scene_Makam` | sudah dikonversi dan dipakai (Task 7) |
| `the-bathroom` | kamar mandi (FBX) | Fase 7, epilog Balas Dendam | tekstur bermerek nyata diganti label rekaan saat dipakai |
| `keris` | keris (`.zip` berisi OBJ) | Fase 7, sanggar Mbah Wiryo | |
| `kuntilanak-merah-scary` | kuntilanak ber-rig | tidak dipakai jika K9-A | hapus jika tidak dipakai |

## Nama file dan prefab yang dipakai kode Fase 5

Claude membuat pengganti dengan nama ini (Task 7). Untuk menukar dengan model asli: buka prefab di folder, ganti anaknya (depan = +Z, titik nol di lantai), jangan ganti nama prefab.

### Prefab di `Assets/_Project/Prefabs/Pengganti/`

| Prefab | Isi |
|---|---|
| `PohonAsem` | pohon asem besar, tinggi sekitar 7 m, dahan atas |
| `Pacul` | cangkul |
| `Galah` | galah bambu panjang |
| `KendiTanahMerah` | kendi berisi tanah merah (Media 5) |
| `KainKafan` | kain kafan berajah di dahan (Media 6) |
| `NisanKayu` | nisan kayu |
| `NisanHadi` | nisan makam ayah Raka |
| `GundukanMakam` | gundukan tanah makam |
| `Kamboja` | pohon kamboja |
| `GerbangMakam` | gerbang makam |
| `PakLasno` | penjaga makam |
| `Anjing` | anjing kampung penjaga rumah Pakde |
| `MangkukAnjing` | mangkuk anjing |
| `PotBunga` | pot bunga besar di teras Pakde |
| `MejaKerjaLaci` | meja kerja berlaci |
| `RumahPakdeLuar` | rumah Pakde dari luar (dinding batu putih, teras) |
| `PagarBesi` | pagar besi |
| `GelasTeh` | gelas teh |
| `SisaMakanan` | sisa makanan untuk anjing |
| `TumpukanTanah` | tumpukan tanah galian |

### Suara di `Assets/_Project/Audio/SFX/`

(S) = ada WAV sintetis pengganti; hapus WAV dan `.meta` sebelum menaruh rekaman bernama sama, lalu jalankan `bash tools/normalisasi-audio.sh`.

| Nama file (tanpa akhiran) | Isi |
|---|---|
| `GaliTanah` (S) | pacul menggali tanah |
| `KainJatuh` (S) | kain digapai galah lalu jatuh |
| `SuasanaMakam` | angin, jangkrik jauh, burung hantu (Ambience, loop) |
| `AnjingMenggonggong` (S) | gonggongan |
| `AnjingMenggeram` | geram pelan (loop 3D) |
| `AnjingMakan` | mengunyah |
| `LaciDibuka` (S) | kunci diputar, laci ditarik |
| `GelasDiletakkan` | gelas teh di meja |
