using NetEscapades.EnumGenerators;

namespace XnbReader.MonoGameShims;

/// <summary>
///     Legacy surface formats for XNA Game Studio 1.0, 2.0, 3.0, 3.1
/// </summary>
[EnumExtensions]
internal enum SurfaceFormatLegacy
{
    Color = 1,
    Bgr32,
    Bgra1010102,
    Rgba32,
    Rgb32,
    Rgba1010102,
    Rg32,
    Rgba64,
    Bgr565,
    Bgra5551,
    Bgr555,
    Bgra4444,
    Bgr444,
    Bgra2338,
    Alpha8,
    Bgr233,
    Bgr24,
    NormalizedByte2,
    NormalizedByte4,
    NormalizedShort2,
    NormalizedShort4,
    Single,
    Vector2,
    Vector4,
    HalfSingle,
    HalfVector2,
    HalfVector4,
    Dxt1,
    Dxt2,
    Dxt3,
    Dxt4,
    Dxt5,
    Luminance8,
    Luminance16,
    LuminanceAlpha8,
    LuminanceAlpha16,
    Palette8,
    PaletteAlpha16,
    NormalizedLuminance16,
    NormalizedLuminance32,
    NormalizedAlpha1010102,
    NormalizedByte2Computed,
    VideoYuYv,
    VideoUyVy,
    VideoGrGb,
    VideoRgBg,
    Multi2Bgra32,
    Depth24Stencil8,
    Depth24Stencil8Single,
    Depth24Stencil4,
    Depth24,
    Depth32,
    Depth16 = 54,
    Depth15Stencil1 = 56,
    Unknown = -1
}

