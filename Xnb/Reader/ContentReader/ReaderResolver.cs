using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Buffers;

namespace Xnb.Reader.ContentReader;

public static partial class ReaderResolver
{
    private static readonly Dictionary<Type, BaseReader> Readers = new();

    private static readonly Dictionary<Type, Type> GenericReaders = new()
    {
        [typeof(List<>)] = typeof(ListReader<>),
        [typeof(Dictionary<,>)] = typeof(DictionaryReader<,>),
        [typeof(Nullable<>)] = typeof(NullableReader<>),
        [typeof(MemoryOwner<>)] = typeof(MemoryOwnerReader<>)
    };
    
    private static void Register<T>(BaseReader<T> reader)
    {
        Readers[typeof(T)] = reader;
    }

    static ReaderResolver()
    {
        RegisterPrimitiveReaders();
        RegisterExplicitReaders();
        RegisterNumericsReaders();
        RegisterClassReaders();
    }

    static partial void RegisterPrimitiveReaders();
    static partial void RegisterExplicitReaders();
    static partial void RegisterNumericsReaders();
    static partial void RegisterClassReaders();
    
    public static object Read(string reader, BinaryReader buffer)
    {
        // read the buffer using the selected reader
        return Read(buffer, Type.GetType(TypeResolver.SimplifyType(reader))!);
    }

    private static object Read(BinaryReader buffer, Type type)
    {
        if (!type.IsValueType && !type.IsArray)
        {
            // read and discard the reader
            if (buffer.Read7BitEncodedInt() == 0)
            {
                return null;
            }
        }
        
        if (!Readers.TryGetValue(type, out var reader))
        {
            reader = (BaseReader)GetGenericReader(type, true)!;
        }

        // read the buffer using the selected reader
        reader.Read(buffer, out object obj);

        return obj;
    }
    
    public static T Read<T>(BinaryReader buffer, bool readHeader = false)
    {
        var type = typeof(T);
        if (readHeader || (!type.IsValueType && !type.IsArray && !type.IsSubclassOfRawGeneric(typeof(MemoryOwner<>))))
        {
            // read and discard the reader
            if (buffer.Read7BitEncodedInt() == 0)
            {
                return default;
            }
        }
        
        if (!Readers.TryGetValue(type, out var reader))
        {
            reader = GetGenericReader<T>();
        }
        // read the buffer using the selected reader
        return ((BaseReader<T>)reader).Read(buffer);
    }

    private static BaseReader<T> GetGenericReader<T>()
    {
        return (BaseReader<T>)GetGenericReader(typeof(T),  typeof(T).IsArray ? !typeof(T).GetElementType()!.IsValueType : RuntimeHelpers.IsReferenceOrContainsReferences<T>())!;
    }

    private static object GetGenericReader(Type type, bool containRef)
    {
        Type readerType;
        
        if (type.IsArray)
        {
            if (type.IsSZArray)
            {
                var arrReader = containRef || type.GetElementType() == typeof(char) ? typeof(ArrayReader<>) : typeof(UnmanagedArrayReader<>);
                readerType = arrReader.MakeGenericType(type.GetElementType()!);
            }
            else
            {
                return null;
            }
        }
        else
        {
            var genericType = type.GetGenericTypeDefinition();

            if (GenericReaders.TryGetValue(genericType, out var genericReader))
            {
                readerType = genericReader.MakeGenericType(type.GetGenericArguments());
            }
            else
            {
                return null;
            }
        }

        return Readers[type] = (BaseReader)Activator.CreateInstance(readerType)!;
    }

    public static void Write<T>(BinaryWriter buffer, T content)
    {
        throw new NotImplementedException();
    }
}
