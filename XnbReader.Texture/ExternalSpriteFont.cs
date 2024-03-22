using System.Drawing;
using System.Numerics;
using XnbReader.Types;

namespace XnbReader.Texture;

public record ExternalSpriteFont(string Texture, EnumerableMemoryOwner<Rectangle> Glyphs, EnumerableMemoryOwner<Rectangle> Cropping, char[] CharMap, int LineSpacing, float Spacing, Vector3[] Kerning, char? DefaultCharacter) : IDisposable
{
    public ExternalSpriteFont(SpriteFont spriteFont, string file): this(file, spriteFont.Glyphs, spriteFont.Cropping, spriteFont.CharMap, spriteFont.LineSpacing, spriteFont.Spacing, spriteFont.Kerning, spriteFont.DefaultCharacter) { }
    public void Dispose()
    {
        Glyphs.Dispose();
        Cropping.Dispose();
        GC.SuppressFinalize(this);
    }
}
