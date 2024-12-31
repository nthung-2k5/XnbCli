using System.Numerics;
using System.Runtime.InteropServices;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using XnbReader.Buffers;
using XnbReader.MonoGameShims.Helpers;

namespace XnbReader.MonoGameShims;

public record Texture2D(SurfaceFormat Format, int Width, int Height, MemoryOwner<byte> Data): IDisposable, ICustomReader<Texture2D>
{
    public void Dispose()
    {
        Data.Dispose();
        GC.SuppressFinalize(this);
    }

    public static Texture2D Read(BinaryReader reader, bool legacy = false)
    {
        int formatValue = reader.ReadInt32();
        var surfaceFormat = legacy ? SurfaceFormatExtensions.Parse(((SurfaceFormatLegacy)formatValue).ToStringFast()) : (SurfaceFormat)formatValue;

        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int levelCount = reader.ReadInt32();

        if (levelCount > 1)
        {
            Log.Warning("Found mipcount of {0}, only the first will be used.", levelCount);
        }
        
        MemoryOwner<byte> dataOwner;

        //Convert the image data if required
        int length = reader.ReadInt32();
        switch (surfaceFormat)
        {
            case SurfaceFormat.Dxt1:
            case SurfaceFormat.Dxt3:
            case SurfaceFormat.Dxt5:
                dataOwner = DxtUtil.Decompress(reader, width, height, surfaceFormat);
                break;
            case SurfaceFormat.Color:
                dataOwner = MemoryOwner<byte>.Allocate(length);
                _ = reader.Read(dataOwner.Span);
                break;
            default:
            {
                using var temp = MemoryOwner<byte>.Allocate(length);
                _ = reader.Read(temp.Span);

                dataOwner = surfaceFormat switch
                {
                    SurfaceFormat.Bgr565 => DecompressPrimitive<Bgr565>(temp),
                    SurfaceFormat.Bgra5551 => DecompressPrimitive<Bgra5551>(temp),
                    SurfaceFormat.Bgra4444 => DecompressPrimitive<Bgra4444>(temp),
                    SurfaceFormat.NormalizedByte2 => DecompressPrimitive<NormalizedByte2>(temp),
                    SurfaceFormat.NormalizedByte4 => DecompressPrimitive<NormalizedByte4>(temp),
                    SurfaceFormat.Rgba1010102 => DecompressPrimitive<Rgba1010102>(temp),
                    SurfaceFormat.Rg32 => DecompressPrimitive<Rg32>(temp),
                    SurfaceFormat.Rgba64 => DecompressPrimitive<Rgba64>(temp),
                    SurfaceFormat.Alpha8 => DecompressPrimitive<A8>(temp),
                    SurfaceFormat.Single => DecompressPrimitive<SinglePixel>(temp),
                    SurfaceFormat.Vector2 => DecompressPrimitive<Vector2Pixel>(temp),
                    SurfaceFormat.HdrBlendable or SurfaceFormat.Vector4 => DecompressPrimitive<RgbaVector>(temp),
                    SurfaceFormat.HalfSingle => DecompressPrimitive<HalfSingle>(temp),
                    SurfaceFormat.HalfVector2 => DecompressPrimitive<HalfVector2>(temp),
                    SurfaceFormat.HalfVector4 => DecompressPrimitive<HalfVector4>(temp),
                    _ => MemoryOwner<byte>.Empty
                };

                if (dataOwner.Length == 0)
                {
                    throw new NotSupportedException($"Non-implemented Texture2D format type ({surfaceFormat}) found.");
                }
                break;
            }
        }

        var data = dataOwner.Span;

        // add the alpha channel into the image
        for (int i = 0; i < data.Length; i += 4)
        {
            float inverseAlpha = 255f / data[i + 3];
            data[i] = (byte)Math.Min(MathF.Ceiling(data[i] * inverseAlpha), 255);
            data[i + 1] = (byte)Math.Min(MathF.Ceiling(data[i + 1] * inverseAlpha), 255);
            data[i + 2] = (byte)Math.Min(MathF.Ceiling(data[i + 2] * inverseAlpha), 255);
        }

        return new Texture2D(surfaceFormat, width, height, dataOwner);

        static MemoryOwner<byte> DecompressPrimitive<TPixel>(MemoryOwner<byte> data) where TPixel: unmanaged, IPixel<TPixel>
        {
            var span = MemoryMarshal.Cast<byte, TPixel>(data.Span);
            
            var decompressed = MemoryOwner<byte>.Allocate(span.Length * 4);
            var decompressedSpan = MemoryMarshal.Cast<byte, Rgba32>(decompressed.Span);

            for (int i = 0; i < span.Length; i++)
            {
                span[i].ToRgba32(ref decompressedSpan[i]);
            }
            
            return decompressed;
        }
    }

    public void WriteTo(Stream stream)
    {
        using var image = Image.LoadPixelData<Rgba32>(Data.Span, Width, Height);
        image.SaveAsPng(stream);
    }
}

/// <summary>
/// Stub pixel format for <see cref="SurfaceFormat.Single"/>
/// </summary>
file struct SinglePixel : IPixel<SinglePixel>, IPackedVector<float>
{
    public PixelOperations<SinglePixel> CreatePixelOperations() => throw new InvalidOperationException();

    public void FromScaledVector4(Vector4 vector)
    {
        throw new InvalidOperationException();
    }

    public Vector4 ToScaledVector4() => throw new InvalidOperationException();

    public void FromVector4(Vector4 vector)
    {
        throw new InvalidOperationException();
    }

    public Vector4 ToVector4() => throw new InvalidOperationException();

    public void FromArgb32(Argb32 source)
    {
        throw new InvalidOperationException();
    }

    public void FromBgra5551(Bgra5551 source)
    {
        throw new InvalidOperationException();
    }

    public void FromBgr24(Bgr24 source)
    {
        throw new InvalidOperationException();
    }

    public void FromBgra32(Bgra32 source)
    {
        throw new InvalidOperationException();
    }

    public void FromAbgr32(Abgr32 source)
    {
        throw new InvalidOperationException();
    }

    public void FromL8(L8 source)
    {
        throw new InvalidOperationException();
    }

    public void FromL16(L16 source)
    {
        throw new InvalidOperationException();
    }

    public void FromLa16(La16 source)
    {
        throw new InvalidOperationException();
    }

    public void FromLa32(La32 source)
    {
        throw new InvalidOperationException();
    }

    public void FromRgb24(Rgb24 source)
    {
        throw new InvalidOperationException();
    }

    public void FromRgba32(Rgba32 source)
    {
        throw new InvalidOperationException();
    }

    public void ToRgba32(ref Rgba32 dest)
    {
        dest = new Rgba32(PackedValue, PackedValue, PackedValue);
    }

    public void FromRgb48(Rgb48 source)
    {
        throw new InvalidOperationException();
    }

    public void FromRgba64(Rgba64 source)
    {
        throw new InvalidOperationException();
    }

    public bool Equals(SinglePixel other) => throw new InvalidOperationException();

    public float PackedValue { get; set; }
}

/// <summary>
/// Stub pixel format for <see cref="SurfaceFormat.Vector2"/>
/// </summary>
file struct Vector2Pixel : IPixel<Vector2Pixel>, IPackedVector<Vector2>
{
    public PixelOperations<Vector2Pixel> CreatePixelOperations() => throw new InvalidOperationException();

    public void FromScaledVector4(Vector4 vector)
    {
        throw new InvalidOperationException();
    }

    public Vector4 ToScaledVector4() => throw new InvalidOperationException();

    public void FromVector4(Vector4 vector)
    {
        throw new InvalidOperationException();
    }

    public Vector4 ToVector4() => throw new InvalidOperationException();

    public void FromArgb32(Argb32 source)
    {
        throw new InvalidOperationException();
    }

    public void FromBgra5551(Bgra5551 source)
    {
        throw new InvalidOperationException();
    }

    public void FromBgr24(Bgr24 source)
    {
        throw new InvalidOperationException();
    }

    public void FromBgra32(Bgra32 source)
    {
        throw new InvalidOperationException();
    }

    public void FromAbgr32(Abgr32 source)
    {
        throw new InvalidOperationException();
    }

    public void FromL8(L8 source)
    {
        throw new InvalidOperationException();
    }

    public void FromL16(L16 source)
    {
        throw new InvalidOperationException();
    }

    public void FromLa16(La16 source)
    {
        throw new InvalidOperationException();
    }

    public void FromLa32(La32 source)
    {
        throw new InvalidOperationException();
    }

    public void FromRgb24(Rgb24 source)
    {
        throw new InvalidOperationException();
    }

    public void FromRgba32(Rgba32 source)
    {
        throw new InvalidOperationException();
    }

    public void ToRgba32(ref Rgba32 dest)
    {
        dest = new Rgba32(new Vector3(PackedValue, 1));
    }

    public void FromRgb48(Rgb48 source)
    {
        throw new InvalidOperationException();
    }

    public void FromRgba64(Rgba64 source)
    {
        throw new InvalidOperationException();
    }

    public bool Equals(Vector2Pixel other) => throw new InvalidOperationException();

    public Vector2 PackedValue { get; set; }
}