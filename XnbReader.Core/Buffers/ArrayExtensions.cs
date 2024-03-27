// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET6_0_OR_GREATER
#endif
#if NETSTANDARD
using CommunityToolkit.HighPerformance.Helpers;
#endif
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    namespace XnbReader.Buffers;

/// <summary>
/// Helpers for working with the <see cref="Array"/> type.
/// </summary>
public static partial class ArrayExtensions
{
    /// <summary>
    /// Returns a reference to the first element within a given <typeparamref name="T"/> array, with no bounds checks.
    /// </summary>
    /// <typeparam name="T">The type of elements in the input <typeparamref name="T"/> array instance.</typeparam>
    /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
    /// <returns>A reference to the first element within <paramref name="array"/>, or the location it would have used, if <paramref name="array"/> is empty.</returns>
    /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to perform checks in case the returned value is dereferenced.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T DangerousGetReference<T>(this T[] array)
    {
#if NET6_0_OR_GREATER
        return ref MemoryMarshal.GetArrayDataReference(array);
#else
        IntPtr offset = RuntimeHelpers.GetArrayDataByteOffset<T>();

        return ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
#endif
    }

    /// <summary>
    /// Returns a reference to an element at a specified index within a given <typeparamref name="T"/> array, with no bounds checks.
    /// </summary>
    /// <typeparam name="T">The type of elements in the input <typeparamref name="T"/> array instance.</typeparam>
    /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
    /// <param name="i">The index of the element to retrieve within <paramref name="array"/>.</param>
    /// <returns>A reference to the element within <paramref name="array"/> at the index specified by <paramref name="i"/>.</returns>
    /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to ensure the <paramref name="i"/> parameter is valid.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T DangerousGetReferenceAt<T>(this T[] array, int i)
    {
#if NET6_0_OR_GREATER
        ref var r0 = ref MemoryMarshal.GetArrayDataReference(array);
        ref var ri = ref Unsafe.Add(ref r0, (nint)(uint)i);

        return ref ri;
#else
        IntPtr offset = RuntimeHelpers.GetArrayDataByteOffset<T>();
        ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
        ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)i);

        return ref ri;
#endif
    }
}