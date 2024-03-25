namespace XnbReader.Generator.Helpers;

public static class ConstStrings
{
    public const string Namespace = nameof(XnbReader);
    public const string XnaFrameworkContentNamespace = "Microsoft.Xna.Framework.Content";
    
    public const string XnbReadableAttributeName = "XnbReadableAttribute";
    public const string FullXnbReadableAttribute = $"{Namespace}.{XnbReadableAttributeName}";
    
    public const string HasHeaderAttributeName = "HasHeaderAttribute";
    public const string FullHasHeaderAttribute = $"{Namespace}.{HasHeaderAttributeName}";
    
    public const string ReaderConstructorAttributeName = "ReaderConstructorAttribute";
    public const string FullReaderConstructorAttribute = $"{Namespace}.{ReaderConstructorAttributeName}";
    
    public const string InterfaceCustomReaderName = "ICustomReader`1";
    public const string FullInterfaceCustomReader = $"{Namespace}.{InterfaceCustomReaderName}";

    public const string XnbContentReaderName = "XnbContentReader";
    public const string FullXnbContentReader = $"{Namespace}.{XnbContentReaderName}";

    public const string DefaultReader = $"{XnaFrameworkContentNamespace}.{{Name}}Reader";
    public const string ReflectiveReader = $"{XnaFrameworkContentNamespace}.ReflectiveReader`1[{{FullName}}]";
    public const string ListReader = $"{XnaFrameworkContentNamespace}.ListReader`1[{{Value}}]";
    public const string DictionaryReader = $"{XnaFrameworkContentNamespace}.DictionaryReader`2[{{Key}},{{Value}}]";
    
}
