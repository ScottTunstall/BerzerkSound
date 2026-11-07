using System;
using System.IO;
using System.IO.Compression;
using System.IO.Hashing;
using System.Linq;
using BerzerkSound.Synthesis;

namespace BerzerkSound.Roms;

/// <summary>
/// Loads a speech ROM set from a directory of ROM files or a MAME zip, checking every chip's CRC-32.
/// </summary>
internal sealed class SpeechRomLoader
{
    private readonly RomSetDefinition _definition;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechRomLoader"/> class.
    /// </summary>
    /// <param name="definition">The ROM set to load.</param>
    /// <exception cref="ArgumentNullException"><paramref name="definition"/> is <see langword="null"/>.</exception>
    public SpeechRomLoader(RomSetDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        _definition = definition;
    }

    /// <summary>
    /// Loads the ROM set from <paramref name="path"/>: a MAME zip, or a directory holding either the ROM
    /// files themselves or one of the set's MAME zips.
    /// </summary>
    /// <param name="path">The zip file or directory.</param>
    /// <returns>The speech ROM.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="RomNotFoundException"><paramref name="path"/> does not exist or does not hold the set.</exception>
    /// <exception cref="InvalidRomException">A file cannot be read, or is not a good dump of its chip.</exception>
    public SpeechRom Load(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (Directory.Exists(path))
        {
            return LoadFromDirectory(path);
        }

        if (File.Exists(path))
        {
            return LoadFromArchive(path);
        }

        throw new RomNotFoundException($"'{path}' does not exist. Give the directory or MAME zip that holds the {_definition.Description}.");
    }

    private SpeechRom LoadFromDirectory(string directory)
    {
        string?[] looseFiles = [.. _definition.Parts.Select(part => FindLooseFile(directory, part))];

        if (looseFiles.All(file => file is not null))
        {
            byte[] image = new byte[ChipConstants.RomSize];

            for (int index = 0; index < looseFiles.Length; index++)
            {
                string file = looseFiles[index]!;
                Place(image, _definition.Parts[index], ReadFile(file), file);
            }

            return new SpeechRom(image);
        }

        string? archive = _definition.ArchiveNames
            .Select(name => Path.Combine(directory, name))
            .FirstOrDefault(File.Exists);

        return archive is null
            ? throw new RomNotFoundException(
                $"The {_definition.Description} were not found in '{directory}'. It needs {DescribeLooseFiles()}, "
                + $"or {string.Join(" or ", _definition.ArchiveNames)}.")
            : LoadFromArchive(archive);
    }

    private SpeechRom LoadFromArchive(string path)
    {
        try
        {
            using ZipArchive archive = ZipFile.OpenRead(path);
            byte[] image = new byte[ChipConstants.RomSize];

            foreach (RomPart part in _definition.Parts)
            {
                ZipArchiveEntry entry = FindEntry(archive, part)
                    ?? throw new RomNotFoundException(
                        $"'{path}' does not contain the {part.Socket} speech ROM ({string.Join(" or ", part.FileNames)}, CRC32 {part.Crc32:x8}).");

                Place(image, part, ReadEntry(entry), $"{path}{Path.DirectorySeparatorChar}{entry.FullName}");
            }

            return new SpeechRom(image);
        }
        catch (InvalidDataException exception)
        {
            throw new InvalidRomException($"'{path}' is not a valid zip file: {exception.Message}", exception);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new InvalidRomException($"'{path}' could not be read: {exception.Message}", exception);
        }
    }

    private static string? FindLooseFile(string directory, RomPart part) =>
        part.FileNames.Select(name => Path.Combine(directory, name)).FirstOrDefault(File.Exists);

    private static ZipArchiveEntry? FindEntry(ZipArchive archive, RomPart part) =>
        archive.Entries.FirstOrDefault(entry => entry.Crc32 == part.Crc32 && entry.Length == part.Size)
        ?? archive.Entries.FirstOrDefault(entry => part.FileNames.Contains(entry.Name, StringComparer.OrdinalIgnoreCase));

    private static byte[] ReadFile(string path)
    {
        try
        {
            return File.ReadAllBytes(path);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new InvalidRomException($"The speech ROM '{path}' could not be read: {exception.Message}", exception);
        }
    }

    private static byte[] ReadEntry(ZipArchiveEntry entry)
    {
        using Stream stream = entry.Open();
        using var buffer = new MemoryStream();

        stream.CopyTo(buffer);
        return buffer.ToArray();
    }

    private void Place(byte[] image, RomPart part, byte[] data, string source)
    {
        if (data.Length != part.Size)
        {
            throw new InvalidRomException(
                $"'{source}' is {data.Length} bytes, but the {part.Socket} speech ROM is {part.Size} bytes.");
        }

        uint crc = Crc32.HashToUInt32(data);

        if (crc != part.Crc32)
        {
            throw new InvalidRomException(
                $"'{source}' is not a good dump of the {part.Socket} chip of the {_definition.Description} "
                + $"(CRC32 {crc:x8}, expected {part.Crc32:x8}). Only the English Berzerk/Frenzy speech ROMs are supported.");
        }

        data.CopyTo(image, part.Offset);
    }

    private string DescribeLooseFiles() =>
        string.Join(" and ", _definition.Parts.Select(part => string.Join(" or ", part.FileNames)));
}
