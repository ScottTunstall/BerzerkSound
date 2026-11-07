using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace BerzerkSound.Audio;

/// <summary>
/// Win32 functions used for audio playback.
/// </summary>
[SupportedOSPlatform("windows")]
internal static partial class NativeMethods
{
    /// <summary>
    /// Plays a sound (winmm.dll <c>PlaySoundW</c>).
    /// </summary>
    /// <param name="sound">With <see cref="PlaySoundOptions.Memory"/>, a complete WAV file image.</param>
    /// <param name="module">Unused; pass <see cref="nint.Zero"/>.</param>
    /// <param name="options">How to play the sound.</param>
    /// <returns><see langword="true"/> if the sound was played.</returns>
    [LibraryImport("winmm.dll", EntryPoint = "PlaySoundW", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PlaySound(byte[] sound, nint module, PlaySoundOptions options);
}
