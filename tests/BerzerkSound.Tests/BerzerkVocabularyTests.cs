using System;
using System.Linq;
using BerzerkSound.Vocabulary;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class BerzerkVocabularyTests
{
    [Fact]
    public void ListsEveryWordOnceInWordNumberOrder() =>
        Assert.Equal(Enum.GetValues<SpeechWord>().Order(), BerzerkVocabulary.Entries.Select(entry => entry.Word));

    [Fact]
    public void CoversTheRomWordTableFromZeroToRobot()
    {
        Assert.Equal(0, (int)BerzerkVocabulary.Entries[0].Word);
        Assert.Equal(0x1C, (int)BerzerkVocabulary.Entries[^1].Word);
        Assert.Equal(0x1D, BerzerkVocabulary.Entries.Count);
    }

    [Fact]
    public void HasNoSpellingTwice()
    {
        string[] spellings = [.. BerzerkVocabulary.Entries.SelectMany(entry => entry.Spellings)];

        Assert.Equal(spellings.Length, spellings.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Theory]
    [InlineData("humanoid", (int)SpeechWord.Humanoid)]
    [InlineData("HUMANOID", (int)SpeechWord.Humanoid)]
    [InlineData("their", (int)SpeechWord.There)]
    [InlineData("there", (int)SpeechWord.There)]
    public void FindsAWordBySpelling(string spelling, int expected) =>
        Assert.Equal((SpeechWord)expected, BerzerkVocabulary.Find(spelling));

    [Fact]
    public void ReturnsNullForAnUnknownWord() => Assert.Null(BerzerkVocabulary.Find("hello"));
}
