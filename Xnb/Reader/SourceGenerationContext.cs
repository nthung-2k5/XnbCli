using System.Text.Json.Serialization;
using Xnb.Types;
using Xnb.Types.StardewValley;
using Xnb.Types.StardewValley.Crafting;
using Xnb.Types.StardewValley.FishPond;
using Xnb.Types.StardewValley.HomeRenovations;
using Xnb.Types.StardewValley.Movies;

namespace Xnb.Reader;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(XnbFile))]

[JsonSerializable(typeof(Dictionary<int, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(List<string>))]

[JsonSerializable(typeof(ExternalSpriteFont))]

[JsonSerializable(typeof(List<ModFarmType>))]
[JsonSerializable(typeof(List<ModLanguage>))]
[JsonSerializable(typeof(List<ConcessionItemData>))]
[JsonSerializable(typeof(List<ConcessionTaste>))]
[JsonSerializable(typeof(List<FishPondData>))]
[JsonSerializable(typeof(Dictionary<string, HomeRenovation>))]
[JsonSerializable(typeof(List<MovieCharacterReaction>))]
[JsonSerializable(typeof(Dictionary<string, MovieData>))]
[JsonSerializable(typeof(List<RandomBundleData>))]
[JsonSerializable(typeof(Dictionary<string, SpecialOrderData>))]
[JsonSerializable(typeof(List<TailorItemRecipe>))]
[JsonSerializable(typeof(List<ModWallpaperOrFlooring>))]
internal partial class SourceGenerationContext : JsonSerializerContext;
