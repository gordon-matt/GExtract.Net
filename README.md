# GExtract

GExtract is a Windows Forms app that reads graphics tiles out of an NES ROM and saves them as 24-bit bitmaps.

It is a .NET 10 version of Graphics Extractor v1.1, written by Griffin Knodle in 1998. The tile layout and placeholder colors follow that program: 32 tiles per row, with black, blue, red, and white standing in for the four colors stored in each tile.

## What it extracts

NES graphics are stored as 8×8 tiles. Each tile is 16 bytes: eight bytes for the first bitplane, then eight bytes for the second. Two bits per pixel give four colors. GExtract does not apply a game's actual palette. It uses fixed colors so the tile shapes are easy to see in any image editor:

| Tile value | Color |
| --- | --- |
| 0 | Black |
| 1 | Blue |
| 2 | Red |
| 3 | White |

A short final row is padded with gray when the bitmap is wider than the remaining tiles.

Most cartridge graphics live in CHR ROM, after the 16-byte iNES header and the program ROM. GExtract reads that header and offers three ways to write the tiles:

- **CHR banks** writes one bitmap per 8 KB bank. This is the default. A game such as *Flying Warriors* (16 CHR banks) produces 16 files named `Game.chr-00.bmp` through `Game.chr-15.bmp`.
- **All CHR graphics** writes every CHR tile into one bitmap, `Game.chr.bmp`.
- **Entire file** writes the whole file from the first byte, including the header and program code, as `Game.tiles.bmp`. Program code looks like static. Use this when a file has no CHR ROM, or when you want to search the whole image for tiles.

Each bitmap is 256 pixels wide (32 tiles). A full 8 KB bank is 128 pixels tall.

If the file is not a valid iNES ROM, only **Entire file** is available. The file is still read as raw 16-byte tiles.

## Run the app

You need the .NET 10 SDK on Windows.

```powershell
dotnet run --project src/GExtract
```

Or open `GExtract.Net.slnx` in Visual Studio and start the **GExtract** project.

1. Choose a `.nes` file. You can also drop a file onto the window.
2. Choose an output folder. If you leave the suggested path, GExtract creates a folder named `<rom name> graphics` next to the ROM.
3. Pick what to extract.
4. Click **Extract**.

The progress bar advances by tile. **Extract** becomes **Cancel** while the bitmaps are being written. Closing the window during an extraction cancels it and deletes the bitmap that was in progress.

## Project layout

```
src/GExtract/           WinForms app
  MainForm.cs           Extraction UI
  NesRom.cs             iNES header parsing
  NesTileBitmap.cs      Tile decoding and BMP writing
  GraphicsExtractor.cs  Writes CHR banks, all CHR, or the whole file
tests/GExtract.Tests/   Unit tests, including checks against sample ROMs
```

## Credits

Tile decoding follows Graphics Extractor v1.1 by Griffin Knodle (Jair), 15 October 1998. That program was distributed with its source, and this project is a new implementation of the same extraction.
