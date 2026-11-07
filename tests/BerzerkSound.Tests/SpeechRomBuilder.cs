using BerzerkSound.Synthesis;

namespace BerzerkSound.Tests;

/// <summary>
/// Builds synthetic S14001A speech ROMs laid out the way the chip reads them.
/// </summary>
internal sealed class SpeechRomBuilder
{
    /// <summary>Control byte of an unvoiced syllable that lasts four quarter pitch periods.</summary>
    public const byte ShortSyllable = 0x1F;

    /// <summary>Control bit marking the last syllable of a word.</summary>
    public const byte Stop = 0x80;

    /// <summary>Control bit marking a voiced syllable.</summary>
    public const byte Voiced = 0x40;

    /// <summary>Control bit marking a silent syllable.</summary>
    public const byte Silence = 0x20;

    private readonly byte[] _image = new byte[ChipConstants.RomSize];

    /// <summary>
    /// Points a word table entry at a syllable list.
    /// </summary>
    /// <param name="word">The word number.</param>
    /// <param name="syllableAddress">The address of the word's first syllable entry.</param>
    public void SetWord(int word, int syllableAddress)
    {
        _image[word * 2] = (byte)(syllableAddress >> 4);
        _image[(word * 2) + 1] = (byte)((syllableAddress & 0x0F) << 4);
    }

    /// <summary>
    /// Writes a two-byte syllable entry.
    /// </summary>
    /// <param name="address">Where the entry goes.</param>
    /// <param name="deltaDataAddress">Where the syllable's delta data starts; a multiple of 16.</param>
    /// <param name="controlBits">The control byte: stop, voiced, silence, length and repeat.</param>
    public void SetSyllable(int address, int deltaDataAddress, byte controlBits)
    {
        _image[address] = (byte)(deltaDataAddress >> 4);
        _image[address + 1] = controlBits;
    }

    /// <summary>
    /// Fills a range of the ROM with one value.
    /// </summary>
    /// <param name="address">The first address.</param>
    /// <param name="length">How many bytes to fill.</param>
    /// <param name="value">The value.</param>
    public void Fill(int address, int length, byte value)
    {
        for (int offset = 0; offset < length; offset++)
        {
            _image[address + offset] = value;
        }
    }

    /// <summary>
    /// Returns the ROM built so far.
    /// </summary>
    /// <returns>The ROM.</returns>
    public SpeechRom Build() => new(_image);
}
