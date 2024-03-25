namespace XnbReader.StardewValley;

public record TriggerActionData(string Id, string Trigger, string Condition, bool HostOnly, string Action, List<string> Actions, Dictionary<string,string> CustomFields, bool MarkActionApplied);
