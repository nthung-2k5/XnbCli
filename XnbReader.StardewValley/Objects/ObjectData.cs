namespace XnbReader.StardewValley.Objects;

public record ObjectData(string Name, string DisplayName, string Description, string Type, int Category, int Price, string Texture, int SpriteIndex, int Edibility, bool IsDrink, List<ObjectBuffData> Buffs, bool GeodeDropsDefaultItems, List<ObjectGeodeDropData> GeodeDrops, Dictionary<string,float> ArtifactSpotChances, bool ExcludeFromFishingCollection, bool ExcludeFromShippingCollection, bool ExcludeFromRandomSale, List<string> ContextTags, Dictionary<string,string> CustomFields);
