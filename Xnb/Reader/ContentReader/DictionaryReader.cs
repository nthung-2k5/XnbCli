namespace Xnb.Reader.ContentReader;

public class DictionaryReader<TKey, TValue>: BaseReader<Dictionary<TKey, TValue>> where TKey: notnull
{
    public override Dictionary<TKey, TValue> Read(BinaryReader buffer)
    {
        int len = buffer.ReadInt32();
        var dict = new Dictionary<TKey, TValue>(len);
        
        for (int i = 0; i < len; i++)
        {
            dict.Add(ReaderResolver.Read<TKey>(buffer), ReaderResolver.Read<TValue>(buffer));
        }

        return dict;
    }

    public override void Write(BinaryWriter buffer, Dictionary<TKey, TValue> content)
    {
        buffer.Write(content.Count);

        foreach (var con in content)
        {
            ReaderResolver.Write(buffer, con.Key);
            ReaderResolver.Write(buffer, con.Value);
        }
    }
}
