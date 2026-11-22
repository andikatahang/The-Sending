# Props Dekor Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Pakai sebanyak mungkin props dari `Assets/Props TheSending/` untuk menggantikan visual pengganti dan mengisi lingkungan (rumah keluarga termasuk kamar mandi dan dapur, desa, rumah Bu Sumi, rumah Pakde, makam), tanpa mengubah logika atau interaksi.

**Architecture:** Kuncinya sudah ada di kode: `PembuatPengganti.Pasang(nama, ...)` lebih dulu memuat `Prefabs/Asli/<nama>.prefab` dan baru jatuh ke `Prefabs/Pengganti/<nama>.prefab`. Jadi **penggantian visual = membuat prefab Asli dengan nama yang sama**; skrip pembuat scene dan logika tidak disentuh. Dekor baru = prefab Asli bernama baru, dipasang di skrip pembuat scene lewat `PembuatPengganti.Pasang`. Semua prefab props dibuat di file baru `PembuatAsetAsli.Props.cs` (kelas `PembuatAsetAsli` dijadikan `partial`), memakai helper yang ada (`Prefab`, `Taruh`, `Vendor`, `Seragam`, `Kayu`).

**Tech Stack:** Unity 6000.6.4f1, URP 17.6, glTFast 6.21.0 (untuk .glb/.gltf), Git LFS, `bsdtar`.

## Global Constraints

- Scene dibuat skrip; jangan menyunting scene dengan tangan. Perubahan permanen masuk skrip pembuat.
- Editor Unity **harus tertutup** saat batchmode. Bangun ulang: `bash tools/bangun-semua.sh`. Test: `bash tools/run-editmode-tests.sh`.
- Logika/interaksi tidak berubah: objek logika (collider + skrip) tetap di posisi dan ukuran yang sama; prefab visual hanya anak.
- Jangan menghalangi NavMesh (agen radius 0,25 m) atau jalur pemain/pintu. Dekor lantai tanpa collider kecuali benda besar yang memang harus padat (lemari, kulkas, meja); celah jalan ≥ 0,9 m.
- Gaya PSX: tekstur lewat `PrasetelTeksturPsx` (maks 256, Point, tanpa mipmap; dilakukan otomatis oleh `PembuatAsetLuar` untuk semua tekstur di `Art/ThirdParty`). Material URP Lit Smoothness 0.
- Utamakan solusi sederhana. Tanpa interaksi baru.
- Commit `<type>: <deskripsi>`, **tanpa** baris `Co-Authored-By`. Jangan `git add -A`; tambahkan path secara eksplisit.
- Setelah "Bangun Ulang Semua", commit scene + `Assets/_Project/Timeline/*.playable` + `Assets/_Project/Prefabs/Asli/` + mesh baru di `Assets/_Project/Art/Models/Kubus/` bersama-sama.
- Model berat (> ~20.000 segitiga) dicatat di laporan task; jangan dipasang berulang lebih dari 2 kali per scene.

---

## Inventaris Aset (`Assets/Props TheSending/`)

| Aset | Model | Perlu ekstrak? | Tekstur | Catatan / pakai untuk |
|---|---|---|---|---|
| antique-wall-clock | `source/clock.zip` → `clock.obj` (150 KB) | Ya (zip) | 5 png (4K) di `textures/` | Ganti `JamDinding` |
| bathroom_accesories | `scene.gltf` + `scene.bin` | Tidak | jpeg/png 2K | Kamar mandi. CC-BY-4.0, gorzi |
| candle-low | `source/CandleLow.fbx` | Tidak | 4 png | Dekor lilin (bukan teplok, lihat Ambigu 1) |
| carpet | `source/Carpet_Sketchfab.fbx` | Tidak | 2 set jpg (0102, 0304) | Karpet ruang tengah, rumah Pakde |
| ceramic-floor-texture | — (tekstur saja) | Tidak | Poliigon 2K (+ `.tiff` displacement, `.mtlx`, preview: hapus) | Dinding/lantai kamar mandi |
| door-padlock-key | `source/padlock.zip` → `padlock.fbx` | Ya (zip) | png 4K | Ganti `Gembok` |
| floor-tile-toilet.jpg | — | Tidak | 1 jpg (4,3 MB) | Lantai kamar mandi |
| foto-keluarga.jpg | — | Tidak | 1 jpg | Ganti isi `FotoKeluarga` |
| house-texture.jpg | — | Tidak | 1 jpg (8 MB) | Dinding luar rumah Pakde/Bu Sumi (lihat Task 5) |
| kitchen-decor-pack | `source/KitchenDecorPack.fbx` (7,8 MB, banyak objek) | Tidak | **338 file, 115 MB** | Pilih 4–6 objek saja (`hanya:`); hapus tekstur yang tidak dipakai. Berat |
| kulkas | `source/KULKAS.fbx` (**38 MB**) | Tidak | jpeg 3K | Dapur. **Kemungkinan sangat berat**, cek segitiga |
| led-tv | `source/led tv.rar` → `tv.fbx` | Ya (rar) | jpeg | Ruang tengah |
| low-poly-door | `source/PortalDoor.zip` → `PortalDoor.fbx` | Ya (zip) | jpg 2K | Daun pintu (lihat Ambigu 3) |
| plastic-chair | `source/Plastic Chair.fbx` | Tidak | png (tanpa BaseColor; pakai warna material) | Pos ronda, teras, dapur |
| pos-ronda | `source/model.glb` | Tidak | jpeg | Ganti `PosRonda` |
| power-outlet-light-switch | `source/Untitled 2.fbx` | Tidak | png | Ganti `StopKontak` + sakelar dekor |
| simple-wood-fence | `source/Simple Wood Fence.rar` → fbx | Ya (rar; isinya 29 MB png → pakai `textures/` yang sudah ada) | png | Pagar pekarangan/desa |
| sink-set | `source/sink2.fbx` | Tidak | jpeg 1K | Kamar mandi |
| source | `2Gapura.fbx` | Tidak | — (tanpa tekstur; pakai material `Tembok`/`BatuPutih`) | Ganti `Gapura` |
| source-2 | `CeilingLightRound.fbx` | Tidak | — | Lampu plafon (ganti `LampuNeon` di rumah keluarga? lihat Ambigu 2) |
| source-3 | kosong | — | — | Hapus folder |
| source-BbLght | `BbLght.obj` + `.mtl` | Tidak | — | Bohlam gantung dapur/kamar mandi |
| teapot | `scene.gltf` | Tidak | jpeg/png | Dapur. CC-BY-4.0, ksa2030 |
| toilet | `source/Toilet.zip` → `Toilet.fbx` | Ya (zip) | png | Kamar mandi |
| toilet-wall.jpg | — | Tidak | 1 jpg | Dinding kamar mandi |
| tools-asset | `source/Tools.fbx` (palu, linggis "Fomka", tang "Pliers") | Tidak | jpg/png | Ganti `Linggis`, `Tang` (pacul tidak ada; `Pacul` tetap pengganti) |
| tv_remote.glb | `tv_remote.glb` (4,4 MB) | Tidak | tertanam | Meja ruang tengah |
| voodoo-doll | `source/Doll(stab).fbx` | Tidak | jpg | Ganti `Boneka` |
| water-dispenser | `source/Waterdispenser.fbx` | Tidak | png besar (56 MB total) | Dapur |
| well-building-sumur | `source/Sumur_Baked.fbx` | Tidak | jpg/png (`Ground_*` tidak dipakai) | Ganti `SumurTimba` (rumah keluarga) dan `Sumur` (Bu Sumi) |
| window-curtains | `source/curtains-animated-512.glb` | Tidak | jpeg | Gorden jendela (animasi dimatikan) |
| wooden-cabinet | `source/Cabinet_old.zip` → `Cabinet_old.fbx` (+ dae/obj: hapus) | Ya (zip) | png 4K | Lemari dapur/Bu Sumi |
| wooden-kitchen-cabinet | `scene.gltf` | Tidak | jpeg/png | Dapur. CC-BY-4.0, jimbogies |
| wooden-kitchen-table-chair | `source/Mesa e cadeiras.rar` → `Table_and_Chair.fbx` (rar berisi TGA 50 MB: **jangan** disimpan) | Ya (rar, ambil fbx saja) | png di `textures/` (163 MB → setelah hapus yang tak dipakai) | Dapur |
| wooden-ladder | `source/Ladder Wooden.fbx` | Tidak | png | Ganti `TanggaBambu` (lihat Ambigu 4) |
| wooden-wardrobe | `source/FurnitureDresser001a_substanceuv.fbx` | Tidak | jpg | Ganti `LemariPakaian` (visual saja) |
| wooden-window | `source/window_Wooden.fbx` | Tidak | png | Bingkai jendela |

Jumlah segitiga: tidak bisa diketahui tanpa Unity; Task 1 menambah log jumlah segitiga per prefab di akhir `PembuatAsetAsli.Buat()`.

**Lisensi:** hanya `bathroom_accesories`, `teapot`, `wooden-kitchen-cabinet` punya `license.txt` (semua CC-BY-4.0, kredit wajib). Sisanya: "cek lisensi" di KREDIT.

## Struktur File

- Modify: `Assets/_Project/Scripts/Editor/PembuatAsetAsli.cs` — `static class` → `static partial class`; panggil `BuatProps()` dan `LaporSegitiga()` sebelum `AssetDatabase.SaveAssets()` di `Buat()`.
- Create: `Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs` — semua prefab props + helper `Bahan`.
- Create: `tools/siapkan-props.sh` — ekstrak arsip, pindah ke `Art/ThirdParty/<nama>/`, hapus file tak terpakai.
- Modify: `.gitattributes` — tambah `*.bin`, `*.tga` sudah ada.
- Modify: `PembuatRumahKeluarga.cs` (Task 3), `PembuatDesa.cs`, `PembuatRumahBuSumi.cs`, `PembuatRumahPakde.cs`, `PembuatMakam.cs` (Task 4).
- Modify: `docs/aset/KREDIT.md`, `CLAUDE.md` (Task 5).

---

### Task 1: Pipeline impor (ekstrak, pindah, bersihkan, helper material)

**Files:**
- Create: `tools/siapkan-props.sh`
- Modify: `.gitattributes`
- Modify: `Assets/_Project/Scripts/Editor/PembuatAsetAsli.cs`
- Create: `Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs`

**Interfaces:**
- Produces: folder `Assets/_Project/Art/ThirdParty/<nama-aset>/` (nama folder sama dengan di `Props TheSending`, `source` → `gapura`, `source-2` → `lampu-plafon`, `source-BbLght` → `bohlam`); konstanta `const string Props = Pihak3 + "/";`; helper `static void Bahan(GameObject g, string folder, string baseColor, string warnaHex = "FFFFFF")` (membuat/menimpa `Art/ThirdParty/<folder>/Bahan_<nama file>.mat` URP Lit, `_BaseMap` = tekstur, Smoothness 0, dan memasangnya ke semua renderer `g`); `static void BuatProps()`; `static void LaporSegitiga()`.

- [ ] **Step 1: Pastikan editor tertutup dan cek LFS.** Jalankan `git lfs install` sekali. Tambahkan ke `.gitattributes` (setelah baris `*.glb`):

```
*.bin filter=lfs diff=lfs merge=lfs -text
```

- [ ] **Step 2: Tulis `tools/siapkan-props.sh`** (aman diulang):

```bash
#!/usr/bin/env bash
# Ekstrak arsip props, pindah ke Art/ThirdParty, hapus arsip dan file tak terpakai. Editor Unity harus tertutup.
set -euo pipefail
cd "$(dirname "$0")/.."
SRC="Assets/Props TheSending"; DST="Assets/_Project/Art/ThirdParty"
[ -d "$SRC" ] || { echo "Sudah dipindah."; exit 0; }
x() { /usr/bin/bsdtar -xf "$1" -C "$2"; rm -f "$1" "$1.meta"; }
x "$SRC/antique-wall-clock/source/clock.zip"        "$SRC/antique-wall-clock/source"
x "$SRC/door-padlock-key/source/padlock.zip"        "$SRC/door-padlock-key/source"
x "$SRC/low-poly-door/source/PortalDoor.zip"        "$SRC/low-poly-door/source"
x "$SRC/toilet/source/Toilet.zip"                   "$SRC/toilet/source"
x "$SRC/wooden-cabinet/source/Cabinet_old.zip"      "$SRC/wooden-cabinet/source"
x "$SRC/led-tv/source/led tv.rar"                   "$SRC/led-tv/source"
# rar besar: ambil model saja (tekstur sudah ada di textures/)
/usr/bin/bsdtar -xf "$SRC/simple-wood-fence/source/Simple Wood Fence.rar" -C "$SRC/simple-wood-fence/source" --include '*.fbx' --include '*.FBX' --include '*.obj'
rm -f "$SRC/simple-wood-fence/source/Simple Wood Fence.rar"*
/usr/bin/bsdtar -xf "$SRC/wooden-kitchen-table-chair/source/Mesa e cadeiras.rar" -C "$SRC/wooden-kitchen-table-chair/source" --include '*.fbx'
rm -f "$SRC/wooden-kitchen-table-chair/source/Mesa e cadeiras.rar"*
# tekstur ganda/berlebih dari zip (textures/ sudah punya), dan file yang tak dipakai Unity
find "$SRC" \( -name '*.tga' -o -name '*.tif' -o -name '*.tiff' -o -name '*.mtlx' -o -name '*.dae' -o -name 'Thumbs.db' \
  -o -name '*Height*' -o -name '*Displacement*' -o -name '*Preview*' -o -name 'internal_ground_ao_texture.jpeg' \) -print -delete
find "$SRC/wooden-cabinet/source" -name 'Cabinet_old.obj' -o -name 'Cabinet_old.mtl' | xargs rm -f
rm -rf "$SRC/source-3" "$SRC/source-3.meta" "$SRC/well-building-sumur/textures/Ground_"*
mkdir -p "$DST"
mv "$SRC/source" "$DST/gapura"; mv "$SRC/source-2" "$DST/lampu-plafon"; mv "$SRC/source-BbLght" "$DST/bohlam"
for f in "$SRC"/*.jpg "$SRC"/*.glb; do mkdir -p "$DST/props-lepas"; mv "$f" "$DST/props-lepas/"; done
for d in "$SRC"/*/; do mv "$d" "$DST/"; done
find "$SRC" -name '*.meta' -delete; rmdir "$SRC"
echo "Props siap di $DST"
```

Catatan: `.meta` di sumber ikut dipindah bersama foldernya; `.meta` file yang dihapus dibuat ulang Unity. Jika folder tujuan sudah ada (mis. dari percobaan sebelumnya), `mv` gagal: hapus tujuannya dulu.

- [ ] **Step 3: Jalankan** `bash tools/siapkan-props.sh`. Expected: "Props siap di ...". Lalu `du -sh Assets/_Project/Art/ThirdParty/*/ | sort -h | tail` untuk mencatat ukuran.

- [ ] **Step 4: Jadikan partial dan siapkan file props.** Di `PembuatAsetAsli.cs` ganti `public static class PembuatAsetAsli` menjadi `public static partial class PembuatAsetAsli`, dan di `Buat()` tepat sebelum `AssetDatabase.SaveAssets();` tambahkan:

```csharp
            BuatProps();
            LaporSegitiga();
```

Buat `PembuatAsetAsli.Props.cs`:

```csharp
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Props dari "Props TheSending" (pindahan ke Art/ThirdParty). Nama prefab yang sama dengan prefab Pengganti
    // otomatis menggantikannya (PembuatPengganti.Pasang memuat Prefabs/Asli lebih dulu); logika tidak berubah.
    public static partial class PembuatAsetAsli
    {
        const string Props = Pihak3 + "/";

        static void BuatProps()
        {
            // Diisi Task 2-4.
        }

        // Material URP Lit dari satu tekstur warna; dipasang ke semua renderer (material bawaan FBX hanya-baca).
        static void Bahan(GameObject g, string folder, string baseColor, string warnaHex = "FFFFFF")
        {
            string jalurTex = Props + folder + "/" + baseColor;
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(jalurTex) ?? throw new FileNotFoundException(jalurTex);
            string jalurMat = $"{Props}{folder}/Bahan_{Path.GetFileNameWithoutExtension(baseColor)}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(jalurMat);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, jalurMat);
            }
            mat.SetTexture("_BaseMap", tex);
            mat.SetColor("_BaseColor", PembuatRig.Hex(warnaHex));
            mat.SetFloat("_Smoothness", 0f);
            EditorUtility.SetDirty(mat);
            foreach (var r in g.GetComponentsInChildren<Renderer>())
                r.sharedMaterials = System.Array.ConvertAll(r.sharedMaterials, _ => mat);
        }

        // Jumlah segitiga per prefab Asli, untuk mencatat model berat (Global Constraints).
        static void LaporSegitiga()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { FolderPrefab }))
            {
                var p = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                int n = 0;
                foreach (var mf in p.GetComponentsInChildren<MeshFilter>(true))
                    if (mf.sharedMesh != null) n += mf.sharedMesh.triangles.Length / 3;
                foreach (var sm in p.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    if (sm.sharedMesh != null) n += sm.sharedMesh.triangles.Length / 3;
                Debug.Log($"[Segitiga] {p.name}: {n}");
            }
        }
    }
}
```

- [ ] **Step 5: Jalankan prasetel tekstur dan cek kompilasi.** `bash tools/bangun-semua.sh TheSending.EditorTools.PembuatAsetLuar.Siapkan` → `OK`. Lalu `bash tools/bangun-semua.sh` → `OK`. Lalu `grep "\[Segitiga\]" "${TMPDIR:-/tmp}/the-sending-batch.log"`: daftar prefab Asli lama tercetak.

- [ ] **Step 6: Test.** `bash tools/run-editmode-tests.sh` → semua lulus (378).

- [ ] **Step 7: Commit** (aset belum dipakai tetap ikut, karena Task 2–4 memakainya; Task 5 menghapus yang tidak terpakai):

```bash
git add .gitattributes tools/siapkan-props.sh Assets/_Project/Scripts/Editor/PembuatAsetAsli.cs Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs.meta Assets/_Project/Art/ThirdParty
git status --short | grep -v '^??' ; git commit -m "chore: impor props dekor ke Art/ThirdParty dengan prasetel PSX"
```

Jangan ikut-sertakan `Assets/ALP_Assets` atau file untracked lain di luar daftar.

---

### Task 2: Penggantian visual (nama prefab sama, logika tetap)

**Files:**
- Modify: `Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs` (isi `BuatProps`)
- (Scene, `Prefabs/Asli`, Timeline dibuat ulang)

**Interfaces:**
- Consumes: `Prefab`, `Taruh`, `Seragam`, `Kayu`, `Bahan`, `Props` dari Task 1.
- Produces: prefab Asli `Gapura`, `PosRonda`, `SumurTimba`, `Sumur`, `TanggaBambu`, `Boneka`, `Gembok`, `FotoKeluarga`, `StopKontak`, `Linggis`, `Tang`, `JamDinding`, `LemariPakaian`.

Aturan ukuran: `ukuran` di `Taruh` = ukuran collider logika (atau pengganti) di skrip pembuat, supaya model mengisi tempat yang sama. Titik nol prefab = alas tengah (seperti pengganti), kecuali benda dinding (`FotoKeluarga`, `JamDinding`, `StopKontak`) yang dipasang `BendaDialog(..., tengah: true)`/offset di pembuat: cek pengganti di `PembuatPengganti.cs` (daftar `("FotoKeluarga", ...)` dll.) dan samakan titik nolnya. Untuk menemukan nama anak FBX multi-objek (Tools.fbx): panggil `Taruh(..., hanya: new[] { "?" })` sekali; `Saring` melempar exception berisi "Anak yang ada: ...". Baca log, isi nama yang benar.

- [ ] **Step 1: Isi `BuatProps()`** (nama file tekstur sesuai inventaris; sesuaikan jika log berkata tidak ada):

```csharp
        static void BuatProps()
        {
            // --- Penggantian visual (Task 2) ---
            Prefab("Gapura", r => Kayu(Taruh(Props + "gapura/2Gapura.fbx", r, Vector3.zero, new Vector3(5f, 4f, 0.8f), 0f), "BatuPutih"), false);
            Prefab("PosRonda", r => Taruh(Props + "pos-ronda/source/model.glb", r, Vector3.zero, new Vector3(3f, 2.8f, 2.5f), 0f), true);
            Prefab("SumurTimba", r => Bahan(Taruh(Props + "well-building-sumur/source/Sumur_Baked.fbx", r, Vector3.zero,
                new Vector3(1.3f, 2.4f, 1.3f), 0f), "well-building-sumur", "textures/Diffuse_Bake.jpg"), false);
            Prefab("Sumur", r => Bahan(Taruh(Props + "well-building-sumur/source/Sumur_Baked.fbx", r, Vector3.zero,
                new Vector3(1.3f, 2.4f, 1.3f), 0f), "well-building-sumur", "textures/Diffuse_Bake.jpg"), false);
            Prefab("TanggaBambu", r => Bahan(Taruh(Props + "wooden-ladder/source/Ladder Wooden.fbx", r, Vector3.zero,
                new Vector3(0.6f, 2.5f, 0.15f), 0f), "wooden-ladder", "textures/Ladder_Wooden_BaseColor.png"), false);
            Prefab("Boneka", r => Bahan(Taruh(Props + "voodoo-doll/source/Doll(stab).fbx", r, Vector3.zero,
                new Vector3(0.3f, 0.12f, 0.4f), 0f), "voodoo-doll", "textures/voodoo_-_base.jpg"), false);
            Prefab("Gembok", r => Bahan(Taruh(Props + "door-padlock-key/source/padlock.fbx", r, Vector3.zero,
                new Vector3(0.08f, 0.12f, 0.04f), 0f), "door-padlock-key", "textures/padlock_Padlock_BaseColor.png"), false);
            Prefab("StopKontak", r => Bahan(Taruh(Props + "power-outlet-light-switch/source/Untitled 2.fbx", r, Vector3.zero,
                new Vector3(0.1f, 0.1f, 0.03f), 0f), "power-outlet-light-switch", "textures/BaseColor.png"), false);
            Prefab("JamDinding", r => Bahan(Taruh(Props + "antique-wall-clock/source/clock.obj", r, Vector3.zero,
                new Vector3(0.35f, 0.7f, 0.15f), 0f), "antique-wall-clock", "textures/clock_low_clock_BaseColor.png"), false);
            Prefab("LemariPakaian", r => Bahan(Taruh(Props + "wooden-wardrobe/source/FurnitureDresser001a_substanceuv.fbx", r, Vector3.zero,
                new Vector3(1f, 2f, 0.6f), 0f), "wooden-wardrobe", "textures/FurnitureDresser001a_color.jpg"), false);
            Prefab("Linggis", r => Bahan(Taruh(Props + "tools-asset/source/Tools.fbx", r, Vector3.zero, new Vector3(0.05f, 1.2f, 0.12f), 0f,
                hanya: new[] { "Fomka1" }), "tools-asset", "textures/Fomka1_baseColor.jpg"), false);
            Prefab("Tang", r => Bahan(Taruh(Props + "tools-asset/source/Tools.fbx", r, Vector3.zero, new Vector3(0.22f, 0.03f, 0.08f), 0f,
                hanya: new[] { "Pliers" }), "tools-asset", "textures/PliersMat_baseColor.jpg"), false);
            Prefab("FotoKeluarga", FotoKeluarga, false);
        }

        // Bingkai kayu + foto (props-lepas/foto-keluarga.jpg). Titik nol di tengah bawah seperti pengganti (0,6 x 0,45 x 0,04).
        static void FotoKeluarga(Transform r)
        {
            Kotak(r, "Bingkai", "KayuGelap", new Vector3(0f, 0.225f, 0f), new Vector3(0.6f, 0.45f, 0.04f), false);
            var foto = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(foto.GetComponent<Collider>());
            foto.name = "Foto";
            foto.transform.SetParent(r, false);
            foto.transform.localPosition = new Vector3(0f, 0.225f, 0.021f);   // sisi +Z; pengganti diputar 180° di pembuat
            foto.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            foto.transform.localScale = new Vector3(0.52f, 0.37f, 1f);
            Bahan(foto, "props-lepas", "foto-keluarga.jpg");
        }
```

Periksa arah: `LemariPakaian` di `PembuatMalam2` dipasang dengan putaran 90° dan `ukuran` collider (0,6; 2; 1) — samakan `ukuran` model dengan `new Vector3(1f, 2f, 0.6f)` sebelum putaran (lebar di X). Jika depan model menghadap dinding, ubah `putarY` di `Taruh` menjadi 180f. Sama untuk `SumurTimba`/`Sumur` (collider 1,3 × 1,8 × 1,3; tinggi visual boleh lebih karena atap/kerekan). Jika `Kotak` bersifat private dengan signature berbeda, baca `PembuatAsetAsli.cs:424` dan sesuaikan.

- [ ] **Step 2: Bangun ulang.** `bash tools/bangun-semua.sh` → `OK`. Jika gagal karena `Saring` ("Anak yang ada: ..."), perbaiki nama di `hanya` dan ulangi.

- [ ] **Step 3: Catat segitiga.** `grep "\[Segitiga\]" "${TMPDIR:-/tmp}/the-sending-batch.log"`; catat yang > 20.000 di laporan task.

- [ ] **Step 4: Test.** `bash tools/run-editmode-tests.sh` → semua lulus.

- [ ] **Step 5: Cek diff scene tidak menyentuh logika.** `git diff --stat Assets/_Project/Scenes` boleh berubah; `git diff Assets/_Project/Scripts/Runtime Assets/_Project/Scripts/Core` harus kosong.

- [ ] **Step 6: Commit**

```bash
git add Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs Assets/_Project/Prefabs/Asli Assets/_Project/Scenes Assets/_Project/Timeline Assets/_Project/Art/Models/Kubus Assets/_Project/Art/ThirdParty
git commit -m "feat: ganti visual pengganti dengan props (gapura, pos ronda, sumur, tangga, boneka, gembok, foto, stop kontak, alat, jam, lemari)"
```

---

### Task 3: Rumah keluarga (barang modern, kamar mandi, dapur, dekor)

**Files:**
- Modify: `Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs` (prefab dekor baru)
- Modify: `Assets/_Project/Scripts/Editor/PembuatRumahKeluarga.cs` (`Perabot`, ~baris 153–181)

**Interfaces:**
- Consumes: Task 1–2.
- Produces: prefab Asli `TvLed`, `RemoteTv`, `Kulkas`, `Dispenser`, `KursiPlastik`, `Sakelar`, `LampuPlafon`, `Bohlam`, `Kloset`, `Wastafel`, `AksesorisMandi`, `LemariDapur`, `MejaMakan`, `TekoTeh`, `DekorDapur`, `Karpet`, `Gorden`, `Lemari`, `Lilin`; material `UbinMandi`, `DindingMandi`.

Denah rumah (dari `PembuatRumahKeluarga.cs`): rumah x −7..7, z 0..12, tinggi plafon `Tinggi`. Ruang tengah x −3..3, z 0..8 (saka di x ±1,5, z 2,5/5,5; sofa (−1,95; 0,5); meja (−2,2; 3,6); lemari persembunyian (−2,65; 4,8)). **Kamar mandi x 3..7, z 0..4**, pintu di sekat x = 3, z 1,5–2,5; ada `BakMandi` (6,3; 0,4; 0,8) ukuran 1 × 0,8 × 1. Kamar Laras x 3..7, z 4..8. Dapur x −7..3?, z 8..12 (pintu dari ruang tengah x −0,6..0,6; meja kayu (−5; 10,5) dengan rak bumbu; anglo (−2,6; 11,3); meja ritual (−1,5; 11,4); bambu (−0,85; 11,55); titik muncul Sosok (−0,5; 10,5); pintu belakang (−4; 12)). Gudang x 3..7, z 8..12. **Zona terlarang** (jangan taruh apa pun): jalur pintu ±0,6 m di depan setiap pintu, garis lurus pintu dapur (x −0,6..0,6, z 7..11), sekitar anglo/meja ritual (x −3,2..−0,4, z 10,6..12), titik muncul Sosok, jalur ke pintu belakang (x −4,6..−3,4, z 10..12), sekitar lemari persembunyian dan `KeluarLemari` (−1,9; 4,8).

- [ ] **Step 1: Tambah prefab dekor di `BuatProps()`** (setelah blok Task 2):

```csharp
            // --- Dekor rumah keluarga (Task 3) ---
            Prefab("TvLed", r => Bahan(Taruh(Props + "led-tv/source/tv.fbx", r, Vector3.zero, new Vector3(0.9f, 0.55f, 0.15f), 0f),
                "led-tv", "textures/phong1_Base_Color.jpeg"), false);
            Prefab("RemoteTv", r => Taruh(Props + "props-lepas/tv_remote.glb", r, Vector3.zero, new Vector3(0.05f, 0.02f, 0.17f), 0f), false);
            Prefab("Kulkas", r => Taruh(Props + "kulkas/source/KULKAS.fbx", r, Vector3.zero, new Vector3(0.6f, 1.5f, 0.6f), 0f), true);
            Prefab("Dispenser", r => Bahan(Taruh(Props + "water-dispenser/source/Waterdispenser.fbx", r, Vector3.zero,
                new Vector3(0.32f, 1.1f, 0.32f), 0f), "water-dispenser", "textures/BakedWaterDispenser.png"), true);
            Prefab("KursiPlastik", r => Taruh(Props + "plastic-chair/source/Plastic Chair.fbx", r, Vector3.zero, new Vector3(0.5f, 0.8f, 0.5f), 0f), true);
            Prefab("Sakelar", r => Bahan(Taruh(Props + "power-outlet-light-switch/source/Untitled 2.fbx", r, Vector3.zero,
                new Vector3(0.08f, 0.12f, 0.03f), 0f), "power-outlet-light-switch", "textures/BaseColor.png"), false);
            Prefab("LampuPlafon", r => Kayu(Seragam(Props + "lampu-plafon/CeilingLightRound.fbx", r, Vector3.zero, 0.35f), "Kertas"), false);
            Prefab("Bohlam", r => Kayu(Seragam(Props + "bohlam/BbLght.obj", r, Vector3.zero, 0.12f), "Kertas"), false);
            Prefab("Kloset", r => Bahan(Taruh(Props + "toilet/source/Toilet.fbx", r, Vector3.zero, new Vector3(0.4f, 0.4f, 0.55f), 0f),
                "toilet", "textures/Toilet_BaseColor.png"), true);
            Prefab("Wastafel", r => Bahan(Taruh(Props + "sink-set/source/sink2.fbx", r, Vector3.zero, new Vector3(0.55f, 0.85f, 0.45f), 0f),
                "sink-set", "textures/Fabric020_1K_Color.jpeg"), false);
            Prefab("AksesorisMandi", r => Taruh(Props + "bathroom_accesories/scene.gltf", r, Vector3.zero, new Vector3(0.5f, 0.3f, 0.2f), 0f), false);
            Prefab("LemariDapur", r => Taruh(Props + "wooden-kitchen-cabinet/scene.gltf", r, Vector3.zero, new Vector3(1.6f, 0.9f, 0.6f), 0f), true);
            Prefab("MejaMakan", r => Bahan(Taruh(Props + "wooden-kitchen-table-chair/source/Table_and_Chair.fbx", r, Vector3.zero,
                new Vector3(1.2f, 0.95f, 1.2f), 0f), "wooden-kitchen-table-chair", "textures/Table_BaseColor.png"), true);
            Prefab("TekoTeh", r => Taruh(Props + "teapot/scene.gltf", r, Vector3.zero, new Vector3(0.22f, 0.16f, 0.15f), 0f), false);
            // Pilih 4-6 objek dari paket dapur (nama anak dari exception Saring); sisanya dibuang.
            Prefab("DekorDapur", r => Taruh(Props + "kitchen-decor-pack/source/KitchenDecorPack.fbx", r, Vector3.zero,
                new Vector3(0.9f, 0.35f, 0.3f), 0f, hanya: new[] { "CuttingBoard", "JarTap", "FruitPlate", "TallSpiceBottle" }), false);
            Prefab("Karpet", r => Bahan(Taruh(Props + "carpet/source/Carpet_Sketchfab.fbx", r, Vector3.zero, new Vector3(2f, 0.02f, 1.4f), 0f),
                "carpet", "textures/TX_Carpet_0102_albedo.jpg"), false);
            Prefab("Gorden", r => Bahan(Taruh(Props + "window-curtains/source/curtains-animated-512.glb", r, Vector3.zero,
                new Vector3(1.2f, 1.6f, 0.1f), 0f), "window-curtains", "textures/A23DTEX_Albedo_1.jpeg"), false);
            Prefab("Lemari", r => Bahan(Taruh(Props + "wooden-cabinet/source/Cabinet_old.fbx", r, Vector3.zero, new Vector3(0.9f, 1.8f, 0.45f), 0f),
                "wooden-cabinet", "textures/Wood_aged_Base_color.png"), true);
            Prefab("Lilin", r => Bahan(Taruh(Props + "candle-low/source/CandleLow.fbx", r, Vector3.zero, new Vector3(0.05f, 0.15f, 0.05f), 0f),
                "candle-low", "textures/Candle_Base_Color.png"), false);
```

Gorden glb beranimasi: setelah `Taruh`, hapus komponen `Animator`/`Animation` di prefab (`foreach (var a in r.GetComponentsInChildren<Animator>(true)) Object.DestroyImmediate(a);`), supaya statis. Jika `KULKAS.fbx` > 50.000 segitiga (lihat log), ganti isi `Kulkas` dengan `Kotak(r, "Kulkas", "Kertas", new Vector3(0f, 0.75f, 0f), new Vector3(0.6f, 1.5f, 0.6f), false)` dan laporkan (aset diganti pengganti, KULKAS.fbx dihapus di Task 5).

- [ ] **Step 2: Material kamar mandi.** Tambahkan dua entri di `DaftarTekstur` (`PembuatAsetAsli.cs:40`) — material dibuat `PembuatPengganti.Material(bahan)` sehingga `Balok(..., "UbinMandi")` bisa dipakai:

```csharp
            ("UbinMandi", Pihak3 + "/props-lepas/floor-tile-toilet.jpg", "FFFFFF", 1f),
            ("DindingMandi", Pihak3 + "/props-lepas/toilet-wall.jpg", "FFFFFF", 1f),
```

Jika `PembuatPengganti.Material` menolak nama yang belum ada di daftar warna pengganti, tambahkan juga `("UbinMandi", "D8D8D0"), ("DindingMandi", "C8D0C8")` di daftar warna `PembuatPengganti.cs:~23`. Ubin keramik Poliigon (`ceramic-floor-texture/2K/..._BaseColor.jpg`) dipakai untuk `DindingMandi` jika `toilet-wall.jpg` terlihat buruk di Play Mode.

- [ ] **Step 3: Pasang di `PembuatRumahKeluarga.Perabot`.** Ganti baris `Balok("BakMandi", ...)` dengan blok kamar mandi, dan tambahkan dekor lain. Koordinat awal (dunia; verifikasi bebas tabrakan di Step 5):

```csharp
            // Kamar mandi (x 3..7, z 0..4), pintu di x = 3, z 1,5-2,5. Hanya dekor; tanpa interaksi baru.
            Balok("BakMandi", new Vector3(6.3f, 0.4f, 0.8f), new Vector3(1f, 0.8f, 1f), "UbinMandi");
            Balok("LantaiMandi", new Vector3(5f, 0.005f, 2f), new Vector3(3.9f, 0.01f, 3.9f), "UbinMandi");
            Balok("DindingMandiTimur", new Vector3(6.98f, 0.75f, 2f), new Vector3(0.02f, 1.5f, 3.9f), "DindingMandi");
            Balok("DindingMandiSelatan", new Vector3(5f, 0.75f, 0.02f), new Vector3(3.9f, 1.5f, 0.02f), "DindingMandi");
            PembuatPengganti.Pasang("Kloset", null, new Vector3(6.6f, 0f, 3.2f), -90f);
            PembuatPengganti.Pasang("Wastafel", null, new Vector3(4.2f, 0f, 3.75f), 180f);
            PembuatPengganti.Pasang("AksesorisMandi", null, new Vector3(4.2f, 1.2f, 3.9f), 180f);
            PembuatPengganti.Pasang("Bohlam", null, new Vector3(5f, Tinggi - 0.3f, 2f));

            // Ruang tengah: TV di atas meja kecil di dinding timur, karpet di depan sofa, sakelar dekat pintu depan.
            PembuatPengganti.Pasang("Karpet", null, new Vector3(-1.95f, 0f, 1.6f));
            PembuatPengganti.Pasang("Laci", null, new Vector3(2.7f, 0f, 1.0f), -90f);
            PembuatPengganti.Pasang("TvLed", null, new Vector3(2.7f, 0.85f, 1.0f), -90f);
            PembuatPengganti.Pasang("RemoteTv", null, new Vector3(-2.2f, 0.76f, 3.5f), 30f);
            PembuatPengganti.Pasang("Sakelar", null, new Vector3(0.8f, 1.3f, 0.06f));
            PembuatPengganti.Pasang("KursiPlastik", null, new Vector3(2.4f, 0f, 7.3f), 200f);

            // Dapur: lemari dan kulkas di dinding barat, dispenser dan meja makan di sisi timur pintu dapur.
            PembuatPengganti.Pasang("LemariDapur", null, new Vector3(-6.65f, 0f, 9.4f), 90f);
            PembuatPengganti.Pasang("Kulkas", null, new Vector3(-6.6f, 0f, 11.4f), 90f);
            PembuatPengganti.Pasang("DekorDapur", null, new Vector3(-6.65f, 0.9f, 9.4f), 90f);
            PembuatPengganti.Pasang("Dispenser", null, new Vector3(2.6f, 0f, 8.4f));
            PembuatPengganti.Pasang("MejaMakan", null, new Vector3(1.6f, 0f, 10.6f));
            PembuatPengganti.Pasang("TekoTeh", null, new Vector3(1.6f, 0.76f, 10.6f));
            PembuatPengganti.Pasang("JamDinding", null, new Vector3(-3.5f, 1.9f, 8.14f));   // jam kedua: dapur; hapus jika terlalu ramai

            // Kamar: lemari kayu tua di kamar Raka; gorden di jendela (lihat jendela yang ada di dinding luar).
            PembuatPengganti.Pasang("Lemari", null, new Vector3(-4.2f, 0f, 3.7f), 180f);
```

Lalu di `Listrik(...)` (baris ~194) ganti `"LampuNeon"` dengan `"LampuPlafon"` **hanya jika** tim memilih itu (Ambigu 2); jika tidak, pasang `LampuPlafon` hanya di kamar mandi dan dapur. Gorden: cari jendela yang ada dengan `grep -n "Jendela" PembuatRumahKeluarga.cs PembuatAsetAsli.cs`; pasang `Gorden` di sisi dalam tiap jendela (offset 0,08 m ke dalam). Jika tidak ada jendela di interior, lewati gorden dan catat.

- [ ] **Step 4: Bangun ulang dan test.** `bash tools/bangun-semua.sh` → `OK`; `bash tools/run-editmode-tests.sh` → lulus.

- [ ] **Step 5: Verifikasi tabrakan dan NavMesh tanpa Play Mode.** Tambahkan sementara (tidak di-commit) pemeriksaan di scratch: buka `Scene_RumahKeluarga` di editor oleh tim, atau periksa koordinat manual terhadap Zona terlarang di atas. Wajib: tiap dekor ≥ 0,9 m dari garis tengah setiap pintu; `MejaMakan` dan `Dispenser` tidak berada di x −0,6..0,6. Catat koordinat akhir di laporan.

- [ ] **Step 6: Commit** (scene + prefab + timeline + kubus bersama):

```bash
git add Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs Assets/_Project/Scripts/Editor/PembuatAsetAsli.cs Assets/_Project/Scripts/Editor/PembuatRumahKeluarga.cs Assets/_Project/Scripts/Editor/PembuatPengganti.cs Assets/_Project/Prefabs/Asli Assets/_Project/Prefabs/Pengganti Assets/_Project/Art/Materials/Pengganti Assets/_Project/Scenes Assets/_Project/Timeline Assets/_Project/Art/Models/Kubus Assets/_Project/Art/ThirdParty
git commit -m "feat: isi rumah keluarga dengan props (kamar mandi, dapur, barang modern)"
```

---

### Task 4: Scene lain (desa, rumah Bu Sumi, rumah Pakde, makam)

**Files:**
- Modify: `Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs`
- Modify: `PembuatDesa.cs` (bagian bersama ~baris 330–380), `PembuatRumahBuSumi.cs` (~90–140), `PembuatRumahPakde.cs` (~60–80), `PembuatMakam.cs` (~85–110), `PembuatRumahKeluarga.cs` (pekarangan belakang)

**Interfaces:**
- Consumes: prefab Task 2–3 (`KursiPlastik`, `Lemari`, `Karpet`, `Lilin`, `TekoTeh`, `JamDinding`, `Gorden`).
- Produces: prefab Asli `PagarKayu` (satu segmen, panjang diketahui dari log `Batas`), `JendelaKayu`, `PintuKayu`.

- [ ] **Step 1: Prefab baru:**

```csharp
            // --- Scene lain (Task 4) ---
            Prefab("PagarKayu", r => Bahan(Seragam(Props + "simple-wood-fence/source/Simple Wood Fence.fbx", r, Vector3.zero, 2f),
                "simple-wood-fence", "textures/Diffuse.png"), false);   // panjang 2 m per segmen
            Prefab("JendelaKayu", r => Bahan(Taruh(Props + "wooden-window/source/window_Wooden.fbx", r, Vector3.zero,
                new Vector3(1f, 1.2f, 0.12f), 0f), "wooden-window", "textures/BaseColor.png"), false);
            Prefab("PintuKayu", r => Bahan(Taruh(Props + "low-poly-door/source/PortalDoor.fbx", r, Vector3.zero,
                new Vector3(1f, 2.1f, 0.1f), 0f), "low-poly-door", "textures/Wood027_2K_Color.jpeg"), false);
```

Nama file fbx pagar diketahui setelah ekstrak (`ls Assets/_Project/Art/ThirdParty/simple-wood-fence/source`); sesuaikan.

- [ ] **Step 2: Pasang dekor** (tanpa collider kecuali pagar di batas yang memang sudah dibatasi tembok tak terlihat):
  - **Desa** (`PembuatDesa`, bagian dipakai semua malam): `KursiPlastik` dua buah di pos ronda (−3,3; 0; 87,5) dan (−3,3; 0; 89), menghadap jalan (90°); `PagarKayu` berderet di sepanjang tepi halaman depan rumah keluarga dan rumah Bu Sumi, hanya di sisi yang **sudah** tertutup batas tak terlihat (cari `Batas`/`Dinding` di `PembuatDesa`), jangan di depan pintu atau jalan. Maksimal 12 segmen per scene.
  - **Rumah Bu Sumi** (`PembuatRumahBuSumi`): `Lemari` di dinding belakang (cari dinding dari `Dinding(` di file), `Lilin` di `MejaKayu` (−2,5; 0,76; 1,2) di samping teplok (jangan menutupi collider teplok), `TekoTeh` di bale-bale (−2,4; 0,45; 3,8). Tidak boleh menutupi `PintuBelakang`/sumur.
  - **Rumah Pakde** (`PembuatRumahPakde`): `Karpet` di ruang dalam/teras, `KursiPlastik` di teras, `JamDinding` di dinding dalam. Hindari titik patroli anjing (cari `Patroli` di file) dengan jarak ≥ 1 m; tanpa collider.
  - **Makam** (`PembuatMakam`): `Lilin` 3–4 buah di depan nisan (posisi nisan dari `Nisan`/`GundukanMakam` di file, offset 0,3 m ke depan). Tanpa collider.
  - **Rumah keluarga pekarangan belakang**: `PagarKayu` mengikuti pagar yang sudah ada (cari `Pagar` di `PembuatRumahKeluarga.cs`/`PembuatMalam3.cs`) sebagai visual; collider pagar lama tetap.
  - `JendelaKayu`/`PintuKayu`: lihat Ambigu 3; default = `JendelaKayu` hanya pada bukaan jendela yang sudah ada di `PembuatAsetAsli.Badan/Bingkai` (ganti kotak bingkai), `PintuKayu` hanya sebagai visual di `PintuDepan`/`PintuGudang` (anak objek logika yang sama, renderer kotak lama dimatikan).

- [ ] **Step 3: Bangun ulang + test.** `bash tools/bangun-semua.sh` → `OK`; `bash tools/run-editmode-tests.sh` → lulus. `grep -i "NavMesh gagal" "${TMPDIR:-/tmp}/the-sending-batch.log"` → kosong.

- [ ] **Step 4: Commit**

```bash
git add Assets/_Project/Scripts/Editor/PembuatAsetAsli.Props.cs Assets/_Project/Scripts/Editor/PembuatDesa.cs Assets/_Project/Scripts/Editor/PembuatRumahBuSumi.cs Assets/_Project/Scripts/Editor/PembuatRumahPakde.cs Assets/_Project/Scripts/Editor/PembuatMakam.cs Assets/_Project/Scripts/Editor/PembuatRumahKeluarga.cs Assets/_Project/Prefabs/Asli Assets/_Project/Scenes Assets/_Project/Timeline Assets/_Project/Art/Models/Kubus Assets/_Project/Art/ThirdParty
git commit -m "feat: dekor props di desa, rumah Bu Sumi, rumah Pakde, dan makam"
```

---

### Task 5: Bersih-bersih, kredit, build Windows, daftar cek

**Files:**
- Delete: file di `Assets/_Project/Art/ThirdParty/<props>/` yang tidak dipakai
- Modify: `docs/aset/KREDIT.md`, `CLAUDE.md`

- [ ] **Step 1: Temukan tekstur/model props yang tidak dipakai.** Untuk setiap folder props, file dipakai jika GUID-nya muncul di `Prefabs/Asli/*.prefab`, material `Bahan_*.mat`, `Art/Materials/Pengganti/*.mat`, atau model sumbernya:

```bash
cd "/Users/andika/Unity-Project/The Sending"
for d in antique-wall-clock bathroom_accesories candle-low carpet ceramic-floor-texture door-padlock-key kitchen-decor-pack kulkas led-tv low-poly-door plastic-chair pos-ronda power-outlet-light-switch simple-wood-fence sink-set gapura lampu-plafon bohlam teapot toilet tools-asset voodoo-doll water-dispenser well-building-sumur window-curtains wooden-cabinet wooden-kitchen-cabinet wooden-kitchen-table-chair wooden-ladder wooden-wardrobe wooden-window props-lepas; do
  find "Assets/_Project/Art/ThirdParty/$d" -type f ! -name '*.meta' ! -name '*.mat' ! -name 'license.txt' | while read -r f; do
    g=$(grep -m1 '^guid:' "$f.meta" | awk '{print $2}')
    grep -rqs "$g" Assets/_Project/Prefabs/Asli Assets/_Project/Art/ThirdParty --include='*.prefab' --include='*.mat' Assets/_Project/Art/Materials || echo "$f"
  done
done > /tmp/props-tak-terpakai.txt; wc -l /tmp/props-tak-terpakai.txt
```

Model (.fbx/.glb/.gltf/.obj/.bin) yang dipakai prefab muncul lewat GUID di prefab. **Tekstur glTF** (dirujuk dari `scene.gltf`, bukan GUID) dan **tekstur yang dipakai model glb/fbx secara tertanam** tidak terdeteksi: jangan hapus file di folder `bathroom_accesories`, `teapot`, `wooden-kitchen-cabinet` yang namanya ada di `scene.gltf` (`grep -o '"uri": *"[^"]*"' scene.gltf`). Tinjau daftar, lalu hapus (beserta `.meta`) — khususnya ratusan tekstur `kitchen-decor-pack`. Folder aset yang sama sekali tak dipakai (mis. `ceramic-floor-texture` bila tidak dipilih, `kulkas` bila diganti kotak) dihapus seluruhnya.

- [ ] **Step 2: Bangun ulang + test lagi** (memastikan tidak ada yang hilang): `bash tools/bangun-semua.sh` → `OK` (tanpa `FileNotFoundException`); `bash tools/run-editmode-tests.sh` → lulus.

- [ ] **Step 3: KREDIT.** Tambah satu baris per props yang dipakai di tabel `docs/aset/KREDIT.md`. Tiga yang punya `license.txt`:

```
| Bathroom accesories (kamar mandi rumah keluarga) | `Assets/_Project/Art/ThirdParty/bathroom_accesories/` | gorzi | https://sketchfab.com/3d-models/bathroom-accesories-1103528bef074721a5a54d1a60ea56ab | CC-BY-4.0 | Ya |
| Teapot (dapur, rumah Bu Sumi) | `Assets/_Project/Art/ThirdParty/teapot/` | ksa2030 | https://sketchfab.com/3d-models/teapot-2218240dae8e4e8dba9f51c0cc78301c | CC-BY-4.0 | Ya |
| Kitchen Cabinets (dapur) | `Assets/_Project/Art/ThirdParty/wooden-kitchen-cabinet/` | jimbogies | https://sketchfab.com/3d-models/kitchen-cabinets-5926fa64fcaa498c818682990db3df27 | CC-BY-4.0 | Ya |
```

Sisanya (jam dinding, lilin, karpet, ubin Poliigon, gembok, kulkas, TV, pintu, kursi plastik, pos ronda, stop kontak, pagar, wastafel, gapura, lampu plafon, bohlam, kloset, alat, remote TV, boneka voodoo, dispenser, sumur, gorden, lemari, lemari dapur, meja makan, tangga, lemari pakaian, jendela, dan foto/house-texture/toilet-wall/floor-tile jpg) dengan kolom Pembuat/Sumber/Lisensi = `cek lisensi (isi tim)`, Kredit wajib = `**(isi)**`.

- [ ] **Step 4: Build Windows.** `bash tools/bangun-semua.sh TheSending.EditorTools.PembuatBuild.Windows` → log berisi "Build Finished, Result: Success" (abaikan GAGAL palsu karena kata "Exception" di nama file pada laporan ukuran build, lihat log Fase 6). Catat ukuran `Builds/Windows/` dibanding sebelumnya.

- [ ] **Step 5: CLAUDE.md.** Tambah di bagian Konvensi Kode satu baris: "**Props dekor:** prefab Asli bernama sama dengan prefab Pengganti otomatis menggantikannya (`PembuatPengganti.Pasang`); props di `PembuatAsetAsli.Props.cs`, aset di `Art/ThirdParty/<nama>/`." Tambah bagian **Daftar Cek Play Mode Props Dekor** (isi dari bagian bawah rencana ini) dan satu baris di Log Progres.

- [ ] **Step 6: Commit**

```bash
git add -u Assets/_Project/Art/ThirdParty
git add docs/aset/KREDIT.md CLAUDE.md Assets/_Project/Prefabs/Asli Assets/_Project/Scenes Assets/_Project/Timeline Assets/_Project/Art/Models/Kubus
git commit -m "chore: hapus props tak terpakai, catat kredit, daftar cek props dekor"
```

---

## Daftar Cek Play Mode Props Dekor (dikerjakan tim)

Prasyarat: editor dibuka setelah "Bangun Ulang Semua". Tombol uji F5/F6/F9 seperti biasa. Catat nomor butir yang gagal.

1. Prolog: gapura baru tampil utuh, tidak melayang/tenggelam, mobil dan kamera adegan gapura tidak tertutup. Tidak bisa keluar lewat gapura.
2. Rumah keluarga: foto keluarga menampilkan gambar foto, E tetap membuka dialog, Q tetap memberi `[FOTO]` bila dipakai. Stop kontak baru: E tetap mengisi baterai. Jam dinding antik tampil di dinding (tidak menembus).
3. Gembok lemari Bapak: model gembok di tempat yang sama, panel kode tetap terbuka, gembok hilang setelah kode benar.
4. Malam 2: boneka voodoo di lubang lantai dan (jika disimpan) di kotak kayu kamar Raka; linggis baru bisa diambil. Lemari persembunyian (model lemari baru): masuk/keluar dengan E, Sosok berpatroli tanpa macet di sekitar lemari, karpet, TV, dan meja.
5. Malam 3: tang baru bisa diambil; ritual dapur tetap bisa dibidik (anglo, garam, kelor, korek, bambu) dan tidak tertutup lemari dapur/meja makan/kulkas. Sosok muncul dari dapur tanpa error NavMesh dan tidak macet di meja makan atau dispenser.
6. Malam 4: sumur baru di pekarangan: tahan E kerekan bekerja; Sosok kejaran tidak macet di sumur; tangga kayu di gudang bisa diambil. Pos ronda baru: Pak Joko dan warga tidak menembus pos; kursi plastik tidak menghalangi.
7. Kamar mandi: pintu bisa dilewati, ruangan berubin, ada kloset, wastafel, aksesori, bak mandi, bohlam. Tidak ada interaksi baru. Tidak tersangkut di dalam.
8. Dapur: lemari dapur, kulkas, dispenser, meja makan, teko, dekor dapur tampil; jalan ke pintu belakang dan gudang lapang; kalender tetap terbaca.
9. Ruang tengah: TV, remote, karpet, sakelar, kursi plastik tampil; Pakde (Malam 3) dan Laras tidak berdiri menembus perabot baru.
10. Desa: pagar kayu tidak menghalangi jalan, pintu rumah, gerbang makam, atau pagar Pakde. Kerumunan sidang Malam 6 tidak menembus pagar.
11. Rumah Bu Sumi: lilin dan teko tampil; teplok, sumur, dan pintu tetap bisa dipakai.
12. Rumah Pakde: anjing berpatroli tanpa macet di karpet/kursi; pot, laci, mangkuk tetap bekerja.
13. Makam: lilin di depan nisan; sampel tanah dan nisan Pak Hadi tetap bisa dipakai.
14. Gaya PSX sama di semua props (piksel tajam, tidak ada tekstur yang terlihat halus/tajam beresolusi tinggi atau mengkilap). Catat props yang tampak tidak cocok (terlalu modern, warna mencolok).
15. Tidak ada error merah di Console di semua scene. Catat FPS kasar di rumah keluarga (props terberat: kulkas, dispenser, dekor dapur).
16. Build Windows (Fase 8): cek ulang FPS di laptop Windows di dapur dan kamar mandi.

## Ambigu yang Perlu Diputuskan Tim (sebelum Task 3–4)

1. **Teplok:** `candle-low` adalah lilin, bukan lampu teplok minyak. Default rencana: teplok tetap pengganti, lilin dipakai sebagai dekor tambahan. Ganti teplok dengan lilin?
2. **Lampu plafon:** ganti lampu neon di semua kamar rumah keluarga dengan `CeilingLightRound`, atau hanya pasang di kamar mandi/dapur (default)?
3. **Pintu `low-poly-door`:** rumah keluarga sebagian besar bukaan tanpa daun pintu (dan pintu gudang hilang mulai Malam 3). Default: hanya mengganti visual pintu yang sudah ada (pintu depan, pintu belakang) tanpa menambah daun pintu di bukaan kamar. Setuju?
4. **Tangga:** item inventaris bernama "Tangga bambu" tetapi model `wooden-ladder` adalah tangga kayu. Pakai model kayu dan ubah teksnya jadi "Tangga kayu", atau biarkan teks bambu?
5. **Kulkas/dispenser/TV di rumah desa 2003:** ini perlu dipasang meski `PembuatAsetAsli` sebelumnya sengaja mengganti perabot modern menjadi kayu (komentar "rumah desa 2003")? Keputusan tim #2 berkata ya; konfirmasi bahwa kulkas berat (38 MB FBX) boleh diganti kotak bila > 50.000 segitiga.

## Keputusan Tim (22 Nov, mengalahkan isi task di atas)

- D1 Teplok tetap pengganti; `candle-low` hanya dekor.
- D2 `CeilingLightRound` mengganti lampu neon di **semua ruangan** rumah keluarga (cahaya/logika lampu padam Malam 1 tetap sama, hanya visual).
- D3 `low-poly-door` dipasang di **semua ambang kamar** juga, dalam posisi **terbuka menempel dinding, tanpa collider** (tidak menghalangi NavMesh Sosok, pemain, atau raycast E/Q).
- D4 "Tangga bambu" diganti "Tangga kayu" di semua teks (nama barang Core, prompt, notifikasi, dialog); perbarui test yang menyebut teks itu.
- D5 `kulkas` **tidak dipakai** (tim mencari pengganti). Jangan impor, jangan commit; hapus dari daftar task.
