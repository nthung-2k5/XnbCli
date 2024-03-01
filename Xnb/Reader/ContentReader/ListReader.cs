namespace Xnb.Reader.ContentReader;

public class ListReader<T>: BaseReader<List<T>>
{
    public override List<T> Read(BinaryReader buffer)
    {
        int len = buffer.ReadInt32();
        var array = new List<T>(len);

        for (int i = 0; i < len; i++)
        {
            array.Add(ReaderResolver.Read<T>(buffer));
        }

        return array;
    }

    public override void Write(BinaryWriter buffer, List<T> content)
    {
        buffer.Write(content.Count);

        foreach (var con in content)
        {
            ReaderResolver.Write(buffer, con);
        }
    }
}
