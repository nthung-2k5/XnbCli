namespace XnbReader.StardewValley.Fences;

public record FenceData(int Health, float RepairHealthAdjustmentMinimum, float RepairHealthAdjustmentMaximum, string Texture, string PlacementSound, string RemovalSound, List<string> RemovalToolIds, List<string> RemovalToolTypes, int RemovalDebrisType, System.Numerics.Vector2 HeldObjectDrawOffset, float LeftEndHeldObjectDrawX, float RightEndHeldObjectDrawX);
