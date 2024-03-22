// Taken from MonoGame

using CommunityToolkit.HighPerformance.Buffers;
using MemoryPack;

namespace XnbReader.Texture.Helpers;

internal static class DxtUtil
{
    public static MemoryOwner<byte> Decompress(ReadOnlySpan<byte> imageSpan, int width, int height, SurfaceFormat format)
    {
        var imageData = MemoryOwner<byte>.Allocate(width * height * 4);

        var reader = new MemoryPackReader(imageSpan, MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default));

        int blockCountX = (width + 3) / 4;
        int blockCountY = (height + 3) / 4;

        DecompressBlockDelegate func = format switch
        {
            SurfaceFormat.Dxt1 => DecompressDxt1Block,
            SurfaceFormat.Dxt3 => DecompressDxt3Block,
            SurfaceFormat.Dxt5 => DecompressDxt5Block,
            _ => throw new ArgumentException("Format not supported", nameof(format))
        };

        for (int y = 0; y < blockCountY; y++)
        {
            for (int x = 0; x < blockCountX; x++)
            {
                func(ref reader, x, y, width, height, imageData.Span);
            }
        }

        return imageData;
    }

    private static void DecompressDxt1Block(scoped ref MemoryPackReader imageReader, int x, int y, int width, int height, Span<byte> imageData)
    {
        Span<ushort> c = new ushort[2];
        imageReader.ReadSpanWithoutReadLengthHeader(2, ref c);

        ConvertRgb565ToRgb888(c[0], out byte r0, out byte g0, out byte b0);
        ConvertRgb565ToRgb888(c[1], out byte r1, out byte g1, out byte b1);

        uint lookupTable = imageReader.ReadUnmanaged<uint>();

        for (int blockY = 0; blockY < 4; blockY++)
        {
            for (int blockX = 0; blockX < 4; blockX++)
            {
                byte r = 0, g = 0, b = 0, a = 255;
                uint index = (lookupTable >> (2 * (4 * blockY + blockX))) & 0x03;

                if (c[0] > c[1])
                {
                    switch (index)
                    {
                        case 0:
                            r = r0;
                            g = g0;
                            b = b0;
                            break;
                        case 1:
                            r = r1;
                            g = g1;
                            b = b1;
                            break;
                        case 2:
                            r = (byte)((2 * r0 + r1) / 3);
                            g = (byte)((2 * g0 + g1) / 3);
                            b = (byte)((2 * b0 + b1) / 3);
                            break;
                        case 3:
                            r = (byte)((r0 + 2 * r1) / 3);
                            g = (byte)((g0 + 2 * g1) / 3);
                            b = (byte)((b0 + 2 * b1) / 3);
                            break;
                    }
                }
                else
                {
                    switch (index)
                    {
                        case 0:
                            r = r0;
                            g = g0;
                            b = b0;
                            break;
                        case 1:
                            r = r1;
                            g = g1;
                            b = b1;
                            break;
                        case 2:
                            r = (byte)((r0 + r1) / 2);
                            g = (byte)((g0 + g1) / 2);
                            b = (byte)((b0 + b1) / 2);
                            break;
                        case 3:
                            r = 0;
                            g = 0;
                            b = 0;
                            a = 0;
                            break;
                    }
                }

                int px = (x << 2) + blockX;
                int py = (y << 2) + blockY;

                if (px < width && py < height)
                {
                    int offset = (py * width + px) << 2;
                    imageData[offset] = r;
                    imageData[offset + 1] = g;
                    imageData[offset + 2] = b;
                    imageData[offset + 3] = a;
                }
            }
        }
    }

    private static void DecompressDxt3Block(ref MemoryPackReader imageReader, int x, int y, int width, int height, Span<byte> imageData)
    {
        Span<byte> alpha = new byte[8];
        imageReader.ReadSpanWithoutReadLengthHeader(8, ref alpha);

        Span<ushort> c = new ushort[2];
        imageReader.ReadSpanWithoutReadLengthHeader(2, ref c);

        ConvertRgb565ToRgb888(c[0], out byte r0, out byte g0, out byte b0);
        ConvertRgb565ToRgb888(c[1], out byte r1, out byte g1, out byte b1);

        uint lookupTable = imageReader.ReadUnmanaged<uint>();

        int alphaIndex = 0;

        for (int blockY = 0; blockY < 4; blockY++)
        {
            for (int blockX = 0; blockX < 4; blockX++)
            {
                byte r = 0, g = 0, b = 0;

                uint index = (lookupTable >> (2 * (4 * blockY + blockX))) & 0x03;

                byte a = alphaIndex switch
                {
                    0 => (byte)((alpha[0] & 0x0F) | ((alpha[0] & 0x0F) << 4)),
                    1 => (byte)((alpha[0] & 0xF0) | ((alpha[0] & 0xF0) >> 4)),
                    2 => (byte)((alpha[1] & 0x0F) | ((alpha[1] & 0x0F) << 4)),
                    3 => (byte)((alpha[1] & 0xF0) | ((alpha[1] & 0xF0) >> 4)),
                    4 => (byte)((alpha[2] & 0x0F) | ((alpha[2] & 0x0F) << 4)),
                    5 => (byte)((alpha[2] & 0xF0) | ((alpha[2] & 0xF0) >> 4)),
                    6 => (byte)((alpha[3] & 0x0F) | ((alpha[3] & 0x0F) << 4)),
                    7 => (byte)((alpha[3] & 0xF0) | ((alpha[3] & 0xF0) >> 4)),
                    8 => (byte)((alpha[4] & 0x0F) | ((alpha[4] & 0x0F) << 4)),
                    9 => (byte)((alpha[4] & 0xF0) | ((alpha[4] & 0xF0) >> 4)),
                    10 => (byte)((alpha[5] & 0x0F) | ((alpha[5] & 0x0F) << 4)),
                    11 => (byte)((alpha[5] & 0xF0) | ((alpha[5] & 0xF0) >> 4)),
                    12 => (byte)((alpha[6] & 0x0F) | ((alpha[6] & 0x0F) << 4)),
                    13 => (byte)((alpha[6] & 0xF0) | ((alpha[6] & 0xF0) >> 4)),
                    14 => (byte)((alpha[7] & 0x0F) | ((alpha[7] & 0x0F) << 4)),
                    15 => (byte)((alpha[7] & 0xF0) | ((alpha[7] & 0xF0) >> 4)),
                    _ => 0
                };
                ++alphaIndex;

                switch (index)
                {
                    case 0:
                        r = r0;
                        g = g0;
                        b = b0;
                        break;
                    case 1:
                        r = r1;
                        g = g1;
                        b = b1;
                        break;
                    case 2:
                        r = (byte)((2 * r0 + r1) / 3);
                        g = (byte)((2 * g0 + g1) / 3);
                        b = (byte)((2 * b0 + b1) / 3);
                        break;
                    case 3:
                        r = (byte)((r0 + 2 * r1) / 3);
                        g = (byte)((g0 + 2 * g1) / 3);
                        b = (byte)((b0 + 2 * b1) / 3);
                        break;
                }

                int px = (x << 2) + blockX;
                int py = (y << 2) + blockY;

                if (px < width && py < height)
                {
                    int offset = (py * width + px) << 2;
                    imageData[offset] = r;
                    imageData[offset + 1] = g;
                    imageData[offset + 2] = b;
                    imageData[offset + 3] = a;
                }
            }
        }
    }

    private static void DecompressDxt5Block(ref MemoryPackReader imageReader, int x, int y, int width, int height, Span<byte> imageData)
    {
        Span<byte> alpha = new byte[2];
        imageReader.ReadSpanWithoutReadLengthHeader(2, ref alpha);

        Span<byte> mask = stackalloc byte[8];
        imageReader.ReadSpanWithoutReadLengthHeader(6, ref alpha);

        ulong alphaMask = BitConverter.ToUInt64(mask);

        Span<ushort> c = new ushort[2];
        imageReader.ReadSpanWithoutReadLengthHeader(2, ref c);

        ConvertRgb565ToRgb888(c[0], out byte r0, out byte g0, out byte b0);
        ConvertRgb565ToRgb888(c[1], out byte r1, out byte g1, out byte b1);

        uint lookupTable = imageReader.ReadUnmanaged<uint>();

        for (int blockY = 0; blockY < 4; blockY++)
        {
            for (int blockX = 0; blockX < 4; blockX++)
            {
                byte r = 0, g = 0, b = 0, a;
                uint index = (lookupTable >> (2 * (4 * blockY + blockX))) & 0x03;

                uint alphaIndex = (uint)((alphaMask >> (3 * (4 * blockY + blockX))) & 0x07);

                switch (alphaIndex)
                {
                    case 0:
                        a = alpha[0];

                        break;
                    case 1:
                        a = alpha[1];

                        break;
                    case 6:
                        a = 0;

                        break;
                    case 7:
                        a = 0xff;

                        break;
                    default:
                    {
                        if (alpha[0] > alpha[1])
                        {
                            a = (byte)(((8 - alphaIndex) * alpha[0] + (alphaIndex - 1) * alpha[1]) / 7);
                        }
                        else
                        {
                            a = (byte)(((6 - alphaIndex) * alpha[0] + (alphaIndex - 1) * alpha[1]) / 5);
                        }

                        break;
                    }
                }

                switch (index)
                {
                    case 0:
                        r = r0;
                        g = g0;
                        b = b0;
                        break;
                    case 1:
                        r = r1;
                        g = g1;
                        b = b1;
                        break;
                    case 2:
                        r = (byte)((2 * r0 + r1) / 3);
                        g = (byte)((2 * g0 + g1) / 3);
                        b = (byte)((2 * b0 + b1) / 3);
                        break;
                    case 3:
                        r = (byte)((r0 + 2 * r1) / 3);
                        g = (byte)((g0 + 2 * g1) / 3);
                        b = (byte)((b0 + 2 * b1) / 3);
                        break;
                }

                int px = (x << 2) + blockX;
                int py = (y << 2) + blockY;

                if (px < width && py < height)
                {
                    int offset = (py * width + px) << 2;
                    imageData[offset] = r;
                    imageData[offset + 1] = g;
                    imageData[offset + 2] = b;
                    imageData[offset + 3] = a;
                }
            }
        }
    }

    private static void ConvertRgb565ToRgb888(ushort color, out byte r, out byte g, out byte b)
    {
        int temp = (color >> 11) * 255 + 16;
        r = (byte)((temp / 32 + temp) / 32);
        temp = ((color & 0x07E0) >> 5) * 255 + 32;
        g = (byte)((temp / 64 + temp) / 64);
        temp = (color & 0x001F) * 255 + 16;
        b = (byte)((temp / 32 + temp) / 32);
    }

    private delegate void DecompressBlockDelegate(ref MemoryPackReader imageReader, int x, int y, int width, int height, Span<byte> imageData);
}
