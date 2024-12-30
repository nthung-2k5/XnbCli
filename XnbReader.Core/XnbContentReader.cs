using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Serilog;
using XnbReader.Buffers;

namespace XnbReader;

public abstract class XnbContentReader(XnbStream stream, TypeResolver resolver) : BinaryReader(stream)
{
    protected abstract object Read(string readerType, bool legacy = false);
    
    public object LoadObject(bool loadIntoXnbFile = true, bool legacy = false)
    {
        content ??= Read(resolver.SimplifyType(stream.File.Readers[0].Type), legacy);
        
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
        int readLength = Read(MemoryMarshal.AsBytes(array.AsSpan()));
        
        Debug.Assert(length * Unsafe.SizeOf<T>() == readLength);
                                                  
        return array;
    }

    protected T ReadUnmanaged<T>() where T : unmanaged
    {
        unsafe
        {
            int length = Unsafe.SizeOf<T>();
            byte* temp = stackalloc byte[length];
            int readLength = Read(new Span<byte>(temp, length));
            
            Debug.Assert(length == readLength);
            
            return Unsafe.ReadUnaligned<T>(temp);
        }
    }

    protected MemoryOwner<T> ReadMemoryOwner<T>() where T : unmanaged
    {
        int length = ReadInt32();
        var memory = MemoryOwner<T>.Allocate(length);
                                                  
        var span = MemoryMarshal.AsBytes(memory.Span);
        int readLength = Read(span);
        
        Debug.Assert(length * Unsafe.SizeOf<T>() == readLength);
                                                  
        return memory;
    }

    public override char[] ReadChars(int count)
    {
        char[] array = GC.AllocateUninitializedArray<char>(count);
        int readLength = Read(array);
        
        Debug.Assert(count == readLength);
                                                  
        return array;
    }

    private object? content;
}
