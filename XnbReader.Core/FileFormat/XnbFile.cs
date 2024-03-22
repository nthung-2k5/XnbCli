namespace XnbReader.FileFormat;

public record XnbFile(XnbHeader Header, XnbTypeReader[] Readers)
{
    public object? Content { get; set; }
}
