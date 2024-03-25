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

[XnbReadable(typeof(string), TypeReader = ContentTypeReader.FullCollection)]
[XnbReadable(typeof(ModFarmType), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(ModLanguage), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(ModWallpaperOrFlooring), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(AudioCueData))]
[XnbReadable(typeof(BigCraftableData))]
[XnbReadable(typeof(BuffData))]
[XnbReadable(typeof(BuildingData))]
[XnbReadable(typeof(CharacterData))]
[XnbReadable(typeof(ConcessionItemData), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(ConcessionTaste), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(CropData))]
[XnbReadable(typeof(FarmAnimalData))]
[XnbReadable(typeof(FenceData))]
[XnbReadable(typeof(FishPondData), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(FloorPathData))]
[XnbReadable(typeof(FruitTreeData))]
[XnbReadable(typeof(GarbageCanData), TypeReader = ContentTypeReader.Reflective)]
[XnbReadable(typeof(GiantCropData))]
[XnbReadable(typeof(HomeRenovation))]
[XnbReadable(typeof(IncomingPhoneCallData))]
[XnbReadable(typeof(JukeboxTrackData))]
[XnbReadable(typeof(LocationContextData))]
[XnbReadable(typeof(LocationData))]
[XnbReadable(typeof(MachineData))]
[XnbReadable(typeof(MakeoverOutfit), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(MannequinData))]
[XnbReadable(typeof(MinecartNetworkData))]
[XnbReadable(typeof(MonsterSlayerQuestData))]
[XnbReadable(typeof(MovieData), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(MovieCharacterReaction), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(MuseumRewards))]
[XnbReadable(typeof(ObjectData))]
[XnbReadable(typeof(PantsData))]
[XnbReadable(typeof(PassiveFestivalData))]
[XnbReadable(typeof(PetData))]
[XnbReadable(typeof(PowersData))]
[XnbReadable(typeof(RandomBundleData), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(ShirtData))]
[XnbReadable(typeof(ShopData))]
[XnbReadable(typeof(SpecialOrderData))]
[XnbReadable(typeof(TailorItemRecipe), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(ToolData))]
[XnbReadable(typeof(TriggerActionData), TypeReader = ContentTypeReader.List)]
[XnbReadable(typeof(TrinketData))]
[XnbReadable(typeof(WeaponData))]
[XnbReadable(typeof(WeddingData), TypeReader = ContentTypeReader.Reflective)]
[XnbReadable(typeof(WildTreeData))]
[XnbReadable(typeof(WorldMapRegionData))]
[XnbReadable(typeof(SpriteFont), TypeReader = ContentTypeReader.Default)]
[XnbReadable(typeof(Texture2D), TypeReader = ContentTypeReader.Default)]
[XnbReadable(typeof(TBin), TypeReader = ContentTypeReader.Custom, ReaderOverride = "xTile.Pipeline.TideReader")]
[XnbReadable(typeof(Effect), TypeReader = ContentTypeReader.Default)]
[XnbReadable(typeof(BmFont), TypeReader = ContentTypeReader.Custom, ReaderOverride = "BmFont.XmlSourceReader")]
public partial class ContentReader: XnbContentReader;
