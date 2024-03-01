using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley;

[ClassReader]
public record BundleData(string Name, int Index, string Sprite, string Color, string Items, int Pick, int RequiredItems, string Reward);
