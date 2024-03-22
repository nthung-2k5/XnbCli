namespace XnbReader.StardewValley.Movies;

public record MovieData(string ID, int SheetIndex, string Title, string Description, List<string> Tags, List<MovieScene> Scenes);
