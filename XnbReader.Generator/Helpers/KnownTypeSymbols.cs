// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace XnbReader.Generator.Helpers;

internal sealed class KnownTypeSymbols(Compilation compilation)
{
    public Compilation Compilation { get; } = compilation;

    // Caches a set of types that will not discard reader despite being a reference type. Populated by the Parser class.
    public HashSet<ITypeSymbol>? DiscardReaderTypes { get; set; }
        
    public INamedTypeSymbol? InterfaceEnumerableType => interfaceEnumerableType ??= Compilation.GetSpecialType(SpecialType.System_Collections_IEnumerable);
    private INamedTypeSymbol? interfaceEnumerableType;

    public INamedTypeSymbol? ListOfTType => GetOrResolveType(typeof(List<>), ref listOfTType);
    private Option<INamedTypeSymbol?> listOfTType;

    public INamedTypeSymbol? DictionaryOfTKeyTValueType => GetOrResolveType(typeof(Dictionary<,>), ref dictionaryOfTKeyTValueType);
    private Option<INamedTypeSymbol?> dictionaryOfTKeyTValueType;

    public INamedTypeSymbol ObjectType => objectType ??= Compilation.GetSpecialType(SpecialType.System_Object);
    private INamedTypeSymbol? objectType;

    public INamedTypeSymbol Int32Type => int32Type ??= Compilation.GetSpecialType(SpecialType.System_Int32);
    private INamedTypeSymbol? int32Type;
    public INamedTypeSymbol StringType => stringType ??= Compilation.GetSpecialType(SpecialType.System_String);
    private INamedTypeSymbol? stringType;
        
    public INamedTypeSymbol ArrayType => arrayType ??= Compilation.GetSpecialType(SpecialType.System_Array);
    private INamedTypeSymbol? arrayType;

    public INamedTypeSymbol? XnbReadableAttributeType => GetOrResolveType(ConstStrings.FullXnbReadableAttribute, ref xnbReadableAttributeType);
    private Option<INamedTypeSymbol?> xnbReadableAttributeType;

    public INamedTypeSymbol? MemoryOwnerType => GetOrResolveType("CommunityToolkit.HighPerformance.Buffers.MemoryOwner`1", ref memoryOwnerType);
    private Option<INamedTypeSymbol?> memoryOwnerType;
        
    public INamedTypeSymbol? InterfaceCustomReaderType => GetOrResolveType(ConstStrings.FullInterfaceCustomReader, ref interfaceCustomReaderType);
    private Option<INamedTypeSymbol?> interfaceCustomReaderType;
        
    public INamedTypeSymbol? ReaderConstructorAttributeType => GetOrResolveType(ConstStrings.FullReaderConstructorAttribute, ref readerConstructorAttributeType);
    private Option<INamedTypeSymbol?> readerConstructorAttributeType;

    public INamedTypeSymbol? XnbContentReaderType => GetOrResolveType(ConstStrings.FullXnbContentReader, ref xnbContentReaderType);
    private Option<INamedTypeSymbol?> xnbContentReaderType;

    private INamedTypeSymbol? GetOrResolveType(Type type, ref Option<INamedTypeSymbol?> field) => GetOrResolveType(type.FullName!, ref field);

    private INamedTypeSymbol? GetOrResolveType(string fullyQualifiedName, ref Option<INamedTypeSymbol?> field)
    {
        if (field.HasValue)
        {
            return field.Value;
        }

        var type = Compilation.GetTypeByMetadataName(fullyQualifiedName);
        field = new Option<INamedTypeSymbol>(type);
        return type;
    }

    private readonly struct Option<T>(T value)
    {
        public readonly bool HasValue = true;
        public readonly T Value = value;
    }
}