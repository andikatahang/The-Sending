#!/usr/bin/env bash
# Samakan kenyaringan suara gim: loudnorm dua lintasan ke -18 LUFS (true peak -1,5 dB);
# suara pendek (< 1 dtk) cukup dinormalkan puncaknya ke -3 dBFS.
# Pakai: bash tools/normalisasi-audio.sh                  (semua suara proyek)
#        bash tools/normalisasi-audio.sh file1 file2 ...   (hanya file itu)
#        bash tools/normalisasi-audio.sh -k sumber.aif Assets/_Project/Audio/Music/Nama.ogg   (konversi + normalisasi sekali encode)
# Nama file, format, dan .meta tetap. File yang sudah dalam -1 LU dari target dilewati, jadi aman dijalankan ulang.
# Butuh ffmpeg (brew install ffmpeg). Log sebelum/sesudah: stdout (kolom: file, LUFS, puncak dBFS).
set -euo pipefail
cd "$(dirname "$0")/.."
TARGET=-18 TP=-1.5 PUNCAK=-3
TMP="$(mktemp -d)"; trap 'rm -rf "$TMP"' EXIT

ukur() { # file -> "LUFS puncak" (puncak sampel dBFS)
  local f="$1" l p
  l=$(ffmpeg -nostats -i "$f" -af ebur128=peak=true -f null - 2>&1 | awk '/Integrated loudness:/{g=1} g&&/I:/{print $2; exit}')
  p=$(ffmpeg -nostats -i "$f" -af volumedetect -f null - 2>&1 | awk '/max_volume/{print $5}')
  echo "${l:--70} ${p:--99}"
}
kodek() { # ekstensi + file sumber -> argumen encoder
  case "$(echo "$1" | tr A-Z a-z)" in
    mp3) echo "-c:a libmp3lame -b:a 192k" ;;
    ogg) echo "-c:a vorbis -strict -2 -b:a 160k" ;;
    *)   echo "-c:a $(ffprobe -v error -select_streams a:0 -show_entries stream=codec_name -of csv=p=0 "$2")" ;;
  esac
}
normal() { # sumber tujuan
  local src="$1" dst="$2" ext="${2##*.}" dur sr filter out
  dur=$(ffprobe -v error -show_entries format=duration -of csv=p=0 "$src")
  sr=$(ffprobe -v error -select_streams a:0 -show_entries stream=sample_rate -of csv=p=0 "$src")
  read -r l0 p0 <<<"$(ukur "$src")"
  if [ "$src" = "$dst" ] && awk "BEGIN{exit !(($l0-($TARGET))<1 && ($l0-($TARGET))>-1 && $p0<=($TP)+0.1)}" && awk "BEGIN{exit !($dur>=1)}"; then
    printf '%s\t%s\t%s\t(dilewati)\n' "$dst" "$l0" "$p0"; return
  fi
  if awk "BEGIN{exit !($dur<1)}"; then
    filter="volume=$(awk "BEGIN{print $PUNCAK-($p0)}")dB"
  else
    local j; j=$(ffmpeg -nostats -i "$src" -af "loudnorm=I=$TARGET:TP=$TP:LRA=11:print_format=json" -f null - 2>&1 | sed -n '/^{/,/^}/p')
    local v; v() { echo "$j" | awk -F'"' -v k="$1" '$2==k{print $4}'; }
    filter="loudnorm=I=$TARGET:TP=$TP:LRA=11:measured_I=$(v input_i):measured_TP=$(v input_tp):measured_LRA=$(v input_lra):measured_thresh=$(v input_thresh):offset=$(v target_offset):linear=true"
  fi
  out="$TMP/out.$ext"
  # shellcheck disable=SC2046
  ffmpeg -v error -y -i "$src" -vn -af "$filter" -ar "$sr" $(kodek "$ext" "$src") "$out"
  cp "$out" "$dst"   # cp, bukan mv: .meta dan izin tetap
  read -r l1 p1 <<<"$(ukur "$dst")"
  printf '%s\t%s -> %s LUFS\t%s -> %s dBFS\n' "$dst" "$l0" "$l1" "$p0" "$p1"
}

if [ "${1:-}" = "-k" ]; then
  # konversi: normalkan ke berkas sementara lalu tulis ke tujuan (satu kali encode)
  [ $# -eq 3 ] || { echo "pakai: -k sumber tujuan"; exit 1; }
  ext="${3##*.}"; dur=$(ffprobe -v error -show_entries format=duration -of csv=p=0 "$2")
  j=$(ffmpeg -nostats -i "$2" -af "loudnorm=I=$TARGET:TP=$TP:LRA=11:print_format=json" -f null - 2>&1 | sed -n '/^{/,/^}/p')
  v() { echo "$j" | awk -F'"' -v k="$1" '$2==k{print $4}'; }
  # shellcheck disable=SC2046
  ffmpeg -v error -y -i "$2" -vn -af "loudnorm=I=$TARGET:TP=$TP:LRA=11:measured_I=$(v input_i):measured_TP=$(v input_tp):measured_LRA=$(v input_lra):measured_thresh=$(v input_thresh):offset=$(v target_offset):linear=true" $(kodek "$ext" "$2") "$3"
  echo "dibuat: $3"; exit 0
fi

if [ $# -gt 0 ]; then FILES=("$@"); else
  FILES=(); while IFS= read -r f; do FILES+=("$f"); done < <(find Assets/_Project/Audio/SFX Assets/_Project/Audio/Music Assets/_Project/Audio/Resources Assets/_Project/Audio/ThirdParty/FootstepsEssentials -type f \( -iname '*.wav' -o -iname '*.mp3' -o -iname '*.ogg' \) | sort)
fi
for f in "${FILES[@]}"; do normal "$f" "$f"; done
