using System;
using System.IO;
using System.Linq;
using System.Text;
using BerzerkSound.Audio;
using NVorbis;
using Xunit;

namespace BerzerkSound.Tests;

public sealed class OggVorbisFileEncoderTests
{
    private const int SampleRate = 24_038;
    private const int SampleCount = 12_000;
    private const int MaximumDelay = 3_000;

    [Fact]
    public void WritesAnOggContainer()
    {
        byte[] file = OggVorbisFileEncoder.Encode(Sweep());

        Assert.Equal("OggS", Encoding.ASCII.GetString(file, 0, 4));
    }

    [Fact]
    public void DecodesToTheSameFormat()
    {
        using var reader = Decode(OggVorbisFileEncoder.Encode(Sweep()));

        Assert.Equal(SampleRate, reader.SampleRate);
        Assert.Equal(1, reader.Channels);
    }

    [Fact]
    public void ReproducesTheSignal()
    {
        float[] decoded = ReadAll(OggVorbisFileEncoder.Encode(Sweep()));
        float[] original = [.. Sweep().Samples];

        (int delay, double error) = FindDelay(original, decoded);

        Assert.True(error < 0.02, $"Mean absolute error was {error} at a delay of {delay}.");
    }

    [Fact]
    public void DoesNotCutOffTheStartOfTheSpeech()
    {
        float[] decoded = ReadAll(OggVorbisFileEncoder.Encode(Sweep()));

        (int delay, _) = FindDelay([.. Sweep().Samples], decoded);

        Assert.True(delay >= 0, $"The first {-delay} samples of the speech are missing from the decoded file.");
    }

    [Fact]
    public void DoesNotCutOffTheEndOfTheSpeech()
    {
        float[] decoded = ReadAll(OggVorbisFileEncoder.Encode(Sweep()));

        (int delay, _) = FindDelay([.. Sweep().Samples], decoded);

        Assert.True(decoded.Length - delay >= SampleCount, $"The decoded file ends {SampleCount - (decoded.Length - delay)} samples early.");
    }

    [Fact]
    public void ClipsSamplesOutsideTheValidRange()
    {
        float[] decoded = ReadAll(OggVorbisFileEncoder.Encode(new AudioClip(SampleRate, Enumerable.Repeat(5f, SampleCount))));

        Assert.True(decoded.Max() <= 1.05f);
    }

    [Fact]
    public void EncodesAnEmptyClipAsSilence()
    {
        float[] decoded = ReadAll(OggVorbisFileEncoder.Encode(new AudioClip(SampleRate, [])));

        Assert.All(decoded, sample => Assert.InRange(sample, -0.001f, 0.001f));
    }

    [Fact]
    public void GivesTheSameFileForTheSameSpeech() =>
        Assert.Equal(OggVorbisFileEncoder.Encode(Sweep()), OggVorbisFileEncoder.Encode(Sweep()));

    [Fact]
    public void RejectsANullClip() => Assert.Throws<ArgumentNullException>(() => OggVorbisFileEncoder.Encode(null!));

    /// <summary>
    /// A frequency sweep, which, unlike a steady tone, only lines up with itself at one delay.
    /// </summary>
    /// <returns>The clip.</returns>
    private static AudioClip Sweep() =>
        new(SampleRate, Enumerable.Range(0, SampleCount).Select(index =>
            0.5f * (float)Math.Sin(2 * Math.PI * ((200.0 * index) + (0.5 * 2800.0 * index * index / SampleCount)) / SampleRate)));

    /// <summary>
    /// Finds how many samples later than in the original the same signal appears in the decoded file:
    /// positive if the file begins with extra silence, negative if its start is missing.
    /// </summary>
    /// <param name="original">The samples that were encoded.</param>
    /// <param name="decoded">The samples decoded from the file.</param>
    /// <returns>The delay and the mean absolute error at that delay.</returns>
    private static (int Delay, double Error) FindDelay(float[] original, float[] decoded)
    {
        int bestDelay = 0;
        double bestError = double.MaxValue;

        for (int delay = -MaximumDelay; delay <= MaximumDelay; delay++)
        {
            double total = 0;
            int count = 0;

            for (int index = 3_000; index < 9_000; index += 3)
            {
                int position = index + delay;

                if (position >= 0 && position < decoded.Length)
                {
                    total += Math.Abs(decoded[position] - original[index]);
                    count++;
                }
            }

            if (count > 0 && total / count < bestError)
            {
                bestError = total / count;
                bestDelay = delay;
            }
        }

        return (bestDelay, bestError);
    }

    private static VorbisReader Decode(byte[] file) => new(new MemoryStream(file), true);

    private static float[] ReadAll(byte[] file)
    {
        using var reader = Decode(file);
        float[] samples = new float[reader.TotalSamples];
        int total = 0;

        while (total < samples.Length)
        {
            int read = reader.ReadSamples(samples, total, samples.Length - total);

            if (read == 0)
            {
                break;
            }

            total += read;
        }

        return samples[..total];
    }
}
