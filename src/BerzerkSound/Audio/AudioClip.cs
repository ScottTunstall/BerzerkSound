using System;
using System.Collections.Generic;

namespace BerzerkSound.Audio;

/// <summary>
/// A mono recording: samples nominally in the range -1 to 1, at a fixed rate.
/// </summary>
internal sealed class AudioClip
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioClip"/> class.
    /// </summary>
    /// <param name="sampleRateHz">Samples per second.</param>
    /// <param name="samples">The samples, which are copied.</param>
    public AudioClip(int sampleRateHz, IEnumerable<float> samples)
    {
        if (sampleRateHz <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sampleRateHz), sampleRateHz, "The sample rate must be positive.");
        }

        SampleRateHz = sampleRateHz;
        Samples = [.. samples];
    }

    /// <summary>Gets the number of samples per second.</summary>
    public int SampleRateHz { get; }

    /// <summary>Gets the samples.</summary>
    public IReadOnlyList<float> Samples { get; }
}
