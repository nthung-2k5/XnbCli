namespace Xnb.Types.StardewValley.Movies;

public record MovieReaction(string Tag, string Response, List<string> Whitelist, SpecialResponses SpecialResponses, string ID);
