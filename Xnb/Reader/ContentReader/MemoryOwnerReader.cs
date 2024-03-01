using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Buffers;

namespace Xnb.Reader.ContentReader;

public class MemoryOwnerReader<T>: BaseReader<MemoryOwner<T>> where T: unmanaged
{
    public override MemoryOwner<T> Read(BinaryReader buffer)
    {
        int length = buffer.ReadInt32();
        var memory = MemoryOwner<T>.Allocate(length);

        var span = MemoryMarshal.AsBytes(memory.Span);
        _ = buffer.Read(span);
        
        return memory;
    }

    public override void Write(BinaryWriter buffer, MemoryOwner<T> content)
    {
        var dest = MemoryMarshal.AsBytes(content.Span);
        buffer.Write(dest);
    }
}
