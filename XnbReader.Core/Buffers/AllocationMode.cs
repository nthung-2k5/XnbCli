namespace XnbReader.Buffers;

/// <summary>
/// An <see langword="enum"/> that indicates a mode to use when allocating buffers.
/// </summary>
public enum AllocationMode
{
    /// <summary>
    /// The default allocation mode for pooled memory (rented buffers are not cleared).
    /// </summary>
    Default,

    /// <summary>
    /// Clear pooled buffers when renting them.
    /// </summary>
    Clear
}
