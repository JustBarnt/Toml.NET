using Toml.Exceptions;
using Toml.Reader.Extensions;
using Toml.Syntax;

namespace Toml.Reader;

/// <summary>
/// The class responsible for reading the Toml file given into an <c>IDictionary&lt;string,object&gt;</c>
/// </summary>
/// <param name="lines">An array of lines from a given file</param>
public class TomlReader(string[] lines)
{
    private List<string> unread_lines = lines.ToList();
    private List<string> table_names = [];

    private string? current_key;
    private TomlDictionary? current;

    // Used for separating the values in an array
    private static readonly char separator = ',';

    /// <summary>
    /// The Toml file parsed into a <c>Dictionary&lt;string, object&gt;</c>
    /// </summary>
    public TomlDictionary TomlDictionary { get; private set; } = [];

    /// <summary>
    /// Reads the given Toml file into a <see cref="TomlDictionary"/>
    /// </summary>
    /// <exception cref="TomlException">Throws if it fails to read and store into a <see cref="TomlDictionary"/> at any point</exception>
    public void ToDictionary()
    {
        current = TomlDictionary;
        // Set our initial table to the root of our dictionary
        using (var lines = unread_lines.GetEnumerator())
        {
            while (lines.MoveNext())
            {
                string line = lines.Current.Trim();

                char first_char = line[0];

                if (string.IsNullOrWhiteSpace(line)) continue;


                switch (line.SelectFirstWith(['#', '[']))
                {
                     case '#':
                     case null:
                         continue;

                    case '[':
                        // Get the substring of our line. I.E. [table] => table
                        string table_name = line[1..^1];
                        CreateTable(table_name);
                        continue;
                    default:
                        // Figure out type of value
                        // Parse out the key/value pair then move on
                        if (current_key == null) TomlDictionary.Add(line.Split("=")[0].Trim(), line.Split("=")[1].Trim());
                        continue;
                }

                // First check if line is a key/value or table declaration
                // if key/value split on '=' then check if the first char of the value side is one of the items mentioned above
                // to get know what its value type is

                // if the line is a table declaration create a new Dictionary<string,object> and add that to the TomlDictionary and
                // split the string on "." to get the total length of segments to add to the dictionary
                // we loop through each segment and check if the key already exists and if it doesn't we add that key and a new Dictionary
                // to the TomlDictionary and return our current position
            }
        }
    }

    // Get value after '='
    // if value is surround in quotes it is a string
    // if value is number its an int, float, etc.
    // if value is true or false it is a boolean
    // if value starts with [, or { it is an array or inline table

    private void ReadLine(string line)
    {
        // We are an empty line, so we are done here.
        if (string.IsNullOrWhiteSpace(line)) return;

        char first_char = line[0];

        switch (first_char)
        {
            case '[':
                // Get the substring of our line. I.E. [table] => table
                string table_name = line[1..^1];
                CreateTable(table_name);
                break;
            case '#':
                return;

            default:
                // Figure out type of value
                // Parse out the key/value pair then move on
                if (current_key == null) TomlDictionary.Add(line.Split("=")[0].Trim(), line.Split("=")[1].Trim());
                return;
        }

        // First check if line is a key/value or table declaration
        // if key/value split on '=' then check if the first char of the value side is one of the items mentioned above
        // to get know what its value type is

        // if the line is a table declaration create a new Dictionary<string,object> and add that to the TomlDictionary and
        // split the string on "." to get the total length of segments to add to the dictionary
        // we loop through each segment and check if the key already exists and if it doesn't we add that key and a new Dictionary
        // to the TomlDictionary and return our current position

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
            if (current is not null && current.TryGetValue(key, out object? value))
            {
                current = (TomlDictionary)TomlDictionary[key];
                continue;
            }

            // Add our key and a new empty dictionary to our TomlDictionary
            TomlDictionary.Add(key, new TomlDictionary());
            // Then set our current and current_key to continue moving inward as far as we need to
            current = (TomlDictionary)TomlDictionary[key];
            current_key = key;
        }
    }
}
