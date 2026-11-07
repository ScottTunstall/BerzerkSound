using System;

namespace BerzerkSound.Audio;

/// <summary>
/// The <c>fdwSound</c> flags of the Win32 <c>PlaySound</c> function that this app uses.
/// </summary>
[Flags]
internal enum PlaySoundOptions
{
    /// <summary>SND_SYNC: play synchronously, returning when the sound has finished.</summary>
    None = 0,

    /// <summary>SND_NODEFAULT: stay silent rather than play the default sound if the sound cannot be played.</summary>
    NoDefault = 1 << 1,

    /// <summary>SND_MEMORY: the sound argument points to a WAV file image in memory.</summary>
    Memory = 1 << 2,
}
