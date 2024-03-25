namespace XnbReader.StardewValley;

public record ModLanguage(string Id, string LanguageCode, string ButtonTexture, bool UseLatinFont, string FontFile, float FontPixelZoom, bool FontApplyYOffset, int SmallFontLineSpacing, bool UseGenderedCharacterTranslations, string NumberComma, string TimeFormat, string ClockTimeFormat, string ClockDateFormat, Dictionary<string,string> CustomFields);
