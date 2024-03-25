namespace XnbReader.StardewValley.Pets;

public record PetAnimationFrame(int Frame, int Duration, bool HitGround, bool Jump, string Sound, int SoundRangeFromBorder, int SoundRange, bool SoundIsVoice);
