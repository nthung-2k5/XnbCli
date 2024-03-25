using System.Text.Json.Serialization;
using XnbCli.TextureHelper;
using XnbReader.FileFormat;
using XnbReader.StardewValley;
using XnbReader.StardewValley.BigCraftables;
using XnbReader.StardewValley.Buffs;
using XnbReader.StardewValley.Buildings;
using XnbReader.StardewValley.Bundles;
using XnbReader.StardewValley.Characters;
using XnbReader.StardewValley.Crafting;
using XnbReader.StardewValley.Crops;
using XnbReader.StardewValley.FarmAnimals;
using XnbReader.StardewValley.Fences;
using XnbReader.StardewValley.FishPonds;
using XnbReader.StardewValley.FloorsAndPaths;
using XnbReader.StardewValley.FruitTrees;
using XnbReader.StardewValley.GarbageCans;
using XnbReader.StardewValley.GiantCrops;
using XnbReader.StardewValley.HomeRenovations;
using XnbReader.StardewValley.LocationContexts;
using XnbReader.StardewValley.Locations;
using XnbReader.StardewValley.Machines;
using XnbReader.StardewValley.MakeoverOutfits;
using XnbReader.StardewValley.Minecarts;
using XnbReader.StardewValley.Movies;
using XnbReader.StardewValley.Museum;
using XnbReader.StardewValley.Objects;
using XnbReader.StardewValley.Pants;
using XnbReader.StardewValley.Pets;
using XnbReader.StardewValley.Powers;
using XnbReader.StardewValley.Shirts;
using XnbReader.StardewValley.Shops;
using XnbReader.StardewValley.SpecialOrders;
using XnbReader.StardewValley.Tools;
using XnbReader.StardewValley.Weapons;
using XnbReader.StardewValley.Weddings;
using XnbReader.StardewValley.WildTrees;
using XnbReader.StardewValley.WorldMaps;

namespace XnbCli;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(XnbFile))]
[JsonSerializable(typeof(ExternalSpriteFont))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<int, string>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<ModFarmType>))]
[JsonSerializable(typeof(List<ModLanguage>))]
[JsonSerializable(typeof(List<ModWallpaperOrFlooring>))]
[JsonSerializable(typeof(Dictionary<string, AudioCueData>))]
[JsonSerializable(typeof(Dictionary<string, BigCraftableData>))]
[JsonSerializable(typeof(Dictionary<string, BuffData>))]
[JsonSerializable(typeof(Dictionary<string, BuildingData>))]
[JsonSerializable(typeof(Dictionary<string, CharacterData>))]
[JsonSerializable(typeof(List<ConcessionItemData>))]
[JsonSerializable(typeof(List<ConcessionTaste>))]
[JsonSerializable(typeof(Dictionary<string, CropData>))]
[JsonSerializable(typeof(Dictionary<string, FarmAnimalData>))]
[JsonSerializable(typeof(Dictionary<string, FenceData>))]
[JsonSerializable(typeof(List<FishPondData>))]
[JsonSerializable(typeof(Dictionary<string, FloorPathData>))]
[JsonSerializable(typeof(Dictionary<string, FruitTreeData>))]
[JsonSerializable(typeof(GarbageCanData))]
[JsonSerializable(typeof(Dictionary<string, GiantCropData>))]
[JsonSerializable(typeof(Dictionary<string, HomeRenovation>))]
[JsonSerializable(typeof(Dictionary<string, IncomingPhoneCallData>))]
[JsonSerializable(typeof(Dictionary<string, JukeboxTrackData>))]
[JsonSerializable(typeof(Dictionary<string, LocationContextData>))]
[JsonSerializable(typeof(Dictionary<string, LocationData>))]
[JsonSerializable(typeof(Dictionary<string, MachineData>))]
[JsonSerializable(typeof(List<MakeoverOutfit>))]
[JsonSerializable(typeof(Dictionary<string, MannequinData>))]
[JsonSerializable(typeof(Dictionary<string, MinecartNetworkData>))]
[JsonSerializable(typeof(Dictionary<string, MonsterSlayerQuestData>))]
[JsonSerializable(typeof(List<MovieData>))]
[JsonSerializable(typeof(List<MovieCharacterReaction>))]
[JsonSerializable(typeof(Dictionary<string, MuseumRewards>))]
[JsonSerializable(typeof(Dictionary<string, ObjectData>))]
[JsonSerializable(typeof(Dictionary<string, PantsData>))]
[JsonSerializable(typeof(Dictionary<string, PassiveFestivalData>))]
[JsonSerializable(typeof(Dictionary<string, PetData>))]
[JsonSerializable(typeof(Dictionary<string, PowersData>))]
[JsonSerializable(typeof(List<RandomBundleData>))]
[JsonSerializable(typeof(Dictionary<string, ShirtData>))]
[JsonSerializable(typeof(Dictionary<string, ShopData>))]
[JsonSerializable(typeof(Dictionary<string, SpecialOrderData>))]
[JsonSerializable(typeof(List<TailorItemRecipe>))]
[JsonSerializable(typeof(Dictionary<string, ToolData>))]
[JsonSerializable(typeof(List<TriggerActionData>))]
[JsonSerializable(typeof(Dictionary<string, TrinketData>))]
[JsonSerializable(typeof(Dictionary<string, WeaponData>))]
[JsonSerializable(typeof(WeddingData))]
[JsonSerializable(typeof(Dictionary<string, WildTreeData>))]
[JsonSerializable(typeof(Dictionary<string, WorldMapRegionData>))]
public partial class JsonContextForXnbReader : JsonSerializerContext;
