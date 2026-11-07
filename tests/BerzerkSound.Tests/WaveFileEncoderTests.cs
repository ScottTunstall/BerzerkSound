using System;
using System.Text;
using BerzerkSound.Audio;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class WaveFileEncoderTests
{
    private const int HeaderSize = 44;

    [Fact]
    public void WritesAStandardPcmHeader()
    {
        byte[] wave = WaveFileEncoder.Encode(new AudioClip(40_000, [0f, 0f, 0f]));

        Assert.Equal("RIFF", Encoding.ASCII.GetString(wave, 0, 4));
        Assert.Equal(wave.Length - 8, BitConverter.ToInt32(wave, 4));
        Assert.Equal("WAVEfmt ", Encoding.ASCII.GetString(wave, 8, 8));
        Assert.Equal(1, BitConverter.ToInt16(wave, 20));
        Assert.Equal(1, BitConverter.ToInt16(wave, 22));
        Assert.Equal(40_000, BitConverter.ToInt32(wave, 24));
        Assert.Equal(80_000, BitConverter.ToInt32(wave, 28));
        Assert.Equal(2, BitConverter.ToInt16(wave, 32));
        Assert.Equal(16, BitConverter.ToInt16(wave, 34));
        Assert.Equal("data", Encoding.ASCII.GetString(wave, 36, 4));
        Assert.Equal(6, BitConverter.ToInt32(wave, 40));
        Assert.Equal(HeaderSize + 6, wave.Length);
    }

    [Fact]
    public void ScalesAndClipsSamples()
    {
        byte[] wave = WaveFileEncoder.Encode(new AudioClip(8_000, [1f, -1f, 0.5f, 2f, -3f]));

        Assert.Equal(short.MaxValue, SampleAt(wave, 0));
        Assert.Equal(-short.MaxValue, SampleAt(wave, 1));
        Assert.Equal(16_384, SampleAt(wave, 2));
        Assert.Equal(short.MaxValue, SampleAt(wave, 3));
        Assert.Equal(-short.MaxValue, SampleAt(wave, 4));
    }

    private static short SampleAt(byte[] wave, int index) => BitConverter.ToInt16(wave, HeaderSize + (2 * index));
}
