namespace XnbReader.StardewValley.Characters;

public record CharacterData(string DisplayName, Season? BirthSeason, int BirthDay, string HomeRegion, NpcLanguage Language, Gender Gender, NpcAge Age, NpcManner Manner, NpcSocialAnxiety SocialAnxiety, NpcOptimism Optimism, bool IsDarkSkinned, bool CanBeRomanced, string LoveInterest, CalendarBehavior Calendar, SocialTabBehavior SocialTab, string CanSocialize, bool CanReceiveGifts, bool CanGreetNearbyCharacters, bool? CanCommentOnPurchasedShopItems, string CanVisitIsland, bool? IntroductionsQuest, string ItemDeliveryQuests, bool PerfectionScore, EndSlideShowBehavior EndSlideShow, string SpouseAdopts, string SpouseWantsChildren, string SpouseGiftJealousy, int SpouseGiftJealousyFriendshipChange, CharacterSpouseRoomData SpouseRoom, CharacterSpousePatioData SpousePatio, List<string> SpouseFloors, List<string> SpouseWallpapers, int DumpsterDiveFriendshipEffect, int? DumpsterDiveEmote, Dictionary<string,string> FriendsAndFamily, bool? FlowerDanceCanDance, List<GenericSpawnItemDataWithCondition> WinterStarGifts, string WinterStarParticipant, string UnlockConditions, bool SpawnIfMissing, List<CharacterHomeData> Home, string TextureName, List<CharacterAppearanceData> Appearance, System.Drawing.Rectangle? MugShotSourceRect, System.Drawing.Point Size, bool Breather, System.Drawing.Rectangle? BreathChestRect, System.Drawing.Point? BreathChestPosition, CharacterShadowData Shadow, System.Drawing.Point EmoteOffset, List<int> ShakePortraits, int KissSpriteIndex, bool KissSpriteFacingRight, string HiddenProfileEmoteSound, int HiddenProfileEmoteDuration, int HiddenProfileEmoteStartFrame, int HiddenProfileEmoteFrameCount, float HiddenProfileEmoteFrameDuration, List<string> FormerCharacterNames, int FestivalVanillaActorIndex, Dictionary<string,string> CustomFields);