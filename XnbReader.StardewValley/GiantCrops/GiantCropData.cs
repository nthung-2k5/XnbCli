namespace XnbReader.StardewValley.GiantCrops;

public record GiantCropData(string FromItemId, List<GiantCropHarvestItemData> HarvestItems, string Texture, System.Drawing.Point TexturePosition, System.Drawing.Point TileSize, int Health, float Chance, string Condition, Dictionary<string,string> CustomFields);
