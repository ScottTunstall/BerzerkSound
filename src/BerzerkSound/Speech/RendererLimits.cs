namespace BerzerkSound.Speech;

/// <summary>
/// Safety limits of <see cref="BerzerkSpeechRenderer"/>.
/// </summary>
internal static class RendererLimits
{
    /// <summary>
    /// The longest a single word may last. Real Berzerk words last well under a second; a word that runs
    /// longer means the ROM data never reaches a stop bit.
    /// </summary>
    public const double MaximumSecondsPerWord = 10;
}
