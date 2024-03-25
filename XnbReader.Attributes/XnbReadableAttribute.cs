namespace XnbReader;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class XnbReadableAttribute(Type type) : Attribute
{
    public ContentTypeReader TypeReader = ContentTypeReader.StringKeyDictionary;
    
    public string? ReaderOverride = null;
}