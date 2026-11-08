using System.Globalization;
using System.Linq;
using System.Text;
using BerzerkSound.Speech;
using BerzerkSound.Vocabulary;

namespace BerzerkSound.Cli;

/// <summary>
/// The help text shown for --help, and the hint shown after a command-line error.
/// </summary>
internal static class UsageText
{
    /// <summary>The line shown after a command-line error.</summary>
    public const string HelpHint = "Try 'BerzerkSound --help' for more information.";

    /// <summary>The indent of the word list under its heading.</summary>
    private const string LineIndent = "  ";

    /// <summary>The width at which the word list wraps.</summary>
    private const int WordsLineWidth = 100;

    /// <summary>Gets the help text, including every word Berzerk can say.</summary>
    public static string Text { get; } = string.Format(
        CultureInfo.InvariantCulture,
        """
        Usage: BerzerkSound [options] [--] <words>
               BerzerkSound --list

        Says <words> in the robot voice of Stern's 1980 arcade game Berzerk, through an emulation of its
        S14001A speech chip (ported from MAME) and the game's own speech ROMs. Only the words Berzerk knows
        can be said; run with --list to see them. Case and punctuation are ignored.

        Options:
          -f, --file <path>      Write the speech to a file instead of playing it: Ogg Vorbis if the name
                                 ends in .ogg, otherwise WAV (.wav is added if the name has no extension).
          -p, --pitch <{0}-{1}>      Speech clock setting; higher is faster and higher pitched (default {2},
                                 as in the game's "Intruder alert!").
              --volume <{3}-{4}>     Volume; {3} is silent, as on the real board (default {5}).
          -r, --rom <path>       Directory or MAME zip (berzerk.zip or frenzy.zip) holding the English
                                 speech ROMs (default: next to the app or in the current directory).
          -l, --list             List the words Berzerk can say.
          -h, --help             Show this help.
          --                     Treat everything after this as words, even if it starts with '-'.

        Words Berzerk can say:
        {6}

        Exit codes: 0 success, 2 invalid arguments, 3 speech ROMs missing or invalid,
                    4 speech could not be played or written, 5 a word is not in Berzerk's vocabulary.

        Examples:
          BerzerkSound "INTRUDER ALERT INTRUDER ALERT"
          BerzerkSound "DESTROY THE CHICKEN"
          BerzerkSound "THE HUMANOID MUST NOT ESCAPE"
          BerzerkSound -p 5 chicken fight like a robot
          BerzerkSound -f escape.wav "The humanoid must not escape"
          BerzerkSound -f escape.ogg "The humanoid must not escape"

        """,
        VoiceLimits.MinimumPitch,
        VoiceLimits.MaximumPitch,
        VoiceLimits.DefaultPitch,
        VoiceLimits.MinimumVolume,
        VoiceLimits.MaximumVolume,
        VoiceLimits.DefaultVolume,
        WordsParagraph());

    private static string WordsParagraph()
    {
        var paragraph = new StringBuilder();
        var line = new StringBuilder(LineIndent);

        foreach (string spelling in BerzerkVocabulary.Entries.SelectMany(entry => entry.Spellings))
        {
            if (line.Length > LineIndent.Length && line.Length + spelling.Length + 1 > WordsLineWidth)
            {
                _ = paragraph.AppendLine(line.ToString().TrimEnd());
                _ = line.Clear().Append(LineIndent);
            }

            _ = line.Append(spelling).Append(' ');
        }

        return paragraph.Append(line.ToString().TrimEnd()).ToString();
    }
}
