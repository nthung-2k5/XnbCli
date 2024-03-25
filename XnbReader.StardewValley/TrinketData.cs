namespace XnbReader.StardewValley;

public record TrinketData(string ID, string DisplayName, string Description, string Texture, int SheetIndex, string TrinketEffectClass, bool DropsNaturally, bool CanBeReforged, Dictionary<string,string> TrinketMetadata);
