using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xnb.Reader.ContentReader;

public class UnmanagedReader<T>: BaseReader<T> where T: unmanaged
{
    public override T Read(BinaryReader buffer)
    {
        return Unsafe.ReadUnaligned<T>(ref buffer.ReadBytes(Unsafe.SizeOf<T>())[0]);
    }

    public override void Write(BinaryWriter buffer, T content)
    {
        byte[] data = new byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref data[0], content);
        buffer.Write(data);
    }
}

public class UnmanagedArrayReader<T> : BaseReader<T[]> where T: unmanaged
{
    public override T[] Read(BinaryReader buffer)
    {
        int length = buffer.ReadInt32();
        var array = GC.AllocateUninitializedArray<T>(length);

        var span = MemoryMarshal.AsBytes(array.AsSpan());
        _ = buffer.Read(span);
        
        return array;
    }

    public override void Write(BinaryWriter buffer, T[] content)
    {
        var dest = MemoryMarshal.AsBytes(content.AsSpan());
        buffer.Write(dest);
    }
}