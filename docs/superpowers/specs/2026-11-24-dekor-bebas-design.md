# Dekor Bebas: prefab dekor per scene

Tanggal: 24 Nov 2026. Branch: `dekor-bebas`. Disetujui user di percakapan (cara A).

## Maksud

Tim ingin menggeser, menghapus, dan menambah benda dekorasi dengan tangan di Unity, tanpa hilang saat The Sending > Bangun Ulang Semua, dan tetap ikut build. Benda cerita tetap diatur skrip supaya alur, puzzle, dan kejaran tidak rusak.

## Pendekatan (dipilih: A)

- **A. Satu prefab dekor per scene** (`Prefabs/Dekor/Dekor_<Scene>.prefab`). Skrip pembuat hanya membuat prefab itu jika belum ada; setelah itu hanya memasangnya. Pola sama dengan `Dekor_Desa` yang sudah ada. Dipilih karena cocok dengan alur skrip sekarang dan dekor ikut build otomatis.
- B. Berhenti membangun scene dari skrip: ditolak, perubahan cerita Fase 8 jadi manual di 9 scene.
- C. Scene dekor additive: ditolak, menambah urusan muat scene, cahaya, Build Settings tanpa manfaat lebih.

## Batas dekor

**Dekor (pindah ke prefab):** perabot, tanaman, pagar, lampu hias, pohon latar, barang rumah tangga yang tidak bisa di-E. Syarat teknis: objeknya tidak punya skrip Runtime, tidak dirujuk field serialized mana pun (sutradara, `AktifDiMalam`, Timeline, `KameraAdegan`), dan tampil sama di setiap malam.

**Tetap skrip:** benda cerita (lemari Bapak, saka, sumur, anglo, ranjang Ibu, pohon asem, nisan, lilin, kendi, kandang), tempat sembunyi, pintu, dinding/lantai/atap/batas, tokoh, titik muncul, zona pemicu, cahaya utama, dan apa pun yang hanya muncul di malam tertentu.

**Scene:** Desa, RumahKeluarga, RumahBuSumi, RumahPakde, Makam, Watuireng. `Scene_MainMenu` tetap memasang `Dekor_Desa` (latar menu). `Scene_Epilog` dan `Scene_Sandbox0B` tidak diubah (bidikan kamera epilog bergantung pada posisi set).

## Aturan

- Prefab yang sudah ada tidak pernah ditimpa. Hapus prefab untuk membuat ulang dari skrip.
- Migrasi `Dekor_Desa`: posisi gapura yang ada di prefab sekarang dipertahankan.
- Akar prefab di titik nol dunia tanpa rotasi/skala, supaya posisi anak = posisi dunia.
- Tidak ada objek yang berpindah posisi akibat migrasi (dicek dengan perbandingan sebelum/sesudah).

## Pemeriksaan "tidak memengaruhi alur cerita"

1. Semua EditMode test lulus.
2. Dump posisi dunia, rotasi, skala, collider, dan komponen setiap objek di setiap scene sebelum dan sesudah; bedanya hanya induk (pindah ke bawah `Dekor_<Scene>`), bukan posisi.
3. Tidak ada field serialized di scene yang jadi kosong (referensi ke objek yang dipindah).
4. Timeline (`.playable`) tidak berubah.

## Cara pakai untuk tim (masuk CLAUDE.md)

Edit dekor langsung di scene lalu pilih objek `Dekor_<Scene>` > Overrides > Apply All, atau buka prefabnya di Prefab Mode. Perubahan yang tidak di-Apply hilang saat Bangun Ulang Semua. Jangan menaruh benda cerita di dalam prefab dekor.
