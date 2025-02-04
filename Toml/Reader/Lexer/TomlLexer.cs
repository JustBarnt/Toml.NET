using System.Text;

using Toml.Exceptions;

namespace Toml.Reader.Lexicon;

public class Lexer(string input)
{
    private int position = 0;

    //Helpers
    private bool EndOfInput() => position >= input.Length;
    private char Peek() => input[position];
    private void NextChar() => position++;
    private static bool IsIdentifierStart(char c) => char.IsLetter(c) || c == '_' || c == '-';
    private static bool IsIdentifierPart(char c) => char.IsLetterOrDigit(c) || c == '_';

    public IEnumerable<Token> Tokenize()
    {
        while(!EndOfInput())
        {
            char current = Peek();

            switch (current)
            {
                case '\n':
                    yield return new Token(TokenType.NewLine, "\\n", position);
                    NextChar();
                    break;
                case '\r':
                    yield return new Token(TokenType.CarriageReturn, "\\r", position);
                    NextChar();
                    break;
                case '#':
                    yield return ReadComment();
                    break;
                case '[':
                    yield return new Token(TokenType.LeftBracket, "[", position);
                    NextChar();
                    break;
                case ']':
                    yield return new Token(TokenType.RightBracket, "]", position);
                    NextChar();
                    break;
                case '{':
                    yield return new Token(TokenType.LeftBrace, "{", position);
                    NextChar();
                    break;
                case '}':
                    yield return new Token(TokenType.RightBrace, "}", position);
                    NextChar();
                    break;
                case '=':
                    yield return new Token(TokenType.Equals, "=", position);
                    NextChar();
                    break;
                case ',':
                    yield return new Token(TokenType.Comma, ",", position);
                    NextChar();
                    break;
                case '.':
                    yield return new Token(TokenType.Dot, ".", position);
                    NextChar();
                    break;
                case '\"':
                case '\'':
                    // Add support for reading Multiline strings
                    yield return ReadString();
                    break;
                default:
                    if (char.IsWhiteSpace(current)) NextChar();
                    else if (char.IsDigit(current) || current == '-' || current == '+') yield return ReadNumber();
                    else if (IsIdentifierStart(current)) yield return ReadIdentifier();
                    else throw new TomlException($"Unexpected character '{current}' at position {position}");
                    break;
            }
        }

        yield return new Token(TokenType.EndOfFile, string.Empty, position);
    }

    private Token ReadComment()
    {
        int start = position;
        while (!EndOfInput() && (Peek() != '\n' && Peek() != '\r'))
        {
            NextChar();
        }
        string text = input.Substring(start, position - start);
        return new Token(TokenType.Comment, text, start);

    }

    private Token ReadIdentifier()
    {
        int start = position;
        while (!EndOfInput() && IsIdentifierPart(Peek()))
        {
            NextChar();
        }
        string text = input.Substring(start, position - start);
        return new Token(TokenType.Identifier, text, start);
    }

    private Token ReadString()
    {
        char quote = Peek();
        TokenType type = quote == '"' ? TokenType.String : TokenType.StringLiteral;
        int start = position;

        //Skip starting quote;
        NextChar();
        StringBuilder sb = new StringBuilder();

        while (!EndOfInput() && Peek() != quote)
        {
            sb.Append(Peek());
            NextChar();
        }

        if (EndOfInput()) throw new TomlException("Unterminated string literal");

        NextChar();
        string text = sb.ToString()
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace(@"\r\n", "\r\n")
            .Replace("\\t", "\t")
            .Replace("\\\"", "\"")
            .Replace("\\\'", "\'")
            .Replace(@"\\", "\\");

        return new Token(type, text, start);
    }

    private Token ReadNumber()
    {
        int start = position;
        while (!EndOfInput()
               && (char.IsDigit(Peek())
                   || Peek() == '.'
                   || Peek() == '-'
                   || Peek() == '+'))
        {
            NextChar();
        }
        string text = input.Substring(start, position - start);
        return new Token(TokenType.Number, text, start);
    }
}
