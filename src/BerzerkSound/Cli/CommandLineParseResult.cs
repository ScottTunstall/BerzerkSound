namespace BerzerkSound.Cli;

/// <summary>
/// The outcome of parsing the command line: either options or an error message.
/// </summary>
internal sealed class CommandLineParseResult
{
    private CommandLineParseResult(CommandLineOptions? options, string error)
    {
        Options = options;
        Error = error;
    }

    /// <summary>Gets the parsed options, or <see langword="null"/> if parsing failed.</summary>
    public CommandLineOptions? Options { get; }

    /// <summary>Gets the reason parsing failed, or an empty string if it succeeded.</summary>
    public string Error { get; }

    /// <summary>Creates a successful result.</summary>
    /// <param name="options">The parsed options.</param>
    /// <returns>The result.</returns>
    public static CommandLineParseResult Success(CommandLineOptions options) => new(options, string.Empty);

    /// <summary>Creates a failed result.</summary>
    /// <param name="error">Why parsing failed, phrased for the user.</param>
    /// <returns>The result.</returns>
    public static CommandLineParseResult Failure(string error) => new(null, error);
}
