namespace XnbReader.StardewValley.GarbageCans;

public record GarbageCanEntryData(float BaseChance, List<GarbageCanItemData> Items, Dictionary<string,string> CustomFields);
