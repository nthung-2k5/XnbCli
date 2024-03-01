namespace Xnb.Reader.ContentReader;

public class NullableReader<T>: BaseReader<T?> where T: struct
{
    public override T? Read(BinaryReader buffer) => buffer.ReadBoolean() ? ReaderResolver.Read<T>(buffer) : default;

    public override void Write(BinaryWriter buffer, T? content)
    {
        buffer.Write(content.HasValue);

        if (content.HasValue)
        {
            ReaderResolver.Write(buffer, content.Value);
        }
    }
}
