using System.Diagnostics;
using System.Text;

using Toml.Exceptions;
using Toml.Reader;

using TomlDictionary = System.Collections.Generic.IDictionary<string, object>;

namespace Toml;

public partial class Toml
{
    public TomlDictionary TomlDictionary { get; private set; }
    private string path { get; }

    public Toml(string filePath)
    {
        try
        {
            if (!File.Exists(path)) throw new FileNotFoundException("File not found", path);
            path = filePath;
        }
        catch(FileNotFoundException ex)
        {
            throw new TomlException(ex);
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
            string[] lines = File.ReadAllLines(path);

            TomlReader reader = new (lines);
            TomlDictionary = reader.TomlDictionary;
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
