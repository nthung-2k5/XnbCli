using System.Numerics;

namespace XnbReader.MonoGameShims;

/// <summary>
/// Represents an axis-aligned bounding box (AABB) in 3D space.
/// </summary>
public record struct BoundingBox(Vector3 Min, Vector3 Max);
