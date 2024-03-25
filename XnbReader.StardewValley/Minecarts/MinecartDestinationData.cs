namespace XnbReader.StardewValley.Minecarts;

public record MinecartDestinationData(string Id, string DisplayName, string Condition, int Price, string BuyTicketMessage, string TargetLocation, System.Drawing.Point TargetTile, string TargetDirection, Dictionary<string,string> CustomFields);
