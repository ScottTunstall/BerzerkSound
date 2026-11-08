namespace BerzerkSound.Audio;

/// <summary>
/// The file formats speech can be saved in.
/// </summary>
internal enum AudioFileFormat
{
    /// <summary>Uncompressed 16-bit mono PCM in a RIFF WAVE file.</summary>
    Wav = 0,

    /// <summary>Ogg Vorbis.</summary>
    Ogg = 1,
}
