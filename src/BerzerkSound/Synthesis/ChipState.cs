// license:BSD-3-Clause
// copyright-holders:Ed Bernard, Jonathan Gevaryahu, hap
// thanks-to:Kevin Horton
//
// Ported from MAME src/devices/sound/s14001a.h (enum class states).

namespace BerzerkSound.Synthesis;

/// <summary>
/// The states of the S14001A's internal sequencer.
/// </summary>
internal enum ChipState
{
    /// <summary>Not speaking; the output is held at the silent level.</summary>
    Idle = 0,

    /// <summary>START is (or was just) high: the word number is being latched.</summary>
    WordWait = 1,

    /// <summary>Reading the 8 most significant bits of the control word address register (CWAR).</summary>
    ControlWordAddressHigh = 2,

    /// <summary>Reading the 4 least significant bits of the CWAR from ROM bits D7-D4.</summary>
    ControlWordAddressLow = 3,

    /// <summary>Reading the 8 most significant bits of the delta address register (DAR).</summary>
    DeltaAddressHigh = 4,

    /// <summary>Reading the control bits: stop, voiced, silence, length and repeat.</summary>
    ControlBits = 5,

    /// <summary>Playing delta-modulated samples.</summary>
    Play = 6,

    /// <summary>The word has ended; one more cycle before returning to idle.</summary>
    Delay = 7,
}
