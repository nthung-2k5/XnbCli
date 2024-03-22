namespace XnbReader;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class XnbReadableAttribute(Type type) : Attribute
{
    public string? NamespaceOverride = null;
    
    public string? ReaderOverride = null;
    
    public const string DefaultReaderOverride = "Microsoft.Xna.Framework.Content.{Name}Reader";
    public const string ReflectiveReaderOverride = "Microsoft.Xna.Framework.Content.ReflectiveReader`1[{FullName}]";
}