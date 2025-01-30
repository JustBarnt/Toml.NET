namespace Toml;

public static class StringExtensions
{
    public static char? SelectFirstWith(this string str, char[] chars)
    {
        char? found = null;

        foreach (char c in chars)
            if (str.StartsWith(c)) found = c;

        return found;
    }
}
