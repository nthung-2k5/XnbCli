namespace XnbReader.StardewValley.Minecarts;

public record MinecartNetworkData(string UnlockCondition, string LockedMessage, string ChooseDestinationMessage, string BuyTicketMessage, List<MinecartDestinationData> Destinations);
