namespace XnbReader.StardewValley.Tools;

public record ToolData(string ClassName, string Name, int AttachmentSlots, int SalePrice, string DisplayName, string Description, string Texture, int SpriteIndex, int MenuSpriteIndex, int UpgradeLevel, bool ApplyUpgradeLevelToDisplayName, string ConventionalUpgradeFrom, List<ToolUpgradeData> UpgradeFrom, bool CanBeLostOnDeath, Dictionary<string,string> SetProperties, Dictionary<string,string> ModData, Dictionary<string,string> CustomFields);
