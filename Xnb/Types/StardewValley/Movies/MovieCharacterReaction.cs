using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.Movies;

[ClassReader]
public record MovieCharacterReaction(string NPCName, List<MovieReaction> Reactions);
