using System.Collections.Generic;
using BerzerkSound.Speech;

namespace BerzerkSound.Cli;

/// <summary>
/// Collects option values while the command line is parsed.
/// </summary>
internal sealed class CommandLineOptionsBuilder
{
    private readonly List<string> _textParts = [];
    private int _pitch = VoiceLimits.DefaultPitch;
    private int _volume = VoiceLimits.DefaultVolume;
    private string? _outputFile;
    private string? _romPath;
    private bool _showHelp;
    private bool _listWords;

    /// <summary>Adds a word (or quoted phrase) to the text to say.</summary>
    /// <param name="text">The text.</param>
    public void AddText(string text) => _textParts.Add(text);

    /// <summary>Sets the pitch. The last value given wins.</summary>
    /// <param name="pitch">The pitch, already range-checked.</param>
    public void SetPitch(int pitch) => _pitch = pitch;

    /// <summary>Sets the volume. The last value given wins.</summary>
    /// <param name="volume">The volume, already range-checked.</param>
    public void SetVolume(int volume) => _volume = volume;

    /// <summary>Sets the WAV file to write. The last value given wins.</summary>
    /// <param name="path">The file path.</param>
    public void SetOutputFile(string path) => _outputFile = path;

    /// <summary>Sets the ROM directory or zip. The last value given wins.</summary>
    /// <param name="path">The path.</param>
    public void SetRomPath(string path) => _romPath = path;

    /// <summary>Records that usage information was requested.</summary>
    public void RequestHelp() => _showHelp = true;

    /// <summary>Records that the word list was requested.</summary>
    public void RequestWordList() => _listWords = true;

    /// <summary>
    /// Returns the options collected so far.
    /// </summary>
    /// <returns>The options.</returns>
    /// <exception cref="CommandLineException">No text was given, and neither help nor the word list was requested.</exception>
    public CommandLineOptions Build()
    {
        string text = string.Join(" ", _textParts).Trim();

        if (text.Length == 0 && !_showHelp && !_listWords)
        {
            throw new CommandLineException("No words to say were given.");
        }

        return new CommandLineOptions(text, new VoiceSettings(_pitch, _volume), _outputFile, _romPath, _showHelp, _listWords);
    }
}
