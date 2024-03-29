﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace XnbReader.Generator.Model;

public enum CollectionType
{
    Unsupported,
    // Dictionary types
    Dictionary,
    // Non-dictionary types
    MultiArray,
    Array,
    List,
    MemoryOwnerOfT
}