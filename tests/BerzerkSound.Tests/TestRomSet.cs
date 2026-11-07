using System.IO.Hashing;
using System.Linq;
using BerzerkSound.Roms;

namespace BerzerkSound.Tests;

/// <summary>
/// A synthetic two-chip ROM set, so the loader can be tested without copyrighted ROM images.
/// </summary>
internal static class TestRomSet
{
    /// <summary>The size of each chip.</summary>
    public const int ChipSize = 0x800;

    /// <summary>The 1C chip's file name.</summary>
    public const string FirstFileName = "test_1c.1c";

    /// <summary>The 2C chip's file name.</summary>
    public const string SecondFileName = "test_2c.2c";

    /// <summary>The set's zip name.</summary>
    public const string ArchiveName = "test.zip";

    /// <summary>Gets the contents of the 1C chip.</summary>
    public static byte[] FirstChip { get; } = [.. Enumerable.Repeat((byte)0x11, ChipSize)];

    /// <summary>Gets the contents of the 2C chip.</summary>
    public static byte[] SecondChip { get; } = [.. Enumerable.Repeat((byte)0x22, ChipSize)];

    /// <summary>Gets the definition of the set.</summary>
    public static RomSetDefinition Definition { get; } = new(
        "test speech ROMs",
        [
            new("1C", 0x0000, ChipSize, Crc32.HashToUInt32(FirstChip), [FirstFileName]),
            new("2C", ChipSize, ChipSize, Crc32.HashToUInt32(SecondChip), [SecondFileName]),
        ],
        [ArchiveName]);
}
