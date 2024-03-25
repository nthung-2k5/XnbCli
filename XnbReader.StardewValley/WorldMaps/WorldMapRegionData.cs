namespace XnbReader.StardewValley.WorldMaps;

public record WorldMapRegionData(List<WorldMapTextureData> BaseTexture, Dictionary<string,string> MapNeighborIdAliases, List<WorldMapAreaData> MapAreas);
