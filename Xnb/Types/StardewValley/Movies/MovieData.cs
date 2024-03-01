using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.Movies;

[ClassReader]
public record MovieData(
    string ID,
    int SheetIndex,
    string Title,
    string Description,
    List<string> Tags,
    List<MovieScene> Scenes);
