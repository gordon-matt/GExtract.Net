using System.Buffers.Binary;

namespace GExtract.Tests;

public class NesTileBitmapTests
{
    [Fact]
    public void One_tile_uses_bitplanes_and_bottom_up_rows()
    {
        var tile = new byte[16];
        tile[0] = 0b1000_0000;
        tile[2] = 0b0001_0000;
        tile[7] = 0b0000_0001;
        tile[8] = 0b0100_0000;
        tile[9] = 0b0010_0000;
        tile[10] = 0b0001_1000;

        using var bmp = Write(tile, tileCount: 1);

        Assert.Equal(8, bmp.Width);
        Assert.Equal(8, bmp.Height);
        AssertColor(Color.FromArgb(0, 0, 255), bmp, 0, 0);
        AssertColor(Color.FromArgb(255, 0, 0), bmp, 1, 0);
        AssertColor(Color.Black, bmp, 2, 0);
        AssertColor(Color.FromArgb(255, 0, 0), bmp, 2, 1);
        AssertColor(Color.White, bmp, 3, 2);
        AssertColor(Color.FromArgb(255, 0, 0), bmp, 4, 2);
        AssertColor(Color.FromArgb(0, 0, 255), bmp, 7, 7);
    }

    [Fact]
    public void Partial_last_row_is_gray_and_the_first_tiles_stay_on_top()
    {
        var data = new byte[33 * 16];
        data[0] = 0xFF;
        data[32 * 16] = 0x00;
        data[(32 * 16) + 8] = 0x80;

        using var bmp = Write(data, tileCount: 33);

        Assert.Equal(256, bmp.Width);
        Assert.Equal(16, bmp.Height);
        AssertColor(Color.FromArgb(0, 0, 255), bmp, 0, 0);
        AssertColor(Color.Black, bmp, 8, 0);
        AssertColor(Color.FromArgb(255, 0, 0), bmp, 0, 8);
        AssertColor(Color.FromArgb(0x80, 0x80, 0x80), bmp, 8, 8);
        AssertColor(Color.FromArgb(0x80, 0x80, 0x80), bmp, 255, 15);
    }

    [Fact]
    public void Header_records_the_real_file_size()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".bmp");
        try
        {
            using (var stream = File.Create(path))
            {
                NesTileBitmap.Write(new byte[16], 0, 1, stream, null, CancellationToken.None);
            }

            var bytes = File.ReadAllBytes(path);
            Assert.Equal((byte)'B', bytes[0]);
            Assert.Equal((byte)'M', bytes[1]);
            Assert.Equal(bytes.Length, BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(2)));
            Assert.Equal(8, BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(18)));
            Assert.Equal(8, BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(22)));
            Assert.Equal(24, BinaryPrimitives.ReadInt16LittleEndian(bytes.AsSpan(28)));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Cancel_removes_the_partial_file()
    {
        var data = new byte[64 * 16];
        var rom = NesRom.Parse(data, "raw.bin");
        var directory = Directory.CreateTempSubdirectory("gextract-cancel");

        try
        {
            using var cancellation = new CancellationTokenSource();
            var progress = new InlineProgress(_ => cancellation.Cancel());

            Assert.Throws<OperationCanceledException>(() =>
                GraphicsExtractor.Extract(rom, directory.FullName, ExtractionMode.EntireFile, progress, cancellation.Token));

            Assert.Empty(Directory.GetFiles(directory.FullName));
        }
        finally
        {
            directory.Delete(true);
        }
    }

    private static Bitmap Write(byte[] data, int tileCount)
    {
        using var stream = new MemoryStream();
        NesTileBitmap.Write(data, 0, tileCount, stream, null, CancellationToken.None);
        stream.Position = 0;
        using var loaded = new Bitmap(stream);
        return new Bitmap(loaded);
    }

    private static void AssertColor(Color expected, Bitmap bitmap, int x, int y)
    {
        Assert.Equal(expected.ToArgb(), bitmap.GetPixel(x, y).ToArgb());
    }

    private sealed class InlineProgress : IProgress<ExtractionProgress>
    {
        private readonly Action<ExtractionProgress> _report;

        public InlineProgress(Action<ExtractionProgress> report) => _report = report;

        public void Report(ExtractionProgress value) => _report(value);
    }
}
