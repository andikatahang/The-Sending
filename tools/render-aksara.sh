#!/usr/bin/env bash
# Gambar aksara Jawa untuk puzzle Malam 2 (PanelAksara), dirender Google Chrome headless dengan font Noto Sans Javanese,
# supaya sandhangan dan pasangan tersusun benar (uGUI tidak bisa menyusun aksara Jawa).
# Hasil: Assets/_Project/Art/UI/Aksara/Suku_<n>.png (tulisan di boneka) dan Tabel.png (Halaman 2).
# Pemakaian: bash tools/render-aksara.sh   (jalankan ulang setelah mengubah isi di bawah)
set -euo pipefail
AKAR="$(cd "$(dirname "$0")/.." && pwd)"
FONT="$AKAR/Assets/_Project/Art/Fonts/Resources/NotoSansJavanese-Regular.ttf"
KELUAR="$AKAR/Assets/_Project/Art/UI/Aksara"
CHROME="/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"
[ -f "$FONT" ] || { echo "Font belum ada: $FONT (lihat docs/aset/fase-2.md)"; exit 1; }
[ -x "$CHROME" ] || { echo "Google Chrome tidak ditemukan: $CHROME"; exit 1; }
mkdir -p "$KELUAR"
SEMENTARA="$(mktemp -d)"
trap 'rm -rf "$SEMENTARA"' EXIT
URL_FONT="file://${FONT// /%20}"

# render <nama> <lebar> <tinggi> <css tambahan> <isi html>
render() {
  cat > "$SEMENTARA/$1.html" <<EOF
<!DOCTYPE html><html><head><meta charset="utf-8"><style>
@font-face { font-family: Jawa; src: url("$URL_FONT"); }
html, body { margin: 0; background: transparent; color: #E6E0D1; }
body { width: ${2}px; height: ${3}px; font-family: Jawa, sans-serif; }
$4
</style></head><body>$5</body></html>
EOF
  rm -f "$KELUAR/$1.png"
  "$CHROME" --headless=new --disable-gpu --hide-scrollbars --allow-file-access-from-files \
    --default-background-color=00000000 --virtual-time-budget=3000 \
    --window-size="$2,$3" --screenshot="$KELUAR/$1.png" "file://$SEMENTARA/$1.html" >/dev/null 2>&1 || true
  [ -s "$KELUAR/$1.png" ] || { echo "Gagal membuat $1.png"; exit 1; }
  echo "  $1.png"
}

# Tulisan di boneka, urutan sama dengan BacaanAksara.Kata: RA T MI | SE LA SA KLI WO N
SUKU=( "&#xA9AB;" "&#xA9A0;&#xA9C0;" "&#xA9A9;&#xA9B6;"
       "&#xA9B1;&#xA9BA;" "&#xA9AD;" "&#xA9B1;" "&#xA98F;&#xA9C0;&#xA9AD;&#xA9B6;" "&#xA9AE;&#xA9BA;&#xA9B4;" "&#xA9A4;&#xA9C0;" )
for i in "${!SUKU[@]}"; do
  render "Suku_$i" 256 256 \
    "body { display: flex; align-items: center; justify-content: center; font-size: 92px; line-height: 1; }" \
    "<span>${SUKU[$i]}</span>"
done

# Tabel Halaman 2: 20 aksara dasar dan sandhangan yang dipakai tulisan di boneka (contoh dengan ka).
SEL=""
for p in "A9B2 ha" "A9A4 na" "A995 ca" "A9AB ra" "A98F ka" "A9A2 da" "A9A0 ta" "A9B1 sa" "A9AE wa" "A9AD la" \
         "A9A5 pa" "A99D dha" "A997 ja" "A9AA ya" "A99A nya" "A9A9 ma" "A992 ga" "A9A7 ba" "A99B tha" "A994 nga"; do
  set -- $p
  SEL="$SEL<div><b>&#x$1;</b>$2</div>"
done
SANDHANGAN="<div><b>&#xA98F;&#xA9B6;</b>ki (wulu)</div><div><b>&#xA98F;&#xA9BA;</b>ke (taling)</div><div><b>&#xA98F;&#xA9BA;&#xA9B4;</b>ko (taling tarung)</div><div><b>&#xA98F;&#xA9C0;</b>k (pangkon)</div><div><b>&#xA98F;&#xA9C0;&#xA9AD;</b>kla (pasangan la)</div>"
render "Tabel" 768 960 \
  "body { padding: 24px; box-sizing: border-box; } h1, h2 { font-family: sans-serif; font-size: 30px; margin: 0 0 10px; }
   .g { display: grid; grid-template-columns: repeat(5, 1fr); gap: 4px; margin-bottom: 18px; row-gap: 10px; }
   .g div { text-align: center; font-family: sans-serif; font-size: 24px; }
   .g b { display: block; font-family: Jawa; font-size: 54px; font-weight: normal; line-height: 1.35; height: 110px; }" \
  "<h1>Aksara Jawa</h1><div class=\"g\">$SEL</div><h2>Sandhangan</h2><div class=\"g\">$SANDHANGAN</div>"
echo "Selesai: $KELUAR"
