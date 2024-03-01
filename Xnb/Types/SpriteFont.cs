using System.Drawing;
using System.Numerics;
using System.Text.Json.Serialization;
using CommunityToolkit.HighPerformance.Buffers;

namespace Xnb.Types;

public record SpriteFont(
    char[] CharMap,
    int LineSpacing,
    float Spacing,
    Vector3[] Kerning,
    char? DefaultCharacter) : ExplicitType<Texture2D>, IDisposable
{
    [JsonIgnore]
    public MemoryOwner<Rectangle> GlyphsOwner { get; init; }
    [JsonIgnore]
    public MemoryOwner<Rectangle> CroppingOwner { get; init; }
    public Rectangle[] Glyphs => GlyphsOwner.DangerousGetArray().Array;
    public Rectangle[] Cropping => CroppingOwner.DangerousGetArray().Array;

    public void Dispose()
    {
        Data?.Dispose();
        GlyphsOwner?.Dispose();
        CroppingOwner?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public record ExternalSpriteFont(
    string Texture,
    char[] CharMap,
    int LineSpacing,
    float Spacing,
    Vector3[] Kerning,
    char? DefaultCharacter) : IDisposable
{
    public ExternalSpriteFont(SpriteFont spr, string texName) : this(texName, spr.CharMap, spr.LineSpacing, spr.Spacing,
                                                                     spr.Kerning, spr.DefaultCharacter)
    {
        GlyphsOwner = spr.GlyphsOwner;
        CroppingOwner = spr.CroppingOwner;
    }
    [JsonIgnore]
    public MemoryOwner<Rectangle> GlyphsOwner { get; init; }
    [JsonIgnore]
    public MemoryOwner<Rectangle> CroppingOwner { get; init; }
    public Rectangle[] Glyphs => GlyphsOwner.DangerousGetArray().Array;
    public Rectangle[] Cropping => CroppingOwner.DangerousGetArray().Array;

    public void Dispose()
    {
        GlyphsOwner?.Dispose();
        CroppingOwner?.Dispose();
        GC.SuppressFinalize(this);
    }
}