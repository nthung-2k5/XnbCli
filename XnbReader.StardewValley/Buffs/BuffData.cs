namespace XnbReader.StardewValley.Buffs;

public record BuffData(string DisplayName, string Description, bool IsDebuff, string GlowColor, int Duration, int MaxDuration, string IconTexture, int IconSpriteIndex, BuffAttributesData Effects, List<string> ActionsOnApply, Dictionary<string,string> CustomFields);
