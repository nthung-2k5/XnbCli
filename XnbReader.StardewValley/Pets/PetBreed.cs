namespace XnbReader.StardewValley.Pets;

public record PetBreed(string Id, string Texture, string IconTexture, System.Drawing.Rectangle IconSourceRect, bool CanBeChosenAtStart, bool CanBeAdoptedFromMarnie, int AdoptionPrice, string BarkOverride, float VoicePitch);
