using System.Drawing;
using System.Numerics;
using CommunityToolkit.HighPerformance.Buffers;
using XnbReader;

namespace Xnb.Types;

public record SpriteFont(Texture2D Texture, [HasHeader(true)] MemoryOwner<Rectangle> Glyphs, [HasHeader(true)] MemoryOwner<Rectangle> Cropping, [HasHeader(true)] char[] CharMap, int LineSpacing, float Spacing, [HasHeader(true)] Vector3[] Kerning, char? DefaultCharacter) : IDisposable
{
    public void Dispose()
    {
        Texture?.Dispose();
        Glyphs?.Dispose();
        Cropping?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public record ExternalSpriteFont(string Texture, EnumerableMemoryOwner<Rectangle> Glyphs, EnumerableMemoryOwner<Rectangle> Cropping, char[] CharMap, int LineSpacing, float Spacing, Vector3[] Kerning, char? DefaultCharacter) : IDisposable
{
    public ExternalSpriteFont(SpriteFont spriteFont, string file): this(file, spriteFont.Glyphs, spriteFont.Cropping, spriteFont.CharMap, spriteFont.LineSpacing, spriteFont.Spacing, spriteFont.Kerning, spriteFont.DefaultCharacter) { }
    public void Dispose()
    {
        Glyphs?.Dispose();
        Cropping?.Dispose();
        GC.SuppressFinalize(this);
    }
}
