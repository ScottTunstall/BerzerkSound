// license:BSD-3-Clause
// copyright-holders:Ed Bernard, Jonathan Gevaryahu, hap
// thanks-to:Kevin Horton
//
// SSi TSI S14001A speech IC emulator, ported from MAME src/devices/sound/s14001a.cpp and s14001a.h.
// Originally written for MAME by Jonathan Gevaryahu (Lord Nightmare) 2006-2013, replaced with a
// near-complete rewrite by Ed Bernard in 2016.

using System;

namespace BerzerkSound.Synthesis;

/// <summary>
/// An emulation of the TSI S14001A speech chip (the "Custom ROM Controller"), cycle for cycle as in MAME.
/// </summary>
/// <remarks>
/// <para>
/// To say a word, put its number on the word select bus with <see cref="SelectWord"/>, pulse START with
/// <see cref="SetStart"/> (high, then low), and call <see cref="Clock"/> once per output sample until
/// <see cref="IsBusy"/> falls. The output sample rate equals the chip's clock rate.
/// </para>
/// <para>
/// The chip is two-phase. On silicon every phase 1 register is refreshed from phase 2; here registers are
/// static, so a phase 1 value is only written when it differs from the preceding phase 2 value. MAME's
/// phase 2 copies that are never read (state, high delta address and external repeat) are omitted.
/// </para>
/// </remarks>
internal sealed class S14001AChip
{
    private readonly SpeechRom _rom;

    private bool _isPhase1;
    private ChipState _state;

    /// <summary>The 9 high bits of the delta address register; each step advances the ROM address 8 bytes.</summary>
    private int _deltaAddressHighP1;

    /// <summary>
    /// The 5 low bits of the delta address register: 3 address the byte within the 8, and 2 select one of its
    /// four deltas. A carry ends a quarter pitch period (32 deltas).
    /// </summary>
    private int _deltaAddressLowP1;
    private int _deltaAddressLowP2;

    /// <summary>The 12-bit control word address register (the syllable table pointer).</summary>
    private int _controlWordAddressP1;
    private int _controlWordAddressP2;

    private bool _isStopP1;
    private bool _isStopP2;
    private bool _isVoicedP1;
    private bool _isVoicedP2;
    private bool _isSilenceP1;
    private bool _isSilenceP2;

    /// <summary>
    /// The 7-bit length counter: the upper three bits are loaded from the ROM length, the middle two from the
    /// ROM or external repeat, and the lowest two count pitch period quarters (bit 0: mirror, bit 1: silent half).
    /// </summary>
    private int _lengthP1;
    private int _lengthP2;

    private int _externalRepeatP1;
    private int _previousDeltaP1;
    private int _previousDeltaP2;
    private int _outputP1;
    private int _romAddressP1;
    private int _romAddressP2;

    /// <summary>Carry out of the low delta address bits, derived from the phase 2 registers.</summary>
    private bool _deltaAddressLowCarryP2;
    private bool _repeatCarryP2;
    private bool _lengthCarryP2;

    private bool _isStart;
    private int _word;

    /// <summary>
    /// Initializes a new instance of the <see cref="S14001AChip"/> class: idle, silent and not busy.
    /// </summary>
    /// <param name="rom">The speech ROM the chip reads.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rom"/> is <see langword="null"/>.</exception>
    public S14001AChip(SpeechRom rom)
    {
        ArgumentNullException.ThrowIfNull(rom);

        _rom = rom;
        _state = ChipState.Idle;
        _outputP1 = ChipConstants.SilentOutput;
        Output = ChipConstants.SilentOutput;
    }

    /// <summary>Gets a value indicating whether the chip is speaking (the /BUSY pin is active).</summary>
    public bool IsBusy { get; private set; }

    /// <summary>Gets the 4-bit DAC output level, 0 to 15; <see cref="ChipConstants.SilentOutput"/> is silence.</summary>
    public int Output { get; private set; }

    /// <summary>
    /// Puts a word number on the word select bus (C0-C5).
    /// </summary>
    /// <param name="wordNumber">The word number, 0 to 63.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="wordNumber"/> is outside 0 to 63.</exception>
    public void SelectWord(int wordNumber)
    {
        if (wordNumber is < 0 or > ChipConstants.MaximumWordNumber)
        {
            throw new ArgumentOutOfRangeException(
                nameof(wordNumber),
                wordNumber,
                $"The word number must be from 0 to {ChipConstants.MaximumWordNumber}.");
        }

        _word = wordNumber;
    }

    /// <summary>
    /// Drives the START pin. A rising edge starts the selected word; while START is high the chip keeps
    /// re-latching the word number.
    /// </summary>
    /// <param name="isHigh">The new level of the pin.</param>
    public void SetStart(bool isHigh)
    {
        if (isHigh && !_isStart)
        {
            _state = ChipState.WordWait;
        }

        _isStart = isHigh;
    }

    /// <summary>
    /// Advances the chip by one clock cycle, which toggles its internal phase and produces one output sample.
    /// </summary>
    public void Clock()
    {
        if (_isPhase1)
        {
            _isPhase1 = false;
            TransferToPhase2();
            return;
        }

        _isPhase1 = true;
        RunPhase1();
    }

    private void TransferToPhase2()
    {
        _deltaAddressLowP2 = _deltaAddressLowP1;
        _controlWordAddressP2 = _controlWordAddressP1;
        _isStopP2 = _isStopP1;
        _isVoicedP2 = _isVoicedP1;
        _isSilenceP2 = _isSilenceP1;
        _lengthP2 = _lengthP1;
        _previousDeltaP2 = _previousDeltaP1;
        Output = _outputP1;
        _romAddressP2 = _romAddressP1;

        _deltaAddressLowCarryP2 = _deltaAddressLowP2 == ChipConstants.DeltaAddressLowMaximum;

        bool quarterCarry = _deltaAddressLowCarryP2
            && (_lengthP2 & ChipConstants.QuarterMask) == ChipConstants.QuarterMask;

        _repeatCarryP2 = quarterCarry
            && (_lengthP2 & ChipConstants.LengthRepeatMask) == ChipConstants.LengthRepeatMask;

        _lengthCarryP2 = _repeatCarryP2 && _lengthP2 == ChipConstants.LengthMaximum;
    }

    private void RunPhase1()
    {
        switch (_state)
        {
            case ChipState.Idle:
                Idle();
                break;

            case ChipState.WordWait:
                LatchWord();
                break;

            case ChipState.ControlWordAddressHigh:
                ReadControlWordAddressHigh();
                break;

            case ChipState.ControlWordAddressLow:
                ReadControlWordAddressLow();
                break;

            case ChipState.DeltaAddressHigh:
                ReadDeltaAddressHigh();
                break;

            case ChipState.ControlBits:
                ReadControlBits();
                break;

            case ChipState.Play:
                Play();
                break;

            case ChipState.Delay:
                _outputP1 = ChipConstants.SilentOutput;
                _state = NextStateUnlessStarted(ChipState.Idle);
                break;

            default:
                throw new InvalidOperationException($"The chip is in an unknown state ({_state}).");
        }
    }

    private void Idle()
    {
        _outputP1 = ChipConstants.SilentOutput;

        if (_isStart)
        {
            _state = ChipState.WordWait;
        }

        IsBusy = false;
    }

    private void LatchWord()
    {
        // The delta address register latches the word number into bits 3 to 8, with every other bit
        // forced to 0; bits 4 to 8 make a multiply by two (each word table entry is two bytes).
        _deltaAddressHighP1 = (_word & ChipConstants.WordHighBitsMask) >> ChipConstants.WordHighBitsShift;
        _deltaAddressLowP1 = (_word & ChipConstants.WordLowBitsMask) << ChipConstants.WordLowBitsShift;
        _romAddressP1 = DeltaByteAddress(_deltaAddressLowP1);

        _outputP1 = ChipConstants.SilentOutput;
        _state = NextStateUnlessStarted(ChipState.ControlWordAddressHigh);
        IsBusy = true;
    }

    private void ReadControlWordAddressHigh()
    {
        // The ROM address was set up in the previous state.
        _controlWordAddressP1 = ReadRom(_romAddressP2) << ChipConstants.ControlWordAddressShift;

        // Advance to the next byte; the two least significant bits count deltas within a byte.
        _deltaAddressLowP1 += ChipConstants.DeltaAddressByteStep;

        if (_deltaAddressLowP1 >= ChipConstants.DeltaAddressLowLimit)
        {
            _deltaAddressLowP1 = 0;
        }

        _romAddressP1 = DeltaByteAddress(_deltaAddressLowP1);

        _outputP1 = ChipConstants.SilentOutput;
        _state = NextStateUnlessStarted(ChipState.ControlWordAddressLow);
    }

    private void ReadControlWordAddressLow()
    {
        _controlWordAddressP1 = _controlWordAddressP2 | (ReadRom(_romAddressP2) >> ChipConstants.ControlWordAddressShift);
        _romAddressP1 = _controlWordAddressP1;

        _outputP1 = ChipConstants.SilentOutput;
        _state = NextStateUnlessStarted(ChipState.DeltaAddressHigh);
    }

    private void ReadDeltaAddressHigh()
    {
        // A 9-bit counter: the 8 most significant bits come from ROM and the least significant bit is zeroed.
        _deltaAddressHighP1 = ReadRom(_romAddressP2) << ChipConstants.DeltaAddressFromRomShift;
        _deltaAddressLowP1 = 0;
        _controlWordAddressP1++;
        _romAddressP1 = _controlWordAddressP1;

        _outputP1 = ChipConstants.SilentOutput;
        _state = NextStateUnlessStarted(ChipState.ControlBits);
    }

    private void ReadControlBits()
    {
        int data = ReadRom(_romAddressP2);

        _isStopP1 = (data & ChipConstants.StopBit) != 0;
        _isVoicedP1 = (data & ChipConstants.VoicedBit) != 0;
        _isSilenceP1 = (data & ChipConstants.SilenceBit) != 0;
        _externalRepeatP1 = data & ChipConstants.RepeatMask;
        _lengthP1 = (data & ChipConstants.LengthMask) << ChipConstants.LengthShift; // includes the external length and repeat
        _deltaAddressLowP1 = 0;
        _controlWordAddressP1++; // ready for the next syllable's delta address
        _romAddressP1 = DeltaByteAddress(_deltaAddressLowP1);

        _outputP1 = ChipConstants.SilentOutput;
        _state = NextStateUnlessStarted(ChipState.Play);
    }

    private void Play()
    {
        UpdateOutput();
        AdvanceCounters();

        // Voiced speech plays the odd quarters of each pitch period backwards (mirroring).
        int deltaAddressLow = _deltaAddressLowP1;

        if (_isVoicedP2 && (_lengthP1 & ChipConstants.MirrorQuarterBit) != 0)
        {
            deltaAddressLow ^= ChipConstants.DeltaAddressMirrorMask;
        }

        _romAddressP1 = DeltaByteAddress(deltaAddressLow);

        if (_isStart)
        {
            _state = ChipState.WordWait;
        }
        else if (_isStopP2 && _lengthCarryP2)
        {
            _state = ChipState.Delay;
        }
        else if (_lengthCarryP2)
        {
            _state = ChipState.DeltaAddressHigh;
            _romAddressP1 = _controlWordAddressP1; // output the next syllable's address
        }
        else
        {
            _state = ChipState.Play;
        }
    }

    private void UpdateOutput()
    {
        int quarter = _lengthP2 & ChipConstants.QuarterMask;
        bool isQuarterStart = _deltaAddressLowP2 == 0;

        int delta = DeltaDecoder.SelectDelta(
            _isVoicedP2,
            quarter,
            _deltaAddressLowP2 & ChipConstants.DeltaIndexMask,
            ReadRom(_romAddressP2));

        DeltaStep step = DeltaDecoder.CalculateStep(_isVoicedP2, quarter, isQuarterStart, delta, _previousDeltaP2);

        _previousDeltaP1 = delta;
        _outputP1 = DeltaDecoder.CalculateOutput(_isVoicedP2, _isSilenceP2, quarter, isQuarterStart, Output, step);
    }

    private void AdvanceCounters()
    {
        _deltaAddressLowP1++;

        // The end of a pitch period quarter.
        if (_deltaAddressLowCarryP2)
        {
            _deltaAddressLowP1 = 0;
            _lengthP1++;

            if (_lengthP1 >= ChipConstants.LengthLimit)
            {
                _lengthP1 = 0;
            }
        }

        // A voiced repeat is complete: keep the current length and reload the repeat count.
        if (_isVoicedP2 && _repeatCarryP2)
        {
            _lengthP1 &= ChipConstants.LengthKeepMask;
            _lengthP1 |= _externalRepeatP1 << ChipConstants.LengthShift;
            AdvanceDeltaAddressHigh();
        }

        // Unvoiced speech advances every quarter pitch period; its repeat counter is not reloaded.
        if (!_isVoicedP2 && _deltaAddressLowCarryP2)
        {
            AdvanceDeltaAddressHigh();
        }
    }

    private void AdvanceDeltaAddressHigh()
    {
        _deltaAddressHighP1++;

        if (_deltaAddressHighP1 >= ChipConstants.DeltaAddressHighLimit)
        {
            _deltaAddressHighP1 = 0;
        }
    }

    private ChipState NextStateUnlessStarted(ChipState next) => _isStart ? ChipState.WordWait : next;

    private int DeltaByteAddress(int deltaAddressLow) =>
        (_deltaAddressHighP1 << ChipConstants.DeltaAddressHighShift) | (deltaAddressLow >> ChipConstants.DeltaAddressLowShift);

    private int ReadRom(int address) => _rom.Read(address & ChipConstants.RomAddressMask);
}
