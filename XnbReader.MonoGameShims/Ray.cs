using System.Numerics;

namespace XnbReader.MonoGameShims;

/// <summary>
/// Represents a ray with an origin and a direction in 3D space.
/// </summary>
public record struct Ray(Vector3 Position, Vector3 Direction);
