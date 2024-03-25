namespace XnbReader.StardewValley.Weapons;

public record WeaponProjectile(string Id, int Damage, bool Explodes, int Bounces, int MaxDistance, int Velocity, int RotationVelocity, int TailLength, string FireSound, string BounceSound, string CollisionSound, float MinAngleOffset, float MaxAngleOffset, int SpriteIndex, GenericSpawnItemData Item);
