namespace XnbReader.Generator.Helpers;

public static class ConstStrings
{
    private const string Namespace = nameof(XnbReader);
    public const string XnaFrameworkContentNamespace = "Microsoft.Xna.Framework.Content";

    private const string XnbReadableAttributeName = "XnbReadableAttribute";
    public const string FullXnbReadableAttribute = $"{Namespace}.{XnbReadableAttributeName}";

    private const string HasHeaderAttributeName = "HasHeaderAttribute";
    public const string FullHasHeaderAttribute = $"{Namespace}.{HasHeaderAttributeName}";

    private const string ReaderConstructorAttributeName = "ReaderConstructorAttribute";
    public const string FullReaderConstructorAttribute = $"{Namespace}.{ReaderConstructorAttributeName}";

    private const string InterfaceCustomReaderName = "ICustomReader`1";
    public const string FullInterfaceCustomReader = $"{Namespace}.{InterfaceCustomReaderName}";

    private const string XnbContentReaderName = "XnbContentReader";
    public const string FullXnbContentReader = $"{Namespace}.{XnbContentReaderName}";
}
