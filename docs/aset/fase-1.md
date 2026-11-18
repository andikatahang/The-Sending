# Daftar Aset Fase 1 (Prolog + Malam 1)

Dikerjakan 19–21 Nov. Cari mulai sekarang. Aturan sama dengan `fase-0.md`: catat lisensi di `KREDIT.md`, beri ✗ jika tidak ketemu (Claude membuat penggantinya).

Model: utamakan **low poly** (di bawah 5.000 segitiga per benda, karakter di bawah 15.000), format FBX atau GLB. Gaya PSX akan menyamakan tampilannya.

Sumber: **SF** Sketchfab (filter *Downloadable*, lisensi CC0 atau CC-BY) · **AS** Unity Asset Store (gratis) · **MX** Mixamo (karakter + animasi) · **PH** Poly Haven · **AC** ambientCG · **FS** Freesound · **PX** Pixabay

## Lingkungan

| Nama | Kata kunci (EN / ID) | Sumber | Tujuan | Status |
|---|---|---|---|---|
| Rumah kayu Jawa (limasan/joglo), interior | `javanese house`, `joglo`, `limasan`, `traditional wooden house interior asia` / rumah limasan | SF, AS | `Art/Models/RumahKeluarga/` | |
| Tiang kayu besar (saka guru) | `wooden pillar`, `wood column carved` / saka guru | SF | `Art/Models/RumahKeluarga/` | |
| Gapura desa | `village gate`, `indonesian gate`, `gapura` | SF | `Art/Models/Desa/` | |
| Jembatan bambu | `bamboo bridge` / jembatan bambu | SF, AS | `Art/Models/Desa/` | |
| Rumpun bambu | `bamboo cluster`, `bamboo forest low poly` | SF, AS | `Art/Models/Desa/` | |
| Lampu jalan tua | `old street lamp`, `village lamp post` | SF, AS | `Art/Models/Desa/` | |
| Rumah kecil berjendela (luar rumah Bu Sumi) | `small village house`, `asian hut window` | SF, AS | `Art/Models/Desa/` | |
| Tekstur: papan kayu, dinding anyaman bambu (gedek), tanah, genteng | `wood planks`, `woven bamboo wall`, `dirt`, `clay roof tiles` | PH, AC | `Art/Textures/` | |

## Perabot dan benda

| Nama | Kata kunci (EN / ID) | Sumber | Status |
|---|---|---|---|
| Ranjang kayu tua | `old wooden bed` | SF, AS | |
| Lemari kayu tua (lemari ayah) | `old wooden wardrobe`, `antique cabinet` | SF, AS | |
| Gembok angka 4 digit | `combination padlock`, `number lock` | SF | |
| Foto keluarga berbingkai | `picture frame`, `old photo frame` | SF, AS | |
| Jam dinding bandul | `pendulum wall clock` | SF | |
| Kalender dinding | `wall calendar` | SF | |
| Meja dan kursi kayu | `wooden table chair rustic` | SF, AS | |
| Buku harian tua | `old diary`, `leather notebook` | SF | |
| Map dokumen | `document folder`, `paper folder` | SF | |
| Obat dan sachet oralit | `medicine box`, `pill pack`, `sachet` | SF | |
| Paku berkarat | `rusty nails` | SF | |
| HP di tangan (opsional) | `smartphone hand first person` | SF, AS | |

## Karakter (model + animasi)

Model bisa dari Sketchfab atau Asset Store, lalu di-rig dan diberi animasi di Mixamo. Pakaian sederhana sehari-hari di desa Jawa.

| Tokoh | Kata kunci | Animasi Mixamo yang dibutuhkan | Status |
|---|---|---|---|
| Laras (16, siswi SMA, di rumah) | `asian teenage girl casual`, `low poly girl character` | Idle, Talking, Crying, Praying (berlutut) | |
| Bidan Wulan (30, bidan) | `nurse character`, `female doctor low poly` | Idle, Talking | |
| Bu Ratmi (47, sakit, di ranjang) | `middle aged asian woman`, `old woman character` | Lying Down/Sleeping, Seizure atau Lying Struggle | |
| Siluet Bu Sumi di jendela | `old woman character` (boleh sama dengan Bu Ratmi, beda rambut/warna) | Idle | |

## Audio

| Nama | Kata kunci (EN / ID) | Sumber | Catatan | Status |
|---|---|---|---|---|
| Gadis menangis di telepon | `girl crying phone`, `teen girl sobbing` | FS, PX | layar hitam Prolog | belakangan (suara tokoh ditunda) |
| Mobil menjauh | `car drive away`, `car engine pass by` / mobil pergi | FS, PX | | |
| Azan magrib dari kejauhan | `adhan distant`, `call to prayer far away` | FS, PX | hormati: pakai rekaman utuh, tanpa diedit aneh | |
| Pintu kayu berderit | `wooden door creak` | FS | | |
| Jam dinding berdentang | `wall clock chime`, `grandfather clock strike` | FS | | |
| Perempuan batuk dan tersedak | `woman coughing choking`, `woman vomiting` | FS | cutscene pukul 23.00 | |
| Paku jatuh ke lantai | `nails dropping floor`, `metal small objects drop` | FS | | |
| Listrik padam | `power outage`, `electric hum cut off` | FS | | |
| Langkah berat di atap | `footsteps on roof`, `heavy steps above ceiling` | FS | | |
| Bisikan doa perempuan | `female whisper prayer`, `whispering` | FS | Laras berdoa | belakangan (suara tokoh ditunda) |
| Musik drone horor | `dark ambient drone`, `horror drone loop` | PX, FS | latar tegang, bisa di-loop | |

## Nama file dan prefab yang dipakai kode Fase 1

Kode Fase 1 berjalan dengan aset pengganti. Untuk memakai aset asli, taruh atau tukar di jalur berikut; tidak ada kode yang perlu diubah. Skrip pembuat **tidak menimpa** file yang sudah ada.

**Suara** (`.wav`, `.ogg`, atau `.mp3`; nama dasar harus persis). (S) = sudah ada WAV sintetis; hapus file sintetis dan `.meta`-nya sebelum menaruh yang asli.

| Aset di daftar atas | Jalur |
|---|---|
| Mobil menjauh (nama file tetap `MotorMenjauh`) | `Assets/_Project/Audio/SFX/MotorMenjauh` |
| Azan magrib | `Assets/_Project/Audio/SFX/AzanMagrib` |
| Pintu kayu berderit | `Assets/_Project/Audio/SFX/PintuBerderit` |
| Jam dinding berdentang (S) | `Assets/_Project/Audio/SFX/DentangJam` |
| Perempuan batuk dan tersedak | `Assets/_Project/Audio/SFX/BatukTersedak` |
| Paku jatuh ke lantai (S) | `Assets/_Project/Audio/SFX/PakuJatuh` |
| Listrik padam (S) | `Assets/_Project/Audio/SFX/ListrikPadam` |
| Langkah berat di atap (S) | `Assets/_Project/Audio/SFX/LangkahDiAtap` |
| Musik drone horor (S) | `Assets/_Project/Audio/Music/DroneHoror` |

**Prefab** di `Assets/_Project/Prefabs/Pengganti/` (buka prefab, ganti anak-anaknya dengan model asli; depan = +Z, titik nol di lantai; jangan ganti nama file):

| Prefab | Menggantikan |
|---|---|
| `Laras`, `Laras_Berdoa` | Laras berdiri; Laras berlutut berdoa |
| `BidanWulan` | Bidan Wulan |
| `BuRatmi_Berbaring` | Bu Ratmi telentang di ranjang (kepala ke arah -Z) |
| `BuSumi_Siluet` | siluet Bu Sumi di jendela |
| `Ojek` | cadangan saja: mobil pengantar memakai model `Art/Models/Desa/MobilLMPV.glb` (Sketchfab, diimpor glTFast); prefab ini dipakai jika model belum terimpor |
| `Ranjang`, `LemariAyah`, `Gembok`, `FotoKeluarga`, `JamDinding`, `Kalender`, `Meja`, `Kursi`, `Paku`, `Teplok`, `StopKontak`, `SakaGuru` | perabot dan benda rumah |
| `Gapura`, `JembatanBambu`, `RumpunBambu`, `LampuJalan`, `RumahKecil`, `RumahKeluargaLuar` | jalan masuk desa |

**Tekstur:** pasang pada material di `Assets/_Project/Art/Materials/Pengganti/` (`KayuTua`, `KayuGelap`, `Gedek`, `Tanah`, `Genteng`, `Bambu`, `Kain`). Dinding, lantai, dan plafon rumah memakai material itu.

**Tidak dipakai di Fase 1** (tidak perlu dicari sekarang): model buku harian, map dokumen, obat dan sachet (ketiganya hanya tampil sebagai teks di inventaris), HP di tangan. Suara tokoh (gadis menangis di telepon, bisikan doa perempuan) **belakangan**: voice acting ditunda, semua percakapan teks saja.
