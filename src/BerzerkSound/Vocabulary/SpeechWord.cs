namespace BerzerkSound.Vocabulary;

/// <summary>
/// The words in the English Berzerk speech ROMs, valued by their S14001A word number.
/// </summary>
/// <remarks>
/// Numbers 0x01 to 0x1C match the word equates in the Berzerk disassembly
/// (https://github.com/ScottTunstall/Berzerk/blob/main/berzerk.asm). Word 0x00 is not used by the game; it
/// was identified from the ROM data in the "TSI S14001A Speech Synthesizer LSI Integrated Circuit Guide".
/// The ROM's word table ends at 0x1C.
/// </remarks>
internal enum SpeechWord
{
    /// <summary>"help" (not used by the game).</summary>
    Help = 0x00,

    /// <summary>"kill".</summary>
    Kill = 0x01,

    /// <summary>"attack".</summary>
    Attack = 0x02,

    /// <summary>"charge".</summary>
    Charge = 0x03,

    /// <summary>"got".</summary>
    Got = 0x04,

    /// <summary>"shoot".</summary>
    Shoot = 0x05,

    /// <summary>"get".</summary>
    Get = 0x06,

    /// <summary>"is".</summary>
    Is = 0x07,

    /// <summary>"alert".</summary>
    Alert = 0x08,

    /// <summary>"detected".</summary>
    Detected = 0x09,

    /// <summary>"the".</summary>
    The = 0x0A,

    /// <summary>"in".</summary>
    In = 0x0B,

    /// <summary>"it".</summary>
    It = 0x0C,

    /// <summary>"there" (also "their").</summary>
    There = 0x0D,

    /// <summary>"where".</summary>
    Where = 0x0E,

    /// <summary>"humanoid".</summary>
    Humanoid = 0x0F,

    /// <summary>"coins".</summary>
    Coins = 0x10,

    /// <summary>"pocket".</summary>
    Pocket = 0x11,

    /// <summary>"intruder".</summary>
    Intruder = 0x12,

    /// <summary>"no".</summary>
    No = 0x13,

    /// <summary>"escape".</summary>
    Escape = 0x14,

    /// <summary>"destroy".</summary>
    Destroy = 0x15,

    /// <summary>"must".</summary>
    Must = 0x16,

    /// <summary>"not".</summary>
    Not = 0x17,

    /// <summary>"chicken".</summary>
    Chicken = 0x18,

    /// <summary>"fight".</summary>
    Fight = 0x19,

    /// <summary>"like".</summary>
    Like = 0x1A,

    /// <summary>"a".</summary>
    A = 0x1B,

    /// <summary>"robot".</summary>
    Robot = 0x1C,
}
