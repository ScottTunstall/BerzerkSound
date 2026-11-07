using System;
using BerzerkSound.Roms;
using BerzerkSound.Synthesis;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class SpeechRomLoaderTests : IDisposable
{
    private readonly TemporaryDirectory _directory = new();
    private readonly SpeechRomLoader _loader = new(TestRomSet.Definition);

    [Fact]
    public void LoadsLooseRomFilesFromADirectory()
    {
        _directory.WriteFile(TestRomSet.FirstFileName, TestRomSet.FirstChip);
        _directory.WriteFile(TestRomSet.SecondFileName, TestRomSet.SecondChip);

        AssertIsTheTestSet(_loader.Load(_directory.Path));
    }

    [Fact]
    public void LoadsAZipFile()
    {
        _directory.WriteZip(
            "any-name.zip",
            (TestRomSet.FirstFileName, TestRomSet.FirstChip),
            (TestRomSet.SecondFileName, TestRomSet.SecondChip));

        AssertIsTheTestSet(_loader.Load(_directory.PathOf("any-name.zip")));
    }

    [Fact]
    public void LoadsTheSetsZipFromADirectory()
    {
        _directory.WriteZip(
            TestRomSet.ArchiveName,
            (TestRomSet.FirstFileName, TestRomSet.FirstChip),
            (TestRomSet.SecondFileName, TestRomSet.SecondChip));

        AssertIsTheTestSet(_loader.Load(_directory.Path));
    }

    [Fact]
    public void FindsZipEntriesByCrcWhateverTheirNames()
    {
        _directory.WriteZip(TestRomSet.ArchiveName, ("renamed-b.bin", TestRomSet.SecondChip), ("renamed-a.bin", TestRomSet.FirstChip));

        AssertIsTheTestSet(_loader.Load(_directory.PathOf(TestRomSet.ArchiveName)));
    }

    [Fact]
    public void RejectsABadDump()
    {
        byte[] corrupt = [.. TestRomSet.SecondChip];
        corrupt[10] ^= 0xFF;
        _directory.WriteFile(TestRomSet.FirstFileName, TestRomSet.FirstChip);
        _directory.WriteFile(TestRomSet.SecondFileName, corrupt);

        InvalidRomException exception = Assert.Throws<InvalidRomException>(() => _loader.Load(_directory.Path));

        Assert.Contains("2C chip", exception.Message, StringComparison.Ordinal);
        Assert.Contains("CRC32", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsAFileOfTheWrongSize()
    {
        _directory.WriteFile(TestRomSet.FirstFileName, new byte[TestRomSet.ChipSize / 2]);
        _directory.WriteFile(TestRomSet.SecondFileName, TestRomSet.SecondChip);

        InvalidRomException exception = Assert.Throws<InvalidRomException>(() => _loader.Load(_directory.Path));

        Assert.Contains("1024 bytes", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsAFileThatIsNotAZip()
    {
        _directory.WriteFile("notes.txt", [1, 2, 3]);

        _ = Assert.Throws<InvalidRomException>(() => _loader.Load(_directory.PathOf("notes.txt")));
    }

    [Fact]
    public void ReportsAZipThatLacksAChip()
    {
        _directory.WriteZip(TestRomSet.ArchiveName, (TestRomSet.FirstFileName, TestRomSet.FirstChip));

        RomNotFoundException exception = Assert.Throws<RomNotFoundException>(() => _loader.Load(_directory.PathOf(TestRomSet.ArchiveName)));

        Assert.Contains("2C", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ReportsADirectoryWithoutTheRoms()
    {
        _directory.WriteFile(TestRomSet.FirstFileName, TestRomSet.FirstChip);

        RomNotFoundException exception = Assert.Throws<RomNotFoundException>(() => _loader.Load(_directory.Path));

        Assert.Contains(TestRomSet.SecondFileName, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ReportsAPathThatDoesNotExist() =>
        Assert.Throws<RomNotFoundException>(() => _loader.Load(_directory.PathOf("missing.zip")));

    public void Dispose() => _directory.Dispose();

    private static void AssertIsTheTestSet(SpeechRom rom)
    {
        Assert.Equal(0x11, rom.Read(0));
        Assert.Equal(0x11, rom.Read(TestRomSet.ChipSize - 1));
        Assert.Equal(0x22, rom.Read(TestRomSet.ChipSize));
        Assert.Equal(0x22, rom.Read(ChipConstants.RomSize - 1));
    }
}
