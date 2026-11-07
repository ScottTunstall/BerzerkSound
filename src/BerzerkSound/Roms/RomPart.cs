using System.Collections.Generic;

namespace BerzerkSound.Roms;

/// <summary>
/// One ROM chip of a speech ROM set.
/// </summary>
/// <param name="Socket">The board socket the chip sits in, for example "1C".</param>
/// <param name="Offset">Where the chip's data starts in the 4 KB speech address space.</param>
/// <param name="Size">The chip's size in bytes.</param>
/// <param name="Crc32">The CRC-32 of a good dump, as MAME lists it.</param>
/// <param name="FileNames">The file names MAME uses for the dump.</param>
internal sealed record RomPart(string Socket, int Offset, int Size, uint Crc32, IReadOnlyList<string> FileNames);
