using System.Collections.Generic;

namespace BerzerkSound.Vocabulary;

/// <summary>
/// A word Berzerk can say and the spellings that select it.
/// </summary>
/// <param name="Word">The word.</param>
/// <param name="Spellings">The spellings that select the word, preferred spelling first.</param>
/// <param name="Note">Extra information shown in the word list, or an empty string.</param>
internal sealed record VocabularyEntry(SpeechWord Word, IReadOnlyList<string> Spellings, string Note);
