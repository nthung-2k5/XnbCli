namespace XnbReader.StardewValley.Buildings;

public record BuildingDrawLayer(string Id, string Texture, System.Drawing.Rectangle SourceRect, System.Numerics.Vector2 DrawPosition, bool DrawInBackground, float SortTileOffset, string OnlyDrawIfChestHasContents, int FrameDuration, int FrameCount, int FramesPerRow, System.Drawing.Point AnimalDoorOffset);
