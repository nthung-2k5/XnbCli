namespace XnbReader.StardewValley;

public record GenericSpawnItemData(string Id, string ItemId, List<string> RandomItemId, int? MaxItems, int MinStack, int MaxStack, int Quality, string ObjectInternalName, string ObjectDisplayName, int ToolUpgradeLevel, bool IsRecipe, List<QuantityModifier> StackModifiers, QuantityModifier.QuantityModifierMode StackModifierMode, List<QuantityModifier> QualityModifiers, QuantityModifier.QuantityModifierMode QualityModifierMode, Dictionary<string,string> ModData, string PerItemCondition): ISpawnItemData;
