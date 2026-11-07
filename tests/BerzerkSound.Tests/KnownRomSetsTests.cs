using System.Linq;
using BerzerkSound.Roms;
using BerzerkSound.Synthesis;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class KnownRomSetsTests
{
    [Fact]
    public void TheEnglishChipsFillTheSpeechAddressSpaceWithoutOverlapping()
    {
        RomPart[] parts = [.. KnownRomSets.EnglishBerzerk.Parts.OrderBy(part => part.Offset)];

        Assert.Equal(0, parts[0].Offset);
        Assert.All(parts.Zip(parts.Skip(1)), pair => Assert.Equal(pair.First.Offset + pair.First.Size, pair.Second.Offset));
        Assert.Equal(ChipConstants.RomSize, parts[^1].Offset + parts[^1].Size);
    }

    [Fact]
    public void UsesMamesCrcsForTheEnglishChips() =>
        Assert.Equal([0x2CFE825Du, 0xD2B6324Eu], KnownRomSets.EnglishBerzerk.Parts.Select(part => part.Crc32));
}
