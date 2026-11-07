using BerzerkSound.Synthesis;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class DeltaDecoderTests
{
    private const int DeltasZeroToThree = 0b00_01_10_11;

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    public void ReadsDeltasMostSignificantBitsFirst(int index, int expected) =>
        Assert.Equal(expected, DeltaDecoder.SelectDelta(voiced: false, quarter: 0, index, DeltasZeroToThree));

    [Fact]
    public void ReadsDeltasBackwardsInAMirroredVoicedQuarter() =>
        Assert.Equal(3, DeltaDecoder.SelectDelta(voiced: true, quarter: 1, deltaIndex: 0, DeltasZeroToThree));

    [Fact]
    public void DoesNotMirrorUnvoicedSpeech() =>
        Assert.Equal(0, DeltaDecoder.SelectDelta(voiced: false, quarter: 1, deltaIndex: 0, DeltasZeroToThree));

    [Fact]
    public void AssumesARisingPreviousDeltaAtThePitchPeriodStart() =>
        Assert.Equal(new DeltaStep(3, true), DeltaDecoder.CalculateStep(false, 0, isQuarterStart: true, delta: 3, previousDelta: 0));

    [Fact]
    public void StepsByThreeForTwoLargeRisingDeltas() =>
        Assert.Equal(new DeltaStep(3, true), DeltaDecoder.CalculateStep(false, 1, isQuarterStart: false, delta: 3, previousDelta: 3));

    [Fact]
    public void FallsForSmallDeltas() =>
        Assert.False(DeltaDecoder.CalculateStep(false, 1, isQuarterStart: false, delta: 0, previousDelta: 0).IsAddition);

    [Fact]
    public void HoldsStillOnTheFirstDeltaOfAMirroredQuarter() =>
        Assert.Equal(0, DeltaDecoder.CalculateStep(true, 1, isQuarterStart: true, delta: 3, previousDelta: 3).Increment);

    [Fact]
    public void ForcesSilenceForASilentSyllable() =>
        Assert.Equal(ChipConstants.SilentOutput, DeltaDecoder.CalculateOutput(false, true, 1, false, 12, new DeltaStep(3, true)));

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public void SilencesTheSecondHalfOfAVoicedPitchPeriod(int quarter) =>
        Assert.Equal(ChipConstants.SilentOutput, DeltaDecoder.CalculateOutput(true, false, quarter, false, 12, new DeltaStep(3, true)));

    [Fact]
    public void StartsEachPitchPeriodFromTheSilentLevel() =>
        Assert.Equal(ChipConstants.SilentOutput + 1, DeltaDecoder.CalculateOutput(false, false, 0, true, 12, new DeltaStep(1, true)));

    [Fact]
    public void LimitsTheOutputToFifteen() =>
        Assert.Equal(ChipConstants.MaximumOutput, DeltaDecoder.CalculateOutput(false, false, 1, false, 14, new DeltaStep(3, true)));

    [Fact]
    public void LimitsTheOutputToZero() =>
        Assert.Equal(0, DeltaDecoder.CalculateOutput(false, false, 1, false, 1, new DeltaStep(3, false)));

    [Fact]
    public void SubtractsAFallingStep() =>
        Assert.Equal(9, DeltaDecoder.CalculateOutput(false, false, 1, false, 10, new DeltaStep(1, false)));
}
