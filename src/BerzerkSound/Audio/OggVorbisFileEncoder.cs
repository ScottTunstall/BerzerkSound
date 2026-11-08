using System;
using System.IO;
using OggVorbisEncoder;

namespace BerzerkSound.Audio;

/// <summary>
/// Encodes audio as a mono Ogg Vorbis file.
/// </summary>
internal static class OggVorbisFileEncoder
{
    /// <summary>
    /// Returns the bytes of an Ogg Vorbis file holding <paramref name="clip"/>. Samples outside -1 to 1 are clipped.
    /// </summary>
    /// <param name="clip">The audio to encode.</param>
    /// <returns>The complete file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="clip"/> is <see langword="null"/>.</exception>
    public static byte[] Encode(AudioClip clip)
    {
        ArgumentNullException.ThrowIfNull(clip);

        var info = VorbisInfo.InitVariableBitRate(OggVorbisFormat.ChannelCount, clip.SampleRateHz, OggVorbisFormat.Quality);
        var stream = new OggStream(OggVorbisFormat.StreamSerial);
        using var output = new MemoryStream();

        WriteHeaders(info, stream, output);
        WriteSamples(clip, info, stream, output);

        return output.ToArray();
    }

    private static void WriteHeaders(VorbisInfo info, OggStream stream, MemoryStream output)
    {
        stream.PacketIn(HeaderPacketBuilder.BuildInfoPacket(info));
        stream.PacketIn(HeaderPacketBuilder.BuildCommentsPacket(new Comments()));
        stream.PacketIn(HeaderPacketBuilder.BuildBooksPacket(info));

        // The headers must end a page of their own.
        WritePages(stream, output, force: true);
    }

    private static void WriteSamples(AudioClip clip, VorbisInfo info, OggStream stream, MemoryStream output)
    {
        var state = ProcessingState.Create(info);
        float[][] block = [new float[OggVorbisFormat.BlockSamples]];
        int totalSamples = OggVorbisFormat.LeadInSamples + clip.Samples.Count + OggVorbisFormat.TailPaddingSamples;

        for (int start = 0; start < totalSamples; start += OggVorbisFormat.BlockSamples)
        {
            int count = Math.Min(OggVorbisFormat.BlockSamples, totalSamples - start);

            for (int index = 0; index < count; index++)
            {
                int position = start + index - OggVorbisFormat.LeadInSamples;
                bool isSpeech = position >= 0 && position < clip.Samples.Count;
                block[0][index] = isSpeech ? Math.Clamp(clip.Samples[position], -1f, 1f) : 0f;
            }

            state.WriteData(block, count);
            ReadPackets(state, stream, output);
        }

        state.WriteEndOfStream();
        ReadPackets(state, stream, output);
        WritePages(stream, output, force: true);
    }

    private static void ReadPackets(ProcessingState state, OggStream stream, MemoryStream output)
    {
        while (!stream.Finished && state.PacketOut(out OggPacket packet))
        {
            stream.PacketIn(packet);
            WritePages(stream, output, force: false);
        }
    }

    private static void WritePages(OggStream stream, MemoryStream output, bool force)
    {
        while (stream.PageOut(out OggPage page, force))
        {
            output.Write(page.Header);
            output.Write(page.Body);
        }
    }
}
