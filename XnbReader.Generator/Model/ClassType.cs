// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace XnbReader.Generator.Model;

public enum ClassType
{
    Object = 0,
    BuiltInSupportType = 1,
    Enumerable = 2,
    Dictionary = 3,
    Nullable = 4,
    Enum = 5,
    Unmanaged = 6
}