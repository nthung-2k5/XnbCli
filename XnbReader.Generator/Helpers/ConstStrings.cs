namespace XnbReader.Generator.Helpers;

public static class ConstStrings
{
    public const string Namespace = "XnbReader";
    public const string XnaFrameworkContentNamespace = "Microsoft.Xna.Framework.Content";
    
    public const string ReaderAttributeName = "XnbReadableAttribute";
    public const string FullReaderAttribute = $"{Namespace}.{ReaderAttributeName}";
    
    public const string ReaderConstructorAttributeName = "ReaderConstructorAttribute";
    public const string FullReaderConstructorAttribute = $"{Namespace}.{ReaderConstructorAttributeName}";
    
    public const string HasHeaderAttributeName = "HasHeaderAttribute";
    public const string FullHasHeaderAttribute = $"{Namespace}.{HasHeaderAttributeName}";
    
    public const string PostInitSourceCode = $$"""
// <auto-generated/>

namespace {{Namespace}};

[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public class {{ReaderAttributeName}}(Type ClassType) : System.Attribute
{
    public string? NamespaceOverride = null;
    
    public string? ReaderOverride = null;
    
    public const string DefaultReaderOverride = "Microsoft.Xna.Framework.Content.{Name}Reader";
}

[System.AttributeUsage(System.AttributeTargets.Constructor)]
public class {{ReaderConstructorAttributeName}}: System.Attribute;

[System.AttributeUsage(System.AttributeTargets.Parameter | System.AttributeTargets.Property)]
public class {{HasHeaderAttributeName}}(bool required): System.Attribute;

public interface ICustomReader<out T>
{
    static abstract T Read(BinaryReader reader);
}
""";
}
