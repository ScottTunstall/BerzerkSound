namespace BerzerkSound.Cli;

/// <summary>
/// The command-line switches the app accepts.
/// </summary>
internal static class CommandLineSwitches
{
    /// <summary>Write the speech to a WAV file instead of playing it.</summary>
    public const string FileShort = "-f";

    /// <summary>Long form of <see cref="FileShort"/>.</summary>
    public const string FileLong = "--file";

    /// <summary>Pitch (speech clock) setting.</summary>
    public const string PitchShort = "-p";

    /// <summary>Long form of <see cref="PitchShort"/>.</summary>
    public const string PitchLong = "--pitch";

    /// <summary>Volume setting. There is no short form: <c>-v</c> conventionally means verbose or version.</summary>
    public const string VolumeLong = "--volume";

    /// <summary>Directory or MAME zip holding the speech ROMs.</summary>
    public const string RomShort = "-r";

    /// <summary>Long form of <see cref="RomShort"/>.</summary>
    public const string RomLong = "--rom";

    /// <summary>List the words Berzerk can say.</summary>
    public const string ListShort = "-l";

    /// <summary>Long form of <see cref="ListShort"/>.</summary>
    public const string ListLong = "--list";

    /// <summary>Show usage.</summary>
    public const string HelpShort = "-h";

    /// <summary>Long form of <see cref="HelpShort"/>.</summary>
    public const string HelpLong = "--help";

    /// <summary>Windows-style alternative for <see cref="HelpShort"/>.</summary>
    public const string HelpQuestion = "-?";

    /// <summary>Everything after this is text to say, even if it starts with '-'.</summary>
    public const string EndOfOptions = "--";

    /// <summary>Arguments starting with this character are options.</summary>
    public const char OptionPrefix = '-';
}
