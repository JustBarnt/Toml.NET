
using System.Diagnostics;
using System.Text;

namespace Toml.Exceptions;

/// <summary>
/// Handles errors thrown during reading or writing a TOML file.
/// </summary>
public class TomlException : Exception
{
    /// <summary>
    /// Gets the path to the TOML file that caused the error
    /// </summary>
    private string? file_path { get; }

    public TomlException() {}
    public TomlException(string message) : base(message) {}

    public TomlException(string message, string filePath) : base(message) => file_path = filePath;

    public TomlException(string message, Exception innerException) : base(message, innerException) {}
    public TomlException(string message, string filePath, Exception innerException) : base(message, innerException) => file_path = filePath;

    public override string ToString() => !string.IsNullOrEmpty(file_path) ? $"{base.ToString()}\nFile Path: {file_path}" : base.ToString();

    public static string ExceptionBuilder<TException>(TException exception) where TException : Exception
        {
            StringBuilder sb = new();
            StackTrace st = new(exception, true);
            StackFrame? frame = st.GetFrame(0);

            if (frame is null) return exception.Message;

            sb.AppendLine($"Exception in File - {frame.GetFileName()}");
            sb.AppendLine($"In Method - {frame.GetMethod()}");
            sb.AppendLine($"On Line {frame.GetFileLineNumber()} at Position {frame.GetFileColumnNumber()}");

            return sb.ToString();
        }
}
