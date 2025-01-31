using System.Text.Json;
using System.Text.Json.Nodes;

using Toml.Exceptions;

namespace Toml.Reader;

/// <summary>
/// The class responsible for reading the Toml file given into an <c>IDictionary&lt;string,object&gt;</c>
/// </summary>
/// <param name="lines">An array of lines from a given file</param>
public class TomlReader(string[] lines)
{
    private List<string> unread_lines = lines.ToList();
    private List<string> table_names = [];

    private TomlDictionary? current_dictionary;

    /// <summary>
    /// The Toml file parsed into a <c>Dictionary&lt;string, object&gt;</c>
    /// </summary>
    public TomlDictionary TomlDictionary { get; private set; } = [];

    /// <summary>
    /// Reads the given Toml file into a <see cref="TomlDictionary"/>
    /// </summary>
    /// <exception cref="TomlException">Throws if it fails to read and store into a <see cref="TomlDictionary"/> at any point</exception>
    public TomlDictionary ToDictionary()
    {
        current_dictionary = TomlDictionary;
        // Set our initial table to the root of our dictionary
        using (var lines = unread_lines.GetEnumerator())
        {
            while (lines.MoveNext())
            {
                string line = lines.Current.Trim();

                if (string.IsNullOrWhiteSpace(line)) continue;

                switch (line.FirstOrDefaultWith(['#', '[']))
                {
                    // If a comment or an empty line we just continue to the next line
                    case '#':

                    // We are at the start of a new table we need to create a new table set our current_dictionary to it
                    case '[':
                        // Get the value of our table without the braces
                        CreateTable(line[1..^1]);
                        continue;

                    // We are at a key/value pair so we just add it to our current dictionary
                    // We send the value through JsonSerializer to get out what should be the correct value of the even if
                    // it is an array or object
                    default:
                        string[] segments = line.Split("=", 2,StringSplitOptions.TrimEntries);
                        string key = segments[0];
                        string string_value = segments[1];
                        object? value;

                        if (string_value.StartsWith('[') && string_value.EndsWith(']'))
                        {
                            JsonElement[] elements = JsonSerializer.Deserialize<JsonElement[]>(string_value)!;
                            value = GetEnumerationValue(elements);
                        }
                        else if (string_value.StartsWith('{') && string_value.EndsWith('}'))
                        {
                            TomlDictionary table = new();
                            string[] obj_values = string_value
                                .Trim('{', '}')
                                .Split(",", StringSplitOptions.TrimEntries);

                            foreach (string v in obj_values)
                            {
                                string[] kvp = v.Split('=', StringSplitOptions.TrimEntries);
                                string t_key = kvp[0];
                                object? t_value = GetValue(JsonSerializer.Deserialize<JsonElement>(kvp[1]));

                                if (t_value is null) throw new TomlException($"Error reading TOML into dictionary");
                                table.Add(t_key, t_value);
                            }
                            value = table;
                        }
                        else
                        {
                            JsonElement element = JsonSerializer.Deserialize<JsonElement>(string_value);
                            value = GetValue(element);
                        }

                        if (value is null) throw new TomlException("Error reading TOML file keys must have a value");

                        current_dictionary.Add(key, value);
                        continue;
                }
            }
        }

        return TomlDictionary;
    }

    private static object?[] GetEnumerationValue(JsonElement[] elements)
    {
        List<object?> converted_values = [];
        converted_values.AddRange(elements.Select(GetValue));
        return converted_values.Count == 0 ? [] : converted_values.ToArray();
    }

    private static object? GetValue(JsonElement value)
    {
        JsonValueKind kind = value.ValueKind;

        return kind switch
        {
            JsonValueKind.True or JsonValueKind.False => value.GetBoolean(),
            JsonValueKind.Number or JsonValueKind.String => value.GetRawText(),
            _ => null
        };
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
            if (current_dictionary is not null && current_dictionary.TryGetValue(key, out object? _))
            {
                // If the key exists, we just update our current_dictionary to that dictionary.
                // and continue through until our we have a new key
                current_dictionary = (TomlDictionary)TomlDictionary[key];
                continue;
            }

            // Add our key and a new empty dictionary to our TomlDictionary
            TomlDictionary.Add(key, new TomlDictionary());
            // Then set our current and current_key to continue moving inward as far as we need to
            current_dictionary = (TomlDictionary)TomlDictionary[key];
        }
    }
}
