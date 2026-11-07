using System.Globalization;
using BerzerkSound.Speech;

namespace BerzerkSound.Cli;

/// <summary>
/// The help text shown for --help, and the hint shown after a command-line error.
/// </summary>
internal static class UsageText
{
    /// <summary>The line shown after a command-line error.</summary>
    public const string HelpHint = "Try 'BerzerkSound --help' for more information.";

    /// <summary>Gets the help text.</summary>
    public static string Text { get; } = string.Format(
        CultureInfo.InvariantCulture,
        """
        Usage: BerzerkSound [options] [--] <words>
               BerzerkSound --list

        Says <words> in the robot voice of Stern's 1980 arcade game Berzerk, through an emulation of its
        S14001A speech chip (ported from MAME) and the game's own speech ROMs. Only the words Berzerk knows
        can be said; run with --list to see them. Case and punctuation are ignored.

        Options:
          -f, --file <path>      Write the speech to a WAV file instead of playing it.
          -p, --pitch <{0}-{1}>      Speech clock setting; higher is faster and higher pitched (default {2},
                                 as in the game's "Intruder alert!").
              --volume <{3}-{4}>     Volume; {3} is silent, as on the real board (default {5}).
          -r, --rom <path>       Directory or MAME zip (berzerk.zip or frenzy.zip) holding the English
                                 speech ROMs (default: next to the app or in the current directory).
          -l, --list             List the words Berzerk can say.
          -h, --help             Show this help.
          --                     Treat everything after this as words, even if it starts with '-'.

        Exit codes: 0 success, 2 invalid arguments, 3 speech ROMs missing or invalid,
                    4 speech could not be played or written, 5 a word is not in Berzerk's vocabulary.

        Examples:
          BerzerkSound "Intruder alert! Intruder alert!"
          BerzerkSound -p 5 chicken fight like a robot
          BerzerkSound -f escape.wav "The humanoid must not escape"

        """,
        VoiceLimits.MinimumPitch,
        VoiceLimits.MaximumPitch,
        VoiceLimits.DefaultPitch,
        VoiceLimits.MinimumVolume,
        VoiceLimits.MaximumVolume,
        VoiceLimits.DefaultVolume);
}
