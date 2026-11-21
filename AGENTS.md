# The Sending: instruksi untuk agent AI (alat apa pun)

File ini dibaca otomatis oleh banyak alat AI (Codex, Copilot, Cursor, Gemini, dan lain-lain). Claude Code membaca `CLAUDE.md`. Isinya sama: aturan proyek ada di satu tempat.

Sebelum bekerja, baca berurutan:
1. `CLAUDE.md`: aturan tim, konvensi kode, keputusan, tabel fase, dan catatan untuk fase berikutnya. Semua aturan di sana berlaku untuk alat apa pun, bukan hanya Claude.
2. `docs/panduan-rencana-fase.md`: alur satu fase, bentuk rencana, dan **Bagian 4: kebiasaan kerja tim dengan agent AI** (jelaskan sebelum bertanya, hemat token, pilihan model, satu agent per task, review akhir, commit tanpa jejak alat).
3. `docs/roadmap-fase-4-8.md`: isi fase berikutnya.

Ringkas yang paling sering dilanggar:
- Jelaskan pilihan teknis (masalah, beda, yang bisa dikerjakan tim di editor) sebelum meminta keputusan.
- Scene dibuat skrip; jangan sunting scene dengan tangan. Editor Unity harus tertutup saat test/batchmode (`bash tools/run-editmode-tests.sh`, `bash tools/bangun-semua.sh <method>`).
- Logika Core ditulis TDD. Commit `<type>: <deskripsi>` tanpa `Co-Authored-By` dan tanpa link sesi. Jangan `git add -A`.
- Push dan PR hanya dengan izin tim.
