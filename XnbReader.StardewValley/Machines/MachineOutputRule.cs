namespace XnbReader.StardewValley.Machines;

public record MachineOutputRule(string Id, List<MachineOutputTriggerRule> Triggers, bool UseFirstValidOutput, List<MachineItemOutput> OutputItem, int MinutesUntilReady, int DaysUntilReady, string InvalidCountMessage, bool RecalculateOnCollect);
