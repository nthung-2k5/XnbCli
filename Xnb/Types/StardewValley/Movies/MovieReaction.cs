using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.Movies;

[ClassReader]
public record MovieReaction(
    string Tag,
    string Response,
    List<string> Whitelist,
    SpecialResponses SpecialResponses,
    string ID);
