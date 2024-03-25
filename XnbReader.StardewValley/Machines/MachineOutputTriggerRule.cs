namespace XnbReader.StardewValley.Machines;

public record MachineOutputTriggerRule(string Id, MachineOutputTrigger Trigger, string RequiredItemId, List<string> RequiredTags, int RequiredCount, string Condition);
