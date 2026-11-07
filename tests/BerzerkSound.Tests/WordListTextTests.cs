using System;
using BerzerkSound.Vocabulary;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class WordListTextTests
{
    [Theory]
    [InlineData("  00  help (not used by the game)")]
    [InlineData("  0D  there / their")]
    [InlineData("  1C  robot")]
    public void ShowsEachWordWithItsNumber(string line) =>
        Assert.Contains(line + Environment.NewLine, WordListText.Text, StringComparison.Ordinal);
}
