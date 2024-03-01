namespace Xnb;

public record XnbHeader(char Target, byte FormatVersion, bool Hidef, bool Compressed, int CompressionType);
