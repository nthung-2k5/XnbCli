using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.Movies;

[ClassReader]
public record SpecialResponses(
    CharacterResponse BeforeMovie,
    CharacterResponse DuringMovie,
    CharacterResponse AfterMovie);
