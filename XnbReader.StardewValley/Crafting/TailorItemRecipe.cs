namespace XnbReader.StardewValley.Crafting;

public record TailorItemRecipe(string Id, List<string> FirstItemTags, List<string> SecondItemTags, bool SpendRightItem, string CraftedItemId, List<string> CraftedItemIds, string CraftedItemIdFeminine);
