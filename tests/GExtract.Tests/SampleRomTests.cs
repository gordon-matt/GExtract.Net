namespace GExtract.Tests;

public class SampleRomTests
{
    private const string SampleDirectory = @"D:\Users\Matt\Downloads\gextract\Sample ROMs";

    [Fact]
    public void Sample_headers_match_the_rom_files()
    {
        AssertRom("Flying Warriors (USA).nes", mapper: 1, prg: 128 * 1024, chr: 128 * 1024, banks: 16);
        AssertRom("Super Mario Bros. (World) (HVC-SM).nes", mapper: 0, prg: 32 * 1024, chr: 8 * 1024, banks: 1);
        AssertRom("Super Mario Bros. 2 (USA).nes", mapper: 4, prg: 128 * 1024, chr: 128 * 1024, banks: 16);
        AssertRom("Super Mario Bros. 3 (USA).nes", mapper: 4, prg: 256 * 1024, chr: 128 * 1024, banks: 16);
    }

    [Fact]
    public void Flying_Warriors_chr_banks_match_the_rom_bytes()
    {
        var rom = Load("Flying Warriors (USA).nes");
        var directory = Directory.CreateTempSubdirectory("gextract-fw");

        try
        {
            var files = GraphicsExtractor.Extract(rom, directory.FullName, ExtractionMode.ChrBanks, progress: null, CancellationToken.None);

            Assert.Equal(16, files.Count);
            Assert.Contains(files, path => path.EndsWith("Flying Warriors (USA).chr-00.bmp", StringComparison.Ordinal));
            Assert.Contains(files, path => path.EndsWith("Flying Warriors (USA).chr-15.bmp", StringComparison.Ordinal));

            using (var first = new Bitmap(files[0]))
            {
                Assert.Equal(256, first.Width);
                Assert.Equal(128, first.Height);
                AssertTile(first, rom.Data, rom.ChrOffset, tileX: 0, tileY: 0);
                AssertTile(first, rom.Data, rom.ChrOffset + (31 * 16), tileX: 31, tileY: 0);
            }

            using (var second = new Bitmap(files[1]))
            {
                AssertTile(second, rom.Data, rom.ChrOffset + NesRom.ChrBankSize, tileX: 0, tileY: 0);
            }

            Assert.True(CountColors(files[0]) > 1);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Fact]
    public void Flying_Warriors_combined_chr_keeps_bank_order()
    {
        var rom = Load("Flying Warriors (USA).nes");
        var directory = Directory.CreateTempSubdirectory("gextract-fw-all");

        try
        {
            var files = GraphicsExtractor.Extract(rom, directory.FullName, ExtractionMode.AllChr, progress: null, CancellationToken.None);

            Assert.Single(files);
            using var bmp = new Bitmap(files[0]);
            Assert.Equal(256, bmp.Width);
            Assert.Equal(2048, bmp.Height);
            AssertTile(bmp, rom.Data, rom.ChrOffset, tileX: 0, tileY: 0);
            AssertTile(bmp, rom.Data, rom.ChrOffset + NesRom.ChrBankSize, tileX: 0, tileY: 16);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Fact]
    public void Mario_entire_file_pads_the_final_row()
    {
        var rom = Load("Super Mario Bros. (World) (HVC-SM).nes");
        var directory = Directory.CreateTempSubdirectory("gextract-smb");

        try
        {
            var files = GraphicsExtractor.Extract(rom, directory.FullName, ExtractionMode.EntireFile, progress: null, CancellationToken.None);
            using var bmp = new Bitmap(files[0]);

            Assert.Equal(256, bmp.Width);
            Assert.Equal(648, bmp.Height);
            AssertTile(bmp, rom.Data, sourceOffset: 0, tileX: 0, tileY: 0);
            AssertTile(bmp, rom.Data, sourceOffset: 2560 * 16, tileX: 0, tileY: 80);
            Assert.Equal(Color.FromArgb(0x80, 0x80, 0x80).ToArgb(), bmp.GetPixel(8, 640).ToArgb());
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Theory]
    [InlineData("Flying Warriors (USA).nes")]
    [InlineData("Super Mario Bros. (World) (HVC-SM).nes")]
    [InlineData("Super Mario Bros. 2 (USA).nes")]
    [InlineData("Super Mario Bros. 3 (USA).nes")]
    public void Every_sample_chr_bank_matches_the_rom(string fileName)
    {
        var rom = Load(fileName);
        var directory = Directory.CreateTempSubdirectory("gextract-sample");

        try
        {
            var files = GraphicsExtractor.Extract(rom, directory.FullName, ExtractionMode.ChrBanks, progress: null, CancellationToken.None);

            Assert.Equal(rom.ChrBankCount, files.Count);

            using var bmp = new Bitmap(files[0]);
            Assert.Equal(256, bmp.Width);
            Assert.Equal(128, bmp.Height);
            AssertTile(bmp, rom.Data, rom.ChrOffset, tileX: 0, tileY: 0);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    private static void AssertRom(string fileName, int mapper, int prg, int chr, int banks)
    {
        var rom = Load(fileName);
        Assert.True(rom.HasInesHeader);
        Assert.Equal(mapper, rom.Mapper);
        Assert.Equal(prg, rom.PrgSize);
        Assert.Equal(chr, rom.ChrSize);
        Assert.Equal(16 + prg, rom.ChrOffset);
        Assert.Equal(banks, rom.ChrBankCount);
        Assert.Equal(chr / 16, rom.ChrTileCount);
    }

    private static NesRom Load(string fileName)
    {
        var path = Path.Combine(SampleDirectory, fileName);
        Assert.True(File.Exists(path), $"Sample ROM was not found: {path}");
        return NesRom.Load(path);
    }

    private static void AssertTile(Bitmap bmp, byte[] rom, int sourceOffset, int tileX, int tileY)
    {
        for (int py = 0; py < 8; py++)
        {
            byte plane0 = rom[sourceOffset + py];
            byte plane1 = rom[sourceOffset + 8 + py];

            for (int px = 0; px < 8; px++)
            {
                int bit = 7 - px;
                int value = ((plane0 >> bit) & 1) | (((plane1 >> bit) & 1) << 1);
                var expected = value switch
                {
                    0 => Color.Black,
                    1 => Color.FromArgb(0, 0, 255),
                    2 => Color.FromArgb(255, 0, 0),
                    _ => Color.White
                };

                Assert.Equal(expected.ToArgb(), bmp.GetPixel((tileX * 8) + px, (tileY * 8) + py).ToArgb());
            }
        }
    }

    private static int CountColors(string path)
    {
        using var bmp = new Bitmap(path);
        var colors = new HashSet<int>();

        for (int y = 0; y < bmp.Height; y += 4)
        {
            for (int x = 0; x < bmp.Width; x += 4)
                colors.Add(bmp.GetPixel(x, y).ToArgb());
        }

        return colors.Count;
    }
}
