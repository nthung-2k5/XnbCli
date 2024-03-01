using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.Movies;

[ClassReader]
public record ConcessionTaste(string Name, List<string> LovedTags, List<string> LikedTags, List<string> DislikedTags);