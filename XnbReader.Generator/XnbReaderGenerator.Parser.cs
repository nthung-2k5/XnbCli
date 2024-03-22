// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using XnbReader.Generator.Helpers;
using XnbReader.Generator.Immutable;
using XnbReader.Generator.Model;

namespace XnbReader.Generator;

public sealed partial class XnbReaderGenerator
{
    // The source generator requires NRT and init-only property support.
    private const LanguageVersion MinimumSupportedLanguageVersion = LanguageVersion.CSharp9;

    private sealed partial class Parser(KnownTypeSymbols knownSymbols)
    {
        
        private readonly HashSet<ITypeSymbol> discardReaderTypes = knownSymbols.DiscardReaderTypes ??= CreateDiscardReaderTypeSet(knownSymbols);
        private readonly Queue<TypeToGenerate> typesToGenerate = new();
        private readonly Dictionary<ITypeSymbol, TypeGenerationSpec> generatedTypes = new(SymbolEqualityComparer.Default);
        
        public List<Diagnostic> Diagnostics { get; } = [];

        public ReaderGenerationSpec? ParseReaderGenerationSpec(TypeDeclarationSyntax readerClassDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            // Ensure reader-scoped metadata caches are empty.
            Debug.Assert(typesToGenerate.Count == 0);
            Debug.Assert(generatedTypes.Count == 0);

            var readerTypeSymbol = semanticModel.GetDeclaredSymbol(readerClassDeclaration, cancellationToken)!;
            
            Debug.Assert(readerTypeSymbol != null);
            Debug.Assert(readerTypeSymbol.GetLocation() is not null);

            ParseClassReaderAttributes(readerTypeSymbol, out var rootTypes);
            
            if (rootTypes is null)
            {
                // No types were annotated with ClassReaderAttribute.
                // Can only be reached if a [ClassReader(null)] declaration has been made.
                // Do not emit a diagnostic since a NRT warning will also be emitted.
                return null;
            }

            Debug.Assert(rootTypes.Count > 0);

            var langVersion = knownSymbols.Compilation.GetLanguageVersion();
            if (langVersion is null or < MinimumSupportedLanguageVersion)
            {
                // Unsupported lang version should be the first (and only) diagnostic emitted by the generator.
                return null;
            }

            if (!knownSymbols.XnbContentReaderType.IsAssignableFrom(readerTypeSymbol))
            {
                ReportDiagnostic(DiagnosticDescriptors.MustInheritXnbContentReader, readerTypeSymbol);
                return null;
            }

            if (!TryGetNestedTypeDeclarations(readerClassDeclaration, semanticModel, cancellationToken, out var classDeclarationList))
            {
                // Class or one of its containing types is not partial so we can't add to it.
                ReportDiagnostic(DiagnosticDescriptors.MustBePartial, readerClassDeclaration.Identifier.GetLocation(), readerTypeSymbol.Name);
                return null;
            }

            // Enqueue attribute data for spec generation
            foreach (var rootType in rootTypes)
            {
                if (string.IsNullOrEmpty(rootType.ReaderFormat))
                {
                    if (TryGetListTypeSymbol(rootType, out var list))
                    {
                        typesToGenerate.Enqueue(list.Value);
                    }
                
                    if (TryGetDictionaryTypeSymbol(knownSymbols.Int32Type, rootType, out var dictInt) &&
                        TryGetDictionaryTypeSymbol(knownSymbols.StringType, rootType, out var dictStr))
                    {
                        typesToGenerate.Enqueue(dictInt.Value);
                        typesToGenerate.Enqueue(dictStr.Value);
                    }
                }
                else
                {
                    typesToGenerate.Enqueue(rootType);
                }
            }

            var rootTypesToGenerate = typesToGenerate.Select(type => type.Type).ToArray();
            
            // Walk the transitive type graph generating specs for every encountered type.
            while (typesToGenerate.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var typeToGenerate = typesToGenerate.Dequeue();
                if (!generatedTypes.ContainsKey(typeToGenerate.Type))
                {
                    var spec = ParseTypeGenerationSpec(typeToGenerate);
                    generatedTypes.Add(typeToGenerate.Type, spec);
                }
            }
            
            var rootLevelTypes = rootTypesToGenerate.Select(type => generatedTypes[type]).ToImmutableEquatableArray();
            
            Debug.Assert(generatedTypes.Count > 0);

            ReaderGenerationSpec readerGenSpec = new()
            {
                ReaderType = new TypeRef(readerTypeSymbol),
                GeneratedTypes = generatedTypes.Values.OrderBy(t => t.TypeRef.FullyQualifiedName).ToImmutableEquatableArray(),
                RootLevelTypes = rootLevelTypes,
                Namespace = readerTypeSymbol.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : null,
                ReaderClassDeclarations = classDeclarationList.ToImmutableEquatableArray()
            };

            // Clear the caches of generated metadata between the processing of reader classes.
            generatedTypes.Clear();
            typesToGenerate.Clear();
            return readerGenSpec;
        }

        private static bool TryGetNestedTypeDeclarations(TypeDeclarationSyntax readerClassSyntax, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out List<string>? typeDeclarations)
        {
            typeDeclarations = null;

            for (var currentType = readerClassSyntax; currentType != null; currentType = currentType.Parent as TypeDeclarationSyntax)
            {
                StringBuilder stringBuilder = new();
                bool isPartialType = false;

                foreach (var modifier in currentType.Modifiers)
                {
                    stringBuilder.Append(modifier.Text).Append(' ');
                    isPartialType |= modifier.IsKind(SyntaxKind.PartialKeyword);
                }

                if (!isPartialType)
                {
                    typeDeclarations = null;
                    return false;
                }

                stringBuilder.Append(currentType.GetTypeKindKeyword()).Append(' ');

                var typeSymbol = semanticModel.GetDeclaredSymbol(currentType, cancellationToken);
                Debug.Assert(typeSymbol != null);

                string typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                stringBuilder.Append(typeName);

                (typeDeclarations ??= []).Add(stringBuilder.ToString());
            }

            Debug.Assert(typeDeclarations?.Count > 0);
            return true;
        }

        private TypeRef EnqueueType(ITypeSymbol type, bool discardReaderType = false)
        {
            // Trim compile-time erased metadata such as tuple labels and NRT annotations.
            type = knownSymbols.Compilation.EraseCompileTimeMetadata(type);

            if (generatedTypes.TryGetValue(type, out var spec))
            {
                return spec.TypeRef;
            }

            typesToGenerate.Enqueue(new TypeToGenerate(type));

            return new TypeRef(type, discardReaderType);
        }

        private TypeGenerationSpec? ParseTypeGenerationSpec(in TypeToGenerate typeToGenerate)
        {
            (var type, string? format, _) = typeToGenerate;
            
            if (type is INamedTypeSymbol { IsUnboundGenericType: true } or IErrorTypeSymbol)
            {
                return null;
            }

            if (IsUnsupportedType(type))
            {
                return null;
            }

            ClassType classType;
            TypeRef? collectionKeyType = null;
            TypeRef? collectionValueType = null;
            string keyTypeFormat = null;
            string valueTypeFormat = null;
            TypeRef? underlyingType = null;
            List<PropertyGenerationSpec>? propertySpecs = null;
            ObjectConstructionStrategy constructionStrategy = default;
            ParameterGenerationSpec[]? ctorParamSpecs = null;
            var collectionType = CollectionType.Unsupported;
            bool implementsICustomReader = knownSymbols.InterfaceCustomReaderType.IsAssignableFrom(type);
            
            if (IsBuiltInSupportType(type))
            {
                classType = ClassType.BuiltInSupportType;
            }
            else if (type.IsNullableValueType(out var underlying))
            {
                classType = ClassType.Nullable;
                underlyingType = EnqueueType(underlying);
            }
            else if (type.TypeKind is TypeKind.Enum)
            {
                classType = ClassType.Enum;
                underlyingType = EnqueueType((type as INamedTypeSymbol).EnumUnderlyingType);
            }
            else if (TryResolveCollectionType(type, out var valueType, out var keyType, out collectionType, ref format))
            {
                if (collectionType == CollectionType.Unsupported)
                {
                    ReportDiagnostic(DiagnosticDescriptors.UnsupportedCollection, type);
                }
                
                classType = keyType is not null ? ClassType.Dictionary : ClassType.Enumerable;
                collectionValueType = EnqueueType(valueType.Value.Type);
                valueTypeFormat = valueType.Value.Format;

                if (keyType.HasValue)
                {
                    collectionKeyType = EnqueueType(keyType.Value.Type);
                    keyTypeFormat = keyType.Value.Format;
                }
            }
            else if (type is INamedTypeSymbol { IsUnmanagedType: true } unmanagedType && !ContainsVariableSizedMember(unmanagedType))
            {
                classType = ClassType.Unmanaged;

                if (unmanagedType.GetExplicitlyDeclaredInstanceConstructors().Any(ctor => ctor.ContainsAttribute(knownSymbols.ReaderConstructorAttributeType)))
                {
                    ReportDiagnostic(DiagnosticDescriptors.UnmanagedStructConstructor, type);
                }

                if (implementsICustomReader)
                {
                    ReportDiagnostic(DiagnosticDescriptors.InterfaceCustomReaderInUnmanagedType, type);
                }
            }
            else
            {
                if (!ValidateConstructor(type, out var constructor))
                {
                    return null;
                }

                constructionStrategy = constructor.Value.Strategy;
                classType = ClassType.Object;
                ctorParamSpecs = ParseConstructorParameters(typeToGenerate, constructor.Value);

                if (constructionStrategy == ObjectConstructionStrategy.ParameterlessConstructor)
                {
                    propertySpecs = ParsePropertyGenerationSpecs(typeToGenerate);
                }
            }

            return new TypeGenerationSpec
            {
                TypeRef = new TypeRef(type),
                ClassType = classType,
                PropertyGenSpecs = propertySpecs?.ToImmutableEquatableArray() ?? ImmutableEquatableArray<PropertyGenerationSpec>.Empty,
                CtorParamGenSpecs = ctorParamSpecs?.ToImmutableEquatableArray() ?? ImmutableEquatableArray<ParameterGenerationSpec>.Empty,
                CollectionType = collectionType,
                CollectionKeyType = collectionKeyType,
                CollectionValueType = collectionValueType,
                ConstructionStrategy = constructionStrategy,
                UnderlyingType = underlyingType,
                ImplementsICustomReader = implementsICustomReader,
                TypeFormat = typeToGenerate.ReaderFormat ?? format,
                KeyTypeFormat = keyTypeFormat,
                ValueTypeFormat = valueTypeFormat
            };
        }

        private bool TryResolveCollectionType(
            ITypeSymbol type,
            [NotNullWhen(true)] out TypeToGenerate? valueType,
            out TypeToGenerate? keyType,
            out CollectionType collectionType,
            ref string typeFormat)
        {
            INamedTypeSymbol? actualTypeToConvert;
            valueType = null;
            keyType = null;
            collectionType = CollectionType.Unsupported;
            
            if (SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, knownSymbols.MemoryOwnerType))
            {
                valueType = new TypeToGenerate(((INamedTypeSymbol)type).TypeArguments[0]);
                collectionType = CollectionType.MemoryOwnerOfT;
                return true;
            }

            if (!knownSymbols.InterfaceEnumerableType.IsAssignableFrom(type))
            {
                // Type is not IEnumerable and therefore not a collection type
                return false;
            }

            if (type is IArrayTypeSymbol arraySymbol)
            {
                //Debug.Assert(arraySymbol.Rank == 1, "multi-dimensional arrays should have been handled earlier.");
                collectionType = arraySymbol.Rank == 1 ? CollectionType.Array : CollectionType.MultiArray;
                valueType = new TypeToGenerate(arraySymbol.ElementType, typeFormat);
                typeFormat = $"{ConstStrings.XnaFrameworkContentNamespace}.{collectionType.ToString()}Reader`1[{{Value}}]";
            }
            else if ((actualTypeToConvert = type.GetCompatibleGenericBaseType(knownSymbols.ListOfTType)) != null)
            {
                collectionType = CollectionType.List;
                valueType = new TypeToGenerate(actualTypeToConvert.TypeArguments[0], typeFormat);
                typeFormat = $"{ConstStrings.XnaFrameworkContentNamespace}.ListReader`1[{{Value}}]";
            }
            else if ((actualTypeToConvert = type.GetCompatibleGenericBaseType(knownSymbols.DictionaryOfTKeyTValueType)) != null)
            {
                collectionType = CollectionType.Dictionary;
                keyType = new TypeToGenerate(actualTypeToConvert.TypeArguments[0]);
                valueType = new TypeToGenerate(actualTypeToConvert.TypeArguments[1], typeFormat);
                typeFormat += $"{ConstStrings.XnaFrameworkContentNamespace}.DictionaryReader`2[{{Key}},{{Value}}]";
            }
            else
            {
                collectionType = CollectionType.Unsupported;
                valueType = new TypeToGenerate(knownSymbols.ObjectType);
            }

            return true;
        }

        private bool TryGetDictionaryTypeSymbol(ITypeSymbol keyType, TypeToGenerate valueType, [NotNullWhen(true)] out TypeToGenerate? dictionary)
        {
            dictionary = knownSymbols.DictionaryOfTKeyTValueType is null ? null : valueType with { Type = knownSymbols.DictionaryOfTKeyTValueType?.Construct(keyType, valueType.Type) };
            return dictionary is not null;
        }
        
        private bool TryGetListTypeSymbol(TypeToGenerate valueType, [NotNullWhen(true)] out TypeToGenerate? list)
        {
            list = knownSymbols.ListOfTType is null ? null : valueType with { Type = knownSymbols.ListOfTType?.Construct(valueType.Type) };
            return list is not null;
        }
        
        private static bool IsUnsupportedType(ITypeSymbol type)
        {
            return false; // no type is considered unsupported yet
        }

        private static bool IsBuiltInSupportType(ITypeSymbol type)
        {
            return type.SpecialType is >= SpecialType.System_Boolean and <= SpecialType.System_String;
        }

        private static bool ContainsVariableSizedMember(ITypeSymbol type)
        {
            Debug.Assert(type.IsUnmanagedType);

            foreach (var member in type.GetMembers())
            {
                if (member is IPropertySymbol or IFieldSymbol)
                {
                    var memberType = member.GetMemberType();

                    if (IsBuiltInSupportType(memberType))
                    {
                        return memberType.SpecialType == SpecialType.System_Char; // char can be 1 or 2 bytes in data, but always 2 bytes in memory
                    }

                    if (ContainsVariableSizedMember(memberType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static HashSet<ITypeSymbol> CreateDiscardReaderTypeSet(KnownTypeSymbols knownSymbols)
        {
            HashSet<ITypeSymbol> discardReaderTypes = new(SymbolEqualityComparer.Default);
            AddTypeIfNotNull(knownSymbols.MemoryOwnerType);
            AddTypeIfNotNull(knownSymbols.ArrayType);
            return discardReaderTypes;

            void AddTypeIfNotNull(ITypeSymbol? type)
            {
                if (type != null)
                {
                    discardReaderTypes.Add(type);
                }
            }
        }

        private void ReportDiagnostic(DiagnosticDescriptor descriptor, ISymbol symbol)
        {
            ReportDiagnostic(descriptor, symbol.GetLocation(), symbol.Name);
        }
        
        private void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, string symbolName)
        {
            Diagnostics.Add(Diagnostic.Create(descriptor, location, symbolName));
        }

        private bool IsDiscardReaderType(ITypeSymbol type) => discardReaderTypes.Contains(type.BaseType) || discardReaderTypes.Contains(type.OriginalDefinition);

        public readonly record struct TypeToGenerate(ITypeSymbol Type, string? Format = null, string? ReaderFormat = null);
    }
}