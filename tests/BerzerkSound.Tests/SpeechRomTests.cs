using System;
using BerzerkSound.Synthesis;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class SpeechRomTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(0x0FFF)]
    [InlineData(0x1001)]
    public void RejectsAnImageThatIsNotFourKilobytes(int size) =>
        Assert.Throws<ArgumentException>(() => new SpeechRom(new byte[size]));

    [Theory]
    [InlineData(-1)]
    [InlineData(0x1000)]
    public void RejectsAnAddressOutsideTheRom(int address) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SpeechRom(new byte[0x1000]).Read(address));

    [Fact]
    public void CopiesTheImage()
    {
        byte[] image = new byte[0x1000];
        image[0x123] = 0x45;
        var rom = new SpeechRom(image);

        image[0x123] = 0;

        Assert.Equal(0x45, rom.Read(0x123));
    }
}
