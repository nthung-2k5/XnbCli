namespace Xnb.Reader.ContentReader;

public class ArrayReader<T>: BaseReader<T[]>
{
    public override T[] Read(BinaryReader buffer)
    {
        int len = buffer.ReadInt32();
        var array = new T[len];

        for (int i = 0; i < len; i++)
        {
            array[i] = ReaderResolver.Read<T>(buffer);
        }

        return array;
    }

    public override void Write(BinaryWriter buffer, T[] content)
    {
        buffer.Write(content.Length);

        foreach (var con in content)
        {
            ReaderResolver.Write(buffer, con);
        }
    }
}
