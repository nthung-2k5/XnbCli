namespace XnbReader.StardewValley.Characters;

public record CharacterAppearanceData(string Id, string Condition, Season? Season, bool Indoors, bool Outdoors, string Portrait, string Sprite, bool IsIslandAttire, int Precedence, int Weight);
