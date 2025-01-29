
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
}
