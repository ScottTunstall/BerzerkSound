// license:BSD-3-Clause
// copyright-holders:Zsolt Vasvari, Aaron Giles, R. Belmont, Jonathan Gevaryahu
//
// ROM names, sizes and CRCs from the "speech" region of MAME src/mame/stern/berzerk.cpp.

namespace BerzerkSound.Roms;

/// <summary>
/// The speech ROM sets this app can use.
/// </summary>
internal static class KnownRomSets
{
    /// <summary>The size of each speech ROM chip on the VSU-1000 board.</summary>
    private const int ChipSize = 0x800;

    /// <summary>
    /// Gets the English Berzerk speech ROMs (VSU-1000 board sockets 1C and 2C). Berzerk revisions RC28, RC31
    /// and RC31A and Frenzy all use the same data; MAME names Frenzy's copies e169-1cvo.1c and e169-2cvo.2c.
    /// </summary>
    public static RomSetDefinition EnglishBerzerk { get; } = new(
        "Berzerk English speech ROMs",
        [
            new("1C", 0x0000, ChipSize, 0x2CFE825D, ["berzerk_r_vo_1c.1c", "e169-1cvo.1c"]),
            new("2C", ChipSize, ChipSize, 0xD2B6324E, ["berzerk_r_vo_2c.2c", "e169-2cvo.2c"]),
        ],
        ["berzerk.zip", "frenzy.zip"]);
}
