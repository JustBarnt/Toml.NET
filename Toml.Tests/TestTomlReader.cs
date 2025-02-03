using System.Text;
using Toml;

using Toml.Exceptions;

namespace TomlTests;

public class TestTomlReader : IDisposable
{
    private readonly Dictionary<string, string> file_dictionary = [];

    /// <summary>
    /// Our test fixture to generate bad and good toml files to test with
    /// </summary>
    public TestTomlReader()
    {
        file_dictionary.Add("empty_toml.toml", "");
        file_dictionary.Add("simple_toml.toml", """
            strings = "Hello World"
            boolean = true
            integer = 12
            decimal = 1.2
        """);

        foreach (KeyValuePair<string, string> entry in file_dictionary)
        {
            using (FileStream fs = File.Create(entry.Key))
            {
                byte[] bytes = new UTF8Encoding(true).GetBytes(entry.Value);
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }

    [Fact]
    public void Fails_On_KeyNotFound()
    {
        Assert.Throws<KeyNotFoundException>(() =>
        {
            TomlFile toml = new("simple_toml.toml");
            toml.Read();
            TomlDictionary keys = toml.TomlDictionary;
            return keys["unknown"];
        });
    }

    [Fact]
    public void Fails_On_Empty_Toml() =>
        Assert.Throws<FileNotFoundException>(() => new TomlFile("FileNotReal.toml"));

    [Fact]
    public void Passes_Reads_SimpleTomlFile()
    {
        TomlFile toml = new("simple_toml.toml");
        toml.Read();
        var dict = toml.TomlDictionary;
        Assert.Multiple(
            () => Assert.True(dict.TryGetValue("strings", out _)),
            () => Assert.True(dict.TryGetValue("boolean", out _)),
            () => Assert.True(dict.TryGetValue("integer", out _)),
            () => Assert.True(dict.TryGetValue("decimal", out _))
        );
    }

    public void Dispose()
    {
        //Clean up and remove the toml files so they are not polluting the output
        foreach (KeyValuePair<string, string> entry in file_dictionary)
            File.Delete(entry.Key);

        GC.SuppressFinalize(this);
    }
}
