namespace XnbReader.StardewValley;

public interface ISpawnItemData
{
    public string ItemId { get; init; }
    public List<string> RandomItemId { get; init; }
    public int? MaxItems { get; init; }
    public int MinStack { get; init; }
    public int MaxStack { get; init; }
    public int Quality { get; init; }
    public string ObjectInternalName { get; init; }
    public string ObjectDisplayName { get; init; }
    public int ToolUpgradeLevel { get; init; }
    public bool IsRecipe { get; init; }
    public List<QuantityModifier> StackModifiers { get; init; }
    public QuantityModifier.QuantityModifierMode StackModifierMode { get; init; }
    public List<QuantityModifier> QualityModifiers { get; init; }
    public QuantityModifier.QuantityModifierMode QualityModifierMode { get; init; }
    public Dictionary<string,string> ModData { get; init; }
    public string PerItemCondition { get; init; }
}
