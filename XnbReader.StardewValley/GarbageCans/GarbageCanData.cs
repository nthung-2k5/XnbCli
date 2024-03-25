namespace XnbReader.StardewValley.GarbageCans;

public record GarbageCanData(float DefaultBaseChance, List<GarbageCanItemData> BeforeAll, List<GarbageCanItemData> AfterAll, Dictionary<string,GarbageCanEntryData> GarbageCans);
