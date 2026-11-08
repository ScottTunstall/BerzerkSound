using System;
using System.IO;

namespace BerzerkSound.Audio;

/// <summary>
/// Chooses a file format from a file name.
/// </summary>
internal static class AudioFileFormats
{
    /// <summary>
    /// Returns the format for <paramref name="path"/>: Ogg Vorbis for a <c>.ogg</c> extension, and WAV for
    /// everything else, so file names that worked before still write WAV files.
    /// </summary>
    /// <param name="path">The output file path.</param>
    /// <returns>The format.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    public static AudioFileFormat FromPath(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        return string.Equals(Path.GetExtension(path), AudioFileExtensions.Ogg, StringComparison.OrdinalIgnoreCase)
            ? AudioFileFormat.Ogg
            : AudioFileFormat.Wav;
    }
}
