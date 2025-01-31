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
            isTrue = true
            obj = {foo = "bar", fizz = "buzz"}
            strArray = ["one", "two", "three", "four"]
            numArray = [1,2,3,4,5,6,7.5,3.14568,9123123]
            [table1]
            DayOfWeek = "Monday"
            HourOfDay = 12
            MinuteOfHour = 0
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


    private static string GenerateToml(string[] lines)
    {
        StringBuilder sb = new();
        foreach (string line in lines)
            sb.AppendLine(line);
        return sb.ToString();
    }

    [Fact]
    public void Fails_With_Empty_Toml()
    {
        string test_data = GenerateToml([]);
        Assert.Throws<TomlException>(() => new TomlFile(test_data));
    }

    [Fact]
    public void Fails_With_Bad_TomlPath() =>
        Assert.Throws<TomlException>(() => new TomlFile("fake_file.toml"));

    [Fact]
    public void Pass_Reads_SimpleTomlFile()
    {
        object? assignment;
        TomlFile toml = new("simple_toml.toml");
        toml.Read();
        var dict = toml.TomlDictionary;
        Assert.Multiple(
            () => Assert.True(dict.TryGetValue("isTrue", out assignment)),
            () => Assert.True(dict.TryGetValue("obj", out assignment)),
            () => Assert.True(dict.TryGetValue("strArray", out assignment)),
            () => Assert.True(dict.TryGetValue("numArray", out assignment)),
            () => Assert.True(dict.TryGetValue("table1.DayOfWeek", out assignment)),
            () => Assert.True(dict.TryGetValue("table1.HourOfDay", out assignment)),
            () => Assert.True(dict.TryGetValue("table1.MinuteOfHour", out assignment))
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
