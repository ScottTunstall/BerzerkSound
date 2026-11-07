using System.IO;

namespace BerzerkSound.Audio;

/// <summary>
/// Writes speech to a WAV file, replacing any existing file.
/// </summary>
internal sealed class WaveFileOutput : IAudioOutput
{
    private readonly string _path;

    /// <summary>
    /// Initializes a new instance of the <see cref="WaveFileOutput"/> class.
    /// </summary>
    /// <param name="path">The WAV file to write.</param>
    public WaveFileOutput(string path)
    {
        _path = path;
    }

    /// <inheritdoc/>
    public void Deliver(AudioClip clip) => File.WriteAllBytes(_path, WaveFileEncoder.Encode(clip));
}
