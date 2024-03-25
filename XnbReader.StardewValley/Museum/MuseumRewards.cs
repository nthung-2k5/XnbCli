namespace XnbReader.StardewValley.Museum;

public record MuseumRewards(List<MuseumDonationRequirement> TargetContextTags, string RewardItemId, int RewardItemCount, bool RewardItemIsSpecial, bool RewardItemIsRecipe, List<string> RewardActions, bool FlagOnCompletion, Dictionary<string,string> CustomFields);
