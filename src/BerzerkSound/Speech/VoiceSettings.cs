using System;

namespace BerzerkSound.Speech;

/// <summary>
/// The pitch and volume the game writes to the speech board before a phrase (port 44h, <c>01vvvppp</c>).
/// </summary>
internal sealed class VoiceSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VoiceSettings"/> class.
    /// </summary>
    /// <param name="pitch">The pitch setting, 0 to 7; each step speeds up the speech clock.</param>
    /// <param name="volume">The volume setting, 0 (silent) to 7.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pitch"/> or <paramref name="volume"/> is outside 0 to 7.</exception>
    public VoiceSettings(int pitch, int volume)
    {
        if (pitch is < VoiceLimits.MinimumPitch or > VoiceLimits.MaximumPitch)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pitch),
                pitch,
                $"The pitch must be from {VoiceLimits.MinimumPitch} to {VoiceLimits.MaximumPitch}.");
        }

        if (volume is < VoiceLimits.MinimumVolume or > VoiceLimits.MaximumVolume)
        {
            throw new ArgumentOutOfRangeException(
                nameof(volume),
                volume,
                $"The volume must be from {VoiceLimits.MinimumVolume} to {VoiceLimits.MaximumVolume}.");
        }

        Pitch = pitch;
        Volume = volume;
    }

    /// <summary>Gets the default settings: those of the game's fixed phrases.</summary>
    public static VoiceSettings Default { get; } = new(VoiceLimits.DefaultPitch, VoiceLimits.DefaultVolume);

    /// <summary>Gets the pitch setting, 0 to 7.</summary>
    public int Pitch { get; }

    /// <summary>Gets the volume setting, 0 to 7.</summary>
    public int Volume { get; }

    /// <summary>
    /// Gets the S14001A clock rate, which is also its output sample rate: from about 19.5 kHz (pitch 0) to
    /// 34.7 kHz (pitch 7), as in MAME's <c>berzerk_state::audio_w</c>.
    /// </summary>
    public double ChipClockHz => BerzerkTiming.SpeechClockHz / (BerzerkTiming.PitchDividerBase - Pitch) / BerzerkTiming.SecondDivider;

    /// <summary>Gets the output gain, from 0 (volume 0) to 1 (volume 7).</summary>
    public float Gain => (float)Volume / VoiceLimits.MaximumVolume;
}
