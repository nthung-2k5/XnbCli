namespace XnbReader.StardewValley.Movies;

public record MovieData(string Id, List<Season> Seasons, int? YearModulus, int? YearRemainder, string Texture, int SheetIndex, string Title, string Description, List<string> Tags, List<MovieCranePrizeData> CranePrizes, List<int> ClearDefaultCranePrizeGroups, List<MovieScene> Scenes, Dictionary<string,string> CustomFields);
