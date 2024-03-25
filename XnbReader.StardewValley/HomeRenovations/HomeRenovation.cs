namespace XnbReader.StardewValley.HomeRenovations;

public record HomeRenovation(string TextStrings, string AnimationType, bool CheckForObstructions, int Price, string RoomId, List<RenovationValue> Requirements, List<RenovationValue> RenovateActions, List<RectGroup> RectGroups, string SpecialRect, Dictionary<string,string> CustomFields);
