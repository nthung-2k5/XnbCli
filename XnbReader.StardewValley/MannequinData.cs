namespace XnbReader.StardewValley;

public record MannequinData(string ID, string DisplayName, string Description, string Texture, string FarmerTexture, int SheetIndex, bool DisplaysClothingAsMale, bool Cursed, Dictionary<string,string> CustomFields);
