using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Serilog;
using XnbReader.Buffers;

namespace XnbReader;

public abstract class XnbContentReader(XnbStream stream, TypeResolver resolver) : BinaryReader(stream)
{
    protected abstract object Read(string readerType);
    
    public object LoadObject(bool loadIntoXnbFile = true)
    {
        content ??= Read(resolver.SimplifyType(stream.File.Readers[0].Type));
        
        if (loadIntoXnbFile && stream.File.Content is null)
        {
            stream.File.Content = content;
        }
        
        // we loaded the XNB file successfully
        Log.Information("Successfully read XNB file!");
        
        return content;
    }
    
    protected T[] ReadUnmanagedArray<T>() where T : unmanaged
    {
        int length = ReadInt32();
                                                  
        var array = GC.AllocateUninitializedArray<T>(length);
        _ = Read(MemoryMarshal.AsBytes(array.AsSpan()));
                                                  
        return array;
    }

    protected T ReadUnmanaged<T>() where T : unmanaged
    {
        unsafe
        {
            byte* temp = stackalloc byte[Unsafe.SizeOf<T>()];
            _ = Read(new Span<byte>(temp, Unsafe.SizeOf<T>()));
            return Unsafe.ReadUnaligned<T>(temp);
        }
    }

    protected MemoryOwner<T> ReadMemoryOwner<T>() where T : unmanaged
    {
        int length = ReadInt32();
        var memory = MemoryOwner<T>.Allocate(length);
                                                  
        var span = MemoryMarshal.AsBytes(memory.Span);
        _ = Read(span);
                                                  
        return memory;
    }

    public override char[] ReadChars(int count)
    {
        int length = ReadInt32();
                                                  
        char[] array = GC.AllocateUninitializedArray<char>(length);
        _ = Read(array);
                                                  
        return array;
    }

    private object? content;
}
