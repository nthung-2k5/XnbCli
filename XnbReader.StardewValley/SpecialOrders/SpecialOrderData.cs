namespace XnbReader.StardewValley.SpecialOrders;

public record SpecialOrderData(string Name, string Requester, QuestDuration Duration, bool Repeatable, string RequiredTags, string Condition, string OrderType, string SpecialRule, string Text, string ItemToRemoveOnEnd, string MailToRemoveOnEnd, List<RandomizedElement> RandomizedElements, List<SpecialOrderObjectiveData> Objectives, List<SpecialOrderRewardData> Rewards, Dictionary<string,string> CustomFields);
