namespace XnbReader.StardewValley.Shops;

public record ShopData(bool? ApplyProfitMargins, int Currency, StackSizeVisibility? StackSizeVisibility, string OpenSound, string PurchaseSound, string PurchaseRepeatSound, List<QuantityModifier> PriceModifiers, QuantityModifier.QuantityModifierMode PriceModifierMode, List<ShopOwnerData> Owners, List<ShopThemeData> VisualTheme, List<string> SalableItemTags, List<ShopItemData> Items, Dictionary<string,string> CustomFields);
