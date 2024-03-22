namespace XnbReader.FileFormat;

[Flags]
public enum XnbFlag: byte
{
    Uncompressed = 0x0,
    HiDef = 0x1,
    Lz4 = 0x40,
    Lzx = 0x80
}
