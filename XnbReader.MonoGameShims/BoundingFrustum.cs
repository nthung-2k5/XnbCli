using System.Numerics;

namespace XnbReader.MonoGameShims;

/// <summary>
/// Defines a viewing frustum for intersection operations.
/// </summary>
public record BoundingFrustum
{
    #region Private Fields

    private readonly Matrix4x4 matrix;
    private readonly Vector3[] corners = new Vector3[CornerCount];
    private readonly Plane[] planes = new Plane[PlaneCount];

    #endregion

    #region Public Fields

    /// <summary>
    /// The number of planes in the frustum.
    /// </summary>
    private const int PlaneCount = 6;

    /// <summary>
    /// The number of corner points in the frustum.
    /// </summary>
    private const int CornerCount = 8;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the <see cref="System.Numerics.Matrix4x4"/> of the frustum.
    /// </summary>
    public Matrix4x4 Matrix => matrix;

    /// <summary>
    /// Gets the near plane of the frustum.
    /// </summary>
    public Plane Near => planes[0];

    /// <summary>
    /// Gets the far plane of the frustum.
    /// </summary>
    public Plane Far => planes[1];

    /// <summary>
    /// Gets the left plane of the frustum.
    /// </summary>
    public Plane Left => planes[2];

    /// <summary>
    /// Gets the right plane of the frustum.
    /// </summary>
    public Plane Right => planes[3];

    /// <summary>
    /// Gets the top plane of the frustum.
    /// </summary>
    public Plane Top => planes[4];

    /// <summary>
    /// Gets the bottom plane of the frustum.
    /// </summary>
    public Plane Bottom => planes[5];
#endregion

    #region Constructors

    /// <summary>
    /// Constructs the frustum by extracting the view planes from a matrix.
    /// </summary>
    /// <param name="value">Combined matrix which usually is (View * Projection).</param>
    public BoundingFrustum(Matrix4x4 value)
    {
        matrix = value;
        CreatePlanes();
        CreateCorners();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the hash code of this <see cref="BoundingFrustum"/>.
    /// </summary>
    /// <returns>Hash code of this <see cref="BoundingFrustum"/>.</returns>
    public override int GetHashCode()
    {
        return matrix.GetHashCode();
    }

    #endregion

    #region Private Methods

    private void CreateCorners()
    {
        IntersectionPoint(ref planes[0], ref planes[2], ref planes[4], out corners[0]);
        IntersectionPoint(ref planes[0], ref planes[3], ref planes[4], out corners[1]);
        IntersectionPoint(ref planes[0], ref planes[3], ref planes[5], out corners[2]);
        IntersectionPoint(ref planes[0], ref planes[2], ref planes[5], out corners[3]);
        IntersectionPoint(ref planes[1], ref planes[2], ref planes[4], out corners[4]);
        IntersectionPoint(ref planes[1], ref planes[3], ref planes[4], out corners[5]);
        IntersectionPoint(ref planes[1], ref planes[3], ref planes[5], out corners[6]);
        IntersectionPoint(ref planes[1], ref planes[2], ref planes[5], out corners[7]);
    }

    private void CreatePlanes()
    {            
        planes[0] = Plane.Normalize(new Plane(-matrix.M13, -matrix.M23, -matrix.M33, -matrix.M43));
        planes[1] = Plane.Normalize(new Plane(matrix.M13 - matrix.M14, matrix.M23 - matrix.M24, matrix.M33 - matrix.M34, matrix.M43 - matrix.M44));
        planes[2] = Plane.Normalize(new Plane(-matrix.M14 - matrix.M11, -matrix.M24 - matrix.M21, -matrix.M34 - matrix.M31, -matrix.M44 - matrix.M41));
        planes[3] = Plane.Normalize(new Plane(matrix.M11 - matrix.M14, matrix.M21 - matrix.M24, matrix.M31 - matrix.M34, matrix.M41 - matrix.M44));
        planes[4] = Plane.Normalize(new Plane(matrix.M12 - matrix.M14, matrix.M22 - matrix.M24, matrix.M32 - matrix.M34, matrix.M42 - matrix.M44));
        planes[5] = Plane.Normalize(new Plane(-matrix.M14 - matrix.M12, -matrix.M24 - matrix.M22, -matrix.M34 - matrix.M32, -matrix.M44 - matrix.M42));
    }

    private static void IntersectionPoint(ref Plane a, ref Plane b, ref Plane c, out Vector3 result)
    {
        // Formula used
        //                d1 ( N2 * N3 ) + d2 ( N3 * N1 ) + d3 ( N1 * N2 )
        //P =   -------------------------------------------------------------------------
        //                             N1 . ( N2 * N3 )
        //
        // Note: N refers to the normal, d refers to the displacement. '.' means dot product. '*' means cross product

        float f = -Vector3.Dot(a.Normal, Vector3.Cross(b.Normal, c.Normal));
        
        var v1 = a.D * Vector3.Cross(b.Normal, c.Normal);
        var v2 = b.D * Vector3.Cross(c.Normal, a.Normal);
        var v3 = c.D * Vector3.Cross(a.Normal, b.Normal);
        
        result = (v1 + v2 + v3) / f;
    }

    #endregion
}
