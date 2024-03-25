namespace XnbReader.StardewValley.Weddings;

public record WeddingData(Dictionary<string,string> EventScript, Dictionary<string,WeddingAttendeeData> Attendees);
