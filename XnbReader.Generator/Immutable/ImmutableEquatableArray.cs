// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XnbReader.Generator.Helpers;

namespace XnbReader.Generator.Immutable;

/// <summary>
/// Provides an immutable list implementation which implements sequence equality.
/// </summary>
public sealed class ImmutableEquatableArray<T>(IEnumerable<T> values) : IEquatable<ImmutableEquatableArray<T>>, IReadOnlyList<T> where T : IEquatable<T>
{
    public static ImmutableEquatableArray<T> Empty { get; } = new(Array.Empty<T>());

    private readonly T[] values = values.ToArray();
    public T this[int index] => values[index];
    public int Count => values.Length;

    public bool Equals(ImmutableEquatableArray<T>? other)
        => other != null && ((ReadOnlySpan<T>)values).SequenceEqual(other.values);

    public override bool Equals(object? obj)
        => obj is ImmutableEquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        return values.Aggregate(0, (current, value) => HashHelpers.Combine(current, value.GetHashCode()));
    }

    public Enumerator GetEnumerator() => new(values);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();

    public struct Enumerator
    {
        private readonly T[] values;
        private int index;

        internal Enumerator(T[] values)
        {
            this.values = values;
            index = -1;
        }

        public bool MoveNext()
        {
            int newIndex = index + 1;

            if ((uint)newIndex >= (uint)values.Length)
            {
                return false;
            }

            index = newIndex;
            return true;

        }

        public readonly T Current => values[index];
    }
}

internal static class ImmutableEquatableArray
{
    public static ImmutableEquatableArray<T> ToImmutableEquatableArray<T>(this IEnumerable<T> values) where T : IEquatable<T> => new(values);
}