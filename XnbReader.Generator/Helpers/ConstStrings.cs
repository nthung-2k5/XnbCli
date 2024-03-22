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
    
    public const string InterfaceCustomReaderName = "ICustomReader`1";
    public const string FullInterfaceCustomReader = $"{Namespace}.{InterfaceCustomReaderName}";

    public const string XnbContentReaderName = "XnbContentReader";
    public const string FullXnbContentReader = $"{Namespace}.{XnbContentReaderName}";
}
