# Daftar Aset Fase 8 (Polish & Rilis)

Dikerjakan 5–6 Des. Ini daftar pengganti yang masih tersisa setelah Fase 7. Aturan sama dengan `fase-7.md`: catat lisensi di `KREDIT.md`, beri ✗ jika tidak ketemu (Claude membuat penggantinya atau dicatat di layar kredit).

Model: utamakan **low poly** (di bawah 5.000 segitiga per benda, karakter di bawah 15.000), format FBX atau GLB. Paket `.zip`/`.rar` boleh ditaruh apa adanya di `Assets/_Project/Art/ThirdParty/<nama>/`.

Sumber: **SF** Sketchfab (filter *Downloadable*, CC0 atau CC-BY) · **AS** Unity Asset Store (gratis) · **MX** Mixamo · **PH** Poly Haven · **AC** ambientCG · **FS** Freesound · **PX** Pixabay

## Lingkungan dan benda

| Nama | Kata kunci (EN / ID) | Sumber | Dari fase | Status |
|---|---|---|---|---|
| Obor (kain, kayu, api) | `torch low poly`, `wooden torch` / obor bambu | SF, AS | 6 | |
| Api dan asap (partikel; material partikel masih bisa tampil kotak) | `fire particle`, `smoke particle` / api, asap | AS, SF | 6 | |
| Puing rumah Bu Sumi | `burnt house ruins`, `rubble low poly` / puing rumah terbakar | SF | 6 | |
| Sanggar/rumah Mbah Wiryo (dalam penuh sesaji) | `old javanese house`, `shaman hut` / rumah dukun, sanggar | SF | 7 | |
| Kandang ayam cemani | `chicken coop low poly`, `wooden cage` / kandang ayam | SF | 7 | |
| Ayam cemani (hitam) | `black chicken low poly`, `ayam cemani` | SF | 7 | |
| Lilin sesaji (5 lilin berderet, bisa dipadamkan) | `candle low poly`, `candles row` / lilin | SF, AS | 7 | |
| Kendi induk (tempat M7) | `clay pitcher`, `kendi`, `terracotta jug` / kendi tanah liat | SF | 7 | |
| Lontar (mantra pambalik) | `palm leaf manuscript`, `lontar` / naskah lontar | SF | 7 | |
| Buku catatan Mbah Wiryo (B8) | `old notebook`, `ledger book low poly` / buku tulis tua | SF | 7 | |
| Cermin kamar mandi (retak boleh) | `wall mirror low poly`, `broken mirror` / cermin | SF | 7 | |
| Hujan pemakaman (partikel hujan dan payung) | `rain particle`, `umbrella low poly` / hujan, payung hitam | AS, SF | 7 | |

## Karakter

| Tokoh | Kata kunci | Animasi Mixamo yang dibutuhkan | Dari fase | Status |
|---|---|---|---|---|
| Pak Joko dan warga (pengganti) | `villager low poly`, `indonesian man` / warga desa | Idle, Walking, Talking | 6 | |
| Bripka Anton (polisi, seragam coklat) | `indonesian police uniform`, `policeman low poly` / polisi | Idle, Talking | 7 | |
| Raka (pengganti jika masih kasar) | — | Idle, Walking, Running | 7 | |

## Nama file suara Fase 7 yang dicari kode

Taruh di `Assets/_Project/Audio/SFX/` (dua musik di `Assets/_Project/Audio/Music/`), lalu jalankan `bash tools/normalisasi-audio.sh [file...]`. Selama belum ada, bunyi tampil sebagai teks "(Suara: ...)".

- `KokokCemani`
- `ApiBesar`
- `Kemenyan`
- `GerimisPemakaman`
- `MusikPenutup` (Music)
- `MusikKejarWiryo` (Music)
- `BurungPagi`

## Catatan Fase 8

- Fase 8 mencakup balancing (tempo jam Malam 6 dan 7, kesulitan Sosok dan sidang) dan build rilis Windows (non-development, layanan/analytics Unity dimatikan, tes di laptop Windows). Lihat Bagian 7.1 dokumen desain.
- Lisensi semua aset di atas dan aset pihak ketiga lain diisi di `KREDIT.md`; aset pengganti yang tersisa dicatat di layar kredit.
- `the-bathroom` memakai tekstur bermerek nyata: ganti sebelum build rilis.
- Lapis B (boleh menyusul): cermin, ayam cemani berkokok, Halaman 8.
