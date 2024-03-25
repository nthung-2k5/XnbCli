namespace XnbReader.StardewValley.WildTrees;

public record WildTreeData(List<WildTreeTextureData> Textures, string SeedItemId, bool SeedPlantable, float GrowthChance, float FertilizedGrowthChance, float SeedSpreadChance, float SeedOnShakeChance, float SeedOnChopChance, bool DropWoodOnChop, bool DropHardwoodOnLumberChop, bool IsLeafy, bool IsLeafyInWinter, bool IsLeafyInFall, List<PlantableRule> PlantableLocationRules, bool GrowsInWinter, bool IsStumpDuringWinter, bool AllowWoodpeckers, bool UseAlternateSpriteWhenNotShaken, bool UseAlternateSpriteWhenSeedReady, string DebrisColor, List<WildTreeSeedDropItemData> SeedDropItems, List<WildTreeChopItemData> ChopItems, List<WildTreeTapItemData> TapItems, List<WildTreeItemData> ShakeItems, Dictionary<string,string> CustomFields, bool GrowsMoss);
