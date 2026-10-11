namespace GExtract;

public enum ExtractionMode
{
    ChrBanks,
    AllChr,
    EntireFile
}

public readonly record struct ExtractionProgress(int CompletedTiles, int TotalTiles, string Status);

/// <summary>
/// Extracts NES graphics tiles into 24-bit bitmap files.
/// </summary>
public static class GraphicsExtractor
{
    public static int CountTiles(NesRom rom, ExtractionMode mode)
    {
        ArgumentNullException.ThrowIfNull(rom);

        return mode switch
        {
            ExtractionMode.EntireFile => rom.TileCount,
            ExtractionMode.ChrBanks or ExtractionMode.AllChr => rom.ChrTileCount,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown extraction mode.")
        };
    }

    public static IReadOnlyList<string> Extract(
        NesRom rom,
        string outputDirectory,
        ExtractionMode mode,
        IProgress<ExtractionProgress>? progress,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(rom);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        int totalTiles = CountTiles(rom, mode);
        if (totalTiles <= 0)
        {
            throw new InvalidOperationException(mode == ExtractionMode.EntireFile
                ? "The file does not contain a complete tile (16 bytes)."
                : "This ROM has no CHR graphics to extract.");
        }

        Directory.CreateDirectory(outputDirectory);

        var files = new List<string>();
        string baseName = MakeBaseName(rom);

        switch (mode)
        {
            case ExtractionMode.ChrBanks:
                WriteChrBanks(rom, outputDirectory, baseName, totalTiles, files, progress, cancellationToken);
                break;

            case ExtractionMode.AllChr:
                WriteBitmap(
                    rom.Data,
                    Path.Combine(outputDirectory, baseName + ".chr.bmp"),
                    rom.ChrOffset,
                    totalTiles,
                    0,
                    totalTiles,
                    files,
                    progress,
                    cancellationToken);
                break;

            case ExtractionMode.EntireFile:
                WriteBitmap(
                    rom.Data,
                    Path.Combine(outputDirectory, baseName + ".tiles.bmp"),
                    0,
                    totalTiles,
                    0,
                    totalTiles,
                    files,
                    progress,
                    cancellationToken);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown extraction mode.");
        }

        return files;
    }

    private static void WriteChrBanks(
        NesRom rom,
        string outputDirectory,
        string baseName,
        int totalTiles,
        List<string> files,
        IProgress<ExtractionProgress>? progress,
        CancellationToken cancellationToken)
    {
        int completed = 0;

        for (int bank = 0; bank < rom.ChrBankCount; bank++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int bankByteOffset = rom.ChrOffset + (bank * NesRom.ChrBankSize);
            int bankBytes = Math.Min(NesRom.ChrBankSize, rom.ChrSize - (bank * NesRom.ChrBankSize));
            int tiles = bankBytes / NesTileBitmap.BytesPerTile;
            if (tiles == 0)
                continue;

            string fileName = $"{baseName}.chr-{bank:00}.bmp";
            int completedBefore = completed;

            WriteBitmap(
                rom.Data,
                Path.Combine(outputDirectory, fileName),
                bankByteOffset,
                tiles,
                completedBefore,
                totalTiles,
                files,
                progress,
                cancellationToken,
                $"{fileName} ({bank + 1} of {rom.ChrBankCount})");

            completed += tiles;
        }
    }

    private static void WriteBitmap(
        byte[] data,
        string path,
        int sourceOffset,
        int tileCount,
        int completedBefore,
        int totalTiles,
        List<string> files,
        IProgress<ExtractionProgress>? progress,
        CancellationToken cancellationToken,
        string? statusName = null)
    {
        string displayName = statusName ?? Path.GetFileName(path);
        var completedWrite = false;

        try
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                NesTileBitmap.Write(
                    data,
                    sourceOffset,
                    tileCount,
                    stream,
                    written =>
                    {
                        progress?.Report(new ExtractionProgress(
                            completedBefore + written,
                            totalTiles,
                            $"Writing {displayName} - {completedBefore + written:N0} of {totalTiles:N0} tiles"));
                    },
                    cancellationToken);
            }

            completedWrite = true;
            files.Add(path);
        }
        finally
        {
            if (!completedWrite)
            {
                try
                {
                    File.Delete(path);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
    }

    private static string MakeBaseName(NesRom rom)
    {
        var name = Path.GetFileNameWithoutExtension(rom.FilePath);
        if (string.IsNullOrWhiteSpace(name))
            return "rom";

        var invalid = Path.GetInvalidFileNameChars();
        if (name.IndexOfAny(invalid) < 0)
            return name;

        var chars = name.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (Array.IndexOf(invalid, chars[i]) >= 0)
                chars[i] = '_';
        }

        return new string(chars);
    }
}
