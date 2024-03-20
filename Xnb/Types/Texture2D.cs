using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using CommunityToolkit.HighPerformance.Buffers;
using StbImageWriteSharp;
using Xnb.Decoder;
using XnbReader;

namespace Xnb.Types;

public record Texture2D(SurfaceFormat Format, int Width, int Height, Memory<byte> Data): IDisposable, ICustomReader<Texture2D>
{
    [JsonIgnore]
    private readonly MemoryOwner<byte> dataOwner;

    [ReaderConstructor]
    public Texture2D(SurfaceFormat format, int width, int height, MemoryOwner<byte> owner) : this(format, width, height, owner.Memory)
    {
        dataOwner = owner;
    }

    public void Dispose()
    {
        dataOwner?.Dispose();
        GC.SuppressFinalize(this);
    }

    public void WriteTo(ImageWriter writer, Stream stream)
    {
        unsafe
        {
            fixed (byte* pHandle = &MemoryMarshal.GetReference(dataOwner.Span))
            {
                writer.WritePng(pHandle, Width, Height, ColorComponents.RedGreenBlueAlpha, stream);
            }
        }
    }

    public static Texture2D Read(BinaryReader reader)
    {
        var surfaceFormat = (SurfaceFormat)reader.ReadInt32();
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int levelCount = reader.ReadInt32();

        if (levelCount > 1)
        {
            //Log.Warning("Found mipcount of {0}, only the first will be used.", levelCount);
        }
        
        int dataLength = reader.ReadInt32();
        var dataOwner = MemoryOwner<byte>.Allocate(dataLength);

        var data = MemoryMarshal.AsBytes(dataOwner.Span);
        _ = reader.Read(data);
        

        //Convert the image data if required
        switch (surfaceFormat)
        {
            case SurfaceFormat.Dxt1:
            case SurfaceFormat.Dxt3:
            case SurfaceFormat.Dxt5:
                var decompressed = DxtUtil.Decompress(data, width, height, surfaceFormat);
                dataOwner.Dispose();

                dataOwner = decompressed;
                data = dataOwner.Span;
                break;
            case SurfaceFormat.Color:
                break;
            case SurfaceFormat.Bgra5551:
                throw new XnbException("Texture2D format type ECT1 not implemented!");
            default:
                throw new XnbException($"Non-implemented Texture2D format type ({surfaceFormat}) found.");
        }

        // add the alpha channel into the image
        for (int i = 0; i < data.Length; i += 4)
        {
            float inverseAlpha = 255f / data[i + 3];
            data[i] = (byte)Math.Min(MathF.Ceiling(data[i] * inverseAlpha), 255);
            data[i + 1] = (byte)Math.Min(MathF.Ceiling(data[i + 1] * inverseAlpha), 255);
            data[i + 2] = (byte)Math.Min(MathF.Ceiling(data[i + 2] * inverseAlpha), 255);
        }

        return new Texture2D(surfaceFormat, width, height, dataOwner);
    }
}
