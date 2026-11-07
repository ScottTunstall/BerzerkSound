namespace BerzerkSound.Audio;

/// <summary>
/// Constants of the RIFF WAVE format written by <see cref="WaveFileEncoder"/>: 16-bit mono PCM.
/// </summary>
internal static class WaveFormat
{
    /// <summary>The RIFF container tag.</summary>
    public const string RiffTag = "RIFF";

    /// <summary>The WAVE form type.</summary>
    public const string WaveTag = "WAVE";

    /// <summary>The format chunk tag (note the trailing space).</summary>
    public const string FormatChunkTag = "fmt ";

    /// <summary>The sample data chunk tag.</summary>
    public const string DataChunkTag = "data";

    /// <summary>Size of the PCM format chunk body.</summary>
    public const int FormatChunkSize = 16;

    /// <summary>Bytes in the RIFF header that the RIFF size field counts before the data: "WAVE", the format chunk and the data chunk header.</summary>
    public const int HeaderBytesAfterRiffSize = 36;

    /// <summary>Bytes taken by the "RIFF" tag and the RIFF size field itself.</summary>
    public const int RiffPreambleSize = 8;

    /// <summary>Format code for uncompressed PCM.</summary>
    public const short PcmFormat = 1;

    /// <summary>Mono.</summary>
    public const short ChannelCount = 1;

    /// <summary>Bits per sample.</summary>
    public const short BitsPerSample = 16;

    /// <summary>Bytes per sample (and per frame, being mono).</summary>
    public const short BytesPerSample = BitsPerSample / 8;

    /// <summary>The largest 16-bit sample value.</summary>
    public const float FullScale = short.MaxValue;
}
