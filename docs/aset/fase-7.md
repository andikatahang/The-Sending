# Daftar Aset Fase 7 (Malam 7 + Ending)

Dikerjakan 2–4 Des. Cari mulai sekarang. Aturan sama dengan `fase-6.md`: catat lisensi di `KREDIT.md`, beri ✗ jika tidak ketemu (Claude membuat penggantinya).

Model: utamakan **low poly** (di bawah 5.000 segitiga per benda, karakter di bawah 15.000), format FBX atau GLB. Paket `.zip`/`.rar` boleh ditaruh apa adanya di `Assets/_Project/Art/ThirdParty/<nama>/`.

Sumber: **SF** Sketchfab (filter *Downloadable*, CC0 atau CC-BY) · **AS** Unity Asset Store (gratis) · **MX** Mixamo · **PH** Poly Haven · **AC** ambientCG · **FS** Freesound · **PX** Pixabay

## Lingkungan

| Nama | Kata kunci (EN / ID) | Sumber | Tujuan | Status |
|---|---|---|---|---|
| Sawah berkabut (petak padi, pematang) | `rice paddy low poly`, `rice field night fog` / sawah, pematang | SF, AS | `Scene_Watuireng` | |
| Jalan setapak tanah di antara sawah | `dirt path`, `village footpath` / jalan setapak desa | SF, PH | `Scene_Watuireng` | |
| Sanggar/rumah Mbah Wiryo (rumah kayu tua, dalam penuh sesaji) | `old javanese house`, `shaman hut`, `dukun house interior` / rumah dukun, sanggar | SF | `Scene_Watuireng` | |
| Kandang ayam cemani | `chicken coop low poly`, `wooden cage` / kandang ayam | SF | `Scene_Watuireng` | |
| Lingkaran garam di lantai (decal/tekstur) | `salt circle`, `ritual circle decal` / lingkaran garam | SF, AC | `Scene_RumahKeluarga` | |
| Pemakaman dengan payung dan gerimis (dipakai ulang `indonesia-grave`) | `rain particle`, `umbrella low poly` / hujan, payung hitam | AS, SF | `Scene_Epilog` | |
| Kamar mandi bercermin (sudah ada `the-bathroom`; ganti tekstur bermerek nyata) | `bathroom mirror` / kamar mandi, cermin | — | `Scene_Epilog` (Balas Dendam) | |
| Latar pagi/siang (langit cerah, cahaya hangat) | `morning sky HDRI`, `sunrise sky` | PH | `Scene_Epilog` (Volume profile pagi) | |
| Puing rumah Bu Sumi (dipakai ulang `PuingRumahKecil` dari Fase 6) | — | — | `Scene_Epilog` (Fitnah) | |
| Latar rumah Pakde (dipakai ulang dari Fase 5) | — | — | `Scene_Epilog` | |

## Benda

| Nama | Kata kunci (EN / ID) | Sumber | Status |
|---|---|---|---|
| Keris (sudah ada di `Art/ThirdParty/keris`) | — | — | ada |
| Kamar mandi (sudah ada di `Art/ThirdParty/the-bathroom`; tekstur bermerek diganti) | — | — | ada |
| Sesaji (nampan, bunga, nasi tumpeng kecil, rokok) | `javanese offering`, `sesajen`, `tumpeng low poly` / sesaji | SF | |
| Lilin sesaji (5 lilin berderet, bisa dipadamkan) | `candle low poly`, `candles row` / lilin | SF, AS | |
| Kendi induk (tempat M7) | `clay pitcher`, `kendi`, `terracotta jug` / kendi tanah liat | SF | |
| Lontar (daun lontar bertulis mantra pambalik) | `palm leaf manuscript`, `lontar` / naskah lontar | SF | |
| Buku catatan Mbah Wiryo (B8, Halaman 8) | `old notebook`, `ledger book low poly` / buku tulis tua | SF | |
| Kemenyan (pedupaan dan asap) | `incense burner`, `incense smoke particle` / kemenyan, pedupaan | SF, AS | |
| Cermin (kamar mandi, retak boleh) | `wall mirror low poly`, `broken mirror` / cermin | SF | |
| Api besar dan asap (dipakai ulang `ApiBesar`, `AsapHitam` dari Fase 6) | — | — | dipakai ulang |

## Karakter (model + animasi)

| Tokoh | Kata kunci | Animasi Mixamo yang dibutuhkan | Status |
|---|---|---|---|
| Mbah Wiryo (blangkon, baju hitam, akik; sudah ada sebagai tokoh PSX) | — | Walking, Running (kejar), Idle | ada |
| Bripka Anton (polisi, seragam coklat) | `indonesian police uniform`, `policeman low poly` / polisi | Idle, Talking | |
| Bidan Wulan (dipakai ulang dari Fase 1) | — | Idle | dipakai ulang |
| Raka, Laras, Bu Sumi, Pak Joko (dipakai ulang untuk epilog) | — | Idle, Walking | dipakai ulang |

## Audio

| Nama | Kata kunci (EN / ID) | Sumber | Catatan | Status |
|---|---|---|---|---|
| Ayam berkokok (cemani dekat Raka) | `rooster crow`, `chicken alarm` | FS | sudah ada di AS4 (Fase 3); cek dan pakai ulang | |
| Kemenyan dan api besar | `incense burning`, `big fire crackle` | FS | ritual dan sanggar | |
| Gerimis pemakaman | `light rain on umbrella`, `rain cemetery` | FS | epilog | |
| Musik penutup (sedih, tenang) | `sad piano ambient`, `ending theme melancholic` | FS, PX | grup Music, epilog Sejati | |
| Kejaran Mbah Wiryo (musik dan langkah) | `chase horror`, `heavy footsteps gravel` | FS, PX | grup Music dan SFX | |
| Kokok dan angin pagi | `morning birds`, `village morning ambience` | FS | epilog | |

## Catatan

- `kuntilanak-merah-scary` di `Art/ThirdParty` tidak punya tujuan di gim. Hapus jika tetap tidak dipakai (lihat `fase-3.md`).
- `keris` dan `the-bathroom` sudah ada (untracked); `the-bathroom` memakai tekstur bermerek nyata, ganti sebelum build rilis.
- Lapis B (boleh menyusul): kamar mandi bercermin, ayam cemani berkokok, Halaman 8, `keris`.
- Puing dan api dari Fase 6 dipakai ulang di `Scene_Epilog`.
