# Style Guide The Sending (Retro PSX / low-fi)

**Status: DRAF.** Dikunci setelah tim menyelesaikan Daftar Cek Play Mode Fase 0C. Setelah dikunci, perubahan hanya boleh atas persetujuan tim, dan setiap fase ditutup dengan pengecekan gaya terhadap dokumen ini.

Semua nilai render dipasang oleh skrip, bukan lewat Inspector: menu **The Sending > Pasang Gaya PSX** (`Assets/_Project/Scripts/Editor/PembuatGayaPsx.cs`). Untuk mengubah nilai: ubah konstanta di skrip itu, jalankan menunya, lalu perbarui tabel di bawah.

## Render

| Aspek | Nilai | Tempat |
|---|---|---|
| Render Scale | 0.4 | `Assets/Settings/PC_RPAsset.asset` (dan Mobile) |
| Upscaling Filter | Point (Nearest-Neighbor) | aset URP |
| Anti-aliasing | MSAA mati, AA kamera None | aset URP, kamera |
| Full-screen pass | Kuantisasi 5 bit per kanal + dither Bayer 4×4 (kuat 1, rentang slider 0–2), dihitung di ruang sRGB, sebelum post-processing (injection point `BeforeRenderingPostProcessing`, 550) pada resolusi render-scale | Shader Graph `Assets/_Project/Art/Shaders/PSX_Layar.shadergraph` (URP Fullscreen) + Custom Function `PSX_Layar.hlsl`, material `PSX_Layar.mat` di folder yang sama (slider Bit Warna = 5, Kuat Dither = 1), fitur renderer `PSX_Layar` |
| SSAO | Mati | fitur renderer `ScreenSpaceAmbientOcclusion` |

## Post-processing

Satu profil bersama: `Assets/_Project/Settings/PSX_Volume.asset`, dipasang sebagai Volume Profile bawaan aset URP sehingga berlaku di semua scene tanpa objek Volume. Kamera wajib menyalakan Post Processing (`PembuatGayaPsx.SiapkanKamera`).

| Efek | Nilai |
|---|---|
| Film Grain | tipe Thin 1, intensitas 0.4, response 0.8 |
| Vignette | hitam, intensitas 0.35, kehalusan 0.4 |
| Color Adjustments | saturasi -30 |
| Tonemapping | Neutral |

Epilog siang/pagi: Volume lokal terpisah di scene epilog, tetap memakai kabut dan pass PSX yang sama.

## Kabut dan cahaya

| Aspek | Nilai |
|---|---|
| Kabut | Exponential Squared, `#1A1F24`, kepadatan 0.06 (jarak pandang sekitar 15 m) |
| Latar kamera | Solid Color `#1A1F24` (sama dengan kabut) |
| Ambient | Flat `#05070A` |
| Cahaya | Realtime. Senter = spotlight dengan bayangan. Bake lightmap opsional di Fase 8 |

Setiap scene baru memanggil `PembuatGayaPsx.TerapkanKeScene()` dan `SiapkanKamera()` dari skrip pembuatnya.

## Palet semantik

| Arti | Warna |
|---|---|
| Hangat = aman (lampu teplok) | `#FFB46B` |
| Dingin = ancaman (cahaya bulan) | `#6D86A8` |
| Karat dan darah; sorotan tombol menu | `#5A1A12` |
| Teks UI | `#E6E0D1` |
| Kayu tua | cokelat gelap |

## Tekstur

Semua tekstur di `Assets/_Project/Art/` otomatis diimpor dengan ukuran maksimal 256, filter Point, tanpa mipmap (`PrasetelTeksturPsx`, hanya pada impor pertama). Tekstur di luar folder itu tidak ikut aturan ini, jadi jangan menaruh tekstur gim di luar `Art/`.

## Font

| Pemakaian | Font | File |
|---|---|---|
| UI dan subtitle | VT323 | `Assets/_Project/Art/Fonts/Resources/VT323-Regular.ttf` |
| Buku harian (tulisan tangan) | Kalam | `Assets/_Project/Art/Fonts/Resources/Kalam-Regular.ttf` |

Kode mengambil font hanya lewat `UiPabrik.Font` dan `UiPabrik.FontTulisan`. Kedua font sudah terpasang (lisensi OFL, tercatat di `docs/aset/KREDIT.md`). Jika file hilang, dipakai font bawaan Unity dan muncul peringatan di Console.

## Audio

Mixer: `Assets/_Project/Audio/Resources/MixerUtama.mixer`. Grup: `Master` > `Music`, `SFX`, `Ambience`, `Dialog`. Parameter terekspos: `VolMaster`, `VolMusic`, `VolSFX`, `VolAmbience`, `VolDialog`. Setiap `AudioSource` wajib diarahkan ke salah satu grup, supaya pengaturan volume berlaku.

## UI

Dibangun lewat kode (`UiPabrik`), kanvas acuan 1920×1080. Panel dalam gim (dialog, Jurnal) memakai keyboard. Menu (utama, jeda, pengaturan) memakai tombol yang bisa mouse dan keyboard.

## Catatan PSX

Pass `PSX_Layar` berjalan sebelum post-processing pada resolusi render-scale, jadi upscale akhir tetap Point (piksel besar tajam). HDR menyala, jadi `saturate()` memotong highlight di atas 1 sebelum tonemapping. Tonemapping Neutral, saturasi -30, grain, dan vignette berjalan di atas level 5 bit (grain seukuran piksel layar). Jika highlight terlihat kusam atau abu-abu, coba Tonemapping None atau matikan HDR (juga lebih ringan).
