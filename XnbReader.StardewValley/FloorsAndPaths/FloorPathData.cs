namespace XnbReader.StardewValley.FloorsAndPaths;

public record FloorPathData(string Id, string ItemId, string Texture, System.Drawing.Point Corner, string WinterTexture, System.Drawing.Point WinterCorner, string PlacementSound, string RemovalSound, int RemovalDebrisType, string FootstepSound, FloorPathConnectType ConnectType, FloorPathShadowType ShadowType, int CornerSize, float FarmSpeedBuff);
