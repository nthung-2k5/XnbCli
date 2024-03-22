namespace XnbReader.MonoGameShims;

public class BmFont([HasHeader(false)] string xml)
{
    public string Xml { get; } = xml;
}
