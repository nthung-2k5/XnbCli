using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley;

[ClassReader]
public record ModFarmType(
    string ID,
    string TooltipStringPath,
    string MapName,
    string IconTexture,
    string WorldMapTexture,
    Dictionary<string, string> ModData);
