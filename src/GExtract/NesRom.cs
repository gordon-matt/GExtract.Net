namespace GExtract;

/// <summary>
/// An iNES ROM, or a raw file that can still be read as tiles.
/// </summary>
public sealed class NesRom
{
    public const int HeaderSize = 16;
    public const int PrgBankSize = 16 * 1024;
    public const int ChrBankSize = 8 * 1024;
    public const int TrainerSize = 512;

    private NesRom(
        string filePath,
        byte[] data,
        bool hasInesHeader,
        int mapper,
        int prgSize,
        int chrSize,
        int chrOffset,
        bool hasTrainer)
    {
        FilePath = filePath;
        Data = data;
        HasInesHeader = hasInesHeader;
        Mapper = mapper;
        PrgSize = prgSize;
        ChrSize = chrSize;
        ChrOffset = chrOffset;
        HasTrainer = hasTrainer;
    }

    public string FilePath { get; }

    public byte[] Data { get; }

    public bool HasInesHeader { get; }

    public int Mapper { get; }

    public int PrgSize { get; }

    public int ChrSize { get; }

    public int ChrOffset { get; }

    public bool HasTrainer { get; }

    public int ChrBankCount => ChrSize == 0 ? 0 : (ChrSize + ChrBankSize - 1) / ChrBankSize;

    public int ChrTileCount => ChrSize / NesTileBitmap.BytesPerTile;

    public int TileCount => Data.Length / NesTileBitmap.BytesPerTile;

    public static NesRom Load(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        var data = File.ReadAllBytes(filePath);
        return Parse(data, Path.GetFullPath(filePath));
    }

    public static NesRom Parse(byte[] data, string filePath)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (TryReadHeader(data, out var mapper, out var prgSize, out var chrSize, out var chrOffset, out var hasTrainer))
        {
            return new NesRom(filePath, data, true, mapper, prgSize, chrSize, chrOffset, hasTrainer);
        }

        return new NesRom(filePath, data, false, 0, 0, 0, 0, false);
    }

    private static bool TryReadHeader(
        byte[] data,
        out int mapper,
        out int prgSize,
        out int chrSize,
        out int chrOffset,
        out bool hasTrainer)
    {
        mapper = 0;
        prgSize = 0;
        chrSize = 0;
        chrOffset = 0;
        hasTrainer = false;

        if (data.Length < HeaderSize)
            return false;

        if (data[0] != (byte)'N' || data[1] != (byte)'E' || data[2] != (byte)'S' || data[3] != 0x1A)
            return false;

        bool nes2 = (data[7] & 0x0C) == 0x08;
        int prgUnits = data[4];
        int chrUnits = data[5];
        mapper = (data[6] >> 4) | (data[7] & 0xF0);

        if (nes2)
        {
            mapper |= (data[8] & 0x0F) << 8;
            prgUnits |= (data[9] & 0xF0) << 4;
            chrUnits |= (data[9] & 0x0F) << 8;

            // Exponent-encoded NES 2.0 sizes are not used by the sample ROMs.
            if ((prgUnits & 0xFFF) == 0xFFF || (chrUnits & 0xFFF) == 0xFFF)
                return false;
        }

        hasTrainer = (data[6] & 0x04) != 0;
        long prgBytes = (long)prgUnits * PrgBankSize;
        long chrBytes = (long)chrUnits * ChrBankSize;
        long offset = HeaderSize + (hasTrainer ? TrainerSize : 0) + prgBytes;
        long total = offset + chrBytes;

        if (total > data.Length || prgBytes > int.MaxValue || chrBytes > int.MaxValue || offset > int.MaxValue)
            return false;

        prgSize = (int)prgBytes;
        chrSize = (int)chrBytes;
        chrOffset = (int)offset;
        return true;
    }
}
