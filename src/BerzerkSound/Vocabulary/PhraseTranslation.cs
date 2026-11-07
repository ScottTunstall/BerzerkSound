using System.Collections.Generic;

namespace BerzerkSound.Vocabulary;

/// <summary>
/// The outcome of translating text into Berzerk words: either the words or an error message.
/// </summary>
internal sealed class PhraseTranslation
{
    private PhraseTranslation(IReadOnlyList<SpeechWord> words, string error)
    {
        Words = words;
        Error = error;
    }

    /// <summary>Gets a value indicating whether every word in the text could be translated.</summary>
    public bool IsSuccess => Error.Length == 0;

    /// <summary>Gets the words to say, in order; empty if the translation failed.</summary>
    public IReadOnlyList<SpeechWord> Words { get; }

    /// <summary>Gets the reason the translation failed, or an empty string if it succeeded.</summary>
    public string Error { get; }

    /// <summary>Creates a successful result.</summary>
    /// <param name="words">The words to say; there must be at least one.</param>
    /// <returns>The result.</returns>
    public static PhraseTranslation Success(IReadOnlyList<SpeechWord> words) => new(words, string.Empty);

    /// <summary>Creates a failed result.</summary>
    /// <param name="error">Why the text cannot be said, phrased for the user.</param>
    /// <returns>The result.</returns>
    public static PhraseTranslation Failure(string error) => new([], error);
}
