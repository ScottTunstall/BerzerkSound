namespace BerzerkSound.Audio;

/// <summary>
/// Creates the output that writes a speech file in the format its name asks for.
/// </summary>
internal static class FileOutputFactory
{
    /// <summary>
    /// Creates an output for <paramref name="path"/>: Ogg Vorbis for <c>.ogg</c>, WAV otherwise.
    /// </summary>
    /// <param name="path">The file to write.</param>
    /// <returns>The output.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    public static IAudioOutput Create(string path) => AudioFileFormats.FromPath(path) switch
    {
        AudioFileFormat.Ogg => new OggFileOutput(path),
        _ => new WaveFileOutput(path),
    };
}
