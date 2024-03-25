namespace XnbReader.StardewValley.WorldMaps;

public record WorldMapAreaPositionData(string Id, string Condition, string LocationContext, string LocationName, List<string> LocationNames, System.Drawing.Rectangle TileArea, System.Drawing.Rectangle? ExtendedTileArea, System.Drawing.Rectangle MapPixelArea, string ScrollText, List<WorldMapAreaPositionScrollTextZoneData> ScrollTextZones);
