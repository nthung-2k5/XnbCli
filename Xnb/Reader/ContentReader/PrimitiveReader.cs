namespace Xnb.Reader.ContentReader;

public class BooleanReader: UnmanagedReader<bool>;
public class DoubleReader: UnmanagedReader<double>;
public class Int32Reader: UnmanagedReader<int>;
public class SingleReader: UnmanagedReader<float>;
public class UInt32Reader: UnmanagedReader<uint>;
public class CharReader: BaseReader<char>
{
    public override char Read(BinaryReader buffer) => buffer.ReadChar();
    public override void Write(BinaryWriter buffer, char content) => buffer.Write(content);
}
public class StringReader: BaseReader<string>
{
    public override string Read(BinaryReader buffer) => buffer.ReadString();
    public override void Write(BinaryWriter buffer, string content) => buffer.Write(content);
}

public static partial class ReaderResolver
{
    static partial void RegisterPrimitiveReaders()
    {
        Register(new BooleanReader());
        Register(new DoubleReader());
        Register(new Int32Reader());
        Register(new SingleReader());
        Register(new UInt32Reader());
        Register(new CharReader());
        Register(new StringReader());
    }
}