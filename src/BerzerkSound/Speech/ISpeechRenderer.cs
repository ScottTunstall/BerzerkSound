using System.Collections.Generic;
using BerzerkSound.Audio;
using BerzerkSound.Vocabulary;

namespace BerzerkSound.Speech;

/// <summary>
/// Turns words into audio.
/// </summary>
internal interface ISpeechRenderer
{
    /// <summary>
    /// Renders <paramref name="words"/>, one after another.
    /// </summary>
    /// <param name="words">The words to say.</param>
    /// <param name="voice">The pitch and volume.</param>
    /// <returns>The audio.</returns>
    AudioClip Render(IReadOnlyList<SpeechWord> words, VoiceSettings voice);
}
