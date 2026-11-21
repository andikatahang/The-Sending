# Panduan Menulis dan Mengerjakan Rencana Fase

Untuk siapa pun (manusia atau agent AI) yang melanjutkan fase berikutnya. Panduan ini berdiri sendiri: **tidak butuh skill atau plugin apa pun** (termasuk "superpowers"). Jika sebuah rencana lama menyebut `superpowers:...`, abaikan baris itu dan ikuti panduan ini.

Baca dulu, berurutan:
1. `CLAUDE.md`: aturan tim, konvensi kode, keputusan, tabel fase dan progres, dan **"Catatan untuk fase berikutnya"** (wajib dikerjakan di fase yang disebut).
2. `docs/roadmap-fase-4-8.md`: isi, sumber, calon keputusan, dan risiko setiap fase berikutnya.
3. Dokumen desain `docs/superpowers/specs/2026-11-15-the-sending-design.md`. **Bagian 4 mengalahkan Game Story dan GDD.**
4. Game Story `docs/TheSending_FullGameStory.md` dan GDD `docs/TheSending_GDD.md` (**baca GDD sampai baris 620 saja**; baris terakhir gambar base64 sekitar 730 KB).
5. Contoh rencana yang sudah jadi: `docs/superpowers/plans/2026-11-18-fase-3-malam-3.md` (paling baru) dan `2026-11-18-fase-2-malam-2.md`. Tiru bentuk dan tingkat detailnya.

## 1. Alur satu fase

| # | Langkah | Siapa | Hasil |
|---|---|---|---|
| 1 | Pastikan PR fase sebelumnya sudah di-merge ke `main`. Buat branch `fase-N` dari `main` | pengembang | branch baru |
| 2 | Tulis rencana `docs/superpowers/plans/<tanggal>-fase-N-<nama>.md` (bentuk di Bagian 2). Bagian keputusan bertanda **MENUNGGU JAWABAN TIM** | agent | rencana di-commit |
| 3 | Jelaskan setiap keputusan K ke tim dengan bahasa sederhana (masalahnya apa, beda pilihannya apa, dampak ke jadwal), baru minta jawaban. Ubah rencana sesuai jawaban, tandai **SUDAH DIPUTUSKAN (tanggal)** | agent + tim | rencana final |
| 4 | **Tutup editor Unity.** Kerjakan task berurutan. Setiap task: tulis, uji, commit | agent | satu commit (atau lebih) per task |
| 5 | **Berhenti** sebelum task terakhir. Tim membuka editor dan menjalankan Daftar Cek Play Mode fase itu, lalu menutup editor dan melapor | tim | hasil cek per nomor butir |
| 6 | Perbaiki butir yang gagal (commit `fix:`), bangun ulang scene, test lagi | agent | test lulus |
| 7 | Task terakhir: dokumen aset, build uji Windows, perbarui `CLAUDE.md` (tabel fase, catatan untuk fase berikutnya, daftar cek, log progres) | agent | fase selesai |
| 8 | Push branch dan buat PR ke `main` (ringkasan + test plan, contoh: PR #3 dan #4) | agent, dengan izin tim | PR |

Jika satu fase melewati jatah harinya, **laporkan ke tim**. Jangan memotong konten GDD sendiri; penyederhanaan ditulis sebagai risiko dan diputuskan tim.

## 2. Bentuk rencana (wajib, berurutan)

1. **Judul dan header:** `# Fase N: Malam N — Implementation Plan`, lalu **Goal** (apa yang bisa dimainkan dari mana sampai mana), **Architecture** (aturan baru apa di Core, komponen Runtime apa, skrip pembuat scene apa), **Tech Stack**.
2. **RISIKO LINGKUP DAN JADWAL:** tabel `# | Risiko / penyederhanaan | Dampak | Usul`. Masukkan semua yang disederhanakan atau ditunda, jadwal, aset yang belum ada, dan hal yang hanya bisa dicek di Play Mode.
3. **KEPUTUSAN TIM:** nomor melanjutkan fase sebelumnya (Fase 3 berakhir di K25, jadi Fase 4 mulai K26). Setiap keputusan: pertanyaan, pilihan A (rekomendasi) dan B, alasan, dan "Jika tim memilih B: ..." (task mana yang berubah). Tulis task untuk rekomendasi.
4. **Urutan Eksekusi:** kapan editor ditutup, urutan task, titik berhenti untuk cek Play Mode, model AI yang disarankan per task.
5. **Global Constraints:** salin dan sesuaikan dari rencana Fase 3 (versi Unity, letak Core/Runtime/Editor, `GameState` hanya diubah kelas Core, jeda dan adegan, angka malam itu, checkpoint, scene dan indeks build, aturan aset, ProjectSettings, paket untracked, format commit).
6. **Cara Menjalankan Test dan Skrip Editor**, termasuk **jumlah test sekarang dan target setelah setiap task**.
7. **Struktur File:** daftar file baru dan yang diubah, plus file yang dibuat skrip dan ikut di-commit.
8. **Persiapan Tim:** A = blocking sebelum Task 1; B = suara (nama file persis, folder, isi); C = model; D = setelah task scene terakhir (cek Play Mode).
9. **Task 1 selalu:** tulis `docs/aset/fase-(N+1).md` (nama, kata kunci Inggris dan Indonesia, sumber, spesifikasi) dan daftar nama file/prefab yang dipakai kode fase ini.
10. **Task berikutnya**, urutan lazim: Core (TDD) → Runtime komponen → sutradara `AlurMalamN` → Editor (prefab/suara pengganti, data dialog `PembuatDataFaseN`, isi scene `PembuatMalamN`, scene baru, "Bangun Ulang Semua") → task terakhir (dokumen, build, `CLAUDE.md`).
11. **Daftar Cek Play Mode Fase N (dikerjakan tim):** butir bernomor, setiap butir menyebut apa yang dilakukan dan apa yang harus terlihat/terdengar persis. Tambahkan "Jika butir X gagal: ..." untuk hal yang berisiko (NavMesh, Timeline, raycast).
12. **Self-Review:** tabel cakupan (setiap butir lingkup fase dari desain Bagian 5–6, Game Story, catatan `CLAUDE.md`, dan keputusan K → task yang mengerjakannya), lalu cek nama tipe dan metode konsisten antar-task.

### Bentuk satu task

````markdown
### Task 2: Core — <ringkas>

**Files:**
- Create: `Assets/_Project/Scripts/Core/X.cs`, `Assets/_Project/Tests/EditMode/XTests.cs`
- Modify: `Assets/_Project/Scripts/Core/GameState.cs`

**Interfaces:**
- Consumes: kelas dan metode yang sudah ada (nama persis).
- Produces: tipe, konstanta, dan tanda tangan metode baru (nama persis), dipakai task sesudahnya.

- [ ] **Step 1: Tulis test yang gagal** (kode test lengkap)
- [ ] **Step 2: Jalankan, pastikan gagal**
  Run: `bash tools/run-editmode-tests.sh TheSending.Tests.XTests`
  Expected: gagal (compile error `error CS` untuk kelas baru juga dihitung gagal)
- [ ] **Step 3: Tulis kode minimal** (kode lengkap)
- [ ] **Step 4: Jalankan semua test**, Expected: `total="<N>" passed="<N>"`
- [ ] **Step 5: Commit** (perintah `git add <jalur persis>` dan `git commit` lengkap)
````

Aturan menulis task:
- **Kode lengkap, bukan uraian.** Nama kelas, field, dan metode harus sama dengan kode di repo saat rencana ditulis. Baca kode yang disentuh sebelum menulis task.
- Setiap langkah punya perintah dan hasil yang diharapkan.
- Tidak ada "TODO", "sesuaikan nanti", atau "mirip task sebelumnya" tanpa kode.
- Task Runtime dan Editor tidak punya EditMode test; verifikasinya: semua test lulus (semua assembly terkompilasi), run batchmode tanpa `error CS`/`Exception`, dan butir daftar cek Play Mode.

## 3. Aturan kerja (berlaku di setiap fase)

- **Core TDD:** test dulu, lihat gagal, baru kode. Core tanpa MonoBehaviour. Hanya kelas Core yang mengubah `GameState`.
- **Scene dibuat skrip.** Jangan menyunting scene dengan tangan; perubahan masuk skrip pembuat lalu "Bangun Ulang Semua" (batchmode: `-executeMethod TheSending.EditorTools.PembuatMenuUtama.BangunSemua`).
- **Editor Unity harus tertutup** saat test atau batchmode. Setelah setiap run: `git diff --quiet ProjectSettings/ProjectSettings.asset || git restore ProjectSettings/ProjectSettings.asset`.
- Hasil regenerasi (scene, prefab `Asli`, `.playable`) di-commit bersama task yang membangunnya, karena ID di dalamnya saling merujuk.
- **Jangan `git add -A`.** Tambahkan hanya jalur yang disebut. Paket di `Art/ThirdParty` yang belum dipakai tetap untracked.
- **Commit:** format `<type>: <deskripsi>` (bahasa Inggris), akhiri dengan baris sesi jika ada. **Tanpa baris `Co-Authored-By`** (aturan tim).
- **Aset:** butuh aset → tulis nama dan kata kunci pencariannya untuk tim. Belum ada → buat pengganti yang menyerupai (`Art/Materials/Pengganti/`, `Prefabs/Pengganti/`, `Audio/SFX/`). Lisensi dicatat di `docs/aset/KREDIT.md`.
- **Teks dialog** ditulis di skrip `PembuatDataFaseN`, bukan di Inspector. Teks yang tidak ada di Game Story/GDD ditulis sebagai draf dan ditandai sebagai risiko supaya dicek tim.
- **Sederhana dulu.** Tim mahasiswa: pilih solusi paling sederhana yang memenuhi GDD.

## 4. Cara kerja dengan agent AI (kebiasaan tim)

- **Jelaskan sebelum bertanya.** Untuk pilihan teknis, jelaskan masalah dan bedanya (tabel singkat), sebut apa yang bisa dikerjakan tim sendiri di editor, baru minta jawaban.
- **Hemat token.** Perintah dan balasan singkat; satu review per kelompok task, satu review akhir seluruh branch; perbaikan kecil diverifikasi sendiri tanpa agent tambahan.
- **Pilih model sesuai berat kerja (berlaku untuk alat AI apa pun):** model terkuat untuk menyusun rencana, task yang menyambung banyak kode lama atau scene, dan review akhir; model menengah untuk task yang kodenya sudah lengkap di rencana, review per task, dan perbaikan kecil. Jangan model termurah/tercepat (sering salah di task bertahap). Contoh Claude: opus / sonnet, tanpa haiku. Jika teks cerita horor diblokir filter konten sebuah model, pakai model lain.
- **Satu agent per task (jika alatnya mendukung subagent):** agent utama hanya mengatur dan memeriksa; setiap task dikerjakan agent baru yang diberi file brief (teks task dari rencana) dan menulis laporan ke file, bukan menempel isi panjang di percakapan. Catat kemajuan di file ledger (mis. `.superpowers/sdd/progress.md`: `Task N: complete (commits a..b, review clean)`) supaya bisa dilanjutkan setelah sesi terputus. Task yang menjalankan Unity batchmode tidak boleh paralel. Tanpa subagent: kerjakan task berurutan seperti butir di bawah.
- **Commit tanpa jejak alat:** tanpa baris `Co-Authored-By`, tanpa link sesi (`Claude-Session: ...` atau sejenisnya) di commit maupun deskripsi PR.
- **Mengerjakan rencana tanpa alat khusus:** baca satu task, kerjakan langkahnya persis, jalankan perintah verifikasinya, commit, centang, lanjut task berikutnya. Jika langkah rencana tidak cocok dengan kode sebenarnya, buat perbaikan minimal dan catat di laporan.
- **Review akhir** sebelum cek Play Mode: baca `git diff <commit awal fase>..HEAD -- '*.cs'` dan cari bug yang merusak permainan: jeda (`Time.timeScale`) tidak dilepas tepat sekali, event tanpa unsubscribe, alur macet (tujuan tidak bisa selesai), state hilang saat ganti scene atau "Lanjutkan", ending tidak dicek setelah `PergantianMalam.Lanjut`, `WaitForSeconds` di adegan, field `static` baru tanpa dibersihkan.
- Push dan PR hanya dengan izin tim.

## 5. Serah terima ke orang berikutnya

Di akhir setiap fase, pastikan:
- `CLAUDE.md`: baris fase di tabel diperbarui, log progres bertanggal, daftar cek Play Mode fase itu, dan **catatan untuk fase berikutnya** (hal yang sengaja ditunda, nama kelas/scene yang harus diganti fase berikutnya).
- `docs/aset/fase-(N+1).md` sudah ada.
- PR sudah dibuat dan di-merge sebelum fase berikutnya mulai. Jangan dua orang mengerjakan fase berbeda di waktu yang sama: scene dibuat skrip dan mudah bentrok.
