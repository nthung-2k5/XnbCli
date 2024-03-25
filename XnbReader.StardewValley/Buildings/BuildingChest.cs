namespace XnbReader.StardewValley.Buildings;

public record BuildingChest(string Id, BuildingChestType Type, string Sound, string InvalidItemMessage, string InvalidItemMessageCondition, string InvalidCountMessage, string ChestFullMessage, System.Numerics.Vector2 DisplayTile, float DisplayHeight);
