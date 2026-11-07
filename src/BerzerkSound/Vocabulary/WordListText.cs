using System.Globalization;
using System.Linq;
using System.Text;

namespace BerzerkSound.Vocabulary;

/// <summary>
/// The word list shown for --list.
/// </summary>
internal static class WordListText
{
    /// <summary>Gets the word list: one word per line with its S14001A word number in hexadecimal.</summary>
    public static string Text { get; } = Create();

    private static string Create()
    {
        var text = new StringBuilder();

        _ = text.AppendLine("Words Berzerk can say (S14001A word number, word):");

        foreach (VocabularyEntry entry in BerzerkVocabulary.Entries)
        {
            string spellings = string.Join(" / ", entry.Spellings);
            string note = entry.Note.Length == 0 ? string.Empty : $" ({entry.Note})";

            _ = text.AppendLine(CultureInfo.InvariantCulture, $"  {(int)entry.Word:X2}  {spellings}{note}");
        }

        _ = text.AppendLine(CultureInfo.InvariantCulture, $"{BerzerkVocabulary.Entries.Sum(entry => entry.Spellings.Count)} spellings of {BerzerkVocabulary.Entries.Count} words.");
        return text.ToString();
    }
}
