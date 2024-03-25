namespace XnbReader.StardewValley;

public record AudioCueData(string Id, List<string> FilePaths, string Category, bool StreamedVorbis, bool Looped, bool UseReverb, Dictionary<string,string> CustomFields);
