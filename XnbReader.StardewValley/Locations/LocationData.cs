namespace XnbReader.StardewValley.Locations;

public record LocationData(string DisplayName, System.Drawing.Point? DefaultArrivalTile, bool ExcludeFromNpcPathfinding, CreateLocationData CreateOnLoad, List<string> FormerLocationNames, bool? CanPlantHere, bool CanHaveGreenRainSpawns, List<ArtifactSpotDropData> ArtifactSpots, Dictionary<string,FishAreaData> FishAreas, List<SpawnFishData> Fish, List<SpawnForageData> Forage, int MinDailyWeeds, int MaxDailyWeeds, int FirstDayWeedMultiplier, int MinDailyForageSpawn, int MaxDailyForageSpawn, int MaxSpawnedForageAtOnce, double ChanceForClay, List<LocationMusicData> Music, string MusicDefault, MusicContext MusicContext, bool MusicIgnoredInRain, bool MusicIgnoredInSpring, bool MusicIgnoredInSummer, bool MusicIgnoredInFall, bool MusicIgnoredInFallDebris, bool MusicIgnoredInWinter, bool MusicIsTownTheme, Dictionary<string,string> CustomFields);