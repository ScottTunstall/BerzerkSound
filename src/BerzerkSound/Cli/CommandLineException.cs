using System;

namespace BerzerkSound.Cli;

/// <summary>
/// Thrown inside the parser when the command line is invalid; the message is shown to the user.
/// </summary>
internal sealed class CommandLineException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineException"/> class.
    /// </summary>
    public CommandLineException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineException"/> class.
    /// </summary>
    /// <param name="message">What is wrong, phrased for the user.</param>
    public CommandLineException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineException"/> class.
    /// </summary>
    /// <param name="message">What is wrong, phrased for the user.</param>
    /// <param name="innerException">The underlying failure.</param>
    public CommandLineException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
