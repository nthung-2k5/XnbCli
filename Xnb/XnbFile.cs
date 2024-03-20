namespace Xnb;

public class XnbFile
{
    public XnbHeader Header { get; set; }
    public XnbReader[] Readers { get; set; }
    public object Content { get; set; }
}
