// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace XnbReader.Generator.Model;

/// <summary>
/// Models a property for a generated type.
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
[DebuggerDisplay("Name = {MemberName}, Type = {PropertyType.Name}")]
public sealed record PropertyGenerationSpec
{
    public required string MemberName { get; init; }
    
    /// <summary>
    /// Whether the property has a header of a reader when read.
    /// </summary>
    public required bool? HasHeader { get; init; }

    /// <summary>
    /// Gets a reference to the property type.
    /// </summary>
    public required TypeRef PropertyType { get; init; }
}