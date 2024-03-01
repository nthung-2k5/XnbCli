using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.Crafting;

[ClassReader]
public record TailorItemRecipe(List<string> FirstItemTags, List<string> SecondItemTags, bool SpendRightItem, int CraftedItemId, List<string> CraftedItemIDs, string CraftedItemColor);