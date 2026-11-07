using System;
using BerzerkSound.Roms;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class RomLocatorTests : IDisposable
{
    private readonly TemporaryDirectory _empty = new();
    private readonly TemporaryDirectory _withRoms = new();

    [Fact]
    public void UsesTheRequestedPathAsGiven() =>
        Assert.Equal("elsewhere.zip", new RomLocator(TestRomSet.Definition, [_withRoms.Path]).Locate("elsewhere.zip"));

    [Fact]
    public void FindsTheFirstDirectoryHoldingTheSetsZip()
    {
        _withRoms.WriteFile(TestRomSet.ArchiveName, [0]);

        Assert.Equal(_withRoms.Path, new RomLocator(TestRomSet.Definition, [_empty.Path, _withRoms.Path]).Locate(null));
    }

    [Fact]
    public void FindsADirectoryHoldingLooseRomFiles()
    {
        _withRoms.WriteFile(TestRomSet.FirstFileName, [0]);

        Assert.Equal(_withRoms.Path, new RomLocator(TestRomSet.Definition, [_empty.Path, _withRoms.Path]).Locate(null));
    }

    [Fact]
    public void ReportsTheSearchedDirectoriesWhenNothingIsFound()
    {
        RomNotFoundException exception = Assert.Throws<RomNotFoundException>(
            () => new RomLocator(TestRomSet.Definition, [_empty.Path]).Locate(null));

        Assert.Contains(_empty.Path, exception.Message, StringComparison.Ordinal);
        Assert.Contains("--rom", exception.Message, StringComparison.Ordinal);
    }

    public void Dispose()
    {
        _empty.Dispose();
        _withRoms.Dispose();
    }
}
