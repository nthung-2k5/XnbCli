﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace XnbReader.Generator.Model;

/// <summary>
/// Models a constructor parameter for a generated type.
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
public sealed record ParameterGenerationSpec
{
    public required TypeRef ParameterType { get; init; }
    public required string Name { get; init; }
    public required bool? HasHeader { get; init; }
}