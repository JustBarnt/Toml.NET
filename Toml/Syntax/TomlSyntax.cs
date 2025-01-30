namespace Toml.Syntax;

/// <summary>
/// Enum type referencing different common string values
/// </summary>
public enum TomlStringToken
{
    String,
    StringLiteral,

    Comment,

    CarriageReturn,
    LineFeed,
    BackSlash,
    DoubleQuote,
    SingleQuote,
    Comma,
    LeftBracket,
    RightBracket,
    LeftBrace,
    RightBrace,
}

public enum LineType
{
    Table,
    String,
    StringLiteral,
    Integer,
    Boolean,
    Float,
    Array,
    InlineTable,
    Comment
}
