using System;
using System.Collections.Generic;
using System.Collections.Frozen;
using System.Linq;

namespace BerzerkSound.Vocabulary;

/// <summary>
/// Everything the English Berzerk speech ROMs can say.
/// </summary>
internal static class BerzerkVocabulary
{
    /// <summary>Gets every entry, in word-number order.</summary>
    public static IReadOnlyList<VocabularyEntry> Entries { get; } =
    [
        new(SpeechWord.Help, ["help"], "not used by the game"),
        new(SpeechWord.Kill, ["kill"], string.Empty),
        new(SpeechWord.Attack, ["attack"], string.Empty),
        new(SpeechWord.Charge, ["charge"], string.Empty),
        new(SpeechWord.Got, ["got"], string.Empty),
        new(SpeechWord.Shoot, ["shoot"], string.Empty),
        new(SpeechWord.Get, ["get"], string.Empty),
        new(SpeechWord.Is, ["is"], string.Empty),
        new(SpeechWord.Alert, ["alert"], string.Empty),
        new(SpeechWord.Detected, ["detected"], string.Empty),
        new(SpeechWord.The, ["the"], string.Empty),
        new(SpeechWord.In, ["in"], string.Empty),
        new(SpeechWord.It, ["it"], string.Empty),
        new(SpeechWord.There, ["there", "their"], string.Empty),
        new(SpeechWord.Where, ["where"], string.Empty),
        new(SpeechWord.Humanoid, ["humanoid"], string.Empty),
        new(SpeechWord.Coins, ["coins"], string.Empty),
        new(SpeechWord.Pocket, ["pocket"], string.Empty),
        new(SpeechWord.Intruder, ["intruder"], string.Empty),
        new(SpeechWord.No, ["no"], string.Empty),
        new(SpeechWord.Escape, ["escape"], string.Empty),
        new(SpeechWord.Destroy, ["destroy"], string.Empty),
        new(SpeechWord.Must, ["must"], string.Empty),
        new(SpeechWord.Not, ["not"], string.Empty),
        new(SpeechWord.Chicken, ["chicken"], string.Empty),
        new(SpeechWord.Fight, ["fight"], string.Empty),
        new(SpeechWord.Like, ["like"], string.Empty),
        new(SpeechWord.A, ["a"], string.Empty),
        new(SpeechWord.Robot, ["robot"], string.Empty),
    ];

    /// <summary>The words by spelling. Declared after <see cref="Entries"/>: static initializers run in declaration order.</summary>
    private static readonly FrozenDictionary<string, SpeechWord> WordsBySpelling = CreateLookup();

    /// <summary>
    /// Returns the word spelled <paramref name="spelling"/>, ignoring case.
    /// </summary>
    /// <param name="spelling">The spelling to look up.</param>
    /// <returns>The word, or <see langword="null"/> if Berzerk cannot say it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="spelling"/> is <see langword="null"/>.</exception>
    public static SpeechWord? Find(string spelling)
    {
        ArgumentNullException.ThrowIfNull(spelling);

        return WordsBySpelling.TryGetValue(spelling, out SpeechWord word) ? word : null;
    }

    private static FrozenDictionary<string, SpeechWord> CreateLookup() =>
        Entries
            .SelectMany(entry => entry.Spellings, (entry, spelling) => KeyValuePair.Create(spelling, entry.Word))
            .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
}
