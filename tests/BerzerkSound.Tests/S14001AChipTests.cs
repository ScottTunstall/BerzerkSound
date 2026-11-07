using System;
using System.Linq;
using BerzerkSound.Synthesis;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class S14001AChipTests
{
    private const int Word = 5;
    private const int SyllableAddress = 0x100;
    private const int DeltaData = 0x200;

    [Fact]
    public void StartsIdleAndSilent()
    {
        var chip = new S14001AChip(new SpeechRomBuilder().Build());

        chip.Clock();
        chip.Clock();

        Assert.False(chip.IsBusy);
        Assert.Equal(ChipConstants.SilentOutput, chip.Output);
    }

    [Fact]
    public void IsBusyWhileSpeakingAndIdleAfterwards()
    {
        var run = ChipRun.Say(BuildSingleSyllableRom(SpeechRomBuilder.Stop | SpeechRomBuilder.ShortSyllable, 0xFF), Word);

        Assert.True(run.Finished);
    }

    [Fact]
    public void RisingDeltasDriveTheOutputToItsMaximum()
    {
        var run = ChipRun.Say(BuildSingleSyllableRom(SpeechRomBuilder.Stop | SpeechRomBuilder.ShortSyllable, 0xFF), Word);

        Assert.Equal(ChipConstants.MaximumOutput, run.Outputs.Max());
    }

    [Fact]
    public void FallingDeltasDriveTheOutputToZero()
    {
        var run = ChipRun.Say(BuildSingleSyllableRom(SpeechRomBuilder.Stop | SpeechRomBuilder.ShortSyllable, 0x00), Word);

        Assert.Equal(0, run.Outputs.Min());
    }

    [Fact]
    public void ASilentSyllableStaysAtTheSilentLevel()
    {
        const byte Control = SpeechRomBuilder.Stop | SpeechRomBuilder.Silence | SpeechRomBuilder.ShortSyllable;
        var run = ChipRun.Say(BuildSingleSyllableRom(Control, 0xFF), Word);

        Assert.True(run.Finished);
        Assert.All(run.Outputs, output => Assert.Equal(ChipConstants.SilentOutput, output));
    }

    [Fact]
    public void AVoicedSyllableIsSilentForHalfOfEachPitchPeriod()
    {
        const byte Control = SpeechRomBuilder.Stop | SpeechRomBuilder.Voiced | SpeechRomBuilder.ShortSyllable;
        var voiced = ChipRun.Say(BuildSingleSyllableRom(Control, 0xFF), Word);
        var unvoiced = ChipRun.Say(BuildSingleSyllableRom(SpeechRomBuilder.Stop | SpeechRomBuilder.ShortSyllable, 0xFF), Word);

        Assert.True(voiced.Finished);
        Assert.Equal(ChipConstants.MaximumOutput, voiced.Outputs.Max());
        Assert.True(
            voiced.Outputs.Count(output => output == ChipConstants.SilentOutput)
            > unvoiced.Outputs.Count(output => output == ChipConstants.SilentOutput));
    }

    [Fact]
    public void ALongerSyllableTakesLonger()
    {
        var shortRun = ChipRun.Say(BuildSingleSyllableRom(SpeechRomBuilder.Stop | 0x1F, 0xFF), Word);
        var longRun = ChipRun.Say(BuildSingleSyllableRom(SpeechRomBuilder.Stop | 0x1B, 0xFF), Word);

        Assert.True(longRun.Outputs.Count > shortRun.Outputs.Count);
    }

    [Fact]
    public void PlaysSyllablesUntilOneHasTheStopBit()
    {
        var builder = new SpeechRomBuilder();
        builder.SetWord(Word, SyllableAddress);
        builder.SetSyllable(SyllableAddress, DeltaData, SpeechRomBuilder.ShortSyllable);
        builder.SetSyllable(SyllableAddress + 2, DeltaData, SpeechRomBuilder.Stop | SpeechRomBuilder.ShortSyllable);
        builder.Fill(DeltaData, 0x100, 0xFF);

        var twoSyllables = ChipRun.Say(builder.Build(), Word);
        var oneSyllable = ChipRun.Say(BuildSingleSyllableRom(SpeechRomBuilder.Stop | SpeechRomBuilder.ShortSyllable, 0xFF), Word);

        Assert.True(twoSyllables.Finished);
        Assert.True(twoSyllables.Outputs.Count > oneSyllable.Outputs.Count);
    }

    [Fact]
    public void ReadsTheWordTableEntryForTheSelectedWord()
    {
        var builder = new SpeechRomBuilder();
        builder.SetWord(1, SyllableAddress);
        builder.SetWord(2, SyllableAddress + 2);
        builder.SetSyllable(SyllableAddress, DeltaData, SpeechRomBuilder.Stop | SpeechRomBuilder.Silence | SpeechRomBuilder.ShortSyllable);
        builder.SetSyllable(SyllableAddress + 2, DeltaData, SpeechRomBuilder.Stop | SpeechRomBuilder.ShortSyllable);
        builder.Fill(DeltaData, 0x100, 0xFF);
        SpeechRom rom = builder.Build();

        Assert.Equal(ChipConstants.SilentOutput, ChipRun.Say(rom, 1).Outputs.Max());
        Assert.Equal(ChipConstants.MaximumOutput, ChipRun.Say(rom, 2).Outputs.Max());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(64)]
    public void RejectsAWordNumberOutsideTheSelectBus(int word)
    {
        var chip = new S14001AChip(new SpeechRomBuilder().Build());

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => chip.SelectWord(word));

        Assert.Equal("wordNumber", exception.ParamName);
    }

    [Fact]
    public void RejectsANullRom() => Assert.Throws<ArgumentNullException>(() => new S14001AChip(null!));

    private static SpeechRom BuildSingleSyllableRom(int controlBits, byte deltaData)
    {
        var builder = new SpeechRomBuilder();
        builder.SetWord(Word, SyllableAddress);
        builder.SetSyllable(SyllableAddress, DeltaData, (byte)controlBits);
        builder.Fill(DeltaData, 0x100, deltaData);
        return builder.Build();
    }
}
