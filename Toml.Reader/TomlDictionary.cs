using System.Collections;
using System.Diagnostics.CodeAnalysis;

using TomlReader.DataTypes;

namespace TomlReader;

public class TomlDictionary :  Dictionary<string, object>, ITomlDictionary
{
    private IDictionary<string, object> self = new Dictionary<string, object>();
    public IDictionary<string, object> CreateTable(string tableName)
    {
        string[] segments = tableName.Split('.');
        IDictionary<string, object> active_table = self;

        foreach (string segment in segments)
        {
            if (!active_table.TryGetValue(segment, out object? value))
            {
                active_table.Add(segment, new Dictionary<string, object>());
                value = new Dictionary<string, object>();
                active_table[segment] = value;
            }

            if (value is IDictionary<string, object> next) active_table = next;
        }

        return active_table;
    }

    public bool ValidateTableName(string tableName) => !string.IsNullOrWhiteSpace(tableName) && !tableName.Any(static c => char.IsLetterOrDigit(c) && c != '.');

    public string ParseMultilineString(string start, IEnumerator<string> lines, StringType stringType)
    {
        // Get string after `"""`
        string content = start[3..];

        while (lines.MoveNext())
        {
            string line = lines.Current;

            if (stringType is StringType.Default)
            {
                content += BuildMultilineString(line, "\"\"\"", "\\");
                break;
            }

            // ReSharper disable once InvertIf
            if (stringType is StringType.Literal)
            {
                content += BuildMultilineString(line, "'''", null);
                break;
            }
        }

        return content;
    }

    public string ParseString(string line)
    {
        char[] trim_chars = ['"', '\''];
        string raw_value = line.Trim(trim_chars);
        raw_value = raw_value
            .Replace("\\n", "\n")
            .Replace("\\t", "\t")
            .Replace("\\\"", "\"")
            .Replace("\\\'", "\'")
            .Replace(@"\\", "\\");

        return raw_value;
    }

    public object ParseValue(string value, IEnumerator<string> lines)
    {
        const string string_literal = "'''";

        try
        {
            if (value.StartsWith("\"\"\"")) return ParseMultilineString(value, lines, StringType.Default);

            if (value.StartsWith(string_literal)) return ParseMultilineString(value, lines, StringType.Literal);

            if ((value.StartsWith('"') && value.EndsWith('"')) || (value.StartsWith('\'') && value.EndsWith('\''))) return ParseString(value);

            if (double.TryParse(value, out double number)) return number;

            if (bool.TryParse(value, out bool boolean)) return boolean;
            //TODO: Handle complex values like TOML arrays
            if (value.StartsWith("[[") && value.EndsWith("]]")) throw new NotImplementedException("TomlParser currently doesn't support TOML Arrays");

            if (!value.StartsWith('[') || !value.EndsWith(']')) return value; //Fallback for unrecognized types

            //TODO: Decide if hardcoding a Character separator is worth?
            string[] items = value[1..^1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            List<object> list = [];
            list.AddRange(items.Select(item => ParseValue(item, lines)));

            return list;
        }
        catch (Exception ex)
        {
            //TODO: Implement TOML EXCEPTION
            throw new Exception();
            /*throw new TomlParsingException(
                "Failed to parse TOML file. Please ensure path resolves to a valid TOML file.",
                ex
            );*/
        }
    }

    public string BuildMultilineString(string line, string endSeq, string? lineSeq)
    {
        // Check for closing `"""` or `'''`
        if (line.Trim().EndsWith(endSeq))
        {
            // Returns the substring minus the end sequence of characters
            return line[..^endSeq.Length];
        }

        if (!string.IsNullOrEmpty(lineSeq) && line.EndsWith(lineSeq))
            return line.TrimEnd('\\'); //Join content without a new line

        return line + "\n";
    }
}
