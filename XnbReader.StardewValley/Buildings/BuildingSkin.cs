namespace XnbReader.StardewValley.Buildings;

public record BuildingSkin(string Id, string Name, string Description, string Texture, string Condition, int? BuildDays, int? BuildCost, List<BuildingMaterial> BuildMaterials, bool ShowAsSeparateConstructionEntry, Dictionary<string,string> Metadata);
