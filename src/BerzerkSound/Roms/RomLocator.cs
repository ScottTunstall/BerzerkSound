using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BerzerkSound.Roms;

/// <summary>
/// Decides where to load the speech ROMs from.
/// </summary>
internal sealed class RomLocator
{
    private readonly RomSetDefinition _definition;
    private readonly IReadOnlyList<string> _searchDirectories;

    /// <summary>
    /// Initializes a new instance of the <see cref="RomLocator"/> class.
    /// </summary>
    /// <param name="definition">The ROM set to look for.</param>
    /// <param name="searchDirectories">Directories searched, in order, when no ROM path is given.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public RomLocator(RomSetDefinition definition, IReadOnlyList<string> searchDirectories)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(searchDirectories);

        _definition = definition;
        _searchDirectories = searchDirectories;
    }

    /// <summary>
    /// Returns the explicitly requested path, or else the first search directory that holds any of the set's
    /// ROM files or MAME zips.
    /// </summary>
    /// <param name="requestedPath">The path the user asked for, if any.</param>
    /// <returns>The zip file or directory to load the ROMs from.</returns>
    /// <exception cref="RomNotFoundException">No path was requested and no search directory holds the ROMs.</exception>
    public string Locate(string? requestedPath)
    {
        if (requestedPath is not null)
        {
            return requestedPath;
        }

        IEnumerable<string> candidateNames = _definition.Parts.SelectMany(part => part.FileNames).Concat(_definition.ArchiveNames);

        return _searchDirectories.FirstOrDefault(directory => candidateNames.Any(name => File.Exists(Path.Combine(directory, name))))
            ?? throw new RomNotFoundException(
                $"The {_definition.Description} were not found. Copy MAME's {string.Join(" or ", _definition.ArchiveNames)} "
                + "next to the application, or pass its location with --rom. Searched: "
                + string.Join(", ", _searchDirectories.Distinct(StringComparer.OrdinalIgnoreCase)));
    }
}
