using Xnb.Types;
using Xnb.Types.StardewValley;
using Xnb.Types.StardewValley.Crafting;
using Xnb.Types.StardewValley.FishPond;
using Xnb.Types.StardewValley.HomeRenovations;
using Xnb.Types.StardewValley.Movies;
using XnbReader;

namespace Xnb.Reader;

[XnbReadable(typeof(string))]
[XnbReadable(typeof(ModFarmType))]
[XnbReadable(typeof(ModLanguage))]
[XnbReadable(typeof(ConcessionItemData))]
[XnbReadable(typeof(ConcessionTaste))]
[XnbReadable(typeof(FishPondData))]
[XnbReadable(typeof(HomeRenovation))]
[XnbReadable(typeof(MovieCharacterReaction))]
[XnbReadable(typeof(MovieData))]
[XnbReadable(typeof(RandomBundleData))]
[XnbReadable(typeof(SpecialOrderData))]
[XnbReadable(typeof(TailorItemRecipe))]
[XnbReadable(typeof(ModWallpaperOrFlooring))]
[XnbReadable(typeof(SpriteFont), ReaderOverride = XnbReadableAttribute.DefaultReaderOverride)]
[XnbReadable(typeof(Texture2D), ReaderOverride = XnbReadableAttribute.DefaultReaderOverride)]
[XnbReadable(typeof(TBin), ReaderOverride = "xTile.Pipeline.TideReader")]
[XnbReadable(typeof(Effect), ReaderOverride = XnbReadableAttribute.DefaultReaderOverride)]
[XnbReadable(typeof(BmFont), ReaderOverride = "BmFont.XmlSourceReader")]
//[XnbReadable(typeof(ConcessionItemData))]
public partial class ContentReader;
