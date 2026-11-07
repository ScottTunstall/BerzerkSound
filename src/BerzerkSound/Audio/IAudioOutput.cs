namespace BerzerkSound.Audio;

/// <summary>
/// Somewhere rendered speech goes: the speakers or a file.
/// </summary>
internal interface IAudioOutput
{
    /// <summary>
    /// Sends <paramref name="clip"/> to this output, returning when it has been delivered.
    /// </summary>
    /// <param name="clip">The audio to deliver.</param>
    void Deliver(AudioClip clip);
}
