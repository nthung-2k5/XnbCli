namespace XnbReader.StardewValley.Weapons;

public record WeaponData(string Name, string DisplayName, string Description, int MinDamage, int MaxDamage, float Knockback, int Speed, int Precision, int Defense, int Type, int MineBaseLevel, int MineMinLevel, int AreaOfEffect, float CritChance, float CritMultiplier, bool CanBeLostOnDeath, string Texture, int SpriteIndex, List<WeaponProjectile> Projectiles, Dictionary<string,string> CustomFields);
