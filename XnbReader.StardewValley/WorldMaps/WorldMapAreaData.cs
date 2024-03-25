namespace XnbReader.StardewValley.WorldMaps;

public record WorldMapAreaData(string Id, string Condition, System.Drawing.Rectangle PixelArea, string ScrollText, List<WorldMapTextureData> Textures, List<WorldMapTooltipData> Tooltips, List<WorldMapAreaPositionData> WorldPositions, Dictionary<string,string> CustomFields);
