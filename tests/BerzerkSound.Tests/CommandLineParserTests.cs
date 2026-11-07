using System;
using BerzerkSound.Cli;
using BerzerkSound.Speech;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class CommandLineParserTests
{
    [Fact]
    public void UsesDefaultsWhenOnlyWordsAreGiven()
    {
        CommandLineOptions options = ParseSuccessfully("Intruder alert");

        Assert.Equal("Intruder alert", options.Text);
        Assert.Equal(VoiceLimits.DefaultPitch, options.Voice.Pitch);
        Assert.Equal(VoiceLimits.DefaultVolume, options.Voice.Volume);
        Assert.Null(options.OutputFile);
        Assert.Null(options.RomPath);
        Assert.False(options.ShowHelp);
        Assert.False(options.ListWords);
    }

    [Fact]
    public void JoinsSeparateWordsIntoOneText() =>
        Assert.Equal("chicken fight like a robot", ParseSuccessfully("chicken", "-p", "5", "fight", "like a robot").Text);

    [Theory]
    [InlineData("-f")]
    [InlineData("--file")]
    public void ReadsTheOutputFile(string option) =>
        Assert.Equal("out.wav", ParseSuccessfully(option, "out.wav", "robot").OutputFile);

    [Theory]
    [InlineData("-r")]
    [InlineData("--rom")]
    public void ReadsTheRomPath(string option) =>
        Assert.Equal("berzerk.zip", ParseSuccessfully(option, "berzerk.zip", "robot").RomPath);

    [Theory]
    [InlineData("-p")]
    [InlineData("--pitch")]
    public void ReadsThePitch(string option) => Assert.Equal(6, ParseSuccessfully(option, "6", "robot").Voice.Pitch);

    [Fact]
    public void ReadsTheVolume() => Assert.Equal(4, ParseSuccessfully("--volume", "4", "robot").Voice.Volume);

    [Fact]
    public void LetsTheLastRepeatedOptionWin() => Assert.Equal(2, ParseSuccessfully("-p", "1", "-p", "2", "robot").Voice.Pitch);

    [Fact]
    public void TreatsEverythingAfterDoubleDashAsWords()
    {
        CommandLineOptions options = ParseSuccessfully("-p", "1", "--", "-robot", "-p");

        Assert.Equal("-robot -p", options.Text);
        Assert.Equal(1, options.Voice.Pitch);
    }

    [Fact]
    public void TreatsALoneDashAsText() => Assert.Equal("-", ParseSuccessfully("-").Text);

    [Theory]
    [InlineData("-h")]
    [InlineData("--help")]
    [InlineData("-?")]
    public void AllowsHelpWithoutWords(string option) => Assert.True(ParseSuccessfully(option).ShowHelp);

    [Theory]
    [InlineData("-l")]
    [InlineData("--list")]
    public void AllowsTheWordListWithoutWords(string option) => Assert.True(ParseSuccessfully(option).ListWords);

    [Fact]
    public void RequiresWords() => Assert.Contains("No words", ParseError(), StringComparison.Ordinal);

    [Fact]
    public void RejectsWhitespaceOnlyWords() => Assert.Contains("No words", ParseError("   "), StringComparison.Ordinal);

    [Theory]
    [InlineData("-p", "-1")]
    [InlineData("-p", "8")]
    [InlineData("-p", "high")]
    [InlineData("--volume", "-1")]
    [InlineData("--volume", "8")]
    public void RejectsOutOfRangeValues(string option, string value) =>
        Assert.Contains($"Option '{option}' needs a whole number", ParseError(option, value, "robot"), StringComparison.Ordinal);

    [Fact]
    public void RejectsAMissingValue() => Assert.Contains("needs a value", ParseError("robot", "-f"), StringComparison.Ordinal);

    [Fact]
    public void RejectsAnEmptyPath() => Assert.Contains("needs a file path", ParseError("-f", " ", "robot"), StringComparison.Ordinal);

    [Fact]
    public void RejectsAnUnknownOption() => Assert.Contains("Unknown option '-x'", ParseError("-x", "robot"), StringComparison.Ordinal);

    [Fact]
    public void RejectsNullArguments() => Assert.Throws<ArgumentNullException>(() => CommandLineParser.Parse(null!));

    private static CommandLineOptions ParseSuccessfully(params string[] arguments)
    {
        CommandLineParseResult result = CommandLineParser.Parse(arguments);

        Assert.Equal(string.Empty, result.Error);
        Assert.NotNull(result.Options);
        return result.Options;
    }

    private static string ParseError(params string[] arguments)
    {
        CommandLineParseResult result = CommandLineParser.Parse(arguments);

        Assert.Null(result.Options);
        return result.Error;
    }
}
