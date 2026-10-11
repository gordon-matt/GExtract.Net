namespace GExtract.Tests;

public class NesRomTests
{
    [Fact]
    public void Ines_header_reads_prg_chr_and_mapper()
    {
        var data = Header(prgUnits: 2, chrUnits: 1, flags6: 0x10);
        var rom = NesRom.Parse(data, "sample.nes");

        Assert.True(rom.HasInesHeader);
        Assert.Equal(1, rom.Mapper);
        Assert.Equal(32 * 1024, rom.PrgSize);
        Assert.Equal(8 * 1024, rom.ChrSize);
        Assert.Equal(16 + (32 * 1024), rom.ChrOffset);
        Assert.False(rom.HasTrainer);
        Assert.Equal(1, rom.ChrBankCount);
        Assert.Equal(512, rom.ChrTileCount);
    }

    [Fact]
    public void Trainer_shifts_chr_past_the_trainer_and_prg()
    {
        int prg = NesRom.PrgBankSize;
        int chr = NesRom.ChrBankSize;
        var data = Header(prgUnits: 1, chrUnits: 1, flags6: 0x04, prgBytes: prg, chrBytes: chr, trainer: true);
        var rom = NesRom.Parse(data, "trainer.nes");

        Assert.True(rom.HasTrainer);
        Assert.Equal(16 + NesRom.TrainerSize + prg, rom.ChrOffset);
    }

    [Fact]
    public void Nes2_extension_that_does_not_fit_is_read_as_raw_tiles()
    {
        var data = Header(prgUnits: 1, chrUnits: 1, flags6: 0, extraLength: 32);
        data[7] = 0x08;
        data[9] = 0x10;

        var rom = NesRom.Parse(data, "bad.nes");

        Assert.False(rom.HasInesHeader);
        Assert.Equal(0, rom.ChrSize);
        Assert.Equal(data.Length / 16, rom.TileCount);
    }

    [Fact]
    public void Missing_magic_is_raw()
    {
        var data = new byte[64];
        var rom = NesRom.Parse(data, "raw.bin");

        Assert.False(rom.HasInesHeader);
        Assert.Equal(4, rom.TileCount);
    }

    private static byte[] Header(
        int prgUnits,
        int chrUnits,
        byte flags6,
        int? prgBytes = null,
        int? chrBytes = null,
        bool trainer = false,
        int extraLength = 0)
    {
        int prg = prgBytes ?? (prgUnits * NesRom.PrgBankSize);
        int chr = chrBytes ?? (chrUnits * NesRom.ChrBankSize);
        int trainerBytes = trainer ? NesRom.TrainerSize : 0;
        var data = new byte[16 + trainerBytes + prg + chr + extraLength];
        data[0] = (byte)'N';
        data[1] = (byte)'E';
        data[2] = (byte)'S';
        data[3] = 0x1A;
        data[4] = (byte)prgUnits;
        data[5] = (byte)chrUnits;
        data[6] = flags6;
        return data;
    }
}
