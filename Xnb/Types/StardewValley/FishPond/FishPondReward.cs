using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.FishPond;

[ClassReader]
public record FishPondReward(int RequiredPopulation, float Chance, int ItemId, int MinQuantity, int MaxQuantity);
