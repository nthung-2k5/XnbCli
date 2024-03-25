namespace XnbReader.StardewValley;

public record PlantableRule(string Id, string Condition, PlantableRuleContext PlantedIn, PlantableResult Result, string DeniedMessage);
