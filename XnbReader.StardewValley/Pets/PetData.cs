namespace XnbReader.StardewValley.Pets;

public record PetData(string DisplayName, string BarkSound, string ContentSound, int RepeatContentSoundAfter, System.Drawing.Point EmoteOffset, System.Drawing.Point EventOffset, string AdoptionEventLocation, string AdoptionEventId, PetSummitPerfectionEventData SummitPerfectionEvent, int MoveSpeed, float SleepOnBedChance, float SleepNearBedChance, float SleepOnRugChance, List<PetBehavior> Behaviors, float GiftChance, List<PetGift> Gifts, List<PetBreed> Breeds, Dictionary<string,string> CustomFields);
