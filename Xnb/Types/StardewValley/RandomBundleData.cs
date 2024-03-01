using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley;

[ClassReader]
public record RandomBundleData(
    string AreaName,
    string Keys,
    List<BundleSetData> BundleSets,
    List<BundleData> Bundles);