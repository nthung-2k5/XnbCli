using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.Movies;

[ClassReader]
public record MovieScene(int Image, string Music, string Sound, int MessageDelay, string Script, string Text, bool Shake,
              string ResponsePoint, string ID);
