namespace XnbReader.StardewValley.FruitTrees;

public record FruitTreeData(string DisplayName, List<Season> Seasons, List<FruitTreeFruitData> Fruit, string Texture, int TextureSpriteRow, Dictionary<string,string> CustomFields, List<PlantableRule> PlantableLocationRules);
