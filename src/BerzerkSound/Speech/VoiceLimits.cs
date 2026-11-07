namespace BerzerkSound.Speech;

/// <summary>
/// Ranges and defaults of the speech controls: the two 3-bit fields of the byte the game writes to the
/// speech board (<c>01vvvppp</c>).
/// </summary>
internal static class VoiceLimits
{
    /// <summary>The lowest pitch setting (the slowest speech clock).</summary>
    public const int MinimumPitch = 0;

    /// <summary>The highest pitch setting (the fastest speech clock).</summary>
    public const int MaximumPitch = 7;

    /// <summary>
    /// The default pitch: 3, the setting of the game's fixed phrases ("Intruder alert! Intruder alert!",
    /// "Got the humanoid, got the intruder!" and "Chicken! Fight like a robot!" all start with 7Bh or 73h).
    /// </summary>
    public const int DefaultPitch = 3;

    /// <summary>The lowest volume setting, which is silent on the real board.</summary>
    public const int MinimumVolume = 0;

    /// <summary>The highest volume setting.</summary>
    public const int MaximumVolume = 7;

    /// <summary>The default volume: 7, the setting of "Intruder alert! Intruder alert!" (7Bh).</summary>
    public const int DefaultVolume = MaximumVolume;
}
