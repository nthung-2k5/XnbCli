// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace XnbReader.Generator.Model;

/// <summary>
/// Indicates which kind of constructor an object is to be created with.
/// </summary>
public enum ObjectConstructionStrategy
{
    /// <summary>
    /// Object should be created with a parameterless constructor.
    /// </summary>
    ParameterlessConstructor = 0,
    /// <summary>
    /// Object should be created with a parameterized constructor.
    /// </summary>
    ParameterizedConstructor = 1
}