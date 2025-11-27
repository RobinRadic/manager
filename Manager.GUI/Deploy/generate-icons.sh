#!/bin/bash
# Generate PNGs using rsvg (renders perfectly)
rsvg-convert -w 16 -h 16 manager.svg -o manager.16x16.png
rsvg-convert -w 24 -h 24 manager.svg -o manager.24x24.png
rsvg-convert -w 32 -h 32 manager.svg -o manager.32x32.png
rsvg-convert -w 48 -h 48 manager.svg -o manager.48x48.png
rsvg-convert -w 64 -h 64 manager.svg -o manager.64x64.png

# Pack them into an ICO
convert manager.16x16.png manager.32x32.png manager.48x48.png manager.64x64.png -colors 256 manager.ico