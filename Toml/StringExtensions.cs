namespace Toml;

public static class StringExtensions
{
    public static char? FirstOrDefaultWith(this string str, char[] chars)
    {
        char? found = null;
        foreach (char c in chars)
            if (str.StartsWith(c)) found = c;
        return found;
    }

    public static bool StartsWithAny(this string str, params char[] chars)
    {
        bool found = false;
        foreach (char c in chars)
            if (str.StartsWith(c)) found = true;
        return found;
    }

    public static bool EndsWithAny(this string str, params char[] chars)
    {
        bool found = false;
        foreach (char c in chars)
            if (str.EndsWith(c)) found = true;
        return found;
    }
}
