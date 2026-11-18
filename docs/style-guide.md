# Style Guide The Sending (Retro PSX / low-fi)

**Status: DRAF.** Dikunci setelah tim menyelesaikan Daftar Cek Play Mode Fase 0C. Setelah dikunci, perubahan hanya boleh atas persetujuan tim, dan setiap fase ditutup dengan pengecekan gaya terhadap dokumen ini.

Semua nilai render dipasang oleh skrip, bukan lewat Inspector: menu **The Sending > Pasang Gaya PSX** (`Assets/_Project/Scripts/Editor/PembuatGayaPsx.cs`). Untuk mengubah nilai: ubah konstanta di skrip itu, jalankan menunya, lalu perbarui tabel di bawah.

## Render

| Aspek | Nilai | Tempat |
|---|---|---|
| Render Scale | 0.4 | `Assets/Settings/PC_RPAsset.asset` (dan Mobile) |
| Upscaling Filter | Point (Nearest-Neighbor) | aset URP |
| Anti-aliasing | MSAA mati, AA kamera None | aset URP, kamera |
| Full-screen pass | Kuantisasi 8 bit per kanal + dither Bayer 4×4 (kuat 1.5), dipilih tim saat cek Play Mode supaya gradasi halus, dihitung di ruang sRGB, sebelum post-processing (injection point `BeforeRenderingPostProcessing`, 550) pada resolusi render-scale | Shader Graph `Assets/_Project/Art/Shaders/PSX_Layar.shadergraph` (URP Fullscreen) + Custom Function `PSX_Layar.hlsl`, material `PSX_Layar.mat` di folder yang sama (slider BitWarna = 8, KuatDither = 1.5; nilai material yang berlaku), fitur renderer `PSX_Layar` |
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
| Kabut Prolog (pengecualian) | Di `Scene_Desa` selama Prolog kabut mulai 0.02, lalu naik ke 0.06 dalam 20 detik setelah azan (`AlurProlog.KabutSebelumMagrib`, `DetikKabutTurun`). Semua scene dan bab lain 0.06 sejak awal |
| Latar kamera | Solid Color `#1A1F24` (sama dengan kabut) |
| Ambient | Flat `#05070A` |
| Cahaya | Realtime. Senter = spotlight dengan bayangan. Bake lightmap opsional di Fase 8 |

Setiap scene baru memanggil `PembuatGayaPsx.TerapkanKeScene()` dan `SiapkanKamera()` dari skrip pembuatnya.

## Palet semantik

| Arti | Warna |
|---|---|
| Hangat = aman (lampu teplok) | `#FFB46B` |
| Lampu listrik rumah (padam sejak cutscene Malam 1 pukul 23.00) | `#FFD9A0` |
| Dingin = ancaman (cahaya bulan) | `#6D86A8` |
| Karat dan darah; sorotan tombol menu | `#5A1A12` |
| Teks UI | `#E6E0D1` |
| Kayu tua | cokelat gelap |
| Sosok Santet | hitam arang; kain kafan diwarnai `#4A4440` (material `Kafan`), cahaya bulan `#6D86A8` di luar celah gudang |

## Tekstur

Semua tekstur di `Assets/_Project/Art/` otomatis diimpor dengan ukuran maksimal 256, filter Point, tanpa mipmap (`PrasetelTeksturPsx`, hanya pada impor pertama). Tekstur di luar folder itu tidak ikut aturan ini, jadi jangan menaruh tekstur gim di luar `Art/`.

Aset pihak ketiga ada di `Assets/_Project/Art/ThirdParty/<Paket>/` (EssentialNaturePack, HouseInteriorPack, TraditionalHouseModular, YughuesConcrete; isi yang tidak dipakai sudah dihapus). Karena datang dengan `.meta` sendiri, prasetel di atas tidak berlaku otomatis. Setelah menambah atau memperbarui paket, jalankan **The Sending > Siapkan Aset Luar** (`PembuatAsetLuar`, aman diulang): material built-in diubah ke URP Lit (tekstur, warna, normal map, cutout/transparan, emisi dipertahankan), semua material diberi Smoothness 0, dan semua tekstur diberi ukuran maks 256, Point, tanpa mipmap (normal map tetap bertipe normal map). Daun pohon juga 256; masih terbaca di pratinjau.

TraditionalHouseModular hanya menyimpan varian `512 - native` (varian 2k dihapus). Tekstur tileable-4 "512" sebenarnya 2048 px, tetapi diimpor 256.

Pengecualian: gambar UI aksara Jawa di `Assets/_Project/Art/UI/Aksara/` (dibuat `tools/render-aksara.sh`). Suku 256 px, tabel Halaman 2 diimpor sampai 1024 px (`PembuatDataFase2`) supaya terbaca. Gambar UI tidak terkena render scale PSX karena digambar di Canvas overlay. Gambar belum ada sampai font Noto Sans Javanese dipasang; sementara panel memakai teks pengganti seperti `[RA]`.

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

Pass `PSX_Layar` berjalan sebelum post-processing pada resolusi render-scale, jadi upscale akhir tetap Point (piksel besar tajam). HDR menyala, jadi `saturate()` memotong highlight di atas 1 sebelum tonemapping. Tonemapping Neutral, saturasi -30, grain, dan vignette berjalan di atas level 8 bit (grain seukuran piksel layar). Jika highlight terlihat kusam atau abu-abu, coba Tonemapping None atau matikan HDR (juga lebih ringan).

## Aset pengganti (sejak Fase 1)

Selama aset asli belum ada, gim memakai pengganti buatan skrip (`PembuatPengganti`, `PembuatSuaraPengganti`): material polos di `Assets/_Project/Art/Materials/Pengganti/`, prefab dari primitif di `Assets/_Project/Prefabs/Pengganti/`, dan WAV sintetis (lima di Fase 1, enam di Fase 2) di `Assets/_Project/Audio/SFX/` dan `Audio/Music/`. Skrip tidak menimpa file yang sudah ada. Cara menukar dengan aset asli: `docs/aset/fase-1.md`, bagian "Nama file dan prefab yang dipakai kode Fase 1". Daftar Fase 2: `docs/aset/fase-2.md`, bagian "Nama file dan prefab yang dipakai kode Fase 2".

Prefab aset asli di `Assets/_Project/Prefabs/Asli/` (dibuat `PembuatAsetAsli`) menggantikan prefab pengganti bernama sama. Perabot nappin memakai material kayu pengganti, bukan warna modernnya. Pohon hutan memakai LOD0 hanya sampai sekitar 20 m (dengan lodBias 2 kualitas PC); lebih jauh papan silang LOD1 di dalam kabut. Sekitar 100-200 ribu segitiga terlihat di Scene_Desa.

Aturan gaya untuk pengganti dan aset asli sama: warna mengikuti palet di atas, tanpa kilap (Smoothness 0), dan tekstur mengikuti prasetel PSX.

Scene gameplay (`Scene_Desa`, `Scene_RumahKeluarga`) dibuat skrip dan ditimpa setiap **The Sending > Bangun Ulang Semua**. Tata letak diubah di skrip pembuatnya, bukan di scene.
