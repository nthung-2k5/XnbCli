namespace XnbReader.StardewValley.Objects;

public record ObjectBuffData(string Id, string BuffId, string IconTexture, int IconSpriteIndex, int Duration, bool IsDebuff, string GlowColor, Buffs.BuffAttributesData CustomAttributes, Dictionary<string,string> CustomFields);
