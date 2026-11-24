#!/usr/bin/env bash
# Menjalankan EditMode test The Sending lewat command line. Editor Unity harus tertutup.
# Pemakaian: bash tools/run-editmode-tests.sh [testFilter]
# Batchmode menghapus define SENTIS_ANALYTICS_ENABLED dari ProjectSettings.asset; skrip ini memulihkannya
# hanya jika file itu bersih sebelum Unity dijalankan (perubahan sengaja tidak ikut terhapus).
set -euo pipefail
UNITY_MAC="/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity"
UNITY_WIN="/c/Program Files/Unity/Hub/Editor/6000.6.4f1/Editor/Unity.exe"
if [ -z "${UNITY:-}" ]; then if [ -x "$UNITY_MAC" ]; then UNITY="$UNITY_MAC"; else UNITY="$UNITY_WIN"; fi; fi
PROJECT="$(cd "$(dirname "$0")/.." && pwd)"
# Windows (Git Bash): Unity.exe butuh jalur Windows.
wp() { if command -v cygpath >/dev/null 2>&1; then cygpath -w "$1"; else printf '%s' "$1"; fi; }
OUT="${TMPDIR:-/tmp}/the-sending-editmode.xml"
LOG="${TMPDIR:-/tmp}/the-sending-editmode.log"
PS="ProjectSettings/ProjectSettings.asset"
rm -f "$OUT"

bersih=0
git -C "$PROJECT" diff --quiet -- "$PS" && bersih=1

args=(-batchmode -projectPath "$(wp "$PROJECT")" -runTests -testPlatform EditMode -testResults "$(wp "$OUT")" -logFile "$(wp "$LOG")")
if [ -n "${1:-}" ]; then args+=(-testFilter "$1"); fi

set +e
"$UNITY" "${args[@]}"
code=$?
set -e

if [ "$bersih" = 1 ]; then git -C "$PROJECT" checkout -- "$PS"; fi

if [ -f "$OUT" ]; then
  grep -o -m1 'result="[^"]*" total="[0-9]*" passed="[0-9]*" failed="[0-9]*"' "$OUT"
  grep -A4 '<test-case.*result="Failed"' "$OUT" | grep -E 'fullname=|<message>' | sed 's/^ *//' | head -40 || true
else
  echo "Tidak ada hasil test (exit $code). Penyebab umum: editor masih terbuka atau compile error. Log: $LOG"
  grep -E 'error CS|another Unity instance' "$LOG" | sort -u | head -20 || true
fi
exit $code
