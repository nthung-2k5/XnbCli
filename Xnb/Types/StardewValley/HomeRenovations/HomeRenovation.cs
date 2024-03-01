using System.Drawing;
using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley.HomeRenovations;

[ClassReader]
public record HomeRenovation(
    string TextStrings,
    string AnimationType,
    bool CheckForObstructions,
    List<RenovationValue> Requirements,
    List<RenovationValue> RenovateActions,
    List<RectGroup> RectGroups,
    string SpecialRect
);
