namespace Xnb.Reader.ContentReader;

public abstract class BaseReader
{
    public abstract void Read(BinaryReader buffer, out object obj);
    public abstract void Write(BinaryWriter buffer, object content);
}

public abstract class BaseReader<T>: BaseReader
{
    public override void Read(BinaryReader buffer, out object obj)
    {
        obj = Read(buffer);
    }
    
    public override void Write(BinaryWriter buffer, object content)
    {
        Write(buffer, (T)content);
    }

    public abstract T Read(BinaryReader buffer);
    public abstract void Write(BinaryWriter buffer, T content);
}