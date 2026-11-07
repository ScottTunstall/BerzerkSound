using System;
using System.Collections.Generic;

namespace BerzerkSound.Synthesis;

/// <summary>
/// The 4 KB of speech data the S14001A reads: the word table, syllable table and delta-modulated samples.
/// </summary>
internal sealed class SpeechRom
{
    private readonly byte[] _image;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechRom"/> class.
    /// </summary>
    /// <param name="image">The ROM contents, exactly <see cref="ChipConstants.RomSize"/> bytes, which are copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="image"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="image"/> is not 4 KB.</exception>
    public SpeechRom(IReadOnlyCollection<byte> image)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (image.Count != ChipConstants.RomSize)
        {
            throw new ArgumentException(
                $"A speech ROM image must be {ChipConstants.RomSize} bytes, but this one is {image.Count}.",
                nameof(image));
        }

        _image = [.. image];
    }

    /// <summary>
    /// Returns the byte at <paramref name="address"/>.
    /// </summary>
    /// <param name="address">The address, 0 to 4095.</param>
    /// <returns>The byte.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="address"/> is outside the ROM.</exception>
    public byte Read(int address)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(address);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(address, ChipConstants.RomSize);

        return _image[address];
    }
}
