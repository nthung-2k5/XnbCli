namespace XnbReader.StardewValley.Pants;

public record PantsData(string Name, string DisplayName, string Description, int Price, string Texture, int SpriteIndex, string DefaultColor, bool CanBeDyed, bool IsPrismatic, bool CanChooseDuringCharacterCustomization, Dictionary<string,string> CustomFields);
