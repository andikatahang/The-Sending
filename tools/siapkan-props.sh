#!/usr/bin/env bash
# Ekstrak arsip props, pindah ke Art/ThirdParty, hapus arsip dan file tak terpakai. Editor Unity harus tertutup.
# kulkas tidak dipakai (keputusan tim): dibiarkan di tempatnya.
set -euo pipefail
cd "$(dirname "$0")/.."
SRC="Assets/Props TheSending"; DST="Assets/_Project/Art/ThirdParty"
[ -d "$SRC/source" ] || { echo "Sudah dipindah."; exit 0; }
x() { [ -f "$1" ] || return 0; /usr/bin/bsdtar -xf "$1" -C "$2"; rm -f "$1" "$1.meta"; }
x "$SRC/antique-wall-clock/source/clock.zip"        "$SRC/antique-wall-clock/source"
x "$SRC/door-padlock-key/source/padlock.zip"        "$SRC/door-padlock-key/source"
x "$SRC/low-poly-door/source/PortalDoor.zip"        "$SRC/low-poly-door/source"
x "$SRC/toilet/source/Toilet.zip"                   "$SRC/toilet/source"
x "$SRC/wooden-cabinet/source/Cabinet_old.zip"      "$SRC/wooden-cabinet/source"
x "$SRC/led-tv/source/led tv.rar"                   "$SRC/led-tv/source"
# rar besar: ambil model saja (tekstur sudah ada di textures/)
[ ! -f "$SRC/simple-wood-fence/source/Simple Wood Fence.rar" ] || /usr/bin/bsdtar -xf "$SRC/simple-wood-fence/source/Simple Wood Fence.rar" -C "$SRC/simple-wood-fence/source" --include '*.obj'
rm -f "$SRC/simple-wood-fence/source/Simple Wood Fence.rar"*
[ ! -f "$SRC/wooden-kitchen-table-chair/source/Mesa e cadeiras.rar" ] || /usr/bin/bsdtar -xf "$SRC/wooden-kitchen-table-chair/source/Mesa e cadeiras.rar" -C "$SRC/wooden-kitchen-table-chair/source" --include '*.fbx'
rm -f "$SRC/wooden-kitchen-table-chair/source/Mesa e cadeiras.rar"*
# tekstur ganda/berlebih dari zip (textures/ sudah punya), dan file yang tak dipakai Unity
find "$SRC" -path "$SRC/kulkas" -prune -o \( -name '*.tga' -o -name '*.tif' -o -name '*.tiff' -o -name '*.mtlx' -o -name '*.dae' -o -name 'Thumbs.db' \
  -o -name '*Height*' -o -name '*Displacement*' -o -name '*Preview*' -o -name 'internal_ground_ao_texture.jpeg' \) -print -delete
find "$SRC/wooden-cabinet/source" \( -name 'Cabinet_old.obj' -o -name 'Cabinet_old.mtl' \) -delete
rm -rf "$SRC/source-3" "$SRC/source-3.meta" "$SRC/well-building-sumur/textures/Ground_"*
mkdir -p "$DST" "$DST/props-lepas"
mv "$SRC/source" "$DST/gapura"; mv "$SRC/source-2" "$DST/lampu-plafon"; mv "$SRC/source-BbLght" "$DST/bohlam"
rm -f "$SRC/source.meta" "$SRC/source-2.meta" "$SRC/source-BbLght.meta"
for f in "$SRC"/*.jpg "$SRC"/*.glb; do mv "$f" "$DST/props-lepas/"; [ -f "$f.meta" ] && mv "$f.meta" "$DST/props-lepas/"; done
for d in "$SRC"/*/; do
  n=$(basename "$d"); [ "$n" = kulkas ] && continue
  mv "$d" "$DST/"; [ -f "$SRC/$n.meta" ] && mv "$SRC/$n.meta" "$DST/"
done
echo "Props siap di $DST"
