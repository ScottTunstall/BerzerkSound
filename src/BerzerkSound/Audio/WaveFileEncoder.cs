using System;
using System.IO;
using System.Text;

namespace BerzerkSound.Audio;

/// <summary>
/// Encodes audio as a 16-bit mono PCM RIFF WAVE file.
/// </summary>
internal static class WaveFileEncoder
{
    /// <summary>
    /// Returns the bytes of a WAV file holding <paramref name="clip"/>. Samples outside -1 to 1 are clipped.
    /// </summary>
    /// <param name="clip">The audio to encode.</param>
    /// <returns>The complete WAV file.</returns>
    public static byte[] Encode(AudioClip clip)
    {
        int dataSize = clip.Samples.Count * WaveFormat.BytesPerSample;

        using var stream = new MemoryStream(WaveFormat.RiffPreambleSize + WaveFormat.HeaderBytesAfterRiffSize + dataSize);

        using (var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true))
        {
            writer.Write(Encoding.ASCII.GetBytes(WaveFormat.RiffTag));
            writer.Write(WaveFormat.HeaderBytesAfterRiffSize + dataSize);
            writer.Write(Encoding.ASCII.GetBytes(WaveFormat.WaveTag));

            writer.Write(Encoding.ASCII.GetBytes(WaveFormat.FormatChunkTag));
            writer.Write(WaveFormat.FormatChunkSize);
            writer.Write(WaveFormat.PcmFormat);
            writer.Write(WaveFormat.ChannelCount);
            writer.Write(clip.SampleRateHz);
            writer.Write(clip.SampleRateHz * WaveFormat.ChannelCount * WaveFormat.BytesPerSample);
            writer.Write((short)(WaveFormat.ChannelCount * WaveFormat.BytesPerSample));
            writer.Write(WaveFormat.BitsPerSample);

            writer.Write(Encoding.ASCII.GetBytes(WaveFormat.DataChunkTag));
            writer.Write(dataSize);

            foreach (float sample in clip.Samples)
            {
                writer.Write(ToPcm16(sample));
            }
        }

        return stream.ToArray();
    }

    private static short ToPcm16(float sample)
    {
        float clipped = Math.Max(-1f, Math.Min(1f, sample));
        return (short)Math.Round(clipped * WaveFormat.FullScale);
    }
}
