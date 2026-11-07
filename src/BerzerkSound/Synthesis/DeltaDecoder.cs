// license:BSD-3-Clause
// copyright-holders:Ed Bernard, Jonathan Gevaryahu, hap
// thanks-to:Kevin Horton
//
// Ported from MAME src/devices/sound/s14001a.cpp (Mux8To2, CalculateIncrement and CalculateOutput).

using System;

namespace BerzerkSound.Synthesis;

/// <summary>
/// The S14001A's "floating-zero, two-bit delta modulation" decoder. Every member is a pure function of its
/// arguments, mimicking the chip's combinational logic.
/// </summary>
internal static class DeltaDecoder
{
    /// <summary>
    /// Picks one 2-bit delta out of a ROM byte (MAME's <c>Mux8To2</c>). Deltas are stored big-endian: delta 0
    /// is bits 7-6. In a mirrored quarter of voiced speech the deltas are read backwards.
    /// </summary>
    /// <param name="voiced">Whether the syllable is voiced.</param>
    /// <param name="quarter">The pitch period quarter counter, 0 to 3.</param>
    /// <param name="deltaIndex">Which delta within the byte, 0 to 3.</param>
    /// <param name="romData">The ROM byte.</param>
    /// <returns>The delta, 0 to 3.</returns>
    public static int SelectDelta(bool voiced, int quarter, int deltaIndex, int romData)
    {
        int index = voiced && IsMirrored(quarter) ? deltaIndex ^ ChipConstants.DeltaIndexMask : deltaIndex;
        int shift = (ChipConstants.DeltasPerByte - 1 - index) * ChipConstants.BitsPerDelta;

        return (romData >> shift) & ChipConstants.DeltaMask;
    }

    /// <summary>
    /// Works out how the output changes for a delta, given the previous delta (MAME's <c>CalculateIncrement</c>).
    /// The caller stores <paramref name="delta"/> as the next previous delta, which the chip always does, even
    /// when silent.
    /// </summary>
    /// <param name="voiced">Whether the syllable is voiced.</param>
    /// <param name="quarter">The pitch period quarter counter, 0 to 3.</param>
    /// <param name="isQuarterStart">Whether this is the first delta of a pitch period quarter.</param>
    /// <param name="delta">The current delta, 0 to 3.</param>
    /// <param name="previousDelta">The previous delta, 0 to 3.</param>
    /// <returns>The size and direction of the change.</returns>
    public static DeltaStep CalculateStep(bool voiced, int quarter, bool isQuarterStart, int delta, int previousDelta)
    {
        // The beginning of a pitch period, for voiced and unvoiced speech alike.
        int effectivePrevious = quarter == 0 && isQuarterStart ? ChipConstants.PitchPeriodStartDelta : previousDelta;
        bool mirrored = IsMirrored(quarter);

        if (voiced && mirrored)
        {
            // No change on the first delta of a mirrored quarter.
            int increment = isQuarterStart ? 0 : IncrementFor(effectivePrevious, delta);
            return new DeltaStep(increment, effectivePrevious < ChipConstants.FirstRisingDelta);
        }

        return new DeltaStep(IncrementFor(delta, effectivePrevious), delta >= ChipConstants.FirstRisingDelta);
    }

    /// <summary>
    /// Applies a delta step to the previous output level (MAME's <c>CalculateOutput</c>), limiting the result
    /// to the DAC's 4-bit range.
    /// </summary>
    /// <param name="voiced">Whether the syllable is voiced.</param>
    /// <param name="isSilence">Whether the syllable is silent.</param>
    /// <param name="quarter">The pitch period quarter counter, 0 to 3.</param>
    /// <param name="isQuarterStart">Whether this is the first delta of a pitch period quarter.</param>
    /// <param name="previousOutput">The previous output level, 0 to 15.</param>
    /// <param name="step">The change to apply.</param>
    /// <returns>The new output level, 0 to 15.</returns>
    public static int CalculateOutput(bool voiced, bool isSilence, int quarter, bool isQuarterStart, int previousOutput, DeltaStep step)
    {
        // Voiced speech is silent for the second half of every pitch period.
        if (isSilence || (voiced && (quarter & ChipConstants.SilentQuarterBit) != 0))
        {
            return ChipConstants.SilentOutput;
        }

        // The beginning of a pitch period, for voiced and unvoiced speech alike.
        int level = quarter == 0 && isQuarterStart ? ChipConstants.SilentOutput : previousOutput;

        // The adder only adds and saturates at 15; inverting before and after turns that into a
        // subtraction that saturates at 0.
        if (!step.IsAddition)
        {
            level ^= ChipConstants.OutputInvertMask;
        }

        level = Math.Min(level + step.Increment, ChipConstants.MaximumOutput);

        if (!step.IsAddition)
        {
            level ^= ChipConstants.OutputInvertMask;
        }

        return level;
    }

    private static bool IsMirrored(int quarter) => (quarter & ChipConstants.MirrorQuarterBit) != 0;

    private static int IncrementFor(int first, int second) => ChipConstants.Increments[(first * ChipConstants.DeltaValueCount) + second];
}
