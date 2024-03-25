namespace XnbReader;

[Flags]
public enum ContentTypeReader: byte
{
    None,
    Default = 1,
    Reflective = 2,
    List = 4,
    IntKeyDictionary = 8,
    StringKeyDictionary = 16,
    Custom = 32,
    FullCollection = List | IntKeyDictionary | StringKeyDictionary
}
