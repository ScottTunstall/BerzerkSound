using System;
using BerzerkSound.Speech;
using BerzerkSound.Synthesis;
using BerzerkSound.Vocabulary;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class SpeechPipelineTests
{
    private const int SyllableAddress = 0x100;
    private const int DeltaData = 0x200;

    [Fact]
    public void DeliversTheRenderedWordsToTheOutput()
    {
        var voice = new VoiceSettings(5, 7);
        var output = new RecordingAudioOutput();
        var pipeline = new SpeechPipeline(new BerzerkSpeechRenderer(BuildRom()), output);

        pipeline.Say([SpeechWord.Robot], voice);

        _ = Assert.Single(output.Clips);
        Assert.Equal((int)Math.Round(voice.ChipClockHz), output.Clips[0].SampleRateHz);
        Assert.NotEmpty(output.Clips[0].Samples);
    }

    [Fact]
    public void RejectsNullDependencies()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new SpeechPipeline(null!, new RecordingAudioOutput()));
        _ = Assert.Throws<ArgumentNullException>(() => new SpeechPipeline(new BerzerkSpeechRenderer(BuildRom()), null!));
    }

    private static SpeechRom BuildRom()
    {
        var builder = new SpeechRomBuilder();
        builder.SetWord((int)SpeechWord.Robot, SyllableAddress);
        builder.SetSyllable(SyllableAddress, DeltaData, SpeechRomBuilder.Stop | SpeechRomBuilder.ShortSyllable);
        return builder.Build();
    }
}
