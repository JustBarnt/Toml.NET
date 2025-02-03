using System.Collections;

using Toml.Exceptions;
using Toml.Reader;

namespace Toml;

public class TomlFile
{
    public TomlDictionary TomlDictionary { get; private set; } = new();
    private string[] stored_lines { get; }

    public TomlFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found", filePath);
            stored_lines = File.ReadAllLines(filePath);
        }
        catch(FileNotFoundException ex)
        {
            throw new FileNotFoundException("File not found", ex);
        }
        catch(Exception ex)
        {
            throw new TomlException("An unknown error occured.", ex);
        }
    }

    /// <summary>
    /// Reads the contents of the Toml file adds to the <see cref="TomlDictionary"/>
    /// </summary>
    /// <exception cref="TomlException"></exception>
    public void Read()
    {
        try
        {
            // Open the file granting read access, and FileShare Reading access meaning another application can open the
            // file without getting an error about the file being used by another process
            TomlReader reader = new(stored_lines);
            TomlDictionary = reader.ToDictionary();
        }
        catch(KeyNotFoundException ex)
        {
            throw new TomlException(ex);
        }
        catch(Exception ex)
        {
            throw new TomlException(ex);
        }
    }

    //TODO: Implement Write()
    public void Write() => throw new NotImplementedException();

    //TODO: Implement validation
    public bool ValidateToml() => throw new NotImplementedException();
}

//TODO: Implement Asinc Class
public partial class TomlAsync(string filePath) { }
