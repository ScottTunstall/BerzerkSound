using System;
using System.Collections.Generic;
using System.Linq;

namespace BerzerkSound.Vocabulary;

/// <summary>
/// Translates text into the words of Berzerk's vocabulary.
/// </summary>
/// <remarks>
/// Words are runs of letters and apostrophes; everything else (spaces, punctuation, digits) separates
/// them. Case is ignored. Nothing is guessed: every word must be in <see cref="BerzerkVocabulary"/>.
/// </remarks>
internal static class PhraseTranslator
{
    /// <summary>
    /// Translates <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text, for example "Intruder alert! Intruder alert!".</param>
    /// <returns>The words, or an error naming every word Berzerk cannot say.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    public static PhraseTranslation Translate(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        List<string> spellings = SplitIntoWords(text);

        if (spellings.Count == 0)
        {
            return PhraseTranslation.Failure("No words to say were given.");
        }

        List<SpeechWord?> words = [.. spellings.Select(BerzerkVocabulary.Find)];

        List<string> unknown =
        [
            .. spellings
                .Where((_, index) => words[index] is null)
                .Distinct(StringComparer.OrdinalIgnoreCase),
        ];

        if (unknown.Count > 0)
        {
            string quoted = string.Join(", ", unknown.Select(spelling => $"'{spelling}'"));
            string noun = unknown.Count == 1 ? "word" : "words";

            return PhraseTranslation.Failure($"Berzerk cannot say the {noun} {quoted}. Run with --list to see the words it knows.");
        }

        return PhraseTranslation.Success([.. words.OfType<SpeechWord>()]);
    }

    private static List<string> SplitIntoWords(string text)
    {
        List<string> words = [];
        int start = -1;

        for (int index = 0; index <= text.Length; index++)
        {
            bool isWordCharacter = index < text.Length && IsWordCharacter(text[index]);

            if (isWordCharacter && start < 0)
            {
                start = index;
            }
            else if (!isWordCharacter && start >= 0)
            {
                words.Add(text[start..index]);
                start = -1;
            }
        }

        return words;
    }

    private static bool IsWordCharacter(char character) => char.IsLetter(character) || character == '\'';
}
