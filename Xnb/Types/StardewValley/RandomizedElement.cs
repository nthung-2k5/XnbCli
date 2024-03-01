using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley;

[ClassReader]
public record RandomizedElement(string Name, List<RandomizedElementItem> Values);
