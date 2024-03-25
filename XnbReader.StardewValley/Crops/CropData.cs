namespace XnbReader.StardewValley.Crops;

public record CropData(List<Season> Seasons, List<int> DaysInPhase, int RegrowDays, bool IsRaised, bool IsPaddyCrop, bool NeedsWatering, List<PlantableRule> PlantableLocationRules, string HarvestItemId, int HarvestMinStack, int HarvestMaxStack, float HarvestMaxIncreasePerFarmingLevel, double ExtraHarvestChance, HarvestMethod HarvestMethod, int HarvestMinQuality, int? HarvestMaxQuality, List<string> TintColors, string Texture, int SpriteIndex, bool CountForMonoculture, bool CountForPolyculture, Dictionary<string,string> CustomFields);
