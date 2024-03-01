using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.FishPond;

[ClassReader]
public record FishPondData(List<string> RequiredTags, int SpawnTime, List<FishPondReward> ProducedItems, Dictionary<int, List<string>> PopulationGates);