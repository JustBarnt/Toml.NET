using System.Runtime.InteropServices.Marshalling;

using Toml.Exceptions;
using Toml.Syntax;

namespace Toml.Reader.Extensions;

public static class TomlStringTokenExtensions
{
    /// <summary>
    /// Returns the text value of the <see cref="TomlStringToken"/> enumeration
    /// </summary>
    /// <returns>A string value equivalent of the Enumeration</returns>
    /// <exception cref="TomlException">Throws TomlException if an invalid StringToken enum is given</exception>
    public static string ToText(this TomlStringToken token)
    {
        return token switch
        {
            TomlStringToken.String => "\"\"\"",
            TomlStringToken.StringLiteral => "'''",
            TomlStringToken.Comment => "#",
            TomlStringToken.CarriageReturn => "\r",
            TomlStringToken.LineFeed => "\n",
            TomlStringToken.BackSlash => "\\",
            TomlStringToken.DoubleQuote => "\"",
            TomlStringToken.SingleQuote => "'",
            TomlStringToken.Comma => ",",
            TomlStringToken.LeftBrace => "{",
            TomlStringToken.RightBrace => "}",
            TomlStringToken.LeftBracket => "[",
            TomlStringToken.RightBracket => "]",
            _ => throw new TomlException($"Invalid StringToken: {token}", new ArgumentOutOfRangeException
            {
                HelpLink = null,
                HResult = 0,
                Source = null
            })
        };
    }

    public static bool Equals(this TomlStringToken token, char character) => token.ToText() == character.ToString();
}
