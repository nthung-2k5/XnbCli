using XnbReader.MonoGameShims;
using XnbReader.StardewValley.Crafting;
using XnbReader.StardewValley.FishPond;
using XnbReader.StardewValley.HomeRenovations;
using XnbReader.StardewValley.Movies;
using XnbReader.Texture;

namespace XnbReader.StardewValley;

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
public partial class ContentReader: XnbContentReader;
