using System.Text.Json.Serialization;
using CommunityToolkit.HighPerformance.Buffers;

namespace Xnb.Types;

public record Texture2D(SurfaceFormat Format, int Width, int Height): ExplicitType<byte[]>, IDisposable
{
    [JsonIgnore]
    public MemoryOwner<byte> DataOwner { get; init; }

    public override byte[] Data => DataOwner.DangerousGetArray().Array;

    public void Dispose()
    {
        DataOwner?.Dispose();
        GC.SuppressFinalize(this);
    }
}
