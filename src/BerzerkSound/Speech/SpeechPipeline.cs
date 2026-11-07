using System;
using System.Collections.Generic;
using BerzerkSound.Audio;
using BerzerkSound.Vocabulary;

namespace BerzerkSound.Speech;

/// <summary>
/// Says words: renders them and delivers the audio to an output.
/// </summary>
internal sealed class SpeechPipeline
{
    private readonly ISpeechRenderer _renderer;
    private readonly IAudioOutput _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechPipeline"/> class.
    /// </summary>
    /// <param name="renderer">Turns words into audio.</param>
    /// <param name="output">Where the audio goes.</param>
    /// <exception cref="ArgumentNullException"><paramref name="renderer"/> or <paramref name="output"/> is <see langword="null"/>.</exception>
    public SpeechPipeline(ISpeechRenderer renderer, IAudioOutput output)
    {
        ArgumentNullException.ThrowIfNull(renderer);
        ArgumentNullException.ThrowIfNull(output);

        _renderer = renderer;
        _output = output;
    }

    /// <summary>
    /// Renders <paramref name="words"/> and delivers them, returning when the output has them.
    /// </summary>
    /// <param name="words">The words to say.</param>
    /// <param name="voice">The pitch and volume.</param>
    public void Say(IReadOnlyList<SpeechWord> words, VoiceSettings voice) => _output.Deliver(_renderer.Render(words, voice));
}
