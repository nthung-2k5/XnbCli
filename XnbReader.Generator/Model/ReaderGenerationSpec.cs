// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using XnbReader.Generator.Immutable;

namespace XnbReader.Generator.Model;

/// <summary>
/// Represents the set of input types and options needed to provide an
/// implementation for a user-provided type with XnbReadable attribute.
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
[DebuggerDisplay("ReaderType = {ReaderType.Name}")]
public sealed record ReaderGenerationSpec
{
    public required TypeRef ReaderType { get; init; }

    public required ImmutableEquatableArray<TypeGenerationSpec> GeneratedTypes { get; init; }
        
    public required ImmutableEquatableArray<RootTypeGenerationSpec> RootLevelTypes { get; init; }

    public required string? Namespace { get; init; }

    public required ImmutableEquatableArray<string> ReaderClassDeclarations { get; init; }
}