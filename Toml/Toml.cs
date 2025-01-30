using System.Diagnostics;
using System.Text;

using Toml.Exceptions;
using Toml.Reader;


namespace Toml;

public partial class Toml
{
    public string FilePath { get; }

    public Toml(string filePath)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(filePath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            FilePath = filePath;
        }
        catch(FileNotFoundException ex)
        {
            //Throw a more detailed Stack Trace, but preserve our original stack strace as well
            throw new FileNotFoundException(TomlException.BuildExceptionMessage(ex), ex);
        }
        catch(ArgumentNullException ex)
        {
            throw new ArgumentNullException(TomlException.BuildExceptionMessage(ex), ex);
        }

    }

    public Dictionary<string, object> Read()
    {
        IDictionary<string, object> toml = new Dictionary<string, object>();
        try
        {
            // Open the file granting read access, and FileShare Reading access meaning another application can open the
            // file without getting an error about the file being used by another process
            string[] lines = File.ReadAllLines(FilePath);

            return new Dictionary<string, object>();
        }
        catch(FileNotFoundException ex)
        {
            throw new FileNotFoundException(TomlException.BuildExceptionMessage(ex));
        }
        catch(Exception ex)
        {
            throw new Exception(TomlException.BuildExceptionMessage(ex));
        }
    }
}

//TODO: Implement Asinc Class
public partial class TomlAsync(string filePath) { }
