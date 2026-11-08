using System;
using BerzerkSound.Audio;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class OutputFilePathsTests
{
    [Theory]
    [InlineData("speech", "speech.wav")]
    [InlineData(@"C:\out\speech", @"C:\out\speech.wav")]
    [InlineData(@"C:\my.folder\speech", @"C:\my.folder\speech.wav")]
    [InlineData("speech.", "speech.wav")]
    public void AddsWavWhenThereIsNoExtension(string path, string expected) =>
        Assert.Equal(expected, OutputFilePaths.WithDefaultExtension(path));

    [Theory]
    [InlineData("speech.wav")]
    [InlineData("speech.ogg")]
    [InlineData("speech.OGG")]
    [InlineData("speech.bin")]
    [InlineData("speech.ogg.bak")]
    [InlineData(@"C:\my.folder\speech.mp3")]
    public void KeepsAnyExistingExtension(string path) =>
        Assert.Equal(path, OutputFilePaths.WithDefaultExtension(path));

    [Fact]
    public void RejectsANullPath() => Assert.Throws<ArgumentNullException>(() => OutputFilePaths.WithDefaultExtension(null!));
}
