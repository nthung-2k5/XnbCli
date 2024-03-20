namespace Xnb.Types;

public record ExplicitType<T>
{
    public virtual T Data { get; init; }
}
