// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace XnbReader.Buffers;

/// <summary>
/// A debug proxy used to display items in a 1D layout.
/// </summary>
/// <typeparam name="T">The type of items to display.</typeparam>
internal sealed class MemoryDebugView<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDebugView{T}"/> class with the specified parameters.
    /// </summary>
    /// <param name="memoryOwner">The input <see cref="MemoryOwner{T}"/> instance with the items to display.</param>
    public MemoryDebugView(MemoryOwner<T>? memoryOwner)
    {
        Items = memoryOwner?.Span.ToArray();
    }

    /// <summary>
    /// Gets the items to display for the current instance
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public T[]? Items { get; }
}