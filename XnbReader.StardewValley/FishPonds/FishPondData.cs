namespace XnbReader.StardewValley.FishPonds;

public record FishPondData(string Id, List<string> RequiredTags, int Precedence, int SpawnTime, List<FishPondReward> ProducedItems, Dictionary<int,List<string>> PopulationGates, Dictionary<string,string> CustomFields);
