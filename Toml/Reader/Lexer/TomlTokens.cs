namespace Toml.Reader.Lexicon;
public enum TokenType
{
    LeftBracket, // [
    RightBracket, // ]
    LeftBrace, // {
    RightBrace, // }
    Equals,
    Comma,
    Dot,
    Identifier, // Any unquoted key or value
    String, // "any string in double quotes"
    StringLiteral, // 'any string in single quotes'
    Number,
    Comment,
    Boolean,
    NewLine,
    CarriageReturn,
    EndOfFile
}

public class Token(TokenType type, string value, int position)
{
    public TokenType Type { get; } = type;
    public string Value { get; } = value;
    public int Position { get; } = position;
    public override string ToString() => $"{Type}: '{Value}' at {Position}";
}
