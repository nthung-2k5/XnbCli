using XnbReader.MonoGameShims;
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

namespace XnbReader.StardewValley;

[XnbReadable(typeof(Dictionary<string, string>))]
[XnbReadable(typeof(Dictionary<int, string>))]
[XnbReadable(typeof(List<string>))]
[XnbReadable(typeof(List<ModFarmType>))]
[XnbReadable(typeof(List<ModLanguage>))]
[XnbReadable(typeof(List<ModWallpaperOrFlooring>))]
[XnbReadable(typeof(Dictionary<string, AudioCueData>))]
[XnbReadable(typeof(Dictionary<string, BigCraftableData>))]
[XnbReadable(typeof(Dictionary<string, BuffData>))]
[XnbReadable(typeof(Dictionary<string, BuildingData>))]
[XnbReadable(typeof(Dictionary<string, CharacterData>))]
[XnbReadable(typeof(List<ConcessionItemData>))]
[XnbReadable(typeof(List<ConcessionTaste>))]
[XnbReadable(typeof(Dictionary<string, CropData>))]
[XnbReadable(typeof(Dictionary<string, FarmAnimalData>))]
[XnbReadable(typeof(Dictionary<string, FenceData>))]
[XnbReadable(typeof(List<FishPondData>))]
[XnbReadable(typeof(Dictionary<string, FloorPathData>))]
[XnbReadable(typeof(Dictionary<string, FruitTreeData>))]
[XnbReadable(typeof(GarbageCanData), Reflective = true)]
[XnbReadable(typeof(Dictionary<string, GiantCropData>))]
[XnbReadable(typeof(Dictionary<string, HomeRenovation>))]
[XnbReadable(typeof(Dictionary<string, IncomingPhoneCallData>))]
[XnbReadable(typeof(Dictionary<string, JukeboxTrackData>))]
[XnbReadable(typeof(Dictionary<string, LocationContextData>))]
[XnbReadable(typeof(Dictionary<string, LocationData>))]
[XnbReadable(typeof(Dictionary<string, MachineData>))]
[XnbReadable(typeof(List<MakeoverOutfit>))]
[XnbReadable(typeof(Dictionary<string, MannequinData>))]
[XnbReadable(typeof(Dictionary<string, MinecartNetworkData>))]
[XnbReadable(typeof(Dictionary<string, MonsterSlayerQuestData>))]
[XnbReadable(typeof(List<MovieData>))]
[XnbReadable(typeof(List<MovieCharacterReaction>))]
[XnbReadable(typeof(Dictionary<string, MuseumRewards>))]
[XnbReadable(typeof(Dictionary<string, ObjectData>))]
[XnbReadable(typeof(Dictionary<string, PantsData>))]
[XnbReadable(typeof(Dictionary<string, PassiveFestivalData>))]
[XnbReadable(typeof(Dictionary<string, PetData>))]
[XnbReadable(typeof(Dictionary<string, PowersData>))]
[XnbReadable(typeof(List<RandomBundleData>))]
[XnbReadable(typeof(Dictionary<string, ShirtData>))]
[XnbReadable(typeof(Dictionary<string, ShopData>))]
[XnbReadable(typeof(Dictionary<string, SpecialOrderData>))]
[XnbReadable(typeof(List<TailorItemRecipe>))]
[XnbReadable(typeof(Dictionary<string, ToolData>))]
[XnbReadable(typeof(List<TriggerActionData>))]
[XnbReadable(typeof(Dictionary<string, TrinketData>))]
[XnbReadable(typeof(Dictionary<string, WeaponData>))]
[XnbReadable(typeof(WeddingData), Reflective = true)]
[XnbReadable(typeof(Dictionary<string, WildTreeData>))]
[XnbReadable(typeof(Dictionary<string, WorldMapRegionData>))]
[XnbReadable(typeof(SpriteFont))]
[XnbReadable(typeof(Texture2D))]
[XnbReadable(typeof(TBin), ReaderOverride = "xTile.Pipeline.TideReader")]
[XnbReadable(typeof(Effect))]
[XnbReadable(typeof(BmFont), ReaderOverride = "BmFont.XmlSourceReader")]
public partial class ContentReader: XnbContentReader;
