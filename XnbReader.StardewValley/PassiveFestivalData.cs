namespace XnbReader.StardewValley;

public record PassiveFestivalData(string DisplayName, string Condition, bool ShowOnCalendar, Season Season, int StartDay, int EndDay, int StartTime, string StartMessage, bool OnlyShowMessageOnFirstDay, Dictionary<string,string> MapReplacements, string DailySetupMethod, string CleanupMethod, Dictionary<string,string> CustomFields);
