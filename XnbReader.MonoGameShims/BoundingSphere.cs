using System.Numerics;

namespace XnbReader.MonoGameShims;

/// <summary>
/// Describes a sphere in 3D-space for bounding operations.
/// </summary>
public record struct BoundingSphere(Vector3 Center, float Radius);
