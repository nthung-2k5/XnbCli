using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley;

[ClassReader]
public record SpecialOrderObjectiveData(
    string Type,
    string Text,
    string RequiredCount,
    Dictionary<string, string> Data
);
