namespace XnbReader.StardewValley.BigCraftables;

public record BigCraftableData(string Name, string DisplayName, string Description, int Price, int Fragility, bool CanBePlacedOutdoors, bool CanBePlacedIndoors, bool IsLamp, string Texture, int SpriteIndex, List<string> ContextTags, Dictionary<string,string> CustomFields);
