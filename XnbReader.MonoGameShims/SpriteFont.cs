using System.Drawing;
using System.Numerics;
using CommunityToolkit.HighPerformance.Buffers;

namespace XnbReader.MonoGameShims;

public record SpriteFont(Texture2D Texture, [HasHeader(true)] MemoryOwner<Rectangle> Glyphs, [HasHeader(true)] MemoryOwner<Rectangle> Cropping, [HasHeader(true)] char[] CharMap, int LineSpacing, float Spacing, [HasHeader(true)] Vector3[] Kerning, char? DefaultCharacter) : IDisposable
{
    public void Dispose()
    {
        Texture.Dispose();
        Glyphs.Dispose();
        Cropping.Dispose();
        GC.SuppressFinalize(this);
    }
}
