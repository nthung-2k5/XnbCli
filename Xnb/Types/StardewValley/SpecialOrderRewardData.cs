using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley;

[ClassReader]
public record SpecialOrderRewardData(string Type, Dictionary<string, string> Data);
