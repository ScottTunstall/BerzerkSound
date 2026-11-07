using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BerzerkSound.Speech;

namespace BerzerkSound.Cli;

/// <summary>
/// Parses the command line: <c>[options] [--] words…</c>.
/// </summary>
/// <remarks>
/// The words may be one quoted argument or several, which are joined with spaces. Options may appear
/// anywhere before <c>--</c>; everything after <c>--</c> is text. If an option is repeated, the last value wins.
/// </remarks>
internal static class CommandLineParser
{
    /// <summary>
    /// Parses <paramref name="arguments"/>.
    /// </summary>
    /// <param name="arguments">The command-line arguments, without the program name.</param>
    /// <returns>The options, or an error message for the user.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="arguments"/> is <see langword="null"/>.</exception>
    public static CommandLineParseResult Parse(IReadOnlyList<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        try
        {
            return CommandLineParseResult.Success(ParseOptions(arguments));
        }
        catch (CommandLineException exception)
        {
            return CommandLineParseResult.Failure(exception.Message);
        }
    }

    private static CommandLineOptions ParseOptions(IReadOnlyList<string> arguments)
    {
        int separator = arguments.ToList().IndexOf(CommandLineSwitches.EndOfOptions);
        int optionCount = separator < 0 ? arguments.Count : separator;

        var builder = new CommandLineOptionsBuilder();
        var cursor = new ArgumentCursor([.. arguments.Take(optionCount)]);

        while (!cursor.IsAtEnd)
        {
            ParseArgument(cursor, builder);
            cursor.MoveNext();
        }

        foreach (string text in arguments.Skip(optionCount + 1))
        {
            builder.AddText(text);
        }

        return builder.Build();
    }

    private static void ParseArgument(ArgumentCursor cursor, CommandLineOptionsBuilder builder)
    {
        string argument = cursor.Current;

        if (!IsOption(argument))
        {
            builder.AddText(argument);
            return;
        }

        switch (argument)
        {
            case CommandLineSwitches.HelpShort or CommandLineSwitches.HelpLong or CommandLineSwitches.HelpQuestion:
                builder.RequestHelp();
                break;

            case CommandLineSwitches.ListShort or CommandLineSwitches.ListLong:
                builder.RequestWordList();
                break;

            case CommandLineSwitches.FileShort or CommandLineSwitches.FileLong:
                cursor.MoveNext();
                builder.SetOutputFile(RequirePath(cursor, argument));
                break;

            case CommandLineSwitches.RomShort or CommandLineSwitches.RomLong:
                cursor.MoveNext();
                builder.SetRomPath(RequirePath(cursor, argument));
                break;

            case CommandLineSwitches.PitchShort or CommandLineSwitches.PitchLong:
                cursor.MoveNext();
                builder.SetPitch(RequireInteger(cursor, argument, VoiceLimits.MinimumPitch, VoiceLimits.MaximumPitch));
                break;

            case CommandLineSwitches.VolumeLong:
                cursor.MoveNext();
                builder.SetVolume(RequireInteger(cursor, argument, VoiceLimits.MinimumVolume, VoiceLimits.MaximumVolume));
                break;

            default:
                throw new CommandLineException(
                    $"Unknown option '{argument}'. To say words that start with '-', put {CommandLineSwitches.EndOfOptions} before them.");
        }
    }

    private static bool IsOption(string argument) => argument.Length > 1 && argument[0] == CommandLineSwitches.OptionPrefix;

    private static string RequireValue(ArgumentCursor cursor, string option) =>
        cursor.IsAtEnd ? throw new CommandLineException($"Option '{option}' needs a value.") : cursor.Current;

    private static string RequirePath(ArgumentCursor cursor, string option)
    {
        string path = RequireValue(cursor, option);

        if (path.Trim().Length == 0)
        {
            throw new CommandLineException($"Option '{option}' needs a file path.");
        }

        return path;
    }

    private static int RequireInteger(ArgumentCursor cursor, string option, int minimum, int maximum)
    {
        string value = RequireValue(cursor, option);
        bool isNumber = int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number);

        if (!isNumber || number < minimum || number > maximum)
        {
            throw new CommandLineException(string.Format(
                CultureInfo.InvariantCulture,
                "Option '{0}' needs a whole number from {1} to {2}, but was given '{3}'.",
                option,
                minimum,
                maximum,
                value));
        }

        return number;
    }
}
