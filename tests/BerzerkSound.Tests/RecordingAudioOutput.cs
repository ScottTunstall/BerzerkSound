using System.Collections.Generic;
using BerzerkSound.Audio;

namespace BerzerkSound.Tests;

/// <summary>
/// An audio output that remembers what it was given.
/// </summary>
internal sealed class RecordingAudioOutput : IAudioOutput
{
    private readonly List<AudioClip> _clips = [];

    /// <summary>Gets the clips delivered so far.</summary>
    public IReadOnlyList<AudioClip> Clips => _clips;

    /// <inheritdoc/>
    public void Deliver(AudioClip clip) => _clips.Add(clip);
}
