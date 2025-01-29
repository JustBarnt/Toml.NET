namespace TomlReader;

public enum TomlReadState
{
    Initial,
    Reading,
    Error,
    EndOfFile,
    Closed
}
