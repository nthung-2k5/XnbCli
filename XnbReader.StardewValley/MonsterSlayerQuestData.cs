namespace XnbReader.StardewValley;

public record MonsterSlayerQuestData(string DisplayName, List<string> Targets, int Count, string RewardItemId, int RewardItemPrice, string RewardDialogue, string RewardDialogueFlag, string RewardFlag, string RewardFlagAll, string RewardMail, string RewardMailAll, Dictionary<string,string> CustomFields);
