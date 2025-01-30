
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

    /// <summary>
    /// Throws an exception with a message
    /// </summary>
    /// <param name="message">What caused the exception to get thrown</param>
    public TomlException(string message) : base(message) {}

    /// <summary>
    /// Wraps the existing exception in a TomlException automatically building a <see cref="StackTrace"/> message
    /// while preserving the original exception as an inner exception.
    /// </summary>
    /// <param name="ex">The exception thrown</param>
    public TomlException(Exception ex) : base(BuildExceptionMessage(ex), ex) {}

    /// <summary>
    /// Creates a new TomlException with <see cref="StackTrace"/> <seealso cref="StackFrame"/> information
    /// to know where in the Toml library the exception was thrown from
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> thrown.</param>
    /// <param name="innerException">The inner exception thrown that caused the TomlException to catch and throw.</param>
    public TomlException(Exception ex, Exception innerException) : base(BuildExceptionMessage(ex), innerException) {}


    /// <summary>
    /// Creates a detailed oriented <see cref="StackTrace"/> message
    /// so we know which file, method, and line in our library the error occured at.
    /// </summary>
    /// <param name="exception">The exception to pull <see cref="StackTrace"/> information out of <seealso cref="StackFrame"/></param>
    /// <returns></returns>
    private static string BuildExceptionMessage(Exception exception)
        {
            StringBuilder sb = new();
            StackTrace st = new(exception, true);
            StackFrame? frame = st.GetFrame(0);

            if (frame is null) return exception.Message;

            sb.AppendLine($"Exception in File - {frame.GetFileName()}");
            sb.AppendLine($"In Method - {frame.GetMethod()}");
            sb.AppendLine($"On Line {frame.GetFileLineNumber()} at Position {frame.GetFileColumnNumber()}");
            sb.AppendLine($"Exception Message - {exception.Message}");

            return sb.ToString();
        }
}
