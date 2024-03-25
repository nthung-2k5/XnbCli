namespace XnbReader.StardewValley.Machines;

public record MachineEffects(string Id, string Condition, List<MachineSoundData> Sounds, int Interval, List<int> Frames, int ShakeDuration, List<TemporaryAnimatedSpriteDefinition> TemporarySprites);
