using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley;

[ClassReader]
public record ModWallpaperOrFlooring(string ID, string Texture, bool IsFlooring, int Count);