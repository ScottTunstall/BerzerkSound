using System;
using System.IO;
using BerzerkSound.Audio;
using BerzerkSound.Cli;
using BerzerkSound.Roms;
using BerzerkSound.Speech;
using BerzerkSound.Vocabulary;

namespace BerzerkSound;

/// <summary>
/// Entry point and composition root.
/// </summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        CommandLineParseResult parsed = CommandLineParser.Parse(args);

        if (parsed.Options is null)
        {
            Console.Error.WriteLine($"BerzerkSound: {parsed.Error}");
            Console.Error.WriteLine(UsageText.HelpHint);
            return (int)ExitCode.InvalidArguments;
        }

        if (parsed.Options.ShowHelp)
        {
            Console.Out.Write(UsageText.Text);
            return (int)ExitCode.Success;
        }

        if (parsed.Options.ListWords)
        {
            Console.Out.Write(WordListText.Text);
            return (int)ExitCode.Success;
        }

        PhraseTranslation translation = PhraseTranslator.Translate(parsed.Options.Text);

        if (!translation.IsSuccess)
        {
            Console.Error.WriteLine($"BerzerkSound: {translation.Error}");
            return (int)ExitCode.UnknownWord;
        }

        return (int)Say(translation, parsed.Options);
    }

    private static ExitCode Say(PhraseTranslation translation, CommandLineOptions options)
    {
        try
        {
            CreatePipeline(options).Say(translation.Words, options.Voice);
            return ExitCode.Success;
        }
        catch (Exception exception) when (exception is RomNotFoundException or InvalidRomException)
        {
            return Fail(exception, ExitCode.RomUnavailable);
        }
        catch (Exception exception) when (exception
            is AudioPlaybackException
            or PlatformNotSupportedException
            or IOException
            or UnauthorizedAccessException)
        {
            return Fail(exception, ExitCode.OutputFailed);
        }
    }

    private static SpeechPipeline CreatePipeline(CommandLineOptions options)
    {
        // The output is created first so that an unsupported output fails before the ROMs are read.
        IAudioOutput output = CreateOutput(options.OutputFile);

        RomSetDefinition romSet = KnownRomSets.EnglishBerzerk;
        var locator = new RomLocator(romSet, [AppContext.BaseDirectory, Environment.CurrentDirectory]);
        var loader = new SpeechRomLoader(romSet);

        return new SpeechPipeline(new BerzerkSpeechRenderer(loader.Load(locator.Locate(options.RomPath))), output);
    }

    private static IAudioOutput CreateOutput(string? outputFile)
    {
        if (outputFile is not null)
        {
            return new WaveFileOutput(outputFile);
        }

        return OperatingSystem.IsWindows()
            ? new SpeakerOutput()
            : throw new PlatformNotSupportedException("Playing speech needs Windows. Use -f to write a WAV file instead.");
    }

    private static ExitCode Fail(Exception exception, ExitCode exitCode)
    {
        Console.Error.WriteLine($"BerzerkSound: {exception.Message}");
        return exitCode;
    }
}
