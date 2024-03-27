namespace XnbReader;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class XnbReadableAttribute(Type type) : Attribute
{
    public bool Reflective = false;
    public string? ReaderOverride = null;
}