using System.Text.Json.Serialization;

namespace XnbReader.FileFormat;

public record XnbHeader(char Target, byte FormatVersion, bool HiDef, XnbFlag Flag)
{
    [JsonIgnore]
    public bool Compressed => Flag >= XnbFlag.Lz4;
}
