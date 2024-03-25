namespace XnbReader.StardewValley.Shops;

public record ShopOwnerData(string Id, string Name, string Condition, string Portrait, List<ShopDialogueData> Dialogues, bool RandomizeDialogueOnOpen, string ClosedMessage);
