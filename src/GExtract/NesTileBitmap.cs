using System.Buffers.Binary;

namespace GExtract;

/// <summary>
/// Writes NES tiles to a 24-bit bitmap. Each tile is 8x8 and 16 bytes
/// (bitplane 0, then bitplane 1). Layout and colors follow Graphics
/// Extractor v1.1 by Griffin Knodle, 1998: 32 tiles per row, with
/// black, blue, red, and white standing in for the four tile colors.
/// </summary>
public static class NesTileBitmap
{
    public const int BytesPerTile = 16;
    public const int TilePixels = 8;
    public const int TilesPerRow = 32;

    public static void Write(
        ReadOnlySpan<byte> source,
        int offset,
        int tileCount,
        Stream output,
        Action<int>? tilesCompleted,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(output);

        if (tileCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(tileCount));

        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        if ((long)offset + ((long)tileCount * BytesPerTile) > source.Length)
            throw new ArgumentOutOfRangeException(nameof(tileCount), "The source does not contain that many tiles.");

        int width = Math.Min(tileCount, TilesPerRow) * TilePixels;
        long tileRowsLong = ((long)tileCount + TilesPerRow - 1) / TilesPerRow;
        long heightLong = tileRowsLong * TilePixels;
        int stride = (width * 3 + 3) & ~3;
        long imageSize = (long)stride * heightLong;

        if (tileRowsLong > int.MaxValue || heightLong > int.MaxValue || imageSize > int.MaxValue - 54)
            throw new InvalidOperationException("The bitmap would be too large to write.");

        int tileRows = (int)tileRowsLong;
        int height = (int)heightLong;

        WriteHeader(output, width, height, stride, (int)imageSize);

        var row = new byte[stride];
        int completed = 0;

        for (int tileRow = tileRows - 1; tileRow >= 0; tileRow--)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int tilesInRow = Math.Min(TilesPerRow, tileCount - (tileRow * TilesPerRow));
            int rowSource = offset + (tileRow * TilesPerRow * BytesPerTile);

            for (int pixelRow = TilePixels - 1; pixelRow >= 0; pixelRow--)
            {
                Array.Clear(row);
                DecodePixelRow(source, rowSource, tilesInRow, pixelRow, width, row);
                output.Write(row);
            }

            completed += tilesInRow;
            tilesCompleted?.Invoke(completed);
        }
    }

    private static void DecodePixelRow(
        ReadOnlySpan<byte> source,
        int rowSource,
        int tilesInRow,
        int pixelRow,
        int width,
        byte[] row)
    {
        int x = 0;

        for (int tile = 0; tile < tilesInRow; tile++)
        {
            int tileStart = rowSource + (tile * BytesPerTile);
            byte plane0 = source[tileStart + pixelRow];
            byte plane1 = source[tileStart + 8 + pixelRow];

            for (int bit = 7; bit >= 0; bit--)
            {
                int value = ((plane0 >> bit) & 1) | (((plane1 >> bit) & 1) << 1);
                int index = x * 3;

                switch (value)
                {
                    case 1:
                        row[index] = 0xFF;
                        break;
                    case 2:
                        row[index + 2] = 0xFF;
                        break;
                    case 3:
                        row[index] = 0xFF;
                        row[index + 1] = 0xFF;
                        row[index + 2] = 0xFF;
                        break;
                }

                x++;
            }
        }

        for (; x < width; x++)
        {
            int index = x * 3;
            row[index] = 0x80;
            row[index + 1] = 0x80;
            row[index + 2] = 0x80;
        }
    }

    private static void WriteHeader(Stream output, int width, int height, int stride, int imageSize)
    {
        Span<byte> header = stackalloc byte[54];
        header[0] = (byte)'B';
        header[1] = (byte)'M';
        BinaryPrimitives.WriteInt32LittleEndian(header[2..], 54 + imageSize);
        BinaryPrimitives.WriteInt32LittleEndian(header[10..], 54);
        BinaryPrimitives.WriteInt32LittleEndian(header[14..], 40);
        BinaryPrimitives.WriteInt32LittleEndian(header[18..], width);
        BinaryPrimitives.WriteInt32LittleEndian(header[22..], height);
        BinaryPrimitives.WriteInt16LittleEndian(header[26..], 1);
        BinaryPrimitives.WriteInt16LittleEndian(header[28..], 24);
        BinaryPrimitives.WriteInt32LittleEndian(header[34..], imageSize);
        BinaryPrimitives.WriteInt32LittleEndian(header[38..], 2835);
        BinaryPrimitives.WriteInt32LittleEndian(header[42..], 2835);
        output.Write(header);
    }
}
