using System;
using System.IO;
using System.IO.Compression;

namespace BerzerkSound.Tests;

/// <summary>
/// A new, empty directory that is deleted when disposed.
/// </summary>
internal sealed class TemporaryDirectory : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemporaryDirectory"/> class.
    /// </summary>
    public TemporaryDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "BerzerkSound.Tests", Guid.NewGuid().ToString("N"));
        _ = Directory.CreateDirectory(Path);
    }

    /// <summary>Gets the directory's full path.</summary>
    public string Path { get; }

    /// <summary>
    /// Writes a file into the directory.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <param name="contents">The file contents.</param>
    public void WriteFile(string name, byte[] contents) => File.WriteAllBytes(PathOf(name), contents);

    /// <summary>
    /// Writes a zip file into the directory.
    /// </summary>
    /// <param name="name">The zip file name.</param>
    /// <param name="entries">The entry names and contents.</param>
    public void WriteZip(string name, params (string Name, byte[] Contents)[] entries)
    {
        using ZipArchive archive = ZipFile.Open(PathOf(name), ZipArchiveMode.Create);

        foreach ((string entryName, byte[] contents) in entries)
        {
            using Stream stream = archive.CreateEntry(entryName).Open();
            stream.Write(contents);
        }
    }

    /// <summary>
    /// Returns the full path of a file in the directory.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <returns>The path.</returns>
    public string PathOf(string name) => System.IO.Path.Combine(Path, name);

    /// <inheritdoc/>
    public void Dispose() => Directory.Delete(Path, recursive: true);
}
