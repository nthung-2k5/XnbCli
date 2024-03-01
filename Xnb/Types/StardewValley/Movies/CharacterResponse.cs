using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.Movies;

[ClassReader]
public record CharacterResponse(string ResponsePoint, string Script, string Text);
