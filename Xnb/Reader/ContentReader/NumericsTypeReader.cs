using System.Drawing;
using System.Numerics;

namespace Xnb.Reader.ContentReader;

public class RectangleReader: UnmanagedReader<Rectangle>;
public class Vector2Reader: UnmanagedReader<Vector2>;
public class Vector3Reader: UnmanagedReader<Vector3>;
public class Vector4Reader: UnmanagedReader<Vector4>;
public static partial class ReaderResolver
{
    static partial void RegisterNumericsReaders()
    {
        Register(new RectangleReader());
        Register(new Vector2Reader());
        Register(new Vector3Reader());
        Register(new Vector4Reader());
    }
}