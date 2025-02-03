
using System.Diagnostics;
using System.Text;

namespace Toml.Exceptions;

/// <summary>
/// Handles errors thrown during reading or writing a TOML file.
/// </summary>
public class TomlException : Exception
{
    /// <summary>
    /// Base Constructor, avoid using unless necessary
    /// </summary>
    public TomlException() {}

    public TomlException(string message) : base(message) {}
    /// <summary>
    /// Throws an exception with a message
    /// </summary>
    /// <param name="message">What caused the exception to get thrown</param>
    /// <param name="innerException">The Inner <see cref="Exception"/> that was thrown</param>
    public TomlException(string message, Exception innerException) : base(message, innerException) {}

    /// <summary>
    /// Wraps the existing exception in a TomlException automatically building a <see cref="StackTrace"/> message
    /// while preserving the original exception as an inner exception.
    /// </summary>
    /// <param name="ex">The exception thrown</param>
    public TomlException(Exception ex) : base(ex.Message) {}

    /// <summary>
    /// Creates a new TomlException with <see cref="StackTrace"/> <seealso cref="StackFrame"/> information
    /// to know where in the Toml library the exception was thrown from
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> thrown.</param>
    /// <param name="innerException">The inner exception thrown that caused the TomlException to catch and throw.</param>
    public TomlException(Exception ex, Exception innerException) : base(ex.Message, innerException) {}
}
