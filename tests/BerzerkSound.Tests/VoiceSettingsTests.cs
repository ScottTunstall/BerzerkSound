using System;
using BerzerkSound.Speech;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class VoiceSettingsTests
{
    [Theory]
    [InlineData(0, 19_531.25)]
    [InlineData(3, 24_038.46)]
    [InlineData(7, 34_722.22)]
    public void DividesTheSpeechClockAsTheBoardDoes(int pitch, double expectedHz) =>
        Assert.Equal(expectedHz, new VoiceSettings(pitch, 7).ChipClockHz, precision: 2);

    [Fact]
    public void DefaultsToTheIntruderAlertSettings()
    {
        Assert.Equal(3, VoiceSettings.Default.Pitch);
        Assert.Equal(7, VoiceSettings.Default.Volume);
    }

    [Theory]
    [InlineData(0, 0f)]
    [InlineData(7, 1f)]
    public void ScalesTheGainWithTheVolume(int volume, float expectedGain) =>
        Assert.Equal(expectedGain, new VoiceSettings(3, volume).Gain);

    [Theory]
    [InlineData(-1)]
    [InlineData(8)]
    public void RejectsAnOutOfRangePitch(int pitch) =>
        Assert.Equal("pitch", Assert.Throws<ArgumentOutOfRangeException>(() => new VoiceSettings(pitch, 7)).ParamName);

    [Theory]
    [InlineData(-1)]
    [InlineData(8)]
    public void RejectsAnOutOfRangeVolume(int volume) =>
        Assert.Equal("volume", Assert.Throws<ArgumentOutOfRangeException>(() => new VoiceSettings(3, volume)).ParamName);
}
