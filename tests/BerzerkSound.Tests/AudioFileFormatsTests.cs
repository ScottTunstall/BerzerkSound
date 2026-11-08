using System;
using BerzerkSound.Audio;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class AudioFileFormatsTests
{
    [Theory]
    [InlineData("speech.ogg")]
    [InlineData("speech.OGG")]
    [InlineData(@"C:\some folder\speech.Ogg")]
    public void ChoosesOggForTheOggExtension(string path) =>
        Assert.Equal(AudioFileFormat.Ogg, AudioFileFormats.FromPath(path));

    [Theory]
    [InlineData("speech.wav")]
    [InlineData("speech.WAV")]
    [InlineData("speech")]
    [InlineData("speech.ogg.bak")]
    [InlineData("speech.bin")]
    public void ChoosesWavForEverythingElse(string path) =>
        Assert.Equal(AudioFileFormat.Wav, AudioFileFormats.FromPath(path));

    [Fact]
    public void RejectsANullPath() => Assert.Throws<ArgumentNullException>(() => AudioFileFormats.FromPath(null!));
}
