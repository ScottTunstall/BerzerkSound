namespace BerzerkSound;

/// <summary>
/// The process exit codes.
/// </summary>
internal enum ExitCode
{
    /// <summary>The words were said or written, or help or the word list was shown.</summary>
    Success = 0,

    /// <summary>The command line was invalid.</summary>
    InvalidArguments = 2,

    /// <summary>The speech ROMs were missing, unreadable or not a known dump.</summary>
    RomUnavailable = 3,

    /// <summary>The speech could not be played or written to the file.</summary>
    OutputFailed = 4,

    /// <summary>The text contained a word that is not in Berzerk's vocabulary.</summary>
    UnknownWord = 5,
}
