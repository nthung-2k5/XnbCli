using System.Collections;
using CommunityToolkit.HighPerformance.Buffers;

namespace XnbReader.Types;
/// <summary>
/// A <see cref="MemoryOwner{T}"/> wrapper that is compatible with JsonSerializerContext source generator.
/// </summary>
/// <param name="owner"></param>
/// <typeparam name="T"></typeparam>
public sealed class EnumerableMemoryOwner<T>(MemoryOwner<T> owner) : IEnumerable<T>, IDisposable
{
    public void Dispose()
    {
        owner.Dispose();
    }

    public IEnumerator<T> GetEnumerator() => owner.DangerousGetArray().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator EnumerableMemoryOwner<T>(MemoryOwner<T> owner) => new(owner);
}
