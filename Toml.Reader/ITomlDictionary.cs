using TomlReader.DataTypes;

namespace TomlReader;

public interface ITomlDictionary
{
    IDictionary<string, object> CreateTable(string tableName);

    // Use key to validate if table exists
    bool ValidateTableName(string tableName);

    string ParseMultilineString(string line, IEnumerator<string> lines, StringType stringType);
    string ParseString(string line);
    object ParseValue(string line, IEnumerator<string> lines);
    string BuildMultilineString(string line, string endSeq, string? lineSeq);
}
