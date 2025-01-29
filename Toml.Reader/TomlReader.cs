namespace TomlReader;

public abstract partial class TomlReader : IDisposable
{
    public abstract TomlReadState ReadState { get; }

    // Indicates if we are at the end of the stream
    public abstract bool EOF { get; }

    // Current depth of the in-memory Dictionary
    public abstract int Depth { get; }

    // Returns the value of the current key
    public abstract string Value { get; }

    // Returns the value type of the current key
    public virtual Type ValueType => typeof(string);

    public virtual TomlDictionary TomlDictionary { get; } = [];

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        throw new NotImplementedException();
    }
}
