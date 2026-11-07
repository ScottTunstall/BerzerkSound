using System;

namespace BerzerkSound.Roms;

/// <summary>
/// Thrown when the speech ROMs cannot be found where they were looked for.
/// </summary>
internal sealed class RomNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RomNotFoundException"/> class.
    /// </summary>
    public RomNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RomNotFoundException"/> class.
    /// </summary>
    /// <param name="message">Where the ROMs were looked for.</param>
    public RomNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RomNotFoundException"/> class.
    /// </summary>
    /// <param name="message">Where the ROMs were looked for.</param>
    /// <param name="innerException">The underlying failure.</param>
    public RomNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
