namespace Xnb.Types.StardewValley.HomeRenovations;

public record HomeRenovation(string TextStrings, string AnimationType, bool CheckForObstructions, List<RenovationValue> Requirements, List<RenovationValue> RenovateActions, List<RectGroup> RectGroups, string SpecialRect);
