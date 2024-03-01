using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.Movies;

[ClassReader]
public record ConcessionItemData(
    int ID,
    string Name,
    string DisplayName,
    string Description,
    int Price,
    List<string> ItemTags);
