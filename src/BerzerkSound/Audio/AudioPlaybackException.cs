using System;

namespace BerzerkSound.Audio;

/// <summary>
/// Thrown when speech cannot be played through the speakers.
/// </summary>
internal sealed class AudioPlaybackException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioPlaybackException"/> class.
    /// </summary>
    public AudioPlaybackException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioPlaybackException"/> class.
    /// </summary>
    /// <param name="message">Why playback failed.</param>
    public AudioPlaybackException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioPlaybackException"/> class.
    /// </summary>
    /// <param name="message">Why playback failed.</param>
    /// <param name="innerException">The underlying failure.</param>
    public AudioPlaybackException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
