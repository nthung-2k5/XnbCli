namespace XnbReader.StardewValley.Crafting;

public record TailorItemRecipe(List<string> FirstItemTags, List<string> SecondItemTags, bool SpendRightItem, int CraftedItemId, List<string> CraftedItemIDs, string CraftedItemColor);
