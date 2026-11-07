using System.Collections.Generic;
using BerzerkSound.Synthesis;

namespace BerzerkSound.Tests;

/// <summary>
/// The outputs of a chip from START until it stopped being busy.
/// </summary>
internal sealed class ChipRun
{
    private const int ClockLimit = 1_000_000;

    private ChipRun(IReadOnlyList<int> outputs, bool finished)
    {
        Outputs = outputs;
        Finished = finished;
    }

    /// <summary>Gets the output level after every clock.</summary>
    public IReadOnlyList<int> Outputs { get; }

    /// <summary>Gets a value indicating whether the chip stopped being busy within the clock limit.</summary>
    public bool Finished { get; }

    /// <summary>
    /// Says <paramref name="word"/> on a new chip and records every output until the chip is idle again.
    /// </summary>
    /// <param name="rom">The speech ROM.</param>
    /// <param name="word">The word number.</param>
    /// <returns>The run.</returns>
    public static ChipRun Say(SpeechRom rom, int word)
    {
        var chip = new S14001AChip(rom);
        List<int> outputs = [];
        bool wasBusy = false;

        chip.SelectWord(word);
        chip.SetStart(true);
        chip.SetStart(false);

        while (outputs.Count < ClockLimit)
        {
            chip.Clock();
            outputs.Add(chip.Output);
            wasBusy |= chip.IsBusy;

            if (wasBusy && !chip.IsBusy)
            {
                return new ChipRun(outputs, finished: true);
            }
        }

        return new ChipRun(outputs, finished: false);
    }
}
