# Daftar Aset Fase 6 (Malam 6: Obor)

Dikerjakan 30 Nov–1 Des. Cari mulai sekarang. Aturan sama dengan `fase-5.md`: catat lisensi di `KREDIT.md`, beri ✗ jika tidak ketemu (Claude membuat penggantinya).

Model: utamakan **low poly** (di bawah 5.000 segitiga per benda, karakter di bawah 15.000), format FBX atau GLB. Paket `.zip`/`.rar` boleh ditaruh apa adanya di `Assets/_Project/Art/ThirdParty/<nama>/`.

Sumber: **SF** Sketchfab (filter *Downloadable*, CC0 atau CC-BY) · **AS** Unity Asset Store (gratis) · **MX** Mixamo · **PH** Poly Haven · **AC** ambientCG · **FS** Freesound · **PX** Pixabay

## Lingkungan

| Nama | Kata kunci (EN / ID) | Sumber | Tujuan | Status |
|---|---|---|---|---|
| Halaman depan rumah Bu Sumi untuk sidang (tanah lapang kecil, pagar bambu) | `village yard night`, `bamboo fence low poly` / halaman rumah desa, pagar bambu | SF, AS | `Scene_Desa`, depan rumah Bu Sumi | |
| Rumah Bu Sumi versi terbakar (dinding hangus, atap runtuh) | `burnt house`, `burned wooden hut` / rumah terbakar, rumah hangus | SF | `Scene_Desa` (cabang sidang gagal) | |
| Tekstur kayu hangus dan abu | `charred wood`, `burnt wood`, `ash ground` | PH, AC | `Art/Textures/` | |

## Benda

| Nama | Kata kunci (EN / ID) | Sumber | Status |
|---|---|---|---|
| Obor bambu (dipegang warga, api menyala) | `bamboo torch`, `tiki torch low poly` / obor bambu | SF, AS | |
| Efek api dan asap (partikel) | `fire particle`, `smoke particle URP` / api, asap | AS | |
| Jeriken minyak tanah | `jerry can`, `kerosene can` / jeriken, kaleng minyak | SF | |
| Ranjang Ibu dengan selimut kusut (kerasukan) | dipakai ulang dari rumah keluarga | — | |

## Karakter (model + animasi)

| Tokoh | Kata kunci | Animasi Mixamo yang dibutuhkan | Status |
|---|---|---|---|
| Kerumunan warga (4–6 orang, variasi baju) | `asian villager`, `crowd low poly people` | Idle, Angry Gesture, Shouting, Holding Torch | |
| Pak Joko (dipakai lagi dari Fase 4) | — | Pointing, Talking | |
| Bu Sumi (dipakai lagi; pose takut) | — | Scared, Crying | |
| Ibu kerasukan (pose duduk kaku) | dipakai ulang model Ibu | Sitting Rigid / Possessed | |

## Audio

| Nama | Kata kunci (EN / ID) | Sumber | Catatan | Status |
|---|---|---|---|---|
| Kerumunan marah | `angry crowd`, `mob shouting` | FS | sidang warga, grup Ambience | |
| Api obor berkobar | `torch fire crackle` | FS | loop | |
| Rumah terbakar | `house fire burning`, `wood fire large` | FS | cabang gagal | |
| Suara berat Ibu kerasukan (geraman, bukan kata) | `demonic growl`, `possessed breathing` | FS | suara tokoh tetap teks; ini hanya efek | |
| Langkah berlari di dalam rumah (Laras masuk) | `running footsteps wood floor` | FS | 21.00 | |
| Kentongan bertalu-talu (tanda bahaya) | `kentongan alarm`, `wooden slit drum fast` | FS | dipakai ulang `Kentongan` jika tidak ada | |
| Musik sidang tegang | `tense courtroom`, `suspense drone` | FS, PX | grup Music | |

## Catatan

- Sidang Warga memakai `Core.SidangWarga` (sudah ada). Panel pilih bukti memakai Papan Bukti; tidak butuh aset khusus selain UI.
- Rumah Pakde (Fase 5) dibuka juga di awal Malam 6 (GS 595); aset dari `fase-5.md` dipakai ulang.

## Nama file dan prefab yang dipakai kode Fase 6

Nama ini dipakai persis oleh kode (Task 7). Jika menukar dengan aset asli, pertahankan nama dan letaknya. Tokoh di `Prefabs/Pengganti/` selalu dibuat ulang oleh `PembuatPengganti.Tokoh`; ganti lewat `DaftarTokoh`, bukan prefab.

| Nama | Isi | Folder |
|---|---|---|
| `Warga3`, `Warga4`, `Warga5` | tokoh warga baru (baju berbeda, satu perempuan berkerudung) | `Prefabs/Pengganti/` |
| `PakJoko_Obor` | sama dengan `PakJoko`, obor di tangan kanan | `Prefabs/Pengganti/` |
| `Obor` | batang bambu 0,9 m, kepala kain bercahaya, `Light` jingga, `ApiBerkedip` | `Prefabs/Pengganti/` |
| `ApiBesar` | `ParticleSystem` api 40 partikel + `Light` jingga + `ApiBerkedip` | `Prefabs/Pengganti/` |
| `AsapHitam` | `ParticleSystem` asap gelap 20 partikel | `Prefabs/Pengganti/` |
| `PuingRumahKecil` | balok hangus rendah: dinding runtuh, tiang miring, atap jatuh | `Prefabs/Pengganti/` |
| `KacaGelap` | balok tipis penutup jendela rumah Bu Sumi (material `KacaGelap`) | dibuat di scene oleh `PembuatDesa` |
| `Hangus` | material hitam kecoklatan, kasar | `Art/Materials/Pengganti/` |
| `KacaGelap` (material) | hitam kebiruan, smoothness 0,6 | `Art/Materials/Pengganti/` |
| `KerumunanMarah` | gumam dan teriakan warga (loop 3D, Ambience) | `Audio/SFX/` |
| `TeriakBakar` | teriakan "Bakar!" bersahutan | `Audio/SFX/` |
| `OborBerkobar` | api obor (loop 3D) | `Audio/SFX/` |
| `RumahTerbakar` | api besar, kayu berderak (loop 3D) | `Audio/SFX/` |
| `GeramIbu` | geram berat tanpa kata (efek) | `Audio/SFX/` |
| `LangkahLariKayu` | Laras berlari di lantai kayu | `Audio/SFX/` |
| `Kentongan` | dipakai ulang jika sudah ada | `Audio/SFX/` |
| `MusikSidang` | drone tegang (jika tidak ada: slot `MusikTegang`) | `Audio/Music/` |

Suara baru dijalankan lewat `bash tools/normalisasi-audio.sh`, lisensinya dicatat di `KREDIT.md`.
