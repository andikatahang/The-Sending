#!/usr/bin/env bash
# Menjalankan EditMode test The Sending lewat command line. Editor Unity harus tertutup.
# Pemakaian: bash tools/run-editmode-tests.sh [testFilter]
# Batchmode menghapus define SENTIS_ANALYTICS_ENABLED dari ProjectSettings.asset; skrip ini memulihkannya
# hanya jika file itu bersih sebelum Unity dijalankan (perubahan sengaja tidak ikut terhapus).
set -euo pipefail
UNITY="/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity"
PROJECT="$(cd "$(dirname "$0")/.." && pwd)"
OUT="${TMPDIR:-/tmp}/the-sending-editmode.xml"
LOG="${TMPDIR:-/tmp}/the-sending-editmode.log"
PS="ProjectSettings/ProjectSettings.asset"
rm -f "$OUT"

bersih=0
git -C "$PROJECT" diff --quiet -- "$PS" && bersih=1

args=(-batchmode -projectPath "$PROJECT" -runTests -testPlatform EditMode -testResults "$OUT" -logFile "$LOG")
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
