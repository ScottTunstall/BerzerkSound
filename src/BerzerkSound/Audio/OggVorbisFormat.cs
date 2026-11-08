namespace BerzerkSound.Audio;

/// <summary>
/// Settings of the Ogg Vorbis files written by <see cref="OggVorbisFileEncoder"/>.
/// </summary>
internal static class OggVorbisFormat
{
    /// <summary>Variable-bit-rate quality from -0.1 (smallest) to 1 (best).</summary>
    public const float Quality = 0.5f;

    /// <summary>The logical stream serial number. Fixed, so the same speech always gives the same file.</summary>
    public const int StreamSerial = 1;

    /// <summary>Mono.</summary>
    public const int ChannelCount = 1;

    /// <summary>
    /// Silence written before the speech. The encoder library reports each page's position up to half a
    /// Vorbis block (512 or 1024 samples) too early, and a decoder that follows those positions discards
    /// that many samples from the start of the stream. Without this lead-in the first sound of the speech
    /// is cut off. One long Vorbis block is more than enough.
    /// </summary>
    public const int LeadInSamples = 2048;

    /// <summary>
    /// Silence written after the speech, because the same early positions also shorten the end of the stream.
    /// </summary>
    public const int TailPaddingSamples = 2048;

    /// <summary>How many samples are handed to the encoder at a time.</summary>
    public const int BlockSamples = 4096;
}
