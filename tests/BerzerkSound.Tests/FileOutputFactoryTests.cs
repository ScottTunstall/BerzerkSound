using System;
using System.IO;
using System.Text;
using BerzerkSound.Audio;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class FileOutputFactoryTests : IDisposable
{
    private readonly TemporaryDirectory _directory = new();

    [Theory]
    [InlineData("speech.ogg", "OggS")]
    [InlineData("speech.wav", "RIFF")]
    [InlineData("speech.bin", "RIFF")]
    public void WritesTheFormatTheFileNameAsksFor(string name, string expectedMagic)
    {
        string path = _directory.PathOf(name);

        FileOutputFactory.Create(path).Deliver(new AudioClip(24_000, [0f, 0.25f, -0.25f, 0f]));

        Assert.Equal(expectedMagic, Encoding.ASCII.GetString(File.ReadAllBytes(path), 0, 4));
    }

    [Fact]
    public void AddsTheWavExtensionToAFileNameWithNone()
    {
        string requested = _directory.PathOf("speech");

        FileOutputFactory.Create(requested).Deliver(new AudioClip(24_000, [0f, 0.25f, -0.25f, 0f]));

        Assert.Equal("RIFF", Encoding.ASCII.GetString(File.ReadAllBytes(requested + ".wav"), 0, 4));
        Assert.False(File.Exists(requested));
    }

    [Fact]
    public void ReplacesAnExistingFile()
    {
        string path = _directory.PathOf("speech.ogg");
        File.WriteAllText(path, "old");

        FileOutputFactory.Create(path).Deliver(new AudioClip(24_000, [0f, 0f]));

        Assert.Equal("OggS", Encoding.ASCII.GetString(File.ReadAllBytes(path), 0, 4));
    }

    [Fact]
    public void RejectsANullPath() => Assert.Throws<ArgumentNullException>(() => FileOutputFactory.Create(null!));

    public void Dispose() => _directory.Dispose();
}
