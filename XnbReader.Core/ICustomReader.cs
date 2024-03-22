namespace XnbReader;

public interface ICustomReader<out T>
{
    static abstract T Read(BinaryReader reader);
}
