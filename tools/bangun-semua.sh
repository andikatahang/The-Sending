#!/usr/bin/env bash
# Menjalankan satu metode editor lewat batchmode (bawaan: "Bangun Ulang Semua"). Editor Unity harus tertutup.
# Pemakaian: bash tools/bangun-semua.sh [Namespace.Kelas.Metode]
# Gagal (exit 1) jika Unity gagal atau log berisi "error CS"/"Exception:". ProjectSettings.asset dipulihkan
# jika file itu bersih sebelum Unity dijalankan.
set -euo pipefail
UNITY="/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity"
PROJECT="$(cd "$(dirname "$0")/.." && pwd)"
METODE="${1:-TheSending.EditorTools.PembuatMenuUtama.BangunSemua}"
LOG="${TMPDIR:-/tmp}/the-sending-batch.log"
PS="ProjectSettings/ProjectSettings.asset"

bersih=0
git -C "$PROJECT" diff --quiet -- "$PS" && bersih=1

set +e
"$UNITY" -batchmode -quit -projectPath "$PROJECT" -executeMethod "$METODE" -logFile "$LOG"
code=$?
set -e

if [ "$bersih" = 1 ]; then git -C "$PROJECT" checkout -- "$PS"; fi

if [ "$code" != 0 ] || grep -qE 'error CS|Exception:' "$LOG"; then
  echo "GAGAL: $METODE (exit $code). Log: $LOG"
  grep -E 'error CS|Exception:|another Unity instance' "$LOG" | sort -u | head -20 || true
  exit 1
fi
echo "OK: $METODE"
