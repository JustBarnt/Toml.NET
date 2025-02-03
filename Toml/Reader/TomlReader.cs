using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Toml.Exceptions;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Toml.Reader;

/// <summary>
/// The class responsible for reading the Toml file given into an <c>IDictionary&lt;string,object&gt;</c>
/// </summary>
/// <param name="lines">An array of lines from a given file</param>
public partial class TomlReader(string[] lines)
{
    [GeneratedRegex(@"([\[|{]).*([\]|}])")]
    private static partial Regex MyRegex();

    private readonly List<string> unread_lines = lines.ToList();
    private readonly List<string> table_names = [];

    private TomlDictionary full_dictionary = new();
    private TomlDictionary partial_dictionary = new();

    /// <summary>
    /// Reads the given Toml file into a <see cref="TomlDictionary"/>
    /// </summary>
    /// <exception cref="TomlException">Throws if it fails to read and store into a <see cref="TomlDictionary"/> at any point</exception>
    public TomlDictionary ToDictionary()
    {
        partial_dictionary = full_dictionary;
        // Set our initial table to the root of our dictionary
        using (var lines = unread_lines.GetEnumerator())
        {
            while (lines.MoveNext())
            {
                string line = lines.Current.Trim();

                if (string.IsNullOrWhiteSpace(line)) continue;

                switch (line.FirstOrDefaultWith(['#', '[', '\n']))
                {
                    // Toml Comment
                    case '#':
                        continue;

                    // Toml Table
                    case '[':
                        // Get the value of our table without the braces
                        CreateTable(line[1..^1]);
                        continue;

                    // Toml Key/Value dataset
                    default:
                        string[] segments = line.Split("=", 2,StringSplitOptions.TrimEntries);

                        //Looks for matches where our value starts and ends with [] or {}
                        //this way we know if we are dealing with an array, object, or pure value
                        Match match = MyRegex().Match(segments[1]);

                        object? value = string.Join("", [match.Groups[1], match.Groups[2]]) switch
                        {
                            "[]" => JsonConvert.DeserializeAnonymousType(segments[1], (object[])[]),
                            "{}" => JsonConvert.DeserializeObject<TomlDictionary>(segments[1].Replace('=', ':')),
                            _ => JsonConvert.DeserializeObject(segments[1])
                        };

                        if (value is null) throw new TomlException("Error reading TOML file keys must have a value");

                        partial_dictionary.Add(segments[0], value);
                        continue;
                }
            }
        }

        return full_dictionary;
    }

    /// <summary>
    /// Creates a new table in our <see cref="TomlDictionary"/> property
    /// </summary>
    /// <param name="inlineTableName">The value of the table name to add to our dictionary.</param>
    /// <exception cref="TomlException">Throws if it finds a duplicate key.</exception>
    private void CreateTable(string inlineTableName)
    {
        string[] keys = inlineTableName.Split('.');

        if (table_names.Contains(inlineTableName))
            throw new TomlException($"Error table `{inlineTableName}` already exists.");

        // Add each unsplit table name into our list to compare against
        // this should prevent duplicate keys like [table.table1] but still allow [table.table2] and so forth
        table_names.Add(inlineTableName);

        foreach(string key in keys)
        {
            if (partial_dictionary.TryGetValue(key, out object? _))
            {
                // If the key exists, we just update our current_dictionary to that dictionary.
                // and continue through until our we have a new key
                partial_dictionary = (TomlDictionary)full_dictionary[key];
                continue;
            }

            // Add our key and a new empty dictionary to our full_dictionary
            full_dictionary.Add(key, new TomlDictionary());
            // Then set our partial
            partial_dictionary = (TomlDictionary)full_dictionary[key];
        }
    }
}
