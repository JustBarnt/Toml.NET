using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Toml.Exceptions;

namespace Toml;


public class TomlDictionary(IDictionary<string, object> dictionary) : IDictionary<string, object>
{
    #region Properties
    public int Count => dictionary.Count;
    public bool IsReadOnly => dictionary.IsReadOnly;
    public ICollection<string> Keys => dictionary.Keys;
    public ICollection<object> Values => dictionary.Values;
    #endregion

    public object this[string key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    #region Methods
    public void Add(KeyValuePair<string, object> item) => dictionary.Add(item);
    public void Add(string key, object value) => dictionary.Add(key, value);
    public void Clear() => dictionary.Clear();
    public bool Contains(KeyValuePair<string, object> item) => dictionary.Contains(item);
    public bool ContainsKey(string key) => dictionary.ContainsKey(key);
    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => dictionary.CopyTo(array, arrayIndex);
    public bool Remove(KeyValuePair<string, object> item) => dictionary.Remove(item);
    public bool Remove(string key) => dictionary.Remove(key);
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
    {
        string[] nodes = key.Split('.');
        value = null;

        foreach (string node in nodes)
        {
            dictionary.TryGetValue(node, out object? attempt);
            if (attempt is null) continue;
            value = attempt;
        }

        return value is not null;
    }


    #endregion
    #region Enumeration
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)dictionary).GetEnumerator();
    #endregion
}
