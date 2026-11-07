// license:BSD-3-Clause
// copyright-holders:Zsolt Vasvari, Aaron Giles, R. Belmont, Jonathan Gevaryahu
//
// Clock and video timing of the Berzerk board, from MAME src/mame/stern/berzerk.cpp.

namespace BerzerkSound.Speech;

/// <summary>
/// Clocks of the Berzerk board that decide the speech chip's sample rate and when the game feeds it words.
/// </summary>
internal static class BerzerkTiming
{
    /// <summary>The board's 10 MHz master crystal (<c>MASTER_CLOCK</c>).</summary>
    public const double MasterClockHz = 10_000_000;

    /// <summary>The clock fed to the speech clock dividers (<c>S14001_CLOCK</c>, master / 4).</summary>
    public const double SpeechClockHz = MasterClockHz / 4;

    /// <summary>
    /// The first LS161 on the VSU-1000 divides the speech clock by this minus the 3-bit pitch setting (9 to 16).
    /// </summary>
    public const int PitchDividerBase = 16;

    /// <summary>The second LS161 divides the speech clock by a further 8.</summary>
    public const int SecondDivider = 8;

    /// <summary>The pixel clock (<c>PIXEL_CLOCK</c>, master / 2).</summary>
    public const double PixelClockHz = MasterClockHz / 2;

    /// <summary>Pixel clocks per scanline (<c>HTOTAL</c>).</summary>
    public const int HorizontalTotal = 0x140;

    /// <summary>Scanlines per frame (<c>VTOTAL</c>).</summary>
    public const int VerticalTotal = 0x106;

    /// <summary>Non-maskable interrupts per video frame (<c>NMIS_PER_FRAME</c>).</summary>
    public const int NmisPerFrame = 8;

    /// <summary>The video frame rate, about 59.64 Hz.</summary>
    public const double FrameRateHz = PixelClockHz / (HorizontalTotal * VerticalTotal);

    /// <summary>
    /// The rate of the NMI that runs the game's speech routine, about 477 Hz. The routine sends the next word
    /// only when the chip is no longer busy, so each word starts on the first NMI after the previous one ends.
    /// </summary>
    public const double NmiRateHz = FrameRateHz * NmisPerFrame;
}
