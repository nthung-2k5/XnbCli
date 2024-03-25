// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Text;
using XnbReader.Generator.Immutable;

namespace XnbReader.Generator.Model;

/// <summary>
/// Models a generated type.
/// </summary>
/// <remarks>
/// Type needs to be cacheable as a Roslyn incremental value so it must be
///
/// 1) immutable and
/// 2) implement structural (pointwise) equality comparison.
///
/// We can get these properties for free provided that we
///
/// a) define the type as an immutable C# record and
/// b) ensure all nested members are also immutable and implement structural equality.
///
/// When adding new members to the type, please ensure that these properties
/// are satisfied otherwise we risk breaking incremental caching in the source generator!
/// </remarks>
[DebuggerDisplay("Type = {TypeRef.Name}, ClassType = {ClassType}")]
public sealed record TypeGenerationSpec
{
    /// <summary>
    /// The type being generated.
    /// </summary>
    public required TypeRef TypeRef { get; init; }

    public required string? TypeFormat { get; init; }

    public required ClassType ClassType { get; init; }

    public required bool ImplementsICustomReader { get; init; }
        
    /// <summary>
    /// List of all properties without conflict resolution or sorting to be generated for the metadata-based serializer.
    /// </summary>
    public required ImmutableEquatableArray<PropertyGenerationSpec> PropertyGenSpecs { get; init; }

    public required ImmutableEquatableArray<ParameterGenerationSpec> CtorParamGenSpecs { get; init; }

    public required CollectionType CollectionType { get; init; }

    public required TypeRef? CollectionKeyType { get; init; }

    public required TypeRef? CollectionValueType { get; init; }

    public required ObjectConstructionStrategy ConstructionStrategy { get; init; }

    public required TypeRef? UnderlyingType { get; init; }

    public override string ToString()
    {
        if (TypeFormat is null)
        {
            return TypeRef.FullName;
        }

        var sb = new StringBuilder(FormatReader(TypeRef, TypeFormat));

        if (CollectionKeyType is not null)
        {
            sb.Replace("{Key}", CollectionKeyType.FullName);
        }
            
        if (CollectionValueType is not null)
        {
            sb.Replace("{Value}", CollectionValueType.FullName);
        }

        return sb.ToString();
    }

    private static string FormatReader(TypeRef type, string format)
    {
        if (!string.IsNullOrEmpty(format))
        {
            return new StringBuilder(format).Replace("{Name}", type.Name)
                                            .Replace("{FullName}", type.FullName)
                                            .ToString();
        }

        return type.FullName;
    }
}