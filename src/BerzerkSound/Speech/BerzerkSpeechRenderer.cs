using System;
using System.Collections.Generic;
using System.Globalization;
using BerzerkSound.Audio;
using BerzerkSound.Roms;
using BerzerkSound.Synthesis;
using BerzerkSound.Vocabulary;

namespace BerzerkSound.Speech;

/// <summary>
/// Renders words through an emulated S14001A, feeding it the way the Berzerk game does.
/// </summary>
/// <remarks>
/// The game's NMI handler (at 1748h in the disassembly) checks the speech chip about 477 times a second.
/// When the chip is not busy it writes the next word number to port 44h, which MAME's
/// <c>berzerk_state::audio_w</c> latches onto the chip before pulsing START. The renderer reproduces that,
/// so the short gaps between words match the arcade machine. Rendering stops at the first NMI after the
/// last word ends.
/// </remarks>
internal sealed class BerzerkSpeechRenderer : ISpeechRenderer
{
    private readonly SpeechRom _rom;

    /// <summary>
    /// Initializes a new instance of the <see cref="BerzerkSpeechRenderer"/> class.
    /// </summary>
    /// <param name="rom">The speech ROM.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rom"/> is <see langword="null"/>.</exception>
    public BerzerkSpeechRenderer(SpeechRom rom)
    {
        ArgumentNullException.ThrowIfNull(rom);

        _rom = rom;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"><paramref name="words"/> or <paramref name="voice"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidRomException">A word never ends, so the ROM data must be corrupt.</exception>
    public AudioClip Render(IReadOnlyList<SpeechWord> words, VoiceSettings voice)
    {
        ArgumentNullException.ThrowIfNull(words);
        ArgumentNullException.ThrowIfNull(voice);

        var chip = new S14001AChip(_rom);
        double samplesPerNmi = voice.ChipClockHz / BerzerkTiming.NmiRateHz;
        long sampleLimit = (long)(voice.ChipClockHz * RendererLimits.MaximumSecondsPerWord) * Math.Max(1, words.Count);
        List<float> samples = [];
        double nextNmiSample = 0;
        int nextWord = 0;

        while (true)
        {
            if (samples.Count >= nextNmiSample)
            {
                nextNmiSample += samplesPerNmi;

                if (!chip.IsBusy)
                {
                    if (nextWord == words.Count)
                    {
                        break;
                    }

                    StartWord(chip, words[nextWord]);
                    nextWord++;
                }
            }

            chip.Clock();
            samples.Add(ToSample(chip.Output, voice.Gain));

            if (samples.Count > sampleLimit)
            {
                throw new InvalidRomException(string.Format(
                    CultureInfo.InvariantCulture,
                    "The speech ROM data for word {0} never ends; the ROM image is probably corrupt.",
                    words[nextWord - 1]));
            }
        }

        return new AudioClip((int)Math.Round(voice.ChipClockHz), samples);
    }

    private static void StartWord(S14001AChip chip, SpeechWord word)
    {
        // As berzerk_state::audio_w: latch the word, then clock START via the board's 555 timer.
        chip.SelectWord((int)word);
        chip.SetStart(true);
        chip.SetStart(false);
    }

    private static float ToSample(int output, float gain) =>
        (output - ChipConstants.SilentOutput) / ChipConstants.OutputScale * gain;
}
