using System.Buffers.Binary;

namespace Xnb.Helper;

internal static class BinaryReaderExtensions
{
    public static int ReadLZXInt16(this BinaryReader reader, bool seek = true)
    {
        // read in the next two bytes worth of data
        byte[] bytes = reader.ReadBytes(2);

        // set the value
        return BinaryPrimitives.ReadInt16BigEndian(bytes);
    }

    public static object ReadPrimitive<T>(this BinaryReader reader)
    {
        return Type.GetTypeCode(typeof(T)) switch
               {
                   TypeCode.Boolean => reader.ReadBoolean(),
                   TypeCode.Char => reader.ReadChar(),
                   TypeCode.SByte => reader.ReadSByte(),
                   TypeCode.Byte => reader.ReadByte(),
                   TypeCode.Int16 => reader.ReadInt16(),
                   TypeCode.UInt16 => reader.ReadUInt16(),
                   TypeCode.Int32 => reader.ReadInt32(),
                   TypeCode.UInt32 => reader.ReadUInt32(),
                   TypeCode.Int64 => reader.ReadInt64(),
                   TypeCode.UInt64 => reader.ReadUInt64(),
                   TypeCode.Single => reader.ReadSingle(),
                   TypeCode.Double => reader.ReadDouble(),
                   TypeCode.Decimal => reader.ReadDecimal(),
                   TypeCode.String => reader.ReadString(),
                   _ => throw new ArgumentOutOfRangeException()
               };
    }
}

internal static class BinaryWriterExtensions
{
    public static void WritePrimitive(this BinaryWriter writer, object content)
    {
        switch (content)
        {
            case bool b:
                writer.Write(b);
                break;
            case char c:
                writer.Write(c);
                break;
            case sbyte s:
                writer.Write(s);
                break;
            case byte b:
                writer.Write(b);
                break;
            case short s:
                writer.Write(s);
                break;
            case ushort us:
                writer.Write(us);
                break;
            case int i:
                writer.Write(i);
                break;
            case uint ui:
                writer.Write(ui);
                break;
            case long l:
                writer.Write(l);
                break;
            case ulong ul:
                writer.Write(ul);
                break;
            case float f:
                writer.Write(f);
                break;
            case double d:
                writer.Write(d);
                break;
            case decimal d:
                writer.Write(d);
                break;
            case string s:
                writer.Write(s);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}