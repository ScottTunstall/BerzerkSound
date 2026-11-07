using BerzerkSound.Speech;

namespace BerzerkSound.Cli;

/// <summary>
/// What the user asked for on the command line.
/// </summary>
internal sealed class CommandLineOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineOptions"/> class.
    /// </summary>
    /// <param name="text">The text to say.</param>
    /// <param name="voice">The pitch and volume.</param>
    /// <param name="outputFile">The WAV file to write instead of playing, if any.</param>
    /// <param name="romPath">The ROM directory or zip to use instead of the default locations, if any.</param>
    /// <param name="showHelp">Whether the user asked for usage information.</param>
    /// <param name="listWords">Whether the user asked for the list of words Berzerk can say.</param>
    public CommandLineOptions(string text, VoiceSettings voice, string? outputFile, string? romPath, bool showHelp, bool listWords)
    {
        Text = text;
        Voice = voice;
        OutputFile = outputFile;
        RomPath = romPath;
        ShowHelp = showHelp;
        ListWords = listWords;
    }

    /// <summary>Gets the text to say.</summary>
    public string Text { get; }

    /// <summary>Gets the pitch and volume.</summary>
    public VoiceSettings Voice { get; }

    /// <summary>Gets the WAV file to write instead of playing, or <see langword="null"/> to play.</summary>
    public string? OutputFile { get; }

    /// <summary>Gets the ROM directory or zip, or <see langword="null"/> to search the default locations.</summary>
    public string? RomPath { get; }

    /// <summary>Gets a value indicating whether the user asked for usage information.</summary>
    public bool ShowHelp { get; }

    /// <summary>Gets a value indicating whether the user asked for the list of words Berzerk can say.</summary>
    public bool ListWords { get; }
}
