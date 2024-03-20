// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XnbReader.Generator.Helpers;

internal static class RoslynExtensions
{
    public static LanguageVersion? GetLanguageVersion(this Compilation compilation)
        => compilation is CSharpCompilation csc ? csc.LanguageVersion : null;

    public static Location? GetLocation(this ISymbol typeSymbol)
        => typeSymbol.Locations.Length > 0 ? typeSymbol.Locations[0] : null;

    public static Location? GetLocation(this AttributeData attributeData)
    {
        var reference = attributeData.ApplicationSyntaxReference;
        return reference?.SyntaxTree.GetLocation(reference.Span);
    }

    /// <summary>
    /// Removes any type metadata that is erased at compile time, such as NRT annotations and tuple labels.
    /// </summary>
    public static ITypeSymbol EraseCompileTimeMetadata(this Compilation compilation, ITypeSymbol type)
    {
        if (type.NullableAnnotation is NullableAnnotation.Annotated)
        {
            type = type.WithNullableAnnotation(NullableAnnotation.None);
        }

        if (type is INamedTypeSymbol namedType)
        {
            if (namedType.IsTupleType)
            {
                if (namedType.TupleElements.Length < 2)
                {
                    return type;
                }

                var erasedElements = namedType.TupleElements
                                              .Select(e => compilation.EraseCompileTimeMetadata(e.Type))
                                              .ToImmutableArray();

                type = compilation.CreateTupleTypeSymbol(erasedElements);
            }
            else if (namedType.IsGenericType)
            {
                if (namedType.IsUnboundGenericType)
                {
                    return namedType;
                }

                var typeArguments = namedType.TypeArguments;
                var containingType = namedType.ContainingType;

                if (containingType?.IsGenericType == true)
                {
                    containingType = (INamedTypeSymbol)compilation.EraseCompileTimeMetadata(containingType);
                    var tempType = namedType;
                    type = namedType = containingType.GetTypeMembers().First(t => t.Name == tempType.Name && t.Arity == tempType.Arity);
                }

                if (typeArguments.Length > 0)
                {
                    var erasedTypeArgs = typeArguments
                                         .Select(compilation.EraseCompileTimeMetadata)
                                         .ToArray();

                    type = namedType.ConstructedFrom.Construct(erasedTypeArgs);
                }
            }
        }

        return type;
    }

    public static IEnumerable<IMethodSymbol> GetExplicitlyDeclaredInstanceConstructors(this INamedTypeSymbol type)
        => type.Constructors.Where(ctor => !ctor.IsStatic && !(ctor.IsImplicitlyDeclared && type.IsValueType && ctor.Parameters.Length == 0));

    public static bool ContainsAttribute(this ISymbol memberInfo, INamedTypeSymbol? attributeType)
        => attributeType != null && memberInfo.GetAttributes().Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeType));

    public static bool IsAssignableFrom(this ITypeSymbol? baseType, ITypeSymbol? type)
    {
        if (baseType is null || type is null)
        {
            return false;
        }

        if (baseType.TypeKind is TypeKind.Interface)
        {
            if (type.AllInterfaces.Concat(type.AllInterfaces.Select(t => t.ConstructedFrom)).Contains(baseType, SymbolEqualityComparer.Default))
            {
                return true;
            }
        }

        for (var current = type as INamedTypeSymbol; current != null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(baseType, current))
            {
                return true;
            }
        }

        return false;
    }

    public static INamedTypeSymbol? GetCompatibleGenericBaseType(this ITypeSymbol type, INamedTypeSymbol? baseType)
    {
        if (baseType is null)
        {
            return null;
        }

        Debug.Assert(baseType.IsGenericTypeDefinition());

        if (baseType.TypeKind is TypeKind.Interface)
        {
            foreach (var interfaceType in type.AllInterfaces)
            {
                if (IsMatchingGenericType(interfaceType, baseType))
                {
                    return interfaceType;
                }
            }
        }

        for (var current = type as INamedTypeSymbol; current != null; current = current.BaseType)
        {
            if (IsMatchingGenericType(current, baseType))
            {
                return current;
            }
        }

        return null;

        static bool IsMatchingGenericType(INamedTypeSymbol candidate, INamedTypeSymbol baseType)
        {
            return candidate.IsGenericType && SymbolEqualityComparer.Default.Equals(candidate.ConstructedFrom, baseType);
        }
    }

    public static bool IsGenericTypeDefinition(this ITypeSymbol type) => type is INamedTypeSymbol { IsGenericType: true } namedType && SymbolEqualityComparer.Default.Equals(namedType, namedType.ConstructedFrom);

    public static bool IsNullableValueType(this ITypeSymbol type, [NotNullWhen(true)] out ITypeSymbol? elementType)
    {
        if (type.IsValueType && type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } symbol)
        {
            elementType = symbol.TypeArguments[0];
            return true;
        }

        elementType = null;
        return false;
    }

    public static ITypeSymbol GetMemberType(this ISymbol member)
    {
        Debug.Assert(member is IFieldSymbol or IPropertySymbol);
        return member is IFieldSymbol fs ? fs.Type : ((IPropertySymbol)member).Type;
    }

    public static IEnumerable<INamedTypeSymbol> GetSortedTypeHierarchy(this ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
        {
            return Array.Empty<INamedTypeSymbol>();
        }

        if (type.TypeKind != TypeKind.Interface)
        {
            var list = new List<INamedTypeSymbol>();
            for (var current = namedType; current != null; current = current.BaseType)
            {
                list.Add(current);
            }

            return list.ToArray();
        }

        // Interface hierarchies support multiple inheritance.
        // For consistency with class hierarchy resolution order,
        // sort topologically from most derived to least derived.
        return TraverseGraphWithTopologicalSort<INamedTypeSymbol>(namedType, static t => t.AllInterfaces, SymbolEqualityComparer.Default);
    }

    /// <summary>
    /// Returns the kind keyword corresponding to the specified declaration syntax node.
    /// </summary>
    public static string GetTypeKindKeyword(this TypeDeclarationSyntax typeDeclaration)
    {
        switch (typeDeclaration.Kind())
        {
            case SyntaxKind.ClassDeclaration:
                return "class";
            case SyntaxKind.InterfaceDeclaration:
                return "interface";
            case SyntaxKind.StructDeclaration:
                return "struct";
            case SyntaxKind.RecordDeclaration:
                return "record";
            case SyntaxKind.RecordStructDeclaration:
                return "record struct";
            case SyntaxKind.EnumDeclaration:
                return "enum";
            case SyntaxKind.DelegateDeclaration:
                return "delegate";
            default:
                Debug.Fail("unexpected syntax kind");
                return null;
        }
    }
    
    /// <summary>
    /// Traverses a DAG and returns its nodes applying topological sorting to the result.
    /// </summary>
    public static T[] TraverseGraphWithTopologicalSort<T>(T entryNode, Func<T, ICollection<T>> getChildren, IEqualityComparer<T>? comparer = null) where T : notnull
    {
        comparer ??= EqualityComparer<T>.Default;

        // Implements Kahn's algorithm.
        // Step 1. Traverse and build the graph, labeling each node with an integer.

        var nodes = new List<T> { entryNode }; // the integer-to-node mapping
        var nodeIndex = new Dictionary<T, int>(comparer) { [entryNode] = 0 }; // the node-to-integer mapping
        var adjacency = new List<bool[]?>(); // the growable adjacency matrix
        var childlessQueue = new Queue<int>(); // the queue of nodes without children or whose children have been visited

        for (int i = 0; i < nodes.Count; i++)
        {
            var next = nodes[i];
            var children = getChildren(next);
            int count = children.Count;

            if (count == 0)
            {
                adjacency.Add(null); // can use null in this row of the adjacency matrix.
                childlessQueue.Enqueue(i);
                continue;
            }

            var adjacencyRow = new bool[Math.Max(nodes.Count, count)];
            foreach (var childNode in children)
            {
                if (!nodeIndex.TryGetValue(childNode, out int index))
                {
                    // this is the first time we're encountering this node.
                    // Assign it an index and append it to the maps.

                    index = nodes.Count;
                    nodeIndex.Add(childNode, index);
                    nodes.Add(childNode);
                }

                // Grow the adjacency row as appropriate.
                if (index >= adjacencyRow.Length)
                {
                    Array.Resize(ref adjacencyRow, index + 1);
                }

                // Set the relevant bit in the adjacency row.
                adjacencyRow[index] = true;
            }

            // Append the row to the adjacency matrix.
            adjacency.Add(adjacencyRow);
        }

        Debug.Assert(childlessQueue.Count > 0, "The graph contains cycles.");

        // Step 2. Build the sorted array, walking from the nodes without children upward.
        var sortedNodes = new T[nodes.Count];
        int idx = sortedNodes.Length;

        do
        {
            int nextIndex = childlessQueue.Dequeue();
            sortedNodes[--idx] = nodes[nextIndex];

            // Iterate over the adjacency matrix, removing any occurrence of nextIndex.
            for (int i = 0; i < adjacency.Count; i++)
            {
                if (adjacency[i] is { } childMap && nextIndex < childMap.Length && childMap[nextIndex])
                {
                    childMap[nextIndex] = false;

                    if (childMap.AsSpan().IndexOf(true) == -1)
                    {
                        // nextIndex was the last child removed from i, add to queue.
                        childlessQueue.Enqueue(i);
                    }
                }
            }

        } while (childlessQueue.Count > 0);

        Debug.Assert(idx == 0, "should have populated the entire sortedNodes array.");
        return sortedNodes;
    }
}