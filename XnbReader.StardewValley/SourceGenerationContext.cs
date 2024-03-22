using System.Text.Json.Serialization;
using XnbReader.FileFormat;
using XnbReader.StardewValley.Crafting;
using XnbReader.StardewValley.FishPond;
using XnbReader.StardewValley.HomeRenovations;
using XnbReader.StardewValley.Movies;
using XnbReader.Texture;

namespace XnbReader.StardewValley;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
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
public partial class SourceGenerationContext : JsonSerializerContext;
