namespace Xnb.Types.StardewValley.FishPond;

public record FishPondData(List<string> RequiredTags, int SpawnTime, List<FishPondReward> ProducedItems, Dictionary<int, List<string>> PopulationGates);
