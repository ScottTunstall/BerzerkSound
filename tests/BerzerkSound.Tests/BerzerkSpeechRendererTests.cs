using System;
using System.Linq;
using BerzerkSound.Audio;
using BerzerkSound.Roms;
using BerzerkSound.Speech;
using BerzerkSound.Synthesis;
using BerzerkSound.Vocabulary;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class BerzerkSpeechRendererTests
{
    private const int SyllableAddress = 0x100;
    private const int DeltaData = 0x200;

    private readonly BerzerkSpeechRenderer _renderer = new(BuildRom());

    [Theory]
    [InlineData(0, 19_531)]
    [InlineData(3, 24_038)]
    [InlineData(7, 34_722)]
    public void RendersAtTheChipClockRate(int pitch, int expectedRate) =>
        Assert.Equal(expectedRate, _renderer.Render([SpeechWord.Robot], new VoiceSettings(pitch, 7)).SampleRateHz);

    [Fact]
    public void RendersAudibleSpeech()
    {
        AudioClip clip = _renderer.Render([SpeechWord.Robot], VoiceSettings.Default);

        Assert.Equal(1f, clip.Samples.Max());
    }

    [Fact]
    public void IsSilentAtVolumeZero()
    {
        AudioClip clip = _renderer.Render([SpeechWord.Robot], new VoiceSettings(3, 0));

        Assert.NotEmpty(clip.Samples);
        Assert.All(clip.Samples, sample => Assert.Equal(0f, sample));
    }

    [Fact]
    public void SaysWordsOneAfterAnother()
    {
        int one = _renderer.Render([SpeechWord.Robot], VoiceSettings.Default).Samples.Count;
        int two = _renderer.Render([SpeechWord.Robot, SpeechWord.Robot], VoiceSettings.Default).Samples.Count;

        Assert.InRange(two, (2 * one) - SamplesPerNmi(), (2 * one) + SamplesPerNmi());
    }

    [Fact]
    public void StopsWithinOneNmiOfTheLastWordEnding()
    {
        AudioClip clip = _renderer.Render([SpeechWord.Robot], VoiceSettings.Default);
        int trailingSilence = clip.Samples.Reverse().TakeWhile(sample => sample == 0f).Count();

        Assert.InRange(trailingSilence, 1, SamplesPerNmi() + 8);
    }

    [Fact]
    public void RendersNothingForNoWords() =>
        Assert.Empty(_renderer.Render([], VoiceSettings.Default).Samples);

    [Fact]
    public void RejectsARomWhoseWordNeverEnds()
    {
        var renderer = new BerzerkSpeechRenderer(new SpeechRom(new byte[ChipConstants.RomSize]));

        _ = Assert.Throws<InvalidRomException>(() => renderer.Render([SpeechWord.Help], VoiceSettings.Default));
    }

    [Fact]
    public void RejectsNullArguments()
    {
        _ = Assert.Throws<ArgumentNullException>(() => _renderer.Render(null!, VoiceSettings.Default));
        _ = Assert.Throws<ArgumentNullException>(() => _renderer.Render([SpeechWord.Robot], null!));
    }

    private static int SamplesPerNmi() => (int)Math.Ceiling(VoiceSettings.Default.ChipClockHz / BerzerkTiming.NmiRateHz);

    private static SpeechRom BuildRom()
    {
        var builder = new SpeechRomBuilder();
        builder.SetWord((int)SpeechWord.Robot, SyllableAddress);
        builder.SetSyllable(SyllableAddress, DeltaData, SpeechRomBuilder.Stop | SpeechRomBuilder.ShortSyllable);
        builder.Fill(DeltaData, 0x100, 0xFF);
        return builder.Build();
    }
}
