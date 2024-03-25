namespace XnbReader.StardewValley.Shirts;

public record ShirtData(string Name, string DisplayName, string Description, int Price, string Texture, int SpriteIndex, string DefaultColor, bool CanBeDyed, bool IsPrismatic, bool HasSleeves, bool CanChooseDuringCharacterCustomization, Dictionary<string,string> CustomFields);
