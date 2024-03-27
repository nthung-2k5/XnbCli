// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;

namespace XnbReader.Generator.Model;

/// <summary>
/// An equatable value representing type identity.
/// </summary>
[DebuggerDisplay("Name = {Name}")]
public sealed class TypeRef(ITypeSymbol type, bool discardReaderType = false) : IEquatable<TypeRef>
{
    public string Name { get; } = type.Name;

    /// <summary>
    /// Fully qualified assembly name, prefixed with "global::", e.g. global::System.Numerics.BigInteger.
    /// </summary>
    public string FullyQualifiedName { get; } = type.GetFullyQualifiedName();

    public string TypeInfoName { get; } = GetTypeInfoPropertyName(type);

    public bool IsValueType { get; } = type.IsValueType;

    public bool IsDiscardReaderType { get; } = discardReaderType;
    
    public bool IsUnmanagedType { get; } = type.IsUnmanagedType;

    public bool Equals(TypeRef? other) => other != null && FullyQualifiedName == other.FullyQualifiedName;
    public override bool Equals(object? obj) => Equals(obj as TypeRef);
    public override int GetHashCode() => FullyQualifiedName.GetHashCode();

    private static string GetTypeInfoPropertyName(ISymbol type)
    {
        if (type is IArrayTypeSymbol arrayType)
        {
            int rank = arrayType.Rank;
            string suffix = rank == 1 ? "Array" : $"Array{rank}D"; // Array, Array2D, Array3D, ...
            return GetTypeInfoPropertyName(arrayType.ElementType) + suffix;
        }

        if (type is not INamedTypeSymbol { IsGenericType: true } namedType)
        {
            return type.Name;
        }

        string name = namedType.Name;
        StringBuilder sb = new(name);

        if (namedType.GetAllTypeArgumentsInScope() is { } typeArgsInScope)
        {
            foreach (var genericArg in typeArgsInScope)
            {
                sb.Append('_').Append(GetTypeInfoPropertyName(genericArg));
            }
        }

        return sb.ToString();
    }
}