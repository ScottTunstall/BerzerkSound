namespace BerzerkSound.Audio;

/// <summary>
/// Creates the output that writes a speech file in the format its name asks for.
/// </summary>
internal static class FileOutputFactory
{
    /// <summary>
    /// Creates an output for <paramref name="path"/>: Ogg Vorbis for <c>.ogg</c>, WAV otherwise. A name with
    /// no extension gets <c>.wav</c> added.
    /// </summary>
    /// <param name="path">The file to write.</param>
    /// <returns>The output.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    public static IAudioOutput Create(string path)
    {
        string fullName = OutputFilePaths.WithDefaultExtension(path);

        return AudioFileFormats.FromPath(fullName) switch
        {
            AudioFileFormat.Ogg => new OggFileOutput(fullName),
            _ => new WaveFileOutput(fullName),
        };
    }
}
