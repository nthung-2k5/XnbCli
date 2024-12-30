using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XnbReader.Buffers;

public static class BinaryReaderExtensions
{
    public static T[] ReadUnmanagedArray<T>(this BinaryReader reader, int length) where T : unmanaged
    {
        var array = GC.AllocateUninitializedArray<T>(length);
        int readLength = reader.Read(MemoryMarshal.AsBytes(array.AsSpan()));
        
        Debug.Assert(length * Unsafe.SizeOf<T>() == readLength);
                                                  
        return array;
    }
    
    public static void ReadUnmanagedArray<T>(this BinaryReader reader, Span<T> span) where T : unmanaged
    {
        int readLength = reader.Read(MemoryMarshal.AsBytes(span));
        Debug.Assert(span.Length * Unsafe.SizeOf<T>() == readLength);
    }

    public static T ReadUnmanaged<T>(this BinaryReader reader) where T : unmanaged
    {
        unsafe
        {
            int length = Unsafe.SizeOf<T>();
            byte* temp = stackalloc byte[length];
            int readLength = reader.Read(new Span<byte>(temp, length));
            
            Debug.Assert(length == readLength);
            
            return Unsafe.ReadUnaligned<T>(temp);
        }
    }
}
