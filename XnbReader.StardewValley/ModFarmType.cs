namespace XnbReader.StardewValley;

public record ModFarmType(string Id, string TooltipStringPath, string MapName, string IconTexture, string WorldMapTexture, bool SpawnMonstersByDefault, Dictionary<string,string> ModData, Dictionary<string,string> CustomFields);
