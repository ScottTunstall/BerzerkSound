// license:BSD-3-Clause
// copyright-holders:Ed Bernard, Jonathan Gevaryahu, hap
// thanks-to:Kevin Horton
//
// Ported from MAME src/devices/sound/s14001a.cpp (the uIncrementP2 and bAddP2 outputs of CalculateIncrement).

namespace BerzerkSound.Synthesis;

/// <summary>
/// How the output level changes for one delta.
/// </summary>
/// <param name="Increment">The size of the change: 0, 1 or 3.</param>
/// <param name="IsAddition"><see langword="true"/> to raise the level; <see langword="false"/> to lower it.</param>
internal readonly record struct DeltaStep(int Increment, bool IsAddition);
