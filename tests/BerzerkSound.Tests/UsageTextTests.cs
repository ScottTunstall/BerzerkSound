using System;
using System.Linq;
using BerzerkSound.Cli;
using BerzerkSound.Vocabulary;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class UsageTextTests
{
    [Fact]
    public void ListsEverySupportedWord()
    {
        string[] words = [.. UsageText.Text.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries)];

        Assert.All(
            BerzerkVocabulary.Entries.SelectMany(entry => entry.Spellings),
            spelling => Assert.Contains(spelling, words));
    }

    [Fact]
    public void WrapsTheWordListWithinOneHundredColumns() =>
        Assert.All(UsageText.Text.Split('\n'), line => Assert.True(line.TrimEnd('\r').Length <= 105));

    [Fact]
    public void ShowsNoUnfilledPlaceholders() =>
        Assert.DoesNotContain("{", UsageText.Text, StringComparison.Ordinal);
}
