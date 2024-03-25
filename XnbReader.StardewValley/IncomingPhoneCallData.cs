namespace XnbReader.StardewValley;

public record IncomingPhoneCallData(string TriggerCondition, string RingCondition, string FromNpc, string FromPortrait, string FromDisplayName, string Dialogue, bool IgnoreBaseChance, string SimpleDialogueSplitBy, int MaxCalls, Dictionary<string,string> CustomFields);
