namespace BerzerkSound.Audio;

/// <summary>
/// The file extensions that choose a file format.
/// </summary>
internal static class AudioFileExtensions
{
    /// <summary>The extension of WAV files, added when the user gives a file name with no extension.</summary>
    public const string Wav = ".wav";

    /// <summary>The extension of Ogg Vorbis files. Matched ignoring case.</summary>
    public const string Ogg = ".ogg";
}
