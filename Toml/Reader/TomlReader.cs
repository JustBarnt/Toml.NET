namespace Toml.Reader;

/// <summary>
/// The class responsible for reading the Toml file given into an <c>IDictionary&lt;string,object&gt;</c>
/// </summary>
/// <param name="lines">A array of lines from a given file</param>
public class TomlReader(string[] lines)
{
    private string[] raw_lines = lines;

    public IDictionary<string, object> TomlDictionary { get; } = new Dictionary<string, object>();

    public static void ReadToDictionary(string[] lines)
    {

    }
}
