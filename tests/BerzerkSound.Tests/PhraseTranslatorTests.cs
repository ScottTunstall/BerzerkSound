using System;
using BerzerkSound.Vocabulary;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class PhraseTranslatorTests
{
    [Fact]
    public void TranslatesAPhraseIgnoringPunctuation()
    {
        PhraseTranslation translation = PhraseTranslator.Translate("Intruder alert! Intruder alert!");

        Assert.True(translation.IsSuccess);
        Assert.Equal([SpeechWord.Intruder, SpeechWord.Alert, SpeechWord.Intruder, SpeechWord.Alert], translation.Words);
    }

    [Fact]
    public void IgnoresCase() =>
        Assert.Equal(
            [SpeechWord.Chicken, SpeechWord.Fight, SpeechWord.Like, SpeechWord.A, SpeechWord.Robot],
            PhraseTranslator.Translate("CHICKEN, fight like a RoBoT").Words);

    [Fact]
    public void AcceptsAnAlternativeSpelling() =>
        Assert.Equal([SpeechWord.There], PhraseTranslator.Translate("their").Words);

    [Fact]
    public void TreatsDigitsAsSeparators() =>
        Assert.Equal([SpeechWord.Robot, SpeechWord.Robot], PhraseTranslator.Translate("robot2robot").Words);

    [Fact]
    public void NamesEveryUnknownWordOnce()
    {
        PhraseTranslation translation = PhraseTranslator.Translate("hello robot world hello");

        Assert.False(translation.IsSuccess);
        Assert.Empty(translation.Words);
        Assert.Contains("words 'hello', 'world'.", translation.Error, StringComparison.Ordinal);
        Assert.Contains("--list", translation.Error, StringComparison.Ordinal);
    }

    [Fact]
    public void UsesTheSingularForOneUnknownWord() =>
        Assert.Contains("the word 'humanoids'", PhraseTranslator.Translate("humanoids").Error, StringComparison.Ordinal);

    [Fact]
    public void KeepsApostrophesInWords() =>
        Assert.Contains("'there's'", PhraseTranslator.Translate("there's").Error, StringComparison.Ordinal);

    [Theory]
    [InlineData("")]
    [InlineData("!!! ... 42")]
    public void RejectsTextWithNoWords(string text) =>
        Assert.Equal("No words to say were given.", PhraseTranslator.Translate(text).Error);

    [Fact]
    public void RejectsNullText() => Assert.Throws<ArgumentNullException>(() => PhraseTranslator.Translate(null!));
}
