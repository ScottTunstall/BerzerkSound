// license:BSD-3-Clause
// copyright-holders:Ed Bernard, Jonathan Gevaryahu, hap
// thanks-to:Kevin Horton
//
// Ported from MAME src/devices/sound/s14001a.cpp: the register widths, ROM data layout and
// output levels that the original code writes as literals.

using System;

namespace BerzerkSound.Synthesis;

/// <summary>
/// Register widths, ROM data layout and output levels of the S14001A.
/// </summary>
internal static class ChipConstants
{
    /// <summary>Highest word number on the 6-bit word select bus (C0-C5).</summary>
    public const int MaximumWordNumber = 0x3F;

    /// <summary>The chip addresses 4 KB of speech ROM (12 address lines, A0-A11).</summary>
    public const int RomSize = 0x1000;

    /// <summary>Mask applied to every ROM address.</summary>
    public const int RomAddressMask = RomSize - 1;

    /// <summary>Output level the DAC is held at when silent (the middle of the 4-bit range).</summary>
    public const int SilentOutput = 7;

    /// <summary>Highest 4-bit DAC output level.</summary>
    public const int MaximumOutput = 0x0F;

    /// <summary>
    /// The divisor MAME uses to scale an output level, after subtracting <see cref="SilentOutput"/>, to the range
    /// -0.875 to 1 (<c>stream.put_int(0, i, sample, 8)</c>).
    /// </summary>
    public const float OutputScale = 8f;

    /// <summary>XORing a 4-bit level with this mask turns a subtraction into an addition and back.</summary>
    public const int OutputInvertMask = 0x0F;

    /// <summary>Word-number bits that become delta address register bits 5-8.</summary>
    public const int WordHighBitsMask = 0x3C;

    /// <summary>Word-number bits that become delta address register bits 3-4.</summary>
    public const int WordLowBitsMask = 0x03;

    /// <summary>Shift that moves word-number bits 0-1 into delta address register bits 3-4.</summary>
    public const int WordLowBitsShift = 3;

    /// <summary>Shift that moves word-number bits 2-5 down to delta address register bit 5 upwards.</summary>
    public const int WordHighBitsShift = 2;

    /// <summary>The low 5 bits of the delta address register count through 32 deltas (8 bytes of 4 deltas).</summary>
    public const int DeltaAddressLowLimit = 32;

    /// <summary>Value of the low 5 delta address bits on the last delta of a quarter pitch period.</summary>
    public const int DeltaAddressLowMaximum = DeltaAddressLowLimit - 1;

    /// <summary>The high 9 bits of the delta address register wrap at this value.</summary>
    public const int DeltaAddressHighLimit = 0x200;

    /// <summary>How far the control-word fetch advances the low delta address bits (one byte).</summary>
    public const int DeltaAddressByteStep = 4;

    /// <summary>Shift from the high delta address bits to a ROM address (each step is 8 bytes).</summary>
    public const int DeltaAddressHighShift = 3;

    /// <summary>Shift from the low delta address bits to a byte offset (4 deltas per byte).</summary>
    public const int DeltaAddressLowShift = 2;

    /// <summary>XOR mask that runs the low delta address bits backwards, for mirrored voiced playback.</summary>
    public const int DeltaAddressMirrorMask = 0x1F;

    /// <summary>Shift from the CWAR byte read from ROM to its 8 most significant bits.</summary>
    public const int ControlWordAddressShift = 4;

    /// <summary>Shift from a ROM byte to the 8 most significant bits of the 9-bit high delta address.</summary>
    public const int DeltaAddressFromRomShift = 1;

    /// <summary>Control byte bit: this is the last syllable of the word.</summary>
    public const int StopBit = 0x80;

    /// <summary>Control byte bit: the syllable is voiced (played mirrored, with a silent half).</summary>
    public const int VoicedBit = 0x40;

    /// <summary>Control byte bit: the syllable is silent.</summary>
    public const int SilenceBit = 0x20;

    /// <summary>Control byte bits holding the external repeat count.</summary>
    public const int RepeatMask = 0x03;

    /// <summary>Control byte bits holding the length and repeat, loaded into the length counter.</summary>
    public const int LengthMask = 0x1F;

    /// <summary>Shift from the control byte's length and repeat bits into the 7-bit length counter.</summary>
    public const int LengthShift = 2;

    /// <summary>The 7-bit length counter wraps at this value.</summary>
    public const int LengthLimit = 0x80;

    /// <summary>Value of the length counter on the last quarter pitch period of a syllable.</summary>
    public const int LengthMaximum = LengthLimit - 1;

    /// <summary>Length counter bits that count quarter pitch periods.</summary>
    public const int QuarterMask = 0x03;

    /// <summary>Length counter bits that count repeats.</summary>
    public const int LengthRepeatMask = 0x0C;

    /// <summary>Length counter bits kept when the repeat count is reloaded.</summary>
    public const int LengthKeepMask = 0x70;

    /// <summary>Quarter-counter bit that marks a mirrored (backwards) quarter in voiced playback.</summary>
    public const int MirrorQuarterBit = 0x01;

    /// <summary>Quarter-counter bit that marks a silent half in voiced playback.</summary>
    public const int SilentQuarterBit = 0x02;

    /// <summary>Bits of the low delta address that select one of the four 2-bit deltas in a byte.</summary>
    public const int DeltaIndexMask = 0x03;

    /// <summary>Number of 2-bit deltas packed into each ROM byte.</summary>
    public const int DeltasPerByte = 4;

    /// <summary>Width of one delta.</summary>
    public const int BitsPerDelta = 2;

    /// <summary>Mask of one 2-bit delta.</summary>
    public const int DeltaMask = 0x03;

    /// <summary>Number of distinct 2-bit delta values; the width of each row of <see cref="Increments"/>.</summary>
    public const int DeltaValueCount = 4;

    /// <summary>The previous delta assumed at the start of each pitch period.</summary>
    public const int PitchPeriodStartDelta = 0x02;

    /// <summary>Deltas at or above this value add to the output; below it they subtract.</summary>
    public const int FirstRisingDelta = 0x02;

    /// <summary>
    /// Gets the output step (0, 1 or 3) for each pair of deltas, indexed by
    /// <c>first * 4 + second</c> (MAME's <c>uIncrements[4][4]</c>).
    /// </summary>
    public static ReadOnlySpan<byte> Increments =>
    [
        3, 3, 1, 1,
        1, 1, 0, 0,
        0, 0, 1, 1,
        1, 1, 3, 3,
    ];
}
