using System.Collections.Generic;

namespace BerzerkSound.Roms;

/// <summary>
/// A set of ROM chips that together fill the S14001A's 4 KB speech address space.
/// </summary>
/// <param name="Description">What the set is, for messages, for example "Berzerk English speech ROMs".</param>
/// <param name="Parts">The chips.</param>
/// <param name="ArchiveNames">The names of MAME zip files that contain the set.</param>
internal sealed record RomSetDefinition(string Description, IReadOnlyList<RomPart> Parts, IReadOnlyList<string> ArchiveNames);
