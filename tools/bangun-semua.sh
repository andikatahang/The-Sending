#!/usr/bin/env bash
# Menjalankan satu metode editor lewat batchmode (bawaan: "Bangun Ulang Semua"). Editor Unity harus tertutup.
# Pemakaian: bash tools/bangun-semua.sh [Namespace.Kelas.Metode]
# Gagal (exit 1) jika Unity gagal atau log berisi "error CS"/"Exception:". ProjectSettings.asset dipulihkan
# jika file itu bersih sebelum Unity dijalankan.
set -euo pipefail
UNITY_MAC="/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity"
UNITY_WIN="/c/Program Files/Unity/Hub/Editor/6000.6.4f1/Editor/Unity.exe"
if [ -z "${UNITY:-}" ]; then if [ -x "$UNITY_MAC" ]; then UNITY="$UNITY_MAC"; else UNITY="$UNITY_WIN"; fi; fi
PROJECT="$(cd "$(dirname "$0")/.." && pwd)"
# Windows (Git Bash): Unity.exe butuh jalur Windows.
wp() { if command -v cygpath >/dev/null 2>&1; then cygpath -w "$1"; else printf '%s' "$1"; fi; }
METODE="${1:-TheSending.EditorTools.PembuatMenuUtama.BangunSemua}"
LOG="${TMPDIR:-/tmp}/the-sending-batch.log"
PS="ProjectSettings/ProjectSettings.asset"

bersih=0
git -C "$PROJECT" diff --quiet -- "$PS" && bersih=1

set +e
"$UNITY" -batchmode -quit -projectPath "$(wp "$PROJECT")" -executeMethod "$METODE" -logFile "$(wp "$LOG")"
code=$?
set -e

if [ "$bersih" = 1 ]; then git -C "$PROJECT" checkout -- "$PS"; fi

if [ "$code" != 0 ] || grep -qE 'error CS|Exception:' "$LOG"; then
  echo "GAGAL: $METODE (exit $code). Log: $LOG"
  grep -E 'error CS|Exception:|another Unity instance' "$LOG" | sort -u | head -20 || true
  exit 1
fi
echo "OK: $METODE"
