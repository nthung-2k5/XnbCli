using XnbReader;

namespace Xnb.Reader;

public class BmFont([HasHeader(false)] string xml)
{
    public string Xml { get; } = xml;
}
