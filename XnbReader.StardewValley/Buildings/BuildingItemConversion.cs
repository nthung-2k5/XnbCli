namespace XnbReader.StardewValley.Buildings;

public record BuildingItemConversion(string Id, List<string> RequiredTags, int RequiredCount, int MaxDailyConversions, string SourceChest, string DestinationChest, List<GenericSpawnItemDataWithCondition> ProducedItems);
